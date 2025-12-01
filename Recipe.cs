using System;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// PM1/PM2용 이온 주입 레시피 파라미터
    /// </summary>
    [Serializable]
    public class IonImplantRecipe
    {
        // RecipeView UI 파라미터
        public string IonGas { get; set; }              // 이온 가스 종류
        public string Gas { get; set; }                 // 캐리어 가스 종류
        public double Dose { get; set; }                // 도즈량
        public double Voltage { get; set; }             // 목표 전압 (kV)
        public double Current { get; set; }             // 자석 전류 (A)
        public double Motor { get; set; }               // 스캐너 모터 속도
        public double ScannerVoltage { get; set; }      // 스캐너 전압 (V)
        public int ProcessTime { get; set; }            // 공정 시간 (초)

        // 시뮬레이션 목표값 (공정 중 표시)
        public double TargetTemperature { get; set; }   // 목표 온도 (°C)
        public double TargetPressure { get; set; }      // 목표 압력 (Torr)
        public double TargetHV { get; set; }            // 가속 전압 HV (kV) - Voltage 값 사용

        public IonImplantRecipe()
        {
            IonGas = "";
            Gas = "";
            Dose = 0;
            Voltage = 0;
            Current = 0;
            Motor = 0;
            ScannerVoltage = 0;
            ProcessTime = 30;

            // 기본 시뮬레이션 목표값 (PM1/PM2: 이온 주입)
            TargetTemperature = 800.0;      // 600-1000°C 범위
            TargetPressure = 1E-5;          // 진공 상태 (Torr)
            TargetHV = 100.0;               // 가속 전압 (kV)
        }
    }

    /// <summary>
    /// PM3용 어닐링 레시피 파라미터
    /// </summary>
    [Serializable]
    public class AnnealingRecipe
    {
        // RecipeView UI 파라미터
        public double Vacuum { get; set; }              // 진공 레벨 (Torr)
        public double Temperature { get; set; }         // 설정 온도 (°C)
        // ProcessTime은 MainView에서 별도 설정하므로 제거

        // 시뮬레이션 목표값 (공정 중 표시) - Temperature, Vacuum 값이 직접 적용됨
        public double TargetTemperature { get; set; }   // 목표 온도 (°C)
        public double TargetPressure { get; set; }      // 목표 압력 (Torr)

        public AnnealingRecipe()
        {
            Vacuum = 1E-6;              // 기본 진공 레벨
            Temperature = 950.0;        // 기본 온도

            // 시뮬레이션 목표값 = UI 입력값 사용
            TargetTemperature = 950.0;
            TargetPressure = 1E-6;
        }
    }

    /// <summary>
    /// 전체 레시피 세트 (PM1, PM2, PM3 포함)
    /// </summary>
    [Serializable]
    public class RecipeSet
    {
        public string Name { get; set; }
        public DateTime LastModified { get; set; }
        public IonImplantRecipe PM1Recipe { get; set; }
        public IonImplantRecipe PM2Recipe { get; set; }
        public AnnealingRecipe PM3Recipe { get; set; }

        public RecipeSet()
        {
            Name = "Default";
            LastModified = DateTime.Now;
            PM1Recipe = new IonImplantRecipe();
            PM2Recipe = new IonImplantRecipe();
            PM3Recipe = new AnnealingRecipe();
        }
    }
}
