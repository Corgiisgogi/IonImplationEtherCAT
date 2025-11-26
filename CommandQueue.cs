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

            while (commands.Count > 0)
            {
                var command = commands.Dequeue();
                await ExecuteCommandAsync(command, previousCommand);
                command.IsCompleted = true;
                previousCommand = command;
            }

            isExecuting = false;
        }

        /// <summary>
        /// 단일 명령 실행
        /// </summary>
        private async Task ExecuteCommandAsync(ProcessCommand command, ProcessCommand previousCommand)
        {
            switch (command.Type)
            {
                case CommandType.RotateTM:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is float angle)
                    {
                        mainView.RotateTM(angle);
                        // TM 회전 완료 대기
                        await WaitForTMRotationComplete();
                    }
                    break;

                case CommandType.ExtendArm:
                    mainView.ExtendTMArm();
                    await WaitForTMArmExtensionComplete();
                    break;

                case CommandType.RetractArm:
                    mainView.RetractTMArm();
                    await WaitForTMArmRetractionComplete();
                    break;

                case CommandType.PickWafer:
                    // 이전 명령의 ResultWafer를 사용 (RemoveWaferFromFoup에서 설정됨)
                    Wafer waferToPick = previousCommand?.ResultWafer;
                    if (waferToPick != null)
                    {
                        mainView.TMPickWafer(waferToPick);
                        await Task.Delay(100); // 픽업 동작 시간
                    }
                    break;

                case CommandType.PlaceWafer:
                    Wafer placedWafer = mainView.TMPlaceWafer();
                    // 배치된 웨이퍼를 파라미터로 저장 (다음 명령에서 사용 가능)
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pm)
                    {
                        pm.LoadWafer(placedWafer);
                    }
                    await Task.Delay(100); // 배치 동작 시간
                    break;

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
                    break;

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
                    break;

                case CommandType.StartProcess:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule module)
                    {
                        module.StartProcess();
                        mainView.UpdateProcessDisplay();
                    }
                    break;

                case CommandType.Delay:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is int milliseconds)
                    {
                        await Task.Delay(milliseconds);
                    }
                    break;

                case CommandType.WaitForCompletion:
                    // 추후 구현: 특정 조건 대기
                    break;

                case CommandType.UnloadWaferFromPM:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmUnload)
                    {
                        Wafer unloadedWafer = pmUnload.UnloadWafer();
                        command.ResultWafer = unloadedWafer;
                        mainView.UpdateProcessDisplay();
                        await Task.Delay(100);
                    }
                    break;

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
                    break;

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
                    break;

                // === 실제 하드웨어 명령 처리 ===

                case CommandType.HomeUDAxis:
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            await controller.HomeUDAxis();
                        }
                    }
                    break;

                case CommandType.HomeLRAxis:
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            await controller.HomeLRAxis();
                        }
                    }
                    break;

                case CommandType.MoveUDAxis:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is long udPos)
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            await controller.MoveUDAxis(udPos);
                        }
                    }
                    break;

                case CommandType.MoveLRAxis:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is long lrPos)
                    {
                        var controller = mainView.GetEtherCATController();
                        if (controller != null)
                        {
                            await controller.MoveLRAxis(lrPos);
                        }
                    }
                    break;

                case CommandType.OpenPMDoor:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmOpenDoor)
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.OpenPMDoor(pmOpenDoor.Type);
                        await Task.Delay(1500); // 문 열림 대기
                    }
                    break;

                case CommandType.ClosePMDoor:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmCloseDoor)
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.ClosePMDoor(pmCloseDoor.Type);
                        await Task.Delay(1500); // 문 닫힘 대기
                    }
                    break;

                case CommandType.SetPMLampOn:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmLampOn)
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.SetPMLamp(pmLampOn.Type, true);
                    }
                    break;

                case CommandType.SetPMLampOff:
                    if (command.Parameters.Length > 0 && command.Parameters[0] is ProcessModule pmLampOff)
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.SetPMLamp(pmLampOff.Type, false);
                    }
                    break;

                case CommandType.ExtendCylinder:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.ExtendCylinder();
                        await Task.Delay(500); // 실린더 전진 대기
                    }
                    break;

                case CommandType.RetractCylinder:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.RetractCylinder();
                        await Task.Delay(500); // 실린더 후진 대기
                    }
                    break;

                case CommandType.EnableSuction:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.EnableSuction();
                        await Task.Delay(500); // 흡착 안정화 대기
                    }
                    break;

                case CommandType.DisableSuction:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.DisableSuction();
                    }
                    break;

                case CommandType.EnableExhaust:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.EnableExhaust();
                        await Task.Delay(300); // 배기 대기
                    }
                    break;

                case CommandType.DisableExhaust:
                    {
                        var controller = mainView.GetEtherCATController();
                        controller?.DisableExhaust();
                    }
                    break;
            }
        }

        /// <summary>
        /// TM 회전 완료 대기
        /// </summary>
        private async Task WaitForTMRotationComplete()
        {
            while (mainView.GetTMState() == TransferModule.TMState.Rotating)
            {
                await Task.Delay(50);
            }
        }

        /// <summary>
        /// TM 암 확장 완료 대기
        /// </summary>
        private async Task WaitForTMArmExtensionComplete()
        {
            while (mainView.GetTMState() == TransferModule.TMState.Moving)
            {
                await Task.Delay(50);
            }
        }

        /// <summary>
        /// TM 암 수축 완료 대기
        /// </summary>
        private async Task WaitForTMArmRetractionComplete()
        {
            while (mainView.GetTMState() == TransferModule.TMState.Moving)
            {
                await Task.Delay(50);
            }
        }
    }
}

