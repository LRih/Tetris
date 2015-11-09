using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Tetris
{
    public static class Fonts
    {
        //===================================================================== VARIABLES
        private static PrivateFontCollection _privateFonts = new PrivateFontCollection();
        private static Dictionary<string, Font> _fonts;

        //===================================================================== INITIALIZE
        static Fonts()
        {
            _fonts = new Dictionary<string, Font>();
        }

        //===================================================================== TERMINATE
        public static void Dispose()
        {
            foreach (Font font in _fonts.Values)
                font.Dispose();
            _privateFonts.Dispose();
        }

        //===================================================================== FUNCTIONS
        public static void Add(string name, string filename, int size)
        {
            _privateFonts.AddFontFile(Application.StartupPath + @"\fonts\" + filename);
            _fonts.Add(name, new Font(_privateFonts.Families[_privateFonts.Families.Length - 1], size));
        }

        public static Font Get(string name)
        {
            return _fonts[name];
        }
    }
}
