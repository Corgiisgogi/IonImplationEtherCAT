using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    internal class Wafer
    {
        public enum WaferState
        {
            ProcessNone,
            IonProcessComplete,
            AnnealingProcessComplete,
            Error
        };

        public WaferState CurrentState { get; set; }
    }
}
