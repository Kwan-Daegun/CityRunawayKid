using UnityEngine;

public class TestCharController : MonoBehaviour
{
    public SpawnerManager spawnerManager;
    public float moveSpeed = 10f;
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.forward;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    void FixedUpdate()
    {
        // Freeze all rotation always to prevent wall bump spinning
        rb.constraints = isGrounded
            ? RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ
            : RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // Apply extra gravity when falling for a natural arc
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        }

        // Always move forward
        moveDirection = Vector3.forward;

        // Add left/right movement based on input
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

        // Apply horizontal velocity while preserving gravity (Y velocity)
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        spawnerManager.SpawnTriggerEntered();
    }
}