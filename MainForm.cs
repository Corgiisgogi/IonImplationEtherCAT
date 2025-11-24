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

        private void btnConnect_Click(object sender, EventArgs e)
        {

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

                //기타 버튼 전부 활성화
                recipeView.ActivateToggle(true);
            }
            else
            {
                MessageBox.Show("로그인 실패!");
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // 로그아웃 시
            textBoxId.Text = "";
            textBoxPw.Text = "";
            panelLoginView.Visible = true;      // 로그인 뷰 보여주기
            panelUserInfoView.Visible = false; // 사용자 정보 뷰 숨기기
            recipeView.ActivateToggle(false);
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
