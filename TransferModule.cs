using System;
using System.Drawing;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// Transfer Module (TM) 클래스
    /// 웨이퍼 이송 로봇의 상태와 위치를 관리
    /// </summary>
    public class TransferModule
    {
        // 상태 정의
        public enum TMState
        {
            Idle,           // 대기
            Moving,         // 이동 중
            Rotating,       // 회전 중
            PickingWafer,   // 웨이퍼 픽업 중
            PlacingWafer    // 웨이퍼 배치 중
        }

        // EtherCAT 컨트롤러 참조
        private IEtherCATController etherCATController;

        // 현재 상태
        public TMState State { get; set; }

        // 웨이퍼 보유 여부
        public bool HasWafer { get; set; }

        // 현재 들고 있는 웨이퍼
        public Wafer CurrentWafer { get; set; }

        // 그래픽 구성 요소들의 위치
        public Point ArmHighPosition { get; set; }
        public Point ArmLowPosition { get; set; }
        public Point BackPosition { get; set; }
        public Point BottomPosition { get; set; }  // 중심축 (고정)
        public Point WaferPosition { get; set; }

        // 초기 위치 (참조용)
        private Point initialArmHighPosition;
        private Point initialArmLowPosition;
        private Point initialBackPosition;
        private Point initialBottomPosition;

        // 회전 각도 (도 단위)
        public float CurrentRotationAngle { get; set; }
        public float TargetRotationAngle { get; set; }

        // 회전 속도 (도/초)
        public float RotationSpeed { get; set; }

        // Arm 확장/수축 상태
        public bool IsArmExtended { get; set; }

        // Arm 확장 거리 (픽셀 단위)
        public float CurrentExtension { get; set; }
        public float TargetExtension { get; set; }
        public float MaxExtension { get; set; }

        // Arm 확장/수축 속도 (픽셀/초)
        public float ExtensionSpeed { get; set; }

        // 완료 이벤트 (이벤트 기반 대기용)
        public event Action OnRotationComplete;
        public event Action OnArmMovementComplete;

        public TransferModule()
        {
            State = TMState.Idle;
            HasWafer = false;
            CurrentWafer = null;
            CurrentRotationAngle = 0f;
            TargetRotationAngle = 0f;
            RotationSpeed = 90f; // 초당 90도 회전
            IsArmExtended = false;

            CurrentExtension = 0f;
            TargetExtension = 0f;
            MaxExtension = 150f; // 최대 150픽셀 확장
            ExtensionSpeed = 200f; // 초당 200픽셀
        }

        /// <summary>
        /// 초기 위치 설정
        /// </summary>
        public void SetInitialPositions(Point armHigh, Point armLow, Point bottom, Point back)
        {
            initialArmHighPosition = armHigh;
            initialArmLowPosition = armLow;
            initialBottomPosition = bottom;
            initialBackPosition = back;

            ArmHighPosition = armHigh;
            ArmLowPosition = armLow;
            BottomPosition = bottom;
            BackPosition = back;
        }

        /// <summary>
        /// 목표 회전 각도 설정 (애니메이션)
        /// </summary>
        public void SetTargetRotation(float angle)
        {
            TargetRotationAngle = angle;
            if (Math.Abs(CurrentRotationAngle - TargetRotationAngle) > 0.1f)
            {
                State = TMState.Rotating;
            }
        }

        /// <summary>
        /// 즉시 회전 (애니메이션 없음)
        /// </summary>
        public void RotateImmediate(float angle)
        {
            CurrentRotationAngle = angle;
            TargetRotationAngle = angle;
            UpdatePositionsAfterRotation();
        }

        /// <summary>
        /// 매 프레임 업데이트 (deltaTime: 초 단위)
        /// </summary>
        public bool Update(float deltaTime)
        {
            bool isAnimating = false;

            // 회전 애니메이션 처리
            if (State == TMState.Rotating)
            {
                float angleDiff = TargetRotationAngle - CurrentRotationAngle;

                // 최단 경로 계산: -180 ~ 180 범위로 정규화
                while (angleDiff > 180f) angleDiff -= 360f;
                while (angleDiff < -180f) angleDiff += 360f;

                if (Math.Abs(angleDiff) > 0.1f)
                {
                    float step = RotationSpeed * deltaTime;
                    if (Math.Abs(angleDiff) < step)
                    {
                        CurrentRotationAngle = TargetRotationAngle;
                        State = TMState.Idle;
                        OnRotationComplete?.Invoke(); // 회전 완료 이벤트
                    }
                    else
                    {
                        CurrentRotationAngle += Math.Sign(angleDiff) * step;
                    }

                    UpdatePositionsAfterRotation();
                    isAnimating = true;
                }
                else
                {
                    CurrentRotationAngle = TargetRotationAngle;
                    State = TMState.Idle;
                    OnRotationComplete?.Invoke(); // 회전 완료 이벤트
                }
            }

            // 암 확장/수축 애니메이션 처리
            if (State == TMState.Moving)
            {
                float extensionDiff = TargetExtension - CurrentExtension;

                if (Math.Abs(extensionDiff) > 0.5f)
                {
                    float step = ExtensionSpeed * deltaTime;
                    if (Math.Abs(extensionDiff) < step)
                    {
                        CurrentExtension = TargetExtension;
                        State = TMState.Idle;
                        IsArmExtended = (CurrentExtension > 0);
                        OnArmMovementComplete?.Invoke(); // 암 이동 완료 이벤트
                    }
                    else
                    {
                        CurrentExtension += Math.Sign(extensionDiff) * step;
                    }

                    isAnimating = true;
                }
                else
                {
                    CurrentExtension = TargetExtension;
                    State = TMState.Idle;
                    IsArmExtended = (CurrentExtension > 0);
                    OnArmMovementComplete?.Invoke(); // 암 이동 완료 이벤트
                }
            }

            return isAnimating;
        }

        /// <summary>
        /// 회전 후 각 구성 요소의 위치 업데이트
        /// </summary>
        private void UpdatePositionsAfterRotation()
        {
            // Bottom을 중심으로 다른 구성 요소들을 회전
            float angleRad = CurrentRotationAngle * (float)Math.PI / 180f;

            // ArmHigh 위치 계산
            ArmHighPosition = RotatePointAroundCenter(
                initialArmHighPosition,
                initialBottomPosition,
                angleRad
            );

            // ArmLow 위치 계산
            ArmLowPosition = RotatePointAroundCenter(
                initialArmLowPosition,
                initialBottomPosition,
                angleRad
            );

            // Back 위치 계산
            BackPosition = RotatePointAroundCenter(
                initialBackPosition,
                initialBottomPosition,
                angleRad
            );

            // 웨이퍼 위치 계산 (웨이퍼를 들고 있을 때)
            if (HasWafer)
            {
                // 웨이퍼는 ArmHigh 앞쪽에 위치
                Point waferOffset = new Point(50, 0); // ArmHigh 기준 상대 위치
                Point waferRelative = RotatePointAroundCenter(
                    new Point(initialArmHighPosition.X + waferOffset.X, initialArmHighPosition.Y + waferOffset.Y),
                    initialBottomPosition,
                    angleRad
                );
                WaferPosition = waferRelative;
            }
        }

        /// <summary>
        /// 점을 중심점 기준으로 회전
        /// </summary>
        private Point RotatePointAroundCenter(Point point, Point center, float angleRad)
        {
            float cos = (float)Math.Cos(angleRad);
            float sin = (float)Math.Sin(angleRad);

            float dx = point.X - center.X;
            float dy = point.Y - center.Y;

            float rotatedX = dx * cos - dy * sin;
            float rotatedY = dx * sin + dy * cos;

            return new Point(
                (int)(center.X + rotatedX),
                (int)(center.Y + rotatedY)
            );
        }

        /// <summary>
        /// Arm 확장
        /// </summary>
        public void ExtendArm()
        {
            TargetExtension = MaxExtension;
            State = TMState.Moving;
        }

        /// <summary>
        /// Arm 수축
        /// </summary>
        public void RetractArm()
        {
            TargetExtension = 0f;
            State = TMState.Moving;
        }

        /// <summary>
        /// Arm을 특정 거리만큼 확장
        /// </summary>
        public void ExtendArmTo(float distance)
        {
            TargetExtension = Math.Max(0, Math.Min(distance, MaxExtension));
            State = TMState.Moving;
        }

        /// <summary>
        /// 현재 회전 각도를 고려한 암의 확장 오프셋 계산
        /// </summary>
        public Point GetExtensionOffset()
        {
            if (CurrentExtension <= 0)
                return Point.Empty;

            // 회전 각도를 라디안으로 변환
            float angleRad = CurrentRotationAngle * (float)Math.PI / 180f;

            // 회전된 방향으로 확장 거리만큼 이동
            int offsetX = (int)(CurrentExtension * Math.Cos(angleRad));
            int offsetY = (int)(CurrentExtension * Math.Sin(angleRad));

            return new Point(offsetX, offsetY);
        }

        /// <summary>
        /// 웨이퍼 픽업
        /// </summary>
        public bool PickWafer(Wafer wafer)
        {
            if (!HasWafer && IsArmExtended && wafer != null)
            {
                HasWafer = true;
                CurrentWafer = wafer;
                State = TMState.PickingWafer;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 웨이퍼 배치 및 반환
        /// </summary>
        public Wafer PlaceWafer()
        {
            if (HasWafer && IsArmExtended)
            {
                HasWafer = false;
                State = TMState.PlacingWafer;
                Wafer wafer = CurrentWafer;
                CurrentWafer = null;
                return wafer;
            }
            return null;
        }

        #region EtherCAT 컨트롤러 관련

        /// <summary>
        /// EtherCAT 컨트롤러 설정
        /// </summary>
        public void SetEtherCATController(IEtherCATController controller)
        {
            this.etherCATController = controller;
        }

        /// <summary>
        /// 실제 하드웨어 모드 여부
        /// </summary>
        public bool IsRealMode => etherCATController != null &&
                                  !(etherCATController is SimulationEtherCATController);

        /// <summary>
        /// EtherCAT 컨트롤러 반환
        /// </summary>
        public IEtherCATController GetEtherCATController()
        {
            return etherCATController;
        }

        #endregion
    }
}

