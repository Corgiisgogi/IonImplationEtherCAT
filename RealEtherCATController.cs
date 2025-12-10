using System;
using System.Threading.Tasks;
using IEG3268_Dll;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 실제 EtherCAT 하드웨어 컨트롤러
    /// IEG3268 DLL을 사용하여 실제 장비 제어
    /// </summary>
    public class RealEtherCATController : IEtherCATController
    {
        private readonly IEG3268 etherCAT;

        // PM 문 상태 추적
        private bool isPM1DoorOpen = false;
        private bool isPM2DoorOpen = false;
        private bool isPM3DoorOpen = false;

        #region DO 채널 상수

        // 타워 램프
        private const int DO_TOWER_RED = 0;
        private const int DO_TOWER_YELLOW = 1;
        private const int DO_TOWER_GREEN = 2;

        // TM 제어
        private const int DO_TM_CYLINDER_EXTEND = 12;
        private const int DO_TM_CYLINDER_RETRACT = 13;
        private const int DO_TM_SUCTION = 14;
        private const int DO_TM_EXHAUST = 15;

        // PM1 제어
        private const int DO_PM1_LAMP = 3;
        private const int DO_PM1_DOOR_UP = 4;
        private const int DO_PM1_DOOR_DOWN = 5;

        // PM2 제어
        private const int DO_PM2_LAMP = 6;
        private const int DO_PM2_DOOR_UP = 7;
        private const int DO_PM2_DOOR_DOWN = 8;

        // PM3 제어
        private const int DO_PM3_LAMP = 9;
        private const int DO_PM3_DOOR_UP = 10;
        private const int DO_PM3_DOOR_DOWN = 11;

        #endregion

        #region 고정 대기시간 설정 (ms)

        // 축 이동 대기시간
        private const int UD_MOVE_DELAY = 1500;     // 상하 축 이동 대기
        private const int LR_MOVE_DELAY = 1200;     // 좌우 축 이동 대기

        // 원점복귀 대기시간
        private const int UD_HOMING_DELAY = 5000;   // 상하 축 원점복귀 대기
        private const int LR_HOMING_DELAY = 6500;   // 좌우 축 원점복귀 대기

        #endregion

        public bool IsConnected => etherCAT != null;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="etherCATInstance">IEG3268 인스턴스 (MainForm에서 생성)</param>
        public RealEtherCATController(IEG3268 etherCATInstance)
        {
            this.etherCAT = etherCATInstance ?? throw new ArgumentNullException(nameof(etherCATInstance));
        }

        #region 축 제어

        public async Task<bool> MoveUDAxis(long position)
        {
            if (etherCAT == null) return false;

            etherCAT.Axis1_UD_POS_Update(position);
            etherCAT.Axis1_UD_Move_Send();

            // 고정 대기시간 사용
            await Task.Delay(UD_MOVE_DELAY);
            return true;
        }

        public async Task<bool> MoveLRAxis(long position)
        {
            if (etherCAT == null) return false;

            etherCAT.Axis2_LR_POS_Update(position);
            etherCAT.Axis2_LR_Move_Send();

            // 고정 대기시간 사용
            await Task.Delay(LR_MOVE_DELAY);
            return true;
        }

        public async Task<bool> HomeUDAxis()
        {
            if (etherCAT == null) return false;

            etherCAT.Axis1_UD_Homming();

            // 고정 대기시간 사용
            await Task.Delay(UD_HOMING_DELAY);
            return true;
        }

        public async Task<bool> HomeLRAxis()
        {
            if (etherCAT == null) return false;

            etherCAT.Axis2_LR_Homming();

            // 고정 대기시간 사용
            await Task.Delay(LR_HOMING_DELAY);
            return true;
        }

        #endregion

        #region 서보 제어

        public void SetServoUD(bool on)
        {
            if (etherCAT == null) return;

            if (on)
                etherCAT.Axis1_ON();
            else
                etherCAT.Axis1_OFF();
        }

        public void SetServoLR(bool on)
        {
            if (etherCAT == null) return;

            if (on)
                etherCAT.Axis2_ON();
            else
                etherCAT.Axis2_OFF();
        }

        #endregion

        #region TM 실린더 제어

        public void ExtendCylinder()
        {
            if (etherCAT == null) return;

            // 상호배타적 제어: 후진 OFF 후 전진 ON
            etherCAT.Digital_Output(DO_TM_CYLINDER_RETRACT, false);
            etherCAT.Digital_Output(DO_TM_CYLINDER_EXTEND, true);
        }

        public void RetractCylinder()
        {
            if (etherCAT == null) return;

            // 상호배타적 제어: 전진 OFF 후 후진 ON
            etherCAT.Digital_Output(DO_TM_CYLINDER_EXTEND, false);
            etherCAT.Digital_Output(DO_TM_CYLINDER_RETRACT, true);
        }

        #endregion

        #region 웨이퍼 흡착 제어

        public void EnableSuction()
        {
            if (etherCAT == null) return;

            // 흡착 ON 전에 배기 OFF
            etherCAT.Digital_Output(DO_TM_EXHAUST, false);
            etherCAT.Digital_Output(DO_TM_SUCTION, true);
        }

        public void DisableSuction()
        {
            if (etherCAT == null) return;

            etherCAT.Digital_Output(DO_TM_SUCTION, false);
        }

        public void EnableExhaust()
        {
            if (etherCAT == null) return;

            // 배기 ON 전에 흡착 OFF
            etherCAT.Digital_Output(DO_TM_SUCTION, false);
            etherCAT.Digital_Output(DO_TM_EXHAUST, true);
        }

        public void DisableExhaust()
        {
            if (etherCAT == null) return;

            etherCAT.Digital_Output(DO_TM_EXHAUST, false);
        }

        #endregion

        #region PM 제어

        public void OpenPMDoor(ProcessModule.ModuleType pm)
        {
            if (etherCAT == null) return;

            // 이미 열려있으면 명령 무시
            if (IsPMDoorOpen(pm)) return;

            var (upChannel, downChannel, _) = GetPMChannels(pm);

            // 상호배타적 제어: DOWN이 열기 신호
            // UP OFF 후 DOWN ON
            etherCAT.Digital_Output(upChannel, false);
            etherCAT.Digital_Output(downChannel, true);

            // 상태 업데이트
            SetPMDoorState(pm, true);
        }

        public void ClosePMDoor(ProcessModule.ModuleType pm)
        {
            if (etherCAT == null) return;

            // 이미 닫혀있으면 명령 무시
            if (!IsPMDoorOpen(pm)) return;

            var (upChannel, downChannel, _) = GetPMChannels(pm);

            // 상호배타적 제어: UP이 닫기 신호
            // DOWN OFF 후 UP ON
            etherCAT.Digital_Output(downChannel, false);
            etherCAT.Digital_Output(upChannel, true);

            // 상태 업데이트
            SetPMDoorState(pm, false);
        }

        /// <summary>
        /// PM 문을 강제로 닫기 (초기화 시 사용)
        /// 상태와 관계없이 무조건 닫기 신호 전송
        /// </summary>
        public void ForceClosePMDoor(ProcessModule.ModuleType pm)
        {
            if (etherCAT == null) return;

            var (upChannel, downChannel, _) = GetPMChannels(pm);

            // 상호배타적 제어: UP이 닫기 신호
            // DOWN OFF 후 UP ON
            etherCAT.Digital_Output(downChannel, false);
            etherCAT.Digital_Output(upChannel, true);

            // 상태 업데이트
            SetPMDoorState(pm, false);
        }

        /// <summary>
        /// PM 문 상태 확인
        /// </summary>
        private bool IsPMDoorOpen(ProcessModule.ModuleType pm)
        {
            switch (pm)
            {
                case ProcessModule.ModuleType.PM1:
                    return isPM1DoorOpen;
                case ProcessModule.ModuleType.PM2:
                    return isPM2DoorOpen;
                case ProcessModule.ModuleType.PM3:
                    return isPM3DoorOpen;
                default:
                    return false;
            }
        }

        /// <summary>
        /// PM 문 상태 설정
        /// </summary>
        private void SetPMDoorState(ProcessModule.ModuleType pm, bool isOpen)
        {
            switch (pm)
            {
                case ProcessModule.ModuleType.PM1:
                    isPM1DoorOpen = isOpen;
                    break;
                case ProcessModule.ModuleType.PM2:
                    isPM2DoorOpen = isOpen;
                    break;
                case ProcessModule.ModuleType.PM3:
                    isPM3DoorOpen = isOpen;
                    break;
            }
        }

        public void SetPMLamp(ProcessModule.ModuleType pm, bool on)
        {
            if (etherCAT == null) return;

            var (_, _, lampChannel) = GetPMChannels(pm);
            etherCAT.Digital_Output(lampChannel, on);
        }

        /// <summary>
        /// PM 타입에 따른 DO 채널 반환
        /// </summary>
        private (int upChannel, int downChannel, int lampChannel) GetPMChannels(ProcessModule.ModuleType pm)
        {
            switch (pm)
            {
                case ProcessModule.ModuleType.PM1:
                    return (DO_PM1_DOOR_UP, DO_PM1_DOOR_DOWN, DO_PM1_LAMP);
                case ProcessModule.ModuleType.PM2:
                    return (DO_PM2_DOOR_UP, DO_PM2_DOOR_DOWN, DO_PM2_LAMP);
                case ProcessModule.ModuleType.PM3:
                    return (DO_PM3_DOOR_UP, DO_PM3_DOOR_DOWN, DO_PM3_LAMP);
                default:
                    return (0, 0, 0);
            }
        }

        #endregion

        #region 타워 램프 제어

        /// <summary>
        /// 타워 램프 제어 (한 번에 하나만 점등)
        /// </summary>
        public void SetTowerLamp(TowerLampState state)
        {
            if (etherCAT == null) return;

            // 모든 램프 상태 설정 (해당 상태의 램프만 ON, 나머지는 OFF)
            etherCAT.Digital_Output(DO_TOWER_RED, state == TowerLampState.Red);
            etherCAT.Digital_Output(DO_TOWER_YELLOW, state == TowerLampState.Yellow);
            etherCAT.Digital_Output(DO_TOWER_GREEN, state == TowerLampState.Green);
        }

        /// <summary>
        /// 타워 램프 개별 제어 (각 색상 독립적으로 ON/OFF 가능)
        /// </summary>
        public void SetTowerLampIndividual(bool red, bool yellow, bool green)
        {
            if (etherCAT == null) return;

            etherCAT.Digital_Output(DO_TOWER_RED, red);
            etherCAT.Digital_Output(DO_TOWER_YELLOW, yellow);
            etherCAT.Digital_Output(DO_TOWER_GREEN, green);
        }

        #endregion
    }
}
