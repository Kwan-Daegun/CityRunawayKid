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

    [Header("Loading Screen")]
    public GameObject loadingPanel;
    public Slider loadingBar;              
    public TextMeshProUGUI loadingText;    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    public void OnPlayerDied()
    {
        if (losePanel != null)
            losePanel.SetActive(true);

        if (finalScoreText != null && ScoreManager.Instance != null)
            finalScoreText.text = "Score: " + Mathf.FloorToInt(ScoreManager.Instance.CurrentScore).ToString("N0");
    }

    public void RestartGame()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        
        if (losePanel != null)
            losePanel.SetActive(false);

        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
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