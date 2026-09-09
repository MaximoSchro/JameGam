using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
