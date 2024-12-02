import json
from weatherAPI import WeatherDataFetcher


def main():
    # Erstelle WeatherDataFetcher-Objekte für verschiedene Orte
    locations = [
        {"name": "Coburg Veste", "latitude": 50.26402784275447, "longitude": 10.981332952461136},
        {"name": "Coburg HS", "latitude": 50.265359270315294, "longitude": 10.95340779077337},
        {"name": "Coburg Markt", "latitude": 50.25831190605361, "longitude": 10.964617036743629},
    ]

    # Eine Liste zum Speichern der Daten von allen Standorten
    all_weather_data = []

    for location in locations:
        # Erstelle und verwende das WeatherDataFetcher-Objekt für jeden Standort
        weather_fetcher = WeatherDataFetcher(
            latitude=location["latitude"], longitude=location["longitude"]
        )

        # Wetterdaten abrufen
        weather_fetcher.fetch_weather_data()

        # Wetterdaten als DataFrame erhalten und in ein JSON-kompatibles Format umwandeln
        weather_data_json = json.loads(
            weather_fetcher.get_weather_data().to_json(orient="records", date_format="iso")
        )

        # Füge den Standortnamen und die Daten zur Liste hinzu
        all_weather_data.append(
            {
                "location": location["name"],
                "latitude": location["latitude"],
                "longitude": location["longitude"],
                "data": weather_data_json,
            }
        )

    # Speichere alle Wetterdaten in einer JSON-Datei
    json_filename = "../weather_data_coburg.json"
    with open(json_filename, "w") as json_file:
        json.dump(all_weather_data, json_file, indent=4)  # Besser lesbare JSON-Datei

    print(f"Wetterdaten wurden erfolgreich als '{json_filename}' gespeichert.")


if __name__ == "__main__":
    main()
