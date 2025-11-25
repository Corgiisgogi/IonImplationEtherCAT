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
    public partial class MainForm : Form
    {
        private MainView mainView;
        private AlarmView alarmView;
        private RecipeView recipeView;
        private LogView logView;

        public static bool IsLogined {  get; set; }
        public static bool IsConnected { get; set; }

        public MainForm()
        {
            InitializeComponent();

            // 폼이 로드될 때 각 화면 인스턴스 생성
            mainView = new MainView();
            alarmView = new AlarmView();
            recipeView = new RecipeView();
            logView = new LogView();

            // 처음 보여줄 화면 설정
            ShowView(mainView);
            
            // 초기에는 Connect 버튼 비활성화 (로그인 필요)
            btnConnect.Enabled = false;
        }

        /// <summary>
        /// panelContent에 원하는 UserControl을 띄우는 헬퍼 메서드
        /// </summary>
        private void ShowView(UserControl viewToShow)
        {
            panelContent.Controls.Clear(); // 기존 컨트롤 모두 제거
            viewToShow.Dock = DockStyle.Fill; // 새 컨트롤을 꽉 채움
            panelContent.Controls.Add(viewToShow); // 패널에 추가
        }

        /// <summary>
        /// 로그인과 연결이 모두 완료되었을 때 모든 버튼 활성화
        /// </summary>
        private void ActivateAllButtons()
        {
            if (IsLogined && IsConnected)
            {
                recipeView.ActivateToggle(true);
                mainView.ActivateButtons(true);
            }
            else
            {
                recipeView.ActivateToggle(false);
                mainView.ActivateButtons(false);
            }
        }

        /// <summary>
        /// 연결 상태에 따라 상태 표시 패널의 색상 변경
        /// </summary>
        private void UpdateStatusPanelColors(bool connected)
        {
            if (connected)
            {
                // 연결됨 - 초록색으로 변경
                panelTmStatusView.BackColor = Color.LimeGreen;
                panelPm1StatusView.BackColor = Color.LimeGreen;
                panelPm2StatusView.BackColor = Color.LimeGreen;
                panelPm3StatusView.BackColor = Color.LimeGreen;
            }
            else
            {
                // 연결 해제 - 회색으로 변경
                panelTmStatusView.BackColor = Color.DarkGray;
                panelPm1StatusView.BackColor = Color.DarkGray;
                panelPm2StatusView.BackColor = Color.DarkGray;
                panelPm3StatusView.BackColor = Color.DarkGray;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!IsConnected)
            {
                // Connect 동작
                // TODO: 실제 연결 로직은 여기에 구현
                // 현재는 바로 연결 성공으로 처리
                IsConnected = true;
                btnConnect.Text = "Disconnect";
                lblEtherCatStatus.Text = "Connected";
                lblSlaveStatus.Text = "Connected";
                
                // 상태 패널 색상 변경 (초록색 - 정상 작동)
                UpdateStatusPanelColors(true);
                
                MessageBox.Show("연결되었습니다!", "연결 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 로그인과 연결이 모두 완료되었는지 확인 후 버튼 활성화
                ActivateAllButtons();
            }
            else
            {
                // Disconnect 동작
                // 공정이 진행 중인지 확인
                if (mainView.IsProcessRunning())
                {
                    MessageBox.Show(
                        "공정이 진행 중입니다.\n연결을 해제하려면 먼저 공정을 중지해주세요.",
                        "연결 해제 불가",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // TODO: 실제 연결 해제 로직은 여기에 구현
                IsConnected = false;
                btnConnect.Text = "Connect";
                lblEtherCatStatus.Text = "-";
                lblSlaveStatus.Text = "-";
                
                // 상태 패널 색상 변경 (회색 - 연결 해제)
                UpdateStatusPanelColors(false);
                
                MessageBox.Show("연결이 해제되었습니다.", "연결 해제", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 연결 해제 시 버튼 비활성화
                ActivateAllButtons();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string adminId = "admin";
            string adminPw = "1234";
            bool loginSuccess = false;

            if (textBoxId.Text == adminId && textBoxPw.Text == adminPw) 
            { 
                loginSuccess = true; 
                IsLogined = true;  
            }

            if (loginSuccess)
            {
                MessageBox.Show("로그인 성공!");
                // 로그인 성공 시
                lblCurrentUserName.Text = "User: " + textBoxId.Text; // 사용자 이름 표시
                panelLoginView.Visible = false;     // 로그인 뷰 숨기기
                panelUserInfoView.Visible = true;   // 사용자 정보 뷰 보여주기

                // 로그인 성공 시 Connect 버튼 활성화
                btnConnect.Enabled = true;

                // 로그인과 연결이 모두 완료되었는지 확인 후 버튼 활성화
                ActivateAllButtons();
            }
            else
            {
                MessageBox.Show("로그인 실패!");
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // 연결 상태 확인
            if (IsConnected)
            {
                MessageBox.Show(
                    "연결된 상태입니다.\n로그아웃하려면 먼저 연결을 해제해주세요.",
                    "로그아웃 불가",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 로그아웃 시
            textBoxId.Text = "";
            textBoxPw.Text = "";
            IsLogined = false;
            panelLoginView.Visible = true;      // 로그인 뷰 보여주기
            panelUserInfoView.Visible = false; // 사용자 정보 뷰 숨기기
            
            // Connect 버튼 비활성화 (로그인 필요)
            btnConnect.Enabled = false;
            
            // 버튼 비활성화
            ActivateAllButtons();
        }

        // --- 푸터 버튼 클릭 이벤트 핸들러 ---
        private void btnMain_Click(object sender, EventArgs e)
        {
            ShowView(mainView);
        }

        private void btnAlarmManager_Click(object sender, EventArgs e)
        {
            ShowView(alarmView);
        }

        private void btnRecipe_Click(object sender, EventArgs e)
        {
            ShowView(recipeView);
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            ShowView(logView);
        }
    }
}
