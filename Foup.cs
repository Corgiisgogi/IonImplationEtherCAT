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
    public class Foup
    {
        // 웨이퍼 슬롯 (5개) - Wafer 객체 배열로 변경
        public Wafer[] WaferSlots { get; set; }

        /// <summary>
        /// FOUP이 완전히 찼는지 확인
        /// </summary>
        public bool IsFull
        {
            get
            {
                return WaferSlots.All(slot => slot != null);
            }
        }

        /// <summary>
        /// FOUP이 완전히 비었는지 확인
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return WaferSlots.All(slot => slot == null);
            }
        }

        /// <summary>
        /// 현재 장착된 웨이퍼 개수
        /// </summary>
        public int WaferCount
        {
            get
            {
                return WaferSlots.Count(slot => slot != null);
            }
        }

        public Foup()
        {
            // 5개의 웨이퍼 슬롯 초기화 (모두 비어있음)
            WaferSlots = new Wafer[5];
            for (int i = 0; i < 5; i++)
            {
                WaferSlots[i] = null;
            }
        }

        /// <summary>
        /// 모든 웨이퍼 슬롯에 웨이퍼 장착
        /// </summary>
        public void LoadWafers()
        {
            for (int i = 0; i < 5; i++)
            {
                WaferSlots[i] = new Wafer();
            }
        }

        /// <summary>
        /// 모든 웨이퍼 슬롯에서 웨이퍼 제거
        /// </summary>
        public void UnloadWafers()
        {
            for (int i = 0; i < 5; i++)
            {
                WaferSlots[i] = null;
            }
        }

        /// <summary>
        /// 특정 슬롯에 웨이퍼 장착
        /// </summary>
        public bool LoadWafer(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 5)
                return false;

            WaferSlots[slotIndex] = new Wafer();
            return true;
        }

        /// <summary>
        /// 특정 슬롯에 웨이퍼 장착 (기존 웨이퍼 객체 사용)
        /// </summary>
        public bool LoadWafer(int slotIndex, Wafer wafer)
        {
            if (slotIndex < 0 || slotIndex >= 5)
                return false;

            WaferSlots[slotIndex] = wafer;
            return true;
        }

        /// <summary>
        /// 특정 슬롯에서 웨이퍼 제거 및 반환
        /// </summary>
        public Wafer UnloadWafer(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 5)
                return null;

            Wafer wafer = WaferSlots[slotIndex];
            WaferSlots[slotIndex] = null;
            return wafer;
        }

        /// <summary>
        /// 특정 슬롯에 웨이퍼가 있는지 확인
        /// </summary>
        public bool HasWafer(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 5)
                return false;

            return WaferSlots[slotIndex] != null;
        }
    }
}

