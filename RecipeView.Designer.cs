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
            this.lblGas = new System.Windows.Forms.Label();
            this.lblIonGas = new System.Windows.Forms.Label();
            this.textBoxVoltage = new System.Windows.Forms.TextBox();
            this.listBoxGas = new System.Windows.Forms.ListBox();
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
            this.lblGas2 = new System.Windows.Forms.Label();
            this.lblIonGas2 = new System.Windows.Forms.Label();
            this.textBoxVoltage2 = new System.Windows.Forms.TextBox();
            this.listBoxGas2 = new System.Windows.Forms.ListBox();
            this.listBoxIonGas2 = new System.Windows.Forms.ListBox();
            this.textBoxDose2 = new System.Windows.Forms.TextBox();
            this.panelPm3 = new System.Windows.Forms.Panel();
            this.textBoxVaccum = new System.Windows.Forms.TextBox();
            this.textBoxTemperature = new System.Windows.Forms.TextBox();
            this.labelProcesstime = new System.Windows.Forms.Label();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.lblVaccum = new System.Windows.Forms.Label();
            this.textBoxProcesstime = new System.Windows.Forms.TextBox();
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
            this.btnRecipePm1.Font = new System.Drawing.Font("나눔고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecipePm1.Location = new System.Drawing.Point(57, 100);
            this.btnRecipePm1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRecipePm1.Name = "btnRecipePm1";
            this.btnRecipePm1.Size = new System.Drawing.Size(269, 158);
            this.btnRecipePm1.TabIndex = 0;
            this.btnRecipePm1.Text = "PM1";
            this.btnRecipePm1.UseVisualStyleBackColor = true;
            this.btnRecipePm1.Click += new System.EventHandler(this.btnRecipePm1_Click);
            // 
            // btnRecipePm2
            // 
            this.btnRecipePm2.Font = new System.Drawing.Font("나눔고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecipePm2.Location = new System.Drawing.Point(57, 286);
            this.btnRecipePm2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRecipePm2.Name = "btnRecipePm2";
            this.btnRecipePm2.Size = new System.Drawing.Size(269, 158);
            this.btnRecipePm2.TabIndex = 1;
            this.btnRecipePm2.Text = "PM2";
            this.btnRecipePm2.UseVisualStyleBackColor = true;
            this.btnRecipePm2.Click += new System.EventHandler(this.btnRecipePm2_Click);
            // 
            // btnRecipePm3
            // 
            this.btnRecipePm3.Font = new System.Drawing.Font("나눔고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecipePm3.Location = new System.Drawing.Point(57, 478);
            this.btnRecipePm3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRecipePm3.Name = "btnRecipePm3";
            this.btnRecipePm3.Size = new System.Drawing.Size(269, 158);
            this.btnRecipePm3.TabIndex = 2;
            this.btnRecipePm3.Text = "PM3";
            this.btnRecipePm3.UseVisualStyleBackColor = true;
            this.btnRecipePm3.Click += new System.EventHandler(this.btnRecipePm3_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("나눔고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.Location = new System.Drawing.Point(647, 826);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(217, 96);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("나눔고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLoad.Location = new System.Drawing.Point(930, 826);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(217, 96);
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
            this.panelPm1.Controls.Add(this.lblGas);
            this.panelPm1.Controls.Add(this.lblIonGas);
            this.panelPm1.Controls.Add(this.textBoxVoltage);
            this.panelPm1.Controls.Add(this.listBoxGas);
            this.panelPm1.Controls.Add(this.listBoxIonGas);
            this.panelPm1.Controls.Add(this.textBoxDose);
            this.panelPm1.Location = new System.Drawing.Point(386, 87);
            this.panelPm1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelPm1.Name = "panelPm1";
            this.panelPm1.Size = new System.Drawing.Size(1460, 674);
            this.panelPm1.TabIndex = 5;
            // 
            // gBoxScanner
            // 
            this.gBoxScanner.Controls.Add(this.lblScannerVoltage);
            this.gBoxScanner.Controls.Add(this.textBoxScannerVoltage);
            this.gBoxScanner.Controls.Add(this.lblMotor);
            this.gBoxScanner.Controls.Add(this.textBoxMotor);
            this.gBoxScanner.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gBoxScanner.Location = new System.Drawing.Point(509, 21);
            this.gBoxScanner.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gBoxScanner.Name = "gBoxScanner";
            this.gBoxScanner.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gBoxScanner.Size = new System.Drawing.Size(481, 222);
            this.gBoxScanner.TabIndex = 9;
            this.gBoxScanner.TabStop = false;
            this.gBoxScanner.Text = "스캐너";
            // 
            // lblScannerVoltage
            // 
            this.lblScannerVoltage.AutoSize = true;
            this.lblScannerVoltage.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblScannerVoltage.Location = new System.Drawing.Point(36, 116);
            this.lblScannerVoltage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblScannerVoltage.Name = "lblScannerVoltage";
            this.lblScannerVoltage.Size = new System.Drawing.Size(75, 36);
            this.lblScannerVoltage.TabIndex = 15;
            this.lblScannerVoltage.Text = "전압";
            // 
            // textBoxScannerVoltage
            // 
            this.textBoxScannerVoltage.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxScannerVoltage.Location = new System.Drawing.Point(131, 114);
            this.textBoxScannerVoltage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxScannerVoltage.Name = "textBoxScannerVoltage";
            this.textBoxScannerVoltage.Size = new System.Drawing.Size(267, 40);
            this.textBoxScannerVoltage.TabIndex = 14;
            // 
            // lblMotor
            // 
            this.lblMotor.AutoSize = true;
            this.lblMotor.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMotor.Location = new System.Drawing.Point(36, 50);
            this.lblMotor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMotor.Name = "lblMotor";
            this.lblMotor.Size = new System.Drawing.Size(75, 36);
            this.lblMotor.TabIndex = 13;
            this.lblMotor.Text = "모터";
            // 
            // textBoxMotor
            // 
            this.textBoxMotor.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMotor.Location = new System.Drawing.Point(131, 48);
            this.textBoxMotor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxMotor.Name = "textBoxMotor";
            this.textBoxMotor.Size = new System.Drawing.Size(267, 40);
            this.textBoxMotor.TabIndex = 12;
            // 
            // gBoxAnalyzer
            // 
            this.gBoxAnalyzer.Controls.Add(this.lblCurrent);
            this.gBoxAnalyzer.Controls.Add(this.textBoxCurrent);
            this.gBoxAnalyzer.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gBoxAnalyzer.Location = new System.Drawing.Point(20, 345);
            this.gBoxAnalyzer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gBoxAnalyzer.Name = "gBoxAnalyzer";
            this.gBoxAnalyzer.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gBoxAnalyzer.Size = new System.Drawing.Size(453, 129);
            this.gBoxAnalyzer.TabIndex = 8;
            this.gBoxAnalyzer.TabStop = false;
            this.gBoxAnalyzer.Text = "분석기";
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrent.Location = new System.Drawing.Point(19, 50);
            this.lblCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(144, 36);
            this.lblCurrent.TabIndex = 11;
            this.lblCurrent.Text = "자석 전류";
            // 
            // textBoxCurrent
            // 
            this.textBoxCurrent.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCurrent.Location = new System.Drawing.Point(164, 42);
            this.textBoxCurrent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxCurrent.Name = "textBoxCurrent";
            this.textBoxCurrent.Size = new System.Drawing.Size(267, 40);
            this.textBoxCurrent.TabIndex = 10;
            // 
            // lblVoltage
            // 
            this.lblVoltage.AutoSize = true;
            this.lblVoltage.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblVoltage.Location = new System.Drawing.Point(40, 286);
            this.lblVoltage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVoltage.Name = "lblVoltage";
            this.lblVoltage.Size = new System.Drawing.Size(144, 36);
            this.lblVoltage.TabIndex = 7;
            this.lblVoltage.Text = "목표 전압";
            // 
            // lblDose
            // 
            this.lblDose.AutoSize = true;
            this.lblDose.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDose.Location = new System.Drawing.Point(94, 207);
            this.lblDose.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDose.Name = "lblDose";
            this.lblDose.Size = new System.Drawing.Size(88, 36);
            this.lblDose.TabIndex = 6;
            this.lblDose.Text = "Dose";
            // 
            // lblGas
            // 
            this.lblGas.AutoSize = true;
            this.lblGas.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblGas.Location = new System.Drawing.Point(106, 134);
            this.lblGas.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGas.Name = "lblGas";
            this.lblGas.Size = new System.Drawing.Size(75, 36);
            this.lblGas.TabIndex = 5;
            this.lblGas.Text = "가스";
            // 
            // lblIonGas
            // 
            this.lblIonGas.AutoSize = true;
            this.lblIonGas.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblIonGas.Location = new System.Drawing.Point(40, 70);
            this.lblIonGas.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIonGas.Name = "lblIonGas";
            this.lblIonGas.Size = new System.Drawing.Size(144, 36);
            this.lblIonGas.TabIndex = 4;
            this.lblIonGas.Text = "이온 가스";
            // 
            // textBoxVoltage
            // 
            this.textBoxVoltage.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxVoltage.Location = new System.Drawing.Point(186, 279);
            this.textBoxVoltage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxVoltage.Name = "textBoxVoltage";
            this.textBoxVoltage.Size = new System.Drawing.Size(267, 40);
            this.textBoxVoltage.TabIndex = 3;
            // 
            // listBoxGas
            // 
            this.listBoxGas.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBoxGas.FormattingEnabled = true;
            this.listBoxGas.ItemHeight = 34;
            this.listBoxGas.Location = new System.Drawing.Point(186, 134);
            this.listBoxGas.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBoxGas.Name = "listBoxGas";
            this.listBoxGas.Size = new System.Drawing.Size(267, 38);
            this.listBoxGas.TabIndex = 2;
            // 
            // listBoxIonGas
            // 
            this.listBoxIonGas.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBoxIonGas.FormattingEnabled = true;
            this.listBoxIonGas.ItemHeight = 34;
            this.listBoxIonGas.Location = new System.Drawing.Point(186, 70);
            this.listBoxIonGas.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBoxIonGas.Name = "listBoxIonGas";
            this.listBoxIonGas.Size = new System.Drawing.Size(267, 38);
            this.listBoxIonGas.TabIndex = 1;
            // 
            // textBoxDose
            // 
            this.textBoxDose.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDose.Location = new System.Drawing.Point(186, 200);
            this.textBoxDose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxDose.Name = "textBoxDose";
            this.textBoxDose.Size = new System.Drawing.Size(267, 40);
            this.textBoxDose.TabIndex = 0;
            // 
            // panelPm2
            // 
            this.panelPm2.Controls.Add(this.groupBox1);
            this.panelPm2.Controls.Add(this.groupBox2);
            this.panelPm2.Controls.Add(this.lblVoltage2);
            this.panelPm2.Controls.Add(this.lblDose2);
            this.panelPm2.Controls.Add(this.lblGas2);
            this.panelPm2.Controls.Add(this.lblIonGas2);
            this.panelPm2.Controls.Add(this.textBoxVoltage2);
            this.panelPm2.Controls.Add(this.listBoxGas2);
            this.panelPm2.Controls.Add(this.listBoxIonGas2);
            this.panelPm2.Controls.Add(this.textBoxDose2);
            this.panelPm2.Location = new System.Drawing.Point(386, 87);
            this.panelPm2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelPm2.Name = "panelPm2";
            this.panelPm2.Size = new System.Drawing.Size(1460, 674);
            this.panelPm2.TabIndex = 10;
            this.panelPm2.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblScannerVoltage2);
            this.groupBox1.Controls.Add(this.textBoxScannerVoltage2);
            this.groupBox1.Controls.Add(this.lblMotor2);
            this.groupBox1.Controls.Add(this.textBoxMotor2);
            this.groupBox1.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(509, 21);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(481, 222);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "스캐너";
            // 
            // lblScannerVoltage2
            // 
            this.lblScannerVoltage2.AutoSize = true;
            this.lblScannerVoltage2.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblScannerVoltage2.Location = new System.Drawing.Point(36, 116);
            this.lblScannerVoltage2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblScannerVoltage2.Name = "lblScannerVoltage2";
            this.lblScannerVoltage2.Size = new System.Drawing.Size(75, 36);
            this.lblScannerVoltage2.TabIndex = 15;
            this.lblScannerVoltage2.Text = "전압";
            // 
            // textBoxScannerVoltage2
            // 
            this.textBoxScannerVoltage2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxScannerVoltage2.Location = new System.Drawing.Point(131, 114);
            this.textBoxScannerVoltage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxScannerVoltage2.Name = "textBoxScannerVoltage2";
            this.textBoxScannerVoltage2.Size = new System.Drawing.Size(267, 40);
            this.textBoxScannerVoltage2.TabIndex = 14;
            // 
            // lblMotor2
            // 
            this.lblMotor2.AutoSize = true;
            this.lblMotor2.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMotor2.Location = new System.Drawing.Point(36, 50);
            this.lblMotor2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMotor2.Name = "lblMotor2";
            this.lblMotor2.Size = new System.Drawing.Size(75, 36);
            this.lblMotor2.TabIndex = 13;
            this.lblMotor2.Text = "모터";
            // 
            // textBoxMotor2
            // 
            this.textBoxMotor2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMotor2.Location = new System.Drawing.Point(131, 48);
            this.textBoxMotor2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxMotor2.Name = "textBoxMotor2";
            this.textBoxMotor2.Size = new System.Drawing.Size(267, 40);
            this.textBoxMotor2.TabIndex = 12;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblCurrent2);
            this.groupBox2.Controls.Add(this.textBoxCurrent2);
            this.groupBox2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.Location = new System.Drawing.Point(20, 345);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(453, 129);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "분석기";
            // 
            // lblCurrent2
            // 
            this.lblCurrent2.AutoSize = true;
            this.lblCurrent2.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrent2.Location = new System.Drawing.Point(19, 50);
            this.lblCurrent2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrent2.Name = "lblCurrent2";
            this.lblCurrent2.Size = new System.Drawing.Size(144, 36);
            this.lblCurrent2.TabIndex = 11;
            this.lblCurrent2.Text = "자석 전류";
            // 
            // textBoxCurrent2
            // 
            this.textBoxCurrent2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCurrent2.Location = new System.Drawing.Point(164, 42);
            this.textBoxCurrent2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxCurrent2.Name = "textBoxCurrent2";
            this.textBoxCurrent2.Size = new System.Drawing.Size(267, 40);
            this.textBoxCurrent2.TabIndex = 10;
            // 
            // lblVoltage2
            // 
            this.lblVoltage2.AutoSize = true;
            this.lblVoltage2.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblVoltage2.Location = new System.Drawing.Point(40, 286);
            this.lblVoltage2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVoltage2.Name = "lblVoltage2";
            this.lblVoltage2.Size = new System.Drawing.Size(144, 36);
            this.lblVoltage2.TabIndex = 7;
            this.lblVoltage2.Text = "목표 전압";
            // 
            // lblDose2
            // 
            this.lblDose2.AutoSize = true;
            this.lblDose2.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDose2.Location = new System.Drawing.Point(94, 207);
            this.lblDose2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDose2.Name = "lblDose2";
            this.lblDose2.Size = new System.Drawing.Size(88, 36);
            this.lblDose2.TabIndex = 6;
            this.lblDose2.Text = "Dose";
            // 
            // lblGas2
            // 
            this.lblGas2.AutoSize = true;
            this.lblGas2.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblGas2.Location = new System.Drawing.Point(106, 134);
            this.lblGas2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGas2.Name = "lblGas2";
            this.lblGas2.Size = new System.Drawing.Size(75, 36);
            this.lblGas2.TabIndex = 5;
            this.lblGas2.Text = "가스";
            // 
            // lblIonGas2
            // 
            this.lblIonGas2.AutoSize = true;
            this.lblIonGas2.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblIonGas2.Location = new System.Drawing.Point(40, 70);
            this.lblIonGas2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIonGas2.Name = "lblIonGas2";
            this.lblIonGas2.Size = new System.Drawing.Size(144, 36);
            this.lblIonGas2.TabIndex = 4;
            this.lblIonGas2.Text = "이온 가스";
            // 
            // textBoxVoltage2
            // 
            this.textBoxVoltage2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxVoltage2.Location = new System.Drawing.Point(186, 279);
            this.textBoxVoltage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxVoltage2.Name = "textBoxVoltage2";
            this.textBoxVoltage2.Size = new System.Drawing.Size(267, 40);
            this.textBoxVoltage2.TabIndex = 3;
            // 
            // listBoxGas2
            // 
            this.listBoxGas2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBoxGas2.FormattingEnabled = true;
            this.listBoxGas2.ItemHeight = 34;
            this.listBoxGas2.Location = new System.Drawing.Point(186, 134);
            this.listBoxGas2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBoxGas2.Name = "listBoxGas2";
            this.listBoxGas2.Size = new System.Drawing.Size(267, 38);
            this.listBoxGas2.TabIndex = 2;
            // 
            // listBoxIonGas2
            // 
            this.listBoxIonGas2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBoxIonGas2.FormattingEnabled = true;
            this.listBoxIonGas2.ItemHeight = 34;
            this.listBoxIonGas2.Location = new System.Drawing.Point(186, 70);
            this.listBoxIonGas2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBoxIonGas2.Name = "listBoxIonGas2";
            this.listBoxIonGas2.Size = new System.Drawing.Size(267, 38);
            this.listBoxIonGas2.TabIndex = 1;
            // 
            // textBoxDose2
            // 
            this.textBoxDose2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDose2.Location = new System.Drawing.Point(186, 200);
            this.textBoxDose2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxDose2.Name = "textBoxDose2";
            this.textBoxDose2.Size = new System.Drawing.Size(267, 40);
            this.textBoxDose2.TabIndex = 0;
            // 
            // panelPm3
            // 
            this.panelPm3.Controls.Add(this.textBoxVaccum);
            this.panelPm3.Controls.Add(this.textBoxTemperature);
            this.panelPm3.Controls.Add(this.labelProcesstime);
            this.panelPm3.Controls.Add(this.lblTemperature);
            this.panelPm3.Controls.Add(this.lblVaccum);
            this.panelPm3.Controls.Add(this.textBoxProcesstime);
            this.panelPm3.Location = new System.Drawing.Point(386, 87);
            this.panelPm3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelPm3.Name = "panelPm3";
            this.panelPm3.Size = new System.Drawing.Size(1460, 674);
            this.panelPm3.TabIndex = 11;
            this.panelPm3.Visible = false;
            // 
            // textBoxVaccum
            // 
            this.textBoxVaccum.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxVaccum.Location = new System.Drawing.Point(184, 64);
            this.textBoxVaccum.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxVaccum.Name = "textBoxVaccum";
            this.textBoxVaccum.Size = new System.Drawing.Size(267, 40);
            this.textBoxVaccum.TabIndex = 8;
            // 
            // textBoxTemperature
            // 
            this.textBoxTemperature.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxTemperature.Location = new System.Drawing.Point(184, 134);
            this.textBoxTemperature.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxTemperature.Name = "textBoxTemperature";
            this.textBoxTemperature.Size = new System.Drawing.Size(267, 40);
            this.textBoxTemperature.TabIndex = 7;
            // 
            // labelProcesstime
            // 
            this.labelProcesstime.AutoSize = true;
            this.labelProcesstime.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelProcesstime.Location = new System.Drawing.Point(40, 207);
            this.labelProcesstime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelProcesstime.Name = "labelProcesstime";
            this.labelProcesstime.Size = new System.Drawing.Size(144, 36);
            this.labelProcesstime.TabIndex = 6;
            this.labelProcesstime.Text = "공정 시간";
            // 
            // lblTemperature
            // 
            this.lblTemperature.AutoSize = true;
            this.lblTemperature.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTemperature.Location = new System.Drawing.Point(106, 134);
            this.lblTemperature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTemperature.Name = "lblTemperature";
            this.lblTemperature.Size = new System.Drawing.Size(75, 36);
            this.lblTemperature.TabIndex = 5;
            this.lblTemperature.Text = "온도";
            // 
            // lblVaccum
            // 
            this.lblVaccum.AutoSize = true;
            this.lblVaccum.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblVaccum.Location = new System.Drawing.Point(104, 70);
            this.lblVaccum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVaccum.Name = "lblVaccum";
            this.lblVaccum.Size = new System.Drawing.Size(75, 36);
            this.lblVaccum.TabIndex = 4;
            this.lblVaccum.Text = "진공";
            // 
            // textBoxProcesstime
            // 
            this.textBoxProcesstime.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxProcesstime.Location = new System.Drawing.Point(186, 200);
            this.textBoxProcesstime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxProcesstime.Name = "textBoxProcesstime";
            this.textBoxProcesstime.Size = new System.Drawing.Size(267, 40);
            this.textBoxProcesstime.TabIndex = 0;
            // 
            // RecipeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelPm3);
            this.Controls.Add(this.panelPm2);
            this.Controls.Add(this.panelPm1);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnRecipePm3);
            this.Controls.Add(this.btnRecipePm2);
            this.Controls.Add(this.btnRecipePm1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "RecipeView";
            this.Size = new System.Drawing.Size(2034, 1072);
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
        private System.Windows.Forms.ListBox listBoxGas;
        private System.Windows.Forms.ListBox listBoxIonGas;
        private System.Windows.Forms.TextBox textBoxDose;
        private System.Windows.Forms.Label lblVoltage;
        private System.Windows.Forms.Label lblDose;
        private System.Windows.Forms.Label lblGas;
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
        private System.Windows.Forms.Label lblGas2;
        private System.Windows.Forms.Label lblIonGas2;
        private System.Windows.Forms.TextBox textBoxVoltage2;
        private System.Windows.Forms.ListBox listBoxGas2;
        private System.Windows.Forms.ListBox listBoxIonGas2;
        private System.Windows.Forms.TextBox textBoxDose2;
        private System.Windows.Forms.Panel panelPm3;
        private System.Windows.Forms.TextBox textBoxVaccum;
        private System.Windows.Forms.TextBox textBoxTemperature;
        private System.Windows.Forms.Label labelProcesstime;
        private System.Windows.Forms.Label lblTemperature;
        private System.Windows.Forms.Label lblVaccum;
        private System.Windows.Forms.TextBox textBoxProcesstime;
    }
}
