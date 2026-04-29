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
        rb.useGravity = false;
        rb.linearDamping = 2f;

        player = GameObject.FindGameObjectWithTag("Player").transform;
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

        // Move toward player (negative Z = toward player)
        float targetSpeed = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentSpeed : 10f;

        // Bob up and down
        float targetY = startY + Mathf.Sin(Time.time * frequency) * amplitude;
        Vector3 direction = new Vector3(0f, targetY - transform.position.y, 0f);

        rb.linearVelocity = new Vector3(
            0f,
            direction.y * 5f,
            -targetSpeed          // Fly toward the player at the same speed as the road
        );
    }
}