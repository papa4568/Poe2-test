# Ancient Value Overlay

A Windows desktop PoE2 reward-value helper currently in beta.

## Beta 0.2 highlights

- Responsive dark dashboard instead of the fixed starter layout.
- Session cards for best reward, total value, rows checked, and unknown rows.
- Activity history that appends results instead of replacing them.
- Copy, reset, clear, and next-step actions.
- Keyboard shortcuts:
  - `Ctrl+Enter`: run the demo value check.
  - `Ctrl+Shift+C`: copy the latest summary.
  - `Ctrl+L`: clear activity.
  - `F1`: show next development steps.
- CI now runs the test suite before packaging the Windows beta.

## Build the app

```powershell
dotnet restore AncientValueOverlay.sln
dotnet test src/AncientValueOverlay.Tests/AncientValueOverlay.Tests.csproj --configuration Release
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
3. Open `package-windows-net8`.
4. Open the latest green run.
5. Download `AncientValueOverlay-win-x64`.
6. Unzip it.
7. Run `AncientValueOverlay.exe`.

## Development rule

Keep tests and the executable build green. Add one feature at a time after the package workflow succeeds.

## License

MIT. See LICENSE.
