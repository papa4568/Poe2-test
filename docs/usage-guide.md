# Usage Guide

Ancient Value Overlay is currently an early scaffold. This guide explains how the finished app is intended to work, how to run the current project, and what pieces still need to be implemented.

## What the app is for

Ancient Value Overlay is a Path of Exile 2 helper for reward/value panels. The intended workflow is:

1. Pick your league.
2. Calibrate the reward panel area.
3. Let the app read visible reward rows.
4. Fetch market prices.
5. Show the best reward, total panel value, unknown rows, and source health.

The current repo already contains the models and services for ranking rows, caching prices, and fetching poe.ninja data. The live screen reader and transparent presentation layer are still upcoming work.

## Requirements

- Windows 10 version 2004 or newer, or Windows 11.
- .NET 10 SDK.
- Visual Studio or another editor that supports .NET projects.
- Path of Exile 2 installed if you are testing the finished overlay behavior later.

## Clone and build

```powershell
git clone https://github.com/papa4568/Poe2-test.git
cd Poe2-test
dotnet restore
dotnet build
dotnet test
```

The app targets Windows desktop, so the full app should be built on Windows. The GitHub Actions workflow also builds on `windows-latest`.

## Run the current scaffold

```powershell
dotnet run --project src/AncientValueOverlay/AncientValueOverlay.csproj
```

At this stage, the app opens a basic WPF shell. The `Calibrate` and `Start` buttons are placeholders until the capture, OCR, and overlay services are wired in.

## Intended user workflow after Milestone 1 and 2

### 1. Start the app

Launch Ancient Value Overlay. The app loads your config from:

```text
%LocalAppData%\AncientValueOverlay
```

If no config exists yet, it uses default settings.

### 2. Select your league

The initial default league is `Runes of Aldur`. League support is stored in `AppConfig.LeagueName`.

### 3. Fetch prices

The `PoeNinjaPriceSource` pulls prices from poe.ninja exchange endpoints and returns a `PriceFetchResult` with:

- price entries,
- source health,
- item count,
- live/cache/failure status.

When a live fetch succeeds, `PriceCacheStore` saves a local cache. When live prices fail later, the app can fall back to cached prices.

### 4. Calibrate a panel

The intended calibration flow is:

1. Click `Calibrate`.
2. Drag a box around the reward panel text area.
3. Review a preview of rows the app can read.
4. Save the profile if the preview looks good.
5. Recalibrate if the row count or matches look wrong.

Calibration profiles are represented by `CalibrationProfile` in `AppConfig.cs`.

### 5. Start value reading

After calibration, the app will read reward rows and convert them into `RewardRead` models.

Each row is then matched to a price entry and converted into a `RewardRow`.

### 6. Review the decision summary

`DecisionAdvisor` ranks the rows and produces a `PanelAdvice` result:

- `BestRow` — the highest total value row.
- `SecondBestRow` — the next highest row.
- `BestGapDivines` — how much better the best row is.
- `TotalDivines` — total value of all priced rows.
- `UnknownRows` — rows that were read but could not be priced.

The intended overlay summary looks like this:

```text
Best: divine orb · 1.00d · +0.85d over next · total 1.25d · unknown 0
```

### 7. Track the session

`SessionStats` can record every analyzed panel and track:

- number of panels read,
- total value seen,
- best reward seen during the session.

This is planned for content-creator and review use.

## Current source files to know

| File | Purpose |
|---|---|
| `AppConfig.cs` | User settings and calibration profiles. |
| `DomainModels.cs` | Reward rows, price entries, advice result, and source health models. |
| `DecisionAdvisor.cs` | Finds the best row, total value, gap, and unknown rows. |
| `PoeNinjaPriceSource.cs` | Fetches and parses poe.ninja exchange data. |
| `PriceCacheStore.cs` | Saves and loads local price snapshots. |
| `CalibrationModels.cs` | Builds a calibration preview from read rows and prices. |
| `SessionStats.cs` | Tracks panels and best reward seen. |
| `Interfaces.cs` | Service interfaces for reader, price source, config, and presenter. |

## Development roadmap

### Milestone 1: Make the scaffold run end-to-end without game capture

- Load config.
- Fetch prices.
- Load/save cache.
- Feed fake reward rows into `DecisionAdvisor`.
- Show the decision summary in the main window.

### Milestone 2: Add calibration preview

- Add a region picker.
- Add OCR or mock OCR service.
- Show raw rows, normalized names, matched price keys, and confidence.
- Save named calibration profiles.

### Milestone 3: Add the overlay presenter

- Add transparent click-through value presentation.
- Show best-row callout.
- Show total value and unknown rows.
- Show price source health.

### Milestone 4: Add polish

- Session stats screen.
- Screenshot/debug export.
- Custom price overrides.
- Modular support for additional PoE2 reward panels.

## Troubleshooting

### Build fails on non-Windows

This is expected for the Windows desktop app target. Use Windows or rely on the GitHub Actions workflow.

### Prices are empty

Possible causes:

- poe.ninja is temporarily unavailable.
- The league name is wrong.
- The endpoint changed.
- There is no cache yet.

Expected behavior is to show `PriceSourceStatus.Failed` when no live or cached prices are available.

### Cache is stale

Delete the cache folder under:

```text
%LocalAppData%\AncientValueOverlay\cache
```

Then restart the app after the price source is working again.

## Clean-room reminder

Do not copy source code from other projects unless their license allows it. This project should remain a clean, original implementation.
