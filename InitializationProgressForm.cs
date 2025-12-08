using System;
using System.Drawing;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 하드웨어 초기화 진행 상황을 표시하는 다이얼로그
    /// </summary>
    public partial class InitializationProgressForm : Form
    {
        private Label lblTitle;
        private Label lblCurrentStep;
        private ProgressBar progressBar;
        private ListBox lstSteps;
        private int totalSteps;
        private int currentStep;

        public InitializationProgressForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form 설정
            this.Text = "하드웨어 초기화";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.BackColor = Color.White;

            // 타이틀 라벨
            lblTitle = new Label();
            lblTitle.Text = "장비 초기화 진행 중...";
            lblTitle.Font = new Font("맑은 고딕", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(400, 30);
            lblTitle.ForeColor = Color.FromArgb(50, 50, 50);
            this.Controls.Add(lblTitle);

            // 현재 단계 라벨
            lblCurrentStep = new Label();
            lblCurrentStep.Text = "준비 중...";
            lblCurrentStep.Font = new Font("맑은 고딕", 10, FontStyle.Regular);
            lblCurrentStep.Location = new Point(20, 55);
            lblCurrentStep.Size = new Size(400, 25);
            lblCurrentStep.ForeColor = Color.FromArgb(80, 80, 80);
            this.Controls.Add(lblCurrentStep);

            // 진행률 바
            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 85);
            progressBar.Size = new Size(395, 25);
            progressBar.Style = ProgressBarStyle.Continuous;
            this.Controls.Add(progressBar);

            // 단계 목록
            lstSteps = new ListBox();
            lstSteps.Location = new Point(20, 120);
            lstSteps.Size = new Size(395, 170);
            lstSteps.Font = new Font("맑은 고딕", 9);
            lstSteps.BorderStyle = BorderStyle.FixedSingle;
            lstSteps.SelectionMode = SelectionMode.None;
            this.Controls.Add(lstSteps);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// 총 단계 수 설정
        /// </summary>
        public void SetTotalSteps(int total)
        {
            totalSteps = total;
            currentStep = 0;
            progressBar.Maximum = total;
            progressBar.Value = 0;
        }

        /// <summary>
        /// 다이얼로그 제목 설정 (STOP 모드 등에서 사용)
        /// </summary>
        public void SetTitle(string windowTitle, string labelTitle)
        {
            this.Text = windowTitle;
            lblTitle.Text = labelTitle;
            Application.DoEvents();
        }

        /// <summary>
        /// 다음 단계로 진행하고 메시지 표시
        /// </summary>
        public void NextStep(string stepDescription)
        {
            currentStep++;
            if (currentStep <= totalSteps)
            {
                progressBar.Value = currentStep;
            }

            lblCurrentStep.Text = $"[{currentStep}/{totalSteps}] {stepDescription}";
            lstSteps.Items.Add($"✓ {stepDescription}");
            lstSteps.TopIndex = lstSteps.Items.Count - 1;

            // UI 업데이트 강제
            Application.DoEvents();
        }

        /// <summary>
        /// 초기화 완료 표시
        /// </summary>
        public void SetComplete()
        {
            lblTitle.Text = "초기화 완료";
            lblCurrentStep.Text = "모든 초기화 단계가 완료되었습니다.";
            progressBar.Value = progressBar.Maximum;
            lstSteps.Items.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            lstSteps.Items.Add("✔ 초기화 완료!");
            lstSteps.TopIndex = lstSteps.Items.Count - 1;
            Application.DoEvents();
        }

        /// <summary>
        /// 정지 완료 표시
        /// </summary>
        public void SetStopComplete()
        {
            lblTitle.Text = "정지 완료";
            lblCurrentStep.Text = "장비가 안전하게 정지되었습니다.";
            progressBar.Value = progressBar.Maximum;
            lstSteps.Items.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            lstSteps.Items.Add("✔ 정지 완료!");
            lstSteps.TopIndex = lstSteps.Items.Count - 1;
            Application.DoEvents();
        }

        /// <summary>
        /// 오류 표시
        /// </summary>
        public void SetError(string errorMessage)
        {
            lblTitle.Text = "초기화 실패";
            lblTitle.ForeColor = Color.Red;
            lblCurrentStep.Text = errorMessage;
            lstSteps.Items.Add($"✗ 오류: {errorMessage}");
            lstSteps.TopIndex = lstSteps.Items.Count - 1;
            Application.DoEvents();
        }
    }
}
