import tomli

from fastapi import FastAPI, WebSocket
import asyncio

import openmeteo_requests
import requests_cache
from retry_requests import retry


app = FastAPI()

def retrieve_service_information():
    with open("pyproject.toml", mode="rb") as config:
        toml_file = tomli.load(config)
        return {
            'service name': toml_file['project']['name'],
            'service version': toml_file['project']['version'],
            'service description': toml_file['project']['description']
        }

def retrieve_current_weather_data(latitude, longitude):
    cache_session = requests_cache.CachedSession('.cache', expire_after=3600)
    retry_session = retry(cache_session, retries=5, backoff_factor=0.2)
    openmeteo_handle = openmeteo_requests.Client(session=retry_session)
    url = "https://api.open-meteo.com/v1/forecast"
    params = {
	    "latitude": latitude,
	    "longitude": longitude,
	    "current": ["temperature_2m", "apparent_temperature", "rain", "snowfall", "is_day"]
    }
    responses = openmeteo_handle.weather_api(url, params=params)
    response = responses[0]

    retrieval_result = {
        "latitude": response.Latitude(),
        "longitude": response.Longitude(),
        "temperature": response.Current().Variables(0).Value(),
        "apparent_temperature": response.Current().Variables(1).Value(),
        "rain": response.Current().Variables(2).Value(),
        "snowfall": response.Current().Variables(3).Value(),
        "is_day": response.Current().Variables(4).Value()
    }
    print(retrieval_result)
    return retrieval_result

@app.get("/")
def get_root():
    return retrieve_service_information()

@app.get("/ping")
def get_ping():
    return "I am a banana!"

@app.get("/current-weather-coburg-university")
def get_current_weather_marktplatz_coburg():
    return retrieve_current_weather_data(50.264805843873596, 10.95194081317186)

@app.get("/current-weather-marketplace-coburg")
def get_current_weather_veste_coburg():
    return retrieve_current_weather_data(50.258344418260336, 10.964638398120213)

@app.get("/current-weather-veste-coburg")
def get_current_weather_marktplatz_coburg():
    return retrieve_current_weather_data(50.26393447177281, 10.98372942394643)

@app.websocket("/ws/current-weather-coburg-university")
async def weather_websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    try:
        while True:
            current_weather_data = retrieve_current_weather_data(50.264805843873596, 10.95194081317186)
            await websocket.send_json(current_weather_data)
            await asyncio.sleep(15)
    except Exception as exception:
        print(f"Excpetion: {exception}")
    finally:
        await websocket.close()


@app.websocket("/ws/current-weather-marketplace-coburg")
async def weather_websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    try:
        while True:
            current_weather_data = retrieve_current_weather_data(50.258344418260336, 10.964638398120213)
            await websocket.send_json(current_weather_data)
            await asyncio.sleep(15)
    except Exception as exception:
        print(f"Excpetion: {exception}")
    finally:
        await websocket.close()

@app.websocket("/ws/current-weather-veste-coburg")
async def weather_websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    try:
        while True:
            current_weather_data = retrieve_current_weather_data(50.26393447177281, 10.98372942394643)
            await websocket.send_json(current_weather_data)
            await asyncio.sleep(15)
    except Exception as exception:
        print(f"Excpetion: {exception}")
    finally:
        await websocket.close()