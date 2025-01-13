using UnityEngine;

public class HelpMenuManager : MonoBehaviour
{
    public static bool helpMenuOpened = false;
    [SerializeField]
    private GameObject helpMenuUserInterface;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if(!PauseMenuController.pauseMenuOpened)
            {
                if (helpMenuOpened)
                    Resume();
                else
                    Pause();
            }

        }
    }

    public void Resume()
    {
        helpMenuUserInterface.SetActive(false);
        Time.timeScale = 1.0f;
        helpMenuOpened = false;
    }

    public void Pause()
    {
        helpMenuUserInterface.SetActive(true);
        Time.timeScale = 0.0f;
        helpMenuOpened = true;
    }
}
