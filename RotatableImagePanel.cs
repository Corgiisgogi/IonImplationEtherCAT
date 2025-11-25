using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    /// <summary>
    /// 회전 가능한 이미지를 표시하는 커스텀 Panel
    /// 투명 배경과 회전을 지원
    /// </summary>
    public class RotatableImagePanel : Panel
    {
        private Image _image;
        private float _rotation = 0f;

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                Invalidate(); // 다시 그리기
            }
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                Invalidate(); // 다시 그리기
            }
        }

        public RotatableImagePanel()
        {
            // 더블 버퍼링 활성화 (깜빡임 방지)
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.SupportsTransparentBackColor, true);
            
            this.BackColor = Color.Transparent;
            this.UpdateStyles();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_image == null)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 패널의 중심점
            float centerX = this.Width / 2f;
            float centerY = this.Height / 2f;

            // 변환 행렬 설정
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(_rotation);
            g.TranslateTransform(-centerX, -centerY);

            // 이미지를 패널 크기에 맞춰 그리기
            g.DrawImage(_image, 0, 0, this.Width, this.Height);
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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // 투명 배경을 위해 배경을 그리지 않음
        }
    }
}

