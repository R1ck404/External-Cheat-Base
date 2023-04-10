using System.Windows.Forms;
using System;
using Client.ClientBase.UI.Notification;
using Client.ClientBase.Utils;
using Client.ClientBase.Fonts;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace Client.ClientBase.UI.Overlay
{
    public class Overlay : Form
    {
        private BufferedGraphics buffer;

        public Overlay()
        {
            FormLayout();

            Program.xSize = GetScreen().Width;
            Program.ySize = GetScreen().Height;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            buffer = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = buffer.Graphics;

            g.Clear(Color.FromArgb(77, 77, 77));
            RenderUtil.RenderDropshadowText(g, Program.client_name, new Font(FontRenderer.comfortaaFamily, 26), Color.FromArgb(122, 183, 255), Color.FromArgb(150, 122, 183, 255), 65, new PointF(10, 10));
            NotificationManager.UpdateNotifications(g);

            foreach (Module mod in ModuleManager.modules)
            {
                if (mod.enabled && mod is VisualModule module)
                {
                    VisualModule visualMod = module;
                    visualMod.OnDraw(g);
                }
            }

            buffer.Render(e.Graphics);
        }

        public void FormLayout()
        {
            this.Name = Program.client_name;
            this.Text = Program.client_name;
            this.Size = new System.Drawing.Size(500, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TransparencyKey = Color.FromArgb(77, 77, 77);
            this.BackColor = this.TransparencyKey;
            this.BackColor = Color.FromArgb(77, 77, 77);
            this.Opacity = .8;
            this.Location = new System.Drawing.Point(0, 0);
            this.AutoScaleMode = AutoScaleMode.None;
            this.TopMost = true;
            this.DoubleBuffered = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                buffer.Dispose();
            }
            base.Dispose(disposing);
        }

        public Rectangle GetScreen()
        {
            return System.Windows.Forms.Screen.FromControl(this).Bounds;
        }
    }
}