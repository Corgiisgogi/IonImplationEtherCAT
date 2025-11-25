# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

IonImplationEtherCAT is a C# WinForms application (.NET Framework 4.7.2) that simulates an ion implantation semiconductor equipment control system. The application manages wafer processing through FOUPs (Front Opening Unified Pods), a Transfer Module (TM), and Process Modules (PMs).

## Build and Development Commands

### Building the Project
```bash
# Build in Debug configuration
msbuild IonImplationEtherCAT.sln /p:Configuration=Debug

# Build in Release configuration
msbuild IonImplationEtherCAT.sln /p:Configuration=Release
```

### Running the Application
- Open the solution in Visual Studio and press F5 to run
- Or build and run the executable from `bin\Debug\IonImplationEtherCAT.exe`

## High-Level Architecture

### View Layer Structure

The application uses a main form (`MainForm`) with a swappable content panel that displays different views:

- **MainForm.cs**: Main application window with header (login/connection controls), content panel, and footer (navigation buttons)
  - Manages login state (`IsLogined`) and connection state (`IsConnected`)
  - Contains navigation to switch between views: Main, Alarm, Recipe, and Log
  - Header shows EtherCAT status and module status indicators (TM, PM1, PM2, PM3)

- **MainView.cs**: Primary equipment control interface
  - Displays FOUP A/B wafer status (5 slots each)
  - Shows Process Module status and progress (PM1/A chamber)
  - Contains TM graphics visualization via `TMGraphicsPanel`
  - Manages recipe settings for ProcessModules A, B, C

- **AlarmView.cs**: Alarm management interface
- **RecipeView.cs**: Recipe configuration interface
- **LogView.cs**: Event logging interface

### Core Equipment Models

**ProcessModule** (`ProcessModule.cs`): Represents a process chamber (PM)
- States: `Idle`, `Running`, `Paused`, `Stoped`, `Error`
- Tracks `processTime`, `elapsedTime`, `isWaferLoaded`
- Methods: `StartProcess()`, `PauseProcess()`, `ResumeProcess()`, `StopProcess()`, `UpdateProcess()`

**TransferModule** (`TransferModule.cs`): Robotic arm for wafer transfer
- States: `Idle`, `Moving`, `Rotating`, `PickingWafer`, `PlacingWafer`
- Manages rotation angle and arm extension with smooth animations (90°/sec rotation, 200px/sec extension)
- Key methods: `SetTargetRotation()`, `ExtendArm()`, `RetractArm()`, `PickWafer()`, `PlaceWafer()`
- Updates position via `Update(deltaTime)` called by 60 FPS timer

**Foup** (`Foup.cs`): Wafer container with 5 slots
- Properties: `WaferSlots[5]`, `IsFull`, `IsEmpty`, `WaferCount`
- Methods: `LoadWafers()`, `UnloadWafers()`, `LoadWafer(index)`, `UnloadWafer(index)`

### Command Queue System

**CommandQueue** (`CommandQueue.cs`): Manages sequential execution of equipment operations
- Queues `ProcessCommand` objects and executes them asynchronously
- Handles wafer transfer sequences (rotate → extend → pick → retract → rotate → extend → place → retract)
- Waits for animations to complete before executing next command
- Created in MainView and used for the "All Process" automated workflow

**ProcessCommand** (`ProcessCommand.cs`): Represents a single equipment operation
- CommandTypes: `RotateTM`, `ExtendArm`, `RetractArm`, `PickWafer`, `PlaceWafer`, `StartProcess`, `RemoveWaferFromFoup`, `AddWaferToFoup`, `Delay`, `WaitForCompletion`
- Contains parameters array for flexible command data (angles, modules, FOUP references, etc.)

### UI Components

**TMGraphicsPanel** (`TMGraphicsPanel.cs`): Custom panel that renders Transfer Module graphics
- Draws TM arm, rotation, and wafer position in real-time
- Positioned at (290, 210) within `MainView.panelMainControl`

**CustomPictureBox** (`CustomPictureBox.cs`): Custom image display component
**RotatableImagePanel** (`RotatableImagePanel.cs`): Panel supporting image rotation

### Data Models

**Wafer** (`Wafer.cs`): Represents a single wafer entity

## Key Workflows

### Process Execution Flow

1. User logs in (hardcoded: `admin`/`1234` in MainForm:147-148)
2. User clicks Connect to establish EtherCAT connection (simulated)
3. User loads wafers into FOUP A via "Load SW" button
4. User sets recipe time for Process Module A (default 60 seconds)
5. User clicks "All Process" which triggers:
   - Command queue creation with wafer transfer sequence
   - TM rotates to FOUP A (-50°)
   - TM extends arm and picks top wafer from FOUP A
   - TM retracts and rotates to PM1 (0°)
   - TM extends and places wafer in PM1
   - Process starts with 1-second timer updates
6. Progress bar updates until process completes
7. Process Module returns to Idle state

### State Management Patterns

- Login/connection state gates button activation via `ActivateButtons()` and `ActivateAllButtons()` in MainForm
- Process cannot be interrupted if running (checked in Disconnect and Logout handlers)
- FOUP validation prevents starting process if FOUP A is empty or FOUP B is full
- Timer-driven updates for process progress (1 second interval) and TM animation (16ms ~60 FPS)

### Graphics Update Pattern

- `TMGraphicsPanel` invalidates/redraws when TM state changes
- FOUP display updates by changing panel background colors (DeepSkyBlue = wafer present, DarkGray = empty)
- Status icons change based on ProcessModule state (Gray/Green/Yellow/Red for Idle/Running/Paused/Error)

## Important Implementation Details

### Coordinates and Positioning

TM components have initial positions set in `MainView.InitializeTransferModule()`:
- ArmHigh: (234, 237)
- ArmLow: (261, 237)
- Bottom (center): (287, 211)
- Back: (472, 237)

Rotation angles:
- FOUP A: -50°
- PM1: 0°
- PM2: 90° (not implemented)
- PM3: 180° (not implemented)

### Async/Await Pattern

The command queue uses `async/await` for sequential command execution while keeping UI responsive. All TM operations wait for animation completion before proceeding (`WaitForTMRotationComplete()`, `WaitForTMArmExtensionComplete()`).

### Resource Management

Images are loaded from `Properties.Resources`:
- `StatusGray`, `StatusGreen`, `StatusYellow`, `StatusRed` for PM status indicators
- `LampOn`, `LampOff` for PM lamp indicators

## Code Architecture Notes

- The application follows a partial separation of concerns: UI logic in views, equipment logic in model classes, but views directly instantiate and manage models
- No EtherCAT implementation exists yet - connection is simulated with boolean flags
- Only Process Module A (PM1) is fully implemented; PM2 (B) and PM3 (C) have models but no UI integration
- Recipe configuration only sets process time; no actual recipe parameters are implemented
- Alarm and Log views are placeholder implementations
