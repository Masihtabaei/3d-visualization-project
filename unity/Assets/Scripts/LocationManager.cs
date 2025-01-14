using System;
using System.Collections;
using UnityEngine;

using NativeWebSocket;
using TMPro;
using UnityEngine.Networking;


public class LocationManager : MonoBehaviour
{
    private WebSocket websocket;

    [SerializeField]
    private TextMeshProUGUI connectionStatusField;

    [SerializeField]
    private TextMeshProUGUI temperatureField;

    [SerializeField]
    private TextMeshProUGUI appearantTemperatureField;

    [SerializeField]
    private TextMeshProUGUI rainIntensityField;

    [SerializeField]
    private TextMeshProUGUI snowIntensityField;

    [SerializeField]
    private TextMeshProUGUI syncStatusField;

    [SerializeField]
    private TextMeshProUGUI dateField;

    [SerializeField]
    private TextMeshProUGUI timeField;

    [SerializeField]
    private GameObject[] controllers;

    [SerializeField]
    private TextMeshProUGUI historicalDataFetchResult;

    [SerializeField]
    private String websocketAddress;

    [SerializeField]
    private String restServerAddress;

    private GameObject controller;

    private bool isSynced;

    private WeatherData currentWeatherData;

    async void Start()
    {
        SetController();
        SyncDateAndTime();

        Enviro.EnviroManager.instance.ChangeCamera(Camera.allCameras[0]);
        websocket = new WebSocket(websocketAddress);

        websocket.OnOpen += () =>
        {
            connectionStatusField.text = "Connected: Yes";
        };

        websocket.OnError += (e) =>
        {
            connectionStatusField.text = "Connected: No (with an error)";
        };

        websocket.OnClose += (e) =>
        {
            connectionStatusField.text = "Connected: No";
        };

        websocket.OnMessage += (bytes) =>
        {
            String message = System.Text.Encoding.UTF8.GetString(bytes);
            if (isSynced)
                currentWeatherData = JsonUtility.FromJson<WeatherData>(message);

        };

        await websocket.Connect();
    }

    void Update()
    {
        if (isSynced)
        {
            UpdateWeatherDataUI();
            UpdateTimeAndDateUI();
            UpdateEnvironment();

        }

#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
        #endif
    }


    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    private void SetController()
    {
        controller = controllers[CrossSceneInformation.controllerIndex];
        controller.SetActive(true);
    }
    private void SyncDateAndTime()
    {
        System.DateTime now = System.DateTime.Now;

        int year = now.Year;
        int month = now.Month;
        int day = now.Day;
        int hour = now.Hour;
        int minute = now.Minute;
        int second = now.Second;

        Enviro.EnviroManager.instance.Time.seconds = second;
        Enviro.EnviroManager.instance.Time.minutes = minute;
        Enviro.EnviroManager.instance.Time.hours = hour;
        Enviro.EnviroManager.instance.Time.days = day;
        Enviro.EnviroManager.instance.Time.months = month;
        Enviro.EnviroManager.instance.Time.years = year;

        isSynced = true;
    }

    private void UpdateDateAndTime(int year, int month, int day, int hour)
    {
        Enviro.EnviroManager.instance.Time.seconds = 0;
        Enviro.EnviroManager.instance.Time.minutes = 0;
        Enviro.EnviroManager.instance.Time.hours = hour;
        Enviro.EnviroManager.instance.Time.days = day;
        Enviro.EnviroManager.instance.Time.months = month;
        Enviro.EnviroManager.instance.Time.years = year;
    }

    private void UpdateTimeAndDateUI()
    {
        string currentDate = 
            Enviro.EnviroManager.instance.Time.days
            + "."
            + Enviro.EnviroManager.instance.Time.months 
            + "."
            + Enviro.EnviroManager.instance.Time.years
            ;
        String currentTime = Enviro.EnviroManager.instance.Time.GetTimeStringWithSeconds();

        syncStatusField.text = "Synced: " + (isSynced ? "Yes" : "No");
        dateField.text = "Date: " + currentDate;
        timeField.text = "Time: " + currentTime;
    }

    public void UpdateTimeAndDateUI(int year, int month, int day, int hour)
    {
        syncStatusField.text = "Synced: " + (isSynced ? "Yes" : "No");
        dateField.text = "Date: " + year + "." + month + "." + day;
        timeField.text = "Time: " + hour + ":" + "00:00";
    }

    private void UpdateWeatherDataUI()
    {
        temperatureField.text = "Temperature: " + currentWeatherData?.temperature;
        appearantTemperatureField.text = $"App. Temperature: {currentWeatherData?.apparent_temperature:F2}";
        rainIntensityField.text = "Rain Intensity (in mm): " + currentWeatherData?.rain;
        snowIntensityField.text = "Snow Intensity (in mm): " + currentWeatherData?.snowfall;
    }

    public void UpdateEnvironment()
    {
        if (currentWeatherData?.rain == 0 && currentWeatherData?.snowfall == 0)
        {
            Enviro.EnviroManager.instance.Weather.ChangeWeather("Clear Sky");
        }
        else if (currentWeatherData?.rain >= currentWeatherData?.snowfall)
        {
            Enviro.EnviroManager.instance.Weather.ChangeWeather("Rain");
        }
        else if (currentWeatherData?.snowfall > currentWeatherData?.rain)
        {
            Enviro.EnviroManager.instance.Weather.ChangeWeather("Snow");
        }
    }
    public void PlaySnowyWeatherScenario()
    {
        StopSyncing();
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Snow");
    }

    public void PlayRainyWeatherScenatio()
    {
        StopSyncing();
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Rain");
    }

    public void PlayFoggyWeatherScenario()
    {
        StopSyncing();
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Foggy");
    }

    public void PlayClearWeatherScenario()
    {
        StopSyncing();
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Clear Sky");
    }

    public void PlayCloudyWeatherScenario()
    {
        StopSyncing();
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Cloudy 1");
    }

    public void StartSyncing() 
    {
        Enviro.EnviroManager.instance.Time.Settings.simulate = true;
        isSynced = true;
    }
    public void StopSyncing()
    {
        Enviro.EnviroManager.instance.Time.Settings.simulate = false;
        isSynced = false;
        UpdateTimeAndDateUI();
    }

    public void LoadHistoricalData(int year, int month, int day, int hour)
    {
        string url = $"{restServerAddress}?year={year}&month={month:D2}&day={day:D2}&hour={hour:D2}";
        if (year < 2000 || year > 2024)
        {
            historicalDataFetchResult.text = "Invalid Year!";
            return;
        }
        if (month < 1 || month > 12)
        {
            historicalDataFetchResult.text = "Invalid Month!";
            return;
        }
        if (day < 1 || day > 31)
        {
            historicalDataFetchResult.text = "Invalid Day!";
            return;
        }
        if (hour < 0 || hour > 23)
        {
            historicalDataFetchResult.text = "Invalid Hour!";
            return;
        }

        historicalDataFetchResult.text = "";
        StartCoroutine(GetRequest(url, year, month, day, hour));
    }
    IEnumerator GetRequest(string uri, int year, int month, int day, int hour)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            historicalDataFetchResult.text = "Error: " + uwr.error;
        }
        else
        {
            String resultAsString = uwr.downloadHandler.text;
            historicalDataFetchResult.text = "Successfully Fetched!";
            currentWeatherData = JsonUtility.FromJson<WeatherData>(resultAsString);
            StopSyncing();
            UpdateEnvironment();
            UpdateDateAndTime(year, month, day, hour);
            UpdateTimeAndDateUI(year, month, day, hour);
            UpdateWeatherDataUI();
        }
    }
}