namespace IonImplationEtherCAT
{
    /// <summary>
    /// 하드웨어 위치 좌표 매핑
    /// 모든 좌표값은 EtherCAT 모터 위치 단위
    /// </summary>
    public static class HardwarePositionMap
    {
        #region FOUP A 좌표 (LR: 13226)

        /// <summary>
        /// FOUP A의 LR(좌우) 좌표
        /// </summary>
        public const long LR_FOUP_A = 13226;

        /// <summary>
        /// FOUP A 각 슬롯의 UD 안착 위치 (웨이퍼 아래)
        /// </summary>

        public static readonly long[] FOUP_A_UD_SEATING = new long[]
        {
            125188,   // 1층
            789441,   // 2층
            1482288,  // 3층
            2143669,  // 4층
            2833894   // 5층
        };

        /// <summary>
        /// FOUP A 각 슬롯의 UD 상승 위치 (웨이퍼 들어올림)
        /// </summary>
        public static readonly long[] FOUP_A_UD_LIFTED = new long[]
        {
            185188,   // 1층
            901866,   // 2층
            1622288,  // 3층
            2323699,  // 4층
            3014110   // 5층
        };

        #endregion

        #region FOUP B 좌표 (LR: -395519)

        /// <summary>
        /// FOUP B의 LR(좌우) 좌표
        /// </summary>
        public const long LR_FOUP_B = -395219;

        /// <summary>
        /// FOUP B 각 슬롯의 UD 안착 위치 (FOUP A와 동일)
        /// </summary>
        public static readonly long[] FOUP_B_UD_SEATING = FOUP_A_UD_SEATING;

        /// <summary>
        /// FOUP B 각 슬롯의 UD 상승 위치 (FOUP A와 동일)
        /// </summary>
        public static readonly long[] FOUP_B_UD_LIFTED = FOUP_A_UD_LIFTED;

        #endregion

        #region PM 좌표 (안착: 706500, 상승: 1156931)

        /// <summary>
        /// PM1의 LR(좌우) 좌표
        /// </summary>
        public const long LR_PM1 = -59064;

        /// <summary>
        /// PM2의 LR(좌우) 좌표
        /// </summary>
        public const long LR_PM2 = -190823;

        /// <summary>
        /// PM3의 LR(좌우) 좌표
        /// </summary>
        public const long LR_PM3 = -322000;

        /// <summary>
        /// PM 공통 UD 안착 위치 (웨이퍼 아래)
        /// </summary>
        public const long PM_UD_SEATING = 806931;

        /// <summary>
        /// PM 공통 UD 상승 위치 (웨이퍼 들어올림)
        /// </summary>
        public const long PM_UD_LIFTED = 1156931;

        #endregion

        #region 헬퍼 메서드

        /// <summary>
        /// FOUP 슬롯 인덱스(0-4)로 UD 안착 위치 가져오기
        /// </summary>
        /// <param name="slotIndex">슬롯 인덱스 (0=1층, 4=5층)</param>
        /// <param name="isFoupA">FOUP A 여부 (false면 FOUP B)</param>
        /// <returns>UD 안착 위치</returns>
        public static long GetFoupSeatingPosition(int slotIndex, bool isFoupA = true)
        {
            if (slotIndex < 0 || slotIndex > 4) return 0;
            return isFoupA ? FOUP_A_UD_SEATING[slotIndex] : FOUP_B_UD_SEATING[slotIndex];
        }

        /// <summary>
        /// FOUP 슬롯 인덱스(0-4)로 UD 상승 위치 가져오기
        /// </summary>
        /// <param name="slotIndex">슬롯 인덱스 (0=1층, 4=5층)</param>
        /// <param name="isFoupA">FOUP A 여부 (false면 FOUP B)</param>
        /// <returns>UD 상승 위치</returns>
        public static long GetFoupLiftedPosition(int slotIndex, bool isFoupA = true)
        {
            if (slotIndex < 0 || slotIndex > 4) return 0;
            return isFoupA ? FOUP_A_UD_LIFTED[slotIndex] : FOUP_B_UD_LIFTED[slotIndex];
        }

        /// <summary>
        /// PM 타입으로 LR 좌표 가져오기
        /// </summary>
        /// <param name="pmType">PM 타입</param>
        /// <returns>LR 좌표</returns>
        public static long GetPMLRPosition(ProcessModule.ModuleType pmType)
        {
            switch (pmType)
            {
                case ProcessModule.ModuleType.PM1:
                    return LR_PM1;
                case ProcessModule.ModuleType.PM2:
                    return LR_PM2;
                case ProcessModule.ModuleType.PM3:
                    return LR_PM3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// LR(좌우) 좌표로 위치 이름 가져오기
        /// </summary>
        /// <param name="lrPosition">LR 좌표</param>
        /// <returns>위치 이름 (예: "FOUP A", "PM1" 등)</returns>
        public static string GetLRPositionName(long lrPosition)
        {
            const long tolerance = 5000; // 허용 오차

            if (System.Math.Abs(lrPosition - LR_FOUP_A) < tolerance)
                return "FOUP A";
            if (System.Math.Abs(lrPosition - LR_FOUP_B) < tolerance)
                return "FOUP B";
            if (System.Math.Abs(lrPosition - LR_PM1) < tolerance)
                return "PM1";
            if (System.Math.Abs(lrPosition - LR_PM2) < tolerance)
                return "PM2";
            if (System.Math.Abs(lrPosition - LR_PM3) < tolerance)
                return "PM3";

            return $"LR:{lrPosition}";
        }

        /// <summary>
        /// UD(상하) 좌표로 위치 이름 가져오기
        /// </summary>
        /// <param name="udPosition">UD 좌표</param>
        /// <returns>위치 이름 (예: "FOUP A 1층 안착", "PM 상승" 등)</returns>
        public static string GetUDPositionName(long udPosition)
        {
            const long tolerance = 5000; // 허용 오차

            // PM 위치 확인
            if (System.Math.Abs(udPosition - PM_UD_SEATING) < tolerance)
                return "PM 안착 위치";
            if (System.Math.Abs(udPosition - PM_UD_LIFTED) < tolerance)
                return "PM 상승 위치";

            // FOUP 슬롯 위치 확인 (안착)
            for (int i = 0; i < FOUP_A_UD_SEATING.Length; i++)
            {
                if (System.Math.Abs(udPosition - FOUP_A_UD_SEATING[i]) < tolerance)
                    return $"FOUP {i + 1}층 안착 위치";
            }

            // FOUP 슬롯 위치 확인 (상승)
            for (int i = 0; i < FOUP_A_UD_LIFTED.Length; i++)
            {
                if (System.Math.Abs(udPosition - FOUP_A_UD_LIFTED[i]) < tolerance)
                    return $"FOUP {i + 1}층 상승 위치";
            }

            // 원점 근처
            if (System.Math.Abs(udPosition) < tolerance)
                return "원점";

            return $"UD:{udPosition}";
        }

        #endregion
    }
}
