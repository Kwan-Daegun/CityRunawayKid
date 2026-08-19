using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    public float pointsPerSecond = 10f;
    [HideInInspector] public float scoreMultiplier = 1f;

    [Header("Speed Settings")]
    public float baseSpeed = 10f;
    public float maxSpeed = 30f;
    public float speedIncreaseAmount = 2f;
    public float[] scoreThresholds = { 100f, 250f, 500f, 1000f, 2000f, 3500f, 5000f };

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI speedLevelText;

    private float currentScore = 0f;
    private float currentSpeed;
    private int nextThresholdIndex = 0;
    private bool isRunning = false;

    [HideInInspector] public bool speedBoostActive = false;
    [HideInInspector] public float speedBoostMultiplier = 1f;

    public float CurrentSpeed => speedBoostActive
    ? Mathf.Min(currentSpeed * speedBoostMultiplier, maxSpeed * 1.2f)
    : currentSpeed;

    public float CurrentScore => currentScore;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentSpeed = baseSpeed;
        isRunning = true;
        UpdateUI();
    }

    private void Update()
    {
        if (!isRunning) return;

        currentScore += pointsPerSecond * scoreMultiplier * Time.deltaTime;
        CheckThresholds();
        UpdateUI();
    }

    private void CheckThresholds()
    {
        if (nextThresholdIndex >= scoreThresholds.Length) return;

        if (currentScore >= scoreThresholds[nextThresholdIndex])
        {
            currentSpeed = Mathf.Min(currentSpeed + speedIncreaseAmount, maxSpeed);
            nextThresholdIndex++;
            Debug.Log($"Speed increased to {currentSpeed}! (Threshold {nextThresholdIndex})");
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score:" + Mathf.FloorToInt(currentScore).ToString("N0");

        if (speedLevelText != null)
            speedLevelText.text = $"LVL {nextThresholdIndex + 1}";
    }

    public void StopScoring()
    {
        isRunning = false;
    }

    public void AddBonus(float bonus)
    {
        currentScore += bonus;
    }
}