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

def retrieve_historical_weather_data(latitude, longitude, year, month, day, hour):

        cache_session = requests_cache.CachedSession('.cache', expire_after=3600)
        retry_session = retry(cache_session, retries=5, backoff_factor=0.2)

        base_url = "https://archive-api.open-meteo.com/v1/archive"
        date = f"{year:04d}-{month:02d}-{day:02d}"
        params = {
            "latitude": latitude,
            "longitude": longitude,
            "start_date": date,
            "end_date": date,
            "hourly": "temperature_2m,apparent_temperature,precipitation,snowfall,is_day",
        }

        response = retry_session.get(base_url, params=params)
        response.raise_for_status()

        data = response.json()

        if "hourly" not in data:
            raise ValueError("No weather data found for the given date and location")

        hourly_data = data["hourly"]
        index = hour

        retrieval_result = {
            "latitude": latitude,
            "longitude": longitude,
            "temperature": hourly_data["temperature_2m"][index],
            "apparent_temperature": hourly_data["apparent_temperature"][index],
            "rain": hourly_data["precipitation"][index],
            "snowfall": hourly_data["snowfall"][index],
            "is_day": hourly_data["is_day"][index]
        }

        print(retrieval_result)
        return retrieval_result

@app.get("/")
def get_root():
    return retrieve_service_information()

@app.get("/ping")
def get_ping():
    return "I am a banana!"

@app.get("/weather/current/coburg-university")
def get_current_weather_coburg_university():
    return retrieve_current_weather_data(50.26590798239408, 10.951368715795986)

@app.get("/weather/historical/coburg-university")
def get_historical_weather_coburg_university(year: int, month: int, day: int, hour: int):
    return retrieve_historical_weather_data(50.26590798239408, 10.951368715795986, year, month, day, hour)

@app.get("/weather/current/marketplace-coburg")
def get_current_weather_marketplace_coburg():
    return retrieve_current_weather_data(50.25843121615498, 10.964631168395876)

@app.get("/weather/historical/marketplace-coburg")
def get_historical_weather_marketplace_coburg(year: int, month: int, day: int, hour: int):
    return retrieve_historical_weather_data(50.25843121615498, 10.964631168395876, year, month, day, hour)

@app.get("/weather/current/veste-coburg")
def get_current_weather_veste_coburg():
    return retrieve_current_weather_data(50.264080161801026, 10.983774111798)

@app.get("/weather/historical/veste-coburg")
def get_historical_weather_veste_coburg(year: int, month: int, day: int, hour: int):
    return retrieve_historical_weather_data(50.264080161801026, 10.983774111798, year, month, day, hour)


@app.websocket("/ws/weather/current/coburg-university")
async def weather_websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    try:
        while True:
            current_weather_data = retrieve_current_weather_data(50.26590798239408, 10.951368715795986)
            await websocket.send_json(current_weather_data)
            await asyncio.sleep(15)
    except Exception as exception:
        print(f"Excpetion: {exception}")
    finally:
        await websocket.close()


@app.websocket("/ws/weather/current/marketplace-coburg")
async def weather_websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    try:
        while True:
            current_weather_data = retrieve_current_weather_data(50.25843121615498, 10.964631168395876)
            await websocket.send_json(current_weather_data)
            await asyncio.sleep(15)
    except Exception as exception:
        print(f"Excpetion: {exception}")
    finally:
        await websocket.close()

@app.websocket("/ws/weather/current/veste-coburg")
async def weather_websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    try:
        while True:
            current_weather_data = retrieve_current_weather_data(50.264080161801026, 10.983774111798)
            await websocket.send_json(current_weather_data)
            await asyncio.sleep(15)
    except Exception as exception:
        print(f"Excpetion: {exception}")
    finally:
        await websocket.close()