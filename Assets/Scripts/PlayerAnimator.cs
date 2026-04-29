using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    public Transform visualCapsule;
    public TestCharController controller;

    [Header("Squash & Stretch")]
    public float squashAmount = 0.75f;
    public float stretchAmount = 1.2f;
    public float squashSpeed = 6f;

    [Header("Tilt")]
    public float tiltAmount = 12f;
    public float tiltSpeed = 5f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private float currentTiltZ;
    private bool wasAirborne;

    private void Start()
    {
        if (visualCapsule == null)
            visualCapsule = transform;

        originalScale = visualCapsule.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        if (controller.isDead)
        {
            DeathVisual();
            return;
        }

        HandleSquashStretch();
        HandleTilt();
    }

    private void HandleSquashStretch()
    {
        bool isAirborne = IsAirborne();

        if (isAirborne && !wasAirborne)
        {
            targetScale = new Vector3(
                originalScale.x * 0.8f,
                originalScale.y * stretchAmount,
                originalScale.z * 0.8f
            );
        }

        if (!isAirborne && wasAirborne)
        {
            targetScale = new Vector3(
                originalScale.x * 1.2f,
                originalScale.y * squashAmount,
                originalScale.z * 1.2f
            );
        }

        wasAirborne = isAirborne;

        // Ease target back toward original
        targetScale = Vector3.Lerp(targetScale, originalScale, Time.deltaTime * squashSpeed);

        // Visual scale chases target — double lerp = elastic feel
        visualCapsule.localScale = Vector3.Lerp(
            visualCapsule.localScale,
            targetScale,
            Time.deltaTime * squashSpeed * 2f
        );
    }

    private void HandleTilt()
    {
        float targetTilt = 0f;
        if (Input.GetKey(KeyCode.A)) targetTilt = tiltAmount;
        if (Input.GetKey(KeyCode.D)) targetTilt = -tiltAmount;

        // Smooth the tilt angle directly instead of lerping quaternions
        currentTiltZ = Mathf.Lerp(currentTiltZ, targetTilt, Time.deltaTime * tiltSpeed);

        visualCapsule.localRotation = Quaternion.Euler(0f, 0f, currentTiltZ);
    }

    private void DeathVisual()
    {
        currentTiltZ = Mathf.Lerp(currentTiltZ, 90f, Time.deltaTime * 4f);
        visualCapsule.localRotation = Quaternion.Euler(0f, 0f, currentTiltZ);

        visualCapsule.localScale = Vector3.Lerp(
            visualCapsule.localScale,
            new Vector3(originalScale.x * 1.2f, originalScale.y * 0.4f, originalScale.z * 1.2f),
            Time.deltaTime * 5f
        );
    }

    private bool IsAirborne()
    {
        return (bool)typeof(TestCharController)
            .GetField("isAirborne", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(controller);
    }
}