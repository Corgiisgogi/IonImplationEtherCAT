namespace IonImplationEtherCAT
{
    partial class ManualControlForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpAxisControl = new System.Windows.Forms.GroupBox();
            this.lblUD = new System.Windows.Forms.Label();
            this.lblLR = new System.Windows.Forms.Label();
            this.numUD = new System.Windows.Forms.NumericUpDown();
            this.numLR = new System.Windows.Forms.NumericUpDown();
            this.btnUDMinus10000 = new System.Windows.Forms.Button();
            this.btnUDMinus1000 = new System.Windows.Forms.Button();
            this.btnUDMinus100 = new System.Windows.Forms.Button();
            this.btnUDPlus100 = new System.Windows.Forms.Button();
            this.btnUDPlus1000 = new System.Windows.Forms.Button();
            this.btnUDPlus10000 = new System.Windows.Forms.Button();
            this.btnUDMove = new System.Windows.Forms.Button();
            this.btnLRMinus10000 = new System.Windows.Forms.Button();
            this.btnLRMinus1000 = new System.Windows.Forms.Button();
            this.btnLRMinus100 = new System.Windows.Forms.Button();
            this.btnLRPlus100 = new System.Windows.Forms.Button();
            this.btnLRPlus1000 = new System.Windows.Forms.Button();
            this.btnLRPlus10000 = new System.Windows.Forms.Button();
            this.btnLRMove = new System.Windows.Forms.Button();
            this.btnHomeUD = new System.Windows.Forms.Button();
            this.btnHomeLR = new System.Windows.Forms.Button();
            this.btnHomeAll = new System.Windows.Forms.Button();
            this.btnServoUDOn = new System.Windows.Forms.Button();
            this.btnServoUDOff = new System.Windows.Forms.Button();
            this.btnServoLROn = new System.Windows.Forms.Button();
            this.btnServoLROff = new System.Windows.Forms.Button();
            this.grpCylinderVacuum = new System.Windows.Forms.GroupBox();
            this.lblCylinder = new System.Windows.Forms.Label();
            this.lblSuction = new System.Windows.Forms.Label();
            this.lblExhaust = new System.Windows.Forms.Label();
            this.btnCylinderExtend = new System.Windows.Forms.Button();
            this.btnCylinderRetract = new System.Windows.Forms.Button();
            this.btnSuctionOn = new System.Windows.Forms.Button();
            this.btnSuctionOff = new System.Windows.Forms.Button();
            this.btnExhaustOn = new System.Windows.Forms.Button();
            this.btnExhaustOff = new System.Windows.Forms.Button();
            this.grpPMControl = new System.Windows.Forms.GroupBox();
            this.lblPM1 = new System.Windows.Forms.Label();
            this.lblPM2 = new System.Windows.Forms.Label();
            this.lblPM3 = new System.Windows.Forms.Label();
            this.lblPM1Door = new System.Windows.Forms.Label();
            this.lblPM2Door = new System.Windows.Forms.Label();
            this.lblPM3Door = new System.Windows.Forms.Label();
            this.lblPM1Lamp = new System.Windows.Forms.Label();
            this.lblPM2Lamp = new System.Windows.Forms.Label();
            this.lblPM3Lamp = new System.Windows.Forms.Label();
            this.btnPM1DoorOpen = new System.Windows.Forms.Button();
            this.btnPM1DoorClose = new System.Windows.Forms.Button();
            this.btnPM1LampOn = new System.Windows.Forms.Button();
            this.btnPM1LampOff = new System.Windows.Forms.Button();
            this.btnPM2DoorOpen = new System.Windows.Forms.Button();
            this.btnPM2DoorClose = new System.Windows.Forms.Button();
            this.btnPM2LampOn = new System.Windows.Forms.Button();
            this.btnPM2LampOff = new System.Windows.Forms.Button();
            this.btnPM3DoorOpen = new System.Windows.Forms.Button();
            this.btnPM3DoorClose = new System.Windows.Forms.Button();
            this.btnPM3LampOn = new System.Windows.Forms.Button();
            this.btnPM3LampOff = new System.Windows.Forms.Button();
            this.grpTowerLamp = new System.Windows.Forms.GroupBox();
            this.btnTowerRedOn = new System.Windows.Forms.Button();
            this.btnTowerRedOff = new System.Windows.Forms.Button();
            this.btnTowerYellowOn = new System.Windows.Forms.Button();
            this.btnTowerYellowOff = new System.Windows.Forms.Button();
            this.btnTowerGreenOn = new System.Windows.Forms.Button();
            this.btnTowerGreenOff = new System.Windows.Forms.Button();
            this.btnTowerAllOff = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.grpAxisControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLR)).BeginInit();
            this.grpCylinderVacuum.SuspendLayout();
            this.grpPMControl.SuspendLayout();
            this.grpTowerLamp.SuspendLayout();
            this.SuspendLayout();
            //
            // grpAxisControl
            //
            this.grpAxisControl.Controls.Add(this.btnServoLROff);
            this.grpAxisControl.Controls.Add(this.btnServoLROn);
            this.grpAxisControl.Controls.Add(this.btnServoUDOff);
            this.grpAxisControl.Controls.Add(this.btnServoUDOn);
            this.grpAxisControl.Controls.Add(this.btnHomeAll);
            this.grpAxisControl.Controls.Add(this.btnHomeLR);
            this.grpAxisControl.Controls.Add(this.btnHomeUD);
            this.grpAxisControl.Controls.Add(this.btnLRMove);
            this.grpAxisControl.Controls.Add(this.btnLRPlus10000);
            this.grpAxisControl.Controls.Add(this.btnLRPlus1000);
            this.grpAxisControl.Controls.Add(this.btnLRPlus100);
            this.grpAxisControl.Controls.Add(this.btnLRMinus100);
            this.grpAxisControl.Controls.Add(this.btnLRMinus1000);
            this.grpAxisControl.Controls.Add(this.btnLRMinus10000);
            this.grpAxisControl.Controls.Add(this.numLR);
            this.grpAxisControl.Controls.Add(this.lblLR);
            this.grpAxisControl.Controls.Add(this.btnUDMove);
            this.grpAxisControl.Controls.Add(this.btnUDPlus10000);
            this.grpAxisControl.Controls.Add(this.btnUDPlus1000);
            this.grpAxisControl.Controls.Add(this.btnUDPlus100);
            this.grpAxisControl.Controls.Add(this.btnUDMinus100);
            this.grpAxisControl.Controls.Add(this.btnUDMinus1000);
            this.grpAxisControl.Controls.Add(this.btnUDMinus10000);
            this.grpAxisControl.Controls.Add(this.numUD);
            this.grpAxisControl.Controls.Add(this.lblUD);
            this.grpAxisControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.grpAxisControl.Location = new System.Drawing.Point(10, 10);
            this.grpAxisControl.Name = "grpAxisControl";
            this.grpAxisControl.Size = new System.Drawing.Size(710, 200);
            this.grpAxisControl.TabIndex = 0;
            this.grpAxisControl.TabStop = false;
            this.grpAxisControl.Text = "축 제어 (Axis Control)";
            //
            // lblUD
            //
            this.lblUD.AutoSize = true;
            this.lblUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblUD.Location = new System.Drawing.Point(15, 28);
            this.lblUD.Name = "lblUD";
            this.lblUD.Size = new System.Drawing.Size(80, 15);
            this.lblUD.TabIndex = 0;
            this.lblUD.Text = "상하 축 (UD):";
            //
            // lblLR
            //
            this.lblLR.AutoSize = true;
            this.lblLR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblLR.Location = new System.Drawing.Point(15, 63);
            this.lblLR.Name = "lblLR";
            this.lblLR.Size = new System.Drawing.Size(79, 15);
            this.lblLR.TabIndex = 1;
            this.lblLR.Text = "좌우 축 (LR):";
            //
            // numUD
            //
            this.numUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.numUD.Location = new System.Drawing.Point(110, 25);
            this.numUD.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.numUD.Minimum = new decimal(new int[] { 100000000, 0, 0, -2147483648 });
            this.numUD.Name = "numUD";
            this.numUD.Size = new System.Drawing.Size(120, 21);
            this.numUD.TabIndex = 2;
            //
            // numLR
            //
            this.numLR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.numLR.Location = new System.Drawing.Point(110, 60);
            this.numLR.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.numLR.Minimum = new decimal(new int[] { 100000000, 0, 0, -2147483648 });
            this.numLR.Name = "numLR";
            this.numLR.Size = new System.Drawing.Size(120, 21);
            this.numLR.TabIndex = 3;
            //
            // btnUDMinus10000
            //
            this.btnUDMinus10000.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUDMinus10000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnUDMinus10000.Location = new System.Drawing.Point(240, 24);
            this.btnUDMinus10000.Name = "btnUDMinus10000";
            this.btnUDMinus10000.Size = new System.Drawing.Size(50, 25);
            this.btnUDMinus10000.TabIndex = 4;
            this.btnUDMinus10000.Text = "-10000";
            this.btnUDMinus10000.UseVisualStyleBackColor = true;
            this.btnUDMinus10000.Click += new System.EventHandler(this.btnUDMinus10000_Click);
            //
            // btnUDMinus1000
            //
            this.btnUDMinus1000.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUDMinus1000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnUDMinus1000.Location = new System.Drawing.Point(295, 24);
            this.btnUDMinus1000.Name = "btnUDMinus1000";
            this.btnUDMinus1000.Size = new System.Drawing.Size(50, 25);
            this.btnUDMinus1000.TabIndex = 5;
            this.btnUDMinus1000.Text = "-1000";
            this.btnUDMinus1000.UseVisualStyleBackColor = true;
            this.btnUDMinus1000.Click += new System.EventHandler(this.btnUDMinus1000_Click);
            //
            // btnUDMinus100
            //
            this.btnUDMinus100.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUDMinus100.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnUDMinus100.Location = new System.Drawing.Point(350, 24);
            this.btnUDMinus100.Name = "btnUDMinus100";
            this.btnUDMinus100.Size = new System.Drawing.Size(50, 25);
            this.btnUDMinus100.TabIndex = 6;
            this.btnUDMinus100.Text = "-100";
            this.btnUDMinus100.UseVisualStyleBackColor = true;
            this.btnUDMinus100.Click += new System.EventHandler(this.btnUDMinus100_Click);
            //
            // btnUDPlus100
            //
            this.btnUDPlus100.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUDPlus100.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnUDPlus100.Location = new System.Drawing.Point(405, 24);
            this.btnUDPlus100.Name = "btnUDPlus100";
            this.btnUDPlus100.Size = new System.Drawing.Size(50, 25);
            this.btnUDPlus100.TabIndex = 7;
            this.btnUDPlus100.Text = "+100";
            this.btnUDPlus100.UseVisualStyleBackColor = true;
            this.btnUDPlus100.Click += new System.EventHandler(this.btnUDPlus100_Click);
            //
            // btnUDPlus1000
            //
            this.btnUDPlus1000.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUDPlus1000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnUDPlus1000.Location = new System.Drawing.Point(460, 24);
            this.btnUDPlus1000.Name = "btnUDPlus1000";
            this.btnUDPlus1000.Size = new System.Drawing.Size(50, 25);
            this.btnUDPlus1000.TabIndex = 8;
            this.btnUDPlus1000.Text = "+1000";
            this.btnUDPlus1000.UseVisualStyleBackColor = true;
            this.btnUDPlus1000.Click += new System.EventHandler(this.btnUDPlus1000_Click);
            //
            // btnUDPlus10000
            //
            this.btnUDPlus10000.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUDPlus10000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnUDPlus10000.Location = new System.Drawing.Point(515, 24);
            this.btnUDPlus10000.Name = "btnUDPlus10000";
            this.btnUDPlus10000.Size = new System.Drawing.Size(50, 25);
            this.btnUDPlus10000.TabIndex = 9;
            this.btnUDPlus10000.Text = "+10000";
            this.btnUDPlus10000.UseVisualStyleBackColor = true;
            this.btnUDPlus10000.Click += new System.EventHandler(this.btnUDPlus10000_Click);
            //
            // btnUDMove
            //
            this.btnUDMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUDMove.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnUDMove.Location = new System.Drawing.Point(585, 22);
            this.btnUDMove.Name = "btnUDMove";
            this.btnUDMove.Size = new System.Drawing.Size(60, 28);
            this.btnUDMove.TabIndex = 10;
            this.btnUDMove.Text = "이동";
            this.btnUDMove.UseVisualStyleBackColor = true;
            this.btnUDMove.Click += new System.EventHandler(this.btnUDMove_Click);
            //
            // btnLRMinus10000
            //
            this.btnLRMinus10000.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLRMinus10000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnLRMinus10000.Location = new System.Drawing.Point(240, 59);
            this.btnLRMinus10000.Name = "btnLRMinus10000";
            this.btnLRMinus10000.Size = new System.Drawing.Size(50, 25);
            this.btnLRMinus10000.TabIndex = 11;
            this.btnLRMinus10000.Text = "-10000";
            this.btnLRMinus10000.UseVisualStyleBackColor = true;
            this.btnLRMinus10000.Click += new System.EventHandler(this.btnLRMinus10000_Click);
            //
            // btnLRMinus1000
            //
            this.btnLRMinus1000.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLRMinus1000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnLRMinus1000.Location = new System.Drawing.Point(295, 59);
            this.btnLRMinus1000.Name = "btnLRMinus1000";
            this.btnLRMinus1000.Size = new System.Drawing.Size(50, 25);
            this.btnLRMinus1000.TabIndex = 12;
            this.btnLRMinus1000.Text = "-1000";
            this.btnLRMinus1000.UseVisualStyleBackColor = true;
            this.btnLRMinus1000.Click += new System.EventHandler(this.btnLRMinus1000_Click);
            //
            // btnLRMinus100
            //
            this.btnLRMinus100.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLRMinus100.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnLRMinus100.Location = new System.Drawing.Point(350, 59);
            this.btnLRMinus100.Name = "btnLRMinus100";
            this.btnLRMinus100.Size = new System.Drawing.Size(50, 25);
            this.btnLRMinus100.TabIndex = 13;
            this.btnLRMinus100.Text = "-100";
            this.btnLRMinus100.UseVisualStyleBackColor = true;
            this.btnLRMinus100.Click += new System.EventHandler(this.btnLRMinus100_Click);
            //
            // btnLRPlus100
            //
            this.btnLRPlus100.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLRPlus100.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnLRPlus100.Location = new System.Drawing.Point(405, 59);
            this.btnLRPlus100.Name = "btnLRPlus100";
            this.btnLRPlus100.Size = new System.Drawing.Size(50, 25);
            this.btnLRPlus100.TabIndex = 14;
            this.btnLRPlus100.Text = "+100";
            this.btnLRPlus100.UseVisualStyleBackColor = true;
            this.btnLRPlus100.Click += new System.EventHandler(this.btnLRPlus100_Click);
            //
            // btnLRPlus1000
            //
            this.btnLRPlus1000.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLRPlus1000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnLRPlus1000.Location = new System.Drawing.Point(460, 59);
            this.btnLRPlus1000.Name = "btnLRPlus1000";
            this.btnLRPlus1000.Size = new System.Drawing.Size(50, 25);
            this.btnLRPlus1000.TabIndex = 15;
            this.btnLRPlus1000.Text = "+1000";
            this.btnLRPlus1000.UseVisualStyleBackColor = true;
            this.btnLRPlus1000.Click += new System.EventHandler(this.btnLRPlus1000_Click);
            //
            // btnLRPlus10000
            //
            this.btnLRPlus10000.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLRPlus10000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.btnLRPlus10000.Location = new System.Drawing.Point(515, 59);
            this.btnLRPlus10000.Name = "btnLRPlus10000";
            this.btnLRPlus10000.Size = new System.Drawing.Size(50, 25);
            this.btnLRPlus10000.TabIndex = 16;
            this.btnLRPlus10000.Text = "+10000";
            this.btnLRPlus10000.UseVisualStyleBackColor = true;
            this.btnLRPlus10000.Click += new System.EventHandler(this.btnLRPlus10000_Click);
            //
            // btnLRMove
            //
            this.btnLRMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLRMove.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLRMove.Location = new System.Drawing.Point(585, 57);
            this.btnLRMove.Name = "btnLRMove";
            this.btnLRMove.Size = new System.Drawing.Size(60, 28);
            this.btnLRMove.TabIndex = 17;
            this.btnLRMove.Text = "이동";
            this.btnLRMove.UseVisualStyleBackColor = true;
            this.btnLRMove.Click += new System.EventHandler(this.btnLRMove_Click);
            //
            // btnHomeUD
            //
            this.btnHomeUD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHomeUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnHomeUD.Location = new System.Drawing.Point(15, 100);
            this.btnHomeUD.Name = "btnHomeUD";
            this.btnHomeUD.Size = new System.Drawing.Size(110, 30);
            this.btnHomeUD.TabIndex = 18;
            this.btnHomeUD.Text = "UD 원점복귀";
            this.btnHomeUD.UseVisualStyleBackColor = true;
            this.btnHomeUD.Click += new System.EventHandler(this.btnHomeUD_Click);
            //
            // btnHomeLR
            //
            this.btnHomeLR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHomeLR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnHomeLR.Location = new System.Drawing.Point(135, 100);
            this.btnHomeLR.Name = "btnHomeLR";
            this.btnHomeLR.Size = new System.Drawing.Size(110, 30);
            this.btnHomeLR.TabIndex = 19;
            this.btnHomeLR.Text = "LR 원점복귀";
            this.btnHomeLR.UseVisualStyleBackColor = true;
            this.btnHomeLR.Click += new System.EventHandler(this.btnHomeLR_Click);
            //
            // btnHomeAll
            //
            this.btnHomeAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHomeAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnHomeAll.Location = new System.Drawing.Point(255, 100);
            this.btnHomeAll.Name = "btnHomeAll";
            this.btnHomeAll.Size = new System.Drawing.Size(110, 30);
            this.btnHomeAll.TabIndex = 20;
            this.btnHomeAll.Text = "전체 원점복귀";
            this.btnHomeAll.UseVisualStyleBackColor = true;
            this.btnHomeAll.Click += new System.EventHandler(this.btnHomeAll_Click);
            //
            // btnServoUDOn
            //
            this.btnServoUDOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServoUDOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnServoUDOn.Location = new System.Drawing.Point(15, 145);
            this.btnServoUDOn.Name = "btnServoUDOn";
            this.btnServoUDOn.Size = new System.Drawing.Size(100, 30);
            this.btnServoUDOn.TabIndex = 21;
            this.btnServoUDOn.Text = "UD 서보 ON";
            this.btnServoUDOn.UseVisualStyleBackColor = true;
            this.btnServoUDOn.Click += new System.EventHandler(this.btnServoUDOn_Click);
            //
            // btnServoUDOff
            //
            this.btnServoUDOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServoUDOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnServoUDOff.Location = new System.Drawing.Point(125, 145);
            this.btnServoUDOff.Name = "btnServoUDOff";
            this.btnServoUDOff.Size = new System.Drawing.Size(100, 30);
            this.btnServoUDOff.TabIndex = 22;
            this.btnServoUDOff.Text = "UD 서보 OFF";
            this.btnServoUDOff.UseVisualStyleBackColor = true;
            this.btnServoUDOff.Click += new System.EventHandler(this.btnServoUDOff_Click);
            //
            // btnServoLROn
            //
            this.btnServoLROn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServoLROn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnServoLROn.Location = new System.Drawing.Point(235, 145);
            this.btnServoLROn.Name = "btnServoLROn";
            this.btnServoLROn.Size = new System.Drawing.Size(100, 30);
            this.btnServoLROn.TabIndex = 23;
            this.btnServoLROn.Text = "LR 서보 ON";
            this.btnServoLROn.UseVisualStyleBackColor = true;
            this.btnServoLROn.Click += new System.EventHandler(this.btnServoLROn_Click);
            //
            // btnServoLROff
            //
            this.btnServoLROff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServoLROff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnServoLROff.Location = new System.Drawing.Point(345, 145);
            this.btnServoLROff.Name = "btnServoLROff";
            this.btnServoLROff.Size = new System.Drawing.Size(100, 30);
            this.btnServoLROff.TabIndex = 24;
            this.btnServoLROff.Text = "LR 서보 OFF";
            this.btnServoLROff.UseVisualStyleBackColor = true;
            this.btnServoLROff.Click += new System.EventHandler(this.btnServoLROff_Click);
            //
            // grpCylinderVacuum
            //
            this.grpCylinderVacuum.Controls.Add(this.btnExhaustOff);
            this.grpCylinderVacuum.Controls.Add(this.btnExhaustOn);
            this.grpCylinderVacuum.Controls.Add(this.lblExhaust);
            this.grpCylinderVacuum.Controls.Add(this.btnSuctionOff);
            this.grpCylinderVacuum.Controls.Add(this.btnSuctionOn);
            this.grpCylinderVacuum.Controls.Add(this.lblSuction);
            this.grpCylinderVacuum.Controls.Add(this.btnCylinderRetract);
            this.grpCylinderVacuum.Controls.Add(this.btnCylinderExtend);
            this.grpCylinderVacuum.Controls.Add(this.lblCylinder);
            this.grpCylinderVacuum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.grpCylinderVacuum.Location = new System.Drawing.Point(10, 220);
            this.grpCylinderVacuum.Name = "grpCylinderVacuum";
            this.grpCylinderVacuum.Size = new System.Drawing.Size(710, 80);
            this.grpCylinderVacuum.TabIndex = 1;
            this.grpCylinderVacuum.TabStop = false;
            this.grpCylinderVacuum.Text = "실린더/진공 제어 (Cylinder/Vacuum)";
            //
            // lblCylinder
            //
            this.lblCylinder.AutoSize = true;
            this.lblCylinder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblCylinder.Location = new System.Drawing.Point(15, 35);
            this.lblCylinder.Name = "lblCylinder";
            this.lblCylinder.Size = new System.Drawing.Size(49, 15);
            this.lblCylinder.TabIndex = 0;
            this.lblCylinder.Text = "실린더:";
            //
            // lblSuction
            //
            this.lblSuction.AutoSize = true;
            this.lblSuction.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblSuction.Location = new System.Drawing.Point(260, 35);
            this.lblSuction.Name = "lblSuction";
            this.lblSuction.Size = new System.Drawing.Size(37, 15);
            this.lblSuction.TabIndex = 1;
            this.lblSuction.Text = "흡착:";
            //
            // lblExhaust
            //
            this.lblExhaust.AutoSize = true;
            this.lblExhaust.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblExhaust.Location = new System.Drawing.Point(470, 35);
            this.lblExhaust.Name = "lblExhaust";
            this.lblExhaust.Size = new System.Drawing.Size(37, 15);
            this.lblExhaust.TabIndex = 2;
            this.lblExhaust.Text = "배기:";
            //
            // btnCylinderExtend
            //
            this.btnCylinderExtend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCylinderExtend.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCylinderExtend.Location = new System.Drawing.Point(80, 30);
            this.btnCylinderExtend.Name = "btnCylinderExtend";
            this.btnCylinderExtend.Size = new System.Drawing.Size(70, 30);
            this.btnCylinderExtend.TabIndex = 3;
            this.btnCylinderExtend.Text = "전진";
            this.btnCylinderExtend.UseVisualStyleBackColor = true;
            this.btnCylinderExtend.Click += new System.EventHandler(this.btnCylinderExtend_Click);
            //
            // btnCylinderRetract
            //
            this.btnCylinderRetract.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCylinderRetract.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCylinderRetract.Location = new System.Drawing.Point(160, 30);
            this.btnCylinderRetract.Name = "btnCylinderRetract";
            this.btnCylinderRetract.Size = new System.Drawing.Size(70, 30);
            this.btnCylinderRetract.TabIndex = 4;
            this.btnCylinderRetract.Text = "후진";
            this.btnCylinderRetract.UseVisualStyleBackColor = true;
            this.btnCylinderRetract.Click += new System.EventHandler(this.btnCylinderRetract_Click);
            //
            // btnSuctionOn
            //
            this.btnSuctionOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSuctionOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSuctionOn.Location = new System.Drawing.Point(310, 30);
            this.btnSuctionOn.Name = "btnSuctionOn";
            this.btnSuctionOn.Size = new System.Drawing.Size(60, 30);
            this.btnSuctionOn.TabIndex = 5;
            this.btnSuctionOn.Text = "ON";
            this.btnSuctionOn.UseVisualStyleBackColor = true;
            this.btnSuctionOn.Click += new System.EventHandler(this.btnSuctionOn_Click);
            //
            // btnSuctionOff
            //
            this.btnSuctionOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSuctionOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSuctionOff.Location = new System.Drawing.Point(380, 30);
            this.btnSuctionOff.Name = "btnSuctionOff";
            this.btnSuctionOff.Size = new System.Drawing.Size(60, 30);
            this.btnSuctionOff.TabIndex = 6;
            this.btnSuctionOff.Text = "OFF";
            this.btnSuctionOff.UseVisualStyleBackColor = true;
            this.btnSuctionOff.Click += new System.EventHandler(this.btnSuctionOff_Click);
            //
            // btnExhaustOn
            //
            this.btnExhaustOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExhaustOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnExhaustOn.Location = new System.Drawing.Point(520, 30);
            this.btnExhaustOn.Name = "btnExhaustOn";
            this.btnExhaustOn.Size = new System.Drawing.Size(60, 30);
            this.btnExhaustOn.TabIndex = 7;
            this.btnExhaustOn.Text = "ON";
            this.btnExhaustOn.UseVisualStyleBackColor = true;
            this.btnExhaustOn.Click += new System.EventHandler(this.btnExhaustOn_Click);
            //
            // btnExhaustOff
            //
            this.btnExhaustOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExhaustOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnExhaustOff.Location = new System.Drawing.Point(590, 30);
            this.btnExhaustOff.Name = "btnExhaustOff";
            this.btnExhaustOff.Size = new System.Drawing.Size(60, 30);
            this.btnExhaustOff.TabIndex = 8;
            this.btnExhaustOff.Text = "OFF";
            this.btnExhaustOff.UseVisualStyleBackColor = true;
            this.btnExhaustOff.Click += new System.EventHandler(this.btnExhaustOff_Click);
            //
            // grpPMControl
            //
            this.grpPMControl.Controls.Add(this.btnPM3LampOff);
            this.grpPMControl.Controls.Add(this.btnPM3LampOn);
            this.grpPMControl.Controls.Add(this.btnPM3DoorClose);
            this.grpPMControl.Controls.Add(this.btnPM3DoorOpen);
            this.grpPMControl.Controls.Add(this.lblPM3Lamp);
            this.grpPMControl.Controls.Add(this.lblPM3Door);
            this.grpPMControl.Controls.Add(this.lblPM3);
            this.grpPMControl.Controls.Add(this.btnPM2LampOff);
            this.grpPMControl.Controls.Add(this.btnPM2LampOn);
            this.grpPMControl.Controls.Add(this.btnPM2DoorClose);
            this.grpPMControl.Controls.Add(this.btnPM2DoorOpen);
            this.grpPMControl.Controls.Add(this.lblPM2Lamp);
            this.grpPMControl.Controls.Add(this.lblPM2Door);
            this.grpPMControl.Controls.Add(this.lblPM2);
            this.grpPMControl.Controls.Add(this.btnPM1LampOff);
            this.grpPMControl.Controls.Add(this.btnPM1LampOn);
            this.grpPMControl.Controls.Add(this.btnPM1DoorClose);
            this.grpPMControl.Controls.Add(this.btnPM1DoorOpen);
            this.grpPMControl.Controls.Add(this.lblPM1Lamp);
            this.grpPMControl.Controls.Add(this.lblPM1Door);
            this.grpPMControl.Controls.Add(this.lblPM1);
            this.grpPMControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.grpPMControl.Location = new System.Drawing.Point(10, 310);
            this.grpPMControl.Name = "grpPMControl";
            this.grpPMControl.Size = new System.Drawing.Size(710, 120);
            this.grpPMControl.TabIndex = 2;
            this.grpPMControl.TabStop = false;
            this.grpPMControl.Text = "PM 제어 (Process Module)";
            //
            // lblPM1
            //
            this.lblPM1.AutoSize = true;
            this.lblPM1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblPM1.Location = new System.Drawing.Point(60, 25);
            this.lblPM1.Name = "lblPM1";
            this.lblPM1.Size = new System.Drawing.Size(39, 17);
            this.lblPM1.TabIndex = 0;
            this.lblPM1.Text = "PM1";
            //
            // lblPM2
            //
            this.lblPM2.AutoSize = true;
            this.lblPM2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblPM2.Location = new System.Drawing.Point(295, 25);
            this.lblPM2.Name = "lblPM2";
            this.lblPM2.Size = new System.Drawing.Size(39, 17);
            this.lblPM2.TabIndex = 1;
            this.lblPM2.Text = "PM2";
            //
            // lblPM3
            //
            this.lblPM3.AutoSize = true;
            this.lblPM3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblPM3.Location = new System.Drawing.Point(530, 25);
            this.lblPM3.Name = "lblPM3";
            this.lblPM3.Size = new System.Drawing.Size(39, 17);
            this.lblPM3.TabIndex = 2;
            this.lblPM3.Text = "PM3";
            //
            // lblPM1Door
            //
            this.lblPM1Door.AutoSize = true;
            this.lblPM1Door.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblPM1Door.Location = new System.Drawing.Point(15, 50);
            this.lblPM1Door.Name = "lblPM1Door";
            this.lblPM1Door.Size = new System.Drawing.Size(37, 15);
            this.lblPM1Door.TabIndex = 3;
            this.lblPM1Door.Text = "Door:";
            //
            // lblPM2Door
            //
            this.lblPM2Door.AutoSize = true;
            this.lblPM2Door.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblPM2Door.Location = new System.Drawing.Point(250, 50);
            this.lblPM2Door.Name = "lblPM2Door";
            this.lblPM2Door.Size = new System.Drawing.Size(37, 15);
            this.lblPM2Door.TabIndex = 4;
            this.lblPM2Door.Text = "Door:";
            //
            // lblPM3Door
            //
            this.lblPM3Door.AutoSize = true;
            this.lblPM3Door.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblPM3Door.Location = new System.Drawing.Point(485, 50);
            this.lblPM3Door.Name = "lblPM3Door";
            this.lblPM3Door.Size = new System.Drawing.Size(37, 15);
            this.lblPM3Door.TabIndex = 5;
            this.lblPM3Door.Text = "Door:";
            //
            // lblPM1Lamp
            //
            this.lblPM1Lamp.AutoSize = true;
            this.lblPM1Lamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblPM1Lamp.Location = new System.Drawing.Point(15, 85);
            this.lblPM1Lamp.Name = "lblPM1Lamp";
            this.lblPM1Lamp.Size = new System.Drawing.Size(42, 15);
            this.lblPM1Lamp.TabIndex = 6;
            this.lblPM1Lamp.Text = "Lamp:";
            //
            // lblPM2Lamp
            //
            this.lblPM2Lamp.AutoSize = true;
            this.lblPM2Lamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblPM2Lamp.Location = new System.Drawing.Point(250, 85);
            this.lblPM2Lamp.Name = "lblPM2Lamp";
            this.lblPM2Lamp.Size = new System.Drawing.Size(42, 15);
            this.lblPM2Lamp.TabIndex = 7;
            this.lblPM2Lamp.Text = "Lamp:";
            //
            // lblPM3Lamp
            //
            this.lblPM3Lamp.AutoSize = true;
            this.lblPM3Lamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblPM3Lamp.Location = new System.Drawing.Point(485, 85);
            this.lblPM3Lamp.Name = "lblPM3Lamp";
            this.lblPM3Lamp.Size = new System.Drawing.Size(42, 15);
            this.lblPM3Lamp.TabIndex = 8;
            this.lblPM3Lamp.Text = "Lamp:";
            //
            // btnPM1DoorOpen
            //
            this.btnPM1DoorOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM1DoorOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM1DoorOpen.Location = new System.Drawing.Point(60, 45);
            this.btnPM1DoorOpen.Name = "btnPM1DoorOpen";
            this.btnPM1DoorOpen.Size = new System.Drawing.Size(55, 28);
            this.btnPM1DoorOpen.TabIndex = 9;
            this.btnPM1DoorOpen.Text = "열기";
            this.btnPM1DoorOpen.UseVisualStyleBackColor = true;
            this.btnPM1DoorOpen.Click += new System.EventHandler(this.btnPM1DoorOpen_Click);
            //
            // btnPM1DoorClose
            //
            this.btnPM1DoorClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM1DoorClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM1DoorClose.Location = new System.Drawing.Point(120, 45);
            this.btnPM1DoorClose.Name = "btnPM1DoorClose";
            this.btnPM1DoorClose.Size = new System.Drawing.Size(55, 28);
            this.btnPM1DoorClose.TabIndex = 10;
            this.btnPM1DoorClose.Text = "닫기";
            this.btnPM1DoorClose.UseVisualStyleBackColor = true;
            this.btnPM1DoorClose.Click += new System.EventHandler(this.btnPM1DoorClose_Click);
            //
            // btnPM1LampOn
            //
            this.btnPM1LampOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM1LampOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM1LampOn.Location = new System.Drawing.Point(60, 80);
            this.btnPM1LampOn.Name = "btnPM1LampOn";
            this.btnPM1LampOn.Size = new System.Drawing.Size(50, 28);
            this.btnPM1LampOn.TabIndex = 11;
            this.btnPM1LampOn.Text = "ON";
            this.btnPM1LampOn.UseVisualStyleBackColor = true;
            this.btnPM1LampOn.Click += new System.EventHandler(this.btnPM1LampOn_Click);
            //
            // btnPM1LampOff
            //
            this.btnPM1LampOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM1LampOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM1LampOff.Location = new System.Drawing.Point(115, 80);
            this.btnPM1LampOff.Name = "btnPM1LampOff";
            this.btnPM1LampOff.Size = new System.Drawing.Size(50, 28);
            this.btnPM1LampOff.TabIndex = 12;
            this.btnPM1LampOff.Text = "OFF";
            this.btnPM1LampOff.UseVisualStyleBackColor = true;
            this.btnPM1LampOff.Click += new System.EventHandler(this.btnPM1LampOff_Click);
            //
            // btnPM2DoorOpen
            //
            this.btnPM2DoorOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM2DoorOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM2DoorOpen.Location = new System.Drawing.Point(295, 45);
            this.btnPM2DoorOpen.Name = "btnPM2DoorOpen";
            this.btnPM2DoorOpen.Size = new System.Drawing.Size(55, 28);
            this.btnPM2DoorOpen.TabIndex = 13;
            this.btnPM2DoorOpen.Text = "열기";
            this.btnPM2DoorOpen.UseVisualStyleBackColor = true;
            this.btnPM2DoorOpen.Click += new System.EventHandler(this.btnPM2DoorOpen_Click);
            //
            // btnPM2DoorClose
            //
            this.btnPM2DoorClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM2DoorClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM2DoorClose.Location = new System.Drawing.Point(355, 45);
            this.btnPM2DoorClose.Name = "btnPM2DoorClose";
            this.btnPM2DoorClose.Size = new System.Drawing.Size(55, 28);
            this.btnPM2DoorClose.TabIndex = 14;
            this.btnPM2DoorClose.Text = "닫기";
            this.btnPM2DoorClose.UseVisualStyleBackColor = true;
            this.btnPM2DoorClose.Click += new System.EventHandler(this.btnPM2DoorClose_Click);
            //
            // btnPM2LampOn
            //
            this.btnPM2LampOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM2LampOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM2LampOn.Location = new System.Drawing.Point(295, 80);
            this.btnPM2LampOn.Name = "btnPM2LampOn";
            this.btnPM2LampOn.Size = new System.Drawing.Size(50, 28);
            this.btnPM2LampOn.TabIndex = 15;
            this.btnPM2LampOn.Text = "ON";
            this.btnPM2LampOn.UseVisualStyleBackColor = true;
            this.btnPM2LampOn.Click += new System.EventHandler(this.btnPM2LampOn_Click);
            //
            // btnPM2LampOff
            //
            this.btnPM2LampOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM2LampOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM2LampOff.Location = new System.Drawing.Point(350, 80);
            this.btnPM2LampOff.Name = "btnPM2LampOff";
            this.btnPM2LampOff.Size = new System.Drawing.Size(50, 28);
            this.btnPM2LampOff.TabIndex = 16;
            this.btnPM2LampOff.Text = "OFF";
            this.btnPM2LampOff.UseVisualStyleBackColor = true;
            this.btnPM2LampOff.Click += new System.EventHandler(this.btnPM2LampOff_Click);
            //
            // btnPM3DoorOpen
            //
            this.btnPM3DoorOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM3DoorOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM3DoorOpen.Location = new System.Drawing.Point(530, 45);
            this.btnPM3DoorOpen.Name = "btnPM3DoorOpen";
            this.btnPM3DoorOpen.Size = new System.Drawing.Size(55, 28);
            this.btnPM3DoorOpen.TabIndex = 17;
            this.btnPM3DoorOpen.Text = "열기";
            this.btnPM3DoorOpen.UseVisualStyleBackColor = true;
            this.btnPM3DoorOpen.Click += new System.EventHandler(this.btnPM3DoorOpen_Click);
            //
            // btnPM3DoorClose
            //
            this.btnPM3DoorClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM3DoorClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM3DoorClose.Location = new System.Drawing.Point(590, 45);
            this.btnPM3DoorClose.Name = "btnPM3DoorClose";
            this.btnPM3DoorClose.Size = new System.Drawing.Size(55, 28);
            this.btnPM3DoorClose.TabIndex = 18;
            this.btnPM3DoorClose.Text = "닫기";
            this.btnPM3DoorClose.UseVisualStyleBackColor = true;
            this.btnPM3DoorClose.Click += new System.EventHandler(this.btnPM3DoorClose_Click);
            //
            // btnPM3LampOn
            //
            this.btnPM3LampOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM3LampOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM3LampOn.Location = new System.Drawing.Point(530, 80);
            this.btnPM3LampOn.Name = "btnPM3LampOn";
            this.btnPM3LampOn.Size = new System.Drawing.Size(50, 28);
            this.btnPM3LampOn.TabIndex = 19;
            this.btnPM3LampOn.Text = "ON";
            this.btnPM3LampOn.UseVisualStyleBackColor = true;
            this.btnPM3LampOn.Click += new System.EventHandler(this.btnPM3LampOn_Click);
            //
            // btnPM3LampOff
            //
            this.btnPM3LampOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPM3LampOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPM3LampOff.Location = new System.Drawing.Point(585, 80);
            this.btnPM3LampOff.Name = "btnPM3LampOff";
            this.btnPM3LampOff.Size = new System.Drawing.Size(50, 28);
            this.btnPM3LampOff.TabIndex = 20;
            this.btnPM3LampOff.Text = "OFF";
            this.btnPM3LampOff.UseVisualStyleBackColor = true;
            this.btnPM3LampOff.Click += new System.EventHandler(this.btnPM3LampOff_Click);
            //
            // grpTowerLamp
            //
            this.grpTowerLamp.Controls.Add(this.btnTowerAllOff);
            this.grpTowerLamp.Controls.Add(this.btnTowerGreenOff);
            this.grpTowerLamp.Controls.Add(this.btnTowerGreenOn);
            this.grpTowerLamp.Controls.Add(this.btnTowerYellowOff);
            this.grpTowerLamp.Controls.Add(this.btnTowerYellowOn);
            this.grpTowerLamp.Controls.Add(this.btnTowerRedOff);
            this.grpTowerLamp.Controls.Add(this.btnTowerRedOn);
            this.grpTowerLamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.grpTowerLamp.Location = new System.Drawing.Point(10, 440);
            this.grpTowerLamp.Name = "grpTowerLamp";
            this.grpTowerLamp.Size = new System.Drawing.Size(710, 80);
            this.grpTowerLamp.TabIndex = 3;
            this.grpTowerLamp.TabStop = false;
            this.grpTowerLamp.Text = "타워 램프 (Tower Lamp) - 개별 토글";
            //
            // btnTowerRedOn
            //
            this.btnTowerRedOn.BackColor = System.Drawing.Color.LightCoral;
            this.btnTowerRedOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTowerRedOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTowerRedOn.Location = new System.Drawing.Point(15, 30);
            this.btnTowerRedOn.Name = "btnTowerRedOn";
            this.btnTowerRedOn.Size = new System.Drawing.Size(70, 30);
            this.btnTowerRedOn.TabIndex = 0;
            this.btnTowerRedOn.Text = "Red ON";
            this.btnTowerRedOn.UseVisualStyleBackColor = false;
            this.btnTowerRedOn.Click += new System.EventHandler(this.btnTowerRedOn_Click);
            //
            // btnTowerRedOff
            //
            this.btnTowerRedOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTowerRedOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTowerRedOff.Location = new System.Drawing.Point(90, 30);
            this.btnTowerRedOff.Name = "btnTowerRedOff";
            this.btnTowerRedOff.Size = new System.Drawing.Size(70, 30);
            this.btnTowerRedOff.TabIndex = 1;
            this.btnTowerRedOff.Text = "Red OFF";
            this.btnTowerRedOff.UseVisualStyleBackColor = true;
            this.btnTowerRedOff.Click += new System.EventHandler(this.btnTowerRedOff_Click);
            //
            // btnTowerYellowOn
            //
            this.btnTowerYellowOn.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.btnTowerYellowOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTowerYellowOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTowerYellowOn.Location = new System.Drawing.Point(180, 30);
            this.btnTowerYellowOn.Name = "btnTowerYellowOn";
            this.btnTowerYellowOn.Size = new System.Drawing.Size(80, 30);
            this.btnTowerYellowOn.TabIndex = 2;
            this.btnTowerYellowOn.Text = "Yellow ON";
            this.btnTowerYellowOn.UseVisualStyleBackColor = false;
            this.btnTowerYellowOn.Click += new System.EventHandler(this.btnTowerYellowOn_Click);
            //
            // btnTowerYellowOff
            //
            this.btnTowerYellowOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTowerYellowOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTowerYellowOff.Location = new System.Drawing.Point(265, 30);
            this.btnTowerYellowOff.Name = "btnTowerYellowOff";
            this.btnTowerYellowOff.Size = new System.Drawing.Size(80, 30);
            this.btnTowerYellowOff.TabIndex = 3;
            this.btnTowerYellowOff.Text = "Yellow OFF";
            this.btnTowerYellowOff.UseVisualStyleBackColor = true;
            this.btnTowerYellowOff.Click += new System.EventHandler(this.btnTowerYellowOff_Click);
            //
            // btnTowerGreenOn
            //
            this.btnTowerGreenOn.BackColor = System.Drawing.Color.LightGreen;
            this.btnTowerGreenOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTowerGreenOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTowerGreenOn.Location = new System.Drawing.Point(365, 30);
            this.btnTowerGreenOn.Name = "btnTowerGreenOn";
            this.btnTowerGreenOn.Size = new System.Drawing.Size(80, 30);
            this.btnTowerGreenOn.TabIndex = 4;
            this.btnTowerGreenOn.Text = "Green ON";
            this.btnTowerGreenOn.UseVisualStyleBackColor = false;
            this.btnTowerGreenOn.Click += new System.EventHandler(this.btnTowerGreenOn_Click);
            //
            // btnTowerGreenOff
            //
            this.btnTowerGreenOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTowerGreenOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTowerGreenOff.Location = new System.Drawing.Point(450, 30);
            this.btnTowerGreenOff.Name = "btnTowerGreenOff";
            this.btnTowerGreenOff.Size = new System.Drawing.Size(80, 30);
            this.btnTowerGreenOff.TabIndex = 5;
            this.btnTowerGreenOff.Text = "Green OFF";
            this.btnTowerGreenOff.UseVisualStyleBackColor = true;
            this.btnTowerGreenOff.Click += new System.EventHandler(this.btnTowerGreenOff_Click);
            //
            // btnTowerAllOff
            //
            this.btnTowerAllOff.BackColor = System.Drawing.Color.LightGray;
            this.btnTowerAllOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTowerAllOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTowerAllOff.Location = new System.Drawing.Point(560, 30);
            this.btnTowerAllOff.Name = "btnTowerAllOff";
            this.btnTowerAllOff.Size = new System.Drawing.Size(90, 30);
            this.btnTowerAllOff.TabIndex = 6;
            this.btnTowerAllOff.Text = "전체 OFF";
            this.btnTowerAllOff.UseVisualStyleBackColor = false;
            this.btnTowerAllOff.Click += new System.EventHandler(this.btnTowerAllOff_Click);
            //
            // btnClose
            //
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.Location = new System.Drawing.Point(310, 530);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 35);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // ManualControlForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 581);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.grpTowerLamp);
            this.Controls.Add(this.grpPMControl);
            this.Controls.Add(this.grpCylinderVacuum);
            this.Controls.Add(this.grpAxisControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ManualControlForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "수동 제어 (Manual Control)";
            this.grpAxisControl.ResumeLayout(false);
            this.grpAxisControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLR)).EndInit();
            this.grpCylinderVacuum.ResumeLayout(false);
            this.grpCylinderVacuum.PerformLayout();
            this.grpPMControl.ResumeLayout(false);
            this.grpPMControl.PerformLayout();
            this.grpTowerLamp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpAxisControl;
        private System.Windows.Forms.Label lblUD;
        private System.Windows.Forms.Label lblLR;
        private System.Windows.Forms.NumericUpDown numUD;
        private System.Windows.Forms.NumericUpDown numLR;
        private System.Windows.Forms.Button btnUDMinus10000;
        private System.Windows.Forms.Button btnUDMinus1000;
        private System.Windows.Forms.Button btnUDMinus100;
        private System.Windows.Forms.Button btnUDPlus100;
        private System.Windows.Forms.Button btnUDPlus1000;
        private System.Windows.Forms.Button btnUDPlus10000;
        private System.Windows.Forms.Button btnUDMove;
        private System.Windows.Forms.Button btnLRMinus10000;
        private System.Windows.Forms.Button btnLRMinus1000;
        private System.Windows.Forms.Button btnLRMinus100;
        private System.Windows.Forms.Button btnLRPlus100;
        private System.Windows.Forms.Button btnLRPlus1000;
        private System.Windows.Forms.Button btnLRPlus10000;
        private System.Windows.Forms.Button btnLRMove;
        private System.Windows.Forms.Button btnHomeUD;
        private System.Windows.Forms.Button btnHomeLR;
        private System.Windows.Forms.Button btnHomeAll;
        private System.Windows.Forms.Button btnServoUDOn;
        private System.Windows.Forms.Button btnServoUDOff;
        private System.Windows.Forms.Button btnServoLROn;
        private System.Windows.Forms.Button btnServoLROff;
        private System.Windows.Forms.GroupBox grpCylinderVacuum;
        private System.Windows.Forms.Label lblCylinder;
        private System.Windows.Forms.Label lblSuction;
        private System.Windows.Forms.Label lblExhaust;
        private System.Windows.Forms.Button btnCylinderExtend;
        private System.Windows.Forms.Button btnCylinderRetract;
        private System.Windows.Forms.Button btnSuctionOn;
        private System.Windows.Forms.Button btnSuctionOff;
        private System.Windows.Forms.Button btnExhaustOn;
        private System.Windows.Forms.Button btnExhaustOff;
        private System.Windows.Forms.GroupBox grpPMControl;
        private System.Windows.Forms.Label lblPM1;
        private System.Windows.Forms.Label lblPM2;
        private System.Windows.Forms.Label lblPM3;
        private System.Windows.Forms.Label lblPM1Door;
        private System.Windows.Forms.Label lblPM2Door;
        private System.Windows.Forms.Label lblPM3Door;
        private System.Windows.Forms.Label lblPM1Lamp;
        private System.Windows.Forms.Label lblPM2Lamp;
        private System.Windows.Forms.Label lblPM3Lamp;
        private System.Windows.Forms.Button btnPM1DoorOpen;
        private System.Windows.Forms.Button btnPM1DoorClose;
        private System.Windows.Forms.Button btnPM1LampOn;
        private System.Windows.Forms.Button btnPM1LampOff;
        private System.Windows.Forms.Button btnPM2DoorOpen;
        private System.Windows.Forms.Button btnPM2DoorClose;
        private System.Windows.Forms.Button btnPM2LampOn;
        private System.Windows.Forms.Button btnPM2LampOff;
        private System.Windows.Forms.Button btnPM3DoorOpen;
        private System.Windows.Forms.Button btnPM3DoorClose;
        private System.Windows.Forms.Button btnPM3LampOn;
        private System.Windows.Forms.Button btnPM3LampOff;
        private System.Windows.Forms.GroupBox grpTowerLamp;
        private System.Windows.Forms.Button btnTowerRedOn;
        private System.Windows.Forms.Button btnTowerRedOff;
        private System.Windows.Forms.Button btnTowerYellowOn;
        private System.Windows.Forms.Button btnTowerYellowOff;
        private System.Windows.Forms.Button btnTowerGreenOn;
        private System.Windows.Forms.Button btnTowerGreenOff;
        private System.Windows.Forms.Button btnTowerAllOff;
        private System.Windows.Forms.Button btnClose;
    }
}
