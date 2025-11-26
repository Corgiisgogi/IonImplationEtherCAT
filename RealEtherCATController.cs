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

        #region DO 채널 상수

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

        #region 타임아웃 설정

        // 축 이동 타임아웃 (ms)
        private const int AXIS_MOVE_TIMEOUT = 30000;

        // 원점복귀 타임아웃 (ms)
        private const int HOMING_TIMEOUT = 60000;

        // 위치 허용 오차
        private const int POSITION_TOLERANCE = 1000;

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

            return await WaitForPosition(GetUDPosition, position, POSITION_TOLERANCE, AXIS_MOVE_TIMEOUT);
        }

        public async Task<bool> MoveLRAxis(long position)
        {
            if (etherCAT == null) return false;

            etherCAT.Axis2_LR_POS_Update(position);
            etherCAT.Axis2_LR_Move_Send();

            return await WaitForPosition(GetLRPosition, position, POSITION_TOLERANCE, AXIS_MOVE_TIMEOUT);
        }

        public async Task<bool> HomeUDAxis()
        {
            if (etherCAT == null) return false;

            etherCAT.Axis1_UD_Homming();

            // 원점복귀는 별도 메서드이므로 완료 대기
            // 원점 위치는 기계에 따라 약간 오차가 있을 수 있으므로 넉넉한 허용치 사용
            return await WaitForHoming(GetUDPosition, 5000, HOMING_TIMEOUT);
        }

        public async Task<bool> HomeLRAxis()
        {
            if (etherCAT == null) return false;

            etherCAT.Axis2_LR_Homming();

            return await WaitForHoming(GetLRPosition, 5000, HOMING_TIMEOUT);
        }

        public long GetUDPosition()
        {
            if (etherCAT == null) return 0;

            string posStr = etherCAT.Axis1_is_PosData();
            if (long.TryParse(posStr, out long pos))
                return pos;
            return 0;
        }

        public long GetLRPosition()
        {
            if (etherCAT == null) return 0;

            string posStr = etherCAT.Axis2_is_PosData();
            if (long.TryParse(posStr, out long pos))
                return pos;
            return 0;
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

            var (upChannel, downChannel, _) = GetPMChannels(pm);

            // 상호배타적 제어: 하강 OFF 후 상승 ON
            etherCAT.Digital_Output(downChannel, false);
            etherCAT.Digital_Output(upChannel, true);
        }

        public void ClosePMDoor(ProcessModule.ModuleType pm)
        {
            if (etherCAT == null) return;

            var (upChannel, downChannel, _) = GetPMChannels(pm);

            // 상호배타적 제어: 상승 OFF 후 하강 ON
            etherCAT.Digital_Output(upChannel, false);
            etherCAT.Digital_Output(downChannel, true);
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

        #region 위치 대기 헬퍼 메서드

        /// <summary>
        /// 지정된 위치에 도달할 때까지 대기
        /// </summary>
        private async Task<bool> WaitForPosition(Func<long> getPosition, long target, int tolerance, int timeoutMs)
        {
            int elapsed = 0;
            while (elapsed < timeoutMs)
            {
                long current = getPosition();
                if (Math.Abs(current - target) <= tolerance)
                    return true;

                await Task.Delay(50);
                elapsed += 50;
            }
            return false; // 타임아웃
        }

        /// <summary>
        /// 원점복귀 완료 대기 (원점 근처에 도달할 때까지)
        /// </summary>
        private async Task<bool> WaitForHoming(Func<long> getPosition, int tolerance, int timeoutMs)
        {
            int elapsed = 0;
            long lastPosition = getPosition();
            int stableCount = 0;

            while (elapsed < timeoutMs)
            {
                await Task.Delay(100);
                elapsed += 100;

                long current = getPosition();

                // 원점 근처에서 위치가 안정되면 완료로 판단
                if (Math.Abs(current) <= tolerance)
                {
                    if (Math.Abs(current - lastPosition) < 100)
                    {
                        stableCount++;
                        if (stableCount >= 5) // 500ms 동안 안정
                            return true;
                    }
                    else
                    {
                        stableCount = 0;
                    }
                }

                lastPosition = current;
            }
            return false; // 타임아웃
        }

        #endregion
    }
}
