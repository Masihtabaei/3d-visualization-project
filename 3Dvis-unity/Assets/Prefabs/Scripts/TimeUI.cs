using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TimeUI:MonoBehaviour {
    [SerializeField]
    private ToD_Base timeOfDayScript; // Referenz zum ToD_Base-Skript

    [SerializeField]
    private TMP_InputField timeInputField; // TMP_InputField f�r die Anzeige der Zeit

    [SerializeField]
    private TMP_InputField dateInputField; // TMP_InputField f�r die Anzeige des Datums

    private bool isAdjustingTime = false; // Status, ob die Zeit angepasst wird
    private bool isAdjustingDate = false; // Status, ob das Datum angepasst wird

    // �ffentliches Datum f�r Verwaltung au�erhalb des Skripts
    public System.DateTime CurrentDate { get; private set; } // �ffentlich, aber nur von innen �nderbar

    private Process pythonProcess; // Referenz zum Python-Prozess

    void Start() {
        // Sicherstellen, dass Referenzen korrekt gesetzt sind
        if (timeOfDayScript == null) {
            Debug.LogError("ToD_Base-Skript ist nicht zugewiesen!");
        }

        if (timeInputField == null) {
            Debug.LogError("TMP_InputField f�r Zeit ist nicht zugewiesen!");
        } else {
            // Event abonnieren, um Benutzereingaben zu verarbeiten
            timeInputField.onEndEdit.AddListener(OnTimeInputChanged);
        }

        if (dateInputField == null) {
            Debug.LogError("TMP_InputField f�r Datum ist nicht zugewiesen!");
        } else {
            // Event abonnieren, um Benutzereingaben zu verarbeiten
            dateInputField.onEndEdit.AddListener(OnDateInputChanged);
        }

        // Initialisiere das Datum mit dem heutigen Tag
        CurrentDate = System.DateTime.Now;
    }

    void Update() {
        // "T" dr�cken, um die Uhrzeit anzupassen
        if (Input.GetKeyDown(KeyCode.T)) {
            StartAdjustingTime();
        }

        // "Z" dr�cken, um das Datum anzupassen
        if (Input.GetKeyDown(KeyCode.Y)) {
            StartAdjustingDate();
        }

        // Wenn Zeit angepasst wird, Enter �berwachen
        if (isAdjustingTime && Input.GetKeyDown(KeyCode.Return)) {
            timeInputField.DeactivateInputField(); // Eingabe beenden
            isAdjustingTime = false; // Anpassung beenden
        }

        // Wenn Datum angepasst wird, Enter �berwachen
        if (isAdjustingDate && Input.GetKeyDown(KeyCode.Return)) {
            dateInputField.DeactivateInputField(); // Eingabe beenden
            isAdjustingDate = false; // Anpassung beenden
        }

        // Aktualisiere Zeit und Datum nur, wenn keine Anpassung l�uft
        if (!isAdjustingTime && timeOfDayScript != null && timeInputField != null) {
            int currentHour = Mathf.FloorToInt(timeOfDayScript.Get_fCurrentHour);
            int currentMinute = Mathf.FloorToInt(timeOfDayScript.Get_fCurrentMinute);
            timeInputField.text = $"{currentHour:00}:{currentMinute:00} (T)";
        }

        if (!isAdjustingDate && dateInputField != null) {
            dateInputField.text = $"{CurrentDate:dd.MM.yyyy} (Z)";
        }
    }

    private void StartAdjustingTime() {
        isAdjustingTime = true; // Status setzen
        timeInputField.text = ""; // Textfeld leeren
        timeInputField.ActivateInputField(); // Textfeld fokussieren
    }

    private void StartAdjustingDate() {
        isAdjustingDate = true; // Status setzen
        dateInputField.text = ""; // Textfeld leeren
        dateInputField.ActivateInputField(); // Textfeld fokussieren
    }

    private void OnTimeInputChanged(string input) {
        if (timeOfDayScript == null) return;

        // Standardwerte f�r Stunden und Minuten
        int hour = 0;
        int minute = 0;

        // Entferne Leerzeichen und pr�fe, ob ein Doppelpunkt vorhanden ist
        input = input.Trim();
        string[] timeParts = input.Split(':');

        if (timeParts.Length == 1) {
            if (int.TryParse(timeParts[0],out hour)) {
                minute = 0;
            } else {
                Debug.LogWarning("Ung�ltige Zeitangabe. Bitte im Format HH:MM eingeben.");
                return;
            }
        } else if (timeParts.Length == 2) {
            if (int.TryParse(timeParts[0],out hour) && int.TryParse(timeParts[1],out minute)) {
            } else {
                Debug.LogWarning("Ung�ltige Zeitangabe. Bitte im Format HH:MM eingeben.");
                return;
            }
        } else {
            Debug.LogWarning("Ung�ltige Zeitangabe. Bitte im Format HH:MM eingeben.");
            return;
        }

        timeOfDayScript.SetTime(hour,minute);
        isAdjustingTime = false;
    }

    private void OnDateInputChanged(string input) {
        input = input.Trim();
        string[] dateParts = input.Split('.');

        int currentDay = System.DateTime.Now.Day;
        int currentMonth = System.DateTime.Now.Month;
        int currentYear = System.DateTime.Now.Year;

        int day = currentDay;
        int month = currentMonth;
        int year = currentYear;

        if (dateParts.Length == 1) {
            if (int.TryParse(dateParts[0],out day)) {
            } else {
                Debug.LogWarning("Ung�ltiges Datumsformat. Bitte im Format DD.MM.YYYY eingeben.");
                return;
            }
        } else if (dateParts.Length == 2) {
            if (int.TryParse(dateParts[0],out day) && int.TryParse(dateParts[1],out month)) {
            } else {
                Debug.LogWarning("Ung�ltiges Datumsformat. Bitte im Format DD.MM.YYYY eingeben.");
                return;
            }
        } else if (dateParts.Length == 3) {
            if (int.TryParse(dateParts[0],out day) &&
                int.TryParse(dateParts[1],out month) &&
                int.TryParse(dateParts[2],out year)) {
            } else {
                Debug.LogWarning("Ung�ltiges Datumsformat. Bitte im Format DD.MM.YYYY eingeben.");
                return;
            }
        } else {
            Debug.LogWarning("Ung�ltiges Datumsformat. Bitte im Format DD.MM.YYYY eingeben.");
            return;
        }

        try {
            CurrentDate = new System.DateTime(year,month,day);
        } catch {
            Debug.LogWarning("Ung�ltiges Datum. Bitte im Format DD.MM.YYYY eingeben.");
            return;
        }

        isAdjustingDate = false;
    }




}
