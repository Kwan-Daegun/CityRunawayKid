using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public float cycleDuration = 120f;
    public float startTime = 0.25f;

    [Header("Sun Colors")]
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moonLight;
    public Color moonColor = new Color(0.4f, 0.5f, 0.8f);
    public float moonMaxIntensity = 0.3f;

    [Header("Fog")]
    public bool enableFog = true;
    public Gradient fogColor;
    public float dayFogDensity = 0.01f;
    public float nightFogDensity = 0.04f;

    [Header("Ambient Light")]
    public Gradient ambientColor;

    [Header("Skybox Materials")]
    public Material daySkybox;
    public Material sunsetSkybox;
    public Material nightSkybox;

    public float CurrentTime => currentTime;
    private Light sun;
    private float currentTime;
    private Material currentSkybox;

    private void Start()
    {
        sun = GetComponent<Light>();
        currentTime = startTime;

        RenderSettings.fog = enableFog;
        RenderSettings.fogMode = FogMode.Exponential;

        if (sunColor == null || sunColor.colorKeys.Length == 0)
            SetDefaultGradients();

        if (moonLight != null)
        {
            moonLight.type = LightType.Directional;
            moonLight.color = moonColor;
            moonLight.intensity = 0f;
        }
    }

    private void Update()
    {
        if (ScoreManager.Instance != null && !IsGameRunning()) return;

        currentTime += Time.deltaTime / cycleDuration;
        if (currentTime >= 1f) currentTime = 0f;

        UpdateSun();
        UpdateMoon();
        UpdateFog();
        UpdateAmbient();
        UpdateSkybox();
    }

    private void UpdateSun()
    {
        transform.localRotation = Quaternion.Euler(
            (currentTime * 360f) - 90f,
            -130f,
            0f
        );

        if (sun != null)
        {
            sun.color = sunColor.Evaluate(currentTime);
            sun.intensity = sunIntensity.Evaluate(currentTime);
        }
    }

    private void UpdateMoon()
    {
        if (moonLight == null) return;

        // Moon is opposite the sun — offset by 0.5
        float moonTime = (currentTime + 0.5f) % 1f;

        moonLight.transform.localRotation = Quaternion.Euler(
            (moonTime * 360f) - 90f,
            -130f,
            0f
        );

        // Only visible at night (currentTime < 0.2 or > 0.8)
        bool isNight = currentTime < 0.2f || currentTime > 0.8f;
        float targetIntensity = isNight ? moonMaxIntensity : 0f;
        moonLight.intensity = Mathf.Lerp(moonLight.intensity, targetIntensity, Time.deltaTime * 2f);
        moonLight.color = moonColor;
    }

    private void UpdateFog()
    {
        if (!enableFog) return;

        RenderSettings.fogColor = fogColor.Evaluate(currentTime);

        float isNight = Mathf.Abs(currentTime - 0.5f) > 0.25f ? 1f : 0f;
        RenderSettings.fogDensity = Mathf.Lerp(dayFogDensity, nightFogDensity,
            Mathf.Clamp01(isNight + (Mathf.Abs(currentTime - 0.5f) - 0.2f) * 4f));
    }

    private void UpdateAmbient()
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(currentTime);
    }

    private void UpdateSkybox()
    {
        Material targetSkybox;

        if (currentTime < 0.2f || currentTime > 0.8f)
            targetSkybox = nightSkybox;
        else if (currentTime < 0.35f || currentTime > 0.65f)
            targetSkybox = sunsetSkybox;
        else
            targetSkybox = daySkybox;

        if (targetSkybox != null && targetSkybox != currentSkybox)
        {
            RenderSettings.skybox = targetSkybox;
            currentSkybox = targetSkybox;
            DynamicGI.UpdateEnvironment();
        }
    }

    private bool IsGameRunning()
    {
        return ScoreManager.Instance != null && !float.IsNaN(ScoreManager.Instance.CurrentScore);
    }

    private void SetDefaultGradients()
    {
        sunColor = new Gradient();
        sunColor.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.1f, 0.1f, 0.2f), 0f),
                new GradientColorKey(new Color(1f, 0.5f, 0.2f), 0.25f),
                new GradientColorKey(new Color(1f, 0.95f, 0.8f), 0.5f),
                new GradientColorKey(new Color(1f, 0.4f, 0.1f), 0.75f),
                new GradientColorKey(new Color(0.1f, 0.1f, 0.2f), 1f),
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );

        sunIntensity = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 0.8f),
            new Keyframe(0.5f, 1.2f),
            new Keyframe(0.75f, 0.8f),
            new Keyframe(1f, 0f)
        );

        fogColor = new Gradient();
        fogColor.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.05f, 0.05f, 0.1f), 0f),
                new GradientColorKey(new Color(0.8f, 0.4f, 0.2f), 0.25f),
                new GradientColorKey(new Color(0.7f, 0.85f, 1f), 0.5f),
                new GradientColorKey(new Color(0.8f, 0.3f, 0.1f), 0.75f),
                new GradientColorKey(new Color(0.05f, 0.05f, 0.1f), 1f),
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );

        ambientColor = new Gradient();
        ambientColor.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 0f),
                new GradientColorKey(new Color(0.4f, 0.3f, 0.3f), 0.25f),
                new GradientColorKey(new Color(0.5f, 0.5f, 0.6f), 0.5f),
                new GradientColorKey(new Color(0.4f, 0.3f, 0.2f), 0.75f),
                new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 1f),
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
    }
}