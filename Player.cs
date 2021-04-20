using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Objects that can move and die
    /// </summary>
    interface ICreature : IGameObject
    {
        int Health { get; set; }
        int Attack { get; set; }
        List<IGameObject> Loot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        bool Move(Point destination);
    }

    /// <summary>
    /// Represents the hero of our story and all that they contain
    /// </summary>
    class Player : ICreature
    {

        public int Row { get; }
        public int Col { get; }
        public bool Solid { get; }

        public Point Point { get; set; }
        public char Display { get; } = '☺';
        public int Health { get; set; }
        public int Attack { get; set; }
        public List<IGameObject> Loot { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public Player(Point origin)
        {
            Point = origin;

            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }

        public void Paint() 
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;

            Console.SetCursorPosition(Point.Col, Point.Row);
            Console.Write(Display);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public bool Move(Point destination)
        {
            Point = destination;
            return true;
        }

        public bool Collision(int x, int y) 
        {
            return this.Collision(new Point(x, y));
        }

        public bool Collision(Point obstacle)
        {
            return obstacle == Point;
        }

        public bool OnCollision()
        {
            return false;

        }
    }
}
