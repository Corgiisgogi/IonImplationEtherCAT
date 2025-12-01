# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

IonImplationEtherCAT is a C# WinForms application (.NET Framework 4.7.2) for ion implantation semiconductor equipment control. The application manages wafer processing through FOUPs (Front Opening Unified Pods), a Transfer Module (TM), and three Process Modules (PM1, PM2, PM3). It supports both real EtherCAT hardware control and simulation mode.

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

### External Dependencies
- `IEG3268_Dll.dll` - EtherCAT hardware control library (HintPath: `D:\2504110105\새 폴더\IEG3268_Dll.dll`)

## High-Level Architecture

### View Layer Structure

The application uses a main form (`MainForm`) with a swappable content panel:

- **MainForm.cs**: Main window with header (login/EtherCAT connection), content panel, and footer navigation
  - Manages `IsLogined`, `IsConnected`, and `IsSimulationMode` static properties
  - Creates `RealEtherCATController` on successful hardware connection, or `SimulationEtherCATController` on failure

- **MainView.cs**: Primary equipment control interface (1600+ lines)
  - Contains the automated workflow state machine in `StartFullAutomatedWorkflow()`
  - Manages three ProcessModules (A=PM1, B=PM2, C=PM3), two FOUPs, and the TransferModule
  - TM animation via 60 FPS timer (`tmAnimationTimer`)
  - Process progress via 1-second timer (`processTimer`)

- **AlarmView.cs**, **RecipeView.cs**, **LogView.cs**: Secondary views (placeholder implementations)

### Equipment Models

**ProcessModule** (`ProcessModule.cs`): Process chamber
- States: `Idle`, `Running`, `Paused`, `Stoped`, `Error`
- ModuleTypes: `PM1` (ion implant), `PM2` (ion implant), `PM3` (annealing)
- `IsUnloadRequested` flag signals workflow to pick up completed wafer
- Hardware integration via `IEtherCATController` for door/lamp control

**TransferModule** (`TransferModule.cs`): Robotic arm
- States: `Idle`, `Moving`, `Rotating`, `PickingWafer`, `PlacingWafer`
- Smooth animations: 90°/sec rotation, 200px/sec extension
- Events: `OnRotationComplete`, `OnArmMovementComplete`

**Foup** (`Foup.cs`): Wafer container with 5 slots (index 0 = bottom/1st floor, index 4 = top/5th floor)

**Wafer** (`Wafer.cs`): Individual wafer with state tracking

### Command Queue System

**CommandQueue** (`CommandQueue.cs`): Async sequential command executor with timing constants for each operation type

**ProcessCommand** (`ProcessCommand.cs`): Command types split into:
- **Simulation commands**: `RotateTM`, `ExtendArm`, `RetractArm`, `PickWafer`, `PlaceWafer`, `StartProcess`, etc.
- **Hardware commands**: `HomeUDAxis`, `HomeLRAxis`, `MoveUDAxis`, `MoveLRAxis`, `OpenPMDoor`, `ClosePMDoor`, `ExtendCylinder`, `RetractCylinder`, `EnableSuction`, `DisableSuction`, `EnableExhaust`, `DisableExhaust`, `ServoOn`, `ServoOff`

### Hardware Abstraction

**IEtherCATController** (`IEtherCATController.cs`): Interface for hardware control
- Axis control: `MoveUDAxis()`, `MoveLRAxis()`, `HomeUDAxis()`, `HomeLRAxis()`
- Servo control: `SetServoUD()`, `SetServoLR()`
- Cylinder/vacuum: `ExtendCylinder()`, `RetractCylinder()`, `EnableSuction()`, `DisableSuction()`, `EnableExhaust()`, `DisableExhaust()`
- PM control: `OpenPMDoor()`, `ClosePMDoor()`, `SetPMLamp()`

**RealEtherCATController** (`RealEtherCATController.cs`): Real hardware implementation using IEG3268_Dll
**SimulationEtherCATController** (`SimulationEtherCATController.cs`): Simulation mode stub

**HardwarePositionMap** (`HardwarePositionMap.cs`): Motor position constants (EtherCAT motor units)
- FOUP A LR: 14140, FOUP B LR: -394293
- PM1 LR: -59064, PM2 LR: -190823, PM3 LR: -321600
- PM UD positions: Seating=776931, Lifted=1156931
- FOUP slot UD arrays: `FOUP_A_UD_SEATING[]` (1층=72379 ~ 5층=2788463), `FOUP_A_UD_LIFTED[]`
- Helper methods: `GetFoupSeatingPosition()`, `GetFoupLiftedPosition()`, `GetPMLRPosition()`

### UI Components

**TMGraphicsPanel** (`TMGraphicsPanel.cs`): Custom panel rendering TM with GDI+ (double-buffered, transparent background)
- Location: (220, 120) in `panelMainControl`, Size: 300x300

## Key Workflows

### Complete Automated Workflow (FOUP A → PM1/PM2 → PM3 → FOUP B)

`StartFullAutomatedWorkflow()` in MainView.cs implements a state-based event loop:
1. **PM3 → FOUP B**: If PM3 has completed wafer (`IsUnloadRequested`), transfer to FOUP B
2. **PM1/PM2 → PM3**: If PM3 empty and PM1 or PM2 has completed wafer, transfer to PM3
3. **FOUP A → PM1**: If PM1 empty and FOUP A has wafers, load PM1
4. **FOUP A → PM2**: If PM2 empty and FOUP A has wafers, load PM2
5. **Wait**: If nothing to do, poll every 500ms

Loop exits when FOUP A is empty, all PMs are empty, and FOUP B contains all wafers.

### Hardware Transfer Sequence (Real Mode)

For FOUP A → PM transfer:
1. Servo ON → UD home → LR to FOUP A
2. UD to slot seating → Cylinder extend → Suction ON
3. Remove wafer data → Pick wafer → UD to lifted → Cylinder retract
4. UD home → LR to PM → PM door open
5. UD to PM lifted → Cylinder extend → UD to PM seating
6. Suction OFF → Exhaust ON/OFF → Place wafer data
7. Cylinder retract → UD home → PM door close → Start process

### Login Credentials
Hardcoded: `admin` / `1234` (MainForm.cs:204-205)

## Coordinate Systems

### TM UI Rotation Angles (simulation)
- FOUP A: -50°
- PM1: 0°
- PM2: 90°
- PM3: 180°
- FOUP B: 230°

### TM Graphics Initial Positions
- ArmHigh: (234, 237)
- ArmLow: (261, 237)
- Bottom (center): (287, 211)
- Back: (472, 237)

## Key Implementation Notes

- `IsRealMode()` in MainView checks if connected to real hardware (not SimulationEtherCATController)
- FOUP wafer extraction is from bottom slot first (slot 0), insertion to bottom empty slot first
- PM lamp blinks at 500ms interval when `IsUnloadRequested` is true (handled by `lampBlinkTimer`)
- Workflow cancellation via `isWorkflowCancelled` flag, checked in the event loop
- All hardware commands have timing delays defined in CommandQueue:
  - Servo: ON=1000ms, OFF=500ms
  - Axis move settle: 1000ms
  - PM door: Open/Close=2500ms
  - Cylinder: Extend/Retract=3000ms
  - Suction: Enable=1500ms, Disable=1000ms
  - Exhaust: Enable=1000ms, Disable=1500ms
  - TM arm: Extend/Retract=1500ms, Settle=1000ms
