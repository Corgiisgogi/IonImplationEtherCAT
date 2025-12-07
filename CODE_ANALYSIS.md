# IonImplationEtherCAT 코드 분석 문서

## 1. 프로젝트 개요

IonImplationEtherCAT는 반도체 이온 주입 장비를 제어하는 C# WinForms 애플리케이션입니다.

### 장비 구성
- **FOUP A/B**: 웨이퍼 5개를 보관하는 컨테이너
- **TM (Transfer Module)**: 웨이퍼를 이송하는 로봇 암
- **PM1/PM2**: 이온 주입 공정 챔버
- **PM3**: 어닐링(열처리) 공정 챔버

### 공정 흐름
```
FOUP A → PM1 또는 PM2 (이온 주입) → PM3 (어닐링) → FOUP B
```

---

## 2. 아키텍처 개요

```
┌─────────────────────────────────────────────────────────────────┐
│                         MainForm.cs                              │
│  (로그인, EtherCAT 연결, View 전환 관리)                           │
└─────────────────────────────────────────────────────────────────┘
                               │
          ┌────────────────────┼────────────────────┐
          ▼                    ▼                    ▼
   ┌────────────┐       ┌────────────┐       ┌────────────┐
   │ MainView   │       │ AlarmView  │       │ RecipeView │
   │ (장비 제어)│       │ (알람 관리) │       │ (레시피)   │
   └────────────┘       └────────────┘       └────────────┘
          │
          ▼
   ┌──────────────────────────────────────────────────────────────┐
   │                      CommandQueue                             │
   │  (비동기 명령 큐 - 하드웨어/시뮬레이션 명령 순차 실행)           │
   └──────────────────────────────────────────────────────────────┘
          │
   ┌──────┴───────┬───────────────┬────────────────┐
   ▼              ▼               ▼                ▼
┌─────────┐  ┌─────────┐   ┌─────────────┐   ┌────────────┐
│ProcessModule│ │TransferModule│  │ Foup       │   │ Wafer      │
│(PM1/2/3) │  │ (TM)         │   │ (A/B)      │   │            │
└─────────┘  └─────────┘   └─────────────┘   └────────────┘
                               │
          ┌────────────────────┴────────────────────┐
          ▼                                         ▼
   ┌──────────────────────┐               ┌──────────────────────┐
   │ IEtherCATController  │               │ LogManager (싱글톤)   │
   │     (인터페이스)      │               │ (로그/알람 관리)       │
   └──────────────────────┘               └──────────────────────┘
          │
   ┌──────┴───────┐
   ▼              ▼
┌─────────────┐ ┌───────────────────────┐
│RealEtherCAT │ │SimulationEtherCAT     │
│Controller   │ │Controller             │
│(실제 장비)   │ │(시뮬레이션)            │
└─────────────┘ └───────────────────────┘
```

---

## 3. 클래스 분할 기준 및 역할

### 3.1 책임 분리 원칙 (SRP)

각 클래스는 단일 책임을 가지도록 설계되었습니다:

| 클래스 | 책임 | 파일 |
|--------|------|------|
| `MainForm` | 전체 앱 관리 (로그인, 연결, View 전환) | MainForm.cs |
| `MainView` | 장비 UI 및 워크플로우 제어 | MainView.cs |
| `ProcessModule` | 개별 PM 상태 및 공정 관리 | ProcessModule.cs |
| `TransferModule` | TM 로봇 상태 및 애니메이션 | TransferModule.cs |
| `Foup` | 웨이퍼 컨테이너 데이터 | Foup.cs |
| `Wafer` | 개별 웨이퍼 상태 추적 | Wafer.cs |
| `CommandQueue` | 비동기 명령 실행 엔진 | CommandQueue.cs |
| `ProcessCommand` | 명령 데이터 구조 정의 | ProcessCommand.cs |
| `LogManager` | 중앙 로그/알람 관리 (싱글톤) | LogManager.cs |

### 3.2 하드웨어 추상화 (인터페이스)

