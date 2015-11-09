using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Tetris
{
    public abstract class Window : Form
    {
        //===================================================================== VARIABLES
        private readonly IContainer _components = new Container();
        private Timer _timer;

        public Scene Scene { get; set; }

        private int _fps;

        //===================================================================== INITIALIZE
        public Window()
        {
            _timer = new Timer(_components);
            _timer.Tick += timer_Tick;

            Cursor = Cursors.Hand;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;

            FPS = 30;

            Scene = new Scene(this);

            InitializeResources();
            Initialize();

            _timer.Start();
        }
        protected virtual void Initialize()
        {
        }
        protected virtual void InitializeResources()
        {
        }

        //===================================================================== TERMINATE
        protected override void Dispose(bool disposing)
        {
            if (disposing && _components != null) _components.Dispose();
            Bitmaps.Dispose();
            Fonts.Dispose();

            base.Dispose(disposing);
        }

        //===================================================================== PROPERTIES
        public int FPS
        {
            get { return _fps; }
            set
            {
                _fps = Math.Min(Math.Max(value, 1), 60);
                _timer.Interval = 1000 / FPS;
                Input.RepeatDelay = FPS / 6;
            }
        }

        protected string EmbeddedIconName
        {
            set
            {
                foreach (string res in Assembly.GetCallingAssembly().GetManifestResourceNames())
                    if (res.EndsWith(value))
                        Icon = new Icon(Assembly.GetCallingAssembly().GetManifestResourceStream(res));
            }
        }
        protected string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public Rectangle Rectangle
        {
            get { return ClientRectangle; }
        }
        public new Size Size
        {
            get { return ClientSize; }
            set { ClientSize = value; }
        }

        //===================================================================== EVENTS
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Scene.Draw(e.Graphics);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Scene.Update();
            Input.Update();
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
                Input.TriggerMouse(Keys.LButton, e.X, e.Y);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
                Input.Untrigger(Keys.LButton);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Input.TriggerKey(e.KeyCode);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Input.Untrigger(e.KeyCode);
        }
    }
}
