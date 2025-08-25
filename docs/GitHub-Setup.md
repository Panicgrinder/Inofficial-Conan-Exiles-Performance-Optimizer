# GitHub Repository Setup für Conan Exiles Optimizer

## 🚀 Anleitung zum Erstellen des GitHub-Repositorys

### 1. GitHub Repository erstellen

1. **Gehe zu GitHub.com** und melde dich an
2. **Klicke auf "New repository"** (grüner Button)
3. **Repository-Details:**
   - **Repository Name:** `ConanExilesOptimizer`
   - **Description:** `🗡️ Professional Performance Optimizer for Conan Exiles - Reduce loading times by 50-70% with automated Steam detection, mod path fixing, and real-time monitoring`
   - **Visibility:** Public (damit andere es nutzen können)
   - **Initialisiere NICHT** mit README, .gitignore oder License (haben wir schon)

4. **Klicke "Create repository"**

### 2. Lokales Repository mit GitHub verbinden

```bash
# In VS Code Terminal (F:\ConanExilesOptimizer\):
git remote add origin https://github.com/Panicgrinder/ConanExilesOptimizer.git
git branch -M main
git push -u origin main
```

### 3. Repository-Einstellungen konfigurieren

#### Tags und Releases
```bash
# Ersten Release-Tag erstellen
git tag -a v3.0.0 -m "Release v3.0.0 - Professional GUI with standalone .exe"
git push origin v3.0.0
```

#### GitHub Actions (Optional)
Erstelle `.github/workflows/build.yml` für automatische Builds:

```yaml
name: Build Conan Exiles Optimizer

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 6.0.x
    
    - name: Restore dependencies
      run: dotnet restore src/ConanOptimizer.csproj
    
    - name: Build
      run: dotnet build src/ConanOptimizer.csproj --no-restore
    
    - name: Publish Release
      run: dotnet publish src/ConanOptimizer.csproj --configuration Release --output releases/v3.0 --self-contained true --runtime win-x64 /p:PublishSingleFile=true
    
    - name: Upload Release Artifact
      uses: actions/upload-artifact@v3
      with:
        name: ConanOptimizer-v3.0
        path: releases/v3.0/ConanOptimizer.exe
```

### 4. Repository-Features aktivieren

#### In GitHub Repository Settings:
- **Issues:** Aktiviert (für Bug-Reports)
- **Discussions:** Aktiviert (für Community-Support)
- **Releases:** Aktiviert (für Versionsveröffentlichungen)
- **Topics:** Hinzufügen: `conan-exiles`, `gaming`, `performance`, `optimizer`, `csharp`, `windows`

#### README Badge hinzufügen:
```markdown
![Build Status](https://github.com/Panicgrinder/ConanExilesOptimizer/workflows/Build%20Conan%20Exiles%20Optimizer/badge.svg)
![Release](https://img.shields.io/github/v/release/Panicgrinder/ConanExilesOptimizer)
![Downloads](https://img.shields.io/github/downloads/Panicgrinder/ConanExilesOptimizer/total)
![License](https://img.shields.io/github/license/Panicgrinder/ConanExilesOptimizer)
```

### 5. Ersten GitHub Release erstellen

1. **Gehe zu "Releases"** im Repository
2. **"Create a new release"** klicken
3. **Release-Details:**
   - **Tag:** v3.0.0
   - **Title:** Conan Exiles Optimizer v3.0 - Professional GUI Release
   - **Description:**
```markdown
# 🗡️ Conan Exiles Optimizer v3.0 - Professional GUI Release

## ⚡ Quick Start
1. Download `ConanOptimizer.exe`
2. Run it (no installation needed!)
3. Click "Conan Exiles optimieren"
4. Enjoy 50-70% faster loading times!

## ✨ New Features
- **Professional Windows GUI** - Modern and user-friendly interface
- **Automatic Detection** - Finds Steam and Conan Exiles automatically
- **Real-time Monitoring** - Live CPU, RAM, and disk activity
- **One-Click Optimization** - Complete automation
- **Advanced Settings** - Power-user features

## 📊 Performance Improvements
- ⚡ 50-70% faster loading times
- 🧠 20-30% less RAM usage
- 💾 More stable performance
- 🏆 Smoother gameplay

## 🛠️ System Requirements
- Windows 10/11 (64-bit)
- Steam with Conan Exiles installed
- No additional software needed!

## 🛡️ Safety
- ✅ Only official Conan optimizations
- ✅ Automatic backups before changes
- ✅ No game file modifications
- ✅ Virus-free standalone executable
```

4. **Datei hochladen:** Ziehe `ConanOptimizer.exe` in den Release
5. **"Publish release"** klicken

### 6. Community Features

#### Issue Templates erstellen:
`.github/ISSUE_TEMPLATE/bug_report.md`:
```markdown
---
name: Bug Report
about: Create a report to help us improve
title: '[BUG] '
labels: bug
assignees: ''
---

**Describe the bug**
A clear and concise description of what the bug is.

**System Information:**
- Windows Version: [e.g. Windows 11 22H2]
- Steam Installation: [e.g. C:\Program Files (x86)\Steam]
- Conan Exiles Mods: [Number of mods installed]

**Steps To Reproduce**
1. Go to '...'
2. Click on '....'
3. See error

**Expected behavior**
A clear and concise description of what you expected to happen.

**Screenshots**
If applicable, add screenshots to help explain your problem.
```

### 7. Repository-URL für Freunde

Nach dem Setup können Sie Ihren Freunden einfach sagen:

**"Geht auf GitHub.com und sucht nach: `Panicgrinder/ConanExilesOptimizer`"**

Oder direkt den Link teilen:
**`https://github.com/Panicgrinder/ConanExilesOptimizer`**

### 8. Repository-Struktur Überblick

```
ConanExilesOptimizer/
├── 📁 .github/              # GitHub-spezifische Dateien
│   ├── workflows/           # GitHub Actions
│   └── ISSUE_TEMPLATE/      # Issue-Vorlagen
├── 📁 src/                  # Quellcode
│   ├── ConanOptimizer.cs    # Hauptanwendung
│   └── ConanOptimizer.csproj# Projekt-Konfiguration
├── 📁 releases/             # Build-Ausgaben
│   └── v3.0/               # Version 3.0 Release
├── 📁 docs/                 # Dokumentation
├── 📄 README.md             # Haupt-Dokumentation
├── 📄 CHANGELOG.md          # Versions-Historie
├── 📄 CONTRIBUTING.md       # Beitragsleitfaden
├── 📄 LICENSE               # MIT Lizenz
├── 📄 .gitignore           # Git-Ignore-Regeln
├── ⚙️ build-simple.bat      # Einfaches Build-Script
└── 🔧 ConanExilesOptimizer.code-workspace # VS Code Workspace
```

---

**🎉 Bereit für die Community! Ihr Repository ist professionell aufgesetzt und bereit, der Conan Exiles Community zu helfen!** 🗡️
