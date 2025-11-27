using System;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 타워 램프 상태
    /// Digital Output 채널: 0(적색), 1(황색), 2(녹색)
    /// </summary>
    public enum TowerLampState
    {
        /// <summary>
        /// 모든 램프 OFF
        /// </summary>
        Off,

        /// <summary>
        /// 녹색 - 공정 정상 진행 중
        /// </summary>
        Green,

        /// <summary>
        /// 황색 - 대기(시작 전/종료 시) 또는 지연(FOUP B 가득 참 등)
        /// </summary>
        Yellow,

        /// <summary>
        /// 적색 - 공정 진행 불가 오류 발생
        /// </summary>
        Red
    }
}
