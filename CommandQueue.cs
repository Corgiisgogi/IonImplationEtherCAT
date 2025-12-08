using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 명령 큐 관리 클래스
    /// </summary>
    public class CommandQueue
    {
        private Queue<ProcessCommand> commands;
        private bool isExecuting;
        private MainView mainView;

        // Stop 요청 플래그 - 현재 명령 완료 후 중단
        private bool stopRequested;

        // 알람 발생으로 인한 즉시 중단 플래그
        private bool alarmStopRequested;

        public bool IsExecuting => isExecuting;
        public int CommandCount => commands.Count;
        public bool IsStopRequested => stopRequested;
        public bool IsAlarmStopRequested => alarmStopRequested;

        #region TM 각도-LR 위치 매핑 상수

        // TM UI 회전 각도
        private const float ANGLE_FOUP_A = -50f;
        private const float ANGLE_PM1 = 0f;
        private const float ANGLE_PM2 = 90f;
        private const float ANGLE_PM3 = 180f;
        private const float ANGLE_FOUP_B = 230f;

        #endregion

        #region 동작 대기 시간 상수 (ms)

        // 서보 제어
        private const int SERVO_ON_DELAY = 1000;        // 서보 ON 안정화
        private const int SERVO_OFF_DELAY = 500;        // 서보 OFF 안정화

        // 축 이동 (원점복귀는 컨트롤러에서 완료 대기)
        private const int AXIS_MOVE_SETTLE_DELAY = 1000; // 축 이동 후 안정화

        // PM 문 제어
        private const int PM_DOOR_OPEN_DELAY = 1200;    // 문 열림 대기
        private const int PM_DOOR_CLOSE_DELAY = 1200;   // 문 닫힘 대기
        private const int PM_LAMP_DELAY = 100;          // 램프 ON/OFF 대기

        // TM 실린더/흡착 제어
        private const int CYLINDER_EXTEND_DELAY = 1500; // 실린더 전진 대기
        private const int CYLINDER_RETRACT_DELAY = 1500;// 실린더 후진 대기
        private const int SUCTION_ENABLE_DELAY = 800;  // 흡착 ON 안정화
        private const int SUCTION_DISABLE_DELAY = 800;  // 흡착 OFF 대기
        private const int EXHAUST_ENABLE_DELAY = 1000;   // 배기 ON 대기
        private const int EXHAUST_DISABLE_DELAY = 1200;  // 배기 OFF 대기

        // 웨이퍼 처리
        private const int WAFER_PICK_DELAY = 500;       // 웨이퍼 픽업 대기
        private const int WAFER_PLACE_DELAY = 500;      // 웨이퍼 배치 대기
        private const int WAFER_UNLOAD_DELAY = 500;     // 웨이퍼 언로드 대기
        private const int FOUP_WAFER_DELAY = 300;       // FOUP 웨이퍼 제거/추가 대기

        // TM 시뮬레이션 동작 (고정 대기시간)
        private const int TM_ARM_EXTEND_DELAY = 1500;   // TM 암 확장 대기
        private const int TM_ARM_RETRACT_DELAY = 1500;  // TM 암 수축 대기
        private const int TM_SETTLE_DELAY = 1000;       // TM 동작 후 안정화 대기

        #endregion

        public CommandQueue(MainView view)
        {
            commands = new Queue<ProcessCommand>();
            isExecuting = false;
            stopRequested = false;
            mainView = view;
        }

        /// <summary>
        /// 명령 추가
        /// </summary>
        public void Enqueue(ProcessCommand command)
        {
            commands.Enqueue(command);
        }

        /// <summary>
        /// 여러 명령 추가
        /// </summary>
        public void EnqueueRange(IEnumerable<ProcessCommand> commandList)
        {
            foreach (var cmd in commandList)
            {
                commands.Enqueue(cmd);
            }
        }

        /// <summary>
        /// 큐 초기화 및 실행 중단
        /// </summary>
        public void Clear()
        {
            commands.Clear();
            // 실행 중이던 워크플로우도 중단
            isExecuting = false;
        }

        /// <summary>
        /// 실행 중단 요청 - 현재 명령 완료 후 중단
        /// </summary>
        public void RequestStop()
        {
            stopRequested = true;
        }

        /// <summary>
        /// 즉시 중단 및 큐 초기화
        /// </summary>
        public void Stop()
        {
            stopRequested = true;
            commands.Clear();
            isExecuting = false;
        }

        /// <summary>
        /// Stop 요청 플래그 리셋
        /// </summary>
        public void ResetStopFlag()
        {
            stopRequested = false;
        }

        /// <summary>
        /// 알람 발생으로 인한 즉시 중단 요청
        /// </summary>
        public void RequestAlarmStop()
        {
            alarmStopRequested = true;
            stopRequested = true;
        }

        /// <summary>
        /// 알람 Stop 플래그 리셋
        /// </summary>
        public void ResetAlarmStopFlag()
        {
            alarmStopRequested = false;
        }

        /// <summary>
        /// 명령 실행 시작
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (isExecuting)
                return;

            isExecuting = true;
            stopRequested = false; // 실행 시작 시 stop 플래그 초기화
            // alarmStopRequested는 외부에서 설정되므로 여기서 초기화하지 않음
            ProcessCommand previousCommand = null;

            while (commands.Count > 0 && isExecuting)
            {
                // Stop 또는 알람 중지 요청 확인
                if (stopRequested || alarmStopRequested)
                {
                    commands.Clear();
                    isExecuting = false;
                    return;
                }

                var command = commands.Dequeue();
                bool success = await ExecuteCommandAsync(command, previousCommand);

                // 명령 실행 중 알람 발생 확인
                if (alarmStopRequested)
                {
                    commands.Clear();
                    isExecuting = false;
                    return;
                }

                if (!success)
                {
                    // 명령 실패 시 워크플로우 중단
                    commands.Clear();
                    isExecuting = false;
                    mainView.ShowErrorMessage($"명령 실패: {command.Description}");
                    return;
                }

                command.IsCompleted = true;
                previousCommand = command;

                // 각 명령 완료 후 Stop 요청 확인
                if (stopRequested || alarmStopRequested)
                {
                    commands.Clear();
                    isExecuting = false;
                    return;
                }
            }

            isExecuting = false;
        }

        /// <summary>
        /// 단일 명령 실행 (성공 시 true, 실패 시 false 반환)
        /// </summary>
        private async Task<bool> ExecuteCommandAsync(ProcessCommand command, ProcessCommand previousCommand)
        {
            switch (command.Type)
            {
                case CommandType.RotateTM:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is float angle)
                    {
                        var controller = mainView.GetEtherCATController();
                        bool isRealMode = controller != null && !(controller is SimulationEtherCATController);

                        if (isRealMode)
                        {
                            // 실제 모드: 장비 LR축 이동과 애니메이션 동시 실행
                            long lrPosition = GetLRPositionFromAngle(angle);

                            // 장비 명령 비동기 시작
                            Task hardwareTask = controller.MoveLRAxis(lrPosition);

                            // 동시에 애니메이션 시작
                            mainView.RotateTM(angle);
                            Task animationTask = WaitForTMRotationComplete();

                            // 둘 다 완료 대기
                            await Task.WhenAll(hardwareTask, animationTask);
                        }
                        else
                        {
                            // 시뮬레이션 모드: 애니메이션만 실행
                            mainView.RotateTM(angle);
                            await WaitForTMRotationComplete();
                        }
                        await Task.Delay(TM_SETTLE_DELAY); // 안정화 대기
                    }
                    return true;

                case CommandType.ExtendArm:
                    {
                        var controller = mainView.GetEtherCATController();
                        bool isRealMode = controller != null && !(controller is SimulationEtherCATController);

                        if (isRealMode)
                        {
                            // 실제 모드: 장비 실린더 전진과 애니메이션 동시 실행
                            controller.ExtendCylinder();

                            // 애니메이션 시작
                            mainView.ExtendTMArm();

                            // 애니메이션 완료와 실린더 대기 중 더 긴 시간 대기
                            Task animationTask = WaitForTMArmExtensionComplete();
                            Task hardwareTask = Task.Delay(CYLINDER_EXTEND_DELAY);
                            await Task.WhenAll(animationTask, hardwareTask);
                        }
                        else
                        {
                            // 시뮬레이션 모드: 애니메이션만 실행
                            mainView.ExtendTMArm();
                            await WaitForTMArmExtensionComplete();
                        }
                        await Task.Delay(TM_SETTLE_DELAY); // 안정화 대기
                    }
                    return true;

                case CommandType.RetractArm:
                    {
                        var controller = mainView.GetEtherCATController();
                        bool isRealMode = controller != null && !(controller is SimulationEtherCATController);

                        if (isRealMode)
                        {
                            // 실제 모드: 장비 실린더 후진과 애니메이션 동시 실행
                            controller.RetractCylinder();

                            // 애니메이션 시작
                            mainView.RetractTMArm();

                            // 애니메이션 완료와 실린더 대기 중 더 긴 시간 대기
                            Task animationTask = WaitForTMArmRetractionComplete();
                            Task hardwareTask = Task.Delay(CYLINDER_RETRACT_DELAY);
                            await Task.WhenAll(animationTask, hardwareTask);
                        }
                        else
                        {
                            // 시뮬레이션 모드: 애니메이션만 실행
                            mainView.RetractTMArm();
                            await WaitForTMArmRetractionComplete();
                        }
                        await Task.Delay(TM_SETTLE_DELAY); // 안정화 대기
                    }
                    return true;

                case CommandType.PickWafer:
                    // 이전 명령의 ResultWafer를 사용 (RemoveWaferFromFoup에서 설정됨)
                    Wafer waferToPick = previousCommand?.ResultWafer;
                    if (waferToPick != null)
                    {
                        mainView.TMPickWafer(waferToPick);
                    }
                    await Task.Delay(WAFER_PICK_DELAY); // 픽업 동작 완료 대기
                    return true;

                case CommandType.PlaceWafer:
                    Wafer placedWafer = mainView.TMPlaceWafer();
                    // 배치된 웨이퍼를 파라미터로 저장 (다음 명령에서 사용 가능)
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pm)
                    {
                        pm.LoadWafer(placedWafer, "TM");  // TM에서 PM으로 이동
                    }
                    await Task.Delay(WAFER_PLACE_DELAY); // 배치 동작 완료 대기
                    return true;

                case CommandType.RemoveWaferFromFoup:
                    if (command.Parameters.Length > 1 &&
                        command.Parameters[0] is Foup foup &&
                        command.Parameters[1] is int slotIndex)
                    {
                        Wafer removedWafer = foup.UnloadWafer(slotIndex);
                        // 제거된 웨이퍼를 다음 명령에서 사용할 수 있도록 임시 저장
                        command.ResultWafer = removedWafer;
                        mainView.UpdateFoupDisplays();

                        // FOUP에서 웨이퍼 꺼내기 로그
                        string foupName = !string.IsNullOrEmpty(foup.Name) ? foup.Name : "FOUP";
                        LogManager.Instance.AddLog("웨이퍼 이동",
                            $"{foupName} (슬롯 {slotIndex + 1}) → TM 웨이퍼 이동 완료",
                            foupName, LogCategory.Transfer, false);
                    }
                    await Task.Delay(FOUP_WAFER_DELAY); // FOUP 웨이퍼 제거 대기
                    return true;

                case CommandType.AddWaferToFoup:
                    if (command.Parameters.Length > 1 &&
                        command.Parameters[0] is Foup foupAdd &&
                        command.Parameters[1] is int slotIndexAdd)
                    {
                        // TM이 웨이퍼를 들고 있으면 TM에서 가져옴
                        Wafer waferToAdd = null;
                        if (mainView.TMHasWafer())
                        {
                            waferToAdd = mainView.TMPlaceWafer();
                        }
                        else
                        {
                            // TM에 웨이퍼가 없으면 이전 명령의 ResultWafer 사용
                            waferToAdd = previousCommand?.ResultWafer;
                        }

                        if (waferToAdd != null)
                        {
                            foupAdd.LoadWafer(slotIndexAdd, waferToAdd);
                        }
                        else
                        {
                            foupAdd.LoadWafer(slotIndexAdd);
                        }
                        mainView.UpdateFoupDisplays();

                        // FOUP에 웨이퍼 넣기 로그
                        string foupAddName = !string.IsNullOrEmpty(foupAdd.Name) ? foupAdd.Name : "FOUP";
                        LogManager.Instance.AddLog("웨이퍼 이동",
                            $"TM → {foupAddName} (슬롯 {slotIndexAdd + 1}) 웨이퍼 이동 완료",
                            foupAddName, LogCategory.Transfer, false);
                    }
                    await Task.Delay(FOUP_WAFER_DELAY); // FOUP 웨이퍼 추가 대기
                    return true;

                case CommandType.StartProcess:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule module)
                    {
                        module.StartProcess();
                        mainView.UpdateProcessDisplay();
                    }
                    await Task.Delay(200); // 공정 시작 명령 처리 대기
                    return true;

                case CommandType.Delay:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is int milliseconds)
                    {
                        await Task.Delay(milliseconds);
                    }
                    return true;

                case CommandType.WaitForCompletion:
                    // 추후 구현: 특정 조건 대기
                    return true;

                case CommandType.UnloadWaferFromPM:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmUnload)
                    {
                        Wafer unloadedWafer = pmUnload.UnloadWafer("TM");  // PM에서 TM으로 이동
                        command.ResultWafer = unloadedWafer;
                        mainView.UpdateProcessDisplay();
                    }
                    await Task.Delay(WAFER_UNLOAD_DELAY); // 언로드 동작 완료 대기
                    return true;

                case CommandType.ResetPMProgress:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmReset)
                    {
                        pmReset.elapsedTime = 0;
                        mainView.ResetPMProgressBar(pmReset);
                    }
                    return true;

                case CommandType.WaitForProcessComplete:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmWait)
                    {
                        // PM 공정이 완료되고 언로드 요청이 있을 때까지 대기
                        while (pmWait.ModuleState == ProcessModule.State.Running ||
                               (pmWait.ModuleState == ProcessModule.State.Idle && !pmWait.IsUnloadRequested))
                        {
                            await Task.Delay(500); // 0.5초마다 확인
                        }
                    }
                    return true;

                case CommandType.LoadWaferToPM:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmLoad)
                    {
                        Wafer waferToLoad = previousCommand?.ResultWafer;
                        if (waferToLoad != null)
                        {
                            pmLoad.LoadWafer(waferToLoad, "TM");  // TM에서 PM으로 이동
                            mainView.UpdateProcessDisplay();
                        }
                    }
                    await Task.Delay(WAFER_PLACE_DELAY); // PM에 웨이퍼 로드 대기
                    return true;

                case CommandType.InitializeParameters:
                    // 공정 시작 전 파라미터 상승 시작 (레시피 적용)
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmInit)
                    {
                        pmInit.InitializeParametersFromRecipe();
                        mainView.UpdateProcessDisplay();
                    }
                    await Task.Delay(200); // 초기화 명령 처리 대기
                    return true;

                case CommandType.WaitForParameterStabilization:
                    // 파라미터가 목표값에 도달할 때까지 대기
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmStabilize)
                    {
                        int stabilizationTimeout = 60000; // 최대 60초 대기
                        int elapsed = 0;
                        int pollInterval = 500; // 0.5초마다 확인

                        while (!pmStabilize.Parameters.IsStable && elapsed < stabilizationTimeout)
                        {
                            // Stop 요청 확인
                            if (stopRequested)
                            {
                                return true; // 정상 종료로 처리하고 상위에서 중단 처리
                            }

                            await Task.Delay(pollInterval);
                            elapsed += pollInterval;
                        }

                        // 타임아웃이어도 계속 진행
                    }
                    return true;

                // === 실제 하드웨어 명령 처리 ===

                case CommandType.HomeUDAxis:
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            bool success = await controller.HomeUDAxis();
                            if (!success)
                            {
                                LogManager.Instance.Alarm("UD축 원점복귀 실패", "TM");
                                return false;
                            }
                            LogManager.Instance.AddLog("", "UD축 원점복귀 완료", "TM", LogCategory.Hardware, false);
                            await Task.Delay(AXIS_MOVE_SETTLE_DELAY); // 안정화 대기
                        }
                    }
                    return true;

                case CommandType.HomeLRAxis:
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            bool success = await controller.HomeLRAxis();
                            if (!success)
                            {
                                LogManager.Instance.Alarm("LR축 원점복귀 실패", "TM");
                                return false;
                            }
                            LogManager.Instance.AddLog("", "LR축 원점복귀 완료", "TM", LogCategory.Hardware, false);
                            await Task.Delay(AXIS_MOVE_SETTLE_DELAY); // 안정화 대기
                        }
                    }
                    return true;

                case CommandType.MoveUDAxis:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is long udPos)
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            string targetName = HardwarePositionMap.GetUDPositionName(udPos);
                            bool success = await controller.MoveUDAxis(udPos);
                            if (!success)
                            {
                                LogManager.Instance.Alarm($"UD축 이동 실패 (목표: {targetName})", "TM");
                                return false;
                            }
                            LogManager.Instance.AddLog("", $"UD축 이동 완료 → {targetName}", "TM", LogCategory.Hardware, false);
                            await Task.Delay(AXIS_MOVE_SETTLE_DELAY); // 안정화 대기
                        }
                    }
                    return true;

                case CommandType.MoveLRAxis:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is long lrPos)
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            string targetName = HardwarePositionMap.GetLRPositionName(lrPos);
                            bool success = await controller.MoveLRAxis(lrPos);
                            if (!success)
                            {
                                LogManager.Instance.Alarm($"LR축 이동 실패 (목표: {targetName})", "TM");
                                return false;
                            }
                            LogManager.Instance.AddLog("", $"LR축 이동 완료 → {targetName}", "TM", LogCategory.Hardware, false);
                            await Task.Delay(AXIS_MOVE_SETTLE_DELAY); // 안정화 대기
                        }
                    }
                    return true;

                case CommandType.OpenPMDoor:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmOpenDoor)
                    {
                        string pmName = GetPMName(pmOpenDoor.Type);
                        var controller = mainView.GetEtherCATController();
                        controller?.OpenPMDoor(pmOpenDoor.Type);
                        pmOpenDoor.IsDoorOpen = true;
                        mainView.UpdatePMDoorDisplay();
                        LogManager.Instance.AddLog("", $"{pmName} 문 열림", pmName, LogCategory.Hardware, false);
                    }
                    await Task.Delay(PM_DOOR_OPEN_DELAY); // 문 열림 완료 대기
                    return true;

                case CommandType.ClosePMDoor:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmCloseDoor)
                    {
                        string pmName = GetPMName(pmCloseDoor.Type);
                        var controller = mainView.GetEtherCATController();
                        controller?.ClosePMDoor(pmCloseDoor.Type);
                        pmCloseDoor.IsDoorOpen = false;
                        mainView.UpdatePMDoorDisplay();
                        LogManager.Instance.AddLog("", $"{pmName} 문 닫힘", pmName, LogCategory.Hardware, false);
                    }
                    await Task.Delay(PM_DOOR_CLOSE_DELAY); // 문 닫힘 완료 대기
                    return true;

                case CommandType.ForceClosePMDoor:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmForceCloseDoor)
                    {
                        string pmName = GetPMName(pmForceCloseDoor.Type);
                        var controller = mainView.GetEtherCATController();
                        if (controller is RealEtherCATController realController)
                        {
                            realController.ForceClosePMDoor(pmForceCloseDoor.Type);
                        }
                        pmForceCloseDoor.IsDoorOpen = false;
                        mainView.UpdatePMDoorDisplay();
                        LogManager.Instance.AddLog("초기화", $"{pmName} 문 강제 닫힘", pmName, LogCategory.Hardware, false);
                        // 시뮬레이션에서도 대기 시간 적용
                    }
                    await Task.Delay(PM_DOOR_CLOSE_DELAY); // 문 닫힘 완료 대기
                    return true;

                case CommandType.SetPMLampOn:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmLampOn)
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.SetPMLamp(pmLampOn.Type, true);
                    }
                    await Task.Delay(PM_LAMP_DELAY); // 램프 동작 대기
                    return true;

                case CommandType.SetPMLampOff:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmLampOff)
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.SetPMLamp(pmLampOff.Type, false);
                    }
                    await Task.Delay(PM_LAMP_DELAY); // 램프 동작 대기
                    return true;

                case CommandType.ExtendCylinder:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.ExtendCylinder();
                    }
                    await Task.Delay(CYLINDER_EXTEND_DELAY); // 실린더 전진 완료 대기
                    return true;

                case CommandType.RetractCylinder:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.RetractCylinder();
                    }
                    await Task.Delay(CYLINDER_RETRACT_DELAY); // 실린더 후진 완료 대기
                    return true;

                case CommandType.EnableSuction:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.EnableSuction();
                        LogManager.Instance.AddLog("", "웨이퍼 흡착 ON", "TM", LogCategory.Hardware, false);
                    }
                    await Task.Delay(SUCTION_ENABLE_DELAY); // 흡착 안정화 대기
                    return true;

                case CommandType.DisableSuction:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.DisableSuction();
                        LogManager.Instance.AddLog("", "웨이퍼 흡착 OFF", "TM", LogCategory.Hardware, false);
                    }
                    await Task.Delay(SUCTION_DISABLE_DELAY); // 흡착 해제 대기
                    return true;

                case CommandType.EnableExhaust:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.EnableExhaust();
                        LogManager.Instance.AddLog("", "웨이퍼 배기 ON", "TM", LogCategory.Hardware, false);
                    }
                    await Task.Delay(EXHAUST_ENABLE_DELAY); // 배기 동작 대기
                    return true;

                case CommandType.DisableExhaust:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.DisableExhaust();
                        LogManager.Instance.AddLog("", "웨이퍼 배기 OFF", "TM", LogCategory.Hardware, false);
                    }
                    await Task.Delay(EXHAUST_DISABLE_DELAY); // 배기 해제 대기
                    return true;

                case CommandType.ServoOn:
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            controller.SetServoUD(true);
                            controller.SetServoLR(true);
                            LogManager.Instance.AddLog("", "서보 ON (UD/LR)", "TM", LogCategory.Hardware, false);
                        }
                    }
                    await Task.Delay(SERVO_ON_DELAY); // 서보 ON 안정화 대기
                    return true;

                case CommandType.ServoOff:
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            controller.SetServoUD(false);
                            controller.SetServoLR(false);
                            LogManager.Instance.AddLog("", "서보 OFF (UD/LR)", "TM", LogCategory.Hardware, false);
                        }
                    }
                    await Task.Delay(SERVO_OFF_DELAY); // 서보 OFF 안정화 대기
                    return true;

                default:
                    return true;
            }
        }

        /// <summary>
        /// TM 회전 완료 대기 (폴링 기반)
        /// </summary>
        private async Task WaitForTMRotationComplete()
        {
            int timeout = 10000; // 10초 타임아웃
            int elapsed = 0;
            int pollInterval = 50; // 50ms마다 확인

            while (mainView.GetTMState() == TransferModule.TMState.Rotating && elapsed < timeout)
            {
                await Task.Delay(pollInterval);
                elapsed += pollInterval;
            }
        }

        /// <summary>
        /// TM 암 확장 완료 대기 (고정 대기시간)
        /// </summary>
        private async Task WaitForTMArmExtensionComplete()
        {
            await Task.Delay(TM_ARM_EXTEND_DELAY);
        }

        /// <summary>
        /// TM 암 수축 완료 대기 (고정 대기시간)
        /// </summary>
        private async Task WaitForTMArmRetractionComplete()
        {
            await Task.Delay(TM_ARM_RETRACT_DELAY);
        }

        /// <summary>
        /// TM 회전 각도에서 LR 위치 계산
        /// </summary>
        private long GetLRPositionFromAngle(float angle)
        {
            // 각도 허용 오차 (±5도)
            const float tolerance = 5f;

            if (Math.Abs(angle - ANGLE_FOUP_A) < tolerance || Math.Abs(angle + 50f) < tolerance)
                return HardwarePositionMap.LR_FOUP_A;

            if (Math.Abs(angle - ANGLE_PM1) < tolerance || Math.Abs(angle) < tolerance)
                return HardwarePositionMap.LR_PM1;

            if (Math.Abs(angle - ANGLE_PM2) < tolerance || Math.Abs(angle - 90f) < tolerance)
                return HardwarePositionMap.LR_PM2;

            if (Math.Abs(angle - ANGLE_PM3) < tolerance || Math.Abs(angle - 180f) < tolerance)
                return HardwarePositionMap.LR_PM3;

            if (Math.Abs(angle - ANGLE_FOUP_B) < tolerance || Math.Abs(angle - 230f) < tolerance)
                return HardwarePositionMap.LR_FOUP_B;

            // 매칭되는 위치가 없으면 PM1 위치 반환 (기본값)
            return HardwarePositionMap.LR_PM1;
        }

        /// <summary>
        /// PM 타입에서 PM 이름 문자열 반환
        /// </summary>
        private string GetPMName(ProcessModule.ModuleType type)
        {
            switch (type)
            {
                case ProcessModule.ModuleType.PM1:
                    return "PM1";
                case ProcessModule.ModuleType.PM2:
                    return "PM2";
                case ProcessModule.ModuleType.PM3:
                    return "PM3";
                default:
                    return "PM";
            }
        }
    }
}

