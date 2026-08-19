using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool isGiantRock = false;

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }

            TestCharController controller = col.gameObject.GetComponent<TestCharController>();
            if (controller != null)
            {
                if (isGiantRock)
                    controller.OnHitGiantRock();
                else
                    controller.OnHitObstacle();
            }
        }
    }
}