using System.Drawing;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    public class CustomPictureBox : PictureBox
    {
        public CustomPictureBox()
        {
            // 생성될 때 배경색을 투명으로 설정
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            // 여기가 핵심 로직: 내 뒤에 있는 컨트롤들을 그려주는 부분
            Graphics g = pevent.Graphics;
            if (this.Parent != null)
            {
                var index = Parent.Controls.GetChildIndex(this);
                for (var i = Parent.Controls.Count - 1; i > index; i--)
                {
                    var c = Parent.Controls[i];
                    if (c.Bounds.IntersectsWith(Bounds) && c.Visible)
                    {
                        using (var bmp = new Bitmap(c.Width, c.Height, g))
                        {
                            c.DrawToBitmap(bmp, c.ClientRectangle);
                            g.TranslateTransform(c.Left - Left, c.Top - Top);
                            g.DrawImageUnscaled(bmp, Point.Empty);
                            g.TranslateTransform(Left - c.Left, Top - c.Top);
                        }
                    }
                }
            }
        }
    }
}