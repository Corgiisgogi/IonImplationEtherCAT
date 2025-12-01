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
        public bool IsDoorOpen { get; set; }

        // 공정 완료 시 TM의 웨이퍼 언로드 요청 플래그
        public bool IsUnloadRequested { get; set; }

        // 공정 완료 시간 (FIFO 판단용)
        public DateTime? CompletedTime { get; set; }

        // 현재 처리 중인 웨이퍼
        public Wafer CurrentWafer { get; set; }

        // 공정 파라미터 (온도, 압력, AV, Dose 시뮬레이션)
        public ProcessParameters Parameters { get; private set; }

        // 레시피 참조
        public IonImplantRecipe IonRecipe { get; set; }      // PM1/PM2용
        public AnnealingRecipe AnnealRecipe { get; set; }    // PM3용

        public ProcessModule()
        {
            ModuleState = State.Idle;
            processTime = 0;
            elapsedTime = 0;
            isWaferLoaded = false;
            Type = ModuleType.PM1;
            IsUnloadRequested = false;
            CompletedTime = null;
            CurrentWafer = null;
            IsDoorOpen = false;
            Parameters = new ProcessParameters();
        }

        public ProcessModule(int defaultProcessTime)
        {
            ModuleState = State.Idle;
            processTime = defaultProcessTime;
            elapsedTime = 0;
            isWaferLoaded = false;
            Type = ModuleType.PM1;
            IsUnloadRequested = false;
            CompletedTime = null;
            CurrentWafer = null;
            IsDoorOpen = false;
            Parameters = new ProcessParameters();
        }

        public ProcessModule(ModuleType moduleType, int defaultProcessTime)
        {
            ModuleState = State.Idle;
            processTime = defaultProcessTime;
            elapsedTime = 0;
            isWaferLoaded = false;
            Type = moduleType;
            IsUnloadRequested = false;
            CompletedTime = null;
            CurrentWafer = null;
            IsDoorOpen = false;
            Parameters = new ProcessParameters();
        }

        public void StartProcess(int time)
        {
            processTime = time;
            elapsedTime = 0;
            ModuleState = State.Running;
            CompletedTime = null; // 완료 시간 초기화
            InitializeParametersFromRecipe();
        }

        /// <summary>
        /// 설정된 공정 시간으로 공정 시작
        /// </summary>
        public void StartProcess()
        {
            elapsedTime = 0;
            ModuleState = State.Running;
            CompletedTime = null; // 완료 시간 초기화
            InitializeParametersFromRecipe();
        }

        /// <summary>
        /// 레시피에서 파라미터 목표값 초기화 및 상승 시작
        /// </summary>
        public void InitializeParametersFromRecipe()
        {
            if (Type == ModuleType.PM1 || Type == ModuleType.PM2)
            {
                // 이온 주입 레시피에서 목표값 로드
                Parameters.TargetTemperature = IonRecipe?.TargetTemperature ?? 800.0;
                Parameters.TargetPressure = IonRecipe?.TargetPressure ?? 1E-5;
                // HV(가속 전압) = 레시피의 Voltage 값 사용
                Parameters.TargetHV = IonRecipe?.Voltage ?? IonRecipe?.TargetHV ?? 100.0;
                // Dose = 레시피에서 설정한 Dose 값 그대로 사용 (ions/cm²)
                Parameters.TargetDose = IonRecipe?.Dose ?? 0;

                // 디버그 로그
                System.Diagnostics.Debug.WriteLine($"[{Type}] InitializeParametersFromRecipe:");
                System.Diagnostics.Debug.WriteLine($"  - IonRecipe null? {IonRecipe == null}");
                if (IonRecipe != null)
                {
                    System.Diagnostics.Debug.WriteLine($"  - Recipe.Dose: {IonRecipe.Dose}");
                    System.Diagnostics.Debug.WriteLine($"  - Recipe.Voltage: {IonRecipe.Voltage}");
                }
                System.Diagnostics.Debug.WriteLine($"  - TargetDose: {Parameters.TargetDose}");
                System.Diagnostics.Debug.WriteLine($"  - TargetHV: {Parameters.TargetHV}");
            }
            else // PM3
            {
                // 어닐링 레시피에서 목표값 로드 - Temperature, Vacuum 값 직접 사용
                Parameters.TargetTemperature = AnnealRecipe?.Temperature > 0
                    ? AnnealRecipe.Temperature
                    : AnnealRecipe?.TargetTemperature ?? 950.0;
                Parameters.TargetPressure = AnnealRecipe?.Vacuum > 0
                    ? AnnealRecipe.Vacuum
                    : AnnealRecipe?.TargetPressure ?? 1E-6;
                Parameters.TargetHV = 0;           // 어닐링은 HV 없음
                Parameters.TargetDose = 0;         // 어닐링은 Dose 없음

                // 디버그 로그
                System.Diagnostics.Debug.WriteLine($"[{Type}] InitializeParametersFromRecipe:");
                System.Diagnostics.Debug.WriteLine($"  - AnnealRecipe null? {AnnealRecipe == null}");
                if (AnnealRecipe != null)
                {
                    System.Diagnostics.Debug.WriteLine($"  - Recipe.Temperature: {AnnealRecipe.Temperature}");
                    System.Diagnostics.Debug.WriteLine($"  - Recipe.Vacuum: {AnnealRecipe.Vacuum}");
                }
                System.Diagnostics.Debug.WriteLine($"  - TargetTemperature: {Parameters.TargetTemperature}");
                System.Diagnostics.Debug.WriteLine($"  - TargetPressure: {Parameters.TargetPressure}");
            }
            Parameters.StartRising();
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
            CompletedTime = null; // 완료 시간 초기화
            Parameters.StartFalling(); // 파라미터 하강 시작
        }

        public void UpdateProcess(int timeIncrement)
        {
            if (ModuleState == State.Running)
            {
                // 파라미터 업데이트 (상승/안정 애니메이션) - 항상 실행
                Parameters.Update(timeIncrement);

                // 파라미터가 안정된 후에만 공정 시간 카운트
                if (Parameters.IsStable)
                {
                    elapsedTime += timeIncrement;

                    if (elapsedTime >= processTime)
                    {
                        ModuleState = State.Idle; // 프로세스 완료 후 대기 상태로 전환
                        elapsedTime = processTime; // 경과 시간을 최대값으로 설정
                        IsUnloadRequested = true; // 웨이퍼 언로드 요청 플래그 설정
                        CompletedTime = DateTime.Now; // 완료 시간 기록 (FIFO 판단용)
                        Parameters.StartFalling(); // 파라미터 하강 시작

                        // 웨이퍼 상태 업데이트
                        if (CurrentWafer != null)
                        {
                            if (Type == ModuleType.PM1 || Type == ModuleType.PM2)
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
        }

        /// <summary>
        /// Idle/Stopped 상태에서 파라미터 애니메이션 업데이트 (상승/하강 모두)
        /// </summary>
        public void UpdateParametersWhenIdle()
        {
            if ((ModuleState == State.Idle || ModuleState == State.Stoped) &&
                (Parameters.IsFalling || Parameters.IsRising))
            {
                Parameters.Update(1.0); // 1초 단위로 업데이트
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
