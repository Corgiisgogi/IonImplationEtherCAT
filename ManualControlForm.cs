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
                await controller.MoveUDAxis((long)numUD.Value);
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
                await controller.MoveLRAxis((long)numLR.Value);
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
                await controller.HomeUDAxis();
                numUD.Value = 0;
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
                await controller.HomeLRAxis();
                numLR.Value = 0;
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
                await controller.HomeUDAxis();
                await controller.HomeLRAxis();
                numUD.Value = 0;
                numLR.Value = 0;
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
        }

        private void btnServoUDOff_Click(object sender, EventArgs e)
        {
            controller?.SetServoUD(false);
        }

        private void btnServoLROn_Click(object sender, EventArgs e)
        {
            controller?.SetServoLR(true);
        }

        private void btnServoLROff_Click(object sender, EventArgs e)
        {
            controller?.SetServoLR(false);
        }

        #endregion

        #region Event Handlers - Cylinder/Vacuum

        private void btnCylinderExtend_Click(object sender, EventArgs e)
        {
            controller?.ExtendCylinder();
        }

        private void btnCylinderRetract_Click(object sender, EventArgs e)
        {
            controller?.RetractCylinder();
        }

        private void btnSuctionOn_Click(object sender, EventArgs e)
        {
            controller?.EnableSuction();
        }

        private void btnSuctionOff_Click(object sender, EventArgs e)
        {
            controller?.DisableSuction();
        }

        private void btnExhaustOn_Click(object sender, EventArgs e)
        {
            controller?.EnableExhaust();
        }

        private void btnExhaustOff_Click(object sender, EventArgs e)
        {
            controller?.DisableExhaust();
        }

        #endregion

        #region Event Handlers - PM Door

        private void btnPM1DoorOpen_Click(object sender, EventArgs e)
        {
            controller?.OpenPMDoor(ProcessModule.ModuleType.PM1);
        }

        private void btnPM1DoorClose_Click(object sender, EventArgs e)
        {
            controller?.ClosePMDoor(ProcessModule.ModuleType.PM1);
        }

        private void btnPM2DoorOpen_Click(object sender, EventArgs e)
        {
            controller?.OpenPMDoor(ProcessModule.ModuleType.PM2);
        }

        private void btnPM2DoorClose_Click(object sender, EventArgs e)
        {
            controller?.ClosePMDoor(ProcessModule.ModuleType.PM2);
        }

        private void btnPM3DoorOpen_Click(object sender, EventArgs e)
        {
            controller?.OpenPMDoor(ProcessModule.ModuleType.PM3);
        }

        private void btnPM3DoorClose_Click(object sender, EventArgs e)
        {
            controller?.ClosePMDoor(ProcessModule.ModuleType.PM3);
        }

        #endregion

        #region Event Handlers - PM Lamp

        private void btnPM1LampOn_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM1, true);
        }

        private void btnPM1LampOff_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM1, false);
        }

        private void btnPM2LampOn_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM2, true);
        }

        private void btnPM2LampOff_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM2, false);
        }

        private void btnPM3LampOn_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM3, true);
        }

        private void btnPM3LampOff_Click(object sender, EventArgs e)
        {
            controller?.SetPMLamp(ProcessModule.ModuleType.PM3, false);
        }

        #endregion

        #region Event Handlers - Tower Lamp

        private void btnTowerRedOn_Click(object sender, EventArgs e)
        {
            towerRedOn = true;
            UpdateTowerLamp();
        }

        private void btnTowerRedOff_Click(object sender, EventArgs e)
        {
            towerRedOn = false;
            UpdateTowerLamp();
        }

        private void btnTowerYellowOn_Click(object sender, EventArgs e)
        {
            towerYellowOn = true;
            UpdateTowerLamp();
        }

        private void btnTowerYellowOff_Click(object sender, EventArgs e)
        {
            towerYellowOn = false;
            UpdateTowerLamp();
        }

        private void btnTowerGreenOn_Click(object sender, EventArgs e)
        {
            towerGreenOn = true;
            UpdateTowerLamp();
        }

        private void btnTowerGreenOff_Click(object sender, EventArgs e)
        {
            towerGreenOn = false;
            UpdateTowerLamp();
        }

        private void btnTowerAllOff_Click(object sender, EventArgs e)
        {
            towerRedOn = false;
            towerYellowOn = false;
            towerGreenOn = false;
            UpdateTowerLamp();
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
