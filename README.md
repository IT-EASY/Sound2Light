# RefFactory 
**Echtzeit-Audio-zu-DMX-Konverter** für professionelle Lichtsteuerungen (GrandMA, Avolites, etc.)  
*(Trigger für Cues, Helligkeitssteuerung, dynamische Effekte via sACN/artNET)*

## 🎯 Kernfunktionen
- **Eingang**: Live-Audio-Stream (ASIO/WASAPI, bevorzugt ASIO-Devices (geringere Latenz)
  Zur Entwicklung dienen Steinberg UR22C (ASIO) sowie Behringer UCA202 (WASAPI).
  AudioQuelle bei Entwicklung: DenonDJ Prime 4 (Line-Out)
- **Ausgang**: DMX-512 über **sACN** (E1.31) oder **artNET** (pro Universum 512 Kanäle).  
- **Analysetypen**:
  | Analyse                                                  | Ausgabe                        | Kanäle       |
  |----------------------------------------------------------|--------------------------------|------------- |
  | **Frequenzbänder**                                       | 4 × 0–255 (Bass, Mitten, etc.) | 4 Kanäle     |
  | **Beat/Rhythmus**  (2 unterschiedliche Berechnungsarten) | 0/255 (Binär-Trigger)          |     2        |
  | **STEM-Split**                                           | Instrumentenbasierte Beats     | 1 Kanal/STEM |
  | **BPM-Fallback**                                         | bpm bei Ausfall                | 1 Kanal      |

## 🚧 Teilziele (Milestones)
### **MVP (Minimum Viable Product)**
- [ ] **UI**: gesamtes UI, inkl. der Setup-Windows überarbeiten.  
- [ ] **ASIO/WASAPI-Capture**: Audio-Streaming von Soundkarte (UR22C/UCA202 kompatibel).  
- [ ] **Frequenzband-Analyse**: 4 Bänder → 4 DMX-Kanäle (0–255, skalierbar).  
- [ ] **sACN-Output**: DMX-Übertragung via E1.31 (Unicast/Multicast).  

### **Erweiterungen**
- [ ] **Beat Detection**: 
  - Algorithmus 1 + 2 : Generelle Takterkennung (z.B. Energy-Based).  
  - Algorithmus 3: STEM-fokussiert (Kick/Snare/Hi-Hat Isolation).  
- [ ] **MIDI-Controller-Support**: Mapping von Softpad-Controllern zur Parametereinstellung.  
- [ ] **ArtNET-Unterstützung**: Alternative zu sACN.  

## 🛠️ Technologien
- **Audio-Capture**: [NAudio](https://github.com/naudio/NAudio) (ASIO/WASAPI-Implementierung).  
- **DMX-Protokolle**: 
  - [sACN.NET](https://github.com/fiveninedigital/sACN) / [ArtNET.NET](https://github.com/hArty1/ArtNet.Net)  
- **Beat Detection**: Eigenentwicklung oder [Essentia](https://essentia.upf.edu/) (C#-Wrapper nötig).  
- **STEM-Split**: [Spleeter](https://github.com/deezer/spleeter) (ggf. als Preprocessing).  

## 🏗️ Architektur
```plaintext
RefFactory/
├── Core/
│   ├── Audio/          # CaptureDevice, FrequencyAnalyzer, BeatDetector
│   ├── Dmx/            # SacnSender, ArtNetTransmitter
│   └── Models/         # DmxUniverse, AudioDeviceConfig
├── ViewModels/         # Hauptlogik (Mapping, Echtzeit-Analyse)
├── Views/              # UI für Device-/Kanal-Konfiguration
└── AppConfig/          # JSON-Konfiguration (Universen, Kanal-Mapping)
