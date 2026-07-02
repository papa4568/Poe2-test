# Ancient Value Overlay

A clean-room Path of Exile 2 overlay project for reading reward panels, estimating value, and helping players make faster decisions without alt-tabbing.

This repository is intentionally **not a copy** of `PoeAncientsPriceHelper`. It is a new project inspired by the same general problem: capture a region, read reward rows, fetch market values, and show a lightweight overlay.

## Goals

- Guided calibration with a live OCR preview.
- Decision-focused overlay: best pick, total panel value, confidence, and unknown rows.
- Resilient price data: live source health, offline cache, and user-editable overrides.
- Modular mechanics: start with Ancient/Remnant rewards, then add other PoE2 reward panels as separate modules.
- Content creator friendly: session stats, screenshots, debug export, and clear value callouts.

## Current status

This is the initial repository scaffold. The core architecture is laid out, with domain models and service interfaces ready for implementation.

## Simple executable

Use the Windows packaging workflow or local script to produce:

```text
AncientValueOverlay.exe
```

See [Executable guide](docs/executable-guide.md).

## Guides

- [Executable guide](docs/executable-guide.md)
- [Usage guide](docs/usage-guide.md)
- [Setup](docs/setup.md)
- [Architecture](docs/architecture.md)
- [Clean-room notes](docs/legal-clean-room.md)

## Proposed stack

- .NET 10 Windows desktop app
- WPF settings/calibration UI
- WinForms or WPF transparent overlay
- Windows OCR abstraction first, with room for alternate OCR engines later
- poe.ninja-compatible price source module
- JSON config/cache files under `%LocalAppData%/AncientValueOverlay`

## Project layout

```text
src/AncientValueOverlay/          App source
src/AncientValueOverlay.Tests/    Unit tests
docs/                            Architecture, setup, and clean-room notes
scripts/                         Repo creation and push helpers
.github/workflows/               CI pipeline
```

## Build

```powershell
dotnet restore
dotnet build
dotnet test
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

On non-Windows systems, the Windows desktop target may not build fully. CI is configured to run on `windows-latest`.

## License

MIT. See [LICENSE](LICENSE).
