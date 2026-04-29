using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Ground Obstacles")]
    public GameObject[] obstaclePrefabs;

    [Header("Flying Obstacles")]
    public GameObject[] flyingObstaclePrefabs;
    public float flyingSpawnChance = 0.4f;
    public float flyingHeight = 2.5f;
    public float flyingUnlockScore = 500f;      

    [Header("Spawn Settings")]
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

        
        bool flyingUnlocked = ScoreManager.Instance != null
            && ScoreManager.Instance.CurrentScore >= flyingUnlockScore;

        if (flyingUnlocked && flyingObstaclePrefabs != null && flyingObstaclePrefabs.Length > 0
            && Random.value <= flyingSpawnChance)
        {
            GameObject flyingPrefab = flyingObstaclePrefabs[Random.Range(0, flyingObstaclePrefabs.Length)];
            float x = lanePositions[Random.Range(0, lanePositions.Length)];
            Vector3 flyPos = new Vector3(x, road.transform.position.y + flyingHeight, road.transform.position.z + spawnZ);
            Instantiate(flyingPrefab, flyPos, Quaternion.identity);
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