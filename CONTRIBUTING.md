# Beitragen zum Conan Exiles Optimizer

Vielen Dank für Ihr Interesse, zum Conan Exiles Optimizer beizutragen! 🎉

## 🎯 Möglichkeiten zum Beitragen

### 🐛 Bug Reports
- Verwenden Sie GitHub Issues für Bug-Meldungen
- Geben Sie detaillierte Reproduktionsschritte an
- Fügen Sie System-Informationen hinzu (Windows-Version, .NET-Version, etc.)

### ✨ Feature Requests
- Beschreiben Sie den gewünschten Use Case
- Erklären Sie, wie es die Benutzererfahrung verbessert
- Berücksichtigen Sie die Auswirkungen auf andere Benutzer

### 🔧 Code Contributions
- Fork das Repository
- Erstellen Sie einen Feature-Branch
- Implementieren Sie Ihre Änderungen
- Erstellen Sie aussagekräftige Commit-Messages
- Öffnen Sie einen Pull Request

## 💻 Entwicklungsumgebung einrichten

### Voraussetzungen
```bash
# .NET 6.0+ SDK installieren
# https://dotnet.microsoft.com/download

# Repository klonen
git clone https://github.com/Panicgrinder/ConanExilesOptimizer.git
cd ConanExilesOptimizer

# Projekt bauen
.\build.ps1

# Für Entwicklung
cd src
dotnet run
```

### Projektstruktur
```
ConanExilesOptimizer/
├── src/                  # Quellcode
│   ├── ConanOptimizer.cs    # Hauptanwendung
│   └── ConanOptimizer.csproj # Projekt-Konfiguration
├── build/                # Build-Skripte
├── docs/                 # Dokumentation
├── releases/             # Release-Builds
├── README.md
├── CHANGELOG.md
└── LICENSE
```

## 📋 Coding Standards

### C# Code Style
- Verwenden Sie die Standard .NET Naming Conventions
- Kommentieren Sie öffentliche APIs
- Folgen Sie den Microsoft C# Coding Guidelines
- Verwenden Sie async/await für I/O-Operationen

### Git Workflow
```bash
# Feature Branch erstellen
git checkout -b feature/mein-neues-feature

# Änderungen committen
git add .
git commit -m "feat: neue Feature-Beschreibung"

# Push und Pull Request
git push origin feature/mein-neues-feature
```

### Commit Message Format
```
type(scope): kurze Beschreibung

Detailliertere Beschreibung falls nötig.

Fixes #123
```

**Types:** feat, fix, docs, style, refactor, test, chore

## 🧪 Testing

### Manuelles Testen
- Testen Sie auf verschiedenen Windows-Versionen
- Verifizieren Sie mit verschiedenen Steam-Installationen
- Prüfen Sie verschiedene Mod-Konfigurationen

### Automatisierte Tests
```bash
cd src
dotnet test
```

## 📝 Dokumentation

### README Updates
- Halten Sie die Feature-Liste aktuell
- Aktualisieren Sie Screenshots bei UI-Änderungen
- Ergänzen Sie neue Systemanforderungen

### Code-Dokumentation
- Dokumentieren Sie öffentliche APIs
- Erklären Sie komplexe Algorithmen
- Fügen Sie Beispiele hinzu

## 🚀 Release-Prozess

### Versionierung
Wir verwenden [Semantic Versioning](https://semver.org/):
- MAJOR: Breaking Changes
- MINOR: Neue Features (rückwärtskompatibel)
- PATCH: Bug Fixes

### Release Checklist
- [ ] Version in `.csproj` aktualisieren
- [ ] CHANGELOG.md ergänzen
- [ ] Build testen (`.\build.ps1`)
- [ ] Tag erstellen (`git tag v3.x.x`)
- [ ] GitHub Release erstellen

## 🤝 Community Guidelines

### Verhalten
- Seien Sie respektvoll und hilfsbereit
- Konstruktive Kritik ist willkommen
- Helfen Sie anderen Benutzern bei Problemen

### Kommunikation
- Issues und Pull Requests auf Deutsch oder Englisch
- Klare und höfliche Kommunikation
- Geduld bei Review-Prozessen

## 📞 Kontakt

Bei Fragen zum Beitragen:
- Öffnen Sie ein GitHub Issue
- Diskutieren Sie in Pull Requests
- Verwenden Sie GitHub Discussions für allgemeine Fragen

---

**Vielen Dank für Ihre Unterstützung der Conan Exiles Community! 🗡️**
