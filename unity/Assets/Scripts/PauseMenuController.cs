using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool isPaused = false;

    [SerializeField]
    private GameObject pauseMenuUserInterface;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUserInterface.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUserInterface.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }


    public void LoadStartMen()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void Exit()
    { 
        Application.Quit();
    }
}
