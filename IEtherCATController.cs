using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// EtherCAT 하드웨어 제어를 위한 추상 인터페이스
    /// 실제 하드웨어(RealEtherCATController)와 시뮬레이션(SimulationEtherCATController)을 동일한 인터페이스로 제어
    /// </summary>
    public interface IEtherCATController
    {
        /// <summary>
        /// 연결 상태
        /// </summary>
        bool IsConnected { get; }

        #region 축 제어 (Axis Control)

        /// <summary>
        /// UD(상하) 축을 지정된 위치로 이동
        /// </summary>
        /// <param name="position">목표 위치</param>
        /// <returns>이동 완료 여부</returns>
        Task<bool> MoveUDAxis(long position);

        /// <summary>
        /// LR(좌우/회전) 축을 지정된 위치로 이동
        /// </summary>
        /// <param name="position">목표 위치</param>
        /// <returns>이동 완료 여부</returns>
        Task<bool> MoveLRAxis(long position);

        /// <summary>
        /// UD축 원점복귀 (Homing)
        /// </summary>
        /// <returns>원점복귀 완료 여부</returns>
        Task<bool> HomeUDAxis();

        /// <summary>
        /// LR축 원점복귀 (Homing)
        /// </summary>
        /// <returns>원점복귀 완료 여부</returns>
        Task<bool> HomeLRAxis();

        #endregion

        #region 서보 제어 (Servo Control)

        /// <summary>
        /// UD축 서보 ON/OFF
        /// </summary>
        void SetServoUD(bool on);

        /// <summary>
        /// LR축 서보 ON/OFF
        /// </summary>
        void SetServoLR(bool on);

        #endregion

        #region TM 실린더 제어 (Cylinder Control)

        /// <summary>
        /// 웨이퍼 이송 실린더 전진
        /// </summary>
        void ExtendCylinder();

        /// <summary>
        /// 웨이퍼 이송 실린더 후진
        /// </summary>
        void RetractCylinder();

        #endregion

        #region 웨이퍼 흡착 제어 (Vacuum Control)

        /// <summary>
        /// 흡착(Suction) ON - 웨이퍼 잡기
        /// </summary>
        void EnableSuction();

        /// <summary>
        /// 흡착(Suction) OFF
        /// </summary>
        void DisableSuction();

        /// <summary>
        /// 배기(Exhaust) ON - 웨이퍼 놓기
        /// </summary>
        void EnableExhaust();

        /// <summary>
        /// 배기(Exhaust) OFF
        /// </summary>
        void DisableExhaust();

        #endregion

        #region PM 제어 (Process Module Control)

        /// <summary>
        /// PM 문 열기
        /// </summary>
        /// <param name="pm">PM 타입 (PM1, PM2, PM3)</param>
        void OpenPMDoor(ProcessModule.ModuleType pm);

        /// <summary>
        /// PM 문 닫기
        /// </summary>
        /// <param name="pm">PM 타입 (PM1, PM2, PM3)</param>
        void ClosePMDoor(ProcessModule.ModuleType pm);

        /// <summary>
        /// PM 램프 제어
        /// </summary>
        /// <param name="pm">PM 타입 (PM1, PM2, PM3)</param>
        /// <param name="on">ON/OFF</param>
        void SetPMLamp(ProcessModule.ModuleType pm, bool on);

        #endregion

        #region 타워 램프 제어 (Tower Lamp Control)

        /// <summary>
        /// 타워 램프 제어 (한 번에 하나만 점등)
        /// DO 채널: 0(적색), 1(황색), 2(녹색)
        /// </summary>
        /// <param name="state">램프 상태</param>
        void SetTowerLamp(TowerLampState state);

        #endregion
    }
}
