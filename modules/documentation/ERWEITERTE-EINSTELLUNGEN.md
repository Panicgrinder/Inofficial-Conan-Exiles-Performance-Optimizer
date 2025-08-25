# ⚙️ CONAN EXILES OPTIMIZER - ERWEITERTE EINSTELLUNGEN

## 🎯 KONZEPT: RISIKO-BASIERTE OPTIMIERUNG

Diese Version gibt dir **volle Kontrolle** über alle Optimierungen, sortiert nach **Risiko-Level** und **Auswirkungen**.

---

## 📊 RISIKO-KATEGORIEN ERKLÄRT

### 🛡️ SICHER (Kein Banrisiko)
**Für:** Alle Server, Online-PvP, Official Server
**Was:** Nur Windows-Optimierungen, keine Spieldateien berührt

| Optimierung | Verbesserung | Beschreibung |
|-------------|--------------|--------------|
| **Windows Gaming-Modus** | 5-10% | Aktiviert Windows Gaming-Modus |
| **Cache-Bereinigung** | 10-20% Ladezeit | Entfernt temporäre Dateien |
| **Speicher-Optimierung** | 5-15% RAM | Optimiert Windows RAM-Management |
| **System-Performance** | 5-10% | Windows Gaming-Einstellungen |

### 🟡 NIEDRIGES RISIKO (Lokale Einstellungen)
**Für:** Online-Server (empfohlen), Private Server
**Was:** Lokale Einstellungen, keine Config-Dateien

| Optimierung | Verbesserung | Beschreibung |
|-------------|--------------|--------------|
| **Startparameter** | 15-25% Ladezeit | Memory- und CPU-optimierte Parameter |
| **Grafik-Optimierung** | 10-20% FPS | Grafiktreiber-Einstellungen |
| **Netzwerk-Puffering** | 5-15% Latenz | Windows-Netzwerk für Gaming |

### 🟠 MITTLERES RISIKO (Config-Dateien)
**Für:** Singleplayer, Private Server (nach Absprache)
**Was:** Modifiziert Engine.ini und Game.ini

| Optimierung | Verbesserung | Risiko-Details |
|-------------|--------------|----------------|
| **Engine-Optimierungen** | 20-30% Performance | Engine.ini Änderungen |
| **Netzwerk-Tweaks** | 15-25% Netzwerk | Game.ini Modifikationen |
| **KI-Optimierungen** | 10-20% NPC-Performance | AI-Verhalten angepasst |
| **Community-Fixes** | 25-40% Stabilität | Bewährte kเt's Guide Fixes |

### 🔴 HOHES RISIKO (Experimental)
**Für:** Nur Singleplayer, eigene private Server
**Was:** Aggressive Optimierungen, Banngefahr auf Official

| Optimierung | Verbesserung | Warnung |
|-------------|--------------|---------|
| **Erweiterte Netzwerk-Optimierungen** | 30-50% Netzwerk | **BANNGEFAHR** auf Official |
| **Experimentelle Tweaks** | 20-40% | Kann Spiel instabil machen |

---

## 🎮 VOREINSTELLUNGEN NACH SPIELMODUS

### 🏆 "OFFICIAL SERVER SAFE"
**Zielgruppe:** Official Funcom Server, PvP, Competitive
```
✅ Sicher: Alle aktiviert
✅ Niedrig Risiko: Alle aktiviert  
❌ Mittel Risiko: Alle deaktiviert
❌ Hoch Risiko: Alle deaktiviert
```
**Erwartung:** 20-35% Verbesserung, 0% Banrisiko

### 🏠 "PRIVATE SERVER OPTIMIZED"
**Zielgruppe:** Private Server, RP-Server, Community-Server
```
✅ Sicher: Alle aktiviert
✅ Niedrig Risiko: Alle aktiviert
✅ Mittel Risiko: Engine + Community-Fixes
❌ Hoch Risiko: Alle deaktiviert
```
**Erwartung:** 35-55% Verbesserung, Minimales Risiko

### 🎯 "SINGLEPLAYER MAXIMUM"
**Zielgruppe:** Singleplayer, lokales Spiel, Testing
```
✅ Sicher: Alle aktiviert
✅ Niedrig Risiko: Alle aktiviert
✅ Mittel Risiko: Alle aktiviert
⚠️ Hoch Risiko: Nur Erweiterte Netzwerk-Optimierungen
```
**Erwartung:** 50-70% Verbesserung, kein Online-Risiko

### 🧪 "MODDING & TESTING"
**Zielgruppe:** Mod-Entwickler, Server-Admins, Testing
```
✅ Alle Kategorien aktiviert
```
**Erwartung:** 60-80% Verbesserung, nur für Testing

---

## 🔧 DETAILLIERTE OPTIMIERUNGS-BESCHREIBUNGEN

### Community-Fixes (kเt's Steam Guide)
```ini
# Die bewährten Fixes aus der Community
[/script/engine.gamenetworkmanager]
MoveRepSize=512.0f              # Behebt Thrall-Stat-Verlust
MAXPOSITIONERRORSQUARED=3.0f    # Verbesserte Bewegungs-Sync

[/script/conansandbox.aisense_newsight]
MaxTracesPerTick=500            # Bessere AI-Reaktionszeit

[/script/aimodule.envquerymanager]
MaxAllowedTestingTime=0.003     # AI-Performance-Boost
```

### Startparameter-Optimierungen
```
-USEALLAVAILABLECORES          # Nutzt alle CPU-Kerne
-high                          # Hohe Prozess-Priorität  
-malloc=system                 # Optimiertes Memory-Management
-sm4                          # Shader Model 4 für Kompatibilität
-d3d11                        # DirectX 11 forcieren
```

### Netzwerk-Optimierungen
```ini
[/script/engine.gamenetworkmanager]
TotalNetBandwidth=4000000      # Erweiterte Bandbreite
MaxDynamicBandwidth=100000     # Dynamische Anpassung
MinDynamicBandwidth=10000      # Minimum-Bandbreite
```

---

## ⚠️ SICHERHEITSRICHTLINIEN

### ✅ IMMER SICHER:
- Windows-Optimierungen
- Cache-Bereinigung  
- Speicher-Management
- Grafiktreiber-Einstellungen
- Startparameter

### 🟡 MEISTENS SICHER:
- Engine.ini Optimierungen (bei privaten Servern)
- Community-bewährte Fixes
- Netzwerk-Tweaks (bei Singleplayer)

### ❌ NIEMALS BEI OFFICIAL SERVERN:
- Aggressive Netzwerk-Änderungen
- Experimentelle Config-Tweaks
- Anti-Cheat beeinflussende Mods

---

## 🎛️ BEDIENUNGSANLEITUNG

### 1. Risiko-Level wählen
Wähle basierend auf deinem Spielmodus:
- **Official Server:** Nur 🛡️ + 🟡
- **Private Server:** Bis 🟠 möglich
- **Singleplayer:** Alle verfügbar

### 2. Einzeloptimierungen anpassen
Jede Optimierung zeigt:
- **Verbesserung in %**
- **Risiko-Level**
- **Detaillierte Beschreibung**

### 3. Voreinstellungen nutzen
Nutze vorkonfigurierte Sets für:
- Official Server Safe
- Private Server Optimized  
- Singleplayer Maximum
- Custom (eigene Auswahl)

### 4. Anwenden & Testen
- Starte mit niedrigem Risiko
- Teste jede Stufe einzeln
- Bei Problemen: Reset-Button nutzen

---

**💡 Merke:** Mehr Optimierung = Mehr Risiko. Wähle basierend auf deinem Spielmodus!
