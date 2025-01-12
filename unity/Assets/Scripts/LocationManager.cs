using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using TMPro;


public class LocationManager : MonoBehaviour
{
    private WebSocket websocket;

    [SerializeField]
    private TextMeshProUGUI temperatureField;

    [SerializeField]
    private TextMeshProUGUI appearantTemperatureField;

    [SerializeField]
    private TextMeshProUGUI rainIntensityField;

    [SerializeField]
    private TextMeshProUGUI snowIntensityField;

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8000/ws/weather");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection opened successfully!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed successfully!");
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            //Debug.Log("Message Received: " + message);
            WeatherData currentWeatherData = JsonUtility.FromJson<WeatherData>(message);
            temperatureField.text = "Temperature: " + currentWeatherData.temperature;
            appearantTemperatureField.text = $"Apparent Temperature: {currentWeatherData.apparent_temperature:F2}";
            rainIntensityField.text = "Rain Intensity: " + currentWeatherData.rain;
            snowIntensityField.text = "Snow Intensity: " + currentWeatherData.snowfall;
        };


        // waiting for messages
        await websocket.Connect();
        StartCoroutine(MyCoroutine());

    }

    void Update()
    {
       
        #if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
        #endif
    }


    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    IEnumerator MyCoroutine()
    {
        yield return new WaitForSeconds(20f);
        Enviro.EnviroManager.instance.Weather.ChangeWeather("Rain");
        Debug.Log("Weather Changed");
        //code here will execute after 5 seconds
    }

}