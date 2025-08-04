using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public void StartLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void RestartLevel()
    {
        string level = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(level);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlaySound(AudioSource audio)
    {
        audio.Play();
    }
}
