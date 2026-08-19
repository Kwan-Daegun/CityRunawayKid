using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [Header("Speed Boost Settings")]
    public float boostMultiplier = 2f;
    public float boostDuration = 5f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 5f;

    [Header("Fish Eye Settings")]
    public float normalFOV = 60f;
    public float boostFOV = 90f;
    public float fovTransitionSpeed = 5f;

    [Header("Lens Distortion")]
    public float normalDistortion = 0f;
    public float boostDistortion = -0.4f;
    public float distortionTransitionSpeed = 3f;

    [Header("References")]
    public Volume postProcessVolume;
    public Camera mainCamera;

    private LensDistortion lensDistortion;
    private bool isBoosting = false;
    private bool isInvincible = false;
    private float targetFOV;
    private float targetDistortion;

    public bool IsBoosting => isBoosting;
    public bool IsInvincible => isInvincible;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        targetFOV = normalFOV;
        targetDistortion = normalDistortion;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (postProcessVolume != null)
            postProcessVolume.profile.TryGet(out lensDistortion);
    }

    private void Update()
    {
        if (mainCamera == null) return;

        // FOV based on current speed — wider as speed increases
        if (!isBoosting)
        {
            float currentSpeed = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentSpeed : 0f;
            float maxSpeed = ScoreManager.Instance != null ? ScoreManager.Instance.maxSpeed : 30f;

            float speedPercent = Mathf.Clamp01(currentSpeed / maxSpeed);
            targetFOV = Mathf.Lerp(normalFOV, boostFOV, speedPercent);
        }

        mainCamera.fieldOfView = Mathf.Lerp(
            mainCamera.fieldOfView,
            targetFOV,
            Time.deltaTime * fovTransitionSpeed
        );

        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = Mathf.Lerp(
                lensDistortion.intensity.value,
                targetDistortion,
                Time.deltaTime * distortionTransitionSpeed
            );
        }
    }

    public void ActivateSpeedBoost()
    {
        if (isBoosting) return;
        StartCoroutine(SpeedBoostCoroutine());
    }

    public void ActivateInvincibility()
    {
        if (isInvincible) return;
        StartCoroutine(InvincibilityCoroutine());
    }

    public void ActivateCombo()
    {
        if (!isBoosting) StartCoroutine(SpeedBoostCoroutine());
        if (!isInvincible) StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator SpeedBoostCoroutine()
    {
        if (ScoreManager.Instance == null) yield break;

        isBoosting = true;
        ScoreManager.Instance.speedBoostActive = true;
        ScoreManager.Instance.speedBoostMultiplier = boostMultiplier;

        targetFOV = boostFOV;
        targetDistortion = boostDistortion;

        yield return new WaitForSeconds(boostDuration);

        ScoreManager.Instance.speedBoostActive = false;
        ScoreManager.Instance.speedBoostMultiplier = 1f;
        targetDistortion = normalDistortion;

        isBoosting = false;
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}