using UnityEngine;
using UnityEngine.UI;

public class HelpMenuManager : MonoBehaviour
{
    public static bool helpMenuOpened = false;

    [SerializeField]
    private GameObject helpMenuUserInterface;

    [SerializeField]
    private GameObject headUpDisplayUserInterface;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!PauseMenuController.pauseMenuOpened && !ConfigurationMenuManager.configurationMenuOpened)
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
        Time.timeScale = 1.0f;
        helpMenuOpened = false;
        helpMenuUserInterface.SetActive(false);
        headUpDisplayUserInterface.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        helpMenuUserInterface.SetActive(true);
        headUpDisplayUserInterface.SetActive(false);
        Time.timeScale = 0.0f;
        helpMenuOpened = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
