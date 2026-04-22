using UnityEngine;

public class TestCharController : MonoBehaviour
{
    public SpawnerManager spawnerManager;
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float fallMultiplier = 3.5f;
    public float upMultiplier = 2.5f;
    public float groundCheckDistance = 0.6f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.forward;
    private bool isGrounded;
    private bool isJumping;
    private bool isAirborne;
    public bool isDead = false;
    private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;
    private float coyoteTime = 0.12f;
    private float coyoteCounter;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && !isAirborne)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f && !isAirborne)
        {
            isJumping = true;
            isAirborne = true;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.constraints = isAirborne
            ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ
            : RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        if (isJumping)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isJumping = false;
        }

        if (rb.linearVelocity.y > jumpForce)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0 && isAirborne)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * upMultiplier * Time.fixedDeltaTime;

        if (isAirborne && isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            isAirborne = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }

        moveDirection = Vector3.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;

        if (!isAirborne && velocity.y > 0 && velocity.y < 1f)
            velocity.y = 0f;

        if (!isAirborne && rb.linearVelocity.z < 1f)
            velocity = new Vector3(velocity.x, 0f, moveSpeed);

        rb.linearVelocity = velocity;
    }

    public void OnHitObstacle()
    {
        isDead = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * 6f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (((1 << col.gameObject.layer) & wallLayer) != 0)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    private void OnTriggerEnter(Collider col)
    {
        spawnerManager.SpawnTriggerEntered();
    }
}