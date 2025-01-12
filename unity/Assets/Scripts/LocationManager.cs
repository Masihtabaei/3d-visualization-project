using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using TMPro;
using JetBrains.Annotations;


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

    private GameObject controller;

    private bool isSynced;

    private WeatherData currentWeatherData;

    async void Start()
    {
        SetController();
        SyncDateAndTime();

        Enviro.EnviroManager.instance.ChangeCamera(Camera.allCameras[0]);
        websocket = new WebSocket("ws://localhost:8000/ws/current-weather-veste-coburg");

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
            currentWeatherData = JsonUtility.FromJson<WeatherData>(message);

        };

        await websocket.Connect();
    }

    void Update()
    {
        UpdateTimeAndDateUI();
        UpdateWeatherDataUI();
        StartCoroutine(UpdateEnvironment());

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

    private void UpdateWeatherDataUI()
    {
        temperatureField.text = "Temperature: " + currentWeatherData?.temperature;
        appearantTemperatureField.text = $"App. Temperature: {currentWeatherData?.apparent_temperature:F2}";
        rainIntensityField.text = "Rain Intensity (in mm): " + currentWeatherData?.rain;
        snowIntensityField.text = "Snow Intensity (in mm): " + currentWeatherData?.snowfall;
    }

    IEnumerator UpdateEnvironment()
    {
        yield return new WaitForSeconds(15f);
        if (currentWeatherData?.rain == 0 && currentWeatherData?.snowfall == 0)
        {
            Enviro.EnviroManager.instance.Weather.ChangeWeather("Clear Sky");
            Debug.Log("Clear Sky");
        }
        else if (currentWeatherData?.rain >= currentWeatherData?.snowfall)
        {
            Enviro.EnviroManager.instance.Weather.ChangeWeather("Rain");
            Debug.Log("Rain");
        }
        else
        {
            Enviro.EnviroManager.instance.Weather.ChangeWeather("Snow");
            Debug.Log("Snow");
        }

    }

    public void PlaySnowyWeatherScenario()
    {
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Snow");
        isSynced = false;
    }

    public void PlayRainyWeatherScenatio()
    {
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Rain");
        isSynced = false;
    }

    public void PlayFoggyWeatherScenario()
    {
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Foggy");
        isSynced = false;
    }

    public void PlayClearWeatherScenario()
    {
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Clear Sky");
        isSynced = false;
    }

    public void PlayCloudyWeatherScenario()
    {
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Cloudy 1");
        isSynced = false;
    }
}