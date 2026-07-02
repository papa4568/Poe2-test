# Executable Guide

This guide explains how to create and run a simple Windows executable for Ancient Value Overlay.

## What users should download

The packaged build is intended to be a zip file named:

```text
AncientValueOverlay-win-x64.zip
```

Inside the zip, the main program is:

```text
AncientValueOverlay.exe
```

The package is self-contained, so users should not need to install the .NET runtime separately.

## Option 1: Download from GitHub Actions

1. Open the GitHub repo.
2. Click the `Actions` tab.
3. Open the `package-windows` workflow.
4. Click the latest successful run.
5. Download the `AncientValueOverlay-win-x64` artifact.
6. Unzip it.
7. Double-click `AncientValueOverlay.exe`.

## Option 2: Create the executable locally

From a Windows PowerShell terminal:

```powershell
git clone https://github.com/papa4568/Poe2-test.git
cd Poe2-test
./scripts/package-windows.ps1
```

The script creates:

```text
artifacts/win-x64/AncientValueOverlay.exe
artifacts/AncientValueOverlay-win-x64.zip
```

Run the executable with:

```powershell
./artifacts/win-x64/AncientValueOverlay.exe
```

## Current behavior

At this stage, the executable opens the app shell. The app is not a finished overlay yet. The next implementation steps are:

1. Wire the price source into the UI.
2. Add config loading and saving.
3. Add the calibration flow.
4. Add the reader service.
5. Add the transparent value display.

## Windows SmartScreen

Because the executable is not code-signed yet, Windows may warn when opening it. For public releases, the long-term fix is code signing.

## Release checklist

Before sharing a release broadly:

- Run the `package-windows` workflow successfully.
- Download and unzip the artifact.
- Confirm `AncientValueOverlay.exe` opens.
- Confirm the version number in `AncientValueOverlay.csproj` is correct.
- Attach `AncientValueOverlay-win-x64.zip` to a GitHub Release.
- Add release notes that clearly say what works and what is still placeholder.
