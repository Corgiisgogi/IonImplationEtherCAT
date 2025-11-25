using System;
using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 공정 명령 타입
    /// </summary>
    public enum CommandType
    {
        RotateTM,           // TM 회전
        ExtendArm,          // 암 확장
        RetractArm,         // 암 수축
        PickWafer,          // 웨이퍼 픽업
        PlaceWafer,         // 웨이퍼 배치
        StartProcess,       // 공정 시작
        WaitForCompletion,  // 완료 대기
        RemoveWaferFromFoup, // FOUP에서 웨이퍼 제거
        AddWaferToFoup,     // FOUP에 웨이퍼 추가
        Delay,              // 지연
        UnloadWaferFromPM,  // PM에서 웨이퍼 언로드
        WaitForProcessComplete, // PM 공정 완료 대기
        LoadWaferToPM       // PM에 웨이퍼 로드
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

