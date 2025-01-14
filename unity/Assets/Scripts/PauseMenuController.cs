using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{

    public static bool pauseMenuOpened = false;


    [SerializeField]
    private GameObject pauseMenuUserInterface;

    [SerializeField]
    private GameObject headUpDisplayUserInterface;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!ConfigurationMenuManager.configurationMenuOpened && !HelpMenuManager.helpMenuOpened)
            {
                if (pauseMenuOpened)
                    Resume();
                else
                    Pause();
            }

        }
    }

    public void Resume()
    {
        pauseMenuUserInterface.SetActive(false);
        headUpDisplayUserInterface.SetActive(true);
        Time.timeScale = 1.0f;
        pauseMenuOpened = false;
    }

    public void Pause()
    {
        pauseMenuUserInterface.SetActive(true);
        headUpDisplayUserInterface.SetActive(false);
        Time.timeScale = 0.0f;
        pauseMenuOpened = true;
    }


    public void LoadStartMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void Exit()
    { 
        Application.Quit();
    }
}
