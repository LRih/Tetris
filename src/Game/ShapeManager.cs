using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tetris
{
    public class ShapeManager
    {
        //===================================================================== CONSTANTS
        private static readonly int[][][,] SHAPES = new int[][][,]
        {
            new int[][,]
            {
                new int[,] { { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 } },
                new int[,] { { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 } },
                new int[,] { { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 } },
                new int[,] { { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 } },
            },
            new int[][,]
            {
                new int[,] { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 0, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 } },
                new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 } }
            },
            new int[][,]
            {
                new int[,] { { 1, 0, 0, 0 }, { 1, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 1, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 0, 0, 0 }, { 1, 1, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 1, 1, 0, 0 }, { 0, 0, 0, 0 } }
            },
            new int[][,]
            {
                new int[,] { { 0, 0, 1, 0 }, { 1, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 0, 0, 0 }, { 1, 1, 1, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 1, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } }
            },
            new int[][,]
            {
                new int[,] { { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } }
            },
            new int[][,]
            {
                new int[,] { { 0, 1, 1, 0 }, { 1, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 0, 0 }, { 0, 1, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 0, 0, 0 }, { 0, 1, 1, 0 }, { 1, 1, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 1, 0, 0, 0 }, { 1, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } }
            },
            new int[][,]
            {
                new int[,] { { 0, 1, 0, 0 }, { 1, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 0, 0, 0 }, { 1, 1, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 0, 0 }, { 1, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } }
            },
            new int[][,]
            {
                new int[,] { { 1, 1, 0, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 0, 1, 0 }, { 0, 1, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 0, 0, 0 }, { 1, 1, 0, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } },
                new int[,] { { 0, 1, 0, 0 }, { 1, 1, 0, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 0 } }
            },
        };
        
        // SRS kick system
        public static readonly Point[][] KICK_DATA = new Point[][]
        {
            new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, 1), new Point(0, -2), new Point(-1, -2) },
            new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, -1), new Point(0, 2), new Point(1, 2) },
            new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, -2), new Point(1, -2) },
            new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, -1), new Point(0, 2), new Point(-1, 2) }
        };
        public static readonly Point[][] I_KICK_DATA = new Point[][]
        {
            new Point[] { new Point(0, 0), new Point(-2, 0), new Point(1, 0), new Point(-2, -1), new Point(1, 2) },
            new Point[] { new Point(0, 0), new Point(-1, 0), new Point(2, 0), new Point(-1, 2), new Point(2, -1) },
            new Point[] { new Point(0, 0), new Point(2, 0), new Point(-1, 0), new Point(2, 1), new Point(-1, -2) },
            new Point[] { new Point(0, 0), new Point(1, 0), new Point(-2, -0), new Point(1, -2), new Point(-2, 1) }
        };

        public const int NEXT_COUNT = 3;
        public const int RECENT_COUNT = 4;

        //===================================================================== VARIABLES
        private Shape _holdShape;
        private Shape _nextFromHold;
        private List<Shape> _nextShapes = new List<Shape>(NEXT_COUNT);

        private Queue<int> _recentIDs = new Queue<int>(RECENT_COUNT);

        private bool _canHold = true;

        //===================================================================== INITIALIZE
        public ShapeManager()
        {
            for (int i = 0; i < NEXT_COUNT; i++)
                Generate();
        }

        //===================================================================== FUNCTIONS
        public Shape PopNext()
        {
            Shape next;

            if (_nextFromHold != null)
            {
                next = Generate(_nextFromHold.ID);
                _nextFromHold = null;
            }
            else
            {
                next = _nextShapes[0];
                _nextShapes.RemoveAt(0);
                Generate();
                _canHold = true;
            }

            return next;
        }
        public bool TryHold(Shape shape)
        {
            if (_canHold)
            {
                _nextFromHold = _holdShape;
                _holdShape = shape;
                _canHold = false;
                return true;
            }
            else
                return false;
        }

        private void Generate()
        {
            int id = 0;

            // try 3 times to roll a shape that wasn't rolled in past 4 shapes
            for (int i = 0; i < 3; i++)
            {
                id = MathUtils.Rand(1, SHAPES.Length - 1);
                if (!_recentIDs.Contains(id)) break;
            }
            _nextShapes.Add(Generate(id));
            AddRecent(id);
        }

        private void AddRecent(int id)
        {
            if (_recentIDs.Count == RECENT_COUNT) _recentIDs.Dequeue();
            _recentIDs.Enqueue(id);
        }

        public static Shape Generate(int id)
        {
            Point[][] kickData = (id == 1 ? I_KICK_DATA : KICK_DATA); // special kick data for I
            return new Shape(id, SHAPES[id], kickData, (Game.COL_COUNT - Shape.SIZE) / 2);
        }
        //===================================================================== PROPERTIES
        public Shape Hold
        {
            get { return _holdShape; }
        }
        public IList<Shape> Next
        {
            get { return _nextShapes; }
        }
    }
}
