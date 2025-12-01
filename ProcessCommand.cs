using System;
using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 공정 명령 타입
    /// </summary>
    public enum CommandType
    {
        // === 기존 시뮬레이션 명령 ===
        RotateTM,           // TM 회전 (시뮬레이션)
        ExtendArm,          // 암 확장 (시뮬레이션)
        RetractArm,         // 암 수축 (시뮬레이션)
        PickWafer,          // 웨이퍼 픽업 (데이터 처리)
        PlaceWafer,         // 웨이퍼 배치 (데이터 처리)
        StartProcess,       // 공정 시작
        WaitForCompletion,  // 완료 대기
        RemoveWaferFromFoup, // FOUP에서 웨이퍼 제거
        AddWaferToFoup,     // FOUP에 웨이퍼 추가
        Delay,              // 지연
        UnloadWaferFromPM,  // PM에서 웨이퍼 언로드
        WaitForProcessComplete, // PM 공정 완료 대기
        LoadWaferToPM,      // PM에 웨이퍼 로드
        InitializeParameters,   // 파라미터 상승 시작 (공정 전 안정화)
        WaitForParameterStabilization, // 파라미터 안정화 대기 (목표값 도달까지)

        // === 실제 하드웨어 명령 ===
        // 축 제어
        HomeUDAxis,         // UD축 원점복귀
        HomeLRAxis,         // LR축 원점복귀
        MoveUDAxis,         // UD축 이동 (파라미터: long position)
        MoveLRAxis,         // LR축 이동 (파라미터: long position)

        // PM 문/램프 제어
        OpenPMDoor,         // PM 문 열기 (파라미터: ProcessModule)
        ClosePMDoor,        // PM 문 닫기 (파라미터: ProcessModule)
        ForceClosePMDoor,   // PM 문 강제 닫기 (초기화용, 파라미터: ProcessModule)
        SetPMLampOn,        // PM 램프 ON (파라미터: ProcessModule)
        SetPMLampOff,       // PM 램프 OFF (파라미터: ProcessModule)

        // TM 실린더/흡착 제어
        ExtendCylinder,     // 실린더 전진
        RetractCylinder,    // 실린더 후진
        EnableSuction,      // 흡착 ON
        DisableSuction,     // 흡착 OFF
        EnableExhaust,      // 배기 ON
        DisableExhaust,     // 배기 OFF

        // 서보 제어
        ServoOn,            // 모든 서보 ON (UD + LR)
        ServoOff            // 모든 서보 OFF (UD + LR)
    }

    /// <summary>
    /// 공정 명령 클래스
    /// </summary>
    public class ProcessCommand
    {
        public CommandType Type { get; set; }
        public object[] Parameters { get; set; }
        public bool IsCompleted { get; set; }
        public string Description { get; set; }
        public Wafer ResultWafer { get; set; } // 명령 실행 결과로 얻은 웨이퍼

        public ProcessCommand(CommandType type, params object[] parameters)
        {
            Type = type;
            Parameters = parameters;
            IsCompleted = false;
            Description = type.ToString();
        }

        public ProcessCommand(CommandType type, string description, params object[] parameters)
        {
            Type = type;
            Parameters = parameters;
            IsCompleted = false;
            Description = description;
        }
    }
}

