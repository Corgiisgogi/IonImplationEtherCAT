namespace IonImplationEtherCAT
{
    partial class RecipeView
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRecipePm1 = new System.Windows.Forms.Button();
            this.btnRecipePm2 = new System.Windows.Forms.Button();
            this.btnRecipePm3 = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.panelPm1 = new System.Windows.Forms.Panel();
            this.gBoxScanner = new System.Windows.Forms.GroupBox();
            this.lblScannerVoltage = new System.Windows.Forms.Label();
            this.textBoxScannerVoltage = new System.Windows.Forms.TextBox();
            this.lblMotor = new System.Windows.Forms.Label();
            this.textBoxMotor = new System.Windows.Forms.TextBox();
            this.gBoxAnalyzer = new System.Windows.Forms.GroupBox();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.textBoxCurrent = new System.Windows.Forms.TextBox();
            this.lblVoltage = new System.Windows.Forms.Label();
            this.lblDose = new System.Windows.Forms.Label();
            this.lblIonGas = new System.Windows.Forms.Label();
            this.textBoxVoltage = new System.Windows.Forms.TextBox();
            this.listBoxIonGas = new System.Windows.Forms.ListBox();
            this.textBoxDose = new System.Windows.Forms.TextBox();
            this.panelPm2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblScannerVoltage2 = new System.Windows.Forms.Label();
            this.textBoxScannerVoltage2 = new System.Windows.Forms.TextBox();
            this.lblMotor2 = new System.Windows.Forms.Label();
            this.textBoxMotor2 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblCurrent2 = new System.Windows.Forms.Label();
            this.textBoxCurrent2 = new System.Windows.Forms.TextBox();
            this.lblVoltage2 = new System.Windows.Forms.Label();
            this.lblDose2 = new System.Windows.Forms.Label();
            this.lblIonGas2 = new System.Windows.Forms.Label();
            this.textBoxVoltage2 = new System.Windows.Forms.TextBox();
            this.listBoxIonGas2 = new System.Windows.Forms.ListBox();
            this.textBoxDose2 = new System.Windows.Forms.TextBox();
            this.panelPm3 = new System.Windows.Forms.Panel();
            this.textBoxVaccum = new System.Windows.Forms.TextBox();
            this.textBoxTemperature = new System.Windows.Forms.TextBox();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.lblVaccum = new System.Windows.Forms.Label();
            this.panelPm1.SuspendLayout();
            this.gBoxScanner.SuspendLayout();
            this.gBoxAnalyzer.SuspendLayout();
            this.panelPm2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelPm3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRecipePm1
            // 
            this.btnRecipePm1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecipePm1.Location = new System.Drawing.Point(40, 67);
            this.btnRecipePm1.Name = "btnRecipePm1";
            this.btnRecipePm1.Size = new System.Drawing.Size(188, 105);
            this.btnRecipePm1.TabIndex = 0;
            this.btnRecipePm1.Text = "PM1";
            this.btnRecipePm1.UseVisualStyleBackColor = true;
            this.btnRecipePm1.Click += new System.EventHandler(this.btnRecipePm1_Click);
            // 
            // btnRecipePm2
            // 
            this.btnRecipePm2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecipePm2.Location = new System.Drawing.Point(40, 191);
            this.btnRecipePm2.Name = "btnRecipePm2";
            this.btnRecipePm2.Size = new System.Drawing.Size(188, 105);
            this.btnRecipePm2.TabIndex = 1;
            this.btnRecipePm2.Text = "PM2";
            this.btnRecipePm2.UseVisualStyleBackColor = true;
            this.btnRecipePm2.Click += new System.EventHandler(this.btnRecipePm2_Click);
            // 
            // btnRecipePm3
            // 
            this.btnRecipePm3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecipePm3.Location = new System.Drawing.Point(40, 319);
            this.btnRecipePm3.Name = "btnRecipePm3";
            this.btnRecipePm3.Size = new System.Drawing.Size(188, 105);
            this.btnRecipePm3.TabIndex = 2;
            this.btnRecipePm3.Text = "PM3";
            this.btnRecipePm3.UseVisualStyleBackColor = true;
            this.btnRecipePm3.Click += new System.EventHandler(this.btnRecipePm3_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.Location = new System.Drawing.Point(453, 551);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(152, 64);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLoad.Location = new System.Drawing.Point(651, 551);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(152, 64);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // panelPm1
            // 
            this.panelPm1.Controls.Add(this.gBoxScanner);
            this.panelPm1.Controls.Add(this.gBoxAnalyzer);
            this.panelPm1.Controls.Add(this.lblVoltage);
            this.panelPm1.Controls.Add(this.lblDose);
            this.panelPm1.Controls.Add(this.lblIonGas);
            this.panelPm1.Controls.Add(this.textBoxVoltage);
            this.panelPm1.Controls.Add(this.listBoxIonGas);
            this.panelPm1.Controls.Add(this.textBoxDose);
            this.panelPm1.Location = new System.Drawing.Point(270, 58);
            this.panelPm1.Name = "panelPm1";
            this.panelPm1.Size = new System.Drawing.Size(1022, 449);
            this.panelPm1.TabIndex = 5;
            // 
            // gBoxScanner
            // 
            this.gBoxScanner.Controls.Add(this.lblScannerVoltage);
            this.gBoxScanner.Controls.Add(this.textBoxScannerVoltage);
            this.gBoxScanner.Controls.Add(this.lblMotor);
            this.gBoxScanner.Controls.Add(this.textBoxMotor);
            this.gBoxScanner.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gBoxScanner.Location = new System.Drawing.Point(356, 14);
            this.gBoxScanner.Name = "gBoxScanner";
            this.gBoxScanner.Size = new System.Drawing.Size(337, 148);
            this.gBoxScanner.TabIndex = 9;
            this.gBoxScanner.TabStop = false;
            this.gBoxScanner.Text = "스캐너";
            // 
            // lblScannerVoltage
            // 
            this.lblScannerVoltage.AutoSize = true;
            this.lblScannerVoltage.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblScannerVoltage.Location = new System.Drawing.Point(25, 77);
            this.lblScannerVoltage.Name = "lblScannerVoltage";
            this.lblScannerVoltage.Size = new System.Drawing.Size(44, 25);
            this.lblScannerVoltage.TabIndex = 15;
            this.lblScannerVoltage.Text = "전압";
            // 
            // textBoxScannerVoltage
            // 
            this.textBoxScannerVoltage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxScannerVoltage.Location = new System.Drawing.Point(92, 76);
            this.textBoxScannerVoltage.Name = "textBoxScannerVoltage";
            this.textBoxScannerVoltage.Size = new System.Drawing.Size(188, 29);
            this.textBoxScannerVoltage.TabIndex = 14;
            // 
            // lblMotor
            // 
            this.lblMotor.AutoSize = true;
            this.lblMotor.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMotor.Location = new System.Drawing.Point(25, 33);
            this.lblMotor.Name = "lblMotor";
            this.lblMotor.Size = new System.Drawing.Size(44, 25);
            this.lblMotor.TabIndex = 13;
            this.lblMotor.Text = "모터";
            // 
            // textBoxMotor
            // 
            this.textBoxMotor.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMotor.Location = new System.Drawing.Point(92, 32);
            this.textBoxMotor.Name = "textBoxMotor";
            this.textBoxMotor.Size = new System.Drawing.Size(188, 29);
            this.textBoxMotor.TabIndex = 12;
            // 
            // gBoxAnalyzer
            // 
            this.gBoxAnalyzer.Controls.Add(this.lblCurrent);
            this.gBoxAnalyzer.Controls.Add(this.textBoxCurrent);
            this.gBoxAnalyzer.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gBoxAnalyzer.Location = new System.Drawing.Point(18, 168);
            this.gBoxAnalyzer.Name = "gBoxAnalyzer";
            this.gBoxAnalyzer.Size = new System.Drawing.Size(317, 86);
            this.gBoxAnalyzer.TabIndex = 8;
            this.gBoxAnalyzer.TabStop = false;
            this.gBoxAnalyzer.Text = "분석기";
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrent.Location = new System.Drawing.Point(13, 33);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(82, 25);
            this.lblCurrent.TabIndex = 11;
            this.lblCurrent.Text = "자석 전류";
            // 
            // textBoxCurrent
            // 
            this.textBoxCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCurrent.Location = new System.Drawing.Point(112, 28);
            this.textBoxCurrent.Name = "textBoxCurrent";
            this.textBoxCurrent.Size = new System.Drawing.Size(188, 29);
            this.textBoxCurrent.TabIndex = 10;
            // 
            // lblVoltage
            // 
            this.lblVoltage.AutoSize = true;
            this.lblVoltage.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblVoltage.Location = new System.Drawing.Point(28, 138);
            this.lblVoltage.Name = "lblVoltage";
            this.lblVoltage.Size = new System.Drawing.Size(82, 25);
            this.lblVoltage.TabIndex = 7;
            this.lblVoltage.Text = "목표 전압";
            // 
            // lblDose
            // 
            this.lblDose.AutoSize = true;
            this.lblDose.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDose.Location = new System.Drawing.Point(66, 94);
            this.lblDose.Name = "lblDose";
            this.lblDose.Size = new System.Drawing.Size(62, 25);
            this.lblDose.TabIndex = 6;
            this.lblDose.Text = "Dose";
            // 
            // lblIonGas
            // 
            this.lblIonGas.AutoSize = true;
            this.lblIonGas.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblIonGas.Location = new System.Drawing.Point(28, 47);
            this.lblIonGas.Name = "lblIonGas";
            this.lblIonGas.Size = new System.Drawing.Size(82, 25);
            this.lblIonGas.TabIndex = 4;
            this.lblIonGas.Text = "이온 가스";
            // 
            // textBoxVoltage
            // 
            this.textBoxVoltage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxVoltage.Location = new System.Drawing.Point(130, 133);
            this.textBoxVoltage.Name = "textBoxVoltage";
            this.textBoxVoltage.Size = new System.Drawing.Size(188, 29);
            this.textBoxVoltage.TabIndex = 3;
            // 
            // listBoxIonGas
            // 
            this.listBoxIonGas.AllowDrop = true;
            this.listBoxIonGas.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBoxIonGas.FormattingEnabled = true;
            this.listBoxIonGas.ItemHeight = 24;
            this.listBoxIonGas.Items.AddRange(new object[] {
            "붕소(B)",
            "인(P)",
            "비소(As)",
            "포스핀(PH₃)"});
            this.listBoxIonGas.Location = new System.Drawing.Point(130, 47);
            this.listBoxIonGas.Name = "listBoxIonGas";
            this.listBoxIonGas.Size = new System.Drawing.Size(188, 28);
            this.listBoxIonGas.TabIndex = 1;
            // 
            // textBoxDose
            // 
            this.textBoxDose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDose.Location = new System.Drawing.Point(130, 89);
            this.textBoxDose.Name = "textBoxDose";
            this.textBoxDose.Size = new System.Drawing.Size(188, 29);
            this.textBoxDose.TabIndex = 0;
            // 
            // panelPm2
            // 
            this.panelPm2.Controls.Add(this.groupBox1);
            this.panelPm2.Controls.Add(this.groupBox2);
            this.panelPm2.Controls.Add(this.lblVoltage2);
            this.panelPm2.Controls.Add(this.lblDose2);
            this.panelPm2.Controls.Add(this.lblIonGas2);
            this.panelPm2.Controls.Add(this.textBoxVoltage2);
            this.panelPm2.Controls.Add(this.listBoxIonGas2);
            this.panelPm2.Controls.Add(this.textBoxDose2);
            this.panelPm2.Location = new System.Drawing.Point(270, 58);
            this.panelPm2.Name = "panelPm2";
            this.panelPm2.Size = new System.Drawing.Size(1022, 449);
            this.panelPm2.TabIndex = 10;
            this.panelPm2.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblScannerVoltage2);
            this.groupBox1.Controls.Add(this.textBoxScannerVoltage2);
            this.groupBox1.Controls.Add(this.lblMotor2);
            this.groupBox1.Controls.Add(this.textBoxMotor2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(356, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 148);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "스캐너";
            // 
            // lblScannerVoltage2
            // 
            this.lblScannerVoltage2.AutoSize = true;
            this.lblScannerVoltage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblScannerVoltage2.Location = new System.Drawing.Point(25, 77);
            this.lblScannerVoltage2.Name = "lblScannerVoltage2";
            this.lblScannerVoltage2.Size = new System.Drawing.Size(44, 25);
            this.lblScannerVoltage2.TabIndex = 15;
            this.lblScannerVoltage2.Text = "전압";
            // 
            // textBoxScannerVoltage2
            // 
            this.textBoxScannerVoltage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxScannerVoltage2.Location = new System.Drawing.Point(92, 76);
            this.textBoxScannerVoltage2.Name = "textBoxScannerVoltage2";
            this.textBoxScannerVoltage2.Size = new System.Drawing.Size(188, 29);
            this.textBoxScannerVoltage2.TabIndex = 14;
            // 
            // lblMotor2
            // 
            this.lblMotor2.AutoSize = true;
            this.lblMotor2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMotor2.Location = new System.Drawing.Point(25, 33);
            this.lblMotor2.Name = "lblMotor2";
            this.lblMotor2.Size = new System.Drawing.Size(44, 25);
            this.lblMotor2.TabIndex = 13;
            this.lblMotor2.Text = "모터";
            // 
            // textBoxMotor2
            // 
            this.textBoxMotor2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMotor2.Location = new System.Drawing.Point(92, 32);
            this.textBoxMotor2.Name = "textBoxMotor2";
            this.textBoxMotor2.Size = new System.Drawing.Size(188, 29);
            this.textBoxMotor2.TabIndex = 12;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblCurrent2);
            this.groupBox2.Controls.Add(this.textBoxCurrent2);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.Location = new System.Drawing.Point(14, 174);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(317, 86);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "분석기";
            // 
            // lblCurrent2
            // 
            this.lblCurrent2.AutoSize = true;
            this.lblCurrent2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrent2.Location = new System.Drawing.Point(13, 33);
            this.lblCurrent2.Name = "lblCurrent2";
            this.lblCurrent2.Size = new System.Drawing.Size(82, 25);
            this.lblCurrent2.TabIndex = 11;
            this.lblCurrent2.Text = "자석 전류";
            // 
            // textBoxCurrent2
            // 
            this.textBoxCurrent2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCurrent2.Location = new System.Drawing.Point(115, 28);
            this.textBoxCurrent2.Name = "textBoxCurrent2";
            this.textBoxCurrent2.Size = new System.Drawing.Size(188, 29);
            this.textBoxCurrent2.TabIndex = 10;
            // 
            // lblVoltage2
            // 
            this.lblVoltage2.AutoSize = true;
            this.lblVoltage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblVoltage2.Location = new System.Drawing.Point(28, 143);
            this.lblVoltage2.Name = "lblVoltage2";
            this.lblVoltage2.Size = new System.Drawing.Size(82, 25);
            this.lblVoltage2.TabIndex = 7;
            this.lblVoltage2.Text = "목표 전압";
            // 
            // lblDose2
            // 
            this.lblDose2.AutoSize = true;
            this.lblDose2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDose2.Location = new System.Drawing.Point(65, 96);
            this.lblDose2.Name = "lblDose2";
            this.lblDose2.Size = new System.Drawing.Size(62, 25);
            this.lblDose2.TabIndex = 6;
            this.lblDose2.Text = "Dose";
            // 
            // lblIonGas2
            // 
            this.lblIonGas2.AutoSize = true;
            this.lblIonGas2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblIonGas2.Location = new System.Drawing.Point(28, 47);
            this.lblIonGas2.Name = "lblIonGas2";
            this.lblIonGas2.Size = new System.Drawing.Size(82, 25);
            this.lblIonGas2.TabIndex = 4;
            this.lblIonGas2.Text = "이온 가스";
            // 
            // textBoxVoltage2
            // 
            this.textBoxVoltage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxVoltage2.Location = new System.Drawing.Point(130, 138);
            this.textBoxVoltage2.Name = "textBoxVoltage2";
            this.textBoxVoltage2.Size = new System.Drawing.Size(188, 29);
            this.textBoxVoltage2.TabIndex = 3;
            // 
            // listBoxIonGas2
            // 
            this.listBoxIonGas2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBoxIonGas2.FormattingEnabled = true;
            this.listBoxIonGas2.ItemHeight = 24;
            this.listBoxIonGas2.Items.AddRange(new object[] {
            "붕소(B)",
            "인(P)",
            "비소(As)",
            "포스핀(PH₃)"});
            this.listBoxIonGas2.Location = new System.Drawing.Point(130, 47);
            this.listBoxIonGas2.Name = "listBoxIonGas2";
            this.listBoxIonGas2.Size = new System.Drawing.Size(188, 28);
            this.listBoxIonGas2.TabIndex = 1;
            // 
            // textBoxDose2
            // 
            this.textBoxDose2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDose2.Location = new System.Drawing.Point(130, 91);
            this.textBoxDose2.Name = "textBoxDose2";
            this.textBoxDose2.Size = new System.Drawing.Size(188, 29);
            this.textBoxDose2.TabIndex = 0;
            // 
            // panelPm3
            // 
            this.panelPm3.Controls.Add(this.textBoxVaccum);
            this.panelPm3.Controls.Add(this.textBoxTemperature);
            this.panelPm3.Controls.Add(this.lblTemperature);
            this.panelPm3.Controls.Add(this.lblVaccum);
            this.panelPm3.Location = new System.Drawing.Point(270, 58);
            this.panelPm3.Name = "panelPm3";
            this.panelPm3.Size = new System.Drawing.Size(1022, 449);
            this.panelPm3.TabIndex = 11;
            this.panelPm3.Visible = false;
            // 
            // textBoxVaccum
            // 
            this.textBoxVaccum.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxVaccum.Location = new System.Drawing.Point(129, 43);
            this.textBoxVaccum.Name = "textBoxVaccum";
            this.textBoxVaccum.Size = new System.Drawing.Size(188, 29);
            this.textBoxVaccum.TabIndex = 8;
            // 
            // textBoxTemperature
            // 
            this.textBoxTemperature.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxTemperature.Location = new System.Drawing.Point(129, 89);
            this.textBoxTemperature.Name = "textBoxTemperature";
            this.textBoxTemperature.Size = new System.Drawing.Size(188, 29);
            this.textBoxTemperature.TabIndex = 7;
            // 
            // lblTemperature
            // 
            this.lblTemperature.AutoSize = true;
            this.lblTemperature.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTemperature.Location = new System.Drawing.Point(74, 89);
            this.lblTemperature.Name = "lblTemperature";
            this.lblTemperature.Size = new System.Drawing.Size(44, 25);
            this.lblTemperature.TabIndex = 5;
            this.lblTemperature.Text = "온도";
            // 
            // lblVaccum
            // 
            this.lblVaccum.AutoSize = true;
            this.lblVaccum.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblVaccum.Location = new System.Drawing.Point(73, 47);
            this.lblVaccum.Name = "lblVaccum";
            this.lblVaccum.Size = new System.Drawing.Size(44, 25);
            this.lblVaccum.TabIndex = 4;
            this.lblVaccum.Text = "진공";
            // 
            // RecipeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelPm2);
            this.Controls.Add(this.panelPm3);
            this.Controls.Add(this.panelPm1);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnRecipePm3);
            this.Controls.Add(this.btnRecipePm2);
            this.Controls.Add(this.btnRecipePm1);
            this.Name = "RecipeView";
            this.Size = new System.Drawing.Size(1424, 715);
            this.panelPm1.ResumeLayout(false);
            this.panelPm1.PerformLayout();
            this.gBoxScanner.ResumeLayout(false);
            this.gBoxScanner.PerformLayout();
            this.gBoxAnalyzer.ResumeLayout(false);
            this.gBoxAnalyzer.PerformLayout();
            this.panelPm2.ResumeLayout(false);
            this.panelPm2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelPm3.ResumeLayout(false);
            this.panelPm3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRecipePm1;
        private System.Windows.Forms.Button btnRecipePm2;
        private System.Windows.Forms.Button btnRecipePm3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Panel panelPm1;
        private System.Windows.Forms.TextBox textBoxVoltage;
        private System.Windows.Forms.ListBox listBoxIonGas;
        private System.Windows.Forms.TextBox textBoxDose;
        private System.Windows.Forms.Label lblVoltage;
        private System.Windows.Forms.Label lblDose;
        private System.Windows.Forms.Label lblIonGas;
        private System.Windows.Forms.GroupBox gBoxScanner;
        private System.Windows.Forms.Label lblMotor;
        private System.Windows.Forms.TextBox textBoxMotor;
        private System.Windows.Forms.GroupBox gBoxAnalyzer;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.TextBox textBoxCurrent;
        private System.Windows.Forms.Label lblScannerVoltage;
        private System.Windows.Forms.TextBox textBoxScannerVoltage;
        private System.Windows.Forms.Panel panelPm2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblScannerVoltage2;
        private System.Windows.Forms.TextBox textBoxScannerVoltage2;
        private System.Windows.Forms.Label lblMotor2;
        private System.Windows.Forms.TextBox textBoxMotor2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblCurrent2;
        private System.Windows.Forms.TextBox textBoxCurrent2;
        private System.Windows.Forms.Label lblVoltage2;
        private System.Windows.Forms.Label lblDose2;
        private System.Windows.Forms.Label lblIonGas2;
        private System.Windows.Forms.TextBox textBoxVoltage2;
        private System.Windows.Forms.ListBox listBoxIonGas2;
        private System.Windows.Forms.TextBox textBoxDose2;
        private System.Windows.Forms.Panel panelPm3;
        private System.Windows.Forms.TextBox textBoxVaccum;
        private System.Windows.Forms.TextBox textBoxTemperature;
        private System.Windows.Forms.Label lblTemperature;
        private System.Windows.Forms.Label lblVaccum;
    }
}
