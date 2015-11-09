using System;

namespace Tetris
{
    public class Gravity
    {
        //===================================================================== CONSTANTS
        private const int LAST_COUNTDOWN_LEVEL = 10;

        //===================================================================== VARIABLES
        private readonly int _fps;

        private int _level;
        private int _countdown = 0;
        private int _rows = 0;

        //===================================================================== INITIALIZE
        public Gravity(int fps, int level)
        {
            _fps = fps;
            _level = level;
        }

        //===================================================================== FUNCTIONS
        public void Update()
        {
            if (Level <= LAST_COUNTDOWN_LEVEL)
            {
                if (_countdown == 0) ResetCountdown();
                else _countdown--;
            }
        }

        public void ResetCountdown()
        {
            if (Level <= LAST_COUNTDOWN_LEVEL)
                _countdown = Math.Max(_fps - (Level - 1) * 3, 0); // falls faster by 3 frames every level
            else
                _rows = (Level - 11) / 5 + 1; // number of rows fallen per frame increases every 5 levels
        }
        
        //===================================================================== PROPERTIES
        public int Level
        {
            get { return _level; }
            set
            {
                _level = Math.Max(value, 0);
                ResetCountdown();
            }
        }

        public int FallRows
        {
            get
            {
                if (Level > LAST_COUNTDOWN_LEVEL) return _rows;
                else return (_countdown == 0 ? 1 : 0);
            }
        }

        public int Countdown // TODO debug only
        {
            get { return _countdown; }
        }
    }
}
