using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 로그 관리 싱글톤 서비스
    /// - 실시간 CSV 자동 저장
    /// - 알람 관리 및 복구
    /// - UI 이벤트 발생
    /// </summary>
    public class LogManager
    {
        #region 싱글톤

        private static readonly Lazy<LogManager> _instance = new Lazy<LogManager>(() => new LogManager());

        /// <summary>싱글톤 인스턴스</summary>
        public static LogManager Instance => _instance.Value;

        private LogManager()
        {
            _logs = new List<LogEntry>();
            InitializeLogFolder();
            LoadTodayLogs();
        }

        #endregion

        #region 필드 및 속성

        private readonly List<LogEntry> _logs;
        private readonly object _lockObject = new object();
        private string _logFolderPath;

        /// <summary>로그 폴더 경로</summary>
        public string LogFolderPath => _logFolderPath;

        /// <summary>활성 알람/워닝 존재 여부</summary>
        public bool HasActiveAlarms
        {
            get
            {
                lock (_lockObject)
                {
                    return _logs.Any(l => (l.IsAlarm || l.IsWarning) && !l.IsRestored);
                }
            }
        }

        #endregion

        #region 이벤트

        /// <summary>새 로그 추가 시 발생</summary>
        public event Action<LogEntry> OnLogAdded;

        /// <summary>알람 복구 시 발생</summary>
        public event Action<LogEntry> OnAlarmRestored;

        /// <summary>로그 파일 로드 완료 시 발생</summary>
        public event Action OnLogsLoaded;

        #endregion

        #region 초기화

        /// <summary>로그 폴더 초기화</summary>
        private void InitializeLogFolder()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            _logFolderPath = Path.Combine(appPath, "Logs");

            if (!Directory.Exists(_logFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(_logFolderPath);
                }
                catch (Exception)
                {
                    // 폴더 생성 실패 시 무시
                }
            }
        }

        /// <summary>오늘 날짜의 로그 파일 로드</summary>
        private void LoadTodayLogs()
        {
            string todayLogFile = GetDailyLogFilePath(DateTime.Today);
            if (File.Exists(todayLogFile))
            {
                LoadFromCSVInternal(todayLogFile);
            }
        }

        #endregion

        #region 로그 추가

        /// <summary>로그 추가</summary>
        /// <param name="job">작업/공정 정보</param>
        /// <param name="description">상세 설명</param>
        /// <param name="location">발생 위치</param>
        /// <param name="category">로그 종류</param>
        /// <param name="isAlarm">알람 여부</param>
        public void AddLog(string job, string description, string location, LogCategory category, bool isAlarm = false)
        {
            var entry = new LogEntry(job, description, location, category, isAlarm);

            lock (_lockObject)
            {
                _logs.Add(entry);
            }

            // 실시간 파일 저장
            AppendToLogFile(entry);

            // 이벤트 발생
            OnLogAdded?.Invoke(entry);
        }

        /// <summary>일반 로그 추가 (간편 버전)</summary>
        public void Log(string description, string location = "", LogCategory category = LogCategory.System)
        {
            AddLog("", description, location, category, false);
        }

        /// <summary>알람 로그 추가 (간편 버전)</summary>
        public void Alarm(string description, string location, string job = "")
        {
            AddLog(job, description, location, LogCategory.Error, true);
        }

        /// <summary>워닝 로그 추가 (AlarmView에 노란색으로 표시)</summary>
        /// <param name="description">상세 설명</param>
        /// <param name="location">발생 위치</param>
        /// <param name="job">작업/공정 정보</param>
        /// <param name="isRestored">처음부터 해결된 상태로 표시 (기본: false)</param>
        public void Warning(string description, string location, string job = "", bool isRestored = false)
        {
            var entry = new LogEntry
            {
                Time = DateTime.Now,
                Job = job ?? "",
                Description = description ?? "",
                Location = location ?? "",
                Category = LogCategory.Warning,
                IsAlarm = false,
                IsWarning = true,
                IsRestored = isRestored
            };

            lock (_lockObject)
            {
                _logs.Add(entry);
            }

            // 실시간 파일 저장
            AppendToLogFile(entry);

            // 이벤트 발생
            OnLogAdded?.Invoke(entry);
        }

        #endregion

        #region 알람 복구

        /// <summary>알람/워닝 복구</summary>
        /// <param name="alarm">복구할 알람/워닝</param>
        /// <returns>복구 성공 여부</returns>
        public bool RestoreAlarm(LogEntry alarm)
        {
            if (alarm == null || (!alarm.IsAlarm && !alarm.IsWarning))
                return false;

            lock (_lockObject)
            {
                alarm.IsRestored = true;
            }

            // 파일에 전체 로그 다시 저장 (복구 상태 반영)
            SaveTodayLogs();

            // 이벤트 발생
            OnAlarmRestored?.Invoke(alarm);

            return true;
        }

        /// <summary>모든 활성 알람/워닝 복구</summary>
        /// <returns>복구된 알람/워닝 수</returns>
        public int RestoreAllAlarms()
        {
            int count = 0;

            lock (_lockObject)
            {
                foreach (var alarm in _logs.Where(l => (l.IsAlarm || l.IsWarning) && !l.IsRestored))
                {
                    alarm.IsRestored = true;
                    OnAlarmRestored?.Invoke(alarm);
                    count++;
                }
            }

            if (count > 0)
            {
                SaveTodayLogs();
            }

            return count;
        }

        #endregion

        #region 로그 조회

        /// <summary>모든 로그 반환</summary>
        public List<LogEntry> GetAllLogs()
        {
            lock (_lockObject)
            {
                return new List<LogEntry>(_logs);
            }
        }

        /// <summary>활성 알람/워닝만 반환 (IsRestored=false)</summary>
        public List<LogEntry> GetActiveAlarms()
        {
            lock (_lockObject)
            {
                return _logs.Where(l => (l.IsAlarm || l.IsWarning) && !l.IsRestored).ToList();
            }
        }

        /// <summary>모든 알람/워닝 반환 (복구된 것 포함)</summary>
        public List<LogEntry> GetAllAlarms()
        {
            lock (_lockObject)
            {
                return _logs.Where(l => l.IsAlarm || l.IsWarning).ToList();
            }
        }

        #endregion

        #region 파일 저장/로드

        /// <summary>일별 로그 파일 경로 반환</summary>
        private string GetDailyLogFilePath(DateTime date)
        {
            return Path.Combine(_logFolderPath, $"log_{date:yyyy-MM-dd}.csv");
        }

        /// <summary>로그 엔트리를 파일에 추가</summary>
        private void AppendToLogFile(LogEntry entry)
        {
            try
            {
                string filePath = GetDailyLogFilePath(DateTime.Today);
                bool fileExists = File.Exists(filePath);

                using (var writer = new StreamWriter(filePath, true, System.Text.Encoding.UTF8))
                {
                    if (!fileExists)
                    {
                        writer.WriteLine(LogEntry.GetCSVHeader());
                    }
                    writer.WriteLine(entry.ToCSVLine());
                }
            }
            catch (Exception)
            {
                // 파일 저장 실패 시 무시
            }
        }

        /// <summary>오늘 로그 전체 저장 (복구 상태 반영용)</summary>
        private void SaveTodayLogs()
        {
            try
            {
                string filePath = GetDailyLogFilePath(DateTime.Today);
                var todayLogs = _logs.Where(l => l.Time.Date == DateTime.Today).ToList();

                using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(LogEntry.GetCSVHeader());
                    foreach (var entry in todayLogs)
                    {
                        writer.WriteLine(entry.ToCSVLine());
                    }
                }
            }
            catch (Exception)
            {
                // 파일 저장 실패 시 무시
            }
        }

        /// <summary>CSV 파일로 내보내기 (수동 저장)</summary>
        /// <param name="filePath">저장할 파일 경로</param>
        /// <param name="logsToSave">저장할 로그 목록 (null이면 전체 로그)</param>
        /// <returns>저장 성공 여부</returns>
        public bool SaveToCSV(string filePath, List<LogEntry> logsToSave = null)
        {
            try
            {
                var logs = logsToSave ?? GetAllLogs();

                using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(LogEntry.GetCSVHeader());
                    foreach (var entry in logs)
                    {
                        writer.WriteLine(entry.ToCSVLine());
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>CSV 파일에서 로그 불러오기</summary>
        /// <param name="filePath">불러올 파일 경로</param>
        /// <returns>불러오기 성공 여부</returns>
        public bool LoadFromCSV(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            bool result = LoadFromCSVInternal(filePath);

            if (result)
            {
                OnLogsLoaded?.Invoke();
            }

            return result;
        }

        /// <summary>CSV 파일 내부 로드 (이벤트 없이)</summary>
        private bool LoadFromCSVInternal(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);

                lock (_lockObject)
                {
                    foreach (var line in lines.Skip(1)) // 헤더 건너뛰기
                    {
                        var entry = LogEntry.FromCSVLine(line);
                        if (entry != null && !_logs.Any(l => l.Time == entry.Time && l.Description == entry.Description))
                        {
                            _logs.Add(entry);
                        }
                    }

                    // 시간순 정렬
                    _logs.Sort((a, b) => a.Time.CompareTo(b.Time));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>로그 폴더 경로 반환</summary>
        public string GetLogFolderPath()
        {
            return _logFolderPath;
        }

        #endregion

        #region 로그 초기화

        /// <summary>메모리 내 로그 초기화 (파일은 유지)</summary>
        public void ClearMemoryLogs()
        {
            lock (_lockObject)
            {
                _logs.Clear();
            }
            OnLogsLoaded?.Invoke();
        }

        #endregion
    }
}
