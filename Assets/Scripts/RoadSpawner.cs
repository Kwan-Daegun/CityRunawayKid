using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoadSpawner : MonoBehaviour
{
    public List<GameObject> roads;
    public float offset = 60f;
    private ObstacleSpawner obstacleSpawner;

    void Start()
    {
        obstacleSpawner = GetComponent<ObstacleSpawner>();
        if (roads != null && roads.Count > 0)
            roads = roads.OrderBy(r => r.transform.position.z).ToList();
    }

    public void MoveRoad()
    {
        GameObject moveRoad = roads[0];
        roads.Remove(moveRoad);

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obs in obstacles)
        {
            if (Mathf.Abs(obs.transform.position.z - moveRoad.transform.position.z) < offset)
                Destroy(obs);
        }

        float newZ = roads[roads.Count - 1].transform.position.z + offset;
        moveRoad.transform.position = new Vector3(0, 0, newZ);
        roads.Add(moveRoad);

        if (obstacleSpawner != null)
            obstacleSpawner.SpawnObstaclesOnRoad(moveRoad);
    }
}