using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    public class Wafer
    {
        public enum WaferState
        {
            ProcessNone,
            IonProcessComplete,
            AnnealingProcessComplete,
            Error
        };

        // 웨이퍼 고유 ID
        public int WaferId { get; set; }

        // 현재 공정 상태
        public WaferState CurrentState { get; set; }

        // 생성 시간 (추적 및 디버깅용)
        public DateTime CreatedTime { get; set; }

        // 정적 ID 카운터
        private static int nextId = 1;

        public Wafer()
        {
            WaferId = nextId++;
            CurrentState = WaferState.ProcessNone;
            CreatedTime = DateTime.Now;
        }

        public Wafer(int id)
        {
            WaferId = id;
            CurrentState = WaferState.ProcessNone;
            CreatedTime = DateTime.Now;
        }

        /// <summary>
        /// 웨이퍼 상태를 다음 단계로 업데이트
        /// </summary>
        public void UpdateState(WaferState newState)
        {
            CurrentState = newState;
        }

        /// <summary>
        /// 웨이퍼 정보를 문자열로 반환
        /// </summary>
        public override string ToString()
        {
            return $"Wafer #{WaferId} - State: {CurrentState}";
        }
    }
}
