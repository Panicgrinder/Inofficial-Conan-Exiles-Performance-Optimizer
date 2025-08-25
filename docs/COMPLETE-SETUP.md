# 🚀 Conan Exiles Optimizer 

- Komplettes Setup




## ⚡ Schnell-Setup (3 Schritte)



### Schritt 1: GitHub Repository erstellen

1. Öffnen Sie: https://github.com/new
2. Repository name: `ConanExilesOptimizer`  
3. Description: `🗡️ Professional Performance Optimizer for Conan Exiles`
4. Public ✅
5. Klicken Sie: "Create repository"


### Schritt 2: Automatisches Setup ausführen

```powershell

# In VS Code Terminal (F:\ConanExilesOptimizer\):

.\setup-github.ps1

```



### Schritt 3: Ersten Release erstellen

```powershell

# Build und Release in einem Schritt:

.\build.ps1 -OpenOutput

```


--

-


## 🔧 Detailliertes Setup



### GitHub Repository Setup



#### 1. Repository erstellen


1. **Gehe zu GitHub.com*

* und melde dich an

2. **Klicke auf "New repository"*

* (grüner Button)

3. **Repository-Details:*

*
   

- **Repository Name:
*

* `ConanExilesOptimizer`

   

- **Description:
*

* `🗡️ Professional Performance Optimizer for Conan Exiles
 

- Reduce loading times by 50-70% with automated Steam detection, mod path fixing, and real-time monitoring`

   

- **Visibility:
*

* Public (damit andere es nutzen können)

   

- **Initialisiere NICHT
*

* mit README, .gitignore oder License (haben wir schon)


4. **Klicke "Create repository"*

*


#### 2. Lokales Repository mit GitHub verbinden



```powershell

# In VS Code Terminal (F:\ConanExilesOptimizer\):

git remote add origin https://github.com/IHR-USERNAME/ConanExilesOptimizer.git
git branch -M main
git push -u origin main

```



#### 3. Automatisches Setup (Alternative)



```powershell

# Verwendet das setup-github.ps1 Script:

.\setup-github.ps1

```



### Build-Anleitung



#### Standard Build

```powershell

# Release Build erstellen:

.\build.ps1


# Mit automatischem Output-Ordner öffnen:

.\build.ps1 -OpenOutput


# Debug Build:

.\build.ps1 -Configuration Debug

```



#### Manuelle Builds

```powershell

# Debug Build:

cd src
dotnet build --configuration Debug


# Release Build:

dotnet build --configuration Release


# Single-File Executable:

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

```



### Entwicklungsumgebung



#### Voraussetzungen


- Windows 10/11


- .NET 6.0 SDK oder höher


- Visual Studio Code mit 
C

# Extension


- Git für Versionskontrolle



#### Projekt öffnen

1. VS Code öffnen
2. `Datei > Ordner öffnen`
3. `F:\ConanExilesOptimizer` auswählen
4. Oder: `ConanExilesOptimizer.code-workspace` öffnen


#### Erste Schritte

```powershell

# Projekt kompilieren:

dotnet build


# Anwendung starten:

dotnet run


# Tests ausführen (falls vorhanden):

dotnet test

```



### GitHub Features nutzen



#### Releases erstellen

1. Gehe zu: `https://github.com/IHR-USERNAME/ConanExilesOptimizer/releases`
2. Klicke "Create a new release"
3. Tag: `v3.0.0`
4. Title: `Conan Exiles Optimizer v3.0 

- Community Optimizations`

5. Beschreibung mit Features und Verbesserungen
6. Lade `ConanOptimizer.exe` aus `releases/v3.0/` hoch


#### Issues und Feedback


- Issues für Bug Reports: `https://github.com/IHR-USERNAME/ConanExilesOptimizer/issues`


- Discussions für Feature Requests


- Wiki für erweiterte Dokumentation



### Troubleshooting



#### Häufige Probleme


- **Git Fehler**: `git config --global user.name "Ihr Name"`


- **Build Fehler**: .NET 6 SDK installieren


- **Permissions**: Als Administrator ausführen



#### Support


- GitHub Issues für Bugs


- Discussions für Fragen


- README.md für grundlegende Informationen


--

-


## 📚 Weitere Ressourcen



- [README.md](../README.md)
 

- Projekt-Übersicht


- [CONTRIBUTING.md](../CONTRIBUTING.md)
 

- Beitragen zum Projekt


- [CHANGELOG.md](../CHANGELOG.md)
 

- Versionshistorie


- [build.ps1](../build.ps1)
 

- Build-Script Dokumentation