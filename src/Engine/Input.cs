using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Tetris
{
    public static class Input
    {
        //===================================================================== VARIABLES
        private static Dictionary<Keys, int> _inputs;

        private static int _repeatDelay;
        private static Point _ptClick;

        //===================================================================== INITIALIZE
        static Input()
        {
            _inputs = new Dictionary<Keys, int>();

            _repeatDelay = 60;
            _ptClick = new Point();
        }

        //===================================================================== FUNCTIONS
        public static void Update()
        {
            List<Keys> keys = new List<Keys>(_inputs.Keys);
            foreach (Keys key in keys)
                _inputs[key]++;
        }

        public static void TriggerMouse(Keys key, int x, int y)
        {
            TriggerKey(key);
            _ptClick = new Point(x, y);
        }
        public static void TriggerKey(Keys key)
        {
            if (!_inputs.ContainsKey(key))
                _inputs.Add(key, 1);
        }
        public static void Untrigger(Keys key)
        {
            if (_inputs.ContainsKey(key))
                _inputs.Remove(key);
        }

        public static bool IsDown(Keys key)
        {
            return _inputs.ContainsKey(key);
        }
        public static bool IsTrigger(Keys key)
        {
            return _inputs.ContainsKey(key) && _inputs[key] == 1;
        }
        public static bool IsRepeat(Keys key)
        {
            return _inputs.ContainsKey(key) && _inputs[key] >= _repeatDelay;
        }

        //===================================================================== PROPERTIES
        public static int RepeatDelay
        {
            get { return _repeatDelay; }
            set { _repeatDelay = Math.Max(value, 0); }
        }

        public static List<Keys> Triggers
        {
            get
            {
                List<Keys> keys = new List<Keys>();
                foreach (Keys key in _inputs.Keys)
                    if (IsTrigger(key)) keys.Add(key);
                return keys;

            }
        }

        public static Point ClickPt
        {
            get { return _ptClick; }
        }
    }
}
