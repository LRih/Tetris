using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tetris
{
    public class HighScoreScene : Scene
    {
        //===================================================================== CONSTANTS
        private static readonly string SAVE_PATH = Application.StartupPath + @"\highscores";

        private const char DELIM = '|';

        private const int HIGH_SCORE_WIDTH = 300;
        private const int HIGH_SCORE_HEIGHT = 40;
        private const int SHADOW_OFFSET = 1;

        private const int MAX_NAME_LENGTH = 10;

        //===================================================================== VARIABLES
        private Dictionary<Mode, List<HighScore>> _allScores;
        private string _lastName;

        private Mode _mode;

        private HighScore _newScore;
        private int _newScoreIndex = -1;

        private Rectangle _rectHighScore;
        //===================================================================== INITIALIZE
        public HighScoreScene(Window container, Mode mode, int newLevel, int newScore) : base(container)
        {
            _mode = mode;

            LoadScores();

            _newScore = new HighScore(_lastName, newLevel, newScore);
            AddScoreIfGreater(_newScore);

            _rectHighScore = new Rectangle((Container.Rectangle.Width - HIGH_SCORE_WIDTH) / 2, Game.MARGIN * 5, HIGH_SCORE_WIDTH, HIGH_SCORE_HEIGHT);
        }

        //===================================================================== FUNCTIONS
        public override void Update()
        {
            if (IsEditing)
            {
                UpdateNameInput();
                if (Input.IsTrigger(Keys.Enter) && _newScore.Name.Length > 0)
                {
                    _newScore.Name = _lastName = _newScore.Name.Replace("_", "");
                    SaveScores();

                    _newScore = null;
                    NewScoreIndex = -1;
                }
            }
            else
            {
                if (_mode == Mode.Marathon && Input.IsTrigger(Keys.Right))
                    _mode = Mode.TimeAttack;
                else if (_mode == Mode.TimeAttack && Input.IsTrigger(Keys.Left))
                    _mode = Mode.Marathon;
                if (Input.IsTrigger(Keys.Enter))
                    Container.Scene = new GameScene(Container, _mode);
            }
        }
        private void UpdateNameInput()
        {
            // remove underscore
            string rawName = _newScore.Name.Replace("_", "");

            // character input
            foreach (Keys key in Input.Triggers)
            {
                if (key >= Keys.A && key <= Keys.Z && rawName.Length < MAX_NAME_LENGTH)
                {
                    string add = ((char)key).ToString();
                    if (!Input.IsDown(Keys.ShiftKey)) add = add.ToLower();
                    rawName += add;
                }
            }

            // backspace
            if ((Input.IsTrigger(Keys.Back) || Input.IsRepeat(Keys.Back)) && rawName.Length > 0)
                rawName = rawName.Substring(0, rawName.Length - 1);

            // re-add underscore
            if (rawName.Length < MAX_NAME_LENGTH) rawName = rawName + "_";

            _newScore.Name = rawName;
        }

        public override void Draw(Graphics g)
        {
            DrawBackground(g);
            DrawTitle(g);
            DrawScores(g);
            DrawArrows(g);
        }
        private void DrawBackground(Graphics g)
        {
            g.DrawImage(Bitmaps.Get("background_marathon"), 0, 0);

            using (Brush b = new SolidBrush(Color.FromArgb(200, Color.Black)))
                g.FillRectangle(b, Container.Rectangle);
        }
        private void DrawTitle(Graphics g)
        {
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;

            using (Brush b = new SolidBrush(Game.COL_TEXT))
            {
                Rectangle rect = Container.Rectangle;
                rect.Y += Game.MARGIN;
                DrawText(g, "High Scores", Fonts.Get("large"), Brushes.LightYellow, rect, format);
                rect.Y += Game.MARGIN + 10;
                DrawText(g, Title, Fonts.Get("normal"), Brushes.LightYellow, rect, format);
            }
        }
        private void DrawScores(Graphics g)
        {
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;

            Rectangle rect = _rectHighScore;

            // title
            DrawScore(g, "Name", "Lvl", "Score", Brushes.PaleGoldenrod, rect);
            rect.Y += HIGH_SCORE_HEIGHT;

            for (int i = 0; i < Scores.Count; i++)
            {
                Brush b = Brushes.White;

                if (IsEditing && i == NewScoreIndex)
                {
                    b = Brushes.Gold;
                    g.FillRectangle(Brushes.Gray, rect);
                }

                DrawScore(g, Scores[i].Name, Scores[i].Level.ToString(), Scores[i].Score.ToString(), b, rect);
                rect.Y += HIGH_SCORE_HEIGHT;
            }
        }
        private void DrawScore(Graphics g, string name, string level, string score, Brush b, Rectangle rect)
        {
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;

            DrawText(g, name, Fonts.Get("normal"), b, rect, format);

            format.Alignment = StringAlignment.Center;
            DrawText(g, level, Fonts.Get("normal"), b, rect, format);

            format.Alignment = StringAlignment.Far;
            DrawText(g, score, Fonts.Get("normal"), b, rect, format);
        }
        private void DrawArrows(Graphics g)
        {
            Rectangle rect = Container.Rectangle;
            rect.X += Game.MARGIN;
            rect.Width -= Game.MARGIN * 2;
            rect.Height -= Game.MARGIN;

            if (!IsEditing)
            {
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Far;

                if (_mode == Mode.Marathon)
                {
                    format.Alignment = StringAlignment.Far;
                    DrawText(g, "Time Attack >", Fonts.Get("normal"), Brushes.LightYellow, rect, format);
                }
                else
                    DrawText(g, "< Marathon", Fonts.Get("normal"), Brushes.LightYellow, rect, format);
            }
        }

        private static void DrawText(Graphics g, string text, Font font, Brush brush, Rectangle rect, StringFormat format)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            rect.X += SHADOW_OFFSET;
            rect.Y += SHADOW_OFFSET;
            using (Brush b = new SolidBrush(Color.FromArgb(100, Color.Black)))
                g.DrawString(text, font, b, rect, format);
            rect.X -= SHADOW_OFFSET;
            rect.Y -= SHADOW_OFFSET;
            g.DrawString(text, font, brush, rect, format);
        }

        private void AddScoreIfGreater(HighScore score)
        {
            if (_newScore.Score > Scores[Scores.Count - 1].Score)
            {
                Scores.RemoveAt(Scores.Count - 1);
                Scores.Add(_newScore);
                Scores.Sort();
            }
        }

        private void LoadScores()
        {
            _allScores = new Dictionary<Mode, List<HighScore>>();

            if (!File.Exists(SAVE_PATH))
                CreateSaveFile();

            string[] data = File.ReadAllLines(SAVE_PATH);

            _allScores.Add(Mode.Marathon, new List<HighScore>());
            for (int i = 0; i < 10; i++)
            {
                string[] split = data[i].Split(new char[] { DELIM });
                _allScores[Mode.Marathon].Add(new HighScore(split[0], int.Parse(split[1]), int.Parse(split[2])));
            }

            _allScores.Add(Mode.TimeAttack, new List<HighScore>());
            for (int i = 10; i < 20; i++)
            {
                string[] split = data[i].Split(new char[] { DELIM });
                _allScores[Mode.TimeAttack].Add(new HighScore(split[0], int.Parse(split[1]), int.Parse(split[2])));
            }

            _lastName = data[20];
        }
        private void SaveScores()
        {
            StringBuilder save = new StringBuilder();
            foreach (Mode mode in _allScores.Keys)
            {
                foreach (HighScore score in _allScores[mode])
                    save.AppendLine(score.Name + DELIM + score.Level + DELIM + score.Score);
            }
            save.AppendLine(_lastName);
            File.WriteAllText(SAVE_PATH, save.ToString());
        }

        private static void CreateSaveFile()
        {
            StringBuilder save = new StringBuilder();

            int[]  scores = new int[] { 100000, 80000, 40000, 20000, 10000, 8000, 4000, 2000, 1000, 500, 50000, 40000, 30000, 15000, 10000, 8000, 4000, 2000, 1000, 500 };
            int[]  levels = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 8, 7, 7, 6, 6, 5, 4, 3, 2, 1 };

            for (int i = 0; i < scores.Length; i++)
                save.AppendLine("Tetris" + DELIM + levels[i] + DELIM + scores[i]);
            save.AppendLine("");

            File.WriteAllText(SAVE_PATH, save.ToString());
        }

        //===================================================================== PROPERTIES
        private List<HighScore> Scores
        {
            get { return _allScores[_mode]; }
        }
        private string Title
        {
            get { return _mode == Mode.Marathon ? "Marathon" : "Time Attack"; }
        }

        private bool IsEditing
        {
            get { return NewScoreIndex != -1; }
        }
        private int NewScoreIndex
        {
            get
            {
                if (_newScoreIndex != -1) return _newScoreIndex;

                for (int i = 0; i < Scores.Count; i++)
                    if (Scores[i] == _newScore) return i;
                return -1;
            }
            set { _newScoreIndex = -1; }
        }
    }
}
