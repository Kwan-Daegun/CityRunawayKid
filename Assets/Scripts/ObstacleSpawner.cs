using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public float spawnZ = 20f;
    public int minObstacles = 1;
    public int maxObstacles = 3;

    private float[] lanePositions = { -4.5f, -1.5f, 1.5f, 4.5f };

    public void SpawnObstaclesOnRoad(GameObject road)
    {
        int count = Random.Range(minObstacles, maxObstacles + 1);
        
        int[] shuffledLanes = ShuffleLanes();
        
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            float x = lanePositions[shuffledLanes[i]];
            Vector3 spawnPos = new Vector3(x, road.transform.position.y + 1f, road.transform.position.z + spawnZ);
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
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