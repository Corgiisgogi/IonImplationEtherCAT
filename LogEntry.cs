using System;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 로그 종류 열거형
    /// </summary>
    public enum LogCategory
    {
        Transfer,   // 웨이퍼 이동 (FOUP↔TM↔PM)
        Process,    // 공정 시작/완료
        Hardware,   // 하드웨어 동작 (모터, 도어, 실린더 등)
        System,     // 시스템 이벤트 (로그인, 연결 등)
        Error,      // 오류 발생 (공정 진행 불가)
        Warning     // 경고 (공정 진행 가능하나 주의 필요)
    }

    /// <summary>
    /// 로그 엔트리 데이터 모델
    /// </summary>
    [Serializable]
    public class LogEntry
    {
        /// <summary>이벤트 발생 시각</summary>
        public DateTime Time { get; set; }

        /// <summary>작업/공정 정보 (LogView용)</summary>
        public string Job { get; set; }

        /// <summary>상세 설명</summary>
        public string Description { get; set; }

        /// <summary>발생 위치 (PM1, PM2, PM3, TM 등)</summary>
        public string Location { get; set; }

        /// <summary>로그 종류</summary>
        public LogCategory Category { get; set; }

        /// <summary>알람 여부 (true=AlarmView+LogView, false=LogView만)</summary>
        public bool IsAlarm { get; set; }

        /// <summary>워닝 여부 (true=AlarmView+LogView에 노란색으로 표시)</summary>
        public bool IsWarning { get; set; }

        /// <summary>복구됨 여부 (Alarm/Warning 해당)</summary>
        public bool IsRestored { get; set; }

        /// <summary>기본 생성자</summary>
        public LogEntry()
        {
            Time = DateTime.Now;
            Job = "";
            Description = "";
            Location = "";
            Category = LogCategory.System;
            IsAlarm = false;
            IsWarning = false;
            IsRestored = false;
        }

        /// <summary>로그 생성 생성자</summary>
        public LogEntry(string job, string description, string location, LogCategory category, bool isAlarm)
        {
            Time = DateTime.Now;
            Job = job ?? "";
            Description = description ?? "";
            Location = location ?? "";
            Category = category;
            IsAlarm = isAlarm;
            IsWarning = false;
            IsRestored = false;
        }

        /// <summary>CSV 헤더 반환</summary>
        public static string GetCSVHeader()
        {
            return "Time,Job,Description,Location,Category,IsAlarm,IsWarning,IsRestored";
        }

        /// <summary>CSV 형식 문자열 반환</summary>
        public string ToCSVLine()
        {
            // CSV에서 쉼표와 따옴표 처리
            string escapedJob = EscapeCSV(Job);
            string escapedDesc = EscapeCSV(Description);
            string escapedLoc = EscapeCSV(Location);

            return $"{Time:yyyy-MM-dd HH:mm:ss},{escapedJob},{escapedDesc},{escapedLoc},{Category},{IsAlarm},{IsWarning},{IsRestored}";
        }

        /// <summary>CSV 라인에서 LogEntry 파싱</summary>
        public static LogEntry FromCSVLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return null;

            try
            {
                var parts = ParseCSVLine(line);
                if (parts.Length < 7)
                    return null;

                var entry = new LogEntry
                {
                    Time = DateTime.Parse(parts[0]),
                    Job = parts[1],
                    Description = parts[2],
                    Location = parts[3],
                    Category = (LogCategory)Enum.Parse(typeof(LogCategory), parts[4]),
                    IsAlarm = bool.Parse(parts[5]),
                    // 기존 CSV 호환성: IsWarning 필드가 없으면 false
                    IsWarning = parts.Length > 7 ? bool.Parse(parts[6]) : false,
                    IsRestored = parts.Length > 7 ? bool.Parse(parts[7]) : bool.Parse(parts[6])
                };

                return entry;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>CSV 필드 이스케이프 처리</summary>
        private static string EscapeCSV(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }

        /// <summary>CSV 라인 파싱 (따옴표 처리 포함)</summary>
        private static string[] ParseCSVLine(string line)
        {
            var result = new System.Collections.Generic.List<string>();
            bool inQuotes = false;
            var currentField = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            currentField.Append('"');
                            i++; // 다음 따옴표 건너뛰기
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        result.Add(currentField.ToString());
                        currentField.Clear();
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
            }

            result.Add(currentField.ToString());
            return result.ToArray();
        }
    }
}
