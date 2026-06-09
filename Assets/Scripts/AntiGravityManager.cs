using UnityEngine;
using System.Collections;

public class AntiGravityManager : MonoBehaviour
{
    public static AntiGravityManager Instance { get; private set; }

    [Header("Settings")]
    public float duration = 8f;
    public float flipSpeed = 3f;         // How fast camera flips
    public float gravityStrength = -9.81f;

    [Header("References")]
    public Transform cameraTransform;
    public Transform playerTransform;

    private bool isAntiGravity = false;
    private Quaternion normalCameraRot;
    private Quaternion flippedCameraRot;
    private Quaternion targetCameraRot;

    public bool IsAntiGravity => isAntiGravity;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        normalCameraRot = cameraTransform.localRotation;
        flippedCameraRot = Quaternion.Euler(0f, 0f, 180f) * normalCameraRot;
        targetCameraRot = normalCameraRot;
    }

    private void Update()
    {
        // Smoothly rotate camera
        cameraTransform.localRotation = Quaternion.Slerp(
            cameraTransform.localRotation,
            targetCameraRot,
            Time.deltaTime * flipSpeed
        );
    }

    public void Activate()
    {
        if (isAntiGravity) return;
        StartCoroutine(AntiGravityCoroutine());
    }

    private IEnumerator AntiGravityCoroutine()
    {
        isAntiGravity = true;

        // Flip gravity
        Physics.gravity = new Vector3(0f, -gravityStrength, 0f);

        // Flip camera
        targetCameraRot = flippedCameraRot;

        yield return new WaitForSeconds(duration);

        // Restore gravity
        Physics.gravity = new Vector3(0f, gravityStrength, 0f);

        // Restore camera
        targetCameraRot = normalCameraRot;

        isAntiGravity = false;
    }
}