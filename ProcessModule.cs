using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    public class ProcessModule
    {
        // 상태 정의
        public enum State
        {
            Idle,
            Running,
            Paused,
            Stoped,
            Error
        };

        public enum RequestType
        {
            LoadWafer,
            UnloadWafer,
            ErrorReport
        };

        // PM 모듈 타입 정의
        public enum ModuleType
        {
            PM1,    // 이온 주입
            PM2,    // 미사용 (향후 확장)
            PM3     // 어닐링
        };

        // EtherCAT 컨트롤러 참조
        private IEtherCATController etherCATController;

        public State ModuleState { get; set; }
        public int processTime { get; set; }
        public int elapsedTime { get; set; }
        public bool isWaferLoaded { get; set; }

        // PM 모듈 타입
        public ModuleType Type { get; set; }

        // 문 상태 (UI 표시용)
        public bool IsDoorOpen { get; private set; }

        // 공정 완료 시 TM의 웨이퍼 언로드 요청 플래그
        public bool IsUnloadRequested { get; set; }

        // 현재 처리 중인 웨이퍼
        public Wafer CurrentWafer { get; set; }

        public ProcessModule()
        {
            ModuleState = State.Idle;
            processTime = 0;
            elapsedTime = 0;
            isWaferLoaded = false;
            Type = ModuleType.PM1;
            IsUnloadRequested = false;
            CurrentWafer = null;
            IsDoorOpen = false;
        }

        public ProcessModule(int defaultProcessTime)
        {
            ModuleState = State.Idle;
            processTime = defaultProcessTime;
            elapsedTime = 0;
            isWaferLoaded = false;
            Type = ModuleType.PM1;
            IsUnloadRequested = false;
            CurrentWafer = null;
            IsDoorOpen = false;
        }

        public ProcessModule(ModuleType moduleType, int defaultProcessTime)
        {
            ModuleState = State.Idle;
            processTime = defaultProcessTime;
            elapsedTime = 0;
            isWaferLoaded = false;
            Type = moduleType;
            IsUnloadRequested = false;
            CurrentWafer = null;
            IsDoorOpen = false;
        }

        public void StartProcess(int time)
        {
            processTime = time;
            elapsedTime = 0;
            ModuleState = State.Running;
        }

        /// <summary>
        /// 설정된 공정 시간으로 공정 시작
        /// </summary>
        public void StartProcess()
        {
            elapsedTime = 0;
            ModuleState = State.Running;
        }

        public void PauseProcess()
        {
            if (ModuleState == State.Running)
            {
                ModuleState = State.Paused;
            }
        }

        public void ResumeProcess()
        {
            if (ModuleState == State.Paused)
            {
                ModuleState = State.Running;
            }
        }

        public void StopProcess()
        {
            ModuleState = State.Stoped;
            elapsedTime = 0;
            IsUnloadRequested = false;
        }

        public void UpdateProcess(int timeIncrement)
        {
            if (ModuleState == State.Running)
            {
                elapsedTime += timeIncrement;
                if (elapsedTime >= processTime)
                {
                    ModuleState = State.Idle; // 프로세스 완료 후 대기 상태로 전환
                    elapsedTime = processTime; // 경과 시간을 최대값으로 설정
                    IsUnloadRequested = true; // 웨이퍼 언로드 요청 플래그 설정

                    // 웨이퍼 상태 업데이트
                    if (CurrentWafer != null)
                    {
                        if (Type == ModuleType.PM1)
                        {
                            CurrentWafer.UpdateState(Wafer.WaferState.IonProcessComplete);
                        }
                        else if (Type == ModuleType.PM3)
                        {
                            CurrentWafer.UpdateState(Wafer.WaferState.AnnealingProcessComplete);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PM에 웨이퍼 로드
        /// </summary>
        public void LoadWafer(Wafer wafer)
        {
            CurrentWafer = wafer;
            isWaferLoaded = true;
        }

        /// <summary>
        /// PM에서 웨이퍼 언로드
        /// </summary>
        public Wafer UnloadWafer()
        {
            Wafer wafer = CurrentWafer;
            CurrentWafer = null;
            isWaferLoaded = false;
            IsUnloadRequested = false; // 언로드 요청 플래그 리셋
            return wafer;
        }

        public void SendRequest(List<RequestType> requestlist, RequestType request)
        {
            if (request == RequestType.ErrorReport)
            {
                requestlist.Insert(0, request);
            }
            requestlist.Add(request);
        }

        #region EtherCAT 컨트롤러 관련

        /// <summary>
        /// EtherCAT 컨트롤러 설정
        /// </summary>
        public void SetEtherCATController(IEtherCATController controller)
        {
            this.etherCATController = controller;
        }

        /// <summary>
        /// PM 문 열기 (실제 하드웨어)
        /// </summary>
        public async Task OpenDoorAsync()
        {
            etherCATController?.OpenPMDoor(this.Type);
            IsDoorOpen = true;
            await Task.Delay(1500); // 문 열림 대기
        }

        /// <summary>
        /// PM 문 닫기 (실제 하드웨어)
        /// </summary>
        public async Task CloseDoorAsync()
        {
            etherCATController?.ClosePMDoor(this.Type);
            IsDoorOpen = false;
            await Task.Delay(1500); // 문 닫힘 대기
        }

        /// <summary>
        /// PM 램프 제어 (실제 하드웨어)
        /// </summary>
        public void SetLamp(bool on)
        {
            etherCATController?.SetPMLamp(this.Type, on);
        }

        #endregion
    }
}
