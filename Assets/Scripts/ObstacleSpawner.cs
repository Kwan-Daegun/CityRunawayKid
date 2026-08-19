using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Ground Obstacles")]
    public GameObject[] obstaclePrefabs;

    [Header("Flying Obstacles")]
    public GameObject[] flyingObstaclePrefabs;
    public float flyingSpawnChance = 0.4f;
    public float flyingHeight = 2.5f;
    public float flyingUnlockScore = 500f;

    [Header("Combo Power Up")]
    public GameObject comboPickupPrefab;
    [Range(0f, 1f)]
    public float comboSpawnChance = 0.15f;

    [Header("Spawn Settings")]
    public float spawnZ = 20f;
    public int minObstacles = 1;
    public int maxObstacles = 3;

    private float[] lanePositions = { -4.5f, -1.5f, 1.5f, 4.5f };
    private List<int> lastUsedLaneIndices = new List<int>();

    public void SpawnObstaclesOnRoad(GameObject road, bool isFlipped = false)
    {
        int count = Random.Range(minObstacles, maxObstacles + 1);
        int[] shuffledLanes = ShuffleLanes();
        lastUsedLaneIndices.Clear();

        float yOffset = isFlipped ? -0.1f : 1f;
        Quaternion spawnRotation = Quaternion.Euler(isFlipped ? 180f : 0f, 0f, 0f);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            float x = lanePositions[shuffledLanes[i]];

            Vector3 spawnPos = new Vector3(
                x,
                road.transform.position.y + yOffset,
                road.transform.position.z + spawnZ
            );

            GameObject obj = Instantiate(prefab, spawnPos, spawnRotation);
            lastUsedLaneIndices.Add(shuffledLanes[i]);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }

        bool flyingUnlocked =
            ScoreManager.Instance != null &&
            ScoreManager.Instance.CurrentScore >= flyingUnlockScore;

        if (flyingUnlocked &&
            flyingObstaclePrefabs != null &&
            flyingObstaclePrefabs.Length > 0 &&
            Random.value <= flyingSpawnChance)
        {
            GameObject flyingPrefab =
                flyingObstaclePrefabs[Random.Range(0, flyingObstaclePrefabs.Length)];

            float x = lanePositions[Random.Range(0, lanePositions.Length)];

            float flyY = isFlipped
                ? road.transform.position.y - flyingHeight
                : road.transform.position.y + flyingHeight;

            Vector3 flyPos = new Vector3(
                x,
                flyY,
                road.transform.position.z + spawnZ
            );

            GameObject flyingObj = Instantiate(flyingPrefab, flyPos, spawnRotation);

            Rigidbody rb = flyingObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }

        SpawnComboPickup(road, isFlipped);

        AntiGravitySpawner agSpawner = GetComponent<AntiGravitySpawner>();
        if (agSpawner != null)
            agSpawner.TrySpawnOnRoad(road);
    }

    private void SpawnComboPickup(GameObject road, bool isFlipped)
    {
        if (comboPickupPrefab == null) return;
        if (Random.value > comboSpawnChance) return;

        float freeLaneX = GetFreeLaneX();
        float yOffset = isFlipped ? -1f : 1f;

        Quaternion rotation = Quaternion.Euler(isFlipped ? 180f : 0f, 0f, 0f);

        Vector3 spawnPos = new Vector3(
            freeLaneX,
            road.transform.position.y + yOffset,
            road.transform.position.z + spawnZ - 5f
        );

        GameObject comboObj = Instantiate(comboPickupPrefab, spawnPos, rotation);

        Rigidbody rb = comboObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    public float GetFreeLaneX()
    {
        List<int> freeLanes = new List<int>();

        for (int i = 0; i < lanePositions.Length; i++)
        {
            if (!lastUsedLaneIndices.Contains(i))
                freeLanes.Add(i);
        }

        if (freeLanes.Count > 0)
            return lanePositions[freeLanes[Random.Range(0, freeLanes.Count)]];

        return lanePositions[Random.Range(0, lanePositions.Length)];
    }

    private int[] ShuffleLanes()
    {
        int[] lanes = { 0, 1, 2, 3 };

        for (int i = lanes.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = lanes[i];
            lanes[i] = lanes[j];
            lanes[j] = temp;
        }

        return lanes;
    }
}