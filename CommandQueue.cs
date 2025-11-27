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

        public bool IsExecuting => isExecuting;
        public int CommandCount => commands.Count;

        #region 동작 대기 시간 상수 (ms)

        // 서보 제어
        private const int SERVO_ON_DELAY = 1000;        // 서보 ON 안정화
        private const int SERVO_OFF_DELAY = 500;        // 서보 OFF 안정화

        // 축 이동 (원점복귀는 컨트롤러에서 완료 대기)
        private const int AXIS_MOVE_SETTLE_DELAY = 1000; // 축 이동 후 안정화

        // PM 문 제어
        private const int PM_DOOR_OPEN_DELAY = 2500;    // 문 열림 대기
        private const int PM_DOOR_CLOSE_DELAY = 2500;   // 문 닫힘 대기
        private const int PM_LAMP_DELAY = 100;          // 램프 ON/OFF 대기

        // TM 실린더/흡착 제어
        private const int CYLINDER_EXTEND_DELAY = 3000; // 실린더 전진 대기
        private const int CYLINDER_RETRACT_DELAY = 3000;// 실린더 후진 대기
        private const int SUCTION_ENABLE_DELAY = 1500;  // 흡착 ON 안정화
        private const int SUCTION_DISABLE_DELAY = 1000;  // 흡착 OFF 대기
        private const int EXHAUST_ENABLE_DELAY = 1000;   // 배기 ON 대기
        private const int EXHAUST_DISABLE_DELAY = 1500;  // 배기 OFF 대기

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
        /// 실행 중단 플래그 설정
        /// </summary>
        public void Stop()
        {
            commands.Clear();
            isExecuting = false;
        }

        /// <summary>
        /// 명령 실행 시작
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (isExecuting)
                return;

            isExecuting = true;
            ProcessCommand previousCommand = null;

            while (commands.Count > 0 && isExecuting)
            {
                var command = commands.Dequeue();
                bool success = await ExecuteCommandAsync(command, previousCommand);

                if (!success)
                {
                    // 명령 실패 시 워크플로우 중단
                    System.Diagnostics.Debug.WriteLine($"명령 실패로 워크플로우 중단: {command.Description}");
                    commands.Clear();
                    isExecuting = false;
                    mainView.ShowErrorMessage($"명령 실패: {command.Description}");
                    return;
                }

                command.IsCompleted = true;
                previousCommand = command;
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
                        mainView.RotateTM(angle);
                        // TM 회전 완료 대기
                        await WaitForTMRotationComplete();
                        await Task.Delay(TM_SETTLE_DELAY); // 안정화 대기
                    }
                    return true;

                case CommandType.ExtendArm:
                    mainView.ExtendTMArm();
                    await WaitForTMArmExtensionComplete();
                    await Task.Delay(TM_SETTLE_DELAY); // 안정화 대기
                    return true;

                case CommandType.RetractArm:
                    mainView.RetractTMArm();
                    await WaitForTMArmRetractionComplete();
                    await Task.Delay(TM_SETTLE_DELAY); // 안정화 대기
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
                        pm.LoadWafer(placedWafer);
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
                        Wafer unloadedWafer = pmUnload.UnloadWafer();
                        command.ResultWafer = unloadedWafer;
                        mainView.UpdateProcessDisplay();
                    }
                    await Task.Delay(WAFER_UNLOAD_DELAY); // 언로드 동작 완료 대기
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
                            pmLoad.LoadWafer(waferToLoad);
                            mainView.UpdateProcessDisplay();
                        }
                    }
                    await Task.Delay(WAFER_PLACE_DELAY); // PM에 웨이퍼 로드 대기
                    return true;

                // === 실제 하드웨어 명령 처리 ===

                case CommandType.HomeUDAxis:
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            bool success = await controller.HomeUDAxis();
                            if (!success) return false;
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
                            if (!success) return false;
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
                            bool success = await controller.MoveUDAxis(udPos);
                            if (!success) return false;
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
                            bool success = await controller.MoveLRAxis(lrPos);
                            if (!success) return false;
                            await Task.Delay(AXIS_MOVE_SETTLE_DELAY); // 안정화 대기
                        }
                    }
                    return true;

                case CommandType.OpenPMDoor:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmOpenDoor)
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.OpenPMDoor(pmOpenDoor.Type);
                    }
                    await Task.Delay(PM_DOOR_OPEN_DELAY); // 문 열림 완료 대기
                    return true;

                case CommandType.ClosePMDoor:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmCloseDoor)
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.ClosePMDoor(pmCloseDoor.Type);
                    }
                    await Task.Delay(PM_DOOR_CLOSE_DELAY); // 문 닫힘 완료 대기
                    return true;

                case CommandType.ForceClosePMDoor:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmForceCloseDoor)
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller is RealEtherCATController realController)
                        {
                            realController.ForceClosePMDoor(pmForceCloseDoor.Type);
                        }
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
                    }
                    await Task.Delay(SUCTION_ENABLE_DELAY); // 흡착 안정화 대기
                    return true;

                case CommandType.DisableSuction:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.DisableSuction();
                    }
                    await Task.Delay(SUCTION_DISABLE_DELAY); // 흡착 해제 대기
                    return true;

                case CommandType.EnableExhaust:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.EnableExhaust();
                    }
                    await Task.Delay(EXHAUST_ENABLE_DELAY); // 배기 동작 대기
                    return true;

                case CommandType.DisableExhaust:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.DisableExhaust();
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
    }
}

