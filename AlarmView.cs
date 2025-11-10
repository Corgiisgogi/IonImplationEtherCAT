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
    public partial class AlarmView : UserControl
    {
        public AlarmView()
        {
            InitializeComponent();
            LoadAlarms();
        }

        // 디자이너에 추가된 컨트롤:
        // DataGridView: dgvAlarms
        // Button: btnRestore

        private void AlarmView_Load(object sender, EventArgs e)
        {
            // 이 화면이 로드될 때 알람 데이터를 불러오는 함수 호출
            LoadAlarms();
        }

        private void LoadAlarms()
        {
            // (임시 데이터 추가)
            // 실제로는 DB나 파일에서 알람 이력을 읽어와야 함
            dgvAlarms.Rows.Clear(); // 기존 데이터 지우기

            // DataGridView에 컬럼이 미리 디자인되어 있어야 함 (시각, 발생 위치, 알람 종류, 상세)
            dgvAlarms.Rows.Add("2025/11/03 16:20", "PM1", "장비 에러", "압력 기준치 초과");

            // 최신 알람이 빨간색으로 보이도록 (예시)
            if (dgvAlarms.Rows.Count > 0)
            {
                dgvAlarms.Rows[0].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            // 알람 복구(해제) 로직
            // 선택된 알람을 dgvAlarms에서 지우거나 상태를 변경
            MessageBox.Show("알람이 복구되었습니다.");
            LoadAlarms(); // 목록 새로고침
        }
    }
}
