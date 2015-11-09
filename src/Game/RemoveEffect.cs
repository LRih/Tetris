using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tetris
{
    public class RemoveEffect
    {
        //===================================================================== VARIABLES
        private List<int> _disappearingRows = new List<int>();
        private float _disappearingOpacity = 1;

        private List<Point> _fallingRows = new List<Point>();

        //===================================================================== INITIALIZE
        public RemoveEffect()
        {
        }

        //===================================================================== FUNCTIONS
        public void Update()
        {
            DisappearingOpacity -= 0.2f;
        }

        public void Set(List<int> disappearingRows)
        {
            _disappearingRows = disappearingRows;
            DisappearingOpacity = 1;
        }

        //===================================================================== PROPERTIES
        public List<int> DisappearingRows
        {
            get { return _disappearingRows; }
        }
        public float DisappearingOpacity
        {
            get { return _disappearingOpacity; }
            set { _disappearingOpacity = Math.Min(Math.Max(value, 0), 1); }
        }

        public bool IsDisappearing
        {
            get { return DisappearingOpacity > 0; }
        }
    }
}
