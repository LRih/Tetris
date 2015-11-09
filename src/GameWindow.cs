using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Tetris
{
    public class GameWindow : Window
    {
        //===================================================================== INITIALIZE
        protected override void InitializeResources()
        {
            Fonts.Add("normal", "Roboto-Regular.ttf", 16);
            Fonts.Add("large", "Roboto-Regular.ttf", 24);
        }
        protected override void Initialize()
        {
            this.EmbeddedIconName = "icon.ico";
            this.ShowIcon = true;
            this.Size = new Size(Game.MARGIN * 4 + Game.RECT_HOLD.Width + Game.RECT_MAIN.Width + Game.RECT_NEXT.Width, Game.MARGIN * 2 + Game.RECT_MAIN.Height);
            this.Title = "Tetris";

            CacheMarathonBackgroundBitmap();
            CacheTimeAttackBackgroundBitmap();

            CacheGrayscaleBlockBitmap();
            CacheBlockBitmaps();
            CacheGhostBlockBitmap();

            CacheHoldBitmaps();

            Scene = new HighScoreScene(this, Mode.Marathon, 1, 0);
        }

        private void CacheMarathonBackgroundBitmap()
        {
            Bitmap bmp = new Bitmap(this.Size.Width, this.Size.Height);
            Graphics g = Graphics.FromImage(bmp);

            // background
            using (Brush b = new SolidBrush(Color.FromArgb(250, 250, 250)))
                g.FillRectangle(b, this.Rectangle);

            // panels
            DrawPanel(g, Game.RECT_HOLD);
            DrawPanel(g, Game.RECT_MAIN);
            DrawPanel(g, Game.RECT_NEXT);

            // text
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            using (Brush b = new SolidBrush(Game.COL_TEXT))
            {
                g.DrawString("HOLD", Fonts.Get("large"), b, Game.RECT_HOLD.Left - Game.BORDER_THICKNESS, Game.RECT_HOLD.Bottom + Game.BORDER_THICKNESS);

                Rectangle rect = Game.RECT_NEXT;
                rect.X -= Game.BORDER_THICKNESS;
                rect.Y += rect.Height + Game.BORDER_THICKNESS;
                rect.Width += Game.BORDER_THICKNESS * 2;
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Far;
                g.DrawString("NEXT", Fonts.Get("large"), b, rect, format);
            }

            // textboxes
            DrawTextBox(g, "SCORE:", Game.RECT_SCORE);
            DrawTextBox(g, "LINES:", Game.RECT_LINES);
            DrawTextBox(g, "LEVEL:", Game.RECT_LEVEL);

            g.Dispose();

            Bitmaps.Add("background_marathon", bmp);
        }
        private void CacheTimeAttackBackgroundBitmap()
        {
            Bitmap bmp = new Bitmap(Bitmaps.Get("background_marathon"));
            Graphics g = Graphics.FromImage(bmp);

            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            DrawTextBox(g, "", Game.RECT_TIME);

            g.Dispose();

            Bitmaps.Add("background_time_attack", bmp);
        }

        private static void CacheGrayscaleBlockBitmap()
        {
            Bitmap bmp = new Bitmap(Game.BLOCK_SIZE, Game.BLOCK_SIZE);
            Graphics g = Graphics.FromImage(bmp);

            // border
            using (Brush b = new SolidBrush(Color.FromArgb(220, 220, 220)))
                g.FillRectangle(b, 0, 0, Game.BLOCK_SIZE, Game.BLOCK_SIZE);

            // padding fill
            g.FillRectangle(Brushes.White, Game.BLOCK_BORDER_THICKNESS, Game.BLOCK_BORDER_THICKNESS,
                Game.BLOCK_SIZE - Game.BLOCK_BORDER_THICKNESS * 2, Game.BLOCK_SIZE - Game.BLOCK_BORDER_THICKNESS * 2);

            // shading
            GraphicsPath path = new GraphicsPath();
            path.AddLine(0, 0, Game.BLOCK_SIZE, Game.BLOCK_SIZE);
            path.AddLine(Game.BLOCK_SIZE, Game.BLOCK_SIZE, 0, Game.BLOCK_SIZE);
            path.CloseFigure();
            using (Brush b = new SolidBrush(Color.FromArgb(60, Color.Black)))
                g.FillPath(b, path);

            path = new GraphicsPath();
            path.AddLine(0, 0, Game.BLOCK_SIZE, Game.BLOCK_SIZE);
            path.AddLine(Game.BLOCK_SIZE, 0, 0, 0);
            path.CloseFigure();
            using (Brush b = new SolidBrush(Color.FromArgb(20, Color.Black)))
                g.FillPath(b, path);

            // inner fill
            g.FillRectangle(Brushes.White, Game.BLOCK_INNER_PADDING, Game.BLOCK_INNER_PADDING,
                Game.BLOCK_SIZE - Game.BLOCK_INNER_PADDING * 2, Game.BLOCK_SIZE - Game.BLOCK_INNER_PADDING * 2);

            g.Dispose();

            Bitmaps.Add("block", bmp);
        }
        private static void CacheBlockBitmaps()
        {
            for (int i = 0; i < Game.BLOCK_COLORS.Length; i++)
            {
                Bitmap bmp = new Bitmap(Game.BLOCK_SIZE, Game.BLOCK_SIZE);
                Graphics g = Graphics.FromImage(bmp);
                Color col = Game.BLOCK_COLORS[i];

                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix00 = col.R / 255f;
                matrix.Matrix11 = col.G / 255f;
                matrix.Matrix22 = col.B / 255f;

                ImageAttributes attr = new ImageAttributes();
                attr.SetColorMatrix(matrix);

                g.DrawImage(Bitmaps.Get("block"), new Rectangle(0, 0, Game.BLOCK_SIZE, Game.BLOCK_SIZE), 0, 0, Game.BLOCK_SIZE, Game.BLOCK_SIZE, GraphicsUnit.Pixel, attr);
                g.Dispose();

                Bitmaps.Add(Game.BLOCK_BMP_NAMES[i], bmp);
            }
        }
        private static void CacheGhostBlockBitmap()
        {
            Bitmap bmp = new Bitmap(Game.BLOCK_SIZE, Game.BLOCK_SIZE);
            Graphics g = Graphics.FromImage(bmp);

            using (Brush b = new SolidBrush(Color.FromArgb(230, 230, 230)))
                g.FillRectangle(b, 0, 0, Game.BLOCK_SIZE, Game.BLOCK_SIZE);

            g.Dispose();

            Bitmaps.Add("ghost", bmp);
        }

        private static void CacheHoldBitmaps()
        {
            for (int i = 1; i < Game.HOLD_BMP_NAMES.Length; i++)
            {
                Shape shape = ShapeManager.Generate(i);

                Bitmap bmp = new Bitmap(Game.RECT_HOLD.Width, Game.RECT_HOLD.Height);
                Graphics g = Graphics.FromImage(bmp);

                int oX = (bmp.Width - Game.BLOCK_SIZE * shape.Width) / 2;
                int oY = (bmp.Height - Game.BLOCK_SIZE * shape.Height) / 2;

                for (int y = 0; y < Shape.SIZE; y++)
                    for (int x = 0; x < Shape.SIZE; x++)
                        if (!shape.IsEmpty(x, y))
                            g.DrawImage(Bitmaps.Get(Game.BLOCK_BMP_NAMES[shape.ID]), oX + (x - shape.Left) * Game.BLOCK_SIZE, oY + (y - shape.Top) * Game.BLOCK_SIZE);

                g.Dispose();

                Bitmaps.Add(Game.HOLD_BMP_NAMES[i], bmp);
            }
        }

        //===================================================================== FUNCTIONS
        private static void DrawPanel(Graphics g, Rectangle rect)
        {
            using (Brush b = new SolidBrush(Color.FromArgb(30, Color.Black)))
                g.FillRectangle(b, rect.Left, rect.Top, rect.Width + (Game.BORDER_THICKNESS * 7) / 4, rect.Height + (Game.BORDER_THICKNESS * 7) / 4);

            using (Brush b = new SolidBrush(Color.FromArgb(150, 150, 150)))
                g.FillRectangle(b, rect.Left - Game.BORDER_THICKNESS, rect.Top - Game.BORDER_THICKNESS,
                    rect.Width + Game.BORDER_THICKNESS * 2, rect.Height + Game.BORDER_THICKNESS * 2);

            g.FillRectangle(Brushes.White, rect.Left, rect.Top, rect.Width, rect.Height);
        }
        private static void DrawTextBox(Graphics g, string text, Rectangle rect)
        {
            using (Brush b = new SolidBrush(Color.FromArgb(30, Color.Black)))
                g.FillRectangle(b, rect.Left + Game.BORDER_THICKNESS / 2, rect.Top + Game.BORDER_THICKNESS / 2, rect.Width, rect.Height);

            g.FillRectangle(Brushes.White, rect);

            using (Brush b = new SolidBrush(Game.COL_TEXT))
                g.DrawString(text, Fonts.Get("normal"), b, rect);
        }
    }
}
