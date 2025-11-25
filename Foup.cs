using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// FOUP (Front Opening Unified Pod) 클래스
    /// 웨이퍼 5개를 보관하는 컨테이너
    /// </summary>
    internal class Foup
    {
        // 웨이퍼 슬롯 (5개)
        public bool[] WaferSlots { get; set; }

        /// <summary>
        /// FOUP이 완전히 찼는지 확인
        /// </summary>
        public bool IsFull
        {
            get
            {
                return WaferSlots.All(slot => slot == true);
            }
        }

        /// <summary>
        /// FOUP이 완전히 비었는지 확인
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return WaferSlots.All(slot => slot == false);
            }
        }

        /// <summary>
        /// 현재 장착된 웨이퍼 개수
        /// </summary>
        public int WaferCount
        {
            get
            {
                return WaferSlots.Count(slot => slot == true);
            }
        }

        public Foup()
        {
            // 5개의 웨이퍼 슬롯 초기화 (모두 비어있음)
            WaferSlots = new bool[5];
            for (int i = 0; i < 5; i++)
            {
                WaferSlots[i] = false;
            }
        }

        /// <summary>
        /// 모든 웨이퍼 슬롯에 웨이퍼 장착
        /// </summary>
        public void LoadWafers()
        {
            for (int i = 0; i < 5; i++)
            {
                WaferSlots[i] = true;
            }
        }

        /// <summary>
        /// 모든 웨이퍼 슬롯에서 웨이퍼 제거
        /// </summary>
        public void UnloadWafers()
        {
            for (int i = 0; i < 5; i++)
            {
                WaferSlots[i] = false;
            }
        }

        /// <summary>
        /// 특정 슬롯에 웨이퍼 장착
        /// </summary>
        public bool LoadWafer(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 5)
                return false;

            WaferSlots[slotIndex] = true;
            return true;
        }

        /// <summary>
        /// 특정 슬롯에서 웨이퍼 제거
        /// </summary>
        public bool UnloadWafer(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 5)
                return false;

            WaferSlots[slotIndex] = false;
            return true;
        }
    }
}