```csharp
// IEtherCATController.cs - 하드웨어 인터페이스
public interface IEtherCATController
{
    // 축 제어 - 비동기 반환
    Task<bool> MoveUDAxis(long position);
    Task<bool> MoveLRAxis(long position);

    // 서보 제어 - 동기 (즉시 실행)
    void SetServoUD(bool on);
    void SetServoLR(bool on);

    // PM 제어
    void OpenPMDoor(ProcessModule.ModuleType pm);
    void ClosePMDoor(ProcessModule.ModuleType pm);
}
```

**왜 인터페이스를 사용하는가?**

1. **실제 하드웨어 없이 개발 가능**: `SimulationEtherCATController`를 사용하면 장비 없이 소프트웨어 테스트 가능
2. **런타임 전환**: 연결 시점에 실제/시뮬레이션 컨트롤러 선택
3. **테스트 용이성**: 테스트 환경에서 Mock 객체 주입 가능

```csharp
// MainForm.cs - 연결 시 컨트롤러 결정
if (EtherCAT_M.CIFX_50RE_Connect() == true)
{
    // 실제 하드웨어 연결 성공
    etherCATController = new RealEtherCATController(EtherCAT_M);
    IsSimulationMode = false;
}
else
{
    // 연결 실패 시 시뮬레이션으로 전환
    etherCATController = new SimulationEtherCATController();
    IsSimulationMode = true;
}
```

---

## 4. 비동기 프로그래밍 (Task와 async/await)

### 4.1 async/await 기본 개념

`async`와 `await`는 비동기 작업을 동기 코드처럼 읽기 쉽게 작성하는 C# 기능입니다.

```csharp
// CommandQueue.cs
public async Task ExecuteAsync()
{
    while (commands.Count > 0 && isExecuting)
    {
        var command = commands.Dequeue();

        // await: 이 작업이 완료될 때까지 기다림
        // 하지만 UI 스레드는 차단되지 않음!
        bool success = await ExecuteCommandAsync(command, previousCommand);

        if (!success)
        {
            // 실패 시 큐 비우고 종료
            commands.Clear();
            return;
        }
    }
}
```

**핵심 포인트:**
- `await`는 비동기 작업이 완료될 때까지 **현재 메서드만** 일시 중단
- UI 스레드는 차단되지 않아 UI가 멈추지 않음
- 작업 완료 후 원래 컨텍스트(UI 스레드)에서 이어서 실행

### 4.2 Task<bool> 반환 타입

`Task<bool>`는 "나중에 bool 값을 반환하는 작업"을 의미합니다.

```csharp
// IEtherCATController.cs
Task<bool> MoveUDAxis(long position);  // 이동 완료 시 true 반환

// RealEtherCATController.cs - 구현
public async Task<bool> MoveUDAxis(long position)
{
    if (etherCAT == null) return false;  // 즉시 실패 반환

    etherCAT.Axis1_UD_POS_Update(position);  // 하드웨어 명령
    etherCAT.Axis1_UD_Move_Send();           // 이동 시작

    await Task.Delay(UD_MOVE_DELAY);  // 완료 대기 (1300ms)
    return true;  // 성공 반환
}
```

**왜 Task<bool>을 사용하는가?**

1. **성공/실패 판단**: 호출자가 결과에 따라 다음 동작 결정
2. **에러 전파**: 예외 대신 false 반환으로 안전한 에러 처리

### 4.3 Task.WhenAll - 병렬 실행

여러 비동기 작업을 동시에 실행하고 모두 완료될 때까지 기다립니다.

```csharp
// MainView.cs - 하드웨어와 애니메이션 동시 실행
public async Task InitializeHardwareAsync()
{
    // 애니메이션 작업 시작 (즉시 반환)
    Task animationTask = ReturnTMToHomeAsync();

    // 하드웨어 명령 큐 실행 시작
    Task hardwareTask = commandQueue.ExecuteAsync();

    // 둘 다 완료될 때까지 대기
    await Task.WhenAll(hardwareTask, animationTask);
}
```

