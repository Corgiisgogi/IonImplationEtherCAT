using System;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 수동 제어 폼 - 티칭값 조절용
    /// Non-Modal 방식으로 MainView와 동시 조작 가능
    /// </summary>
    public partial class ManualControlForm : Form
    {
        private readonly IEtherCATController controller;

        // 타워 램프 상태 추적
        private bool towerRedOn = false;
        private bool towerYellowOn = false;
        private bool towerGreenOn = false;

        public ManualControlForm(IEtherCATController etherCATController)
        {
            InitializeComponent();
            this.controller = etherCATController;
        }

        #region Event Handlers - UD Axis Increment/Decrement

        private void btnUDMinus10000_Click(object sender, EventArgs e)
        {
            numUD.Value -= 10000;
        }

        private void btnUDMinus1000_Click(object sender, EventArgs e)
        {
            numUD.Value -= 1000;
        }

        private void btnUDMinus100_Click(object sender, EventArgs e)
        {
            numUD.Value -= 100;
        }

        private void btnUDPlus100_Click(object sender, EventArgs e)
        {
            numUD.Value += 100;
        }

        private void btnUDPlus1000_Click(object sender, EventArgs e)
        {
            numUD.Value += 1000;
        }

        private void btnUDPlus10000_Click(object sender, EventArgs e)
        {
            numUD.Value += 10000;
        }

        #endregion

        #region Event Handlers - LR Axis Increment/Decrement

        private void btnLRMinus10000_Click(object sender, EventArgs e)
        {
            numLR.Value -= 10000;
        }

        private void btnLRMinus1000_Click(object sender, EventArgs e)
        {
            numLR.Value -= 1000;
        }

        private void btnLRMinus100_Click(object sender, EventArgs e)
        {
            numLR.Value -= 100;
        }

        private void btnLRPlus100_Click(object sender, EventArgs e)
        {
            numLR.Value += 100;
        }

        private void btnLRPlus1000_Click(object sender, EventArgs e)
        {
            numLR.Value += 1000;
        }

        private void btnLRPlus10000_Click(object sender, EventArgs e)
        {
            numLR.Value += 10000;
        }

        #endregion

        #region Event Handlers - Axis Move

        private async void btnUDMove_Click(object sender, EventArgs e)
        {
            if (controller == null) return;
            btnUDMove.Enabled = false;
            try
            {
                long targetPos = (long)numUD.Value;
                LogManager.Instance.AddLog("수동제어", $"UD축 이동 → {targetPos}", "TM", LogCategory.Hardware, false);
                await controller.MoveUDAxis(targetPos);
            }
            finally
            {
                btnUDMove.Enabled = true;
            }
        }

        private async void btnLRMove_Click(object sender, EventArgs e)
        {
            if (controller == null) return;
            btnLRMove.Enabled = false;
            try
            {
                long targetPos = (long)numLR.Value;
                LogManager.Instance.AddLog("수동제어", $"LR축 이동 → {targetPos}", "TM", LogCategory.Hardware, false);
                await controller.MoveLRAxis(targetPos);
            }
            finally
            {
                btnLRMove.Enabled = true;
            }
        }

        #endregion

        #region Event Handlers - Home

        private async void btnHomeUD_Click(object sender, EventArgs e)
        {
            if (controller == null) return;
            btnHomeUD.Enabled = false;
            try
            {
                LogManager.Instance.AddLog("수동제어", "UD축 원점복귀 시작", "TM", LogCategory.Hardware, false);
                await controller.HomeUDAxis();
                numUD.Value = 0;
                LogManager.Instance.AddLog("수동제어", "UD축 원점복귀 완료", "TM", LogCategory.Hardware, false);
            }
            finally
            {
                btnHomeUD.Enabled = true;
            }
        }

        private async void btnHomeLR_Click(object sender, EventArgs e)
        {
            if (controller == null) return;
            btnHomeLR.Enabled = false;
            try
            {
                LogManager.Instance.AddLog("수동제어", "LR축 원점복귀 시작", "TM", LogCategory.Hardware, false);
                await controller.HomeLRAxis();
                numLR.Value = 0;
                LogManager.Instance.AddLog("수동제어", "LR축 원점복귀 완료", "TM", LogCategory.Hardware, false);
            }
            finally
            {
                btnHomeLR.Enabled = true;
            }
        }

        private async void btnHomeAll_Click(object sender, EventArgs e)
        {
            if (controller == null) return;
            btnHomeAll.Enabled = false;
            btnHomeUD.Enabled = false;
            btnHomeLR.Enabled = false;
            try
            {
                LogManager.Instance.AddLog("수동제어", "전체 원점복귀 시작", "TM", LogCategory.Hardware, false);
                await controller.HomeUDAxis();
                await controller.HomeLRAxis();
                numUD.Value = 0;
                numLR.Value = 0;
                LogManager.Instance.AddLog("수동제어", "전체 원점복귀 완료", "TM", LogCategory.Hardware, false);
            }
            finally
            {
                btnHomeAll.Enabled = true;
                btnHomeUD.Enabled = true;
                btnHomeLR.Enabled = true;
            }
        }

        #endregion

        #region Event Handlers - Servo

        private void btnServoUDOn_Click(object sender, EventArgs e)
        {
            controller?.SetServoUD(true);
            LogManager.Instance.AddLog("수동제어", "UD축 서보 ON", "TM", LogCategory.Hardware, false);
        }

        private void btnServoUDOff_Click(object sender, EventArgs e)
        {
            controller?.SetServoUD(false);
            LogManager.Instance.AddLog("수동제어", "UD축 서보 OFF", "TM", LogCategory.Hardware, false);
        }

        private void btnServoLROn_Click(object sender, EventArgs e)
        {
            controller?.SetServoLR(true);
            LogManager.Instance.AddLog("수동제어", "LR축 서보 ON", "TM", LogCategory.Hardware, false);
        }

        private void btnServoLROff_Click(object sender, EventArgs e)
        {
            controller?.SetServoLR(false);
            LogManager.Instance.AddLog("수동제어", "LR축 서보 OFF", "TM", LogCategory.Hardware, false);
        }

        #endregion

        #region Event Handlers - Cylinder/Vacuum

        private void btnCylinderExtend_Click(object sender, EventArgs e)
        {
            controller?.ExtendCylinder();
            LogManager.Instance.AddLog("수동제어", "실린더 전진", "TM", LogCategory.Hardware, false);
        }

        private void btnCylinderRetract_Click(object sender, EventArgs e)
        {
            controller?.RetractCylinder();
            LogManager.Instance.AddLog("수동제어", "실린더 후진", "TM", LogCategory.Hardware, false);
        }

        private void btnSuctionOn_Click(object sender, EventArgs e)
        {
            controller?.EnableSuction();
            LogManager.Instance.AddLog("수동제어", "흡착 ON", "TM", LogCategory.Hardware, false);
        }

        private void btnSuctionOff_Click(object sender, EventArgs e)
        {
            controller?.DisableSuction();
            LogManager.Instance.AddLog("수동제어", "흡착 OFF", "TM", LogCategory.Hardware, false);
        }

        private void btnExhaustOn_Click(object sender, EventArgs e)
        {
            controller?.EnableExhaust();
            LogManager.Instance.AddLog("수동제어", "배기 ON", "TM", LogCategory.Hardware, false);
        }

        private void btnExhaustOff_Click(object sender, EventArgs e)
        {
            controller?.DisableExhaust();
            LogManager.Instance.AddLog("수동제어", "배기 OFF", "TM", LogCategory.Hardware, false);
        }

        #endregion

        #region Event Handlers - PM Door

        private void btnPM1DoorOpen_Click(object sender, EventArgs e)
        {
            controller?.OpenPMDoor(ProcessModule.ModuleType.PM1);
            LogManager.Instance.AddLog("수동제어", "PM1 문 열기", "PM1", LogCategory.Hardware, false);
        }

        private void btnPM1DoorClose_Click(object sender, EventArgs e)
        {
            controller?.ClosePMDoor(ProcessModule.ModuleType.PM1);
            LogManager.Instance.AddLog("수동제어", "PM1 문 닫기", "PM1", LogCategory.Hardware, false);
        }

        private void btnPM2DoorOpen_Click(object sender, EventArgs e)
        {
            controller?.OpenPMDoor(ProcessModule.ModuleType.PM2);
            LogManager.Instance.AddLog("수동제어", "PM2 문 열기", "PM2", LogCategory.Hardware, false);
        }

        private void btnPM2DoorClose_Click(object sender, EventArgs e)
        {
            controller?.ClosePMDoor(ProcessModule.ModuleType.PM2);
            LogManager.Instance.AddLog("수동제어", "PM2 문 닫기", "PM2", LogCategory.Hardware, false);
        }

        private void btnPM3DoorOpen_Click(object sender, EventArgs e)
        {
            controller?.OpenPMDoor(ProcessModule.ModuleType.PM3);
            LogManager.Instance.AddLog("수동제어", "PM3 문 열기", "PM3", LogCategory.Hardware, false);
        }

        private void btnPM3DoorClose_Click(object sender, EventArgs e)
        {
            controller?.ClosePMDoor(ProcessModule.ModuleType.PM3);
            LogManager.Instance.AddLog("수동제어", "PM3 문 닫기", "PM3", LogCategory.Hardware, false);
        }

        #endregion

        #region Event Handlers - PM Lamp

        private void btnPM1LampOn_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM1, true);
            LogManager.Instance.AddLog("수동제어", "PM1 램프 ON", "PM1", LogCategory.Hardware, false);
        }

        private void btnPM1LampOff_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM1, false);
            LogManager.Instance.AddLog("수동제어", "PM1 램프 OFF", "PM1", LogCategory.Hardware, false);
        }

        private void btnPM2LampOn_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM2, true);
            LogManager.Instance.AddLog("수동제어", "PM2 램프 ON", "PM2", LogCategory.Hardware, false);
        }

        private void btnPM2LampOff_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM2, false);
            LogManager.Instance.AddLog("수동제어", "PM2 램프 OFF", "PM2", LogCategory.Hardware, false);
        }

        private void btnPM3LampOn_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM3, true);
            LogManager.Instance.AddLog("수동제어", "PM3 램프 ON", "PM3", LogCategory.Hardware, false);
        }

        private void btnPM3LampOff_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM3, false);
            LogManager.Instance.AddLog("수동제어", "PM3 램프 OFF", "PM3", LogCategory.Hardware, false);
        }

        #endregion

        #region Event Handlers - Tower Lamp

        private void btnTowerRedOn_Click(object sender, EventArgs e)
        {
            towerRedOn = true;
            UpdateTowerLamp();
            LogManager.Instance.AddLog("수동제어", "타워램프 적색 ON", "System", LogCategory.Hardware, false);
        }

        private void btnTowerRedOff_Click(object sender, EventArgs e)
        {
            towerRedOn = false;
            UpdateTowerLamp();
            LogManager.Instance.AddLog("수동제어", "타워램프 적색 OFF", "System", LogCategory.Hardware, false);
        }

        private void btnTowerYellowOn_Click(object sender, EventArgs e)
        {
            towerYellowOn = true;
            UpdateTowerLamp();
            LogManager.Instance.AddLog("수동제어", "타워램프 황색 ON", "System", LogCategory.Hardware, false);
        }

        private void btnTowerYellowOff_Click(object sender, EventArgs e)
        {
            towerYellowOn = false;
            UpdateTowerLamp();
            LogManager.Instance.AddLog("수동제어", "타워램프 황색 OFF", "System", LogCategory.Hardware, false);
        }

        private void btnTowerGreenOn_Click(object sender, EventArgs e)
        {
            towerGreenOn = true;
            UpdateTowerLamp();
            LogManager.Instance.AddLog("수동제어", "타워램프 녹색 ON", "System", LogCategory.Hardware, false);
        }

        private void btnTowerGreenOff_Click(object sender, EventArgs e)
        {
            towerGreenOn = false;
            UpdateTowerLamp();
            LogManager.Instance.AddLog("수동제어", "타워램프 녹색 OFF", "System", LogCategory.Hardware, false);
        }

        private void btnTowerAllOff_Click(object sender, EventArgs e)
        {
            towerRedOn = false;
            towerYellowOn = false;
            towerGreenOn = false;
            UpdateTowerLamp();
            LogManager.Instance.AddLog("수동제어", "타워램프 전체 OFF", "System", LogCategory.Hardware, false);
        }

        private void UpdateTowerLamp()
        {
            controller?.SetTowerLampIndividual(towerRedOn, towerYellowOn, towerGreenOn);
        }

        #endregion

        #region Event Handlers - Close

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        /// <summary>
        /// 모든 컨트롤 활성화/비활성화 (자동 공정 중 사용)
        /// </summary>
        public void SetControlsEnabled(bool enabled)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is GroupBox grp)
                {
                    foreach (Control child in grp.Controls)
                    {
                        if (child is Button || child is NumericUpDown)
                        {
                            child.Enabled = enabled;
                        }
                    }
                }
                else if (ctrl is Button btn && btn != btnClose)
                {
                    btn.Enabled = enabled;
                }
            }
        }
    }
}
