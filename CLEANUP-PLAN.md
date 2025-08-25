# CLEANUP-PLAN 

- CONAN EXILES OPTIMIZER




## MIT HÖCHSTER VORSICHT



### PHASE 1: BUILD-SCRIPTS BEREINIGUNG


**ZU LÖSCHEN (6 Dateien):*

*


- [x] `build-advanced.ps1`
 

- Ersetzt durch build-modular.ps1


- [x] `build-advanced-fixed.ps1`
 

- Ersetzt durch build-modular.ps1  


- [x] `build-all.ps1`
 

- Ersetzt durch build-modular.ps1


- [x] `build-safe.ps1`
 

- Ersetzt durch build-modular.ps1


- [x] `build-safe-simple.ps1`
 

- Ersetzt durch build-modular.ps1


- [x] `build-simple.ps1`
 

- Ersetzt durch build-modular.ps1


**BEHALTEN:*

*


- ✅ `build-modular.ps1`
 

- Der einzige funktionierende Build-Script



### PHASE 2: TEST-SCRIPTS BEREINIGUNG (OPTIONAL)


**ZU LÖSCHEN (3 Dateien) 

- Nur Development:
*

*


- [x] `test-alle-versionen.ps1`
 

- Development only


- [x] `test-modular.ps1`
 

- Development only  


- [x] `test-versionen-simple.ps1`
 

- Development only



### PHASE 3: DOKUMENTATIONS-DUPLIKATE BEREINIGUNG


**ZU LÖSCHEN (13 Dateien):*

*


```
releases\advanced\*.md (3 Dateien)
releases\original\*.md (3 Dateien) 
releases\safe\*.md (3 Dateien)
releases\v3.0.0\README.md
releases\v3.1.0-safe\*.md (2 Dateien)
releases\v3.2.0-advanced\ERWEITERTE-ANLEITUNG.md

```


**MASTER BEHALTEN:*

*


- ✅ `modules\documentation\`
 

- Alle originalen Dokumentationen



### SICHERHEITS-CHECKPOINTS


1. ✅ Analyse abgeschlossen 

- 22 redundante Dateien identifiziert

2. ✅ Backup erstellen (falls gewünscht) 

- Git Backup vorhanden

3. ✅ Phase 1: Build-Scripts gelöscht 

- 6 redundante Scripts entfernt

4. ✅ Test: build-modular.ps1 funktioniert weiterhin 

- ALLE 3 VERSIONEN BAUEN ERFOLGREICH!

5. ✅ Phase 2: Test-Scripts gelöscht 

- 3 Development Scripts entfernt

6. ✅ Phase 3: Dokumentations-Duplikate gelöscht 

- 13 Duplikate entfernt

7. ✅ Final-Test: Safe Version kompiliert erfolgreich 

- BEREINIGUNG ABGESCHLOSSEN!



### WICHTIG



- **NUR build-modular.ps1 funktioniert
*

* 

- alle anderen sind veraltet


- **modules\documentation\ ist Master
*

* 

- alle releases\*\*.md sind Kopien


- **Jederzeit rückgängig machbar
*

* durch Git oder manuelle Wiederherstellung