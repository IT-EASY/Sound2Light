# 📘 AsioCaptureService – API-Dokumentation

Diese Dokumentation beschreibt alle öffentlich verfügbaren Methoden der Klasse `AsioCaptureService` und deren Verwendung aus C# (.NET 8).

## 🔧 bool Start(string deviceName, int bufferMultiplier)

### Beschreibung:

Initialisiert den ASIO-Treiber und startet den Audio-Stream.

### Parameter:

- `deviceName`: Name des ASIO-Geräts (z. B. `"Yamaha Steinberg USB ASIO"`).
- `bufferMultiplier`: Multiplikator für die Ringbuffer-Größe.

### Rückgabewert:

- `true`, wenn erfolgreich gestartet.
- `false`, bei Fehler.

#### Beispiel:

```
var service = new AsioCaptureService();

bool success = service.Start("Yamaha St</sub>einberg USB ASIO", 8);
```

## 🔧 void Stop()

### Beschreibung:

Beendet den ASIO-Stream und gibt Ressourcen frei.

#### Beispiel:

```Input
service.Stop();
```

---

## 🔧 void RegisterConsumer(string name)

### Beschreibung:

Registriert einen Konsumenten für den Ringbuffer.

### Parameter:

- `name`: Eindeutiger Name des Konsumenten (z. B. `"VuMeter"`).

#### Beispiel:

```Langueage
service.RegisterConsumer("VuMeter");
```

---

## 🔧 bool HasNewSamplesFor(string name, int count)

### Beschreibung:

Prüft, ob für den angegebenen Konsumenten mindestens `count` neue Samples verfügbar sind.

#### Beispiel:

```Langueage
bool ready = service.HasNewSamplesFor("VuMeter", 512);
```

---

## 🔧 void CopySamplesTo(string name, IntPtr destLeft, IntPtr destRight, int count)

### Beschreibung:

Kopiert `count` Samples aus dem Ringbuffer für den angegebenen Konsumenten in die Zielpuffer.

#### Beispiel:

```Language
float[] left = new float[512];
float[] right = new float[512];
GCHandle hLeft = GCHandle.Alloc(left, GCHandleType.Pinned);
GCHandle hRight = GCHandle.Alloc(right, GCHandleType.Pinned);

service.CopySamplesTo("VuMeter", hLeft.AddrOfPinnedObject(), hRight.AddrOfPinnedObject(), 512);

hLeft.Free();
hRight.Free();
```

---

## 🔧 IntPtr GetBufferPointerLeft()

### Beschreibung:

Gibt den Zeiger auf den linken Ringbuffer zurück.

#### Beispiel:

```Language
IntPtr leftPtr = service.GetBufferPointerLeft();
```

---

## 🔧 IntPtr GetBufferPointerRight()

### Beschreibung:

Gibt den Zeiger auf den rechten Ringbuffer zurück.

#### Beispiel:

```Language
IntPtr rightPtr = service.GetBufferPointerRight();
```

---

## 🔧 int GetRingBufferSize()

### Beschreibung:

Gibt die Größe des Ringbuffers (pro Kanal) zurück.

---

## 🔧 void CopyLatestSamplesTo(IntPtr destLeft, IntPtr destRight, int count)

### Beschreibung:

Kopiert die neuesten Samples für den **Standard-Konsumenten** `"default"`.

## 🔧 bool HasNewSamples(int count)

### Beschreibung:

Prüft, ob für den Standard-Konsumenten `"default"` neue Samples verfügbar sind.

---
