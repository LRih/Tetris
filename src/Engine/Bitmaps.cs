using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Tetris
{
    public static class Bitmaps
    {
        //===================================================================== VARIABLES
        private static Dictionary<string, Bitmap> _bitmaps;

        //===================================================================== INITIALIZE
        static Bitmaps()
        {
            _bitmaps = new Dictionary<string, Bitmap>();
        }

        //===================================================================== TERMINATE
        public static void Dispose()
        {
            foreach (Bitmap bmp in _bitmaps.Values)
                bmp.Dispose();
        }

        //===================================================================== FUNCTIONS
        public static void Add(string name, string filename)
        {
            Add(name, new Bitmap(Application.StartupPath + @"\images\" + filename));
        }
        public static void Add(string name, Bitmap bmp)
        {
            if (_bitmaps.ContainsKey(name))
            {
                Get(name).Dispose();
                _bitmaps.Remove(name);
            }

            _bitmaps.Add(name, bmp);
        }

        public static Bitmap Get(string name)
        {
            return _bitmaps[name];
        }
    }
}
