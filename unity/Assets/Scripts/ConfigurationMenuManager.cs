using UnityEngine;

public class ConfigurationMenuManager : MonoBehaviour
{
    public static bool configurationMenuOpened = false;

    [SerializeField]
    private GameObject configurationMenuUserInterface;

    [SerializeField]
    private TMPro.TMP_Dropdown scenarios;

    [SerializeField]
    private LocationManager locationManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if(!PauseMenuController.pauseMenuOpened)
            {
                if (configurationMenuOpened)
                    Resume();
                else
                    Pause();
            }

        }
    }

    public void Resume()
    {
        configurationMenuUserInterface.SetActive(false);
        Time.timeScale = 1.0f;
        configurationMenuOpened = false;
    }

    public void Pause()
    {
        configurationMenuUserInterface.SetActive(true);
        Time.timeScale = 0.0f;
        configurationMenuOpened = true;
    }

    public void LoadScenario() {
        switch (scenarios.value) {
            case 0: locationManager.PlayClearWeatherScenario(); Resume(); break;
            case 1: locationManager.PlayRainyWeatherScenatio(); Resume(); break;
            case 2: locationManager.PlaySnowyWeatherScenario(); Resume(); break;
            case 3: locationManager.PlayFoggyWeatherScenario(); Resume(); break;
        }
    }
}
