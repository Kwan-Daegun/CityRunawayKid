using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Bob")]
    public float bobFrequency = 10f;
    public float bobAmplitude = 0.05f;
    public float bobSmoothing = 10f;

    private Vector3 originalLocalPos;
    private float bobTimer;
    private Transform playerBody;
    private TestCharController controller;

    private void Start()
    {
        playerBody = transform.parent;
        originalLocalPos = transform.localPosition;
        controller = playerBody.GetComponent<TestCharController>();
    }

    private void Update()
    {
        HandleCameraBob();
    }

    private void HandleCameraBob()
    {
        bool antiGrav = AntiGravityManager.Instance != null && AntiGravityManager.Instance.IsAntiGravity;
        bool isAirborne = IsAirborne();

        Vector3 baseHeadPos = antiGrav
            ? new Vector3(originalLocalPos.x, -Mathf.Abs(originalLocalPos.y), originalLocalPos.z)
            : originalLocalPos;

        if (!isAirborne && controller != null && !controller.isDead)
        {
            bobTimer += Time.deltaTime * bobFrequency;

            float bobY = Mathf.Sin(bobTimer) * bobAmplitude;
            float bobX = Mathf.Sin(bobTimer * 0.5f) * bobAmplitude * 0.5f;

            if (antiGrav)
                bobY *= -1f;

            Vector3 targetPos = baseHeadPos + new Vector3(bobX, bobY, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * bobSmoothing);
        }
        else
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, baseHeadPos, Time.deltaTime * bobSmoothing);
        }
    }

    private bool IsAirborne()
    {
        if (controller == null) return false;

        return (bool)typeof(TestCharController)
            .GetField("isAirborne", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(controller);
    }
}