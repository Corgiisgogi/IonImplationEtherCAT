namespace IonImplationEtherCAT
{
    partial class MainForm
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelFooter = new System.Windows.Forms.Panel();
            this.btnRecipe = new System.Windows.Forms.Button();
            this.btnMain = new System.Windows.Forms.Button();
            this.btnAlarmManager = new System.Windows.Forms.Button();
            this.btnLog = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelLoginContainer = new System.Windows.Forms.Panel();
            this.panelUserInfoView = new System.Windows.Forms.Panel();
            this.lblCurrentUserName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.panelLoginView = new System.Windows.Forms.Panel();
            this.labelpw = new System.Windows.Forms.Label();
            this.labelId = new System.Windows.Forms.Label();
            this.textBoxPw = new System.Windows.Forms.TextBox();
            this.textBoxId = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.panelStatusView = new System.Windows.Forms.Panel();
            this.panelPm3StatusView = new System.Windows.Forms.Panel();
            this.lblPm3 = new System.Windows.Forms.Label();
            this.panelPm2StatusView = new System.Windows.Forms.Panel();
            this.lblPm2 = new System.Windows.Forms.Label();
            this.panelPm1StatusView = new System.Windows.Forms.Panel();
            this.lblPm1 = new System.Windows.Forms.Label();
            this.panelTmStatusView = new System.Windows.Forms.Panel();
            this.lblTm = new System.Windows.Forms.Label();
            this.panelNetworkAlartView = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblEthercat = new System.Windows.Forms.Label();
            this.panelAlartContainer = new System.Windows.Forms.Panel();
            this.lblAlartContent = new System.Windows.Forms.Label();
            this.lblAlart = new System.Windows.Forms.Label();
            this.lblSlave = new System.Windows.Forms.Label();
            this.lblEtherCatStatus = new System.Windows.Forms.Label();
            this.lblSlaveStatus = new System.Windows.Forms.Label();
            this.panelFooter.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelLoginContainer.SuspendLayout();
            this.panelUserInfoView.SuspendLayout();
            this.panelLoginView.SuspendLayout();
            this.panelStatusView.SuspendLayout();
            this.panelNetworkAlartView.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelAlartContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFooter
            // 
            this.panelFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFooter.Controls.Add(this.btnRecipe);
            this.panelFooter.Controls.Add(this.btnMain);
            this.panelFooter.Controls.Add(this.btnAlarmManager);
            this.panelFooter.Controls.Add(this.btnLog);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 827);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(1424, 158);
            this.panelFooter.TabIndex = 1;
            // 
            // btnRecipe
            // 
            this.btnRecipe.Font = new System.Drawing.Font("나눔고딕", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRecipe.Location = new System.Drawing.Point(717, 1);
            this.btnRecipe.Name = "btnRecipe";
            this.btnRecipe.Size = new System.Drawing.Size(347, 154);
            this.btnRecipe.TabIndex = 4;
            this.btnRecipe.Text = "Recipe Management";
            this.btnRecipe.UseVisualStyleBackColor = true;
            this.btnRecipe.Click += new System.EventHandler(this.btnRecipe_Click);
            // 
            // btnMain
            // 
            this.btnMain.Font = new System.Drawing.Font("나눔고딕", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMain.Location = new System.Drawing.Point(11, 1);
            this.btnMain.Name = "btnMain";
            this.btnMain.Size = new System.Drawing.Size(347, 154);
            this.btnMain.TabIndex = 5;
            this.btnMain.Text = "Main";
            this.btnMain.UseVisualStyleBackColor = true;
            this.btnMain.Click += new System.EventHandler(this.btnMain_Click);
            // 
            // btnAlarmManager
            // 
            this.btnAlarmManager.Font = new System.Drawing.Font("나눔고딕", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAlarmManager.Location = new System.Drawing.Point(364, 3);
            this.btnAlarmManager.Name = "btnAlarmManager";
            this.btnAlarmManager.Size = new System.Drawing.Size(347, 154);
            this.btnAlarmManager.TabIndex = 3;
            this.btnAlarmManager.Text = "Alarm Manager";
            this.btnAlarmManager.UseVisualStyleBackColor = true;
            this.btnAlarmManager.Click += new System.EventHandler(this.btnAlarmManager_Click);
            // 
            // btnLog
            // 
            this.btnLog.Font = new System.Drawing.Font("나눔고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLog.Location = new System.Drawing.Point(1072, 1);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(347, 154);
            this.btnLog.TabIndex = 2;
            this.btnLog.Text = "Log";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // panelContent
            // 
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 149);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1424, 678);
            this.panelContent.TabIndex = 2;
            // 
            // panelHeader
            // 
            this.panelHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHeader.Controls.Add(this.panelLoginContainer);
            this.panelHeader.Controls.Add(this.panelStatusView);
            this.panelHeader.Controls.Add(this.panelNetworkAlartView);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1424, 149);
            this.panelHeader.TabIndex = 7;
            // 
            // panelLoginContainer
            // 
            this.panelLoginContainer.Controls.Add(this.panelUserInfoView);
            this.panelLoginContainer.Controls.Add(this.panelLoginView);
            this.panelLoginContainer.Location = new System.Drawing.Point(0, 0);
            this.panelLoginContainer.Name = "panelLoginContainer";
            this.panelLoginContainer.Size = new System.Drawing.Size(323, 149);
            this.panelLoginContainer.TabIndex = 1;
            // 
            // panelUserInfoView
            // 
            this.panelUserInfoView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUserInfoView.Controls.Add(this.lblCurrentUserName);
            this.panelUserInfoView.Controls.Add(this.label2);
            this.panelUserInfoView.Controls.Add(this.btnLogout);
            this.panelUserInfoView.Location = new System.Drawing.Point(-1, -1);
            this.panelUserInfoView.Name = "panelUserInfoView";
            this.panelUserInfoView.Size = new System.Drawing.Size(323, 149);
            this.panelUserInfoView.TabIndex = 0;
            this.panelUserInfoView.Visible = false;
            // 
            // lblCurrentUserName
            // 
            this.lblCurrentUserName.AutoSize = true;
            this.lblCurrentUserName.Font = new System.Drawing.Font("나눔고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrentUserName.Location = new System.Drawing.Point(54, 79);
            this.lblCurrentUserName.Name = "lblCurrentUserName";
            this.lblCurrentUserName.Size = new System.Drawing.Size(47, 19);
            this.lblCurrentUserName.TabIndex = 4;
            this.lblCurrentUserName.Text = "User";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(70, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Account";
            // 
            // btnLogout
            // 
            this.btnLogout.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLogout.Location = new System.Drawing.Point(198, 38);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(106, 74);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // panelLoginView
            // 
            this.panelLoginView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLoginView.Controls.Add(this.labelpw);
            this.panelLoginView.Controls.Add(this.labelId);
            this.panelLoginView.Controls.Add(this.textBoxPw);
            this.panelLoginView.Controls.Add(this.textBoxId);
            this.panelLoginView.Controls.Add(this.btnLogin);
            this.panelLoginView.Location = new System.Drawing.Point(-1, 0);
            this.panelLoginView.Name = "panelLoginView";
            this.panelLoginView.Size = new System.Drawing.Size(323, 149);
            this.panelLoginView.TabIndex = 0;
            // 
            // labelpw
            // 
            this.labelpw.AutoSize = true;
            this.labelpw.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelpw.Location = new System.Drawing.Point(22, 83);
            this.labelpw.Name = "labelpw";
            this.labelpw.Size = new System.Drawing.Size(26, 14);
            this.labelpw.TabIndex = 4;
            this.labelpw.Text = "PW";
            // 
            // labelId
            // 
            this.labelId.AutoSize = true;
            this.labelId.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelId.Location = new System.Drawing.Point(29, 53);
            this.labelId.Name = "labelId";
            this.labelId.Size = new System.Drawing.Size(19, 14);
            this.labelId.TabIndex = 4;
            this.labelId.Text = "ID";
            // 
            // textBoxPw
            // 
            this.textBoxPw.Font = new System.Drawing.Font("굴림", 11F);
            this.textBoxPw.Location = new System.Drawing.Point(51, 77);
            this.textBoxPw.Name = "textBoxPw";
            this.textBoxPw.Size = new System.Drawing.Size(128, 24);
            this.textBoxPw.TabIndex = 5;
            // 
            // textBoxId
            // 
            this.textBoxId.Font = new System.Drawing.Font("굴림", 11F);
            this.textBoxId.Location = new System.Drawing.Point(51, 47);
            this.textBoxId.Name = "textBoxId";
            this.textBoxId.Size = new System.Drawing.Size(128, 24);
            this.textBoxId.TabIndex = 5;
            // 
            // btnLogin
            // 
            this.btnLogin.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLogin.Location = new System.Drawing.Point(198, 38);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(106, 74);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // panelStatusView
            // 
            this.panelStatusView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusView.Controls.Add(this.panelPm3StatusView);
            this.panelStatusView.Controls.Add(this.lblPm3);
            this.panelStatusView.Controls.Add(this.panelPm2StatusView);
            this.panelStatusView.Controls.Add(this.lblPm2);
            this.panelStatusView.Controls.Add(this.panelPm1StatusView);
            this.panelStatusView.Controls.Add(this.lblPm1);
            this.panelStatusView.Controls.Add(this.panelTmStatusView);
            this.panelStatusView.Controls.Add(this.lblTm);
            this.panelStatusView.Location = new System.Drawing.Point(315, 0);
            this.panelStatusView.Name = "panelStatusView";
            this.panelStatusView.Size = new System.Drawing.Size(355, 149);
            this.panelStatusView.TabIndex = 3;
            // 
            // panelPm3StatusView
            // 
            this.panelPm3StatusView.BackColor = System.Drawing.Color.Chartreuse;
            this.panelPm3StatusView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPm3StatusView.Location = new System.Drawing.Point(260, 60);
            this.panelPm3StatusView.Name = "panelPm3StatusView";
            this.panelPm3StatusView.Size = new System.Drawing.Size(40, 40);
            this.panelPm3StatusView.TabIndex = 3;
            // 
            // lblPm3
            // 
            this.lblPm3.AutoSize = true;
            this.lblPm3.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblPm3.Location = new System.Drawing.Point(255, 32);
            this.lblPm3.Name = "lblPm3";
            this.lblPm3.Size = new System.Drawing.Size(51, 21);
            this.lblPm3.TabIndex = 4;
            this.lblPm3.Text = "PM3";
            // 
            // panelPm2StatusView
            // 
            this.panelPm2StatusView.BackColor = System.Drawing.Color.Chartreuse;
            this.panelPm2StatusView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPm2StatusView.Location = new System.Drawing.Point(190, 60);
            this.panelPm2StatusView.Name = "panelPm2StatusView";
            this.panelPm2StatusView.Size = new System.Drawing.Size(40, 40);
            this.panelPm2StatusView.TabIndex = 3;
            // 
            // lblPm2
            // 
            this.lblPm2.AutoSize = true;
            this.lblPm2.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblPm2.Location = new System.Drawing.Point(185, 32);
            this.lblPm2.Name = "lblPm2";
            this.lblPm2.Size = new System.Drawing.Size(51, 21);
            this.lblPm2.TabIndex = 4;
            this.lblPm2.Text = "PM2";
            // 
            // panelPm1StatusView
            // 
            this.panelPm1StatusView.BackColor = System.Drawing.Color.Chartreuse;
            this.panelPm1StatusView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPm1StatusView.Location = new System.Drawing.Point(120, 60);
            this.panelPm1StatusView.Name = "panelPm1StatusView";
            this.panelPm1StatusView.Size = new System.Drawing.Size(40, 40);
            this.panelPm1StatusView.TabIndex = 1;
            // 
            // lblPm1
            // 
            this.lblPm1.AutoSize = true;
            this.lblPm1.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblPm1.Location = new System.Drawing.Point(115, 32);
            this.lblPm1.Name = "lblPm1";
            this.lblPm1.Size = new System.Drawing.Size(51, 21);
            this.lblPm1.TabIndex = 2;
            this.lblPm1.Text = "PM1";
            // 
            // panelTmStatusView
            // 
            this.panelTmStatusView.BackColor = System.Drawing.Color.Chartreuse;
            this.panelTmStatusView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTmStatusView.Location = new System.Drawing.Point(52, 60);
            this.panelTmStatusView.Name = "panelTmStatusView";
            this.panelTmStatusView.Size = new System.Drawing.Size(40, 40);
            this.panelTmStatusView.TabIndex = 0;
            // 
            // lblTm
            // 
            this.lblTm.AutoSize = true;
            this.lblTm.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTm.Location = new System.Drawing.Point(52, 32);
            this.lblTm.Name = "lblTm";
            this.lblTm.Size = new System.Drawing.Size(38, 21);
            this.lblTm.TabIndex = 0;
            this.lblTm.Text = "TM";
            // 
            // panelNetworkAlartView
            // 
            this.panelNetworkAlartView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelNetworkAlartView.Controls.Add(this.btnConnect);
            this.panelNetworkAlartView.Controls.Add(this.groupBox1);
            this.panelNetworkAlartView.Controls.Add(this.panelAlartContainer);
            this.panelNetworkAlartView.Location = new System.Drawing.Point(669, -1);
            this.panelNetworkAlartView.Name = "panelNetworkAlartView";
            this.panelNetworkAlartView.Size = new System.Drawing.Size(759, 149);
            this.panelNetworkAlartView.TabIndex = 4;
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("나눔고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConnect.Location = new System.Drawing.Point(373, 30);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(144, 90);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.groupBox1.Controls.Add(this.lblSlaveStatus);
            this.groupBox1.Controls.Add(this.lblEtherCatStatus);
            this.groupBox1.Controls.Add(this.lblSlave);
            this.groupBox1.Controls.Add(this.lblEthercat);
            this.groupBox1.Location = new System.Drawing.Point(24, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 117);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // lblEthercat
            // 
            this.lblEthercat.AutoSize = true;
            this.lblEthercat.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblEthercat.Location = new System.Drawing.Point(19, 24);
            this.lblEthercat.Name = "lblEthercat";
            this.lblEthercat.Size = new System.Drawing.Size(105, 21);
            this.lblEthercat.TabIndex = 0;
            this.lblEthercat.Text = "ETHERCAT";
            // 
            // panelAlartContainer
            // 
            this.panelAlartContainer.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelAlartContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAlartContainer.Controls.Add(this.lblAlartContent);
            this.panelAlartContainer.Controls.Add(this.lblAlart);
            this.panelAlartContainer.Location = new System.Drawing.Point(545, 9);
            this.panelAlartContainer.Name = "panelAlartContainer";
            this.panelAlartContainer.Size = new System.Drawing.Size(200, 131);
            this.panelAlartContainer.TabIndex = 2;
            // 
            // lblAlartContent
            // 
            this.lblAlartContent.AutoSize = true;
            this.lblAlartContent.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAlartContent.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lblAlartContent.Location = new System.Drawing.Point(70, 52);
            this.lblAlartContent.Name = "lblAlartContent";
            this.lblAlartContent.Size = new System.Drawing.Size(62, 24);
            this.lblAlartContent.TabIndex = 1;
            this.lblAlartContent.Text = "None";
            // 
            // lblAlart
            // 
            this.lblAlart.AutoSize = true;
            this.lblAlart.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAlart.Location = new System.Drawing.Point(65, 4);
            this.lblAlart.Name = "lblAlart";
            this.lblAlart.Size = new System.Drawing.Size(79, 24);
            this.lblAlart.TabIndex = 0;
            this.lblAlart.Text = "ALART";
            // 
            // lblSlave
            // 
            this.lblSlave.AutoSize = true;
            this.lblSlave.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSlave.Location = new System.Drawing.Point(21, 69);
            this.lblSlave.Name = "lblSlave";
            this.lblSlave.Size = new System.Drawing.Size(67, 21);
            this.lblSlave.TabIndex = 1;
            this.lblSlave.Text = "SLAVE";
            // 
            // lblEtherCatStatus
            // 
            this.lblEtherCatStatus.AutoSize = true;
            this.lblEtherCatStatus.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblEtherCatStatus.Location = new System.Drawing.Point(141, 25);
            this.lblEtherCatStatus.Name = "lblEtherCatStatus";
            this.lblEtherCatStatus.Size = new System.Drawing.Size(78, 21);
            this.lblEtherCatStatus.TabIndex = 0;
            this.lblEtherCatStatus.Text = "STATUS";
            // 
            // lblSlaveStatus
            // 
            this.lblSlaveStatus.AutoSize = true;
            this.lblSlaveStatus.Font = new System.Drawing.Font("나눔고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSlaveStatus.Location = new System.Drawing.Point(141, 70);
            this.lblSlaveStatus.Name = "lblSlaveStatus";
            this.lblSlaveStatus.Size = new System.Drawing.Size(78, 21);
            this.lblSlaveStatus.TabIndex = 2;
            this.lblSlaveStatus.Text = "STATUS";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1424, 985);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Form1";
            this.panelFooter.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelLoginContainer.ResumeLayout(false);
            this.panelUserInfoView.ResumeLayout(false);
            this.panelUserInfoView.PerformLayout();
            this.panelLoginView.ResumeLayout(false);
            this.panelLoginView.PerformLayout();
            this.panelStatusView.ResumeLayout(false);
            this.panelStatusView.PerformLayout();
            this.panelNetworkAlartView.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelAlartContainer.ResumeLayout(false);
            this.panelAlartContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Button btnRecipe;
        private System.Windows.Forms.Button btnMain;
        private System.Windows.Forms.Button btnAlarmManager;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelStatusView;
        private System.Windows.Forms.Panel panelLoginView;
        private System.Windows.Forms.Label labelpw;
        private System.Windows.Forms.Label labelId;
        private System.Windows.Forms.TextBox textBoxPw;
        private System.Windows.Forms.TextBox textBoxId;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Panel panelNetworkAlartView;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panelAlartContainer;
        private System.Windows.Forms.Panel panelUserInfoView;
        private System.Windows.Forms.Label lblCurrentUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelLoginContainer;
        private System.Windows.Forms.Panel panelTmStatusView;
        private System.Windows.Forms.Label lblTm;
        private System.Windows.Forms.Panel panelPm1StatusView;
        private System.Windows.Forms.Label lblPm1;
        private System.Windows.Forms.Panel panelPm3StatusView;
        private System.Windows.Forms.Label lblPm3;
        private System.Windows.Forms.Panel panelPm2StatusView;
        private System.Windows.Forms.Label lblPm2;
        private System.Windows.Forms.Label lblEthercat;
        private System.Windows.Forms.Label lblAlartContent;
        private System.Windows.Forms.Label lblAlart;
        private System.Windows.Forms.Label lblSlave;
        private System.Windows.Forms.Label lblSlaveStatus;
        private System.Windows.Forms.Label lblEtherCatStatus;
    }
}

