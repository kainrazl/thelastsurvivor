using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public void StartLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
