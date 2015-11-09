using System;
using System.Drawing;

namespace Tetris
{
    public static class Game
    {
        //===================================================================== CONSTANTS
        public const int MARGIN = 25;
        public const int BORDER_THICKNESS = 5;

        public const int BLOCK_SIZE = 30;
        public const int BLOCK_INNER_PADDING = 6;
        public const int BLOCK_BORDER_THICKNESS = 2;

        public const int SCORE_HEIGHT = 50;
        public static readonly Color COL_TEXT = Color.FromArgb(100, 100, 100);

        public const int ROW_COUNT = 20;
        public const int COL_COUNT = 10;
        public const int INVISIBLE_ROW_COUNT = 2;

        public static readonly Color[] BLOCK_COLORS = { Color.Gray, Color.LightCyan, Color.SkyBlue, Color.SandyBrown, Color.Khaki, Color.YellowGreen, Color.Plum, Color.IndianRed };
        public static readonly string[] BLOCK_BMP_NAMES = { "gray", "cyan", "blue", "orange", "yellow", "green", "purple", "red" };
        public static readonly string[] HOLD_BMP_NAMES = { "gray_hold", "cyan_hold", "blue_hold", "orange_hold", "yellow_hold", "green_hold", "purple_hold", "red_hold" };

        public static readonly Rectangle RECT_HOLD, RECT_MAIN, RECT_NEXT;
        public static readonly Rectangle RECT_SCORE, RECT_LINES, RECT_LEVEL, RECT_TIME;
        public static readonly Rectangle RECT_HIGH_SCORE;

        //===================================================================== INITIALIZE
        static Game()
        {
            RECT_HOLD = new Rectangle(MARGIN, MARGIN, MARGIN + BLOCK_SIZE * Shape.SIZE, MARGIN + BLOCK_SIZE * Shape.SIZE);
            RECT_MAIN = new Rectangle(RECT_HOLD.Right + MARGIN, MARGIN, BLOCK_SIZE * COL_COUNT, BLOCK_SIZE * ROW_COUNT);
            RECT_NEXT = new Rectangle(RECT_MAIN.Right + MARGIN, MARGIN, MARGIN + BLOCK_SIZE * Shape.SIZE, MARGIN * 2 + BLOCK_SIZE * (Shape.SIZE - 1) * ShapeManager.NEXT_COUNT);

            RECT_SCORE = new Rectangle(MARGIN - BORDER_THICKNESS, RECT_HOLD.Bottom + MARGIN * 4, RECT_HOLD.Width + BORDER_THICKNESS * 2, SCORE_HEIGHT);
            RECT_LINES = new Rectangle(RECT_SCORE.Left, RECT_SCORE.Bottom + MARGIN / 2, RECT_SCORE.Width, RECT_SCORE.Height);
            RECT_LEVEL = new Rectangle(RECT_LINES.Left, RECT_LINES.Bottom + MARGIN / 2, RECT_LINES.Width, RECT_LINES.Height);

            RECT_TIME = new Rectangle(RECT_LEVEL.Left, RECT_MAIN.Bottom - RECT_LEVEL.Height, RECT_LEVEL.Width, RECT_LEVEL.Height);
        }
    }
}
