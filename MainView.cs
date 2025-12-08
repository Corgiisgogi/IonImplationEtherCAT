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
        private const float ANGLE_HOME = -25f;   // 원점 (PM1과 FOUP A 사이)
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

        // 알람 발생 후 재초기화 필요 플래그 (FOUP 재로드 전까지 공정 시작 불가)
        private bool needsReinitialization;

        // EtherCAT 컨트롤러 참조
        private IEtherCATController etherCATController;

        // RecipeView 참조
        private RecipeView recipeView;

        // 타워 램프 상태
        private TowerLampState currentTowerLampState = TowerLampState.Off;

        // 수동 제어 폼 인스턴스
        private ManualControlForm manualControlForm;

        public MainView()
        {
            InitializeComponent();

            // 각 공정용 ProcessModule 객체 생성 (기본값: 모두 35초)
            processModuleA = new ProcessModule(ProcessModule.ModuleType.PM1, 35);
            processModuleB = new ProcessModule(ProcessModule.ModuleType.PM2, 35);
            processModuleC = new ProcessModule(ProcessModule.ModuleType.PM3, 35);
            
            // FOUP 객체 생성
            foupA = new Foup("FOUP A");
            foupB = new Foup("FOUP B");
            
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

            // 알람 이벤트 연결
            LogManager.Instance.OnAlarmRestored += OnAlarmRestored;
            LogManager.Instance.OnAlarmRaised += OnAlarmRaised;
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
            }
            else
            {
                // Idle/Stopped/Stopping 상태에서 파라미터 하강 애니메이션
                processModuleA.UpdateParametersWhenIdle();
            }

            // processModuleB (PM2) 업데이트
            if (processModuleB.ModuleState == ProcessModule.State.Running)
            {
                processModuleB.UpdateProcess(1);
            }
            else
            {
                // Idle/Stopped/Stopping 상태에서 파라미터 하강 애니메이션
                processModuleB.UpdateParametersWhenIdle();
            }

            // processModuleC (PM3) 업데이트
            if (processModuleC.ModuleState == ProcessModule.State.Running)
            {
                processModuleC.UpdateProcess(1);
            }
            else
            {
                // Idle/Stopped/Stopping 상태에서 파라미터 하강 애니메이션
                processModuleC.UpdateParametersWhenIdle();
            }

            // 모든 PM 상태 표시 업데이트 (항상 실행 - Running/Stopping/Idle 모든 상태 반영)
            UpdateProcessDisplay();

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
        /// PM 진행률 바 초기화 (웨이퍼 언로드 + 문 닫힘 후 호출)
        /// </summary>
        public void ResetPMProgressBar(ProcessModule pm)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ResetPMProgressBarInternal(pm)));
            }
            else
            {
                ResetPMProgressBarInternal(pm);
            }
        }

        private void ResetPMProgressBarInternal(ProcessModule pm)
        {
            if (pm == processModuleA)
            {
                progressBarPM1.Value = 0;
                lblPM1Progress.Text = $"0s / {pm.processTime}s";
            }
            else if (pm == processModuleB)
            {
                progressBarPM2.Value = 0;
                lblPM2Progress.Text = $"0s / {pm.processTime}s";
            }
            else if (pm == processModuleC)
            {
                progressBarPM3.Value = 0;
                lblPM3Progress.Text = $"0s / {pm.processTime}s";
            }
        }

        /// <summary>
        /// PM1 상태를 화면에 표시
        /// </summary>
        private void UpdatePM1Display()
        {
            // 연결되지 않았으면 "-" 표시
            if (!MainForm.IsConnected)
            {
                lblPM1Status.Text = "-";
                lblPM1Progress.Text = "-";
                progressBarPM1.Value = 0;
                picBoxPM1Status.BackgroundImage = Properties.Resources.StatusGray;
                picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOff;
                return;
            }

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
                case ProcessModule.State.Stopping:
                    picBoxPM1Status.BackgroundImage = Properties.Resources.StatusYellow;  // 정지 중 (주황색 리소스 없음)
                    picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOff;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM1, false);
                    break;
            }
        }

        /// <summary>
        /// PM2 상태를 화면에 표시
        /// </summary>
        private void UpdatePM2Display()
        {
            // 연결되지 않았으면 "-" 표시
            if (!MainForm.IsConnected)
            {
                lblPM2Status.Text = "-";
                lblPM2Progress.Text = "-";
                progressBarPM2.Value = 0;
                picBoxPM2Status.BackgroundImage = Properties.Resources.StatusGray;
                picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOff;
                return;
            }

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
                case ProcessModule.State.Stopping:
                    picBoxPM2Status.BackgroundImage = Properties.Resources.StatusYellow;  // 정지 중 (주황색 리소스 없음)
                    picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOff;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM2, false);
                    break;
            }
        }

        /// <summary>
        /// PM3 상태를 화면에 표시
        /// </summary>
        private void UpdatePM3Display()
        {
            // 연결되지 않았으면 "-" 표시
            if (!MainForm.IsConnected)
            {
                lblPM3Status.Text = "-";
                lblPM3Progress.Text = "-";
                progressBarPM3.Value = 0;
                picBoxPM3Status.BackgroundImage = Properties.Resources.StatusGray;
                picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOff;
                return;
            }

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
                case ProcessModule.State.Stopping:
                    picBoxPM3Status.BackgroundImage = Properties.Resources.StatusYellow;  // 정지 중 (주황색 리소스 없음)
                    picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOff;
                    etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM3, false);
                    break;
            }
        }

        #region PM 파라미터 표시 업데이트

        /// <summary>
        /// PM1 파라미터 값 표시 업데이트
        /// </summary>
        private void UpdatePM1Parameters()
        {
            // 연결되지 않았으면 "-" 표시
            if (!MainForm.IsConnected)
            {
                lblPM1TemperatureValue.Text = "-";
                lblPM1PressureValue.Text = "-";
                lblPM1AVValue.Text = "-";
                lblPM1DoseValue.Text = "-";
                ResetPM1ParameterColors();
                return;
            }

            var p = processModuleA.Parameters;
            lblPM1TemperatureValue.Text = p.GetTemperatureDisplay();
            lblPM1PressureValue.Text = p.GetPressureDisplay();
            lblPM1AVValue.Text = p.GetHVDisplay();
            lblPM1DoseValue.Text = p.GetDoseDisplay();

            // 이탈 시 라벨 색상 변경
            lblPM1TemperatureValue.ForeColor = GetParameterLabelColor(p, "Temperature");
            lblPM1PressureValue.ForeColor = GetParameterLabelColor(p, "Pressure");
            lblPM1AVValue.ForeColor = GetParameterLabelColor(p, "HV");
        }

        /// <summary>
        /// PM2 파라미터 값 표시 업데이트
        /// </summary>
        private void UpdatePM2Parameters()
        {
            // 연결되지 않았으면 "-" 표시
            if (!MainForm.IsConnected)
            {
                lblPM2TemperatureValue.Text = "-";
                lblPM2PressureValue.Text = "-";
                lblPM2AVValue.Text = "-";
                lblPM2DoseValue.Text = "-";
                ResetPM2ParameterColors();
                return;
            }

            var p = processModuleB.Parameters;
            lblPM2TemperatureValue.Text = p.GetTemperatureDisplay();
            lblPM2PressureValue.Text = p.GetPressureDisplay();
            lblPM2AVValue.Text = p.GetHVDisplay();
            lblPM2DoseValue.Text = p.GetDoseDisplay();

            // 이탈 시 라벨 색상 변경
            lblPM2TemperatureValue.ForeColor = GetParameterLabelColor(p, "Temperature");
            lblPM2PressureValue.ForeColor = GetParameterLabelColor(p, "Pressure");
            lblPM2AVValue.ForeColor = GetParameterLabelColor(p, "HV");
        }

        /// <summary>
        /// PM3 파라미터 값 표시 업데이트 (Temperature, Pressure만)
        /// </summary>
        private void UpdatePM3Parameters()
        {
            // 연결되지 않았으면 "-" 표시
            if (!MainForm.IsConnected)
            {
                lblTemperatureValue.Text = "-";
                lblPressureValue.Text = "-";
                ResetPM3ParameterColors();
                return;
            }

            var p = processModuleC.Parameters;
            lblTemperatureValue.Text = p.GetTemperatureDisplay();
            lblPressureValue.Text = p.GetPressureDisplay();

            // 이탈 시 라벨 색상 변경
            lblTemperatureValue.ForeColor = GetParameterLabelColor(p, "Temperature");
            lblPressureValue.ForeColor = GetParameterLabelColor(p, "Pressure");
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
        /// 파라미터 이탈 상태에 따른 라벨 색상 반환
        /// </summary>
        private System.Drawing.Color GetParameterLabelColor(ProcessParameters p, string paramName)
        {
            if (p.IsDeviated && p.DeviatedParameterName == paramName)
            {
                // Alarm: 빨강, Warning: 주황
                return p.IsAlarmLevel ? System.Drawing.Color.Red : System.Drawing.Color.Orange;
            }
            return System.Drawing.SystemColors.ControlText; // 정상: 기본 색상
        }

        /// <summary>
        /// PM1 파라미터 라벨 색상 초기화
        /// </summary>
        private void ResetPM1ParameterColors()
        {
            lblPM1TemperatureValue.ForeColor = System.Drawing.SystemColors.ControlText;
            lblPM1PressureValue.ForeColor = System.Drawing.SystemColors.ControlText;
            lblPM1AVValue.ForeColor = System.Drawing.SystemColors.ControlText;
        }

        /// <summary>
        /// PM2 파라미터 라벨 색상 초기화
        /// </summary>
        private void ResetPM2ParameterColors()
        {
            lblPM2TemperatureValue.ForeColor = System.Drawing.SystemColors.ControlText;
            lblPM2PressureValue.ForeColor = System.Drawing.SystemColors.ControlText;
            lblPM2AVValue.ForeColor = System.Drawing.SystemColors.ControlText;
        }

        /// <summary>
        /// PM3 파라미터 라벨 색상 초기화
        /// </summary>
        private void ResetPM3ParameterColors()
        {
            lblTemperatureValue.ForeColor = System.Drawing.SystemColors.ControlText;
            lblPressureValue.ForeColor = System.Drawing.SystemColors.ControlText;
        }

        /// <summary>
        /// 알람 복구 시 호출 - 해당 PM의 파라미터 정상화 및 대기 상태로 전환
        /// </summary>
        private void OnAlarmRestored(LogEntry entry)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnAlarmRestored(entry)));
                return;
            }

            // 발생 위치에 따라 해당 PM에 알람 복구 처리
            switch (entry.Location)
            {
                case "PM1":
                    processModuleA.OnAlarmRestored();
                    break;
                case "PM2":
                    processModuleB.OnAlarmRestored();
                    break;
                case "PM3":
                    processModuleC.OnAlarmRestored();
                    break;
            }

            // 모든 알람이 복구되었으면 상태 초기화
            if (!LogManager.Instance.HasActiveAlarms)
            {
                // 워크플로우 취소 플래그 리셋
                isWorkflowCancelled = false;

                // 명령 큐 알람 플래그 리셋
                commandQueue.ResetAlarmStopFlag();
                commandQueue.ResetStopFlag();

                // 로그인 및 연결 상태에 따라 버튼 활성화
                if (MainForm.IsLogined)
                {
                    ActivateRecipeButtons(true);
                    if (MainForm.IsConnected)
                    {
                        ActivateEquipmentButtons(true);
                        // 재초기화가 필요하면 Start 버튼 비활성화 유지
                        if (!needsReinitialization)
                        {
                            btnAllProcess.Enabled = true;
                        }
                    }
                }

                // 황색 타워 램프 (대기 상태)
                UpdateTowerLamp(TowerLampState.Yellow);

                // UI 업데이트
                UpdateProcessDisplay();

                // 복구 완료 메시지
                if (needsReinitialization)
                {
                    MessageBox.Show(
                        "모든 알람이 복구되었습니다.\n\n공정을 처음부터 다시 시작하려면\nFOUP를 다시 로드해주세요.",
                        "알람 복구 완료",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "모든 알람이 복구되었습니다.\n공정을 다시 시작할 수 있습니다.",
                        "알람 복구 완료",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// 알람 발생 시 호출 - 현재 명령 완료 후 즉시 중지
        /// </summary>
        private void OnAlarmRaised(LogEntry entry)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<LogEntry>(OnAlarmRaised), entry);
                return;
            }

            // 워크플로우 취소 플래그 설정
            isWorkflowCancelled = true;

            // 재초기화 필요 플래그 설정 (FOUP 재로드 전까지 공정 시작 불가)
            needsReinitialization = true;

            // 명령 큐에 알람 중지 요청
            commandQueue.RequestAlarmStop();

            // 적색 타워 램프 점등
            UpdateTowerLamp(TowerLampState.Red);

            // Start 버튼 비활성화
            btnAllProcess.Enabled = false;

            // UI 업데이트
            UpdateProcessDisplay();

            // 알람 메시지 표시
            MessageBox.Show(
                $"알람 발생!\n\n위치: {entry.Location}\n내용: {entry.Description}\n\n" +
                $"알람 복구 후 FOUP를 다시 로드해야 공정을 시작할 수 있습니다.",
                "알람",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// 모든 PM 파라미터 표시 초기화
        /// </summary>
        private void ResetPMParametersDisplay()
        {
            // PM1 상태 및 파라미터 초기화
            lblPM1Status.Text = "-";
            lblPM1Progress.Text = "-";
            lblPM1TemperatureValue.Text = "-";
            lblPM1PressureValue.Text = "-";
            lblPM1AVValue.Text = "-";
            lblPM1DoseValue.Text = "-";
            picBoxPM1Status.BackgroundImage = Properties.Resources.StatusGray;

            // PM2 상태 및 파라미터 초기화
            lblPM2Status.Text = "-";
            lblPM2Progress.Text = "-";
            lblPM2TemperatureValue.Text = "-";
            lblPM2PressureValue.Text = "-";
            lblPM2AVValue.Text = "-";
            lblPM2DoseValue.Text = "-";
            picBoxPM2Status.BackgroundImage = Properties.Resources.StatusGray;

            // PM3 상태 및 파라미터 초기화
            lblPM3Status.Text = "-";
            lblPM3Progress.Text = "-";
            lblTemperatureValue.Text = "-";
            lblPressureValue.Text = "-";
            picBoxPM3Status.BackgroundImage = Properties.Resources.StatusGray;
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
            btnManualControl.Enabled = activate;

            // Non-Modal 수동 제어 폼이 열려있으면 컨트롤 비활성화
            if (manualControlForm != null && !manualControlForm.IsDisposed)
            {
                manualControlForm.SetControlsEnabled(activate);
            }

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
            panelYellowAlert.BackColor = (state == TowerLampState.Yellow) ? Color.Orange : Color.Khaki;
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
                return;

            // RecipeView에서 현재 레시피 세트 가져오기
            RecipeSet recipeSet = recipeView.GetCurrentRecipeSet();

            // PM1에 레시피 적용
            processModuleA.IonRecipe = recipeSet.PM1Recipe;

            // PM2에 레시피 적용
            processModuleB.IonRecipe = recipeSet.PM2Recipe;

            // PM3에 레시피 적용
            processModuleC.AnnealRecipe = recipeSet.PM3Recipe;
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
                Text = $"공정 시간을 입력하세요 (35초 ~ 120초):\n현재 설정: {currentTime}초",
                Font = new Font("나눔고딕", 11F, FontStyle.Regular)
            };

            NumericUpDown numericInput = new NumericUpDown()
            {
                Left = 30,
                Top = 90,
                Width = 380,
                Height = 30,
                Minimum = 35,    // 최소 35초 (마무리 단계 10초 확보)
                Maximum = 120,   // 최대 120초
                Value = Math.Max(35, Math.Min(120, currentTime)),  // 현재값을 범위 내로 제한
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

        /// <summary>
        /// 수동 제어 버튼 클릭 이벤트 - ManualControlForm 열기
        /// </summary>
        private void btnManualControl_Click(object sender, EventArgs e)
        {
            // 자동 공정 진행 중 확인
            if (IsAutoProcessRunning())
            {
                MessageBox.Show(
                    "자동 공정이 진행 중입니다.\n공정을 중지한 후 수동 제어를 사용해주세요.",
                    "수동 제어 불가",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 이미 열려있는 폼이 있으면 활성화
            if (manualControlForm != null && !manualControlForm.IsDisposed)
            {
                manualControlForm.Focus();
                return;
            }

            // EtherCAT 컨트롤러 가져오기
            var controller = GetEtherCATController();
            if (controller == null)
            {
                MessageBox.Show("EtherCAT 연결이 필요합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Non-Modal 방식으로 폼 열기
            manualControlForm = new ManualControlForm(controller);
            manualControlForm.FormClosed += (s, args) => manualControlForm = null;
            manualControlForm.Show();
        }

        /// <summary>
        /// 자동 공정이 진행 중인지 확인
        /// </summary>
        private bool IsAutoProcessRunning()
        {
            // 명령 큐가 실행 중이거나
            if (commandQueue.IsExecuting)
                return true;

            // PM 중 하나라도 Running 상태이면
            if (processModuleA.ModuleState == ProcessModule.State.Running ||
                processModuleB.ModuleState == ProcessModule.State.Running ||
                processModuleC.ModuleState == ProcessModule.State.Running)
                return true;

            // TM이 동작 중이면
            if (transferModule.State != TransferModule.TMState.Idle)
                return true;

            return false;
        }

        private async void btnAllProcess_Click(object sender, EventArgs e)
        {
            // 명령 큐가 실행 중인지 확인
            if (commandQueue.IsExecuting)
            {
                MessageBox.Show("명령이 실행 중입니다. 잠시 후 다시 시도해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 활성 알람 확인
            if (LogManager.Instance.HasActiveAlarms)
            {
                MessageBox.Show(
                    "활성 알람이 있습니다.\n알람을 먼저 복구해주세요.",
                    "공정 시작 불가",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 재초기화 필요 여부 확인
            if (needsReinitialization)
            {
                MessageBox.Show(
                    "알람 발생 후 재초기화가 필요합니다.\n\nFOUP A를 다시 로드하여 공정을 처음부터 시작해주세요.",
                    "공정 시작 불가",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
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

                    // 알람 발생 시 즉시 중단
                    if (LogManager.Instance.HasActiveAlarms)
                        throw new OperationCanceledException("알람 발생으로 워크플로우가 중단되었습니다.");

                    // 정상 완료 조건: FOUP A 비어있고, 모든 PM 비어있고, TM도 웨이퍼 없음 (FOUP B 상태 무관)
                    if (foupA.IsEmpty &&
                        !processModuleA.isWaferLoaded &&
                        !processModuleB.isWaferLoaded &&
                        !processModuleC.isWaferLoaded &&
                        !transferModule.HasWafer)
                    {
                        break; // 완료!
                    }

                    // 오류 조건: FOUP B가 꽉 찼는데 PM, FOUP A, 또는 TM에 웨이퍼가 남아있음
                    bool hasRemainingWafers = !foupA.IsEmpty ||
                                               processModuleA.isWaferLoaded ||
                                               processModuleB.isWaferLoaded ||
                                               processModuleC.isWaferLoaded ||
                                               transferModule.HasWafer;
                    if (foupB.IsFull && hasRemainingWafers)
                    {
                        // 적색 램프 (오류)
                        UpdateTowerLamp(TowerLampState.Red);

                        // 알람 로그
                        LogManager.Instance.Alarm("FOUP B가 가득 차서 공정을 진행할 수 없습니다. 웨이퍼가 PM, FOUP A, 또는 TM에 남아있습니다.", "System");

                        throw new InvalidOperationException("FOUP B가 가득 차서 공정을 진행할 수 없습니다.\n\n남은 웨이퍼:\n" +
                            $"- FOUP A: {foupA.WaferCount}개\n" +
                            $"- PM1: {(processModuleA.isWaferLoaded ? "1개" : "없음")}\n" +
                            $"- PM2: {(processModuleB.isWaferLoaded ? "1개" : "없음")}\n" +
                            $"- PM3: {(processModuleC.isWaferLoaded ? "1개" : "없음")}\n" +
                            $"- TM: {(transferModule.HasWafer ? "1개 (이송 중)" : "없음")}");
                    }

                    // 1단계: PM3 완료 → FOUP B로 이송
                    if (processModuleC.IsUnloadRequested && processModuleC.isWaferLoaded)
                    {
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

                // UI 애니메이션 Task (실제/시뮬레이션 모두 실행)
                Task animationTask = ReturnTMToHomeAsync();

                // 실제 모드에서는 안전 종료 시퀀스 실행 (하드웨어 + 애니메이션 병렬)
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

                    // 하드웨어와 애니메이션 병렬 실행
                    Task hardwareTask = commandQueue.ExecuteAsync();
                    await Task.WhenAll(hardwareTask, animationTask);
                }
                else
                {
                    // 시뮬레이션 모드에서는 애니메이션만 실행
                    await animationTask;
                }

                // 황색 램프 (공정 완료 - 대기)
                UpdateTowerLamp(TowerLampState.Yellow);

                // 실제 처리된 웨이퍼 수 (FOUP B에 들어간 웨이퍼)
                int processedWafers = foupB.WaferCount;

                // 공정 완료 로그
                LogManager.Instance.AddLog($"자동 공정", $"전체 자동 공정 완료 - 처리된 웨이퍼: {processedWafers}개", "System", LogCategory.Process, false);

                MessageBox.Show($"전체 공정이 완료되었습니다!\n처리된 웨이퍼: {processedWafers}개",
                    "공정 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                // 워크플로우가 중단됨 - 안전 종료 시퀀스 실행
                // UI 애니메이션 Task (실제/시뮬레이션 모두 실행)
                Task cancelAnimationTask = ReturnTMToHomeAsync();

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

                    // 하드웨어와 애니메이션 병렬 실행
                    Task cancelHardwareTask = commandQueue.ExecuteAsync();
                    await Task.WhenAll(cancelHardwareTask, cancelAnimationTask);
                }
                else
                {
                    // 시뮬레이션 모드에서는 애니메이션만 실행
                    await cancelAnimationTask;
                }

                // 황색 램프 (중단 - 대기)
                UpdateTowerLamp(TowerLampState.Yellow);

                // Stop 버튼에서 이미 메시지를 표시했으므로 여기서는 표시하지 않음
            }
            catch (Exception ex)
            {
                // 오류 발생 - 적색 램프
                UpdateTowerLamp(TowerLampState.Red);

                // UI 애니메이션 Task (실제/시뮬레이션 모두 실행)
                Task errorAnimationTask = ReturnTMToHomeAsync();

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

                    // 하드웨어와 애니메이션 병렬 실행
                    Task errorHardwareTask = commandQueue.ExecuteAsync();
                    await Task.WhenAll(errorHardwareTask, errorAnimationTask);
                }
                else
                {
                    // 시뮬레이션 모드에서는 애니메이션만 실행
                    await errorAnimationTask;
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

                // 11. 소스 PM 진행률 초기화 (공정 완전 종료)
                commandQueue.Enqueue(new ProcessCommand(CommandType.ResetPMProgress, $"{pmName} 진행률 초기화", sourcePM));

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

                // 2-1. 소스 PM 진행률 초기화 (공정 완전 종료)
                commandQueue.Enqueue(new ProcessCommand(CommandType.ResetPMProgress, $"{pmName} 진행률 초기화", sourcePM));

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

                // 11. PM3 진행률 초기화 (공정 완전 종료)
                commandQueue.Enqueue(new ProcessCommand(CommandType.ResetPMProgress, "PM3 진행률 초기화", processModuleC));

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

                // 2-1. PM3 진행률 초기화 (공정 완전 종료)
                commandQueue.Enqueue(new ProcessCommand(CommandType.ResetPMProgress, "PM3 진행률 초기화", processModuleC));

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
        /// TM을 원점(0도)으로 복귀시키는 애니메이션 (실린더 후진 → 회전)
        /// </summary>
        private async Task ReturnTMToHomeAsync()
        {
            // TM이 웨이퍼를 들고 있으면 폐기 처리
            if (transferModule.HasWafer)
            {
                LogManager.Instance.Warning(
                    "TM이 들고 있던 웨이퍼 폐기 처리",
                    "TM",
                    "공정 정지",
                    isRestored: true);
                transferModule.HasWafer = false;
                transferModule.CurrentWafer = null;
            }

            // 1. 실린더가 전진해있으면 후진
            if (transferModule.IsArmExtended || transferModule.CurrentExtension > 0)
            {
                transferModule.RetractArm();

                // 애니메이션 완료 대기
                bool completed = false;
                Action handler = () => completed = true;
                transferModule.OnArmMovementComplete += handler;

                // 최대 3초 대기 (후진 시간)
                int timeout = 3000;
                int elapsed = 0;
                while (!completed && elapsed < timeout)
                {
                    await Task.Delay(50);
                    elapsed += 50;
                }

                transferModule.OnArmMovementComplete -= handler;
            }

            // 2. 원점(ANGLE_HOME)으로 회전
            if (Math.Abs(transferModule.CurrentRotationAngle - ANGLE_HOME) > 0.1f)
            {
                transferModule.SetTargetRotation(ANGLE_HOME);

                // 애니메이션 완료 대기
                bool completed = false;
                Action handler = () => completed = true;
                transferModule.OnRotationComplete += handler;

                // 최대 5초 대기 (회전 시간)
                int timeout = 5000;
                int elapsed = 0;
                while (!completed && elapsed < timeout)
                {
                    await Task.Delay(50);
                    elapsed += 50;
                }

                transferModule.OnRotationComplete -= handler;
            }

            // 3. 그래픽 업데이트
            UpdateTransferModuleGraphics();
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

            // ========== PM 공정 즉시 중지 (Stopping 상태로 전환) ==========
            // processModuleA (PM1) 공정 중지
            if (processModuleA.ModuleState != ProcessModule.State.Idle ||
                processModuleA.Parameters.IsRising ||
                processModuleA.Parameters.IsStable)
            {
                processModuleA.StopProcess();  // Stopping 상태로 전환
                processModuleA.elapsedTime = 0;
                progressBarPM1.Value = 0;
                anyStopped = true;
            }
            processModuleA.IsUnloadRequested = false;

            // processModuleB (PM2) 공정 중지
            if (processModuleB.ModuleState != ProcessModule.State.Idle ||
                processModuleB.Parameters.IsRising ||
                processModuleB.Parameters.IsStable)
            {
                processModuleB.StopProcess();  // Stopping 상태로 전환
                processModuleB.elapsedTime = 0;
                progressBarPM2.Value = 0;
                anyStopped = true;
            }
            processModuleB.IsUnloadRequested = false;

            // processModuleC (PM3) 공정 중지
            if (processModuleC.ModuleState != ProcessModule.State.Idle ||
                processModuleC.Parameters.IsRising ||
                processModuleC.Parameters.IsStable)
            {
                processModuleC.StopProcess();  // Stopping 상태로 전환
                processModuleC.elapsedTime = 0;
                progressBarPM3.Value = 0;
                anyStopped = true;
            }
            // Idle 상태여도 IsUnloadRequested는 무조건 초기화
            processModuleC.IsUnloadRequested = false;

            // Stopping 상태를 UI에 즉시 반영 (하드웨어 초기화 전)
            UpdateProcessDisplay();

            // 명령 큐 완전 중지 및 초기화
            commandQueue.Stop();
            commandQueue.ResetStopFlag();

            // 모든 램프를 명시적으로 끄기 (점멸 상태 해제)
            picBoxPM1Lamp.BackgroundImage = Properties.Resources.LampOff;
            picBoxPM2Lamp.BackgroundImage = Properties.Resources.LampOff;
            picBoxPM3Lamp.BackgroundImage = Properties.Resources.LampOff;

            // 실제 하드웨어 램프도 모두 끄기 (조건 없이 항상)
            etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM1, false);
            etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM2, false);
            etherCATController?.SetPMLamp(ProcessModule.ModuleType.PM3, false);

            // 장비 초기화 (하드웨어 + 애니메이션 병렬 실행)
            if (anyStopped || wasExecuting)
            {
                await StopHardwareAsync();
            }

            // 하드웨어 초기화 완료 후 Stopping → Idle 전환
            processModuleA.CompleteStop();
            processModuleB.CompleteStop();
            processModuleC.CompleteStop();

            UpdateProcessDisplay();

            // 워크플로우 진행 중이었거나 공정이 실행 중이었으면 메시지 표시
            if (anyStopped || wasExecuting)
            {
                // 황색 램프 (중단 - 대기)
                UpdateTowerLamp(TowerLampState.Yellow);

                // 공정 중단 Warning 로깅 (복구된 상태로)
                LogManager.Instance.Warning(
                    "사용자에 의해 공정이 중단되었습니다.",
                    "System",
                    "공정 정지",
                    isRestored: true);

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

            // 재초기화가 필요한 상태에서 로드하면 PM/TM 상태도 초기화
            if (needsReinitialization)
            {
                // PM/TM에 웨이퍼가 남아있으면 경고 후 초기화
                bool hasRemainingWafers = processModuleA.isWaferLoaded ||
                                          processModuleB.isWaferLoaded ||
                                          processModuleC.isWaferLoaded ||
                                          transferModule.HasWafer;

                if (hasRemainingWafers)
                {
                    var result = MessageBox.Show(
                        "알람 발생 후 재초기화가 필요합니다.\n\n" +
                        "PM 또는 TM에 웨이퍼가 남아있습니다.\n" +
                        "모든 장비 상태를 초기화하고 공정을 처음부터 시작하시겠습니까?\n\n" +
                        "(취소 시 웨이퍼 손실이 발생할 수 있습니다)",
                        "재초기화 확인",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Cancel)
                        return;

                    // PM 상태 초기화 (웨이퍼 제거)
                    processModuleA.ModuleState = ProcessModule.State.Idle;
                    processModuleA.isWaferLoaded = false;
                    processModuleA.IsUnloadRequested = false;
                    processModuleA.elapsedTime = 0;
                    processModuleA.Parameters.Reset();

                    processModuleB.ModuleState = ProcessModule.State.Idle;
                    processModuleB.isWaferLoaded = false;
                    processModuleB.IsUnloadRequested = false;
                    processModuleB.elapsedTime = 0;
                    processModuleB.Parameters.Reset();

                    processModuleC.ModuleState = ProcessModule.State.Idle;
                    processModuleC.isWaferLoaded = false;
                    processModuleC.IsUnloadRequested = false;
                    processModuleC.elapsedTime = 0;
                    processModuleC.Parameters.Reset();

                    // TM 상태 초기화
                    transferModule.HasWafer = false;

                    // FOUP B도 초기화 (새로 시작하므로)
                    foupB.UnloadWafers();
                    UpdateFoupBDisplay();

                    // UI 업데이트
                    UpdateProcessDisplay();

                    LogManager.Instance.Log("알람 복구 후 장비 상태 초기화 완료", "System", LogCategory.System);
                }

                // 재초기화 플래그 해제
                needsReinitialization = false;

                // 알람이 없고 연결되어 있으면 Start 버튼 활성화
                if (!LogManager.Instance.HasActiveAlarms && MainForm.IsConnected && MainForm.IsLogined)
                {
                    btnAllProcess.Enabled = true;
                }
            }

            // 모든 웨이퍼 장착
            foupA.LoadWafers();
            UpdateFoupADisplay();

            // 로그 기록
            LogManager.Instance.AddLog("웨이퍼 이동",
                $"FOUP A에 웨이퍼 {foupA.WaferCount}개 S/W 장착",
                "FOUP A", LogCategory.Transfer, false);

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

            // 로그 기록
            LogManager.Instance.AddLog("웨이퍼 이동",
                "FOUP A의 모든 웨이퍼 S/W 제거",
                "FOUP A", LogCategory.Transfer, false);

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

            // 로그 기록
            LogManager.Instance.AddLog("웨이퍼 이동",
                $"FOUP B에 웨이퍼 {foupB.WaferCount}개 S/W 장착",
                "FOUP B", LogCategory.Transfer, false);

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

            // 로그 기록
            LogManager.Instance.AddLog("웨이퍼 이동",
                "FOUP B의 모든 웨이퍼 S/W 제거",
                "FOUP B", LogCategory.Transfer, false);

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

            // 초기 각도 설정 (원점 = PM1과 FOUP A 사이)
            transferModule.RotateImmediate(ANGLE_HOME);
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

            // 연결 시 모든 PM 상태를 Idle로 초기화
            processModuleA.ModuleState = ProcessModule.State.Idle;
            processModuleA.elapsedTime = 0;
            processModuleA.Parameters.Reset();

            processModuleB.ModuleState = ProcessModule.State.Idle;
            processModuleB.elapsedTime = 0;
            processModuleB.Parameters.Reset();

            processModuleC.ModuleState = ProcessModule.State.Idle;
            processModuleC.elapsedTime = 0;
            processModuleC.Parameters.Reset();

            // 라벨 업데이트
            UpdateProcessDisplay();

            // 시작 시 활성 알람 확인 (이전 세션에서 저장된 알람)
            CheckActiveAlarmsOnStartup();
        }

        /// <summary>
        /// 시작 시 활성 알람 확인 (로그 파일에서 불러온 알람)
        /// </summary>
        private void CheckActiveAlarmsOnStartup()
        {
            if (LogManager.Instance.HasActiveAlarms)
            {
                var activeAlarms = LogManager.Instance.GetActiveAlarms();
                int alarmCount = activeAlarms.Count(a => a.IsAlarm);
                int warningCount = activeAlarms.Count(a => a.IsWarning);

                // 알람이 있으면 Start 버튼 비활성화 및 타워 램프 적색
                if (alarmCount > 0)
                {
                    isWorkflowCancelled = true;
                    commandQueue.RequestAlarmStop();
                    UpdateTowerLamp(TowerLampState.Red);
                    btnAllProcess.Enabled = false;

                    MessageBox.Show(
                        $"이전 세션에서 복구되지 않은 알람이 {alarmCount}건 있습니다.\n" +
                        $"알람을 복구하기 전까지 공정을 시작할 수 없습니다.\n\n" +
                        $"[알람 탭에서 복구해주세요]",
                        "활성 알람 존재",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    LogManager.Instance.Log(
                        $"시작 시 활성 알람 {alarmCount}건 감지됨",
                        "MainView",
                        LogCategory.System
                    );
                }
                else if (warningCount > 0)
                {
                    // 경고만 있는 경우 알림만 표시
                    LogManager.Instance.Log(
                        $"시작 시 활성 경고 {warningCount}건 감지됨",
                        "MainView",
                        LogCategory.System
                    );
                }
            }
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

            // TM 상태 초기화 (원점 = PM1과 FOUP A 사이)
            transferModule.State = TransferModule.TMState.Idle;
            transferModule.CurrentRotationAngle = ANGLE_HOME;
            transferModule.TargetRotationAngle = ANGLE_HOME;
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
        public async Task InitializeHardwareAsync(bool showMessage = true)
        {
            // 초기화 진행 다이얼로그 생성 및 표시
            InitializationProgressForm progressForm = null;

            if (showMessage)
            {
                progressForm = new InitializationProgressForm();
                progressForm.Show(this.ParentForm);
                progressForm.SetTotalSteps(IsRealMode() ? 11 : 1);
            }

            // UI 애니메이션 Task (실제/시뮬레이션 모두 실행)
            Task animationTask = ReturnTMToHomeAsync();

            // 초기화 시작 로그
            LogManager.Instance.AddLog("초기화", "장비 초기화 시작", "System", LogCategory.System, false);

            if (!IsRealMode())
            {
                // 시뮬레이션 모드에서는 애니메이션만 실행
                progressForm?.NextStep("시뮬레이션 모드 - TM 원점 복귀");
                await animationTask;

                LogManager.Instance.AddLog("초기화", "시뮬레이션 모드 초기화 완료", "System", LogCategory.System, false);
                progressForm?.SetComplete();
                await Task.Delay(1000);
                progressForm?.Close();
                return;
            }

            try
            {
                // 1. 서보 모터 ON (원점복귀를 위해 먼저 켜야 함)
                progressForm?.NextStep("서보 모터 ON");
                LogManager.Instance.AddLog("초기화", "서보 모터 ON", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOn, "서보 모터 ON"));
                await commandQueue.ExecuteAsync();

                // 2. 실린더 후진 (축 이동 전 안전 확보)
                progressForm?.NextStep("실린더 후진 (안전 확보)");
                LogManager.Instance.AddLog("초기화", "실린더 후진", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));
                await commandQueue.ExecuteAsync();

                // 3. TM 상하축 원점복귀
                progressForm?.NextStep("TM 상하축(UD) 원점복귀");
                LogManager.Instance.AddLog("초기화", "TM 상하축(UD) 원점복귀 시작", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.HomeUDAxis, "TM 상하축(UD) 원점복귀"));
                await commandQueue.ExecuteAsync();

                // 4. TM 좌우축 원점복귀
                progressForm?.NextStep("TM 좌우축(LR) 원점복귀");
                LogManager.Instance.AddLog("초기화", "TM 좌우축(LR) 원점복귀 시작", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.HomeLRAxis, "TM 좌우축(LR) 원점복귀"));
                await commandQueue.ExecuteAsync();

                // 5. 서보 모터 OFF (원점복귀 완료 후)
                progressForm?.NextStep("서보 모터 OFF");
                LogManager.Instance.AddLog("초기화", "서보 모터 OFF", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOff, "서보 모터 OFF"));
                await commandQueue.ExecuteAsync();

                // 6. PM1 램프 OFF 및 문 강제 닫기
                progressForm?.NextStep("PM1 초기화 (램프 OFF, 문 닫기)");
                LogManager.Instance.AddLog("초기화", "PM1 램프 OFF", "PM1", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM1 램프 OFF", processModuleA));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM1 문 강제 닫기", processModuleA));
                await commandQueue.ExecuteAsync();

                // 7. PM2 램프 OFF 및 문 강제 닫기
                progressForm?.NextStep("PM2 초기화 (램프 OFF, 문 닫기)");
                LogManager.Instance.AddLog("초기화", "PM2 램프 OFF", "PM2", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM2 램프 OFF", processModuleB));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM2 문 강제 닫기", processModuleB));
                await commandQueue.ExecuteAsync();

                // 8. PM3 램프 OFF 및 문 강제 닫기
                progressForm?.NextStep("PM3 초기화 (램프 OFF, 문 닫기)");
                LogManager.Instance.AddLog("초기화", "PM3 램프 OFF", "PM3", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM3 램프 OFF", processModuleC));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM3 문 강제 닫기", processModuleC));
                await commandQueue.ExecuteAsync();

                // TM 애니메이션 완료 대기
                progressForm?.NextStep("TM UI 애니메이션 완료 대기");
                await animationTask;

                // 초기화 완료 로그
                LogManager.Instance.AddLog("초기화", "장비 초기화 완료", "System", LogCategory.System, false);

                progressForm?.SetComplete();
                await Task.Delay(1500);
                progressForm?.Close();
            }
            catch (Exception ex)
            {
                LogManager.Instance.Alarm($"장비 초기화 오류: {ex.Message}", "System");
                progressForm?.SetError(ex.Message);
                await Task.Delay(2000);
                progressForm?.Close();

                MessageBox.Show($"장비 초기화 중 오류 발생:\n{ex.Message}",
                    "초기화 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 장비 정지 시퀀스 (STOP 버튼용)
        /// - 진행 다이얼로그 표시
        /// - 실린더 후진
        /// - TM 원점복귀 (UD → LR 순서)
        /// - 서보 모터 OFF
        /// - 모든 PM 램프 OFF
        /// - 모든 PM 문 강제 닫기
        /// </summary>
        private async Task StopHardwareAsync()
        {
            // 정지 진행 다이얼로그 생성 및 표시
            InitializationProgressForm progressForm = new InitializationProgressForm();
            progressForm.SetTitle("장비 정지", "장비 정지 진행 중...");
            progressForm.Show(this.ParentForm);
            progressForm.SetTotalSteps(IsRealMode() ? 13 : 1);

            // UI 애니메이션 Task (실제/시뮬레이션 모두 실행)
            Task animationTask = ReturnTMToHomeAsync();

            // 정지 시작 로그
            LogManager.Instance.AddLog("정지", "장비 정지 시작", "System", LogCategory.System, false);

            if (!IsRealMode())
            {
                // 시뮬레이션 모드에서는 애니메이션만 실행
                progressForm.NextStep("시뮬레이션 모드 - TM 원점 복귀");
                await animationTask;

                LogManager.Instance.AddLog("정지", "시뮬레이션 모드 정지 완료", "System", LogCategory.System, false);
                progressForm.SetStopComplete();
                await Task.Delay(1000);
                progressForm.Close();
                return;
            }

            try
            {
                // 1. 서보 모터 ON (원점복귀를 위해 먼저 켜야 함)
                progressForm.NextStep("서보 모터 ON");
                LogManager.Instance.AddLog("정지", "서보 모터 ON", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOn, "서보 모터 ON"));
                await commandQueue.ExecuteAsync();

                // 2. 실린더 후진 (축 이동 전 안전 확보)
                progressForm.NextStep("실린더 후진 (안전 확보)");
                LogManager.Instance.AddLog("정지", "실린더 후진", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.RetractCylinder, "실린더 후진"));
                await commandQueue.ExecuteAsync();

                // 3. TM 상하축 원점복귀
                progressForm.NextStep("TM 상하축(UD) 원점복귀");
                LogManager.Instance.AddLog("정지", "TM 상하축(UD) 원점복귀 시작", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.HomeUDAxis, "TM 상하축(UD) 원점복귀"));
                await commandQueue.ExecuteAsync();

                // 4. TM 좌우축 원점복귀
                progressForm.NextStep("TM 좌우축(LR) 원점복귀");
                LogManager.Instance.AddLog("정지", "TM 좌우축(LR) 원점복귀 시작", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.HomeLRAxis, "TM 좌우축(LR) 원점복귀"));
                await commandQueue.ExecuteAsync();

                // 5. 흡착 OFF (TM 원점복귀 후)
                progressForm.NextStep("흡착 OFF");
                LogManager.Instance.AddLog("정지", "흡착 OFF", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.DisableSuction, "흡착 OFF"));
                await commandQueue.ExecuteAsync();

                // 6. 배기 OFF
                progressForm.NextStep("배기 OFF");
                LogManager.Instance.AddLog("정지", "배기 OFF", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.DisableExhaust, "배기 OFF"));
                await commandQueue.ExecuteAsync();

                // 7. 서보 모터 OFF (원점복귀 완료 후)
                progressForm.NextStep("서보 모터 OFF");
                LogManager.Instance.AddLog("정지", "서보 모터 OFF", "TM", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.ServoOff, "서보 모터 OFF"));
                await commandQueue.ExecuteAsync();

                // 8. PM1 램프 OFF 및 문 강제 닫기
                progressForm.NextStep("PM1 정지 (램프 OFF, 문 닫기)");
                LogManager.Instance.AddLog("정지", "PM1 램프 OFF", "PM1", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM1 램프 OFF", processModuleA));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM1 문 강제 닫기", processModuleA));
                await commandQueue.ExecuteAsync();

                // 9. PM2 램프 OFF 및 문 강제 닫기
                progressForm.NextStep("PM2 정지 (램프 OFF, 문 닫기)");
                LogManager.Instance.AddLog("정지", "PM2 램프 OFF", "PM2", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM2 램프 OFF", processModuleB));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM2 문 강제 닫기", processModuleB));
                await commandQueue.ExecuteAsync();

                // 10. PM3 램프 OFF 및 문 강제 닫기
                progressForm.NextStep("PM3 정지 (램프 OFF, 문 닫기)");
                LogManager.Instance.AddLog("정지", "PM3 램프 OFF", "PM3", LogCategory.Hardware, false);
                commandQueue.Clear();
                commandQueue.Enqueue(new ProcessCommand(CommandType.SetPMLampOff, "PM3 램프 OFF", processModuleC));
                commandQueue.Enqueue(new ProcessCommand(CommandType.ForceClosePMDoor, "PM3 문 강제 닫기", processModuleC));
                await commandQueue.ExecuteAsync();

                // TM 애니메이션 완료 대기
                progressForm.NextStep("TM UI 애니메이션 완료 대기");
                await animationTask;

                // 정지 완료 로그
                LogManager.Instance.AddLog("정지", "장비 정지 완료", "System", LogCategory.System, false);

                progressForm.SetStopComplete();
                await Task.Delay(1000);
                progressForm.Close();
            }
            catch (Exception ex)
            {
                LogManager.Instance.Alarm($"장비 정지 오류: {ex.Message}", "System");
                progressForm.SetError(ex.Message);
                await Task.Delay(2000);
                progressForm.Close();

                MessageBox.Show($"장비 정지 중 오류 발생:\n{ex.Message}",
                    "정지 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
