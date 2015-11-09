using System;
using System.Drawing;

namespace Tetris
{
    public class Shape
    {
        //===================================================================== CONSTANTS
        public const int SIZE = 4;
        public const int ROTATIONS = 4;

        //===================================================================== VARIABLES
        public readonly int ID;
        private int[][,] _blockData;
        private Point[][] _kickData;

        public int X { get; private set; }
        public int Y { get; private set; }
        private int _rotation = 0;

        //===================================================================== INITIALIZE
        public Shape(int id, int[][,] blockData, Point[][] kickData, int x)
        {
            ID = id;
            _blockData = blockData;
            _kickData = kickData;
            X = x;
        }

        //===================================================================== FUNCTIONS
        public void Move(int rX, int rY)
        {
            X += rX;
            Y += rY;
        }
        public void RotateLeft()
        {
            _rotation--;
            if (_rotation == -1) _rotation = ROTATIONS - 1;
        }
        public void RotateRight()
        {
            _rotation = (_rotation + 1) % ROTATIONS;
        }

        public bool IsEmpty(int x, int y)
        {
            return _blockData[_rotation][y, x] == 0;
        }

        //===================================================================== PROPERTIES
        public Point[] KickData
        {
            get { return _kickData[_rotation]; }
        }

        public int Width
        {
            get
            {
                return Right - Left + 1;
            }
        }
        public int Height
        {
            get
            {
                return Bottom - Top + 1;
            }
        }

        public int Left
        {
            get
            {
                for (int x = 0; x < SIZE; x++)
                    for (int y = 0; y < SIZE; y++)
                        if (!IsEmpty(x, y)) return x;
                throw new Exception("No blocks in shape");
            }
        }
        public int Right
        {
            get
            {
                for (int x = SIZE - 1; x >= 0; x--)
                    for (int y = 0; y < SIZE; y++)
                        if (!IsEmpty(x, y)) return x;
                throw new Exception("No blocks in shape");
            }
        }
        public int Top
        {
            get
            {
                for (int y = 0; y < SIZE; y++)
                    for (int x = 0; x < SIZE; x++)
                        if (!IsEmpty(x, y)) return y;
                throw new Exception("No blocks in shape");
            }
        }
        public int Bottom
        {
            get
            {
                for (int y = SIZE - 1; y >= 0; y--)
                    for (int x = 0; x < SIZE; x++)
                        if (!IsEmpty(x, y)) return y;
                throw new Exception("No blocks in shape");
            }
        }
    }
}
