# UI Design Specification – Sound2Light

## 🎨 **Design-Prinzipien**
- **Optische Anlehnung**: Professionelle Hardware-Ästhetik (Rack-Unit, Industrie-Design).  
- **Modularität**: UI besteht aus wiederverwendbaren **Units** mit einheitlicher Struktur.  
- **Konsistenz**: Alle Units folgen strikten Layout-Regeln.  

---

## 🧩 **Unit-Struktur**
### **Grid-Layout (3×3)**
Jede Unit ist in ein 3×3-Grid unterteilt:  
| Position                   | Inhalt                           | Größe       |  
|----------------------------|----------------------------------|-------------|  
| (0,0), (0,2), (2,0), (2,2) | Dekorative Kreuzschlitzschrauben | 30px × 30px |  
| (1,1)                      | **Hauptinhalt** der Unit         | Flexibel    |  

![Unit-Layout](https://via.placeholder.com/300x300.png?text=Unit+Layout+3x3+Grid)  
*(Hinweis: Platzhalter-Bild – finales Mockup einfügen)*

### **Schrauben-Dekoration**
- als WPF erstellt und per ausgelagerter Funktion randomsiert gedreht
 
### **Rahmen**  
- Jede Unit hat einen 2px-Rahmen (`#404040`) mit `CornerRadius=4`.  

---

## 🕹️ **Bedienelemente**
### **Buttons (Schalter/Tasten)**
- **Optik**:  
  - Plastischer 3D-Effekt (simulierte Beleuchtung, Schlagschatten).  
  - **Zustandsfarben**:  
    | Zustand | Farbe (HEX) | Glow-Effekt |  
    |---------|-------------|-------------|  
    | **ON**  | `#00FF00`   | Ja          |  
    | **OFF** | `#FF0000`   | Nein        |  
  - **Label**: Klare Beschriftung (z.B. "ARM", "MUTE").  

- **Interaktion**:  
  - Keine Slider/Drehknöpfe – ausschließlich **binäre Schalter**.  

### **Beispiel-Button (ON/OFF-Zustand)**  
![Button-Design](https://via.placeholder.com/100x40.png?text=ON/OFF+Button)  

---

## 💡 **Anzeige-Elemente**
### **LEDs**
- **Form**:  
  | Typ          | Form       | Größe (Beispiel) |  
  |--------------|------------|------------------|  
  | Status-LED   | Rund       | 20px × 20px      |  
  | VU-Meter     | Rechteckig | 30px × 5px       |  

- **Effekte**:  
  - **Glow**: Simuliert aktive Beleuchtung (z.B. `DropShadowEffect` in WPF).  
  - **Stapelung** (VU-Meter): Mehrere rechteckige LEDs vertikal/horizontal angeordnet.  

### **Beispiel-LEDs**  
![LEDs](https://via.placeholder.com/100x50.png?text=Status-LEDs+und+VU-Meter)  

---

## 🗂️ **Setup- und Konfigurationsfenster**
- **Zugriff**: Über dedizierte Buttons in den Units (z.B. "⚙"-Icon).  
- **Optik**:  
  - Eigenes Window
  - Enthält Formulare für Geräteauswahl, DMX-Mapping usw.  

---

## 🎨 **Farbpalette & Styling**
| Element          | Farbe (HEX) | Notizen                  |  
|------------------|-------------|--------------------------|  
| **Hintergrund**  | `#333333`   | Dunkelgrau               |  
| **Rahmen**       | `#404040`   | Mittelgrau               |  
| **Text**         | `#FFFFFF`   | Weiß                     |  
| **ON-Status**    | `#00FF00`   | Grün (mit Glow)          |  
| **OFF-Status**   | `#FF0000`   | Rot (ohne Glow)          |  

---

## 🚀 **Nächste Schritte**  
1. **Mockups finalisieren**: Für alle Units (Device-Auswahl, DMX-Output, Beat-Detection).  
2. **Assets erstellen**: PNGs für Schrauben, Buttons, LEDs.  
3. **Styleguide umsetzen**: Globale WPF-Ressourcen (`Brushes`, `Styles`, `ControlTemplates`).  

---

**Letzte Änderung**: 20.10.2023  
**Verantwortlich**: [@IT-EASY](https://github.com/IT-EASY)  
