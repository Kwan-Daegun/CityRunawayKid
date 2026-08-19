using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public GameObject losePanel;
    public TextMeshProUGUI finalScoreText;
    public GameObject PauseBtn;

    [Header("Pause")]
    public GameObject pausePanel;

    [Header("Loading Screen")]
    public GameObject loadingPanel;
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

   public void Pause()
{
    isPaused = true;
    Time.timeScale = 0f;
    if (PauseBtn != null)
        PauseBtn.SetActive(false);
    AudioListener.pause = true;

    if (pausePanel != null)
        pausePanel.SetActive(true);

    
        
}

    public void Resume()
{
    isPaused = false;
    Time.timeScale = 1f;
    if (PauseBtn != null)
        PauseBtn.SetActive(true);
    AudioListener.pause = false;

    if (pausePanel != null)
        pausePanel.SetActive(false);

}

    public void OnPlayerDied()
    {
        if(PauseBtn != null)
            PauseBtn.SetActive(false);
        if (losePanel != null)
            losePanel.SetActive(true);

        if (finalScoreText != null && ScoreManager.Instance != null)
            finalScoreText.text = "Final Score: " + Mathf.FloorToInt(ScoreManager.Instance.CurrentScore).ToString("N0");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (AntiGravityManager.Instance != null)
            AntiGravityManager.Instance.ResetWorldGravity();

        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex));
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (AntiGravityManager.Instance != null)
            AntiGravityManager.Instance.ResetWorldGravity();

        StartCoroutine(LoadSceneAsync(mainMenuSceneName));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        yield return StartCoroutine(ShowLoadingScreen());
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        yield return StartCoroutine(RunLoadingBar(op));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return StartCoroutine(ShowLoadingScreen());
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        yield return StartCoroutine(RunLoadingBar(op));
    }

    private IEnumerator ShowLoadingScreen()
    {
        if (losePanel != null)
            losePanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        yield return null;
    }

    private IEnumerator RunLoadingBar(AsyncOperation op)
    {
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (loadingBar != null)
                loadingBar.value = progress;

            if (loadingText != null)
                loadingText.text = "Loading... " + Mathf.FloorToInt(progress * 100f) + "%";

            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
