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
        /// 큐 초기화
        /// </summary>
        public void Clear()
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

            while (commands.Count > 0)
            {
                var command = commands.Dequeue();
                await ExecuteCommandAsync(command);
                command.IsCompleted = true;
            }

            isExecuting = false;
        }

        /// <summary>
        /// 단일 명령 실행
        /// </summary>
        private async Task ExecuteCommandAsync(ProcessCommand command)
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
                    mainView.TMPickWafer();
                    await Task.Delay(100); // 픽업 동작 시간
                    break;

                case CommandType.PlaceWafer:
                    mainView.TMPlaceWafer();
                    await Task.Delay(100); // 배치 동작 시간
                    break;

                case CommandType.RemoveWaferFromFoup:
                    if (command.Parameters.Length > 1 && 
                        command.Parameters[0] is Foup foup && 
                        command.Parameters[1] is int slotIndex)
                    {
                        foup.UnloadWafer(slotIndex);
                        mainView.UpdateFoupDisplays();
                    }
                    break;

                case CommandType.AddWaferToFoup:
                    if (command.Parameters.Length > 1 && 
                        command.Parameters[0] is Foup foupAdd && 
                        command.Parameters[1] is int slotIndexAdd)
                    {
                        foupAdd.LoadWafer(slotIndexAdd);
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

