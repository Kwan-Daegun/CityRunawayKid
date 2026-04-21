using System.Transactions;
using UnityEngine;

public class TestCharController : MonoBehaviour
{
    public SpawnerManager spawnerManager;
    public float moveSPeed = 10f;
    void Update()
    {
        float hmove = Input.GetAxis("Horizontal") * moveSPeed / 2;
        float vmove = Input.GetAxis("Vertical") * moveSPeed;
        transform.Translate(new Vector3(hmove, 0, vmove) * Time.deltaTime);

    }
    private void OnTriggerEnter(Collider col)
    {
        spawnerManager.SpawnTriggerEntered();
    }

}
