using UnityEngine;
using System.Collections.Generic;

public class AntiGravitySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bigRockPrefab;
    public GameObject antiGravityPickupPrefab;

    [Header("Settings")]
    public float spawnChance = 0.15f;
    public float rockOffsetZ = 30f;
    public float pickupOffsetZ = 12f;
    public float unlockScore = 300f;
    public float eventCooldown = 30f;

    private bool eventActive = false;
    private float cooldownTimer = 0f;
    private ObstacleSpawner obstacleSpawner;
    private static readonly Quaternion NormalRotation = Quaternion.identity;
    private static readonly Quaternion FlippedRotation = Quaternion.Euler(180f, 0f, 0f);

    private void Start()
    {
        obstacleSpawner = GetComponent<ObstacleSpawner>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (eventActive && AntiGravityManager.Instance != null && !AntiGravityManager.Instance.EventInProgress)
            eventActive = false;
    }

    public void TrySpawnOnRoad(GameObject road)
    {
        if (ScoreManager.Instance == null) return;
        if (ScoreManager.Instance.CurrentScore < unlockScore) return;
        if (Random.value > spawnChance) return;
        if (eventActive) return;
        if (cooldownTimer > 0f) return;
        if (AntiGravityManager.Instance != null && AntiGravityManager.Instance.EventInProgress) return;

        eventActive = true;
        cooldownTimer = eventCooldown;

        float roadZ = road.transform.position.z;
        float roadY = road.transform.position.y;

        if (AntiGravityManager.Instance != null)
            AntiGravityManager.Instance.PrepareElevatedRoads();

        float pickupX = 0f;
        float rockX = 0f;

        if (obstacleSpawner != null)
        {
            pickupX = obstacleSpawner.GetFreeLaneX();
            rockX = obstacleSpawner.GetFreeLaneX();
            
            if (Mathf.Approximately(pickupX, rockX))
            {
                pickupX = -pickupX; 
                if (Mathf.Approximately(pickupX, rockX)) pickupX = 0f;
            }
        }

        if (antiGravityPickupPrefab != null)
        {
            Vector3 pickupPos = new Vector3(pickupX, roadY + 1f, roadZ + pickupOffsetZ);
            Instantiate(antiGravityPickupPrefab, pickupPos, NormalRotation);
        }

        if (bigRockPrefab != null)
        {
            Vector3 rockPos = new Vector3(rockX, roadY + 1f, roadZ + rockOffsetZ);
            Instantiate(bigRockPrefab, rockPos, NormalRotation);
        }
    }

    public void SpawnReturnPickupOnRoad(GameObject road)
    {
        float roadZ = road.transform.position.z;
        float roadY = road.transform.position.y;
        
        float pickupX = 0f;
        float rockX = 0f;

        if (obstacleSpawner != null)
        {
            pickupX = obstacleSpawner.GetFreeLaneX();
            rockX = obstacleSpawner.GetFreeLaneX();

            if (Mathf.Approximately(pickupX, rockX))
            {
                pickupX = -pickupX;
                if (Mathf.Approximately(pickupX, rockX)) pickupX = 0f;
            }
        }

        if (antiGravityPickupPrefab != null)
        {
            Vector3 returnPos = new Vector3(pickupX, roadY - 1f, roadZ + pickupOffsetZ);
            Instantiate(antiGravityPickupPrefab, returnPos, FlippedRotation);
        }

        if (bigRockPrefab != null)
        {
            Vector3 rockPos = new Vector3(rockX, roadY - 1f, roadZ + rockOffsetZ);
            Instantiate(bigRockPrefab, rockPos, FlippedRotation);
        }
    }
}