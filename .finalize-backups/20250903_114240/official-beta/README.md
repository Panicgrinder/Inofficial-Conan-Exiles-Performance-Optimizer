# Official Beta: Conan Exiles Optimizer

Dies ist ein eigenstaendiges WinForms-Projekt fuer die offizielle Beta.

Enthalten:
- OfficialConanOptimizer.csproj (net6.0-windows, WinForms, SingleFile)
- Program.cs, MainForm.cs (moderne UI, RAM-/Disk-Erkennung)

Build:
- Mit Visual Studio (Solution hinzufuegen) oder CLI:

```powershell
# im Repo-Root
dotnet build .\official-beta\OfficialConanOptimizer.csproj -c Release
```

Start (Debug):
```powershell
dotnet run --project .\official-beta\OfficialConanOptimizer.csproj
```

Publish (Release):
```powershell
dotnet publish .\official-beta\OfficialConanOptimizer.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o .\releases\official-beta
```

Hinweis: Dieses Projekt verändert keine Game-Config-Dateien und ist als Beta-UI/App gedacht.
