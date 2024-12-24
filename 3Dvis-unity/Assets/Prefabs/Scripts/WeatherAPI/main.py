import json
import sys
from datetime import datetime
from weatherAPI import WeatherDataFetcher


def main():
    # Prüfen, ob Datum und Uhrzeit als Argumente übergeben wurden
    if len(sys.argv) < 3:
        print("Fehlende Argumente. Bitte Datum und Uhrzeit übergeben.")
        return

    # Datum und Uhrzeit aus den Argumenten extrahieren
    date_str = sys.argv[1]  # Beispiel: 05.12.2024
    time_str = sys.argv[2]  # Beispiel: 15:00

    # Konvertiere Datum und Uhrzeit in das gewünschte Format
    try:
        # Kombiniere Datum und Uhrzeit in ein datetime-Objekt
        date_time_str = f"{date_str} {time_str}"
        date_time_obj = datetime.strptime(date_time_str, "%d.%m.%Y %H:%M")
        print(f"Verwendetes Datum und Uhrzeit: {date_time_obj}")
    except ValueError as e:
        print(f"Fehler beim Parsen von Datum und Uhrzeit: {e}")
        return

    # Create WeatherDataFetcher objects for different locations
    locations = [
        {"name": "Coburg Veste", "latitude": 50.26402784275447, "longitude": 10.981332952461136},
        {"name": "Coburg HS", "latitude": 50.265359270315294, "longitude": 10.95340779077337},
        {"name": "Coburg Markt", "latitude": 50.25831190605361, "longitude": 10.964617036743629},
    ]

    # A list to store the data from all locations
    all_weather_data = []

    for location in locations:
        # Create and use the WeatherDataFetcher object for each location
        weather_fetcher = WeatherDataFetcher(
            latitude=location["latitude"], longitude=location["longitude"]
        )

        # Retrieve weather data
        weather_fetcher.fetch_weather_data()

        # Receive weather data as a DataFrame and convert it to a JSON compatible format
        weather_data_json = json.loads(
            weather_fetcher.get_weather_data().to_json(orient="records", date_format="iso")
        )

        # Add the location name and dates to the list
        all_weather_data.append(
            {
                "location": location["name"],
                "latitude": location["latitude"],
                "longitude": location["longitude"],
                "date_time": date_time_obj.isoformat(),  # Include the date_time in the result
                "data": weather_data_json,
            }
        )

    # Save all weather data in a JSON file
    json_filename = "weather_data_coburg.json"
    with open(json_filename, "w") as json_file:
        json.dump(all_weather_data, json_file, indent=4)  # Better readable JSON file

    print(f"Wetterdaten wurden erfolgreich als '{json_filename}' gespeichert.")


if __name__ == "__main__":
    main()
