using System;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 실시간 공정 파라미터 (시뮬레이션 표시용)
    /// 값의 점진적 상승/하강 애니메이션 처리
    /// </summary>
    public class ProcessParameters
    {
        #region 현재값 (UI 표시용)

        /// <summary>현재 온도 (°C)</summary>
        public double CurrentTemperature { get; private set; }

        /// <summary>현재 압력 (Torr)</summary>
        public double CurrentPressure { get; private set; }

        /// <summary>현재 가속 전압 HV (kV)</summary>
        public double CurrentHV { get; private set; }

        /// <summary>누적 도즈 (ions/cm²)</summary>
        public double CurrentDose { get; private set; }

        #endregion

        #region 목표값 (레시피에서 설정)

        /// <summary>목표 온도 (°C)</summary>
        public double TargetTemperature { get; set; }

        /// <summary>목표 압력 (Torr)</summary>
        public double TargetPressure { get; set; }

        /// <summary>목표 가속 전압 HV (kV)</summary>
        public double TargetHV { get; set; }

        /// <summary>목표 도즈 (ions/cm²) - 레시피에서 설정한 값 직접 사용</summary>
        public double TargetDose { get; set; }

        #endregion

        #region 애니메이션 속도 상수 (초당)

        // 온도 상승/하강 속도 (°C/s)
        private const double TEMP_RISE_RATE = 50.0;
        private const double TEMP_FALL_RATE = 30.0;

        // 가속 전압 HV 상승/하강 속도 (kV/s)
        private const double HV_RISE_RATE = 20.0;
        private const double HV_FALL_RATE = 15.0;

        // 압력 변화 계수 (지수적 변화)
        private const double PRESSURE_RISE_FACTOR = 0.7;    // 상승 시 매초 x0.7
        private const double PRESSURE_FALL_FACTOR = 1.5;    // 하강 시 매초 x1.5

        // 도즈 상승 시간 (초) - 이 시간에 걸쳐 목표값까지 상승
        private const double DOSE_RISE_TIME = 5.0;          // 5초에 걸쳐 도달

        #endregion

        #region 기준값 (정지 시)

        private const double BASELINE_TEMP = 25.0;          // 상온 (°C)
        private const double BASELINE_PRESSURE = 760.0;     // 대기압 (Torr)
        private const double BASELINE_HV = 0.0;             // 0 kV
        private const double BASELINE_DOSE = 0.0;           // 0 ions/cm²

        #endregion

        #region 애니메이션 상태

        /// <summary>값이 상승 중인지</summary>
        public bool IsRising { get; private set; }

        /// <summary>값이 하강 중인지</summary>
        public bool IsFalling { get; private set; }

        /// <summary>목표값에 도달하여 안정된 상태인지</summary>
        public bool IsStable { get; private set; }

        #endregion

        /// <summary>
        /// 생성자 - 기준값으로 초기화
        /// </summary>
        public ProcessParameters()
        {
            Reset();
        }

        /// <summary>
        /// 모든 값을 기준값으로 초기화
        /// </summary>
        public void Reset()
        {
            CurrentTemperature = BASELINE_TEMP;
            CurrentPressure = BASELINE_PRESSURE;
            CurrentHV = BASELINE_HV;
            CurrentDose = BASELINE_DOSE;

            TargetTemperature = 0;
            TargetPressure = 0;
            TargetHV = 0;
            TargetDose = 0;

            IsRising = false;
            IsFalling = false;
            IsStable = false;
        }

        /// <summary>
        /// 공정 시작 시 호출 - 목표값을 향해 상승 시작
        /// </summary>
        public void StartRising()
        {
            IsRising = true;
            IsFalling = false;
            IsStable = false;
        }

        /// <summary>
        /// 공정 종료 시 호출 - 기준값을 향해 하강 시작
        /// </summary>
        public void StartFalling()
        {
            IsRising = false;
            IsFalling = true;
            IsStable = false;
        }

        /// <summary>
        /// 타이머에서 호출 - 애니메이션 업데이트
        /// </summary>
        /// <param name="deltaSeconds">경과 시간 (초)</param>
        public void Update(double deltaSeconds)
        {
            if (IsRising)
            {
                UpdateRising(deltaSeconds);
            }
            else if (IsFalling)
            {
                UpdateFalling(deltaSeconds);
            }
            else if (IsStable)
            {
                // 안정 상태에서는 목표 도즈값 유지 (레시피에서 설정한 값)
                CurrentDose = TargetDose;

                // 약간의 랜덤 변동 추가 (더 현실적인 시뮬레이션)
                AddRandomFluctuation();
            }
        }

        /// <summary>
        /// 상승 애니메이션 처리
        /// </summary>
        private void UpdateRising(double deltaSeconds)
        {
            bool allReached = true;

            // 온도 상승
            if (CurrentTemperature < TargetTemperature)
            {
                CurrentTemperature = Math.Min(
                    CurrentTemperature + TEMP_RISE_RATE * deltaSeconds,
                    TargetTemperature
                );
                allReached = false;
            }

            // 압력 감소 (진공으로 이동)
            if (CurrentPressure > TargetPressure)
            {
                CurrentPressure = Math.Max(
                    CurrentPressure * Math.Pow(PRESSURE_RISE_FACTOR, deltaSeconds),
                    TargetPressure
                );
                allReached = false;
            }

            // 가속 전압 HV 상승
            if (CurrentHV < TargetHV)
            {
                CurrentHV = Math.Min(
                    CurrentHV + HV_RISE_RATE * deltaSeconds,
                    TargetHV
                );
                allReached = false;
            }

            // 도즈 상승 (목표값까지 점진적으로)
            if (TargetDose > 0 && CurrentDose < TargetDose)
            {
                double doseRiseRate = TargetDose / DOSE_RISE_TIME;
                double oldDose = CurrentDose;
                CurrentDose = Math.Min(
                    CurrentDose + doseRiseRate * deltaSeconds,
                    TargetDose
                );
                // 디버그 로그 (처음 한 번만)
                if (oldDose == 0 && CurrentDose > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Dose 상승 시작: {oldDose:E2} → {CurrentDose:E2} (Target: {TargetDose:E2})");
                }
                allReached = false;
            }

            // 모든 값이 목표에 도달하면 안정 상태로 전환
            if (allReached)
            {
                IsRising = false;
                IsStable = true;
            }
        }

        /// <summary>
        /// 하강 애니메이션 처리
        /// </summary>
        private void UpdateFalling(double deltaSeconds)
        {
            bool allReached = true;

            // 온도 하강
            if (CurrentTemperature > BASELINE_TEMP)
            {
                CurrentTemperature = Math.Max(
                    CurrentTemperature - TEMP_FALL_RATE * deltaSeconds,
                    BASELINE_TEMP
                );
                allReached = false;
            }

            // 압력 증가 (대기압으로 복귀)
            if (CurrentPressure < BASELINE_PRESSURE)
            {
                CurrentPressure = Math.Min(
                    CurrentPressure * Math.Pow(PRESSURE_FALL_FACTOR, deltaSeconds),
                    BASELINE_PRESSURE
                );
                allReached = false;
            }

            // 가속 전압 HV 하강
            if (CurrentHV > BASELINE_HV)
            {
                CurrentHV = Math.Max(
                    CurrentHV - HV_FALL_RATE * deltaSeconds,
                    BASELINE_HV
                );
                allReached = false;
            }

            // 모든 값이 기준에 도달하면 하강 완료
            if (allReached)
            {
                IsFalling = false;
                CurrentDose = 0.0;  // 도즈 리셋
            }
        }

        /// <summary>
        /// 안정 상태에서 약간의 랜덤 변동 추가 (±0.5%)
        /// </summary>
        private Random random = new Random();
        private void AddRandomFluctuation()
        {
            double fluctuation = (random.NextDouble() - 0.5) * 0.01; // ±0.5%

            // 온도 변동
            CurrentTemperature = TargetTemperature * (1.0 + fluctuation);

            // HV 변동
            CurrentHV = TargetHV * (1.0 + fluctuation * 0.5); // HV는 더 안정적

            // 압력 변동
            CurrentPressure = TargetPressure * (1.0 + fluctuation * 2); // 압력은 더 민감
        }

        #region 표시 문자열 생성 메서드

        /// <summary>온도 표시 문자열 ("800.0 C")</summary>
        public string GetTemperatureDisplay()
        {
            return $"{CurrentTemperature:F1} C";
        }

        /// <summary>압력 표시 문자열 (진공: "1.00E-05 Torr", 대기압: "760.00 Torr")</summary>
        public string GetPressureDisplay()
        {
            if (CurrentPressure < 0.01)
            {
                return $"{CurrentPressure:E2} Torr";
            }
            else
            {
                return $"{CurrentPressure:F2} Torr";
            }
        }

        /// <summary>가속 전압 HV 표시 문자열 ("100.0 kV")</summary>
        public string GetHVDisplay()
        {
            return $"{CurrentHV:F1} kV";
        }

        /// <summary>도즈 표시 문자열 ("1.00E+13 ions/cm2")</summary>
        public string GetDoseDisplay()
        {
            if (CurrentDose == 0)
            {
                return "0 ions/cm2";
            }
            return $"{CurrentDose:E2} ions/cm2";
        }

        #endregion
    }
}
