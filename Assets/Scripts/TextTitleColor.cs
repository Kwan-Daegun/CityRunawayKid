using UnityEngine;
using TMPro;
public class TextTitleColor : MonoBehaviour
{
    [Header("References")]
    public DayNightCycle dayNightCycle;

    [Header("Colors")]
    public Color dayColor = new Color(0.1f, 0.1f, 0.1f);      // Dark color for day
    public Color nightColor = new Color(1f, 0.95f, 0.8f);     // Warm white for night
    public Color sunsetColor = new Color(1f, 0.4f, 0.1f);     // Orange for sunset

    [Header("Transition")]
    public float transitionSpeed = 2f;

    private TextMeshProUGUI textMesh;
    private Color targetColor;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        if (dayNightCycle == null)
            dayNightCycle = FindObjectOfType<DayNightCycle>();

        targetColor = dayColor;
    }

    private void Update()
    {
        if (dayNightCycle == null || textMesh == null) return;

        float currentTime = dayNightCycle.CurrentTime;

        // Pick target color based on time of day
        if (currentTime < 0.2f || currentTime > 0.8f)
            targetColor = nightColor;
        else if (currentTime < 0.35f || currentTime > 0.65f)
            targetColor = sunsetColor;
        else
            targetColor = dayColor;

        // Smoothly transition
        textMesh.color = Color.Lerp(textMesh.color, targetColor, Time.deltaTime * transitionSpeed);
    }
}
