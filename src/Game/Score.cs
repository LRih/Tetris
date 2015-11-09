using System;

namespace Tetris
{
    public class Score
    {
        //===================================================================== VARIABLES
        private int _value = 0;
        private int _lines = 0;

        //===================================================================== FUNCTIONS
        public void AddSoftDrop()
        {
            Value += 1;
        }
        public void AddHardDrop(int dist)
        {
            Value += dist * 2;
        }

        public void AddLines(int lines)
        {
            _lines += lines;
            if (lines == 3)
                Value += 500 * Level;
            else
                Value += 100 * (int)Math.Pow(2, lines - 1) * Level;
        }

        //===================================================================== PROPERTIES
        public int Value
        {
            get { return _value; }
            private set { _value = Math.Max(value, 0); }
        }
        public int Lines
        {
            get { return _lines; }
        }
        public int Level
        {
            get { return _lines / 10 + 1; }
        }
    }
}
