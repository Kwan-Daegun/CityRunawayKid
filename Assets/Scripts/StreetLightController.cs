using UnityEngine;
using System.Collections.Generic;

public class StreetLightController : MonoBehaviour
{
    [Header("Settings")]
    public float turnOnTime = 0.7f;
    public float turnOffTime = 0.3f;
    public float lightIntensity = 1.5f;
    public Color lightColor = new Color(1f, 0.9f, 0.6f);

    [Header("Emissive Glow")]
    public string streetLightTag = "StreetLight";  // Tag your street light prefab with this
    public Color bulbOnColor = new Color(1f, 0.9f, 0.5f);
    public Color bulbOffColor = Color.black;

    private DayNightCycle dayNightCycle;
    private bool lightsOn = false;

    private void Start()
    {
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        RefreshLights(false);
    }

    private void Update()
    {
        if (dayNightCycle == null) return;

        float currentTime = dayNightCycle.CurrentTime;
        bool shouldBeOn = currentTime >= turnOnTime || currentTime <= turnOffTime;

        if (shouldBeOn && !lightsOn)
        {
            lightsOn = true;
            RefreshLights(true);
        }
        else if (!shouldBeOn && lightsOn)
        {
            lightsOn = false;
            RefreshLights(false);
        }
    }

    private void RefreshLights(bool on)
    {
        // Find all street lights in scene by tag every time state changes
        GameObject[] lightObjects = GameObject.FindGameObjectsWithTag(streetLightTag);

        foreach (GameObject obj in lightObjects)
        {
            // Toggle Light component
            Light l = obj.GetComponentInChildren<Light>();
            if (l != null)
            {
                l.enabled = on;
                l.color = lightColor;
                l.intensity = lightIntensity;
            }

            // Toggle emissive glow on bulb renderer
            Renderer r = obj.GetComponentInChildren<Renderer>();
            if (r != null)
            {
                r.material.SetColor("_EmissionColor", on ? bulbOnColor : bulbOffColor);
                if (on)
                    r.material.EnableKeyword("_EMISSION");
                else
                    r.material.DisableKeyword("_EMISSION");
            }
        }
    }
}