**시각적 표현:**
```
시간 ──────────────────────────────────────────▶

animationTask:  [███████████████]          (1500ms)
hardwareTask:   [██████████████████████]   (2500ms)
                                        ↑
                              Task.WhenAll 완료
```

### 4.4 Task.Delay - 비동기 대기

```csharp
// 일반 Thread.Sleep과 차이점
Thread.Sleep(1000);      // UI 스레드 차단! (앱 멈춤)
await Task.Delay(1000);  // UI 스레드 유지 (앱 반응)
```

```csharp
// CommandQueue.cs - 공정 완료 대기
case CommandType.WaitForProcessComplete:
    while (pmWait.ModuleState == ProcessModule.State.Running)
    {
        await Task.Delay(500);  // 0.5초마다 상태 확인
    }
    return true;
```

---

## 5. 이벤트 기반 프로그래밍

### 5.1 C# 이벤트 시스템

이벤트는 "어떤 일이 발생했을 때 알려주는" 시스템입니다.

```csharp
// LogManager.cs - 이벤트 정의
public event Action<LogEntry> OnLogAdded;      // 로그 추가 시 발생
public event Action<LogEntry> OnAlarmRestored; // 알람 복구 시 발생

// 이벤트 발생 (호출)
public void AddLog(string job, string description, ...)
{
    var entry = new LogEntry(...);
    _logs.Add(entry);

    // 이벤트 발생 (구독자에게 알림)
    OnLogAdded?.Invoke(entry);  // ?.는 null 체크
}
```

```csharp
// MainForm.cs - 이벤트 구독
public MainForm()
{
    // 이벤트 구독 (이벤트가 발생하면 메서드 호출)
    LogManager.Instance.OnLogAdded += OnAlarmAdded;
    LogManager.Instance.OnAlarmRestored += OnAlarmRestored;
}

// 이벤트 핸들러 (이벤트 발생 시 실행되는 메서드)
private void OnAlarmAdded(LogEntry entry)
{
    if (entry.IsAlarm || entry.IsWarning)
    {
        UpdateAlertPanel();  // UI 업데이트
    }
}

// 정리 (메모리 누수 방지)
private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
{
    LogManager.Instance.OnLogAdded -= OnAlarmAdded;
    LogManager.Instance.OnAlarmRestored -= OnAlarmRestored;
}
```

### 5.2 TransferModule 완료 이벤트

```csharp
// TransferModule.cs
public event Action OnRotationComplete;     // 회전 완료 시
public event Action OnArmMovementComplete;  // 암 이동 완료 시

public bool Update(float deltaTime)
{
    if (State == TMState.Rotating)
    {
        // 회전 완료 체크
        if (Math.Abs(CurrentRotationAngle - TargetRotationAngle) < 0.1f)
        {
            State = TMState.Idle;
            OnRotationComplete?.Invoke();  // 이벤트 발생
        }
    }
}
```

---

## 6. 타이머와 스레드 사용

### 6.1 WinForms Timer

WinForms의 `Timer`는 **UI 스레드에서 실행**됩니다. 별도 스레드 동기화 불필요.

```csharp
// MainView.cs - 타이머 초기화
public MainView()
{
    // 공정 업데이트 타이머 (1초마다)
    processTimer = new Timer();
    processTimer.Interval = 1000;  // 1000ms = 1초
    processTimer.Tick += ProcessTimer_Tick;  // 이벤트 핸들러 연결
    processTimer.Start();

    // 애니메이션 타이머 (약 60 FPS)
    tmAnimationTimer = new Timer();
    tmAnimationTimer.Interval = 16;  // 약 60fps (1000ms/60 ≈ 16ms)
    tmAnimationTimer.Tick += TmAnimationTimer_Tick;
    tmAnimationTimer.Start();

    // 램프 점멸 타이머 (0.5초 간격)
    lampBlinkTimer = new Timer();
    lampBlinkTimer.Interval = 500;
    lampBlinkTimer.Tick += LampBlinkTimer_Tick;
    lampBlinkTimer.Start();
}
```

