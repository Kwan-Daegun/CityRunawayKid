using UnityEngine;

using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        if (LoadingManager.Instance != null)
            LoadingManager.Instance.LoadScene("GameScene");
        else
            SceneManager.LoadScene("GameScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
