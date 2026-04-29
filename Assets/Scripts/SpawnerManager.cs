using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    RoadSpawner roadSpawner;
    void Start()
    {
        roadSpawner = GetComponent<RoadSpawner>();
    }

    
    void Update()
    {
        
    }
    public void SpawnTriggerEntered()
    {
        roadSpawner.MoveRoad();
    }
}