### 6.2 각 타이머의 역할

| 타이머 | 간격 | 역할 |
|--------|------|------|
| `processTimer` | 1초 | PM 공정 시간 업데이트, 파라미터 변화 |
| `tmAnimationTimer` | 16ms | TM 로봇 암 회전/확장 애니메이션 |
| `lampBlinkTimer` | 500ms | PM 완료 시 램프 점멸 효과 |

```csharp
// 공정 타이머 - 매 1초마다 실행
private void ProcessTimer_Tick(object sender, EventArgs e)
{
    // PM1 업데이트
    if (processModuleA.ModuleState == ProcessModule.State.Running)
    {
        processModuleA.UpdateProcess(1);  // 1초 증가
    }
    else
    {
        // Idle 상태에서 파라미터 하강 애니메이션
        processModuleA.UpdateParametersWhenIdle();
    }

    // UI 갱신
    UpdateProcessDisplay();
    UpdateAllPMParameters();
}
```

```csharp
// 애니메이션 타이머 - 매 16ms마다 실행 (60 FPS)
private void TmAnimationTimer_Tick(object sender, EventArgs e)
{
    DateTime now = DateTime.Now;
    float deltaTime = (float)(now - lastAnimationUpdate).TotalSeconds;
    lastAnimationUpdate = now;

    // TM 상태 업데이트 (회전/확장 애니메이션)
    bool isAnimating = transferModule.Update(deltaTime);

    if (isAnimating)
    {
        // TM 그래픽 다시 그리기
        tmGraphicsPanel.Invalidate();
    }
}
```

### 6.3 스레드 안전성 (Thread Safety)

UI 스레드가 아닌 곳에서 UI를 업데이트하면 에러가 발생합니다.

```csharp
// MainForm.cs - InvokeRequired 패턴
private void UpdateAlertPanel()
{
    // 현재 스레드가 UI 스레드가 아닌지 확인
    if (this.InvokeRequired)
    {
        // UI 스레드에서 다시 호출
        this.Invoke(new Action(UpdateAlertPanel));
        return;
    }

    // 여기서부터는 UI 스레드에서 안전하게 실행
    var activeAlarms = LogManager.Instance.GetActiveAlarms();
    // ... UI 업데이트 코드
}
```

```csharp
// LogManager.cs - lock을 사용한 스레드 안전성
private readonly object _lockObject = new object();

public List<LogEntry> GetActiveAlarms()
{
    lock (_lockObject)  // 한 번에 하나의 스레드만 접근 가능
    {
        return _logs.Where(l => (l.IsAlarm || l.IsWarning) && !l.IsRestored)
                    .ToList();
    }
}
```

---

## 7. 디자인 패턴

### 7.1 싱글톤 패턴 (Singleton)

전역에서 하나의 인스턴스만 존재하는 패턴.

```csharp
// LogManager.cs
public class LogManager
{
    // Lazy<T>: 처음 사용될 때 인스턴스 생성
    private static readonly Lazy<LogManager> _instance =
        new Lazy<LogManager>(() => new LogManager());

    // 전역 접근점
    public static LogManager Instance => _instance.Value;

    // private 생성자: 외부에서 new 불가
    private LogManager()
    {
        _logs = new List<LogEntry>();
        InitializeLogFolder();
    }
}

// 사용 예시
LogManager.Instance.AddLog("PM1 공정", "공정 시작", "PM1", LogCategory.Process, false);
```

### 7.2 커맨드 패턴 (Command)

동작을 객체로 캡슐화하여 큐에 저장하고 순차 실행.

```csharp
// ProcessCommand.cs - 명령 정의
public class ProcessCommand
{
    public CommandType Type { get; set; }        // 명령 종류
    public object[] Parameters { get; set; }     // 매개변수
    public bool IsCompleted { get; set; }        // 완료 여부
    public Wafer ResultWafer { get; set; }       // 결과 웨이퍼

    public ProcessCommand(CommandType type, params object[] parameters)
    {
        Type = type;
        Parameters = parameters;
    }
}
```

