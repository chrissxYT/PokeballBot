using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PokeballBot
{
    public partial class Form1 : Form
    {
        bool running = false;

        public Form1() => InitializeComponent();

        void button1_Click(object sender, EventArgs e)
        {
            running = !running;
            button1.Text = running ? "Stop" : "Start";
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (running)
            {
                Bitmap screenshot = ScreenshotPokeball;
                Point ball = GetLoc(Color.FromArgb(201, 60, 76), screenshot);
                Point pika = GetLoc(Color.FromArgb(248, 208, 80), screenshot);
                pictureBox1.Image = screenshot;
                if (ball.X != -1 && ball.Y != -1 && pika.X != -1 && pika.Y != -1)
                    if (pika.X < ball.X)
                        KeyboardPress(68, 7);
                    else if (pika.Y > ball.Y)
                        KeyboardPress(87, 26);
                    else if (pika.X > ball.X)
                        KeyboardPress(65, 4);
                    else if (pika.Y < ball.Y)
                        KeyboardPress(83, 22);
            }
            Invalidate(false);
            Update();
        }

        Point GetLoc(Color c, Bitmap croppedGameScreen)
        {
            for (int x = 0; x < croppedGameScreen.Width; x++)
                for (int y = 0; y < croppedGameScreen.Height; y++)
                    if (AreAboutEqual(c, croppedGameScreen.GetPixel(x, y)))
                        return new Point(x / 16, y / 16);
            return new Point(-1, -1);
        }

        bool AreAboutEqual(Color f, Color s) => f.R - 10 < s.R && f.R + 10 > s.R && f.G - 10 < s.G && f.G + 10 > s.G && f.B - 10 < s.B && f.B + 10 > s.B;

        static Bitmap ScreenshotPokeball
        {
            get
            {
                Rect rect = new Rect();
                GetWindowRect(Process.GetProcessesByName("Pokeball").ToArray()[0].MainWindowHandle, ref rect);
                Bitmap bmp = new Bitmap(160, 160, PixelFormat.Format24bppRgb);
                Graphics.FromImage(bmp).CopyFromScreen(rect.left + 19, rect.top + 42, 0, 0, new Size(160, 160), CopyPixelOperation.SourceCopy);
                return bmp;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        static void KeyboardPress(byte vk, byte sc)
        {
            keybd_event(vk, sc, 0, 0);
            keybd_event(vk, sc, 2, 0);
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
    }
}
