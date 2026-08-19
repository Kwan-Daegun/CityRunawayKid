using UnityEngine;
using System.Collections;

public class AntiGravityManager : MonoBehaviour
{
    public static AntiGravityManager Instance { get; private set; }

    [Header("Settings")]
    public float flipSpeed = 4f;
     public float elevatedY = 20f;

    [Header("Player Launch")]
    public float launchForce = 20f;

    [Header("Score Bonus")]
    public float antiGravityScoreMultiplier = 2f;

    [Header("References")]
    public Transform cameraTransform;
    public RoadSpawner roadSpawner;
    private float returnFromZ = 0f;

    private bool isAntiGravity = false;
    private bool eventInProgress = false;

    private Quaternion targetCameraRot;
    private Quaternion normalCameraRot;
    private Quaternion flippedCameraRot;

    public bool IsAntiGravity => isAntiGravity;
    public bool EventInProgress => eventInProgress;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetWorldGravity();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (roadSpawner == null)
            roadSpawner = FindObjectOfType<RoadSpawner>();

        normalCameraRot = cameraTransform.localRotation;
        flippedCameraRot = Quaternion.Euler(0f, 0f, 180f);
        targetCameraRot = normalCameraRot;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            ResetWorldGravity();
    }

    public void ResetWorldGravity()
    {
        StopAllCoroutines();
        isAntiGravity = false;
        eventInProgress = false;
        Physics.gravity = new Vector3(0f, -9.81f, 0f);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.scoreMultiplier = 1f;

        targetCameraRot = normalCameraRot;

        if (roadSpawner != null)
            roadSpawner.SetElevated(false, elevatedY);
    }

    private void Update()
    {
        cameraTransform.localRotation = Quaternion.Slerp(
            cameraTransform.localRotation,
            targetCameraRot,
            Time.deltaTime * flipSpeed
        );
    }

    public void PrepareElevatedRoads()
    {
        eventInProgress = true;
        if (roadSpawner != null)
            roadSpawner.SetElevated(true, elevatedY);
    }

    public void PrepareNormalRoads()
    {
        if (roadSpawner != null)
            roadSpawner.SetElevated(false, elevatedY);
    }

    public void PrepareNormalRoadsAfter(float fromZ)
    {
        if (roadSpawner == null) return;
        roadSpawner.SetElevated(false, elevatedY);
        roadSpawner.NormalizeRoadsAfter(fromZ);
    }

    public void Activate()
{
    if (isAntiGravity) return;

    StartCoroutine(AntiGravityCoroutine());

    AntiGravitySpawner spawner = FindObjectOfType<AntiGravitySpawner>();
    if (spawner != null && roadSpawner != null && roadSpawner.roads.Count > 0)
        spawner.SpawnReturnPickupOnRoad(roadSpawner.roads[roadSpawner.roads.Count - 1]);

    // NO PrepareNormalRoads here!
}

    private IEnumerator AntiGravityCoroutine()
    {
        isAntiGravity = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            TestCharController controller = player.GetComponent<TestCharController>();
            if (controller != null)
                controller.ForceAirborne();

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * launchForce, ForceMode.VelocityChange);
            }
        }

        targetCameraRot = flippedCameraRot;
        Physics.gravity = new Vector3(0f, 9.81f, 0f);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.scoreMultiplier = antiGravityScoreMultiplier;

        yield break;
    }

    public void ReturnToNormal()
    {
        if (!isAntiGravity) return;


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            returnFromZ = player.transform.position.z;
        }
        else if (roadSpawner != null && roadSpawner.roads.Count > 0)
        {
            returnFromZ = roadSpawner.roads[0].transform.position.z;
        }

        // Stop elevated spawning NOW
        PrepareNormalRoads();

        StopAllCoroutines();
        StartCoroutine(ReturnCoroutine());
    }

private IEnumerator ReturnCoroutine()
{
    Physics.gravity = new Vector3(0f, -9.81f, 0f);

    if (ScoreManager.Instance != null)
        ScoreManager.Instance.scoreMultiplier = 1f;

    targetCameraRot = normalCameraRot;
    PrepareNormalRoadsAfter(returnFromZ);

    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }

    isAntiGravity = false;
    eventInProgress = false;

    // Wait one frame then force airborne so player falls naturally
    yield return null;

    player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        TestCharController controller = player.GetComponent<TestCharController>();
        if (controller != null)
            controller.ForceAirborne();
    }
}
}