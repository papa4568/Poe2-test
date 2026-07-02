# Setup

## Requirements

- Windows 10 2004 or newer, or Windows 11
- .NET 10 SDK
- Visual Studio 2026 or current .NET SDK tooling

## Build

```powershell
dotnet restore
dotnet build
dotnet test
```

## Local cache

Runtime files should live under:

```text
%LocalAppData%\AncientValueOverlay
```

Do not commit generated cache, screenshots, logs, or local config files.
