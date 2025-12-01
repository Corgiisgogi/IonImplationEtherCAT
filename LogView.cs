using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
            SubscribeToLogManager();
            SetupEventHandlers();
        }

        /// <summary>
        /// 이벤트 핸들러 설정
        /// </summary>
        private void SetupEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            btnLoad.Click += btnLoad_Click;
            this.Load += LogView_Load;
        }

        /// <summary>
        /// LogManager 이벤트 구독
        /// </summary>
        private void SubscribeToLogManager()
        {
            LogManager.Instance.OnLogAdded += OnLogAdded;
            LogManager.Instance.OnLogsLoaded += OnLogsLoaded;
        }

        /// <summary>
        /// 새 로그 추가 시 호출
        /// </summary>
        private void OnLogAdded(LogEntry entry)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<LogEntry>(OnLogAdded), entry);
                return;
            }

            // 최신 로그를 맨 위에 추가
            InsertLogRow(0, entry);
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

            RefreshLogs();
        }

        private void LogView_Load(object sender, EventArgs e)
        {
            RefreshLogs();
        }

        /// <summary>
        /// 로그 목록 새로고침
        /// </summary>
        private void RefreshLogs()
        {
            dgvAlarms.Rows.Clear();

            // 모든 로그 가져오기 (Alarm + 일반 로그)
            var logs = LogManager.Instance.GetAllLogs();

            // 최신순으로 정렬
            logs = logs.OrderByDescending(l => l.Time).ToList();

            foreach (var log in logs)
            {
                AddLogRow(log);
            }
        }

        /// <summary>
        /// DataGridView에 로그 행 추가
        /// </summary>
        private void AddLogRow(LogEntry log)
        {
            // 컬럼 순서: 시각, Job, 상세
            int rowIndex = dgvAlarms.Rows.Add(
                log.Time.ToString("yyyy/MM/dd HH:mm:ss"),
                log.Job,
                log.Description
            );

            // 알람 로그는 다른 색상으로 표시
            SetRowColor(rowIndex, log);
        }

        /// <summary>
        /// DataGridView 특정 위치에 로그 행 삽입
        /// </summary>
        private void InsertLogRow(int index, LogEntry log)
        {
            // 컬럼 순서: 시각, Job, 상세
            dgvAlarms.Rows.Insert(index,
                log.Time.ToString("yyyy/MM/dd HH:mm:ss"),
                log.Job,
                log.Description
            );

            // 알람 로그는 다른 색상으로 표시
            SetRowColor(index, log);
        }

        /// <summary>
        /// 행 색상 설정
        /// </summary>
        private void SetRowColor(int rowIndex, LogEntry log)
        {
            if (log.IsAlarm)
            {
                if (log.IsRestored)
                {
                    // 복구된 알람: 연한 회색
                    dgvAlarms.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                }
                else
                {
                    // 활성 알람: 연한 빨간색
                    dgvAlarms.Rows[rowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                }
            }
            else
            {
                // 일반 로그: 기본 색상
                dgvAlarms.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 저장 버튼 클릭 - CSV 내보내기
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV 파일 (*.csv)|*.csv";
                sfd.FileName = $"logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                sfd.InitialDirectory = LogManager.Instance.GetLogFolderPath();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (LogManager.Instance.SaveToCSV(sfd.FileName))
                    {
                        MessageBox.Show("로그가 저장되었습니다.", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("저장에 실패했습니다.", "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 불러오기 버튼 클릭 - CSV 불러오기
        /// </summary>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV 파일 (*.csv)|*.csv";
                ofd.InitialDirectory = LogManager.Instance.GetLogFolderPath();

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (LogManager.Instance.LoadFromCSV(ofd.FileName))
                    {
                        MessageBox.Show("로그를 불러왔습니다.", "불러오기 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("불러오기에 실패했습니다.", "불러오기 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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
