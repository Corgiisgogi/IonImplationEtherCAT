using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    public partial class AlarmView : UserControl
    {
        // 현재 표시 중인 알람 목록 (행 인덱스와 매핑용)
        private List<LogEntry> _displayedAlarms = new List<LogEntry>();

        public AlarmView()
        {
            InitializeComponent();
            SubscribeToLogManager();

            // 시작 시 이전 세션의 알람 로드 (LogManager 싱글톤이 이미 로그를 로드한 상태)
            RefreshAlarms();
        }

        /// <summary>
        /// LogManager 이벤트 구독
        /// </summary>
        private void SubscribeToLogManager()
        {
            LogManager.Instance.OnLogAdded += OnLogAdded;
            LogManager.Instance.OnAlarmRestored += OnAlarmRestored;
            LogManager.Instance.OnLogsLoaded += OnLogsLoaded;
        }

        /// <summary>
        /// 새 로그 추가 시 호출 (알람 및 워닝 모두 표시)
        /// </summary>
        private void OnLogAdded(LogEntry entry)
        {
            // 알람 또는 워닝만 표시
            if (!entry.IsAlarm && !entry.IsWarning)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<LogEntry>(OnLogAdded), entry);
                return;
            }

            AddAlarmRow(entry);
        }

        /// <summary>
        /// 알람 복구 시 호출
        /// </summary>
        private void OnAlarmRestored(LogEntry entry)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<LogEntry>(OnAlarmRestored), entry);
                return;
            }

            RefreshAlarms();
        }

        /// <summary>
        /// 로그 파일 로드 완료 시 호출
        /// </summary>
        private void OnLogsLoaded()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnLogsLoaded));
                return;
            }

            RefreshAlarms();
        }

        private void AlarmView_Load(object sender, EventArgs e)
        {
            RefreshAlarms();
        }

        /// <summary>
        /// 알람 목록 새로고침
        /// </summary>
        private void RefreshAlarms()
        {
            dgvAlarms.Rows.Clear();
            _displayedAlarms.Clear();

            // 모든 알람 가져오기 (복구된 것 포함)
            var alarms = LogManager.Instance.GetAllAlarms();

            // 최신순으로 정렬
            alarms = alarms.OrderByDescending(a => a.Time).ToList();

            foreach (var alarm in alarms)
            {
                AddAlarmRow(alarm);
            }
        }

        /// <summary>
        /// DataGridView에 알람/워닝 행 추가
        /// </summary>
        private void AddAlarmRow(LogEntry alarm)
        {
            // 컬럼 순서: 시각, 발생위치, 종류, 상세
            int rowIndex = dgvAlarms.Rows.Add(
                alarm.Time.ToString("yyyy/MM/dd HH:mm:ss"),
                alarm.Location,
                alarm.Category.ToString(),
                alarm.Description
            );

            _displayedAlarms.Add(alarm);

            // 색상 설정 (복구 여부 및 알람/워닝 구분)
            if (alarm.IsRestored)
            {
                // 복구된 알람/워닝: 하얀색 바탕, 검은색 글씨 (일반 로그처럼)
                dgvAlarms.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                dgvAlarms.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
            else if (alarm.IsWarning)
            {
                // 활성 워닝: 노란색
                dgvAlarms.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                dgvAlarms.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
            else
            {
                // 활성 알람: 빨간색
                dgvAlarms.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                dgvAlarms.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// 복구 버튼 클릭
        /// </summary>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (dgvAlarms.SelectedRows.Count == 0)
            {
                MessageBox.Show("복구할 알람을 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int restoredCount = 0;

            foreach (DataGridViewRow row in dgvAlarms.SelectedRows)
            {
                int index = row.Index;
                if (index >= 0 && index < _displayedAlarms.Count)
                {
                    var alarm = _displayedAlarms[index];
                    if (!alarm.IsRestored)
                    {
                        LogManager.Instance.RestoreAlarm(alarm);
                        restoredCount++;
                    }
                }
            }

            if (restoredCount > 0)
            {
                MessageBox.Show($"{restoredCount}개의 알람이 복구되었습니다.", "복구 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("이미 복구된 알람입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 컨트롤 해제 시 이벤트 구독 해제
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LogManager.Instance.OnLogAdded -= OnLogAdded;
                LogManager.Instance.OnAlarmRestored -= OnAlarmRestored;
                LogManager.Instance.OnLogsLoaded -= OnLogsLoaded;

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
