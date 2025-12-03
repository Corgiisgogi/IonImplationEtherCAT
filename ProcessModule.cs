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
            Stopping,  // 정지 중 (하드웨어 초기화 완료 전)
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
            PM1,    // 이온 주입1
            PM2,    // 이온 주입2
            PM3     // 어닐링
        };

        // EtherCAT 컨트롤러 참조
        private IEtherCATController etherCATController;

        // 공정 시간 백킹 필드
        private int _processTime;

        public State ModuleState { get; set; }

        /// <summary>
        /// 공정 시간 (35초 ~ 120초)
        /// </summary>
        public int processTime
        {
            get => _processTime;
            set
            {
                if (value < 35 || value > 120)
                {
                    throw new ArgumentOutOfRangeException(nameof(processTime), $"공정 시간은 35초에서 120초 사이여야 합니다. (입력값: {value}초)");
                }
                _processTime = value;
            }
        }

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
            processTime = defaultProcessTime; // setter에서 자동 검증 (30~120초)
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
            processTime = defaultProcessTime; // setter에서 자동 검증 (30~120초)
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
            processTime = time; // setter에서 자동 검증 (30~120초)
            elapsedTime = 0;
            ModuleState = State.Running;
            CompletedTime = null; // 완료 시간 초기화
            InitializeParametersFromRecipe();

            // 공정 시작 로그
            LogManager.Instance.AddLog($"{Type} 공정", $"공정 시작 (시간: {time}초)", Type.ToString(), LogCategory.Process, false);
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

            // 공정 시작 로그
            LogManager.Instance.AddLog($"{Type} 공정", $"공정 시작 (시간: {processTime}초)", Type.ToString(), LogCategory.Process, false);
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
            if (ModuleState == State.Idle)
                return;  // Idle일 때는 변화 없음

            ModuleState = State.Stopping;  // Stoped 대신 Stopping으로
            elapsedTime = 0;
            IsUnloadRequested = false;
            CompletedTime = null; // 완료 시간 초기화
            Parameters.StartFalling(); // 파라미터 하강 시작
        }

        /// <summary>
        /// 하드웨어 초기화 완료 후 Stopping → Idle 전환
        /// </summary>
        public void CompleteStop()
        {
            if (ModuleState == State.Stopping)
            {
                ModuleState = State.Idle;
            }
        }

        // 이전에 기록한 공정 단계 (중복 로그 방지)
        private int lastLoggedStep = -1;

        public void UpdateProcess(int timeIncrement)
        {
            if (ModuleState == State.Running)
            {
                // 안정화 여부와 상관없이 즉시 시간 카운트 시작
                elapsedTime += timeIncrement;

                // 공정 절차에 맞춘 파라미터 업데이트
                bool isAnnealing = (Type == ModuleType.PM3);
                Parameters.UpdatePhased(elapsedTime, processTime, isAnnealing, timeIncrement);

                // 단계별 로그 출력
                LogProcessStep(elapsedTime, processTime);

                if (elapsedTime >= processTime)
                {
                    ModuleState = State.Idle; // 프로세스 완료 후 대기 상태로 전환
                    elapsedTime = processTime; // 경과 시간을 최대값으로 설정
                    IsUnloadRequested = true; // 웨이퍼 언로드 요청 플래그 설정
                    CompletedTime = DateTime.Now; // 완료 시간 기록 (FIFO 판단용)
                    lastLoggedStep = -1; // 다음 공정을 위해 리셋

                    // 파라미터 하강 시작 (공정 완료 후 상온/대기압으로 복귀)
                    Parameters.StartFalling();

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

                    // 공정 완료 로그
                    LogManager.Instance.AddLog($"{Type} 공정", $"공정 완료 (소요 시간: {processTime}초)", Type.ToString(), LogCategory.Process, false);
                }
            }
        }

        /// <summary>
        /// 공정 단계별 로그 출력 (중복 방지)
        /// </summary>
        private void LogProcessStep(int elapsed, int total)
        {
            int step = GetCurrentStep(elapsed, total);
            if (step == lastLoggedStep) return; // 이미 기록된 단계면 스킵

            lastLoggedStep = step;
            string message = GetStepMessage(step, elapsed, total);
            if (!string.IsNullOrEmpty(message))
            {
                LogManager.Instance.AddLog($"{Type} 공정", message, Type.ToString(), LogCategory.Process, false);
            }
        }

        /// <summary>
        /// 현재 공정 단계 번호 반환
        /// </summary>
        private int GetCurrentStep(int elapsed, int total)
        {
            int timeToEnd = total - elapsed;
            bool isAnnealing = (Type == ModuleType.PM3);

            if (isAnnealing)
            {
                // 어닐링 공정 단계
                if (elapsed <= 1) return 1;      // 진공 시작
                if (elapsed <= 3) return 2;      // 진공 도달
                if (elapsed <= 4) return 3;      // RTA 램프 ON
                if (elapsed <= 5) return 4;      // 급속 가열 완료
                if (timeToEnd > 5) return 5;     // 어닐링 진행 중
                if (timeToEnd > 2) return 6;     // 냉각 시작
                return 7;                        // 챔버 벤트
            }
            else
            {
                // 이온 주입 공정 단계
                if (elapsed <= 1) return 1;      // 진공 시작
                if (elapsed <= 3) return 2;      // 진공 도달
                if (elapsed <= 4) return 3;      // 이온 소스 ON
                if (elapsed <= 5) return 4;      // 질량 분석기 ON
                if (elapsed <= 6) return 5;      // 가속 전압 인가
                if (elapsed <= 8) return 6;      // HV 안정화
                if (elapsed <= 10) return 7;     // 빔 게이트 OPEN, 스캐너 시작
                if (timeToEnd > 3) return 8;     // 이온 주입 중
                if (timeToEnd > 1) return 9;     // 빔 게이트 CLOSE
                return 10;                       // 가속 전압 OFF, 벤트
            }
        }

        /// <summary>
        /// 단계별 로그 메시지 반환
        /// </summary>
        private string GetStepMessage(int step, int elapsed, int total)
        {
            bool isAnnealing = (Type == ModuleType.PM3);

            if (isAnnealing)
            {
                switch (step)
                {
                    case 1: return "챔버 진공 시작";
                    case 2: return "진공 도달";
                    case 3: return "RTA 램프 ON";
                    case 4: return "목표 온도 도달";
                    case 5: return "어닐링 진행 중...";
                    case 6: return "냉각 시작";
                    case 7: return "챔버 벤트 (대기압 복구)";
                    default: return null;
                }
            }
            else
            {
                switch (step)
                {
                    case 1: return "챔버 진공 시작";
                    case 2: return "진공 도달";
                    case 3: return "이온 소스 ON";
                    case 4: return "질량 분석기 ON";
                    case 5: return "가속 전압 인가";
                    case 6: return "HV 안정화 완료";
                    case 7: return "빔 게이트 OPEN, 스캐너 작동 시작";
                    case 8: return "이온 주입 중... (도즈 적산)";
                    case 9: return "목표 도즈 도달, 빔 게이트 CLOSE";
                    case 10: return "가속 전압 OFF, 챔버 벤트";
                    default: return null;
                }
            }
        }

        /// <summary>
        /// Idle/Stopped/Stopping 상태에서 파라미터 애니메이션 업데이트 (상승/하강 모두)
        /// </summary>
        public void UpdateParametersWhenIdle()
        {
            if ((ModuleState == State.Idle || ModuleState == State.Stoped || ModuleState == State.Stopping) &&
                (Parameters.IsFalling || Parameters.IsRising))
            {
                Parameters.Update(1.0); // 1초 단위로 업데이트
            }
        }

        /// <summary>
        /// PM에 웨이퍼 로드
        /// </summary>
        /// <param name="wafer">로드할 웨이퍼</param>
        /// <param name="fromLocation">출발 위치 (예: "FOUP A", "TM")</param>
        public void LoadWafer(Wafer wafer, string fromLocation = "")
        {
            CurrentWafer = wafer;
            isWaferLoaded = true;

            // 웨이퍼 로드 로그 (출발지 → 목적지 형식)
            string desc = string.IsNullOrEmpty(fromLocation)
                ? $"{Type} 웨이퍼 로드 완료"
                : $"{fromLocation} → {Type} 웨이퍼 이동 완료";
            LogManager.Instance.AddLog("웨이퍼 이동", desc, Type.ToString(), LogCategory.Transfer, false);
        }

        /// <summary>
        /// PM에서 웨이퍼 언로드
        /// </summary>
        /// <param name="toLocation">목적지 위치 (예: "PM3", "FOUP B")</param>
        public Wafer UnloadWafer(string toLocation = "")
        {
            Wafer wafer = CurrentWafer;
            CurrentWafer = null;
            isWaferLoaded = false;
            IsUnloadRequested = false; // 언로드 요청 플래그 리셋

            // 웨이퍼 언로드 로그 (출발지 → 목적지 형식)
            string desc = string.IsNullOrEmpty(toLocation)
                ? $"{Type} 웨이퍼 언로드 완료"
                : $"{Type} → {toLocation} 웨이퍼 이동 완료";
            LogManager.Instance.AddLog("웨이퍼 이동", desc, Type.ToString(), LogCategory.Transfer, false);

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
