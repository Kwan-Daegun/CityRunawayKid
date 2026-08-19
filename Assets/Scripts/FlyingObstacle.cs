using UnityEngine;

public class FlyingObstacle : MonoBehaviour
{
    [Header("Flight Settings")]
    public float amplitude = 0.5f;
    public float frequency = 1.5f;
    public float destroyBehindZ = -20f;

    private Rigidbody rb;
    private Transform player;
    private float startY;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearDamping = 2f;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        startY = transform.position.y;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (transform.position.z < player.position.z + destroyBehindZ)
        {
            Destroy(gameObject);
            return;
        }

        
        float targetSpeed = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentSpeed : 10f;

        
        float targetY = startY + Mathf.Sin(Time.time * frequency) * amplitude;
        Vector3 direction = new Vector3(0f, targetY - transform.position.y, 0f);

        if (rb == null) return;

        rb.linearVelocity = new Vector3(
            0f,
            direction.y * 5f,
            -targetSpeed          
        );
    }
}
