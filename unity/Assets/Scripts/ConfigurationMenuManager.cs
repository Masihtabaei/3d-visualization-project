using UnityEngine;

public class ConfigurationMenuManager : MonoBehaviour
{
    public static bool configurationMenuOpened = false;

    [SerializeField]
    private GameObject configurationMenuUserInterface;

    [SerializeField]
    private GameObject headUpDisplayUserInterface;

    [SerializeField]
    private UISwitcher.UISwitcher syncToggle;

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
        Time.timeScale = 1.0f;
        configurationMenuOpened = false;
        configurationMenuUserInterface.SetActive(false);
        headUpDisplayUserInterface.SetActive(true);
    }

    public void Pause()
    {
        configurationMenuUserInterface.SetActive(true);
        headUpDisplayUserInterface.SetActive(false);
        Time.timeScale = 0.0f;
        configurationMenuOpened = true;
    }

    public void LoadScenario() {
        switch (scenarios.value) {
            case 0: locationManager.PlayClearWeatherScenario(); DisableSync(); Resume(); break;
            case 1: locationManager.PlayRainyWeatherScenatio(); DisableSync(); Resume(); break;
            case 2: locationManager.PlaySnowyWeatherScenario(); DisableSync(); Resume(); break;
            case 3: locationManager.PlayFoggyWeatherScenario(); DisableSync(); Resume(); break;
        }
    }

    private void DisableSync() 
    {
        syncToggle.isOn = false;
        locationManager.StopSyncing();
        syncToggle.interactable = true;
    }

    public void EnableSync()
    {
        syncToggle.isOn = true;
        locationManager.StartSyncing();
        syncToggle.interactable = false;
    }

}
