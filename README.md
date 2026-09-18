# Ancient Value Overlay

A Windows desktop PoE2 reward-value helper, build lab, and skill-change simulator currently in beta.

## Beta 0.4 — Skill change simulator

The new **Skill Simulator** extends the Build Lab from compatibility checks into visible gear/support behavior changes.

- Normalized damage, speed, area, cost, projectile-count, and Spirit indexes.
- Two independently configured weapon sets with automatic compatible-set selection.
- Illustrative gear-roll profiles for heavy, swift, caster, Spirit, and hybrid setups.
- Current support-gem examples using modern naming and behavior:
  - Rapid Attacks I
  - Rapid Casting I
  - Multishot I
  - Magnified Area I
  - Concentrated Area
  - Elemental Armament I
  - Controlled Destruction
  - Elemental Focus
- Support socket cap handling from 2–5 sockets.
- Incompatible supports are rejected with a reason instead of silently applying.
- Support effects can change projectile count, area, cost, action speed, critical-hit availability, and elemental ailment availability.
- Wrath Sceptre exposes its intrinsic 100 Spirit in the model.
- The simulator explicitly uses normalized comparison indexes and does not claim exact in-game DPS.

The beta 0.4 model is based on current PoE2 0.5.x mechanics, while keeping the data slice intentionally small enough to test and iterate safely.


## Beta 0.3 — Build / Classes patch

The new **Build Lab** models the parts of PoE2 buildcraft that most strongly change how a skill behaves:

- Current eight-class Early Access roster with 22 Ascendancies represented.
- Class identity is guidance, not a hard skill lock.
- Representative weapon-gated skills for bows, crossbows, quarterstaves, spears, shields and melee martial weapons.
- Two weapon sets, so a skill can become usable from either set.
- Item-granted inherent skill examples such as Mana Drain, Sigil of Power, Consecrate and Fulmination.
- Talisman-style shapeshift basic attack support.
- Support-socket planning from 2 to 5 sockets per skill.
- Starter presets for every class.
- Gear mismatch explanations that identify the actual weapon requirement instead of blaming the selected class.
- Tests for the class roster, weapon-set fallback, gear-granted skills and support-socket bounds.

The Build Lab is intentionally a build-planning model rather than a full Path of Building replacement. It focuses on explaining **why** a class/skill/gear combination works and which part of the loadout changes the skill.

## Beta 0.2 — UI foundation

- Responsive dark dashboard.
- Session cards for best reward, total value, rows checked, and unknown rows.
- Activity history that appends results instead of replacing them.
- Copy, reset and clear actions.
- Keyboard shortcuts:
  - `Ctrl+Enter`: run the demo value check.
  - `Ctrl+B`: open Build Lab.
  - `Ctrl+Shift+C`: copy the latest value summary.
  - `Ctrl+L`: clear activity.
- CI runs the test suite before packaging the Windows beta.

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
