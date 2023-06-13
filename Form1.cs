using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS2812BLEDWIFI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var point = new Point();
            point.X = 1440;
            point.Y = 1;
            MessageBox.Show(GetColorFromScreen(point).ToString());
        }


        public Bitmap CaptureFromScreen(Rectangle rect)
        {
            Bitmap bmpScreenCapture = null;
            if (rect == Rectangle.Empty)//capture the whole screen
            {
                rect = Screen.PrimaryScreen.Bounds;
            }
            bmpScreenCapture = new Bitmap(rect.Width, rect.Height);
            Graphics p = Graphics.FromImage(bmpScreenCapture);

            p.CopyFromScreen(rect.X,
                     rect.Y,
                     0, 0,
                     rect.Size,
                     CopyPixelOperation.SourceCopy);

            p.Dispose();
            return bmpScreenCapture;
        }
        public Color GetColorFromScreen(Point p)
        {
            Rectangle rect = new Rectangle(p, new Size(2, 2));
            Bitmap map = CaptureFromScreen(rect);
            Color c = map.GetPixel(0, 0);
            map.Dispose();
            return c;
        }

    }
}
