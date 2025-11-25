using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    public partial class MainView : UserControl
    {
        // 공정 A, B, C용 ProcessModule 객체
        private ProcessModule processModuleA;
        private ProcessModule processModuleB;
        private ProcessModule processModuleC;

        // FOUP A, B 객체
        private Foup foupA;
        private Foup foupB;

        // Transfer Module 객체
        private TransferModule transferModule;

        // TM 그래픽 패널
        private TMGraphicsPanel tmGraphicsPanel;

        // 명령 큐
        private CommandQueue commandQueue;

        // 공정 업데이트용 타이머
        private Timer processTimer;

        // TM 애니메이션용 타이머
        private Timer tmAnimationTimer;
        private DateTime lastAnimationUpdate;

        public MainView()
        {
            InitializeComponent();
            
            // 각 공정용 ProcessModule 객체 생성 (기본값: A=60초, B=120초, C=180초)
            processModuleA = new ProcessModule(60);
            processModuleB = new ProcessModule(120);
            processModuleC = new ProcessModule(180);
            
            // FOUP 객체 생성
            foupA = new Foup();
            foupB = new Foup();
            
            // Transfer Module 객체 생성 및 초기화
            transferModule = new TransferModule();
            InitializeTransferModule();
            
            // TM 그래픽 패널 생성 및 추가
            CreateTMGraphicsPanel();
            
            // 명령 큐 생성
            commandQueue = new CommandQueue(this);
            
            // 타이머 초기화 (1초마다 실행)
            processTimer = new Timer();
            processTimer.Interval = 1000; // 1초
            processTimer.Tick += ProcessTimer_Tick;
            
            // TM 애니메이션 타이머 초기화 (약 60 FPS)
            tmAnimationTimer = new Timer();
            tmAnimationTimer.Interval = 16; // 약 60fps
            tmAnimationTimer.Tick += TmAnimationTimer_Tick;
            tmAnimationTimer.Start();
            lastAnimationUpdate = DateTime.Now;
            
            // 초기에는 모든 버튼 비활성화
            ActivateButtons(false);
            
            // FOUP 상태 초기화
            UpdateFoupADisplay();
            UpdateFoupBDisplay();
        }

        /// <summary>
        /// 타이머 틱 이벤트 - 공정 업데이트
        /// </summary>
        private void ProcessTimer_Tick(object sender, EventArgs e)
        {
            // processModuleA 업데이트 (1초씩 증가)
            if (processModuleA.ModuleState == ProcessModule.State.Running)
            {
                processModuleA.UpdateProcess(1);
                
                // 진행 상황 업데이트
                UpdateProcessDisplay();
                
                // 공정이 완료되었는지 확인
                if (processModuleA.ModuleState == ProcessModule.State.Idle)
                {
                    MessageBox.Show("공정 A가 완료되었습니다!", "공정 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// 공정 진행 상황을 화면에 표시
        /// </summary>
        public void UpdateProcessDisplay()
        {
            // PM1(A챔버) 상태 업데이트
            lblPM1Status.Text = processModuleA.ModuleState.ToString();
            
            // 진행률 계산 및 표시
            if (processModuleA.processTime > 0)
            {
                int progress = (int)((double)processModuleA.elapsedTime / processModuleA.processTime * 100);
                progressBarPM1.Value = Math.Min(progress, 100);
                lblPM1Progress.Text = $"{processModuleA.elapsedTime}s / {processModuleA.processTime}s";
            }
            
            // 상태에 따른 아이콘 변경
            switch (processModuleA.ModuleState)
            {
                case ProcessModule.State.Idle:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusGray;
                    picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOff;
                    break;
                case ProcessModule.State.Running:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusGreen;
                    picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOn;
                    break;
                case ProcessModule.State.Paused:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusYellow;
                    picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOn;
                    break;
                case ProcessModule.State.Error:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusRed;
                    picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOn;
                    break;
            }
        }

        /// <summary>
        /// 로그인 상태에 따라 모든 버튼을 활성화/비활성화
        /// </summary>
        public void ActivateButtons(bool activate)
        {
            btnFoupALoadSW.Enabled = activate;
            buttonFoupAUnloadSW.Enabled = activate;
            btnFoupBLoadSW.Enabled = activate;
            btnFoupBUnloadSW.Enabled = activate;
            btnRecipeA.Enabled = activate;
            btnRecipeB.Enabled = activate;
            btnRecipeC.Enabled = activate;
            btnAllProcess.Enabled = activate;
            btnAllStop.Enabled = activate;
        }

        /// <summary>
        /// 공정이 진행 중인지 확인
        /// </summary>
        public bool IsProcessRunning()
        {
            return processModuleA.ModuleState == ProcessModule.State.Running ||
                   processModuleA.ModuleState == ProcessModule.State.Paused;
        }

        private void LoadSW()
        {

        }

        private void btnRecipeA_Click(object sender, EventArgs e)
        {
            ShowRecipeTimeSettingDialog("A", processModuleA);
        }

        private void btnRecipeB_Click(object sender, EventArgs e)
        {
            ShowRecipeTimeSettingDialog("B", processModuleB);
        }

        private void btnRecipeC_Click(object sender, EventArgs e)
        {
            ShowRecipeTimeSettingDialog("C", processModuleC);
        }

        /// <summary>
        /// 레시피 공정 시간 설정 다이얼로그 표시
        /// </summary>
        private void ShowRecipeTimeSettingDialog(string recipeName, ProcessModule module)
        {
            // 현재 설정된 시간 가져오기
            int currentTime = module.processTime;

            // 입력 다이얼로그 생성
            Form prompt = new Form()
            {
                Width = 450,
                Height = 250,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = $"Recipe {recipeName} 공정 시간 설정",
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                Font = new Font("나눔고딕", 10F, FontStyle.Regular)
            };

            Label textLabel = new Label() 
            { 
                Left = 30, 
                Top = 20, 
                Width = 380,
                Height = 60,
                Text = $"공정 시간을 입력하세요 (초 단위):\n현재 설정: {currentTime}초",
                Font = new Font("나눔고딕", 11F, FontStyle.Regular)
            };
            
            NumericUpDown numericInput = new NumericUpDown() 
            { 
                Left = 30, 
                Top = 90, 
                Width = 380,
                Height = 30,
                Minimum = 1,
                Maximum = 3600,  // 최대 1시간
                Value = currentTime,
                DecimalPlaces = 0,
                Font = new Font("나눔고딕", 12F, FontStyle.Regular)
            };

            Button confirmation = new Button() 
            { 
                Text = "확인", 
                Left = 210, 
                Width = 90, 
                Height = 40,
                Top = 140, 
                DialogResult = DialogResult.OK,
                Font = new Font("나눔고딕", 10F, FontStyle.Bold)
            };
            
            Button cancellation = new Button() 
            { 
                Text = "취소", 
                Left = 310, 
                Width = 90, 
                Height = 40,
                Top = 140, 
                DialogResult = DialogResult.Cancel,
                Font = new Font("나눔고딕", 10F, FontStyle.Bold)
            };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancellation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(numericInput);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancellation);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancellation;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                int newTime = (int)numericInput.Value;
                module.processTime = newTime;  // 객체의 processTime 속성 직접 수정
                MessageBox.Show(
                    $"Recipe {recipeName}의 공정 시간이 {newTime}초로 설정되었습니다.",
                    "설정 완료",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private async void btnAllProcess_Click(object sender, EventArgs e)
        {
            // 명령 큐가 실행 중인지 확인
            if (commandQueue.IsExecuting)
            {
                MessageBox.Show("명령이 실행 중입니다. 잠시 후 다시 시도해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 공정 시작 전 FOUP 상태 확인
            if (processModuleA.ModuleState == ProcessModule.State.Idle || 
                processModuleA.ModuleState == ProcessModule.State.Stoped)
            {
                // FOUP A가 비어있는지 확인
                if (foupA.IsEmpty)
                {
                    MessageBox.Show(
                        "FOUP A에 웨이퍼가 로드되지 않았습니다.\n공정을 시작하려면 먼저 웨이퍼를 로드해주세요.",
                        "공정 시작 불가",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // FOUP B가 가득 차있는지 확인
                if (foupB.IsFull)
                {
                    MessageBox.Show(
                        "FOUP B가 가득 찼습니다.\n공정을 시작하려면 먼저 FOUP B의 웨이퍼를 언로드해주세요.",
                        "공정 시작 불가",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // 웨이퍼 이송 및 공정 시작 시퀀스 실행
                await StartWaferTransferAndProcess();
            }
            else if (processModuleA.ModuleState == ProcessModule.State.Paused)
            {
                processModuleA.ResumeProcess();
                MessageBox.Show("공정 A가 재개되었습니다.", "공정 재개", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateProcessDisplay();
            }
            else
            {
                MessageBox.Show("공정이 이미 진행 중입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 웨이퍼 이송 및 공정 시작 시퀀스
        /// </summary>
        private async Task StartWaferTransferAndProcess()
        {
            // 맨 위 웨이퍼 슬롯 찾기
            int waferSlot = GetTopWaferSlotFromFoupA();
            if (waferSlot < 0)
            {
                MessageBox.Show("FOUP A에 웨이퍼가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 명령 큐 초기화
            commandQueue.Clear();

            // 1. FOUP A로 이동 (-50도)
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "FOUP A로 회전", -50f));
            
            // 2. 암 확장
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));
            
            // 3. 웨이퍼 픽업
            commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));
            
            // 4. FOUP A에서 웨이퍼 제거
            commandQueue.Enqueue(new ProcessCommand(CommandType.RemoveWaferFromFoup, "FOUP A 웨이퍼 제거", foupA, waferSlot));
            
            // 5. 암 수축
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));
            
            // 6. PM1로 회전 (0도)
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM1로 회전", 0f));
            
            // 7. 암 확장
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));
            
            // 8. 웨이퍼 배치
            commandQueue.Enqueue(new ProcessCommand(CommandType.PlaceWafer, "웨이퍼 배치"));
            
            // 9. 암 수축
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));
            
            // 10. 공정 시작
            commandQueue.Enqueue(new ProcessCommand(CommandType.StartProcess, "공정 시작", processModuleA));

            // 명령 큐 실행
            await commandQueue.ExecuteAsync();

            // 공정 타이머 시작
            processTimer.Start();
            UpdateProcessDisplay();
            
            MessageBox.Show($"공정 A가 시작되었습니다.\n공정 시간: {processModuleA.processTime}초", 
                "공정 시작", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAllStop_Click(object sender, EventArgs e)
        {
            // processModuleA 공정 중지
            if (processModuleA.ModuleState == ProcessModule.State.Running || 
                processModuleA.ModuleState == ProcessModule.State.Paused)
            {
                processModuleA.StopProcess();
                progressBarPM1.Value = 0;
                UpdateProcessDisplay();
                MessageBox.Show("공정 A가 중지되었습니다.", "공정 중지", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnFoupALoadSW_Click(object sender, EventArgs e)
        {
            if (foupA.IsFull)
            {
                MessageBox.Show("FOUP A가 이미 가득 찼습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 모든 웨이퍼 장착
            foupA.LoadWafers();
            UpdateFoupADisplay();
            MessageBox.Show($"FOUP A에 웨이퍼 {foupA.WaferCount}개가 장착되었습니다.", "장착 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonFoupAUnloadSW_Click(object sender, EventArgs e)
        {
            if (foupA.IsEmpty)
            {
                MessageBox.Show("FOUP A가 이미 비어있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 모든 웨이퍼 제거
            foupA.UnloadWafers();
            UpdateFoupADisplay();
            MessageBox.Show("FOUP A의 모든 웨이퍼가 제거되었습니다.", "제거 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnFoupBLoadSW_Click(object sender, EventArgs e)
        {
            if (foupB.IsFull)
            {
                MessageBox.Show("FOUP B가 이미 가득 찼습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 모든 웨이퍼 장착
            foupB.LoadWafers();
            UpdateFoupBDisplay();
            MessageBox.Show($"FOUP B에 웨이퍼 {foupB.WaferCount}개가 장착되었습니다.", "장착 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnFoupBUnloadSW_Click(object sender, EventArgs e)
        {
            if (foupB.IsEmpty)
            {
                MessageBox.Show("FOUP B가 이미 비어있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 모든 웨이퍼 제거
            foupB.UnloadWafers();
            UpdateFoupBDisplay();
            MessageBox.Show("FOUP B의 모든 웨이퍼가 제거되었습니다.", "제거 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// FOUP A 웨이퍼 상태를 화면에 표시
        /// </summary>
        private void UpdateFoupADisplay()
        {
            // 웨이퍼 슬롯 1~5 색상 업데이트
            panelFoupAWafer1.BackColor = foupA.WaferSlots[0] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupAWafer2.BackColor = foupA.WaferSlots[1] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupAWafer3.BackColor = foupA.WaferSlots[2] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupAWafer4.BackColor = foupA.WaferSlots[3] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupAWafer5.BackColor = foupA.WaferSlots[4] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
        }

        /// <summary>
        /// FOUP B 웨이퍼 상태를 화면에 표시
        /// </summary>
        private void UpdateFoupBDisplay()
        {
            // 웨이퍼 슬롯 1~5 색상 업데이트
            panelFoupBWafer1.BackColor = foupB.WaferSlots[0] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupBWafer2.BackColor = foupB.WaferSlots[1] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupBWafer3.BackColor = foupB.WaferSlots[2] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupBWafer4.BackColor = foupB.WaferSlots[3] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupBWafer5.BackColor = foupB.WaferSlots[4] ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
        }

        /// <summary>
        /// 모든 FOUP 디스플레이 업데이트 (CommandQueue에서 호출)
        /// </summary>
        public void UpdateFoupDisplays()
        {
            UpdateFoupADisplay();
            UpdateFoupBDisplay();
        }

        /// <summary>
        /// FOUP A에서 맨 위 웨이퍼 슬롯 인덱스 찾기
        /// </summary>
        private int GetTopWaferSlotFromFoupA()
        {
            // 위에서부터 확인 (인덱스 0 -> 4)
            // panelFoupAWafer1이 맨 위(인덱스 0)
            for (int i = 0; i < 5; i++)
            {
                if (foupA.WaferSlots[i])
                {
                    return i;
                }
            }
            return -1; // 웨이퍼 없음
        }

        #region Transfer Module 관련 메서드

        /// <summary>
        /// Transfer Module 초기화 - 실제 컨트롤의 위치 정보 저장
        /// </summary>
        private void InitializeTransferModule()
        {
            // 기본 위치 설정
            transferModule.SetInitialPositions(
                new Point(234, 237),  // ArmHigh 위치
                new Point(261, 237),  // ArmLow 위치
                new Point(287, 211),  // Bottom 위치 (중심)
                new Point(472, 237)   // Back 위치
            );
        }

        /// <summary>
        /// TM 그래픽 패널 생성 및 panelMainControl에 추가
        /// </summary>
        private void CreateTMGraphicsPanel()
        {
            tmGraphicsPanel = new TMGraphicsPanel
            {
                TransferModule = transferModule,
                Location = new Point(290, 210),
                Size = new Size(450, 450),
                BackColor = Color.Transparent
            };

            // panelMainControl에 추가
            if (panelMainControl != null)
            {
                panelMainControl.Controls.Add(tmGraphicsPanel);
                tmGraphicsPanel.BringToFront();
            }
        }

        /// <summary>
        /// TM 애니메이션 타이머 틱 이벤트
        /// </summary>
        private void TmAnimationTimer_Tick(object sender, EventArgs e)
        {
            // 경과 시간 계산
            DateTime now = DateTime.Now;
            float deltaTime = (float)(now - lastAnimationUpdate).TotalSeconds;
            lastAnimationUpdate = now;

            // TransferModule 업데이트
            bool isAnimating = transferModule.Update(deltaTime);

            // 애니메이션 중이면 그래픽 업데이트
            if (isAnimating || transferModule.State != TransferModule.TMState.Idle)
            {
                UpdateTransferModuleGraphics();
            }
        }

        /// <summary>
        /// TransferModule의 위치 정보를 실제 그래픽 컴포넌트에 반영
        /// </summary>
        private void UpdateTransferModuleGraphics()
        {
            // TMGraphicsPanel 다시 그리기
            if (tmGraphicsPanel != null)
            {
                tmGraphicsPanel.Invalidate();
            }
        }

        /// <summary>
        /// TM 회전 (애니메이션)
        /// </summary>
        public void RotateTM(float angle)
        {
            transferModule.SetTargetRotation(angle);
        }

        /// <summary>
        /// TM 회전 (즉시)
        /// </summary>
        public void RotateTMImmediate(float angle)
        {
            transferModule.RotateImmediate(angle);
            UpdateTransferModuleGraphics();
        }

        /// <summary>
        /// TM Arm 확장 (최대 거리)
        /// </summary>
        public void ExtendTMArm()
        {
            transferModule.ExtendArm();
        }

        /// <summary>
        /// TM Arm 수축 (원위치)
        /// </summary>
        public void RetractTMArm()
        {
            transferModule.RetractArm();
        }

        /// <summary>
        /// TM Arm을 특정 거리만큼 확장
        /// </summary>
        public void ExtendTMArmTo(float distance)
        {
            transferModule.ExtendArmTo(distance);
        }

        /// <summary>
        /// TM으로 웨이퍼 픽업
        /// </summary>
        public bool TMPickWafer()
        {
            return transferModule.PickWafer();
        }

        /// <summary>
        /// TM으로 웨이퍼 배치
        /// </summary>
        public bool TMPlaceWafer()
        {
            return transferModule.PlaceWafer();
        }

        /// <summary>
        /// TM 상태 확인
        /// </summary>
        public TransferModule.TMState GetTMState()
        {
            return transferModule.State;
        }

        /// <summary>
        /// TM이 웨이퍼를 가지고 있는지 확인
        /// </summary>
        public bool TMHasWafer()
        {
            return transferModule.HasWafer;
        }

        #endregion
    }
}
