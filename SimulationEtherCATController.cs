using System;
using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 시뮬레이션용 EtherCAT 컨트롤러
    /// 실제 하드웨어 없이 동작 테스트 가능
    /// </summary>
    public class SimulationEtherCATController : IEtherCATController
    {
        private long currentUDPosition = 0;
        private long currentLRPosition = 0;

        // 시뮬레이션 속도 (position units per 50ms)
        private const long SIMULATION_SPEED = 50000;

        // 시뮬레이션 상태 추적 (디버깅/UI 표시용)
        public bool IsCylinderExtended { get; private set; } = false;
        public bool IsSuctionOn { get; private set; } = false;
        public bool IsExhaustOn { get; private set; } = false;

        public bool IsConnected => true;

        #region 축 제어

        public async Task<bool> MoveUDAxis(long position)
        {
            while (Math.Abs(currentUDPosition - position) > 100)
            {
                long step = Math.Min(SIMULATION_SPEED, Math.Abs(position - currentUDPosition));
                currentUDPosition += Math.Sign(position - currentUDPosition) * step;
                await Task.Delay(50);
            }
            currentUDPosition = position;
            return true;
        }

        public async Task<bool> MoveLRAxis(long position)
        {
            while (Math.Abs(currentLRPosition - position) > 100)
            {
                long step = Math.Min(SIMULATION_SPEED, Math.Abs(position - currentLRPosition));
                currentLRPosition += Math.Sign(position - currentLRPosition) * step;
                await Task.Delay(50);
            }
            currentLRPosition = position;
            return true;
        }

        public async Task<bool> HomeUDAxis()
        {
            // 원점복귀 시뮬레이션 - 0으로 이동
            await MoveUDAxis(0);
            return true;
        }

        public async Task<bool> HomeLRAxis()
        {
            // 원점복귀 시뮬레이션 - 0으로 이동
            await MoveLRAxis(0);
            return true;
        }

        public long GetUDPosition() => currentUDPosition;
        public long GetLRPosition() => currentLRPosition;

        #endregion

        #region 서보 제어

        public void SetServoUD(bool on)
        {
            // 시뮬레이션에서는 no-op
        }

        public void SetServoLR(bool on)
        {
            // 시뮬레이션에서는 no-op
        }

        #endregion

        #region TM 실린더 제어

        public void ExtendCylinder()
        {
            IsCylinderExtended = true;
        }

        public void RetractCylinder()
        {
            IsCylinderExtended = false;
        }

        #endregion

        #region 웨이퍼 흡착 제어

        public void EnableSuction()
        {
            IsExhaustOn = false;
            IsSuctionOn = true;
        }

        public void DisableSuction()
        {
            IsSuctionOn = false;
        }

        public void EnableExhaust()
        {
            IsSuctionOn = false;
            IsExhaustOn = true;
        }

        public void DisableExhaust()
        {
            IsExhaustOn = false;
        }

        #endregion

        #region PM 제어

        public void OpenPMDoor(ProcessModule.ModuleType pm)
        {
            // 시뮬레이션에서는 no-op (UI 상태는 ProcessModule에서 관리)
        }

        public void ClosePMDoor(ProcessModule.ModuleType pm)
        {
            // 시뮬레이션에서는 no-op
        }

        public void SetPMLamp(ProcessModule.ModuleType pm, bool on)
        {
            // 시뮬레이션에서는 no-op (UI 상태는 MainView에서 관리)
        }

        #endregion

        #region 타워 램프 제어

        public void SetTowerLamp(TowerLampState state)
        {
            // 시뮬레이션에서는 no-op (UI 상태는 MainView에서 관리)
        }

        #endregion
    }
}
