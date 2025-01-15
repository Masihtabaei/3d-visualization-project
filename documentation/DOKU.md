# Hochschule für angewandte Wissenschaften Coburg  
## Fakultät Elektrotechnik und Informatik  

**Vista and Vortex: EarthSync**  
*Real-Time Simulation of Environmental Data*  

**Seyedmasih Tabaei**  
**Adrian Häfner**  
**Maximilian Heß**   

**Verteilte Systeme WiSe24/25**  
**Abgabe der Arbeit:** 21. Januar 2025  

**Betreut durch:**  
Prof. Dr. Carolin Helbig  

---

## Inhaltsverzeichnis  

1. [Einleitung](#1-einleitung)  
2. [Technischer Überblick](#2-technischer-überblick)  
   2.1 [Projektstruktur und Architektur](#21-projektstruktur-und-architektur)  
   2.2 [Eingesetzte Technologien und Tools](#22-eingesetzte-technologien-und-tools)  
3. [Projektorganisation](#3-projektorganisation)  
   3.1 [Projektstrukturplan (PSP)](#31-projektstrukturplan-psp)  
   3.2 [Team und Rollenverteilung](#32-team-und-rollenverteilung)  
   3.3 [Kommunikations- und Kollaborationsplattformen](#33-kommunikations-und-kollaborationsplattformen)  
4. [Projektplan und Zeitmanagement](#4-projektplan-und-zeitmanagement)  
   4.1 [Arbeitspakete und Meilensteine](#41-arbeitspakete-und-meilensteine)  
   4.2 [Fortschrittskontrollen](#42-fortschrittskontrollen)  
   4.3 [Risikoanalyse und -management](#43-risikoanalyse-und-management)  
5. [Technische Umsetzung in Unity](#5-technische-umsetzung-in-unity)  
6. [Literaturverzeichnis](#6-literaturverzeichnis)  

---

## 1. Einleitung  

Das Ziel des Projekts *Vista and Vortex: EarthSync* ist die Entwicklung einer Unity-Anwendung, die eine interaktive 3D-Visualisierung eines geografischen Gebiets ermöglicht. Nutzer können das Gebiet mit verschiedenen Flugmodi (Drohne, Hubschrauber, Flugzeug) erkunden und gleichzeitig Live-Wetterdaten beobachten. Neben der Darstellung von Echtzeit-Wettersituationen können alternative Wetterszenarien ausgewählt oder historische Wetterdaten für ein festgelegtes Datum visualisiert werden.  

Die Anwendung soll zudem ein Zukunftsszenario integrieren, das nachhaltige Technologien wie Solarpanels visualisiert, um eine nachhaltige Stadtplanung zu unterstützen.  

### Projektumfang und Abgrenzung  

**Projektumfang:**  
- Entwicklung einer Unity-Anwendung mit drei Szenen, basierend auf einem 3D-Modell der Stadt Coburg.  
- Integration von Live-Wetterdaten durch die Open-Meteo API mit einem Python Proxy Service.  
- Benutzerinteraktionen über ein Menü, einschließlich Wetter- und Szenarioauswahl.  
- Zukunftsszenario mit ein- und ausblendbaren Solarpanels.  
- Steuerung durch Drohne, Hubschrauber oder Flugzeugperspektiven.  

**Abgrenzung:**  
- Die 3D-Modelle wurden mit der Google Earth API erstellt und nicht vollständig selbst modelliert.  
- Erweiterte Umweltszenarien wie Klimawandel-Prognosen oder detaillierte Luftqualitätsanalysen sind nicht Teil des aktuellen Projektumfangs.  

---

## 2. Technischer Überblick  

### 2.1 Projektstruktur und Architektur  

Das Projekt *Vista and Vortex: EarthSync* ist modular aufgebaut und umfasst die folgenden Hauptkomponenten:  

- **Unity-Szenen:**  
  - Drei separate Szenen repräsentieren geografische Bereiche von Coburg.  
  - Diese Szenen enthalten 3D-Modelle, die aus der Google Earth API importiert wurden, sowie zusätzliche Elemente wie Vegetation und Gebäude.  

- **API-Integration:**  
  - Live-Wetterdaten werden durch die Open-Meteo API bereitgestellt.  
  - Ein Python Proxy Service dient als Vermittler zwischen Unity und der API, um HTTP-Anfragen zu verarbeiten und Daten an die Anwendung zurückzuliefern.  

- **Benutzerinteraktion:**  
  - Über ein intuitives Menü können Benutzer zwischen Live-Wetter und vordefinierten Szenarien wählen.  
  - Es ist möglich, historische Wetterdaten für ein bestimmtes Datum abzurufen.  
  - Zukunftsszenarien, wie ein- und ausblendbare Solarpanels, sind ebenfalls implementiert.  

- **Navigation:**  
  - Der Benutzer kann das Gebiet aus verschiedenen Perspektiven erkunden:  
    - **Drohne:** Freie Navigation mit langsamer Geschwindigkeit.  
    - **Hubschrauber:** Höhere Geschwindigkeit und größere Flexibilität.  
    - **Flugzeug:** Weitreichende Bewegung für schnelle Gebietsüberflüge.  

---

### 2.2 Eingesetzte Technologien und Tools  

- **Unity Engine:** Hauptentwicklungsplattform für die 3D-Visualisierung und Interaktivität.  
- **Google Earth API:** Generierung der 3D-Modelle von Coburg.  
- **Open-Meteo API:** Bereitstellung von Wetterdaten (Temperatur, Windgeschwindigkeit, Niederschlag, etc.).  
- **Python (Flask):** Erstellung des Proxy Services zur Kommunikation zwischen Unity und der API.  
- **GitLab:** Versionskontrolle und Dokumentation des Projekts.  
- **Blender:** Optionale Nachbearbeitung der importierten 3D-Modelle.  

---

## 3. Projektorganisation  

### 3.1 Projektstrukturplan (PSP)  

| **Arbeitspaket**           | **Beschreibung**                                            | **Meilenstein**               | **Fertigstellung** |
|-----------------------------|------------------------------------------------------------|--------------------------------|--------------------|
| **AP 1: Planung**          | Zieldefinition, Anforderungen, Datenquellenrecherche       | Anforderungen abgeschlossen    | KW 40             |
| **AP 2: Architektur**      | Definition der Projektstruktur, Entwurf der API-Kommunikation | Architekturdesign fertiggestellt | KW 42             |
| **AP 3: Entwicklung**      | 3D-Modellerstellung, Integration der Wetter-API, Python Proxy Service | Funktionale Beta-Version        | KW 45             |
| **AP 4: GUI-Design**       | Erstellung des Menüs und Implementierung der Navigation     | GUI fertiggestellt             | KW 48             |
| **AP 5: Szenarien**        | Entwicklung des Zukunftsszenarios mit Solarpanels          | Szenario implementiert          | KW 50             |
| **AP 6: Tests**            | Funktionstests, Szenarientests und Performanceoptimierungen | Anwendung getestet und optimiert | KW 51             |
| **AP 7: Dokumentation**    | Fertigstellung der Projektdokumentation und Präsentation    | Dokumentation abgeschlossen     | KW 2              |  

---

### 3.2 Team und Rollenverteilung  

- **Projektleiter und Entwickler:** Koordination des Projekts, Architektur und API-Integration, Design und Implementierung des Menüs.  
- **3D-Designer:** Erstellung und Anpassung der 3D-Modelle, einschließlich der Zukunftsszenarien.  
- **Co-Entwickler/Tester:** Erstellung von initialen Assets, API-Anbindung und UI-Elementen sowie Tests.  

---

### 3.3 Kommunikations- und Kollaborationsplattformen  

- **GitLab:** Versionierung und zentrale Ablage der Projektdokumentation und des Codes.  
- **Discord/WhatsApp:** Kommunikation und regelmäßige Statusupdates.  

---

## 4. Projektplan und Zeitmanagement  

### 4.1 Arbeitspakete und Meilensteine  

Details siehe [Tabelle im Abschnitt 3.1](#31-projektstrukturplan-psp).  

### 4.2 Fortschrittskontrollen  

- Wöchentliche Team-Meetings zur Statusbesprechung.  
- Milestone-Prüfungen, um Abweichungen frühzeitig zu erkennen.  
- GitLab zur Dokumentation des Fortschritts.  

### 4.3 Risikoanalyse und -management  

| **Risiko**                 | **Wahrscheinlichkeit** | **Auswirkung** | **Strategie zur Vermeidung**       |
|----------------------------|------------------------|----------------|------------------------------------|
| Verzögerung bei API-Zugriff | Mittel                | Hoch           | Proxy-Service für Fehlerhandling   |
| Performanceprobleme         | Hoch                  | Mittel         | Optimierung der Unity-Szenen       |
| Datenqualität unzureichend  | Niedrig               | Mittel         | Zusätzliche Validierung und Quellen|  

---

## 5. Technische Umsetzung in Unity  

*(Coming Soon: Details zu Skripten und Implementierung.)*  

---

## 6. Literaturverzeichnis  

*(To be added.)*  
