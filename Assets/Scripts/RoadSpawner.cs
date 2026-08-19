using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoadSpawner : MonoBehaviour
{
    public List<GameObject> roads;
    public float offset = 40f;

    [HideInInspector] public bool isFlipped = false;
    [HideInInspector] public float elevatedY = 20f;

    private ObstacleSpawner obstacleSpawner;
    private float normalY = 0f;

    void Start()
    {
        obstacleSpawner = GetComponent<ObstacleSpawner>();

        if (roads != null && roads.Count > 0)
        {
            roads = roads.OrderBy(r => r.transform.position.z).ToList();
            normalY = roads[0].transform.position.y;
        }
    }

    public void MoveRoad()
    {
        if (roads == null || roads.Count == 0) return;

        GameObject moveRoad = roads[0];
        roads.Remove(moveRoad);

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obs in obstacles)
        {
            if (Mathf.Abs(obs.transform.position.z - moveRoad.transform.position.z) < offset)
                Destroy(obs);
        }

        float highestZ = moveRoad.transform.position.z;
        foreach (GameObject r in roads)
        {
            if (r != null && r.transform.position.z > highestZ)
                highestZ = r.transform.position.z;
        }

        float newZ = highestZ + offset;
        float newY;
        Quaternion newRot;

        if (isFlipped)
        {
            newY = normalY + elevatedY;
            newRot = Quaternion.Euler(180f, 0f, 0f);
        }
        else
        {
            newY = normalY;
            newRot = Quaternion.identity;
        }

        moveRoad.transform.position = new Vector3(0f, newY, newZ);
        moveRoad.transform.rotation = newRot;

        roads.Add(moveRoad);

        if (obstacleSpawner != null)
            obstacleSpawner.SpawnObstaclesOnRoad(moveRoad, isFlipped);
    }

    public void SetElevated(bool elevated, float targetY)
    {
        isFlipped = elevated;
        elevatedY = targetY;
    }

    public void NormalizeRoadsAfter(float fromZ)
    {
        if (roads == null || roads.Count == 0) return;

        foreach (GameObject road in roads)
        {
            if (road == null) continue;
            if (road.transform.position.y <= normalY + 0.1f) continue;

            // Normalize ALL elevated roads regardless of Z position
            road.transform.position = new Vector3(
                road.transform.position.x,
                normalY,
                road.transform.position.z
            );

            road.transform.rotation = Quaternion.identity;
        }
    }

    public float GetGroundYBelow(float atZ)
    {
        GameObject closestRoad = null;
        float closestDist = float.MaxValue;

        foreach (GameObject road in roads)
        {
            if (road == null) continue;
            if (road.transform.position.y > normalY + 0.1f) continue;

            float dist = Mathf.Abs(road.transform.position.z - atZ);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestRoad = road;
            }
        }

        return closestRoad != null ? closestRoad.transform.position.y : normalY;
    }
}