using System;

namespace Engine
{
    public static class MathUtils
    {
        //#==================================================================== VARIABLES
        private static Random _rand = new Random();

        //#==================================================================== FUNCTIONS
        public static int Rand(int low, int high)
        {
            return _rand.Next(low, high + 1);
        }
    }
}
