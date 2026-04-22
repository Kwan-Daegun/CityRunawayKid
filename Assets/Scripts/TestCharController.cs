using UnityEngine;

public class TestCharController : MonoBehaviour
{
    public SpawnerManager spawnerManager;
    public float moveSpeed = 10f;
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float groundCheckDistance = 0.6f;
    public LayerMask groundLayer;
    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.forward;
    private bool isGrounded;
    private bool isJumping;
    private bool isAirborne;
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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && !isAirborne)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            isJumping = true;
            isAirborne = true;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }
    }

    void FixedUpdate()
    {
        rb.constraints = isAirborne
            ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ
            : RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        if (isJumping)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isJumping = false;
        }

        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;

        if (isAirborne && isGrounded && rb.linearVelocity.y <= 0)
            isAirborne = false;

        moveDirection = Vector3.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;

        if (!isAirborne && velocity.y > 0 && velocity.y < 1f)
            velocity.y = 0f;

        rb.linearVelocity = velocity;
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