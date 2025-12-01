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

        // 회전 경계 (이 각도 이하로 회전 불가 - 실제 장비 제약)
        private const float MIN_ROTATION_ANGLE = -55f;
        private const float MAX_ROTATION_ANGLE = 305f;  // 360 - 55 = 305

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
            MaxExtension = 90f; // 최대 90픽셀 확장 (기존 150의 60%)
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

                // 경계를 넘지 않는 방향 계산
                // 시계 방향(+) 또는 반시계 방향(-) 중 경계를 넘지 않는 방향 선택
                float clockwiseDiff = angleDiff;       // 시계 방향 (양수로 정규화)
                float counterClockwiseDiff = angleDiff; // 반시계 방향 (음수로 정규화)

                // 정규화: 시계방향은 양수, 반시계방향은 음수
                while (clockwiseDiff < 0) clockwiseDiff += 360f;
                while (clockwiseDiff >= 360f) clockwiseDiff -= 360f;
                while (counterClockwiseDiff > 0) counterClockwiseDiff -= 360f;
                while (counterClockwiseDiff <= -360f) counterClockwiseDiff += 360f;

                // 경계 체크: 반시계 방향(-) 이동 시 MIN_ROTATION_ANGLE을 넘는지 확인
                bool canGoCounterClockwise = CanRotateCounterClockwise(CurrentRotationAngle, TargetRotationAngle);

                // 방향 결정
                float finalDiff;
                if (!canGoCounterClockwise)
                {
                    // 반시계 방향 불가 → 시계 방향 강제
                    finalDiff = clockwiseDiff;
                }
                else if (clockwiseDiff == 0)
                {
                    // 이미 목표 도달
                    finalDiff = 0;
                }
                else
                {
                    // 둘 다 가능하면 짧은 경로 선택
                    finalDiff = (Math.Abs(clockwiseDiff) <= Math.Abs(counterClockwiseDiff))
                                ? clockwiseDiff : counterClockwiseDiff;
                }

                if (Math.Abs(finalDiff) > 0.1f)
                {
                    float step = RotationSpeed * deltaTime;
                    if (Math.Abs(finalDiff) < step)
                    {
                        CurrentRotationAngle = TargetRotationAngle;
                        State = TMState.Idle;
                        OnRotationComplete?.Invoke(); // 회전 완료 이벤트
                    }
                    else
                    {
                        CurrentRotationAngle += Math.Sign(finalDiff) * step;
                    }

                    // 각도 정규화 (경계 범위 내 유지)
                    NormalizeAngle();

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
        /// 각도를 유효 범위 내로 정규화 (-55° ~ 305°)
        /// </summary>
        private void NormalizeAngle()
        {
            while (CurrentRotationAngle > MAX_ROTATION_ANGLE)
                CurrentRotationAngle -= 360f;
            while (CurrentRotationAngle < MIN_ROTATION_ANGLE)
                CurrentRotationAngle += 360f;
        }

        /// <summary>
        /// 반시계 방향 회전이 가능한지 확인 (경계를 넘지 않는지)
        /// 반시계 방향으로 회전 시 MIN_ROTATION_ANGLE(-55°)을 넘으면 불가
        /// </summary>
        private bool CanRotateCounterClockwise(float fromAngle, float toAngle)
        {
            // 반시계 방향: fromAngle에서 toAngle로 갈 때 각도가 감소하는 방향
            // 경계(-55°)를 넘어가는지 확인

            // fromAngle이 toAngle보다 크면 반시계 방향 (예: 230° → -50°)
            // 이 경우 경로가 0°를 지나 음수로 가면서 -55°를 넘는지 확인

            // 두 각도 모두 정규화
            float from = fromAngle;
            float to = toAngle;

            // 반시계 방향으로 갈 때 경계(-55°)를 넘는 경우:
            // 1. from > MIN_ROTATION_ANGLE이고 to < MIN_ROTATION_ANGLE일 때
            //    (예: 0° → -60°는 불가, 0° → -50°는 가능)
            // 2. from과 to 사이의 반시계 방향 경로가 경계를 포함하는 경우
            //    (예: 230° → -50°는 반시계로 가면 -55°를 넘음)

            // 간단한 체크: 반시계 방향(-방향)으로 갈 때 경계 영역을 지나는지
            // 경계 영역: -55° ~ -55° + 360° = 305° 부근의 "금지 구역"

            // 반시계 방향 이동 거리 계산
            float counterClockwisePath = from - to;
            while (counterClockwisePath < 0) counterClockwisePath += 360f;
            while (counterClockwisePath >= 360f) counterClockwisePath -= 360f;

            // 반시계로 이동할 때 지나는 최소 각도 계산
            // from에서 counterClockwisePath만큼 감소했을 때의 최소 각도
            float minAngleOnPath = from - counterClockwisePath;

            // 경계를 넘는지 체크: 경로 중 MIN_ROTATION_ANGLE 이하로 내려가면 불가
            // 또는 from > 0이고 to < MIN_ROTATION_ANGLE이면서 0을 지나가는 경우

            // 더 정확한 체크: 반시계 방향으로 갈 때 경계 영역(305° ~ 360° → 0° ~ -55°)을 지나는지
            if (from >= MIN_ROTATION_ANGLE && to >= MIN_ROTATION_ANGLE)
            {
                // 둘 다 경계 위에 있음
                if (from > to)
                {
                    // 반시계 방향으로 직접 도달 가능 (경계 안 넘음)
                    return true;
                }
                else
                {
                    // 시계 방향이 더 직접적이거나, 반시계로 가면 전체 원을 돌아야 함
                    // 이 경우 경계를 넘어야 하므로 불가
                    return false;
                }
            }
            else if (to < MIN_ROTATION_ANGLE)
            {
                // 목표가 경계 밖 (사실 이런 경우는 없어야 함)
                return false;
            }
            else
            {
                // from < MIN_ROTATION_ANGLE인 경우 (사실 이런 경우도 없어야 함)
                return true;
            }
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

