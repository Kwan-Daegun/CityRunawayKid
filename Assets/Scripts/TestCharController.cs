using UnityEngine;

public class TestCharController : MonoBehaviour
{
    public SpawnerManager spawnerManager;
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
    private float landCooldown = 0f;
    private float landCooldownTime = 0.1f;
    private float inputSuppressedUntil = 0f;

    [Header("Dive / Slam Down")]
    public float diveForce = 30f;
    private bool isDiving = false;

    public float fallbackMoveSpeed = 10f;
    private float moveSpeed => ScoreManager.Instance != null ? ScoreManager.Instance.CurrentSpeed : fallbackMoveSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isDead) return;
        if (Time.timeScale == 0f)
        {
            ClearBufferedActions();
            return;
        }
        if (Time.unscaledTime < inputSuppressedUntil)
        {
            ClearBufferedActions();
            return;
        }

        isGrounded = Physics.Raycast(transform.position, gravityDown, groundCheckDistance, groundLayer);

        if (isGrounded && !isAirborne)
        {
            coyoteCounter = coyoteTime;
            isDiving = false;
        }
        else
            coyoteCounter -= Time.deltaTime;

        if (landCooldown > 0f)
            landCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f && !isAirborne && landCooldown <= 0f)
        {
            isJumping = true;
            isAirborne = true;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isAirborne && !isDiving)
            isDiving = true;
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (Time.timeScale == 0f) return;

        bool antiGrav = AntiGravityManager.Instance != null && AntiGravityManager.Instance.IsAntiGravity;

        rb.constraints = isAirborne
            ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ
            : RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        if (isJumping)
        {
            float jumpDirection = antiGrav ? -jumpForce : jumpForce;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpDirection, rb.linearVelocity.z);
            isJumping = false;
        }

        if (isDiving)
        {
            float diveDirection = antiGrav ? diveForce : -diveForce;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, diveDirection, rb.linearVelocity.z);
            isDiving = false;
        }

        if (!antiGrav && rb.linearVelocity.y > jumpForce)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

        if (antiGrav && rb.linearVelocity.y < -jumpForce)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -jumpForce, rb.linearVelocity.z);

        if (!antiGrav)
        {
            if (rb.linearVelocity.y < 0)
                rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
            else if (rb.linearVelocity.y > 0 && isAirborne && !isDiving)
                rb.linearVelocity += Vector3.up * Physics.gravity.y * upMultiplier * Time.fixedDeltaTime;
        }
        else
        {
            if (rb.linearVelocity.y > 0)
                rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
            else if (rb.linearVelocity.y < 0 && isAirborne && !isDiving)
                rb.linearVelocity += Vector3.up * Physics.gravity.y * upMultiplier * Time.fixedDeltaTime;
        }

        bool movingTowardGround = antiGrav
            ? rb.linearVelocity.y >= -0.1f
            : rb.linearVelocity.y <= 0.1f;

        if (isAirborne && isGrounded && movingTowardGround)
        {
            isAirborne = false;
            landCooldown = landCooldownTime;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }

        moveDirection = Vector3.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;

        if (!isAirborne && !antiGrav && velocity.y > 0 && velocity.y < 1f)
            velocity.y = 0f;

        if (!isAirborne && antiGrav && velocity.y < 0 && velocity.y > -1f)
            velocity.y = 0f;

        if (!isAirborne && rb.linearVelocity.z < 1f)
            velocity = new Vector3(velocity.x, 0f, moveSpeed);

        rb.linearVelocity = velocity;
    }

    public void ForceAirborne()
    {
        isAirborne = true;
        isGrounded = false;
        coyoteCounter = 0f;
        landCooldown = landCooldownTime;
    }

    public void ClearBufferedActions()
    {
        isJumping = false;
        isDiving = false;
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
    }

    public void SuppressInput(float seconds)
    {
        inputSuppressedUntil = Mathf.Max(inputSuppressedUntil, Time.unscaledTime + seconds);
        ClearBufferedActions();
    }

    public void OnHitObstacle()
    {
        if (PowerUpManager.Instance != null && PowerUpManager.Instance.IsInvincible)
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddBonus(Random.Range(100f, 200f));
            return;
        }

        KillPlayer();
    }

    public void OnHitGiantRock()
    {
        KillPlayer();
    }

    private void KillPlayer()
    {
        isDead = true;

        PlayerSounds sounds = GetComponent<PlayerSounds>();
        if (sounds != null)
            sounds.PlayHitSound();

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StopScoring();

        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerDied();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * 6f, ForceMode.Impulse);
    }

    private Vector3 gravityDown => AntiGravityManager.Instance != null && AntiGravityManager.Instance.IsAntiGravity
        ? Vector3.up
        : Vector3.down;

    private void OnCollisionEnter(Collision col)
    {
        if (((1 << col.gameObject.layer) & wallLayer) != 0)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + gravityDown * groundCheckDistance);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (spawnerManager != null)
            spawnerManager.SpawnTriggerEntered();
    }
}