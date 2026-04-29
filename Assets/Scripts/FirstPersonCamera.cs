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
        bool isAirborne = IsAirborne();

        if (!isAirborne && !controller.isDead)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobY = Mathf.Sin(bobTimer) * bobAmplitude;
            float bobX = Mathf.Sin(bobTimer * 0.5f) * bobAmplitude * 0.5f;

            Vector3 targetPos = originalLocalPos + new Vector3(bobX, bobY, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * bobSmoothing);
        }
        else
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPos, Time.deltaTime * bobSmoothing);
        }
    }

    private bool IsAirborne()
    {
        return (bool)typeof(TestCharController)
            .GetField("isAirborne", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(controller);
    }
}