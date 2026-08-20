# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

A C#/MonoGame 3.8.5 (DesktopGL) clone of "Achtung, die Kurve!" (Curve Fever) — a local-multiplayer curve game. Migrated from XNA 4.0; parts of the codebase (screen system) are the stock XNA GameStateManagement sample. Targets `net10.0` (MonoGame's net8.0 assemblies are consumed from it directly).

## Build & run

```
dotnet build AchtungDieKurve.sln
dotnet run --project AchtungDieKurve/AchtungDieKurve.csproj
./publish.sh   # self-contained single-file releases for win-x64, osx-arm64, linux-x64 → publish/
```

Caveats:
- Content pipeline: `AchtungDieKurve/Content/Content.mgcb` is built into `.xnb` files automatically by `MonoGame.Content.Builder.Task` during `dotnet build`. MGCB is a dotnet local tool (`.config/dotnet-tools.json`); the build auto-runs `dotnet tool restore`. The **root-level `/Content/` directory is a stale legacy XNA copy** and is not referenced by the build; never edit it.
- Spritefonts reference Arial (system) and the bundled `Content/Staatliches-Regular.ttf` — keep font references as TTF file names, not installed-font names, so content builds on any machine.
- Publishing: trimming/NativeAOT must stay OFF (powerups are instantiated via reflection), and native libraries (SDL2/OpenAL) must stay as loose files next to the executable — MonoGame's loader does not find them in the single-file self-extract directory (`IncludeNativeLibrariesForSelfExtract` breaks at runtime).
- Display modes are owned by `AchtungDieKurve/Graphics/GraphicsManager.cs`: borderless fullscreen at desktop resolution (default) or windowed at a selectable size. `HardwareModeSwitch` must stay `false` — exclusive fullscreen on macOS renders nothing (and can land on the wrong monitor). macOS clips borderless display-sized windows below the menu bar (~39px); the game adapts via `GraphicsManager.SyncToWindow`, wired to `Window.ClientSizeChanged`, which adopts whatever size the OS actually granted — keep that handler intact. Gameplay tuning (`DefaultDiameter`, `DefaultSpeed`) is scaled relative to a 1080p baseline on every size change. `Defaults.ScreenWidth/Height` are set only by GraphicsManager (applied in `GameBase.LoadContent` before screens load). Headless/CI runs are not viable.
- There are no tests and no lint/analyzer config.

## Architecture

### Entry point & globals
`Program.cs` → `GameBase.cs` (subclass of `Microsoft.Xna.Framework.Game`), which doubles as a global service locator: `GameBase.Defaults` (all gameplay tunables live in `Properties.cs` — speed, turn step, hole probability, powerup probability, goal formula, debug flags), `GameBase.Graphics`, `GameBase.Log`, singleton `GameBase.GetInstance()`. Other global state: `CommonResources` (textures/fonts/theme colors), `TimerPool` (central pausable timer registry), `Powerups.Register`.

### Screen system
`ScreenManager/` + `Screens/` implement a screen stack (XNA GameStateManagement sample, lightly adapted). Flow: `LogoScreen` → `MainMenuScreen` → `GameChoiceScreen` (Classic vs Boosted, toggles `Defaults.PowerupsEnabled`) → `GameConfigScreen` (player selection via `Game/Drawable/Parts/Menu/GameConfigBox.cs`) → `GameplayScreen` → `ScoreScreen`.

### Gameplay wiring
`Screens/GameplayScreen.cs` builds the whole object graph in `LoadContent()`: `GameInterface` (layout) → `GridRegister` → `PlayersManager` → `Kurvy` → `Score`. Its Pause/UnPause events are wired to `TimerPool.Pause/UnPause` so all powerup/protection timers freeze with the pause menu.

