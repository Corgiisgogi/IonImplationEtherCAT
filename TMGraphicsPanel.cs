using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// Transfer Module을 그래픽으로 표시하는 커스텀 Panel
    /// </summary>
    public class TMGraphicsPanel : Panel
    {
        private TransferModule transferModule;
        
        // TM 구성 요소의 크기
        private readonly Size armHighSize = new Size(232, 95);
        private readonly Size armLowSize = new Size(236, 95);
        private readonly Size bottomSize = new Size(150, 150);
        private readonly Size backSize = new Size(25, 95);

        // 색상
        private readonly Color armColor = Color.FromArgb(180, 180, 180);
        private readonly Color bottomColor = Color.FromArgb(150, 150, 150);
        private readonly Color backColor = Color.FromArgb(100, 100, 100);
        private readonly Color waferColor = Color.FromArgb(64, 150, 220);

        public TransferModule TransferModule
        {
            get => transferModule;
            set
            {
                transferModule = value;
                Invalidate();
            }
        }

        public TMGraphicsPanel()
        {
            // 더블 버퍼링 활성화
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.SupportsTransparentBackColor, true);
            
            this.BackColor = Color.Transparent;
            this.UpdateStyles();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // 부모의 배경을 그려서 투명 효과 구현
            if (this.Parent != null)
            {
                using (var bmp = new Bitmap(this.Parent.Width, this.Parent.Height))
                {
                    this.Parent.DrawToBitmap(bmp, this.Parent.ClientRectangle);
                    e.Graphics.DrawImage(bmp, -this.Left, -this.Top);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (transferModule == null)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // TM Bottom (중심축) - 회전하지 않음
            DrawBottom(g);

            // 회전하는 구성 요소들
            DrawRotatingComponents(g);
        }

        private void DrawBottom(Graphics g)
        {
            // TM Bottom을 중심에 그리기
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            
            Rectangle bottomRect = new Rectangle(
                centerX - bottomSize.Width / 2,
                centerY - bottomSize.Height / 2,
                bottomSize.Width,
                bottomSize.Height
            );

            using (SolidBrush brush = new SolidBrush(bottomColor))
            {
                g.FillEllipse(brush, bottomRect);
            }
            
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(pen, bottomRect);
            }
        }

        private void DrawRotatingComponents(Graphics g)
        {
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;

            // 확장 오프셋 계산 (회전 방향으로의 이동량)
            Point extensionOffset = transferModule.GetExtensionOffset();

            // 회전 변환 저장
            GraphicsState state = g.Save();

            // 확장 오프셋만큼 회전 중심 이동
            int newCenterX = centerX - extensionOffset.X;
            int newCenterY = centerY - extensionOffset.Y;

            // 새로운 중심점 기준으로 회전
            g.TranslateTransform(newCenterX, newCenterY);
            g.RotateTransform(transferModule.CurrentRotationAngle);
            g.TranslateTransform(-centerX, -centerY);

            // Back Arm (뒷부분)
            DrawBack(g, centerX, centerY);

            // Low Arm (아래 팔)
            DrawArmLow(g, centerX, centerY);

            // High Arm (위 팔)
            DrawArmHigh(g, centerX, centerY);

            // 웨이퍼 (TM이 들고 있을 때)
            if (transferModule.HasWafer)
            {
                DrawWafer(g, centerX, centerY);
            }

            // 변환 복원
            g.Restore(state);
        }

        private void DrawBack(Graphics g, int centerX, int centerY)
        {
            // 중심에서 오른쪽으로 약간 떨어진 위치
            int x = centerX + 90;
            int y = centerY - backSize.Height / 2;

            Rectangle rect = new Rectangle(x, y, backSize.Width, backSize.Height);

            using (SolidBrush brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, rect);
            }
            
            using (Pen pen = new Pen(Color.Black, 1))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawArmHigh(Graphics g, int centerX, int centerY)
        {
            // 중심에서 왼쪽으로
            int x = centerX - 180;
            int y = centerY - armHighSize.Height / 2;

            Rectangle rect = new Rectangle(x, y, armHighSize.Width, armHighSize.Height);

            // 그라디언트로 입체감
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect, 
                Color.FromArgb(200, 200, 200), 
                Color.FromArgb(160, 160, 160), 
                90f))
            {
                g.FillRectangle(brush, rect);
            }
            
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, rect);
            }

            // Arm 디테일 (라인)
            using (Pen detailPen = new Pen(Color.FromArgb(140, 140, 140), 1))
            {
                int lineY = y + armHighSize.Height / 3;
                g.DrawLine(detailPen, x + 10, lineY, x + armHighSize.Width - 10, lineY);
                lineY = y + (armHighSize.Height * 2) / 3;
                g.DrawLine(detailPen, x + 10, lineY, x + armHighSize.Width - 10, lineY);
            }
        }

        private void DrawArmLow(Graphics g, int centerX, int centerY)
        {
            // 중심에서 왼쪽으로 (High보다 약간 아래)
            int x = centerX - 153;
            int y = centerY - armLowSize.Height / 2 + 5;

            Rectangle rect = new Rectangle(x, y, armLowSize.Width, armLowSize.Height);

            // 그라디언트로 입체감
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect, 
                Color.FromArgb(180, 180, 180), 
                Color.FromArgb(140, 140, 140), 
                90f))
            {
                g.FillRectangle(brush, rect);
            }
            
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, rect);
            }

            // Arm 디테일 (라인)
            using (Pen detailPen = new Pen(Color.FromArgb(120, 120, 120), 1))
            {
                int lineY = y + armLowSize.Height / 3;
                g.DrawLine(detailPen, x + 10, lineY, x + armLowSize.Width - 10, lineY);
                lineY = y + (armLowSize.Height * 2) / 3;
                g.DrawLine(detailPen, x + 10, lineY, x + armLowSize.Width - 10, lineY);
            }
        }

        private void DrawWafer(Graphics g, int centerX, int centerY)
        {
            // Arm High 끝 부분에 위치
            int x = centerX - 230;
            int y = centerY - 40;
            int size = 80;

            Rectangle waferRect = new Rectangle(x, y, size, size);

            using (SolidBrush brush = new SolidBrush(waferColor))
            {
                g.FillEllipse(brush, waferRect);
            }
            
            using (Pen pen = new Pen(Color.FromArgb(40, 100, 150), 2))
            {
                g.DrawEllipse(pen, waferRect);
            }

            // 웨이퍼 중심 마크
            using (Pen markPen = new Pen(Color.FromArgb(30, 80, 130), 1))
            {
                int markSize = 20;
                int markX = x + size / 2;
                int markY = y + size / 2;
                g.DrawLine(markPen, markX - markSize, markY, markX + markSize, markY);
                g.DrawLine(markPen, markX, markY - markSize, markX, markY + markSize);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }
    }
}