```csharp
// CommandQueue.cs - 명령 실행 엔진
public async Task ExecuteAsync()
{
    while (commands.Count > 0 && isExecuting)
    {
        var command = commands.Dequeue();
        bool success = await ExecuteCommandAsync(command, previousCommand);
        // ...
    }
}
```

```csharp
// MainView.cs - 명령 사용 예시
private async Task TransferWaferFromFoupAToIonPM(ProcessModule targetPM)
{
    // 명령들을 큐에 추가
    commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, ANGLE_FOUP_A));
    commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm));
    commandQueue.Enqueue(new ProcessCommand(CommandType.RemoveWaferFromFoup, foupA, waferSlot));
    commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer));
    commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm));
    // ... 더 많은 명령

    // 모든 명령 순차 실행
    await commandQueue.ExecuteAsync();
}
```

### 7.3 전략 패턴 (Strategy)

인터페이스를 통해 런타임에 구현체 교체.

```csharp
// 인터페이스
public interface IEtherCATController
{
    Task<bool> MoveUDAxis(long position);
    // ...
}

// 구현체 1: 실제 하드웨어
public class RealEtherCATController : IEtherCATController
{
    public async Task<bool> MoveUDAxis(long position)
    {
        etherCAT.Axis1_UD_POS_Update(position);
        etherCAT.Axis1_UD_Move_Send();
        await Task.Delay(UD_MOVE_DELAY);
        return true;
    }
}

// 구현체 2: 시뮬레이션
public class SimulationEtherCATController : IEtherCATController
{
    public async Task<bool> MoveUDAxis(long position)
    {
        // 가상으로 위치 이동 시뮬레이션
        while (Math.Abs(currentUDPosition - position) > 100)
        {
            currentUDPosition += Math.Sign(position - currentUDPosition) * SIMULATION_SPEED;
            await Task.Delay(50);
        }
        return true;
    }
}
```

---

## 8. 데이터 흐름 및 워크플로우

### 8.1 자동 워크플로우 상태 머신

```csharp
// MainView.cs - StartFullAutomatedWorkflow()
private async Task StartFullAutomatedWorkflow()
{
    while (!isWorkflowCancelled)
    {
        // 1단계: PM3 완료된 웨이퍼 → FOUP B로 이송
        if (processModuleC.IsUnloadRequested && processModuleC.isWaferLoaded)
        {
            await TransferWaferFromPM3ToFoupB();
            continue;  // 다음 루프
        }

        // 2단계: PM1/PM2 완료된 웨이퍼 → PM3로 이송
        ProcessModule completedPM = GetCompletedIonPM();
        if (processModuleC.CanAcceptWafer() && completedPM != null)
        {
            await TransferWaferFromIonPMToPM3(completedPM);
            continue;
        }

        // 3단계: FOUP A → PM1 이송
        if (!processModuleA.isWaferLoaded && !foupA.IsEmpty)
        {
            await TransferWaferFromFoupAToIonPM(processModuleA);
            continue;
        }

        // 4단계: FOUP A → PM2 이송
        if (!processModuleB.isWaferLoaded && !foupA.IsEmpty)
        {
            await TransferWaferFromFoupAToIonPM(processModuleB);
            continue;
        }

        // 5단계: 할 일 없으면 대기
        await Task.Delay(500);

        // 종료 조건: FOUP A 비고, 모든 PM 비고, FOUP B에 모든 웨이퍼
        if (IsWorkflowComplete()) break;
    }
}
```

