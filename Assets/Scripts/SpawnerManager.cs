using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    RoadSpawner roadSpawner;
    void Start()
    {
        roadSpawner = GetComponent<RoadSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnTriggerEntered()
    {
        roadSpawner.MoveRoad();
    }
}
