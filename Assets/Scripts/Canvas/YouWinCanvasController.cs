using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWinCanvasController : MonoBehaviour
{
    public void OnRestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void OnQuitGame()
    {
        Application.Quit();
    }
}