**워크플로우 시각화:**
```
┌─────────────────────────────────────────────────────────────────┐
│                        Workflow Loop                             │
└─────────────────────────────────────────────────────────────────┘
                               │
        ┌──────────────────────┴──────────────────────┐
        ▼                                              ▼
 [PM3 완료?] ──Yes→ [PM3 → FOUP B] ──────────────────→ continue
        │
        No
        ▼
 [PM1/2 완료?] ──Yes→ [PM1/2 → PM3] ─────────────────→ continue
        │
        No
        ▼
 [PM1 비어있음?] ──Yes→ [FOUP A → PM1] ──────────────→ continue
        │
        No
        ▼
 [PM2 비어있음?] ──Yes→ [FOUP A → PM2] ──────────────→ continue
        │
        No
        ▼
 [500ms 대기] ──→ [종료 조건?] ──Yes→ END
                        │
                        No
                        ↓
                    ← 루프 ←
```

### 8.2 웨이퍼 이송 시퀀스 (실제 모드)

```csharp
// FOUP A → PM 이송 명령 시퀀스
private async Task TransferWaferFromFoupAToIonPM(ProcessModule targetPM)
{
    // 1. 서보 ON
    commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOn));

    // 2. UD축 원점복귀
    commandQueue.Enqueue(new ProcessCommand(CommandType.HomeUDAxis));

    // 3. LR축 FOUP A 위치로 이동
    commandQueue.Enqueue(new ProcessCommand(CommandType.MoveLRAxis,
        HardwarePositionMap.LR_FOUP_A));

    // 4. UD축 안착 위치로 이동 (웨이퍼 아래로)
    commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis,
        HardwarePositionMap.GetFoupSeatingPosition(waferSlot, true)));

    // 5. 실린더 전진 (웨이퍼 아래로 암 삽입)
    commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendCylinder));

    // 6. 흡착 ON (웨이퍼 집기)
    commandQueue.Enqueue(new ProcessCommand(CommandType.EnableSuction));

    // 7. 웨이퍼 데이터 제거 (FOUP에서)
    commandQueue.Enqueue(new ProcessCommand(CommandType.RemoveWaferFromFoup,
        foupA, waferSlot));

    // 8. UD축 상승 (웨이퍼 들어올리기)
    commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis,
        HardwarePositionMap.GetFoupLiftedPosition(waferSlot, true)));

    // 9. 실린더 후진
    commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder));

    // ... PM으로 이동 및 배치 명령들

    // 마지막: 공정 시작
    commandQueue.Enqueue(new ProcessCommand(CommandType.StartProcess, targetPM));

    // 모든 명령 실행
    await commandQueue.ExecuteAsync();
}
```

---

## 9. 하드웨어 위치 매핑

```csharp
// HardwarePositionMap.cs - 모터 좌표 상수
public static class HardwarePositionMap
{
    // LR축 (좌우/회전) 위치
    public const long LR_FOUP_A = 13226;
    public const long LR_PM1 = -59064;
    public const long LR_PM2 = -190823;
    public const long LR_PM3 = -322000;
    public const long LR_FOUP_B = -395219;

    // FOUP 슬롯별 UD축 위치 (안착/상승)
    public static readonly long[] FOUP_A_UD_SEATING = new long[]
    {
        125188,   // 1층
        789441,   // 2층
        1482288,  // 3층
        2143669,  // 4층
        2833894   // 5층
    };

    // PM UD축 위치
    public const long PM_UD_SEATING = 806931;   // 웨이퍼 아래
    public const long PM_UD_LIFTED = 1156931;   // 웨이퍼 들어올림
}
```

---

## 10. 공정 파라미터 시뮬레이션

### 10.1 ProcessParameters 상태 머신

```csharp
// ProcessParameters.cs
public class ProcessParameters
{
    // 상태 플래그
    public bool IsRising { get; private set; }   // 상승 중
    public bool IsFalling { get; private set; }  // 하강 중
    public bool IsStable { get; private set; }   // 안정 상태

    // 현재 값
    public double CurrentTemperature { get; private set; }
    public double CurrentPressure { get; private set; }
    public double CurrentHV { get; private set; }
    public double CurrentDose { get; private set; }

    // 공정 시작 → 상승 시작
    public void StartRising()
    {
        IsRising = true;
        IsFalling = false;
        IsStable = false;

        // 10초 안에 목표값 도달하도록 속도 계산
        calculatedTempRiseRate = (TargetTemperature - 25) / 10.0;
    }

    // 공정 종료 → 하강 시작
    public void StartFalling()
    {
        IsRising = false;
        IsFalling = true;
        IsStable = false;
    }
}
```

