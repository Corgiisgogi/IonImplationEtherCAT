using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IonImplationEtherCAT
{
    internal class ProcessModule
    {
        // 상태 정의
        public enum State
        {
            Idle,
            Running,
            Paused,
            Stoped,
            Error
        };

        public enum RequestType
        {
            LoadWafer,
            UnloadWafer,
            ErrorReport
        };

        public State ModuleState { get; set; }
        public int processTime { get; set; }
        public int elapsedTime { get; set; }
        public bool isWaferLoaded { get; set; }

        public ProcessModule()
        {
            ModuleState = State.Idle;
            processTime = 0;
            elapsedTime = 0;
            isWaferLoaded = false;
        }

        public void StartProcess(int time)
        {
            processTime = time;
            elapsedTime = 0;
            ModuleState = State.Running;
        }

        public void PauseProcess()
        {
            if (ModuleState == State.Running)
            {
                ModuleState = State.Paused;
            }
        }

        public void ResumeProcess()
        {
            if (ModuleState == State.Paused)
            {
                ModuleState = State.Running;
            }
        }

        public void StopProcess()
        {
            ModuleState = State.Stoped;
            elapsedTime = 0;
        }

        public void UpdateProcess(int timeIncrement)
        {
            if (ModuleState == State.Running)
            {
                elapsedTime += timeIncrement;
                if (elapsedTime >= processTime)
                {
                    ModuleState = State.Idle; // 프로세스 완료 후 대기 상태로 전환
                    elapsedTime = processTime; // 경과 시간을 최대값으로 설정
                }
            }
        }

        public void SendRequest(List<RequestType> requestlist, RequestType request)
        {
            if (request == RequestType.ErrorReport)
            {
                requestlist.Insert(0, request);
            }
            requestlist.Add(request);
        }

    }
}
