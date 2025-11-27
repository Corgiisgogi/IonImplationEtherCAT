namespace IonImplationEtherCAT
{
    /// <summary>
    /// 하드웨어 위치 좌표 매핑
    /// 모든 좌표값은 EtherCAT 모터 위치 단위
    /// </summary>
    public static class HardwarePositionMap
    {
        #region FOUP A 좌표 (LR: 12911)
        //도겸씨 자리로 일시 조정함 (테스트)

        /// <summary>
        /// FOUP A의 LR(좌우) 좌표
        /// </summary>
        //public const long LR_FOUP_A = 12911;
        public const long LR_FOUP_A = 14140;

        /// <summary>
        /// FOUP A 각 슬롯의 UD 안착 위치 (웨이퍼 아래)
        /// </summary>
        //public static readonly long[] FOUP_A_UD_SEATING = new long[]
        //{
        //    129136,   // 1층
        //    789441,   // 2층
        //    1432388,  // 3층
        //    2123669,  // 4층
        //    2813894   // 5층
        //};

        public static readonly long[] FOUP_A_UD_SEATING = new long[]
        {
            72379,   // 1층
            752378,   // 2층
            1402388,  // 3층
            2089399,  // 4층
            2788463   // 5층
        };

        /// <summary>
        /// FOUP A 각 슬롯의 UD 상승 위치 (웨이퍼 들어올림)
        /// </summary>
        public static readonly long[] FOUP_A_UD_LIFTED = new long[]
        {
            302380,   // 1층
            982378,   // 2층
            1627604,  // 3층
            2332102,  // 4층
            3018457   // 5층
        };

        #endregion

        #region FOUP B 좌표 (LR: -395519)

        /// <summary>
        /// FOUP B의 LR(좌우) 좌표
        /// </summary>
        public const long LR_FOUP_B = -394293;

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
        public const long LR_PM3 = -321600;

        /// <summary>
        /// PM 공통 UD 안착 위치 (웨이퍼 아래)
        /// </summary>
        public const long PM_UD_SEATING = 776931;

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

        #endregion
    }
}
