import openmeteo_requests
import requests_cache
import pandas as pd
from retry_requests import retry


class WeatherDataFetcher:
    def __init__(self, latitude, longitude):
        self.latitude = latitude
        self.longitude = longitude

        # Setup the Open-Meteo API client with cache and retry on error
        cache_session = requests_cache.CachedSession(".cache", expire_after=3600)
        retry_session = retry(cache_session, retries=5, backoff_factor=0.2)
        self.client = openmeteo_requests.Client(session=retry_session)

        # Define API parameters
        self.params = {
            "latitude": self.latitude,
            "longitude": self.longitude,
            "hourly": [
                "temperature_2m",
                "precipitation",
                "rain",
                "showers",
                "snowfall",
                "cloud_cover_low",
                "cloud_cover_mid",
                "cloud_cover_high",
                "wind_speed_10m",
                "wind_direction_10m",
            ],
            "past_days": 1,
            "models": "icon_seamless",
        }

    def fetch_weather_data(self):
        # Request data from Open-Meteo API
        responses = self.client.weather_api(
            "https://api.open-meteo.com/v1/forecast", params=self.params
        )

        # Process first location (for multiple locations, extend this logic)
        response = responses[0]
        interval = response.Hourly()

        # Extract variables and timestamps for the DataFrame
        hourly_data = {
            "date": pd.date_range(
                start=pd.to_datetime(interval.Time(), unit="s", utc=True),
                end=pd.to_datetime(interval.TimeEnd(), unit="s", utc=True),
                freq=pd.Timedelta(seconds=interval.Interval()),
                inclusive="left",
            ),
            "temperature_2m": interval.Variables(0).ValuesAsNumpy(),
            "precipitation": interval.Variables(1).ValuesAsNumpy(),
            "rain": interval.Variables(2).ValuesAsNumpy(),
            "showers": interval.Variables(3).ValuesAsNumpy(),
            "snowfall": interval.Variables(4).ValuesAsNumpy(),
            "cloud_cover_low": interval.Variables(5).ValuesAsNumpy(),
            "cloud_cover_mid": interval.Variables(6).ValuesAsNumpy(),
            "cloud_cover_high": interval.Variables(7).ValuesAsNumpy(),
            "wind_speed_10m": interval.Variables(8).ValuesAsNumpy(),
            "wind_direction_10m": interval.Variables(9).ValuesAsNumpy(),
        }

        # Convert the dictionary to a DataFrame
        self.hourly_dataframe = pd.DataFrame(data=hourly_data)

    def get_weather_data(self):
        # Return the DataFrame with weather data
        return self.hourly_dataframe
