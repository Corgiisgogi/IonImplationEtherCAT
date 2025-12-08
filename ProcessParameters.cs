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

        // 안정화 목표 시간 (초) - 모든 파라미터가 이 시간 내에 목표값 도달
        private const double STABILIZATION_TIME = 10.0;

        // 동적으로 계산되는 상승 속도 (StartRising에서 설정)
        private double calculatedTempRiseRate;
        private double calculatedHVRiseRate;
        private double calculatedPressureFactor;

        // 하강 속도 (고정)
        private const double TEMP_FALL_RATE = 100.0;     // 빠른 냉각
        private const double HV_FALL_RATE = 50.0;        // 빠른 HV 차단
        private const double PRESSURE_FALL_FACTOR = 2.0; // 빠른 벤트

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

        #region 파라미터 이탈 상태

        /// <summary>파라미터가 이탈된 상태인지</summary>
        public bool IsDeviated { get; set; }

        /// <summary>이탈된 파라미터 이름 (Temperature, Pressure, HV)</summary>
        public string DeviatedParameterName { get; set; }

        /// <summary>Alarm 수준 이탈인지 (true: Alarm/10%초과, false: Warning/5~10%)</summary>
        public bool IsAlarmLevel { get; set; }

        /// <summary>이탈 시 저장된 원래 목표값 대비 편차 (%)</summary>
        public double DeviationPercent { get; set; }

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

            // 이탈 상태 초기화
            IsDeviated = false;
            DeviatedParameterName = null;
            IsAlarmLevel = false;
            DeviationPercent = 0;
        }

        /// <summary>
        /// 공정 시작 시 호출 - 목표값을 향해 상승 시작
        /// 레시피 목표값에서 동적으로 Raise Rate를 계산
        /// </summary>
        public void StartRising()
        {
            IsRising = true;
            IsFalling = false;
            IsStable = false;

            // 동적 Raise Rate 계산 (안정화 시간 내에 목표값 도달)
            // 온도: 목표 온도에서 상온을 뺀 값을 안정화 시간으로 나눔
            double tempDelta = TargetTemperature - BASELINE_TEMP;
            calculatedTempRiseRate = tempDelta > 0 ? tempDelta / STABILIZATION_TIME : 100.0;

            // HV: 목표 HV를 안정화 시간으로 나눔
            calculatedHVRiseRate = TargetHV > 0 ? TargetHV / STABILIZATION_TIME : 10.0;

            // 압력: 목표 진공까지 10초 내 도달하도록 지수 감소율 계산
            // 760 Torr → 1E-5 Torr (약 7.6E7 배 감소)
            // factor^10 = targetPressure / 760 => factor = (targetPressure/760)^0.1
            if (TargetPressure > 0 && TargetPressure < BASELINE_PRESSURE)
            {
                calculatedPressureFactor = Math.Pow(TargetPressure / BASELINE_PRESSURE, 1.0 / STABILIZATION_TIME);
            }
            else
            {
                calculatedPressureFactor = 0.5; // 기본값
            }
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
        /// 상승 애니메이션 처리 (동적 계산된 속도 사용)
        /// </summary>
        private void UpdateRising(double deltaSeconds)
        {
            bool allReached = true;

            // 온도 상승 (동적 계산된 속도)
            if (CurrentTemperature < TargetTemperature)
            {
                CurrentTemperature = Math.Min(
                    CurrentTemperature + calculatedTempRiseRate * deltaSeconds,
                    TargetTemperature
                );
                allReached = false;
            }

            // 압력 감소 (진공으로 이동, 동적 계산된 지수 감소율)
            if (CurrentPressure > TargetPressure)
            {
                CurrentPressure = Math.Max(
                    CurrentPressure * Math.Pow(calculatedPressureFactor, deltaSeconds),
                    TargetPressure
                );
                allReached = false;
            }

            // 가속 전압 HV 상승 (동적 계산된 속도)
            if (CurrentHV < TargetHV)
            {
                CurrentHV = Math.Min(
                    CurrentHV + calculatedHVRiseRate * deltaSeconds,
                    TargetHV
                );
                allReached = false;
            }

            // 도즈는 UpdatePhased에서 처리 (안정화 후 적산 시작)
            // 여기서는 도즈 체크 안 함 (allReached에 영향 없음)

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

        #region 공정 절차에 맞춘 파라미터 변화

        /// <summary>
        /// 공정 절차에 맞춘 파라미터 업데이트 (이온 주입/어닐링)
        /// </summary>
        /// <param name="elapsed">경과 시간 (초)</param>
        /// <param name="total">전체 공정 시간 (초)</param>
        /// <param name="isAnnealing">어닐링 공정 여부 (false면 이온 주입)</param>
        /// <param name="deltaSeconds">이번 프레임 경과 시간</param>
        public void UpdatePhased(double elapsed, double total, bool isAnnealing, double deltaSeconds)
        {
            if (isAnnealing)
            {
                UpdatePhasedAnnealing(elapsed, total, deltaSeconds);
            }
            else
            {
                UpdatePhasedIonImplant(elapsed, total, deltaSeconds);
            }
        }

        /// <summary>
        /// 이온 주입 공정 파라미터 변화 (PM1, PM2)
        /// 마무리 단계: 10초 (HV OFF 5초 → 온도 하강 3초 → 벤트 2초)
        /// </summary>
        private void UpdatePhasedIonImplant(double elapsed, double total, double deltaSeconds)
        {
            double timeToEnd = total - elapsed;

            // 종료 10초 전~: 마무리 단계 시작 (가장 먼저 체크)
            if (timeToEnd <= 10)
            {
                // 도즈 목표 도달 (유지)
                CurrentDose = TargetDose;

                if (timeToEnd > 5)
                {
                    // 종료 10~5초: HV OFF (5초 내 완료)
                    double hvFallRate = TargetHV / 5.0;
                    CurrentHV = Math.Max(CurrentHV - hvFallRate * deltaSeconds, 0);
                    // 온도와 압력은 아직 유지
                    CurrentTemperature = TargetTemperature;
                    CurrentPressure = TargetPressure;
                }
                else if (timeToEnd > 2)
                {
                    // 종료 5~2초: Temperature 하강 (3초 내 상온 복귀)
                    CurrentHV = 0;
                    double tempFallRate = (TargetTemperature - BASELINE_TEMP) / 3.0;
                    CurrentTemperature = Math.Max(CurrentTemperature - tempFallRate * deltaSeconds, BASELINE_TEMP);
                    // 압력은 아직 유지
                    CurrentPressure = TargetPressure;
                }
                else
                {
                    // 종료 2초 전~: Pressure 대기압 복귀 (2초 내 완료)
                    CurrentHV = 0;
                    CurrentTemperature = BASELINE_TEMP;
                    double pressureRiseFactor = Math.Pow(BASELINE_PRESSURE / TargetPressure, 1.0 / 2.0);
                    CurrentPressure = Math.Min(CurrentPressure * Math.Pow(pressureRiseFactor, deltaSeconds), BASELINE_PRESSURE);
                }
            }
            // 0~3초: 진공 펌핑 (Pressure 감소만)
            else if (elapsed <= 3)
            {
                // 압력 감소
                if (CurrentPressure > TargetPressure)
                {
                    double pressureRate = Math.Pow(calculatedPressureFactor, deltaSeconds);
                    CurrentPressure = Math.Max(CurrentPressure * pressureRate, TargetPressure);
                }
            }
            // 3~5초: 이온 소스 ON (온도 상승 시작, HV는 아직 낮음)
            else if (elapsed <= 5)
            {
                // 압력 계속 감소
                if (CurrentPressure > TargetPressure)
                {
                    CurrentPressure = Math.Max(CurrentPressure * Math.Pow(calculatedPressureFactor, deltaSeconds), TargetPressure);
                }
                // 온도 상승 시작 (느리게)
                if (CurrentTemperature < TargetTemperature * 0.3)
                {
                    CurrentTemperature += calculatedTempRiseRate * deltaSeconds;
                }
            }
            // 5~8초: HV 상승 (가속 전압 인가)
            else if (elapsed <= 8)
            {
                // 압력 목표 도달
                CurrentPressure = TargetPressure;
                // 온도 계속 상승
                if (CurrentTemperature < TargetTemperature)
                {
                    CurrentTemperature = Math.Min(CurrentTemperature + calculatedTempRiseRate * deltaSeconds, TargetTemperature);
                }
                // HV 빠르게 상승
                double hvPhaseRate = TargetHV / 3.0; // 3초 안에 완료
                if (CurrentHV < TargetHV)
                {
                    CurrentHV = Math.Min(CurrentHV + hvPhaseRate * deltaSeconds, TargetHV);
                }
            }
            // 8~10초: 모든 파라미터 안정화 완료
            else if (elapsed <= 10)
            {
                CurrentPressure = TargetPressure;
                CurrentTemperature = TargetTemperature;
                CurrentHV = TargetHV;
                // 도즈 아직 시작 안 함
            }
            // 10초~(종료 10초 전): 이온 주입 중 (도즈 적산)
            else
            {
                CurrentPressure = TargetPressure;
                CurrentTemperature = TargetTemperature;
                CurrentHV = TargetHV;

                // 도즈 적산 (남은 시간 동안 목표값까지 증가)
                double implantTime = total - 10 - 10; // 10초 안정화 + 10초 마무리 제외
                if (implantTime > 0 && CurrentDose < TargetDose)
                {
                    double doseRate = TargetDose / implantTime;
                    CurrentDose = Math.Min(CurrentDose + doseRate * deltaSeconds, TargetDose);
                }

                // 약간의 변동 추가 (안정 구간에서)
                AddSmallFluctuation();
            }
        }

        /// <summary>
        /// 어닐링 공정 파라미터 변화 (PM3 - RTA)
        /// 마무리 단계: 10초 (냉각1 5초 → 냉각2 3초 → 벤트 2초)
        /// </summary>
        private void UpdatePhasedAnnealing(double elapsed, double total, double deltaSeconds)
        {
            double timeToEnd = total - elapsed;

            // 종료 10초 전~: 마무리 단계 시작 (가장 먼저 체크)
            if (timeToEnd <= 10)
            {
                // HV, 도즈 없음 (어닐링)
                CurrentHV = 0;
                CurrentDose = 0;

                if (timeToEnd > 5)
                {
                    // 종료 10~5초: Temperature 냉각 1단계 (5초 내 50% 감소)
                    double tempFallRate = (TargetTemperature - BASELINE_TEMP) * 0.5 / 5.0;
                    CurrentTemperature = Math.Max(CurrentTemperature - tempFallRate * deltaSeconds, BASELINE_TEMP);
                    // 압력은 아직 유지
                    CurrentPressure = TargetPressure;
                }
                else if (timeToEnd > 2)
                {
                    // 종료 5~2초: Temperature 냉각 2단계 (3초 내 상온 도달)
                    double tempFallRate = (CurrentTemperature - BASELINE_TEMP) / 3.0;
                    if (tempFallRate < 50) tempFallRate = 50; // 최소 속도 보장
                    CurrentTemperature = Math.Max(CurrentTemperature - tempFallRate * deltaSeconds, BASELINE_TEMP);
                    // 압력은 아직 유지
                    CurrentPressure = TargetPressure;
                }
                else
                {
                    // 종료 2초 전~: Pressure 대기압 복귀 (2초 내 완료)
                    CurrentTemperature = BASELINE_TEMP;
                    double pressureRiseFactor = Math.Pow(BASELINE_PRESSURE / TargetPressure, 1.0 / 2.0);
                    CurrentPressure = Math.Min(CurrentPressure * Math.Pow(pressureRiseFactor, deltaSeconds), BASELINE_PRESSURE);
                }
            }
            // 0~3초: 진공 펌핑
            else if (elapsed <= 3)
            {
                if (CurrentPressure > TargetPressure)
                {
                    CurrentPressure = Math.Max(CurrentPressure * Math.Pow(calculatedPressureFactor, deltaSeconds), TargetPressure);
                }
            }
            // 3~5초: RTA 램프 ON, 급속 가열 시작
            else if (elapsed <= 5)
            {
                CurrentPressure = TargetPressure;
                // 급속 가열 (목표 온도까지 2초 내 도달)
                double rapidHeatRate = TargetTemperature / 2.0;
                if (CurrentTemperature < TargetTemperature)
                {
                    CurrentTemperature = Math.Min(CurrentTemperature + rapidHeatRate * deltaSeconds, TargetTemperature);
                }
            }
            // 5초~(종료 10초 전): 어닐링 진행 (목표 온도 유지)
            else
            {
                CurrentPressure = TargetPressure;
                CurrentTemperature = TargetTemperature;
                // HV 없음 (어닐링)
                CurrentHV = 0;
                // 도즈 없음 (어닐링)
                CurrentDose = 0;

                // 약간의 변동 추가
                AddSmallFluctuation();
            }
        }

        /// <summary>
        /// 작은 변동 추가 (안정 구간에서 더 현실적인 시뮬레이션)
        /// </summary>
        private void AddSmallFluctuation()
        {
            double fluctuation = (random.NextDouble() - 0.5) * 0.002; // ±0.1%
            CurrentTemperature *= (1.0 + fluctuation);
            CurrentHV *= (1.0 + fluctuation * 0.5);
        }

        #endregion

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

        #region 파라미터 이탈 시뮬레이션

        /// <summary>
        /// 1% 확률로 파라미터 이탈 시뮬레이션
        /// </summary>
        /// <param name="tempTolerance">온도 허용 편차 (%)</param>
        /// <param name="pressureTolerance">압력 허용 편차 (%)</param>
        /// <param name="hvTolerance">HV 허용 편차 (%)</param>
        /// <param name="isAnnealing">어닐링 공정 여부 (true면 HV 이탈 없음)</param>
        /// <returns>이탈 발생 시 (파라미터명, 이탈값, 목표값, 편차%), 없으면 null</returns>
        public (string ParameterName, double DeviatedValue, double TargetValue, double DeviationPercent)?
            SimulateParameterDeviation(double tempTolerance, double pressureTolerance, double hvTolerance, bool isAnnealing)
        {
            // 이미 이탈 상태면 추가 이탈 없음
            if (IsDeviated) return null;

            // 0.33% 확률 체크 (기존 1%의 1/3)
            if (random.NextDouble() > 0.0033) return null;

            // 파라미터 유형 선택 (온도:40%, 압력:30%, HV:30%)
            // 어닐링이면 HV 제외 (온도:57%, 압력:43%)
            double typeRoll = random.NextDouble();
            double tempThreshold = isAnnealing ? 0.57 : 0.4;
            double pressureThreshold = isAnnealing ? 1.0 : 0.7;

            // 이탈 심각도 결정 (50% Warning, 50% Alarm)
            bool isAlarm = random.NextDouble() > 0.5;

            // 이탈 편차 계산 (Warning: 허용범위+2~5%, Alarm: 허용범위+7~15%)
            double extraDeviation;
            if (isAlarm)
            {
                extraDeviation = 7 + random.NextDouble() * 8; // 7~15% 추가
            }
            else
            {
                extraDeviation = 2 + random.NextDouble() * 3; // 2~5% 추가
            }

            // 방향 결정 (50% 상승, 50% 하강)
            int sign = random.Next(2) == 0 ? 1 : -1;

            string paramName;
            double deviatedValue, targetValue, deviationPercent;

            if (typeRoll < tempThreshold)
            {
                // 온도 이탈
                paramName = "Temperature";
                targetValue = TargetTemperature;
                deviationPercent = tempTolerance + extraDeviation;
                deviatedValue = targetValue * (1 + sign * deviationPercent / 100);
                CurrentTemperature = deviatedValue;
            }
            else if (typeRoll < pressureThreshold)
            {
                // 압력 이탈 (로그 스케일 고려)
                paramName = "Pressure";
                targetValue = TargetPressure;
                deviationPercent = pressureTolerance + extraDeviation;
                // 압력은 배수로 이탈 (상승 방향만 - 진공 손실)
                double multiplier = 1 + (deviationPercent / 100) * 10; // 10배 더 민감
                deviatedValue = targetValue * multiplier;
                CurrentPressure = deviatedValue;
            }
            else
            {
                // HV 이탈 (어닐링 아닐 때만)
                paramName = "HV";
                targetValue = TargetHV;
                deviationPercent = hvTolerance + extraDeviation;
                deviatedValue = targetValue * (1 + sign * deviationPercent / 100);
                CurrentHV = deviatedValue;
            }

            // 이탈 상태 저장
            IsDeviated = true;
            DeviatedParameterName = paramName;
            IsAlarmLevel = isAlarm;
            DeviationPercent = deviationPercent;

            return (paramName, deviatedValue, targetValue, deviationPercent);
        }

        /// <summary>
        /// 이탈된 파라미터를 목표 범위 내로 복구
        /// </summary>
        public void RestoreToNormal()
        {
            if (!IsDeviated) return;

            // 이탈된 파라미터를 목표값으로 복구
            switch (DeviatedParameterName)
            {
                case "Temperature":
                    CurrentTemperature = TargetTemperature;
                    break;
                case "Pressure":
                    CurrentPressure = TargetPressure;
                    break;
                case "HV":
                    CurrentHV = TargetHV;
                    break;
            }

            // 이탈 상태 초기화
            IsDeviated = false;
            DeviatedParameterName = null;
            IsAlarmLevel = false;
            DeviationPercent = 0;
        }

        #endregion
    }
}
