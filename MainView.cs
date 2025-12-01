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

        // Stop 진행 중 플래그 (버튼 비활성화용)
        private bool isStopping;

        // EtherCAT 컨트롤러 참조
        private IEtherCATController etherCATController;

        // RecipeView 참조
        private RecipeView recipeView;

        // 타워 램프 상태
        private TowerLampState currentTowerLampState = TowerLampState.Off;

        public MainView()
        {
            InitializeComponent();

            // 각 공정용 ProcessModule 객체 생성 (기본값: 모두 30초)
            processModuleA = new ProcessModule(ProcessModule.ModuleType.PM1, 30);
            processModuleB = new ProcessModule(ProcessModule.ModuleType.PM2, 30);
            processModuleC = new ProcessModule(ProcessModule.ModuleType.PM3, 30);
            
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
            
            // 타이머 초기화 (1초마다 실행) - 파라미터 애니메이션을 위해 항상 실행
            processTimer = new Timer();
            processTimer.Interval = 1000; // 1초
            processTimer.Tick += ProcessTimer_Tick;
            processTimer.Start(); // 파라미터 애니메이션을 위해 항상 시작

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

            // Stop 진행 중 플래그 초기화
            isStopping = false;

            // 초기에는 모든 버튼 비활성화
            ActivateButtons(false);

            // FOUP 상태 초기화
            UpdateFoupADisplay();
            UpdateFoupBDisplay();
        }

        /// <summary>
        /// 타이머 틱 이벤트 - 공정 업데이트 및 파라미터 애니메이션
        /// </summary>
        private void ProcessTimer_Tick(object sender, EventArgs e)
        {
            // processModuleA (PM1) 업데이트
            if (processModuleA.ModuleState == ProcessModule.State.Running)
            {
                processModuleA.UpdateProcess(1);
                UpdateProcessDisplay();
            }
            else
            {
                // Idle/Stopped 상태에서 파라미터 하강 애니메이션
                processModuleA.UpdateParametersWhenIdle();
            }

            // processModuleB (PM2) 업데이트
            if (processModuleB.ModuleState == ProcessModule.State.Running)
            {
                processModuleB.UpdateProcess(1);
                UpdateProcessDisplay();
            }
            else
            {
                // Idle/Stopped 상태에서 파라미터 하강 애니메이션
                processModuleB.UpdateParametersWhenIdle();
            }

            // processModuleC (PM3) 업데이트
            if (processModuleC.ModuleState == ProcessModule.State.Running)
            {
                processModuleC.UpdateProcess(1);
                UpdateProcessDisplay();
            }
            else
            {
                // Idle/Stopped 상태에서 파라미터 하강 애니메이션
                processModuleC.UpdateParametersWhenIdle();
            }

            // 모든 PM 파라미터 표시 업데이트 (항상 실행 - 애니메이션용)
            UpdateAllPMParameters();
        }

        /// <summary>
        /// 공정 진행 상황을 화면에 표시
        /// </summary>
        public void UpdateProcessDisplay()
        {
            // PM1(A챔버) 상태 업데이트
            UpdatePM1Display();

            // PM2(B챔버) 상태 업데이트
            UpdatePM2Display();

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

            // 상태에 따른 아이콘 변경 및 실제 램프 제어
            switch (processModuleA.ModuleState)
            {
                case ProcessModule.State.Idle:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusGray;
                    // 언로드 요청이 있으면 램프는 점멸로 처리 (타이머에서 처리)
                    if (!processModuleA.IsUnloadRequested)
                    {
                        picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOff;
                        etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM1, false);
                    }
                    break;
                case ProcessModule.State.Running:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusGreen;
                    picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM1, true);
                    break;
                case ProcessModule.State.Paused:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusYellow;
                    picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM1, true);
                    break;
                case ProcessModule.State.Error:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusRed;
                    picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM1, true);
                    break;
            }
        }

        /// <summary>
        /// PM2 상태를 화면에 표시
        /// </summary>
        private void UpdatePM2Display()
        {
            lblPM2Status.Text = processModuleB.ModuleState.ToString();

            // 진행률 계산 및 표시
            if (processModuleB.processTime > 0)
            {
                int progress = (int)((double)processModuleB.elapsedTime / processModuleB.processTime * 100);
                progressBarPM2.Value = Math.Min(progress, 100);
                lblPM2Progress.Text = $"{processModuleB.elapsedTime}s / {processModuleB.processTime}s";
            }

            // 상태에 따른 아이콘 변경 및 실제 램프 제어
            switch (processModuleB.ModuleState)
            {
                case ProcessModule.State.Idle:
                    picBoxPM2Status.BackgroundImage = Properties.Resources.StatusGray;
                    // 언로드 요청이 있으면 램프는 점멸로 처리 (타이머에서 처리)
                    if (!processModuleB.IsUnloadRequested)
                    {
                        picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOff;
                        etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM2, false);
                    }
                    break;
                case ProcessModule.State.Running:
                    picBoxPM2Status.BackgroundImage = Properties.Resources.StatusGreen;
                    picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM2, true);
                    break;
                case ProcessModule.State.Paused:
                    picBoxPM2Status.BackgroundImage = Properties.Resources.StatusYellow;
                    picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM2, true);
                    break;
                case ProcessModule.State.Error:
                    picBoxPM2Status.BackgroundImage = Properties.Resources.StatusRed;
                    picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM2, true);
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

            // 상태에 따른 아이콘 변경 및 실제 램프 제어
            switch (processModuleC.ModuleState)
            {
                case ProcessModule.State.Idle:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusGray;
                    // 언로드 요청이 있으면 램프는 점멸로 처리 (타이머에서 처리)
                    if (!processModuleC.IsUnloadRequested)
                    {
                        picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOff;
                        etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM3, false);
                    }
                    break;
                case ProcessModule.State.Running:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusGreen;
                    picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM3, true);
                    break;
                case ProcessModule.State.Paused:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusYellow;
                    picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM3, true);
                    break;
                case ProcessModule.State.Error:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusRed;
                    picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOn;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM3, true);
                    break;
            }
        }

        #region PM 파라미터 표시 업데이트

        /// <summary>
        /// PM1 파라미터 값 표시 업데이트
        /// </summary>
        private void UpdatePM1Parameters()
        {
            var p = processModuleA.Parameters;
            lblPM1TemperatureValue.Text = p.GetTemperatureDisplay();
            lblPM1PressureValue.Text = p.GetPressureDisplay();
            lblPM1AVValue.Text = p.GetHVDisplay();
            lblPM1DoseValue.Text = p.GetDoseDisplay();
        }

        /// <summary>
        /// PM2 파라미터 값 표시 업데이트
        /// </summary>
        private void UpdatePM2Parameters()
        {
            var p = processModuleB.Parameters;
            lblPM2TemperatureValue.Text = p.GetTemperatureDisplay();
            lblPM2PressureValue.Text = p.GetPressureDisplay();
            lblPM2AVValue.Text = p.GetHVDisplay();
            lblPM2DoseValue.Text = p.GetDoseDisplay();
        }

        /// <summary>
        /// PM3 파라미터 값 표시 업데이트 (Temperature, Pressure만)
        /// </summary>
        private void UpdatePM3Parameters()
        {
            var p = processModuleC.Parameters;
            lblTemperatureValue.Text = p.GetTemperatureDisplay();
            lblPressureValue.Text = p.GetPressureDisplay();
        }

        /// <summary>
        /// 모든 PM 파라미터 표시 업데이트
        /// </summary>
        private void UpdateAllPMParameters()
        {
            UpdatePM1Parameters();
            UpdatePM2Parameters();
            UpdatePM3Parameters();
        }

        /// <summary>
        /// 모든 PM 파라미터 표시 초기화
        /// </summary>
        private void ResetPMParametersDisplay()
        {
            lblPM1TemperatureValue.Text = "-";
            lblPM1PressureValue.Text = "-";
            lblPM1AVValue.Text = "-";
            lblPM1DoseValue.Text = "-";

            lblPM2TemperatureValue.Text = "-";
            lblPM2PressureValue.Text = "-";
            lblPM2AVValue.Text = "-";
            lblPM2DoseValue.Text = "-";

            lblTemperatureValue.Text = "-";
            lblPressureValue.Text = "-";
        }

        #endregion

        /// <summary>
        /// 로그인 상태에 따라 모든 버튼을 활성화/비활성화
        /// </summary>
        public void ActivateButtons(bool activate)
        {
            ActivateRecipeButtons(activate);
            ActivateEquipmentButtons(activate);
        }

        /// <summary>
        /// 레시피 설정 버튼 활성화 (로그인만 하면 사용 가능)
        /// </summary>
        public void ActivateRecipeButtons(bool activate)
        {
            btnRecipeA.Enabled = activate;
            btnRecipeB.Enabled = activate;
            btnRecipeC.Enabled = activate;
        }

        /// <summary>
        /// 장비 제어 버튼 활성화 (로그인 + 연결 시 사용 가능)
        /// </summary>
        public void ActivateEquipmentButtons(bool activate)
        {
            btnFoupALoadSW.Enabled = activate;
            buttonFoupAUnloadSW.Enabled = activate;
            btnFoupBLoadSW.Enabled = activate;
            btnFoupBUnloadSW.Enabled = activate;
            btnAllProcess.Enabled = activate;
            btnAllStop.Enabled = activate;

            // 연결 시 황색(대기), 연결 해제 시 OFF
            UpdateTowerLamp(activate ? TowerLampState.Yellow : TowerLampState.Off);
        }

        /// <summary>
        /// 타워 램프 상태 업데이트 (한 번에 하나만 점등)
        /// </summary>
        private void UpdateTowerLamp(TowerLampState state)
        {
            currentTowerLampState = state;

            // UI 색상 업데이트 (점등 시 밝은 색, 소등 시 어두운 색)
            panelGreenAlert.BackColor = (state == TowerLampState.Green) ? Color.Lime : Color.DarkSeaGreen;
            panelYellowAlert.BackColor = (state == TowerLampState.Yellow) ? Color.Yellow : Color.Khaki;
            panelRedAlert.BackColor = (state == TowerLampState.Red) ? Color.Red : Color.RosyBrown;

            // 하드웨어 제어
            etherCATController?.SetTowerLamp(state);
        }

        /// <summary>
        /// 공정이 진행 중인지 확인
        /// </summary>
        public bool IsProcessRunning()
        {
            return processModuleA.ModuleState == ProcessModule.State.Running ||
                   processModuleA.ModuleState == ProcessModule.State.Paused ||
                   processModuleB.ModuleState == ProcessModule.State.Running ||
                   processModuleB.ModuleState == ProcessModule.State.Paused ||
                   processModuleC.ModuleState == ProcessModule.State.Running ||
                   processModuleC.ModuleState == ProcessModule.State.Paused;
        }

        private void LoadSW()
        {

        }

        /// <summary>
        /// RecipeView 참조 설정
        /// </summary>
        public void SetRecipeView(RecipeView view)
        {
            recipeView = view;
        }

        /// <summary>
        /// RecipeView에서 레시피를 가져와 PM에 적용
        /// </summary>
        public void LoadRecipesFromRecipeView()
        {
            if (recipeView == null)
            {
                System.Diagnostics.Debug.WriteLine("RecipeView가 설정되지 않았습니다.");
                return;
            }

            // RecipeView에서 현재 레시피 세트 가져오기
            RecipeSet recipeSet = recipeView.GetCurrentRecipeSet();

            // PM1에 레시피 적용
            processModuleA.IonRecipe = recipeSet.PM1Recipe;
            System.Diagnostics.Debug.WriteLine($"PM1 레시피 로드: Dose={processModuleA.IonRecipe.Dose}, Voltage={processModuleA.IonRecipe.Voltage}");

            // PM2에 레시피 적용
            processModuleB.IonRecipe = recipeSet.PM2Recipe;
            System.Diagnostics.Debug.WriteLine($"PM2 레시피 로드: Dose={processModuleB.IonRecipe.Dose}, Voltage={processModuleB.IonRecipe.Voltage}");

            // PM3에 레시피 적용
            processModuleC.AnnealRecipe = recipeSet.PM3Recipe;
            System.Diagnostics.Debug.WriteLine($"PM3 레시피 로드: Temperature={processModuleC.AnnealRecipe.Temperature}, Vacuum={processModuleC.AnnealRecipe.Vacuum}");
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
        /// 전체 웨이퍼 자동 처리 워크플로우 (상태 기반 이벤트 루프)
        /// TM이 대기하지 않고 항상 다음 작업을 찾아서 수행
        /// </summary>
        private async Task StartFullAutomatedWorkflow()
        {
            // RecipeView에서 최신 레시피 로드
            LoadRecipesFromRecipeView();

            int totalWafers = foupA.WaferCount;
            if (totalWafers == 0)
            {
                MessageBox.Show("FOUP A에 웨이퍼가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 워크플로우 취소 플래그 초기화
            isWorkflowCancelled = false;

            MessageBox.Show($"전체 자동 공정을 시작합니다.\n처리할 웨이퍼: {totalWafers}개\n\nFOUP A → (PM1/PM2 이온 주입) → PM3(어닐링) → FOUP B",
                "자동 공정 시작", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 공정 시작 로그
            LogManager.Instance.AddLog($"자동 공정", $"전체 자동 공정 시작 - 처리할 웨이퍼: {totalWafers}개", "System", LogCategory.Process, false);

            // 실제 모드에서는 서보 모터 ON
            if (IsRealMode())
            {
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOn, "서보 모터 ON"));
                await commandQueue.ExecuteAsync();
            }

            // 공정 타이머 시작
            processTimer.Start();

            // 녹색 램프 ON (공정 진행 중)
            UpdateTowerLamp(TowerLampState.Green);

            try
            {
                // 상태 기반 이벤트 루프
                while (true)
                {
                    // 취소 확인 (명령 큐가 실행 중이지 않을 때만)
                    if (isWorkflowCancelled && !commandQueue.IsExecuting)
                        throw new OperationCanceledException();

                    // 완료 조건: FOUP A 비었고, 모든 PM이 비었고, FOUP B에 모든 웨이퍼가 있음
                    if (foupA.IsEmpty &&
                        !processModuleA.isWaferLoaded &&
                        !processModuleB.isWaferLoaded &&
                        !processModuleC.isWaferLoaded &&
                        foupB.WaferCount == totalWafers)
                    {
                        break; // 완료!
                    }

                    // 1단계: PM3 완료 → FOUP B로 이송
                    if (processModuleC.IsUnloadRequested && processModuleC.isWaferLoaded)
                    {
                        // FOUP B 가득 참 체크
                        if (foupB.IsFull)
                        {
                            // 황색 램프 (지연)
                            UpdateTowerLamp(TowerLampState.Yellow);
                            MessageBox.Show("FOUP B가 가득 찼습니다.\nFOUP B를 언로드한 후 공정이 재개됩니다.",
                                "대기 중", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            // 언로드될 때까지 대기
                            while (foupB.IsFull && !isWorkflowCancelled)
                            {
                                await Task.Delay(500);
                            }

                            if (isWorkflowCancelled && !commandQueue.IsExecuting)
                                throw new OperationCanceledException();

                            // 녹색 램프로 복귀 (공정 재개)
                            UpdateTowerLamp(TowerLampState.Green);
                        }

                        await TransferWaferFromPM3ToFoupB();
                        continue; // 다음 루프
                    }

                    // 2단계: PM3 비었고, 이온주입 완료된 웨이퍼 있으면 → PM3로 이송 (FIFO)
                    if (!processModuleC.isWaferLoaded)
                    {
                        // 먼저 완료된 PM에서 웨이퍼 가져옴
                        ProcessModule completedPM = GetFirstCompletedIonPM();
                        if (completedPM != null)
                        {
                            await TransferWaferFromIonPMToPM3(completedPM);
                            continue;
                        }
                    }

                    // 3단계: PM1 비었고 FOUP A에 웨이퍼 있으면 → PM1에 투입
                    if (!processModuleA.isWaferLoaded && !foupA.IsEmpty)
                    {
                        await TransferWaferFromFoupAToIonPM(processModuleA);
                        continue;
                    }

                    // 4단계: PM2 비었고 FOUP A에 웨이퍼 있으면 → PM2에 투입
                    if (!processModuleB.isWaferLoaded && !foupA.IsEmpty)
                    {
                        await TransferWaferFromFoupAToIonPM(processModuleB);
                        continue;
                    }

                    // 5단계: 할 일이 없으면 잠시 대기 (공정 진행 중)
                    await Task.Delay(500);
                }

                // 실제 모드에서는 안전 종료 시퀀스 실행
                if (IsRealMode())
                {
                    commandQueue.Clear();
                    // 1. 실린더 후진
                    commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));
                    // 2. 흡기 OFF
                    commandQueue.Enqueue(new ProcessCommand(CommandType.DisableSuction, "흡기 OFF"));
                    // 3. 배기 OFF
                    commandQueue.Enqueue(new ProcessCommand(CommandType.DisableExhaust, "배기 OFF"));
                    // 4. UD 원점복귀
                    commandQueue.Enqueue(new ProcessCommand(CommandType.HomeUDAxis, "UD 원점복귀"));
                    // 5. LR 원점복귀
                    commandQueue.Enqueue(new ProcessCommand(CommandType.HomeLRAxis, "LR 원점복귀"));
                    // 6. 서보 모터 OFF
                    commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOff, "서보 모터 OFF"));
                    await commandQueue.ExecuteAsync();
                }

                // 황색 램프 (공정 완료 - 대기)
                UpdateTowerLamp(TowerLampState.Yellow);

                // 공정 완료 로그
                LogManager.Instance.AddLog($"자동 공정", $"전체 자동 공정 완료 - 처리된 웨이퍼: {totalWafers}개", "System", LogCategory.Process, false);

                MessageBox.Show($"전체 공정이 완료되었습니다!\n처리된 웨이퍼: {totalWafers}개",
                    "공정 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                // 워크플로우가 중단됨 - 안전 종료 시퀀스 실행
                if (IsRealMode())
                {
                    commandQueue.Clear();
                    // 1. 실린더 후진
                    commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));
                    // 2. 흡기 OFF
                    commandQueue.Enqueue(new ProcessCommand(CommandType.DisableSuction, "흡기 OFF"));
                    // 3. 배기 OFF
                    commandQueue.Enqueue(new ProcessCommand(CommandType.DisableExhaust, "배기 OFF"));
                    // 4. UD 원점복귀
                    commandQueue.Enqueue(new ProcessCommand(CommandType.HomeUDAxis, "UD 원점복귀"));
                    // 5. LR 원점복귀
                    commandQueue.Enqueue(new ProcessCommand(CommandType.HomeLRAxis, "LR 원점복귀"));
                    // 6. 서보 모터 OFF
                    commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOff, "서보 모터 OFF"));
                    await commandQueue.ExecuteAsync();
                }

                // 황색 램프 (중단 - 대기)
                UpdateTowerLamp(TowerLampState.Yellow);

                // 공정 중단 로그
                LogManager.Instance.AddLog($"자동 공정", "사용자에 의해 공정 중단됨", "System", LogCategory.Warning, false);

                // Stop 버튼에서 이미 메시지를 표시했으므로 여기서는 표시하지 않음
            }
            catch (Exception ex)
            {
                // 오류 발생 - 적색 램프
                UpdateTowerLamp(TowerLampState.Red);

                // 실제 모드에서는 안전 종료 시퀀스 실행
                if (IsRealMode())
                {
                    commandQueue.Clear();
                    commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));
                    commandQueue.Enqueue(new ProcessCommand(CommandType.DisableSuction, "흡기 OFF"));
                    commandQueue.Enqueue(new ProcessCommand(CommandType.DisableExhaust, "배기 OFF"));
                    commandQueue.Enqueue(new ProcessCommand(CommandType.HomeUDAxis, "UD 원점복귀"));
                    commandQueue.Enqueue(new ProcessCommand(CommandType.HomeLRAxis, "LR 원점복귀"));
                    commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOff, "서보 모터 OFF"));
                    await commandQueue.ExecuteAsync();
                }

                ShowErrorMessage($"공정 중 오류 발생:\n{ex.Message}");
            }
        }

        /// <summary>
        /// FOUP A → 지정된 이온 주입 PM으로 이송 및 공정 시작
        /// </summary>
        private async Task TransferWaferFromFoupAToIonPM(ProcessModule targetPM)
        {
            int waferSlot = GetTopWaferSlotFromFoupA();
            if (waferSlot < 0)
            {
                return;
            }

            float targetAngle = (targetPM == processModuleA) ? ANGLE_PM1 : ANGLE_PM2;
            string pmName = (targetPM == processModuleA) ? "PM1" : "PM2";

            commandQueue.Clear();

            if (IsRealMode())
            {
                // === 실제 하드웨어 시퀀스 ===

                // 1. LR을 FOUP A 위치로 이동 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "FOUP A로 회전 (UI)", ANGLE_FOUP_A));
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveLRAxis, "FOUP A로 LR 이동", HardwarePositionMap.LR_FOUP_A));

                // 3. UD를 FOUP 안착 위치로 이동
                long foupSeating = HardwarePositionMap.GetFoupSeatingPosition(waferSlot, true);
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 안착 위치 이동", foupSeating));

                // 4. 실린더 전진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendCylinder, "실린더 전진"));

                // 5. 흡착 ON
                commandQueue.Enqueue(new ProcessCommand(CommandType.EnableSuction, "흡착 ON"));

                // 6. FOUP 데이터에서 웨이퍼 제거 및 TM에 픽업
                commandQueue.Enqueue(new ProcessCommand(CommandType.RemoveWaferFromFoup, "FOUP A 웨이퍼 제거", foupA, waferSlot));
                commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));

                // 7. UD를 FOUP 상승 위치로 이동 (웨이퍼 들어올림)
                long foupLifted = HardwarePositionMap.GetFoupLiftedPosition(waferSlot, true);
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 상승 위치 이동", foupLifted));

                // 8. 실린더 후진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));

                // 9. LR을 PM 위치로 이동 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, $"{pmName}로 회전 (UI)", targetAngle));
                long pmLR = HardwarePositionMap.GetPMLRPosition(targetPM.Type);
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveLRAxis, $"{pmName}로 LR 이동", pmLR));

                // 11. PM 문 열기
                commandQueue.Enqueue(new ProcessCommand(CommandType.OpenPMDoor, "PM 문 열기", targetPM));

                // 12. UD를 PM 상승 위치로 이동
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 상승 위치 이동", HardwarePositionMap.PM_UD_LIFTED));

                // 13. 실린더 전진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendCylinder, "실린더 전진"));

                // 14. UD를 PM 안착 위치로 이동 (웨이퍼 내려놓음)
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 안착 위치 이동", HardwarePositionMap.PM_UD_SEATING));

                // 15. 흡착 OFF 및 배기
                commandQueue.Enqueue(new ProcessCommand(CommandType.DisableSuction, "흡착 OFF"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.EnableExhaust, "배기 ON"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.DisableExhaust, "배기 OFF"));

                // 16. PM에 웨이퍼 배치 (데이터 처리)
                commandQueue.Enqueue(new ProcessCommand(CommandType.PlaceWafer, "웨이퍼 배치", targetPM));

                // 17. 실린더 후진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));

                // 18. PM 문 닫기
                commandQueue.Enqueue(new ProcessCommand(CommandType.ClosePMDoor, "PM 문 닫기", targetPM));

                // 20. 공정 시작 (파라미터 안정화 후 시간 카운트 시작)
                commandQueue.Enqueue(new ProcessCommand(CommandType.StartProcess, $"{pmName} 공정 시작", targetPM));
            }
            else
            {
                // === 기존 시뮬레이션 시퀀스 ===

                // 1. FOUP A로 이동
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "FOUP A로 회전", ANGLE_FOUP_A));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));

                // 2. FOUP A에서 웨이퍼 제거 및 픽업
                commandQueue.Enqueue(new ProcessCommand(CommandType.RemoveWaferFromFoup, "FOUP A 웨이퍼 제거", foupA, waferSlot));
                commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

                // 3. 선택된 이온 주입 PM으로 이동 및 배치
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, $"{pmName}로 회전", targetAngle));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.PlaceWafer, "웨이퍼 배치", targetPM));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

                // 4. 공정 시작 (파라미터 안정화 후 시간 카운트 시작)
                commandQueue.Enqueue(new ProcessCommand(CommandType.StartProcess, $"{pmName} 공정 시작", targetPM));
            }

            await commandQueue.ExecuteAsync();
        }

        /// <summary>
        /// FOUP A → PM1 웨이퍼 이송 및 공정 시작 (하위 호환성)
        /// </summary>
        private async Task TransferWaferFromFoupAToPM1()
        {
            await TransferWaferFromFoupAToIonPM(processModuleA);
        }

        /// <summary>
        /// 이온 주입 PM (PM1 또는 PM2) → PM3 웨이퍼 이송 및 공정 시작
        /// </summary>
        private async Task TransferWaferFromIonPMToPM3(ProcessModule sourcePM)
        {
            float sourceAngle = (sourcePM == processModuleA) ? ANGLE_PM1 : ANGLE_PM2;
            string pmName = (sourcePM == processModuleA) ? "PM1" : "PM2";

            commandQueue.Clear();

            if (IsRealMode())
            {
                // === 실제 하드웨어 시퀀스 ===

                // 1. LR을 소스 PM 위치로 이동 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, $"{pmName}로 회전 (UI)", sourceAngle));
                long sourcePMLR = HardwarePositionMap.GetPMLRPosition(sourcePM.Type);
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveLRAxis, $"{pmName}로 LR 이동", sourcePMLR));

                // 3. 소스 PM 문 열기
                commandQueue.Enqueue(new ProcessCommand(CommandType.OpenPMDoor, $"{pmName} 문 열기", sourcePM));

                // 4. UD를 PM 안착 위치로 이동
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 안착 위치 이동", HardwarePositionMap.PM_UD_SEATING));

                // 5. 실린더 전진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendCylinder, "실린더 전진"));

                // 6. 흡착 ON
                commandQueue.Enqueue(new ProcessCommand(CommandType.EnableSuction, "흡착 ON"));

                // 7. 웨이퍼 언로드 및 픽업 (데이터 처리)
                commandQueue.Enqueue(new ProcessCommand(CommandType.UnloadWaferFromPM, $"{pmName} 웨이퍼 언로드", sourcePM));
                commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));

                // 8. UD를 PM 상승 위치로 이동 (웨이퍼 들어올림)
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 상승 위치 이동", HardwarePositionMap.PM_UD_LIFTED));

                // 9. 실린더 후진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));

                // 10. 소스 PM 문 닫기
                commandQueue.Enqueue(new ProcessCommand(CommandType.ClosePMDoor, $"{pmName} 문 닫기", sourcePM));

                // 12. LR을 PM3 위치로 이동 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM3로 회전 (UI)", ANGLE_PM3));
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveLRAxis, "PM3로 LR 이동", HardwarePositionMap.LR_PM3));

                // 13. PM3 문 열기
                commandQueue.Enqueue(new ProcessCommand(CommandType.OpenPMDoor, "PM3 문 열기", processModuleC));

                // 14. UD를 PM3 상승 위치로 이동
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 상승 위치 이동", HardwarePositionMap.PM_UD_LIFTED));

                // 15. 실린더 전진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendCylinder, "실린더 전진"));

                // 16. UD를 PM3 안착 위치로 이동 (웨이퍼 내려놓음)
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 안착 위치 이동", HardwarePositionMap.PM_UD_SEATING));

                // 17. 흡착 OFF 및 배기
                commandQueue.Enqueue(new ProcessCommand(CommandType.DisableSuction, "흡착 OFF"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.EnableExhaust, "배기 ON"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.DisableExhaust, "배기 OFF"));

                // 18. PM3에 웨이퍼 배치 (데이터 처리)
                commandQueue.Enqueue(new ProcessCommand(CommandType.PlaceWafer, "웨이퍼 배치", processModuleC));

                // 19. 실린더 후진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));

                // 20. PM3 문 닫기
                commandQueue.Enqueue(new ProcessCommand(CommandType.ClosePMDoor, "PM3 문 닫기", processModuleC));

                // 22. PM3 공정 시작 (파라미터 안정화 후 시간 카운트 시작)
                commandQueue.Enqueue(new ProcessCommand(CommandType.StartProcess, "PM3 공정 시작", processModuleC));
            }
            else
            {
                // === 기존 시뮬레이션 시퀀스 ===

                // 1. 소스 PM으로 이동
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, $"{pmName}로 회전", sourceAngle));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));

                // 2. 소스 PM에서 웨이퍼 언로드 및 픽업
                commandQueue.Enqueue(new ProcessCommand(CommandType.UnloadWaferFromPM, $"{pmName} 웨이퍼 언로드", sourcePM));
                commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

                // 3. PM3로 이동 및 배치
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM3로 회전", ANGLE_PM3));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.PlaceWafer, "웨이퍼 배치", processModuleC));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축"));

                // 4. PM3 공정 시작 (파라미터 안정화 후 시간 카운트 시작)
                commandQueue.Enqueue(new ProcessCommand(CommandType.StartProcess, "PM3 공정 시작", processModuleC));
            }

            await commandQueue.ExecuteAsync();
        }

        /// <summary>
        /// PM1 → PM3 웨이퍼 이송 및 공정 시작 (하위 호환성)
        /// </summary>
        private async Task TransferWaferFromPM1ToPM3()
        {
            await TransferWaferFromIonPMToPM3(processModuleA);
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

            if (IsRealMode())
            {
                // === 실제 하드웨어 시퀀스 ===

                // 1. LR을 PM3 위치로 이동 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "PM3로 회전 (UI)", ANGLE_PM3));
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveLRAxis, "PM3로 LR 이동", HardwarePositionMap.LR_PM3));

                // 3. PM3 문 열기
                commandQueue.Enqueue(new ProcessCommand(CommandType.OpenPMDoor, "PM3 문 열기", processModuleC));

                // 4. UD를 PM3 안착 위치로 이동
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 안착 위치 이동", HardwarePositionMap.PM_UD_SEATING));

                // 5. 실린더 전진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendCylinder, "실린더 전진"));

                // 6. 흡착 ON
                commandQueue.Enqueue(new ProcessCommand(CommandType.EnableSuction, "흡착 ON"));

                // 7. 웨이퍼 언로드 및 픽업 (데이터 처리)
                commandQueue.Enqueue(new ProcessCommand(CommandType.UnloadWaferFromPM, "PM3 웨이퍼 언로드", processModuleC));
                commandQueue.Enqueue(new ProcessCommand(CommandType.PickWafer, "웨이퍼 픽업"));

                // 8. UD를 PM3 상승 위치로 이동 (웨이퍼 들어올림)
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 상승 위치 이동", HardwarePositionMap.PM_UD_LIFTED));

                // 9. 실린더 후진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));

                // 10. PM3 문 닫기
                commandQueue.Enqueue(new ProcessCommand(CommandType.ClosePMDoor, "PM3 문 닫기", processModuleC));

                // 12. LR을 FOUP B 위치로 이동 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RotateTM, "FOUP B로 회전 (UI)", ANGLE_FOUP_B));
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveLRAxis, "FOUP B로 LR 이동", HardwarePositionMap.LR_FOUP_B));

                // 13. UD를 FOUP B 상승 위치로 이동
                long foupLifted = HardwarePositionMap.GetFoupLiftedPosition(emptySlot, false);
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 상승 위치 이동", foupLifted));

                // 14. 실린더 전진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendArm, "암 확장 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ExtendCylinder, "실린더 전진"));

                // 15. UD를 FOUP B 안착 위치로 이동 (웨이퍼 내려놓음)
                long foupSeating = HardwarePositionMap.GetFoupSeatingPosition(emptySlot, false);
                commandQueue.Enqueue(new ProcessCommand(CommandType.MoveUDAxis, "UD 안착 위치 이동", foupSeating));

                // 16. 흡착 OFF 및 배기
                commandQueue.Enqueue(new ProcessCommand(CommandType.DisableSuction, "흡착 OFF"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.EnableExhaust, "배기 ON"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.DisableExhaust, "배기 OFF"));

                // 17. FOUP B에 웨이퍼 추가 (데이터 처리)
                commandQueue.Enqueue(new ProcessCommand(CommandType.AddWaferToFoup, "FOUP B에 웨이퍼 추가", foupB, emptySlot));

                // 18. 실린더 후진 + UI 애니메이션
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractArm, "암 수축 (UI)"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));
            }
            else
            {
                // === 기존 시뮬레이션 시퀀스 ===

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
            }

            await commandQueue.ExecuteAsync();
        }

        /// <summary>
        /// 특정 PM의 공정 완료 대기 (범용)
        /// </summary>
        private async Task WaitForPMComplete(ProcessModule pm)
        {
            while (pm.ModuleState == ProcessModule.State.Running ||
                   (pm.ModuleState == ProcessModule.State.Idle && !pm.IsUnloadRequested))
            {
                // 워크플로우가 취소되면 즉시 종료
                if (isWorkflowCancelled)
                {
                    throw new OperationCanceledException();
                }

                await Task.Delay(500);
            }

            // 공정이 Stopped 상태인 경우도 취소로 처리
            if (pm.ModuleState == ProcessModule.State.Stoped)
            {
                throw new OperationCanceledException();
            }
        }

        /// <summary>
        /// PM1 공정 완료 대기 (하위 호환성)
        /// </summary>
        private async Task WaitForPM1Complete()
        {
            await WaitForPMComplete(processModuleA);
        }

        /// <summary>
        /// PM3 공정 완료 대기 (하위 호환성)
        /// </summary>
        private async Task WaitForPM3Complete()
        {
            await WaitForPMComplete(processModuleC);
        }

        /// <summary>
        /// PM3가 사용 가능할 때까지 대기 (Idle 또는 언로드 준비 완료)
        /// </summary>
        private async Task WaitForPM3Available()
        {
            // PM3가 비어있으면 즉시 리턴
            if (!processModuleC.isWaferLoaded)
                return;

            // PM3 공정 완료 대기
            await WaitForPMComplete(processModuleC);
        }

        /// <summary>
        /// 웨이퍼 이송 및 공정 시작 시퀀스 (기존 단일 웨이퍼 테스트용)
        /// </summary>
        private async Task StartWaferTransferAndProcess()
        {
            // RecipeView에서 최신 레시피 로드
            LoadRecipesFromRecipeView();

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

            // 10. 공정 시작 (파라미터 안정화 후 시간 카운트 시작)
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
            // 이미 Stop 진행 중이면 무시
            if (isStopping)
            {
                return;
            }

            // Stop 진행 중 플래그 설정 및 모든 버튼 비활성화
            isStopping = true;
            ActivateButtons(false);
            btnAllStop.Enabled = false; // Stop 버튼도 비활성화

            // 워크플로우 취소 플래그 설정
            isWorkflowCancelled = true;

            bool anyStopped = false;
            bool wasExecuting = commandQueue.IsExecuting;

            // TM이 명령 실행 중이면 현재 명령 완료까지 대기
            if (wasExecuting)
            {
                // CommandQueue에 Stop 요청 (현재 명령 완료 후 중단)
                commandQueue.RequestStop();

                // 현재 명령 완료까지 대기 (최대 30초)
                int timeout = 30000;
                int elapsed = 0;
                int pollInterval = 100;

                while (commandQueue.IsExecuting && elapsed < timeout)
                {
                    await Task.Delay(pollInterval);
                    elapsed += pollInterval;
                }
            }

            // processModuleA (PM1) 공정 중지 및 완전 초기화
            if (processModuleA.ModuleState != ProcessModule.State.Idle ||
                processModuleA.Parameters.IsRising ||
                processModuleA.Parameters.IsStable)
            {
                processModuleA.StopProcess();
                processModuleA.ModuleState = ProcessModule.State.Idle;
                processModuleA.elapsedTime = 0;
                progressBarPM1.Value = 0;
                anyStopped = true;
            }
            // Idle 상태여도 IsUnloadRequested는 무조건 초기화
            processModuleA.IsUnloadRequested = false;

            // processModuleB (PM2) 공정 중지 및 완전 초기화
            if (processModuleB.ModuleState != ProcessModule.State.Idle ||
                processModuleB.Parameters.IsRising ||
                processModuleB.Parameters.IsStable)
            {
                processModuleB.StopProcess();
                processModuleB.ModuleState = ProcessModule.State.Idle;
                processModuleB.elapsedTime = 0;
                progressBarPM2.Value = 0;
                anyStopped = true;
            }
            // Idle 상태여도 IsUnloadRequested는 무조건 초기화
            processModuleB.IsUnloadRequested = false;

            // processModuleC (PM3) 공정 중지 및 완전 초기화
            if (processModuleC.ModuleState != ProcessModule.State.Idle ||
                processModuleC.Parameters.IsRising ||
                processModuleC.Parameters.IsStable)
            {
                processModuleC.StopProcess();
                processModuleC.ModuleState = ProcessModule.State.Idle;
                processModuleC.elapsedTime = 0;
                progressBarPM3.Value = 0;
                anyStopped = true;
            }
            // Idle 상태여도 IsUnloadRequested는 무조건 초기화
            processModuleC.IsUnloadRequested = false;

            // 명령 큐 완전 중지 및 초기화
            commandQueue.Stop();
            commandQueue.ResetStopFlag();

            // TM 위치 초기화 (UI 각도를 0도로 리셋)
            transferModule.RotateImmediate(0);
            UpdateTransferModuleGraphics();

            UpdateProcessDisplay();

            // 모든 램프를 명시적으로 끄기 (점멸 상태 해제)
            picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOff;
            picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOff;
            picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOff;

            // 실제 하드웨어 램프도 모두 끄기 (조건 없이 항상)
            etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM1, false);
            etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM2, false);
            etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM3, false);

            // 실제 모드에서는 장비 초기화 (서보 ON → 원점복귀 → 서보 OFF, PM 문 닫기 등)
            if (IsRealMode() && (anyStopped || wasExecuting))
            {
                await InitializeHardwareAsync();
            }

            // 워크플로우 진행 중이었거나 공정이 실행 중이었으면 메시지 표시
            if (anyStopped || wasExecuting)
            {
                // 황색 램프 (중단 - 대기)
                UpdateTowerLamp(TowerLampState.Yellow);

                MessageBox.Show("실행 중인 모든 공정과 워크플로우가 중지 및 초기화되었습니다.",
                    "공정 중지", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("실행 중인 공정이 없습니다.",
                    "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // 플래그 리셋 및 버튼 재활성화
            isWorkflowCancelled = false;
            isStopping = false;

            // 로그인 상태에 따라 버튼 활성화
            if (MainForm.IsLogined)
            {
                ActivateRecipeButtons(true);
                if (MainForm.IsConnected)
                {
                    ActivateEquipmentButtons(true);
                }
            }
            btnAllStop.Enabled = true;
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
        /// 실제 장비는 Slot[0]부터 제거하므로, GUI에서 아래부터 빠지도록 매핑 반전
        /// UI 하단(Wafer1) = 슬롯[4], UI 상단(Wafer5) = 슬롯[0]
        /// </summary>
        private void UpdateFoupADisplay()
        {
            // 웨이퍼 슬롯 색상 업데이트 (매핑 반전: UI 아래 = 데이터 위)
            panelFoupAWafer1.BackColor = foupA.WaferSlots[4] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 아래 ← Slot[4]
            panelFoupAWafer2.BackColor = foupA.WaferSlots[3] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 2번째 ← Slot[3]
            panelFoupAWafer3.BackColor = foupA.WaferSlots[2] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 중간 ← Slot[2]
            panelFoupAWafer4.BackColor = foupA.WaferSlots[1] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 4번째 ← Slot[1]
            panelFoupAWafer5.BackColor = foupA.WaferSlots[0] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 위 ← Slot[0]
        }

        /// <summary>
        /// FOUP B 웨이퍼 상태를 화면에 표시
        /// 실제 장비는 Slot[0]부터 채우므로, GUI에서 아래부터 채워지도록 매핑 반전
        /// UI 하단(Wafer1) = 슬롯[4], UI 상단(Wafer5) = 슬롯[0]
        /// </summary>
        private void UpdateFoupBDisplay()
        {
            // 웨이퍼 슬롯 색상 업데이트 (매핑 반전: UI 아래 = 데이터 위)
            panelFoupBWafer1.BackColor = foupB.WaferSlots[4] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 아래 ← Slot[4]
            panelFoupBWafer2.BackColor = foupB.WaferSlots[3] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 2번째 ← Slot[3]
            panelFoupBWafer3.BackColor = foupB.WaferSlots[2] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 중간 ← Slot[2]
            panelFoupBWafer4.BackColor = foupB.WaferSlots[1] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 4번째 ← Slot[1]
            panelFoupBWafer5.BackColor = foupB.WaferSlots[0] != null ? Color.DeepSkyBlue : Color.FromArgb(64, 64, 64); // UI 위 ← Slot[0]
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
        /// PM 문 상태를 화면에 표시
        /// 문 열림 = DarkGray, 문 닫힘 = ControlLight
        /// </summary>
        public void UpdatePMDoorDisplay()
        {
            panelPM1Door.BackColor = processModuleA.IsDoorOpen ? Color.DarkGray : SystemColors.ControlLight;
            panelPM2Door.BackColor = processModuleB.IsDoorOpen ? Color.DarkGray : SystemColors.ControlLight;
            panelPM3Door.BackColor = processModuleC.IsDoorOpen ? Color.DarkGray : SystemColors.ControlLight;
        }

        /// <summary>
        /// FOUP A에서 맨 아래 웨이퍼 슬롯 인덱스 찾기
        /// 실제 하드웨어는 맨 아래(슬롯 0)부터 꺼냄
        /// </summary>
        private int GetTopWaferSlotFromFoupA()
        {
            // 아래에서부터 확인 (인덱스 0 -> 4)
            // 슬롯 0 = 1층 (맨 아래), 슬롯 4 = 5층 (맨 위)
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
            // 아래에서부터 확인 (인덱스 0 -> 4)
            // 슬롯 0 = 1층 (맨 아래), 슬롯 4 = 5층 (맨 위)
            for (int i = 0; i < 5; i++)
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
                Location = new Point(220, 120),
                Size = new Size(300, 300),
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
                // 실제 하드웨어 램프도 점멸
                etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM1, lampBlinkState);
            }

            // PM2 램프 점멸 처리
            if (processModuleB.IsUnloadRequested && processModuleB.ModuleState == ProcessModule.State.Idle)
            {
                picBoxPM2Lamp.BackgroundImage = lampBlinkState ? Properties.Resources.LampOn : Properties.Resources.LampOff;
                // 실제 하드웨어 램프도 점멸
                etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM2, lampBlinkState);
            }

            // PM3 램프 점멸 처리
            if (processModuleC.IsUnloadRequested && processModuleC.ModuleState == ProcessModule.State.Idle)
            {
                picBoxPM3Lamp.BackgroundImage = lampBlinkState ? Properties.Resources.LampOn : Properties.Resources.LampOff;
                // 실제 하드웨어 램프도 점멸
                etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM3, lampBlinkState);
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

        /// <summary>
        /// TransferModule 객체 반환 (이벤트 구독용)
        /// </summary>
        public TransferModule GetTransferModule()
        {
            return transferModule;
        }

        #endregion

        #region Process Module 접근 메서드

        /// <summary>
        /// 먼저 완료된 이온 주입 PM 반환 (FIFO 기반)
        /// PM1과 PM2 중 먼저 공정이 완료된 PM을 선택
        /// </summary>
        private ProcessModule GetFirstCompletedIonPM()
        {
            bool pm1Ready = processModuleA.IsUnloadRequested && processModuleA.isWaferLoaded;
            bool pm2Ready = processModuleB.IsUnloadRequested && processModuleB.isWaferLoaded;

            if (pm1Ready && pm2Ready)
            {
                // 둘 다 완료된 경우 - 먼저 완료된 PM 선택
                if (processModuleA.CompletedTime <= processModuleB.CompletedTime)
                    return processModuleA;
                else
                    return processModuleB;
            }
            else if (pm1Ready)
            {
                return processModuleA;
            }
            else if (pm2Ready)
            {
                return processModuleB;
            }

            return null; // 완료된 PM 없음
        }

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

        #region EtherCAT 컨트롤러 관련

        /// <summary>
        /// EtherCAT 컨트롤러 설정 (MainForm에서 호출)
        /// </summary>
        public void SetEtherCATController(IEtherCATController controller)
        {
            this.etherCATController = controller;

            // 각 모듈에 컨트롤러 전달
            transferModule.SetEtherCATController(controller);
            processModuleA.SetEtherCATController(controller);
            processModuleB.SetEtherCATController(controller);
            processModuleC.SetEtherCATController(controller);
        }

        /// <summary>
        /// 모든 장비 상태 초기화 (Disconnect 시 호출)
        /// </summary>
        public void ResetAllState()
        {
            // 워크플로우 취소
            isWorkflowCancelled = true;

            // 명령 큐 초기화
            commandQueue.Clear();
            commandQueue.Stop();

            // PM 상태 초기화
            processModuleA.ModuleState = ProcessModule.State.Idle;
            processModuleA.elapsedTime = 0;
            processModuleA.isWaferLoaded = false;
            processModuleA.IsUnloadRequested = false;
            processModuleA.Parameters.Reset();

            processModuleB.ModuleState = ProcessModule.State.Idle;
            processModuleB.elapsedTime = 0;
            processModuleB.isWaferLoaded = false;
            processModuleB.IsUnloadRequested = false;
            processModuleB.Parameters.Reset();

            processModuleC.ModuleState = ProcessModule.State.Idle;
            processModuleC.elapsedTime = 0;
            processModuleC.isWaferLoaded = false;
            processModuleC.IsUnloadRequested = false;
            processModuleC.Parameters.Reset();

            // TM 상태 초기화
            transferModule.State = TransferModule.TMState.Idle;
            transferModule.CurrentRotationAngle = 0;
            transferModule.IsArmExtended = false;
            transferModule.HasWafer = false;

            // FOUP 초기화
            foupA.UnloadWafers();
            foupB.UnloadWafers();

            // UI 업데이트
            UpdateFoupDisplays();
            UpdateProcessDisplay();

            // 진행률 바 초기화
            progressBarPM1.Value = 0;
            progressBarPM2.Value = 0;
            progressBarPM3.Value = 0;

            // PM 파라미터 표시 초기화
            ResetPMParametersDisplay();

            // 램프 끄기
            picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOff;
            picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOff;
            picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOff;

            // 타워 램프 OFF
            UpdateTowerLamp(TowerLampState.Off);
        }

        /// <summary>
        /// 에러 메시지 표시 (UI 스레드에서 실행)
        /// </summary>
        public void ShowErrorMessage(string message, string location = "System")
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowErrorMessage(message, location)));
                return;
            }

            // 알람 로그 추가
            LogManager.Instance.Alarm(message, location);

            MessageBox.Show(message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 실제 장비 초기화 시퀀스
        /// - 서보 모터 ON
        /// - 실린더 후진 (안전)
        /// - TM 원점복귀 (UD → LR 순서)
        /// - 서보 모터 OFF
        /// - 모든 PM 램프 OFF
        /// - 모든 PM 문 강제 닫기
        /// </summary>
        public async Task InitializeHardwareAsync()
        {
            if (!IsRealMode())
            {
                // 시뮬레이션 모드에서는 초기화 불필요
                return;
            }

            try
            {
                commandQueue.Clear();

                // 1. 서보 모터 ON (원점복귀를 위해 먼저 켜야 함)
                commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOn, "서보 모터 ON"));

                // 2. 실린더 후진 (축 이동 전 안전 확보)
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));

                // 3. TM 원점복귀 (UD → LR 순서)
                commandQueue.Enqueue(new ProcessCommand(CommandType.HomeUDAxis, "TM 상하축(UD) 원점복귀"));
                commandQueue.Enqueue(new ProcessCommand(CommandType.HomeLRAxis, "TM 좌우축(LR) 원점복귀"));

                // 4. 서보 모터 OFF (원점복귀 완료 후)
                commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOff, "서보 모터 OFF"));

                // 5. PM1 램프 OFF 및 문 강제 닫기
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM1 램프 OFF", processModuleA));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM1 문 강제 닫기", processModuleA));

                // 6. PM2 램프 OFF 및 문 강제 닫기
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM2 램프 OFF", processModuleB));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM2 문 강제 닫기", processModuleB));

                // 7. PM3 램프 OFF 및 문 강제 닫기
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM3 램프 OFF", processModuleC));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM3 문 강제 닫기", processModuleC));

                // 초기화 명령 실행
                await commandQueue.ExecuteAsync();

                MessageBox.Show("실제 장비 초기화가 완료되었습니다.\n- 서보 모터: ON → 실린더 후진 → 원점복귀 → OFF\n- TM 원점복귀: 완료 (UD → LR)\n- PM 램프: 모두 OFF\n- PM 문: 모두 닫힘",
                    "장비 초기화 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"장비 초기화 중 오류 발생:\n{ex.Message}",
                    "초기화 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// EtherCAT 컨트롤러 반환 (CommandQueue에서 호출)
        /// </summary>
        public IEtherCATController GetEtherCATController()
        {
            return etherCATController;
        }

        /// <summary>
        /// 실제 하드웨어 모드 여부
        /// </summary>
        public bool IsRealMode()
        {
            return etherCATController != null &&
                   !(etherCATController is SimulationEtherCATController);
        }

        #endregion
    }
}
