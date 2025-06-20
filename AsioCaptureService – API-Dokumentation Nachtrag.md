# 📘 AsioCaptureService – API-Dokumentation (Stand: erweitert)

Diese Dokumentation beschreibt alle öffentlich verfügbaren Methoden der Klasse `AsioCaptureService` und deren Verwendung aus C# (.NET 8).

---

## 🔧 Verfügbare Methoden

| Methode                                                                         | Beschreibung                                                                   |
| ------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ |
| `bool Start(string deviceName, int bufferMultiplier)`                           | Initialisiert den ASIO-Treiber und startet den Stream                          |
| `void Stop()`                                                                   | Beendet den Stream und gibt Ressourcen frei                                    |
| `void RegisterConsumer(string name)`                                            | Registriert einen benannten Konsumenten                                        |
| `void UnregisterConsumer(string name)`                                          | Entfernt einen registrierten Konsumenten                                       |
| `bool HasNewSamplesFor(string name, int count)`                                 | Prüft, ob neue Samples für einen Konsumenten verfügbar sind                    |
| `void CopySamplesTo(string name, IntPtr destLeft, IntPtr destRight, int count)` | Kopiert Samples für einen Konsumenten                                          |
| `bool HasNewSamples(int count)`                                                 | Prüft, ob neue Samples für den Standard-Konsumenten `"default"` verfügbar sind |
| `void CopyLatestSamplesTo(IntPtr destLeft, IntPtr destRight, int count)`        | Kopiert Samples für den Standard-Konsumenten                                   |
| `IntPtr GetBufferPointerLeft()`                                                 | Gibt den Zeiger auf den linken Ringbuffer zurück                               |
| `IntPtr GetBufferPointerRight()`                                                | Gibt den Zeiger auf den rechten Ringbuffer zurück                              |
| `int GetRingBufferSize()`                                                       | Gibt die Größe des Ringbuffers zurück                                          |

---

## 🧵 Thread-Sicherheit & Architektur

- ✅ **Thread-safe**: Alle Zugriffe auf Konsumenten-Indizes sind durch `std::mutex` geschützt.
- ✅ **Lock-free**: Die eigentliche Ringbuffer-Verarbeitung (Lesen/Schreiben) erfolgt atomar und ohne Sperren.
- ✅ **Race-Condition-frei**: Durch Einführung von `UnregisterConsumer()` und Schutz der `unordered_map` wurden potenzielle Race Conditions vollständig beseitigt.
- ✅ **Kein `unsafe` in .NET**: Die gesamte Speicherverwaltung erfolgt in C++/CLI. In C# ist keine Verwendung von `unsafe` erforderlich.

---

## 🆕 Neue Methode: `UnregisterConsumer(string name)`

### Beschreibung:

Entfernt einen zuvor registrierten Konsumenten aus dem Ringbuffer-Management.

### Parameter:

- `name`: Der eindeutige Name des Konsumenten (z. B. `"VuMeter"`)

### Hinweise:

- Nach dem Entfernen ist ein Zugriff über `HasNewSamplesFor(...)` oder `CopySamplesTo(...)` mit dem gleichen Namen nicht mehr gültig.
- Sollte aufgerufen werden, wenn ein Konsument nicht mehr benötigt wird (z. B. bei UI-Schließung oder Plugin-Deaktivierung).

#### Beispiel:

---

## 🧠 Best Practices

- Verwende `RegisterConsumer(...)` beim Start eines Moduls (z. B. Visualisierung).
- Verwende `UnregisterConsumer(...)` beim Beenden oder Deaktivieren.
- Nutze `HasNewSamplesFor(...)` und `CopySamplesTo(...)` nur für registrierte Konsumenten.
- Für einfache Anwendungen genügt der `"default"`-Konsument mit `HasNewSamples(...)` und `CopyLatestSamplesTo(...)`.
