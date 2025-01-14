using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuManager : MonoBehaviour
{
    public static bool configurationMenuOpened = false;

    [SerializeField]
    private GameObject configurationMenuUserInterface;

    [SerializeField]
    private GameObject headUpDisplayUserInterface;

    [SerializeField]
    private TMPro.TMP_Dropdown scenarios;

    [SerializeField]
    private Toggle solarPanelToggle;

    [SerializeField]
    private Toggle sceneSyncToggle;

    [SerializeField]
    private GameObject solarPanelGroups;

    [SerializeField]
    private GameObject historicalDataSection;

    [SerializeField]
    private TMPro.TMP_InputField yearInputField;

    [SerializeField]
    private TMPro.TMP_InputField monthInputField;

    [SerializeField]
    private TMPro.TMP_InputField dayInputField;

    [SerializeField]
    private TMPro.TMP_InputField hourInputField;

    [SerializeField]
    private LocationManager locationManager;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if(!PauseMenuController.pauseMenuOpened)
            {
                if (!historicalDataSection.activeInHierarchy)
                {
                    if (configurationMenuOpened)
                        Resume();
                    else
                        Pause();
                }

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

    public void HandleSolarPanelToggle()
    {
        if (solarPanelToggle.isOn)
            solarPanelGroups.SetActive(true);
        else
            solarPanelGroups.SetActive(false);
    }

    public void HandleSyncToggle()
    { 
        if(sceneSyncToggle.isOn)
            EnableSync();
        else
            DisableSync();
    }
    private void DisableSync() 
    {
        locationManager.StopSyncing();
        sceneSyncToggle.isOn = false;
    }

    public void EnableSync()
    {
        locationManager.StartSyncing();
        sceneSyncToggle.isOn = true;
    }

    public void LoadHistoricalData()
    {
        locationManager.LoadHistoricalData(
            int.Parse(yearInputField.text),
            int.Parse(monthInputField.text),
            int.Parse(dayInputField.text),
            int.Parse(hourInputField.text)
        );
    }

}
