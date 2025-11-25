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
        // TM 회전 각도 상수
        private const float ANGLE_FOUP_A = -50f;
        private const float ANGLE_PM1 = 0f;
        private const float ANGLE_PM2 = 90f;
        private const float ANGLE_PM3 = 180f;
        private const float ANGLE_FOUP_B = 230f; // 또는 -130f

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

        // 램프 점멸용 타이머
        private Timer lampBlinkTimer;
        private bool lampBlinkState;

        // 워크플로우 취소 플래그
        private bool isWorkflowCancelled;

        public MainView()
        {
            InitializeComponent();

            // 각 공정용 ProcessModule 객체 생성 (기본값: A=60초, B=120초, C=180초)
            processModuleA = new ProcessModule(ProcessModule.ModuleType.PM1, 60);
            processModuleB = new ProcessModule(ProcessModule.ModuleType.PM2, 120);
            processModuleC = new ProcessModule(ProcessModule.ModuleType.PM3, 180);
            
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

            // 램프 점멸 타이머 초기화 (500ms 간격)
            lampBlinkTimer = new Timer();
            lampBlinkTimer.Interval = 500; // 0.5초마다 점멸
            lampBlinkTimer.Tick += LampBlinkTimer_Tick;
            lampBlinkTimer.Start();
            lampBlinkState = false;

            // 워크플로우 취소 플래그 초기화
            isWorkflowCancelled = false;

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
            // processModuleA (PM1) 업데이트 (1초씩 증가)
            if (processModuleA.ModuleState == ProcessModule.State.Running)
            {
                processModuleA.UpdateProcess(1);

                // 진행 상황 업데이트
                UpdateProcessDisplay();

                // 공정 완료 알림은 제거 (자동 워크플로우에서 자동 처리됨)
            }

            // processModuleC (PM3) 업데이트 (1초씩 증가)
            if (processModuleC.ModuleState == ProcessModule.State.Running)
            {
                processModuleC.UpdateProcess(1);

                // 진행 상황 업데이트
                UpdateProcessDisplay();

                // 공정 완료 알림은 제거 (자동 워크플로우에서 자동 처리됨)
            }
        }

        /// <summary>
        /// 공정 진행 상황을 화면에 표시
        /// </summary>
        public void UpdateProcessDisplay()
        {
            // PM1(A챔버) 상태 업데이트
            UpdatePM1Display();

            // PM3(C챔버) 상태 업데이트
            UpdatePM3Display();
        }

        /// <summary>
        /// PM1 상태를 화면에 표시
        /// </summary>
        private void UpdatePM1Display()
        {
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
                    // 언로드 요청이 있으면 램프는 점멸로 처리 (4단계에서 구현)
                    if (!processModuleA.IsUnloadRequested)
                    {
                        picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOff;
                    }
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
        /// PM3 상태를 화면에 표시
        /// </summary>
        private void UpdatePM3Display()
        {
            lblPM3Status.Text = processModuleC.ModuleState.ToString();

            // 진행률 계산 및 표시
            if (processModuleC.processTime > 0)
            {
                int progress = (int)((double)processModuleC.elapsedTime / processModuleC.processTime * 100);
                progressBarPM3.Value = Math.Min(progress, 100);
                lblPM3Progress.Text = $"{processModuleC.elapsedTime}s / {processModuleC.processTime}s";
            }

            // 상태에 따른 아이콘 변경
            switch (processModuleC.ModuleState)
            {
                case ProcessModule.State.Idle:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusGray;
                    // 언로드 요청이 있으면 램프는 점멸로 처리 (4단계에서 구현)
                    if (!processModuleC.IsUnloadRequested)
                    {
                        picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOff;
                    }
                    break;
                case ProcessModule.State.Running:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusGreen;
                    picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOn;
                    break;
                case ProcessModule.State.Paused:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusYellow;
                    picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOn;
                    break;
                case ProcessModule.State.Error:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusRed;
                    picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOn;
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

                // 전체 자동 워크플로우 시작
                await StartFullAutomatedWorkflow();
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
        /// 전체 5개 웨이퍼 자동 처리 워크플로우
        /// PM1(이온 주입) → PM3(어닐링) → FOUP B
        /// </summary>
        private async Task StartFullAutomatedWorkflow()
        {
            int totalWafers = foupA.WaferCount;
            if (totalWafers == 0)
            {
                MessageBox.Show("FOUP A에 웨이퍼가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 워크플로우 취소 플래그 초기화
            isWorkflowCancelled = false;

            MessageBox.Show($"전체 자동 공정을 시작합니다.\n처리할 웨이퍼: {totalWafers}개\n\nPM1(이온 주입) → PM3(어닐링) → FOUP B",
                "자동 공정 시작", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 공정 타이머 시작
            processTimer.Start();

            try
            {
                // 첫 번째 웨이퍼: FOUP A → PM1
                await TransferWaferFromFoupAToPM1();
                if (isWorkflowCancelled) throw new OperationCanceledException();

                // 남은 웨이퍼 처리
                for (int i = 1; i < totalWafers; i++)
                {
                    // PM1 공정 완료 대기
                    await WaitForPM1Complete();
                    if (isWorkflowCancelled) throw new OperationCanceledException();

                    // PM1 → PM3 이송
                    await TransferWaferFromPM1ToPM3();
                    if (isWorkflowCancelled) throw new OperationCanceledException();

                    // 다음 웨이퍼가 있으면 FOUP A → PM1
                    if (i < totalWafers)
                    {
                        await TransferWaferFromFoupAToPM1();
                        if (isWorkflowCancelled) throw new OperationCanceledException();
                    }

                    // PM3 공정 완료 대기
                    await WaitForPM3Complete();
                    if (isWorkflowCancelled) throw new OperationCanceledException();

                    // PM3 → FOUP B 이송
                    await TransferWaferFromPM3ToFoupB();
                    if (isWorkflowCancelled) throw new OperationCanceledException();
                }

                // 마지막 웨이퍼 처리 (PM1에 남아있음)
                await WaitForPM1Complete();
                if (isWorkflowCancelled) throw new OperationCanceledException();

                await TransferWaferFromPM1ToPM3();
                if (isWorkflowCancelled) throw new OperationCanceledException();

                await WaitForPM3Complete();
                if (isWorkflowCancelled) throw new OperationCanceledException();

                await TransferWaferFromPM3ToFoupB();
                if (isWorkflowCancelled) throw new OperationCanceledException();

                MessageBox.Show($"전체 공정이 완료되었습니다!\n처리된 웨이퍼: {totalWafers}개",
                    "공정 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                // 워크플로우가 중단됨 (Stop 버튼에서 이미 메시지를 표시했으므로 여기서는 표시하지 않음)
            }
        }

        /// <summary>
        /// FOUP A → PM1 웨이퍼 이송 및 공정 시작
        /// </summary>
        private async Task TransferWaferFromFoupAToPM1()
        {
            int waferSlot = GetTopWaferSlotFromFoupA();
            if (waferSlot < 0)
            {
                return;
            }

            commandQueue.Clear();

            // 1. FOUP A로 이동
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "FOUP A로 회전", ANGLE_FOUP_A));
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));

            // 2. FOUP A에서 웨이퍼 제거 및 픽업
            commandQueue.Enqueue(new ProcessCommand(CommandType.RemoveWaferFromFoup, "FOUP A 웨이퍼 제거", foupA, waferSlot));
            commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

            // 3. PM1로 이동 및 배치
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM1로 회전", ANGLE_PM1));
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));
            commandQueue.Enqueue(new ProcessCommand(CommandType.PlaceWafer, "웨이퍼 배치", processModuleA));
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

            // 4. PM1 공정 시작
            commandQueue.Enqueue(new ProcessCommand(CommandType.StartProcess, "PM1 공정 시작", processModuleA));

            await commandQueue.ExecuteAsync();
        }

        /// <summary>
        /// PM1 → PM3 웨이퍼 이송 및 공정 시작
        /// </summary>
        private async Task TransferWaferFromPM1ToPM3()
        {
            commandQueue.Clear();

            // 1. PM1로 이동
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM1로 회전", ANGLE_PM1));
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));

            // 2. PM1에서 웨이퍼 언로드 및 픽업
            commandQueue.Enqueue(new ProcessCommand(CommandType.UnloadWaferFromPM, "PM1 웨이퍼 언로드", processModuleA));
            commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

            // 3. PM3로 이동 및 배치
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM3로 회전", ANGLE_PM3));
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));
            commandQueue.Enqueue(new ProcessCommand(CommandType.PlaceWafer, "웨이퍼 배치", processModuleC));
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

            // 4. PM3 공정 시작
            commandQueue.Enqueue(new ProcessCommand(CommandType.StartProcess, "PM3 공정 시작", processModuleC));

            await commandQueue.ExecuteAsync();
        }

        /// <summary>
        /// PM3 → FOUP B 웨이퍼 이송
        /// </summary>
        private async Task TransferWaferFromPM3ToFoupB()
        {
            int emptySlot = GetBottomEmptySlotFromFoupB();
            if (emptySlot < 0)
            {
                MessageBox.Show("FOUP B가 가득 찼습니다!", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            commandQueue.Clear();

            // 1. PM3로 이동
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM3로 회전", ANGLE_PM3));
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));

            // 2. PM3에서 웨이퍼 언로드 및 픽업
            commandQueue.Enqueue(new ProcessCommand(CommandType.UnloadWaferFromPM, "PM3 웨이퍼 언로드", processModuleC));
            commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

            // 3. FOUP B로 이동 및 배치
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "FOUP B로 회전", ANGLE_FOUP_B));
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));
            commandQueue.Enqueue(new ProcessCommand(CommandType.AddWaferToFoup, "FOUP B에 웨이퍼 추가", foupB, emptySlot));
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

            await commandQueue.ExecuteAsync();
        }

        /// <summary>
        /// PM1 공정 완료 대기
        /// </summary>
        private async Task WaitForPM1Complete()
        {
            while (processModuleA.ModuleState == ProcessModule.State.Running ||
                   (processModuleA.ModuleState == ProcessModule.State.Idle && !processModuleA.IsUnloadRequested))
            {
                // 워크플로우가 취소되면 즉시 종료
                if (isWorkflowCancelled)
                {
                    throw new OperationCanceledException();
                }

                await Task.Delay(500);
            }

            // 공정이 Stopped 상태인 경우도 취소로 처리
            if (processModuleA.ModuleState == ProcessModule.State.Stoped)
            {
                throw new OperationCanceledException();
            }
        }

        /// <summary>
        /// PM3 공정 완료 대기
        /// </summary>
        private async Task WaitForPM3Complete()
        {
            while (processModuleC.ModuleState == ProcessModule.State.Running ||
                   (processModuleC.ModuleState == ProcessModule.State.Idle && !processModuleC.IsUnloadRequested))
            {
                // 워크플로우가 취소되면 즉시 종료
                if (isWorkflowCancelled)
                {
                    throw new OperationCanceledException();
                }

                await Task.Delay(500);
            }

            // 공정이 Stopped 상태인 경우도 취소로 처리
            if (processModuleC.ModuleState == ProcessModule.State.Stoped)
            {
                throw new OperationCanceledException();
            }
        }

        /// <summary>
        /// 웨이퍼 이송 및 공정 시작 시퀀스 (기존 단일 웨이퍼 테스트용)
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

            // 1. FOUP A로 이동
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "FOUP A로 회전", ANGLE_FOUP_A));

            // 2. 암 확장
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));

            // 3. FOUP A에서 웨이퍼 제거 (ResultWafer에 저장됨)
            commandQueue.Enqueue(new ProcessCommand(CommandType.RemoveWaferFromFoup, "FOUP A 웨이퍼 제거", foupA, waferSlot));

            // 4. 웨이퍼 픽업 (이전 명령의 ResultWafer 사용)
            commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));

            // 5. 암 수축
            commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

            // 6. PM1로 회전
            commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM1로 회전", ANGLE_PM1));

            // 7. 암 확장
            commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));

            // 8. 웨이퍼 배치 (PM1에 로드됨)
            commandQueue.Enqueue(new ProcessCommand(CommandType.PlaceWafer, "웨이퍼 배치", processModuleA));

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

        private async void btnAllStop_Click(object sender, EventArgs e)
        {
            // 워크플로우 취소 플래그 설정
            isWorkflowCancelled = true;

            bool anyStopped = false;
            bool workflowWasRunning = commandQueue.IsExecuting;

            // processModuleA (PM1) 공정 중지 및 초기화
            if (processModuleA.ModuleState != ProcessModule.State.Idle)
            {
                processModuleA.StopProcess();
                processModuleA.ModuleState = ProcessModule.State.Idle;
                processModuleA.elapsedTime = 0;
                processModuleA.IsUnloadRequested = false;
                progressBarPM1.Value = 0;
                anyStopped = true;
            }

            // processModuleC (PM3) 공정 중지 및 초기화
            if (processModuleC.ModuleState != ProcessModule.State.Idle)
            {
                processModuleC.StopProcess();
                processModuleC.ModuleState = ProcessModule.State.Idle;
                processModuleC.elapsedTime = 0;
                processModuleC.IsUnloadRequested = false;
                progressBarPM3.Value = 0;
                anyStopped = true;
            }

            // 명령 큐 중지 및 초기화
            commandQueue.Stop();

            UpdateProcessDisplay();

            // 워크플로우 진행 중이었거나 공정이 실행 중이었으면 메시지 표시
            if (anyStopped || workflowWasRunning)
            {
                MessageBox.Show("실행 중인 모든 공정과 워크플로우가 중지 및 초기화되었습니다.",
                    "공정 중지", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("실행 중인 공정이 없습니다.",
                    "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Stop 버튼을 누른 후에는 다시 시작할 수 있도록 플래그 리셋
            // (약간의 지연 후 리셋하여 취소 처리가 완료되도록 함)
            await Task.Delay(100);
            isWorkflowCancelled = false;
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
            panelFoupAWafer1.BackColor = foupA.WaferSlots[0] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupAWafer2.BackColor = foupA.WaferSlots[1] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupAWafer3.BackColor = foupA.WaferSlots[2] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupAWafer4.BackColor = foupA.WaferSlots[3] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupAWafer5.BackColor = foupA.WaferSlots[4] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
        }

        /// <summary>
        /// FOUP B 웨이퍼 상태를 화면에 표시
        /// </summary>
        private void UpdateFoupBDisplay()
        {
            // 웨이퍼 슬롯 1~5 색상 업데이트
            panelFoupBWafer1.BackColor = foupB.WaferSlots[0] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupBWafer2.BackColor = foupB.WaferSlots[1] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupBWafer3.BackColor = foupB.WaferSlots[2] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupBWafer4.BackColor = foupB.WaferSlots[3] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
            panelFoupBWafer5.BackColor = foupB.WaferSlots[4] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64);
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
                if (foupA.WaferSlots[i] != null)
                {
                    return i;
                }
            }
            return -1; // 웨이퍼 없음
        }

        /// <summary>
        /// FOUP B에서 맨 아래 빈 슬롯 인덱스 찾기
        /// </summary>
        private int GetBottomEmptySlotFromFoupB()
        {
            // 아래에서부터 확인 (인덱스 4 -> 0)
            // panelFoupBWafer5가 맨 아래(인덱스 4)
            for (int i = 4; i >= 0; i--)
            {
                if (foupB.WaferSlots[i] == null)
                {
                    return i;
                }
            }
            return -1; // 빈 슬롯 없음
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
        /// 램프 점멸 타이머 틱 이벤트
        /// </summary>
        private void LampBlinkTimer_Tick(object sender, EventArgs e)
        {
            // 점멸 상태 토글
            lampBlinkState = !lampBlinkState;

            // PM1 램프 점멸 처리
            if (processModuleA.IsUnloadRequested && processModuleA.ModuleState == ProcessModule.State.Idle)
            {
                picBoxPM1Lamp.BackgroundImage = lampBlinkState ? Properties.Resources.LampOn : Properties.Resources.LampOff;
            }

            // PM3 램프 점멸 처리
            if (processModuleC.IsUnloadRequested && processModuleC.ModuleState == ProcessModule.State.Idle)
            {
                picBoxPM3Lamp.BackgroundImage = lampBlinkState ? Properties.Resources.LampOn : Properties.Resources.LampOff;
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
        /// TM으로 웨이퍼 픽업 (Wafer 객체 전달)
        /// </summary>
        public bool TMPickWafer(Wafer wafer)
        {
            return transferModule.PickWafer(wafer);
        }

        /// <summary>
        /// TM으로 웨이퍼 배치 및 반환
        /// </summary>
        public Wafer TMPlaceWafer()
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

        #region Process Module 접근 메서드

        /// <summary>
        /// ProcessModule A (PM1) 반환
        /// </summary>
        public ProcessModule GetProcessModuleA()
        {
            return processModuleA;
        }

        /// <summary>
        /// ProcessModule C (PM3) 반환
        /// </summary>
        public ProcessModule GetProcessModuleC()
        {
            return processModuleC;
        }

        /// <summary>
        /// FOUP A 반환
        /// </summary>
        public Foup GetFoupA()
        {
            return foupA;
        }

        /// <summary>
        /// FOUP B 반환
        /// </summary>
        public Foup GetFoupB()
        {
            return foupB;
        }

        #endregion
    }
}
