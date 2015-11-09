using System;

namespace Tetris
{
    public class HighScore : IComparable<HighScore>
    {
        //===================================================================== VARIABLES
        public string Name { get; set; }
        public readonly int Level;
        public readonly int Score;

        //===================================================================== INITIALIZE
        public HighScore(string name, int level, int score)
        {
            Name = name;
            Level = level;
            Score = score;
        }

        //===================================================================== FUNCTIONS
        public int CompareTo(HighScore score)
        {
            return score.Score - Score;
        }
    }
}
