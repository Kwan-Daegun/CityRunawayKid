using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class LoadingManager : MonoBehaviour
{
     [Header("UI References")]
    [SerializeField] private GameObject loadingScreenUI;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Settings")]
    [SerializeField] private float minimumLoadTime = 1.5f;

    public static LoadingManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingScreenUI != null)
            loadingScreenUI.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreenUI != null)
            loadingScreenUI.SetActive(true);

        float timer = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(timer / minimumLoadTime);
            float displayProgress = Mathf.Min(loadProgress, timeProgress);

            UpdateUI(displayProgress);

            if (operation.progress >= 0.9f && timer >= minimumLoadTime)
            {
                UpdateUI(1f);
                yield return null;
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreenUI != null)
            loadingScreenUI.SetActive(false);
    }

    private void UpdateUI(float progress)
    {
        if (progressBar != null)
            progressBar.value = progress;

        if (progressText != null)
            progressText.text = $"Loading... {Mathf.RoundToInt(progress * 100f)}%";
    }   
}
