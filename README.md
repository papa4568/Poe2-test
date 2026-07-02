# Ancient Value Overlay

A simple Windows desktop app for building a PoE2 value helper one step at a time.

## Current goal

Create a reliable executable first:

```text
AncientValueOverlay.exe
```

The current app opens a basic WinForms window and runs a demo value check.

## Build the app

```powershell
dotnet restore src/AncientValueOverlay/AncientValueOverlay.csproj
dotnet build src/AncientValueOverlay/AncientValueOverlay.csproj --configuration Release
```

## Package for Windows

```powershell
./scripts/package-windows.ps1
```

Output:

```text
artifacts/win-x64/AncientValueOverlay.exe
artifacts/AncientValueOverlay-win-x64.zip
```

## Download from GitHub Actions

1. Open the repo.
2. Go to Actions.
3. Open package-windows-net8.
4. Open the latest green run.
5. Download AncientValueOverlay-win-x64.
6. Unzip it.
7. Run AncientValueOverlay.exe.

## Development rule

Keep the executable build green. Add one feature at a time only after the package workflow succeeds.

## License

MIT. See LICENSE.
