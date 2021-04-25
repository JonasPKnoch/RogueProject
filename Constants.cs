using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Tools for moving objects
    /// </summary>
    class Constants
    {
        public enum Cardinal
        {
            None, Up, Down, Left, Right
        }

        public static Point[] DirectionVectors =
            {
            new Point(0, 0),
            new Point(0, -1),
            new Point(0, 1),
            new Point(-1, 0),
            new Point(1, 0)
        };
    }
}