### Core entities
- `Game/Drawable/Kurve.cs` — the core entity: one curve (position/angle/speed, trail stored as `List<Rectangle> Body`, hole generation, spawn protection, wall traversal). Emits events (`Move`, `Death`, `Start`, `ScoreChanged`, …). Naming convention: `Kurve` = one curve, `Kurvy` = the collection. `Player` is the human subclass; `Game/AI/AiPlayer` raises a `Controlling` event instead of reading keys.
- `Game/Drawable/Kurvy.cs` — coordinator: owns `CollisionManager`, `AiDriver`, `PowerupsController`, `GridRegister`, and the boundary `Wall`s; wires each added curve's events.
- `Game/PlayersManager.cs` — round/score logic: a death awards +1 to every surviving player; a match win requires `Score >= GoalPlusPerPlayer * playerCount` with no tie, else `NextRound` fires.

### Collision (spatial hash grid)
`Game/Core/`: `GridRegister` is a spatial hash (cell raster 64) over the playable area. Each curve's `Move` event drives `CollisionManager.Carry`, which registers the new segment and scans the neighborhood. Self-collision is avoided by `Kurve.CollisionCondition` excluding the newest own body segments. Only `CollidableShape.Rectangle` is implemented.

### Powerups
Reflection-based string registry: `PowerupsController` (`Game/Drawable/Powerups.cs`) resolves class names from `Game/Drawable/Powerups/Register.cs` via `Type.GetType` + `Activator.CreateInstance`. **Adding a powerup requires both** a class in `Game/Drawable/Powerups/` (with both the 3-arg and 4-arg constructors, since `Fork()` clones by reflection) **and** an entry in `Register.Load()`.

### AI
`Game/AI/AIDriver.cs` — feeler-based steering: 13 rays at 15° spacing measure free distance (kurve trails as circles from `GridRegister.Neighborhood`, playfield edges analytically; grid walls ignored). Powerups are goals or obstacles: Death is avoided like a trail; Switch always sought; attacking AIs seek Fast/SlowEnemy/FatEnemy, others Slow/Slim/Transcend. The AI steers into the most open direction, and episodically (driven by `Defaults.AiAggressiveness`) targets an intercept point ahead of the nearest enemy, with a survival veto. `Defaults.AiPrecision` sets reaction interval and steering error; both are jittered per player per round in `AiPlayer.Reset()`. All per-player AI state lives on `AiPlayer`; the driver is stateless. `DebugCollisions = true` draws the feelers.

## Conventions & gotchas

- Components communicate via C# events with custom delegates (`WormEvent`, `CollidableObjectMoved`, …) rather than direct calls. New per-player behavior is usually wired in `Kurvy.players_PlayerAdded` or `PlayersManager.AddPlayer`.
- `ref` is used pervasively on reference-type parameters (e.g. `ref GridRegister register`) — a stylistic quirk; match surrounding code.
- Mixed naming styles: `_camelCase` private fields in newer code (`Game/Core`, `Game/AI`), plain `camelCase` in XNA-sample-derived code (`Screens`, `ScreenManager`).
- The curve trail grows in `Kurve.Update()` (one `Body` segment per update, before the `Move` event fires); `Draw()` must stay side-effect free — Draw can run more often than Update on high-refresh displays. Collision detection always uses the head (`ActualBounds`); `CollisionDisabled` gates only grid registration (holes), `CanBeHit` gates detection (protection AND holes — while gapping the kurve is intentionally a ghost, but it collides with whatever it overlaps the moment the gap ends, including the out-of-bounds check).
- Settings persist as JSON to `%LocalAppData%/AchtungDieKurve/` via `Game/Core/RegistrySettings.cs` (the name is a leftover from a Windows-registry implementation).
- Known dead code / rough edges: root `/Content/` tree, `Lib/SFXController.cs` (empty, shadows `Sound/SfxController`), empty `PlayersManager.Reset()`, `NotImplementedException` stubs in some interface members, and blocking `Thread.Sleep` calls on the game thread for round transitions.
