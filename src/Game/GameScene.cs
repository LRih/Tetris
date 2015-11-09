using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Tetris
{
    public class GameScene : Scene
    {
        //===================================================================== ENUM
        private enum State
        {
            Falling, Locking, Removing, Lost
        }

        //===================================================================== VARIABLES
        private int[,] _field = new int[Game.COL_COUNT, Game.ROW_COUNT + Game.INVISIBLE_ROW_COUNT];

        private Mode _mode;

        private Score _score = new Score();

        private ShapeManager _shapeManager = new ShapeManager();
        private Shape _curShape;
        private int _ghostShapeY;

        private Gravity _gravity;

        private int _lockDelay;
        private int _lockDelayLeft;

        private RemoveEffect _removeEffect = new RemoveEffect();

        private State _state;
        private int _frames = 0;
        private bool _isPaused = false;
        private bool _isHighLevel = false;

        //===================================================================== INITIALIZE
        public GameScene(Window container, Mode mode) : base(container)
        {
            _mode = mode;
            _gravity = new Gravity(Container.FPS, _score.Level);
            _lockDelay = Container.FPS / 2;

            GenerateShape();
        }

        //===================================================================== FUNCTIONS
        public override void Update()
        {
            if (!Container.Focused && !_isPaused) Pause();
            if (Input.IsTrigger(Keys.P)) TogglePause();

            if (_isPaused) return;

            if (Input.IsTrigger(Keys.M)) Audio.ToggleMute();

            if (_state == State.Falling || _state == State.Locking)
                UpdateMove();
            else if (_state == State.Removing)
                UpdateRemoving();
            else if (_state == State.Lost)
                UpdateLost();

            if (_state != State.Lost)
            {
                if (_mode == Mode.TimeAttack)
                {
                    if (FramesRemaining == 0 && _state != State.Removing)
                        Lose();
                }
                _frames++;
            }
        }
        private void UpdateMove()
        {
            // move
            if (Input.IsTrigger(Keys.Up) && TryRotate())
                CalculateGhostShapeY();
            if (Input.IsTrigger(Keys.Left) || Input.IsRepeat(Keys.Left))
                if (TryMove(-1, 0)) CalculateGhostShapeY();
            if (Input.IsTrigger(Keys.Right) || Input.IsRepeat(Keys.Right))
                if (TryMove(1, 0)) CalculateGhostShapeY();

            _state = (_curShape.Y == _ghostShapeY ? State.Locking : State.Falling);

            if (Input.IsTrigger(Keys.Space)) // hard drop
                LandCurrentShape();
            else if (Input.IsTrigger(Keys.C) && _shapeManager.TryHold(_curShape)) // hold
                GenerateShape();
            else if (_state == State.Locking) // locking
            {
                if (_lockDelayLeft == 0 || Input.IsTrigger(Keys.Space)) LandCurrentShape();
                else _lockDelayLeft--;
            }
            else if (_state == State.Falling) // soft drop
            {
                _lockDelayLeft = _lockDelay;

                if (_gravity.FallRows > 0 || Input.IsDown(Keys.Down))
                {
                    int rows = (_gravity.FallRows > 0 ? _gravity.FallRows : 1);
                    _curShape.Move(0, Math.Min(rows, _ghostShapeY - _curShape.Y));

                    if (_gravity.FallRows == 0) _score.AddSoftDrop();

                    _gravity.ResetCountdown();
                    _state = (_curShape.Y == _ghostShapeY ? State.Locking : State.Falling);
                }
                else _gravity.Update();
            }

        }
        private void UpdateRemoving()
        {
            if (_removeEffect.IsDisappearing)
                _removeEffect.Update();
            else
            {
                RemoveRows(_removeEffect.DisappearingRows);
                if (IsLose) Lose();
                else GenerateShape();
            }
        }
        private void UpdateLost()
        {
            if (Input.IsTrigger(Keys.Enter)) Container.Scene = new HighScoreScene(Container, _mode, _score.Level, _score.Value);
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(Bitmaps.Get(_mode == Mode.Marathon ? "background_marathon" : "background_time_attack"), 0, 0);

            if (_isPaused)
                DrawPauseText(g);
            else
            {
                if (_state != State.Removing && _state != State.Lost)
                {
                    if (_curShape.Y != _ghostShapeY) DrawGhostShape(g);
                    DrawCurrentShape(g);
                }
                DrawFieldBlocks(g);

                if (_shapeManager.Hold != null) DrawHoldShape(g);
                DrawNextShapes(g);
            }

            DrawScore(g);
            if (_mode == Mode.TimeAttack) DrawTime(g);

            //DrawDebug(g);
        }

        private void DrawPauseText(Graphics g)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            using (Brush b = new SolidBrush(Game.COL_TEXT))
                g.DrawString("PAUSED", Fonts.Get("large"), b, Game.RECT_MAIN, format);
        }

        private void DrawGhostShape(Graphics g)
        {
            for (int y = 0; y < Shape.SIZE; y++)
                for (int x = 0; x < Shape.SIZE; x++)
                    if (!_curShape.IsEmpty(x, y))
                        DrawFieldBlock(g, _curShape.X + x, _ghostShapeY + y, Bitmaps.Get("ghost"));
        }
        private void DrawCurrentShape(Graphics g)
        {
            for (int y = 0; y < Shape.SIZE; y++)
                for (int x = 0; x < Shape.SIZE; x++)
                    if (!_curShape.IsEmpty(x, y))
                        DrawFieldBlock(g, _curShape.X + x, _curShape.Y + y, _state == State.Lost ? 0 : _curShape.ID, CurrentShapeOpacity);
        }
        private void DrawFieldBlocks(Graphics g)
        {
            for (int y = 0; y <= _field.GetUpperBound(1); y++)
            {
                // opacity for when lines are cleared
                float opacity = 1;
                if (_state == State.Removing && _removeEffect.DisappearingRows.Contains(y))
                    opacity = _removeEffect.DisappearingOpacity;

                for (int x = 0; x <= _field.GetUpperBound(0); x++)
                    if (!IsEmpty(x, y) && _curShape.Y + y >= Game.INVISIBLE_ROW_COUNT)
                        DrawFieldBlock(g, x, y, _state == State.Lost ? 0 : GetID(x, y), opacity);
            }
        }

        private void DrawHoldShape(Graphics g)
        {
            g.DrawImage(Bitmaps.Get(Game.HOLD_BMP_NAMES[_shapeManager.Hold.ID]), Game.RECT_HOLD);
        }
        private void DrawNextShapes(Graphics g)
        {
            for (int i = 0; i < _shapeManager.Next.Count; i++)
            {
                Shape shape = _shapeManager.Next[i];
                g.DrawImage(Bitmaps.Get(Game.HOLD_BMP_NAMES[shape.ID]), Game.RECT_NEXT.Left, Game.RECT_NEXT.Top + i * Game.BLOCK_SIZE * (Shape.SIZE - 1));
            }
        }

        private void DrawScore(Graphics g)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            format.LineAlignment = StringAlignment.Far;

            using (Brush b = new SolidBrush(Game.COL_TEXT))
            {
                g.DrawString(_score.Value.ToString(), Fonts.Get("normal"), b, Game.RECT_SCORE, format);
                g.DrawString(_score.Lines.ToString(), Fonts.Get("normal"), b, Game.RECT_LINES, format);
                g.DrawString(_score.Level.ToString(), Fonts.Get("normal"), b, Game.RECT_LEVEL, format);
            }
        }
        private void DrawTime(Graphics g)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            using (Brush b = new SolidBrush(Game.COL_TEXT))
                g.DrawString(TimeRemaining, Fonts.Get("normal"), b, Game.RECT_TIME, format);
        }

        private void DrawFieldBlock(Graphics g, int col, int row, int id)
        {
            DrawFieldBlock(g, col, row, Bitmaps.Get(Game.BLOCK_BMP_NAMES[id]), 1);
        }
        private void DrawFieldBlock(Graphics g, int col, int row, int id, float opacity)
        {
            DrawFieldBlock(g, col, row, Bitmaps.Get(Game.BLOCK_BMP_NAMES[id]), opacity);
        }
        private void DrawFieldBlock(Graphics g, int col, int row, Bitmap bmp)
        {
            DrawFieldBlock(g, col, row, bmp, 1);
        }
        private void DrawFieldBlock(Graphics g, int col, int row, Bitmap bmp, float opacity)
        {
            int oX = Game.RECT_MAIN.Left;
            int oY = Game.RECT_MAIN.Top - Game.BLOCK_SIZE * Game.INVISIBLE_ROW_COUNT;

            if (row >= Game.INVISIBLE_ROW_COUNT)
            {
                if (opacity < 1)
                {
                    ColorMatrix matrix = new ColorMatrix();
                    matrix.Matrix33 = opacity;

                    ImageAttributes attr = new ImageAttributes();
                    attr.SetColorMatrix(matrix);

                    Rectangle dest = new Rectangle(oX + Game.BLOCK_SIZE * col, oY + Game.BLOCK_SIZE * row, bmp.Width, bmp.Height);
                    g.DrawImage(bmp, dest, 0, 0, Game.BLOCK_SIZE, Game.BLOCK_SIZE, GraphicsUnit.Pixel, attr);
                }
                else
                    g.DrawImage(bmp, oX + Game.BLOCK_SIZE * col, oY + Game.BLOCK_SIZE * row);
            }
        }

        [Obsolete]
        private void DrawDebug(Graphics g)
        {
            g.DrawString("Fall countdown: " + _gravity.Countdown + ", " + _gravity.FallRows, Container.Font, Brushes.Black, Game.MARGIN, 600);
            g.DrawString("Lock delay: " + _lockDelayLeft, Container.Font, Brushes.Black, Game.MARGIN, 620);
        }

        private bool TryMove(int rX, int rY)
        {
            _curShape.Move(rX, rY);
            if (IsCollision(_curShape))
            {
                _curShape.Move(-rX, -rY);
                return false;
            }
            else
                return true;
        }
        private bool TryRotate()
        {
            _curShape.RotateRight();

            // wall kick check
            foreach (Point pt in _curShape.KickData)
            {
                _curShape.Move(pt.X, pt.Y);
                if (!IsCollision(_curShape)) return true;
                else _curShape.Move(-pt.X, -pt.Y);
            }

            _curShape.RotateLeft();
            return false;
        }

        private void GenerateShape()
        {
            _curShape = _shapeManager.PopNext();
            CalculateGhostShapeY();

            _gravity.ResetCountdown();
            _lockDelayLeft = _lockDelay;

            _state = (_curShape.Y == _ghostShapeY ? State.Locking : State.Falling);
        }
        private void LandCurrentShape()
        {
            // move to bottom
            int move = _ghostShapeY - _curShape.Y;
            _curShape.Move(0, _ghostShapeY - _curShape.Y);
            _score.AddHardDrop(move);

            // add shape
            for (int y = 0; y < Shape.SIZE; y++)
                for (int x = 0; x < Shape.SIZE; x++)
                    if (!_curShape.IsEmpty(x, y)) _field[_curShape.X + x, _curShape.Y + y] = _curShape.ID;

            // remove completed rows
            _removeEffect.Set(GetCompletedRows(_curShape.Y, _curShape.Y + Shape.SIZE - 1));
            if (_removeEffect.DisappearingRows.Count > 0)
            {
                _score.AddLines(_removeEffect.DisappearingRows.Count);
                _gravity.Level = _score.Level;

                if (!_isHighLevel && _score.Level >= 11)
                    _isHighLevel = true;

                _state = State.Removing;
            }
            else
            {
                _gravity.Level = _score.Level;

                if (IsLose) Lose();
                else GenerateShape();
            }
        }

        private void CalculateGhostShapeY()
        {
            int oY = _curShape.Y;

            // find collision height
            while (TryMove(0, 1))
                ;

            // set ghost shape y and reverse moves
            _ghostShapeY = _curShape.Y;
            _curShape.Move(0, -(_ghostShapeY - oY));
        }

        private void TogglePause()
        {
            if (_isPaused) Resume();
            else Pause();
        }
        private void Pause()
        {
            _isPaused = true;
            Audio.PauseMusic();
        }
        private void Resume()
        {
            _isPaused = false;
            Audio.ResumeMusic();
        }

        private void Lose()
        {
            Audio.StopMusic();
            _state = State.Lost;
        }

        private List<int> GetCompletedRows(int rowMin, int rowMax)
        {
            List<int> rows = new List<int>();
            rowMax = Math.Min(rowMax, Game.ROW_COUNT + Game.INVISIBLE_ROW_COUNT - 1);

            for (int y = rowMin; y <= rowMax; y++)
                if (IsCompletedLine(y)) rows.Add(y);

            return rows;
        }
        private void RemoveRows(List<int> rows)
        {
            foreach (int row in rows)
                RemoveRow(row);
        }
        private void RemoveRow(int row)
        {
            for (int y = row; y > 0; y--)
                for (int x = 0; x <= _field.GetUpperBound(0); x++)
                    _field[x, y] = _field[x, y - 1];
        }

        private int GetID(int x, int y)
        {
            return _field[x, y];
        }

        private bool IsCollision(Shape shape)
        {
            for (int y = 0; y < Shape.SIZE; y++)
            {
                for (int x = 0; x < Shape.SIZE; x++)
                {
                    if (!shape.IsEmpty(x, y))
                    {
                        if (IsOutOfBounds(shape.X + x, shape.Y + y) || !IsEmpty(shape.X + x, shape.Y + y))
                            return true;
                    }
                }
            }
            return false;
        }
        private bool IsCompletedLine(int row)
        {
            for (int x = 0; x <= _field.GetUpperBound(0); x++)
                if (IsEmpty(x, row)) return false;
            return true;
        }

        private bool IsOutOfBounds(int x, int y)
        {
            return (x < 0 || x >= Game.COL_COUNT || y < 0 || y >= Game.ROW_COUNT + Game.INVISIBLE_ROW_COUNT);
        }
        private bool IsEmpty(int x, int y)
        {
            return _field[x, y] == 0;
        }

        //===================================================================== PROPERTIES
        private bool IsLose
        {
            get
            {
                for (int y = 0; y < Game.INVISIBLE_ROW_COUNT; y++)
                    for (int x = 0; x <= _field.GetUpperBound(0); x++)
                        if (!IsEmpty(x, y)) return true;
                return false;
            }
        }

        private int FramesRemaining
        {
            // 3 minutes
            get { return Math.Max(180 * Container.FPS - _frames, 0); }
        }
        private string TimeRemaining
        {
            get
            {
                int remaining = FramesRemaining;
                int ms = (int)((remaining % Container.FPS) / (float)Container.FPS * 100);
                int secs = (remaining / Container.FPS) % 60;
                int mins = (remaining / Container.FPS) / 60;
                return string.Format("{0}:{1}:{2}", mins, secs.ToString("D2"), ms.ToString("D2"));
            }
        }

        private float CurrentShapeOpacity
        {
            // from 1 to 0.25
            get { return 1 - (_lockDelay - _lockDelayLeft) / 20f; }
        }
    }
}