### 10.2 이온 주입 공정 파라미터 변화

```
시간 ──────────────────────────────────────────────────────────────▶

Pressure: ████████─────────────────────────────────────────▓▓▓▓▓▓
          (진공 펌핑)                                      (벤트)

Temperature: ───▓▓▓▓████████████████████████████████████▓▓▓───
                (상승)                                   (냉각)

HV:          ─────▓▓▓▓▓████████████████████████████▓▓▓▓─────
                  (상승)                           (하강)

Dose:            ────────────────▓▓▓▓▓▓▓▓▓▓▓▓▓████
                                 (적산 시작)    (완료)

         0   3   5   8   10      (이온주입중)     -10  -5  -2  완료
```

---

## 11. 에러 처리 및 로깅

### 11.1 알람 시스템

```csharp
// CommandQueue.cs - 하드웨어 명령 실패 시 알람 발생
case CommandType.HomeUDAxis:
    bool success = await controller.HomeUDAxis();
    if (!success)
    {
        // 알람 로그 기록 (빨간색으로 표시)
        LogManager.Instance.Alarm("UD축 원점복귀 실패", "TM");
        return false;  // 워크플로우 중단
    }
    break;
```

### 11.2 로그 카테고리

```csharp
public enum LogCategory
{
    Transfer,   // 웨이퍼 이동
    Process,    // 공정 시작/완료
    Hardware,   // 하드웨어 동작
    System,     // 시스템 이벤트
    Error,      // 오류 (알람)
    Warning     // 경고
}
```

---

## 12. 핵심 개념 요약

### 12.1 async/await 사용 시점

| 상황 | 사용 여부 | 이유 |
|------|----------|------|
| 하드웨어 동작 대기 | O | UI 차단 방지 |
| 파일 읽기/쓰기 | O | I/O 대기 시 CPU 활용 |
| CPU 집약적 계산 | X | Task.Run 사용 권장 |
| 단순 UI 업데이트 | X | 즉시 실행 가능 |

### 12.2 클래스 분할 원칙

1. **단일 책임**: 각 클래스는 하나의 역할만
2. **인터페이스 분리**: 하드웨어 추상화
3. **데이터/로직 분리**: Model 클래스와 Controller 분리

### 12.3 스레드 안전성 규칙

1. UI 업데이트는 항상 UI 스레드에서
2. 공유 데이터 접근 시 `lock` 사용
3. `InvokeRequired` 패턴으로 스레드 체크

---

## 13. 코드 파일 목록

| 파일 | 역할 | 핵심 기능 |
|------|------|----------|
| MainForm.cs | 메인 폼 | 로그인, 연결, View 전환 |
| MainView.cs | 메인 뷰 | 장비 UI, 워크플로우 |
| CommandQueue.cs | 명령 큐 | 비동기 명령 실행 |
| ProcessModule.cs | PM 모델 | 공정 상태 관리 |
| TransferModule.cs | TM 모델 | 로봇 암 상태/애니메이션 |
| Foup.cs | FOUP 모델 | 웨이퍼 슬롯 관리 |
| Wafer.cs | 웨이퍼 모델 | 개별 웨이퍼 상태 |
| IEtherCATController.cs | 인터페이스 | 하드웨어 추상화 |
| RealEtherCATController.cs | 실제 컨트롤러 | EtherCAT 하드웨어 |
| SimulationEtherCATController.cs | 시뮬레이션 | 가상 하드웨어 |
| LogManager.cs | 싱글톤 | 로그/알람 관리 |
| ProcessParameters.cs | 파라미터 | 온도/압력/HV 시뮬레이션 |
| HardwarePositionMap.cs | 좌표 맵 | 모터 위치 상수 |
| ProcessCommand.cs | 명령 정의 | 명령 타입/파라미터 |
