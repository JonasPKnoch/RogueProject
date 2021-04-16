using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    interface ITempTile
    {

        Point Coordinates { get; }

        char Display { get; }

        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }

        void Paint();

        bool Collision(Point obstacle);


    }
    interface ICreature : ITempTile
    {
        int Health { get; set; }
        int Attack { get; set; }
        List<ITempTile> Loot { get; set; }
        bool Move(Point destination);
    }
    class Player : ICreature
    {
        public Point Coordinates { get; }
        public char Display { get; } = '☺';
        public int Health { get; set; }
        public int Attack { get; set; }
        public List<ITempTile> Loot { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public Player(Point origin)
        {
            Coordinates = origin;

            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }

        public void Paint() 
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;

            Console.SetCursorPosition(Coordinates.X, Coordinates.Y);
            Console.Write(Display);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public bool Move(Point destination)
        {
            return false;
        }

        public bool Collision(int x, int y) 
        {
            return this.Collision(new Point(x, y));
        }

        public bool Collision(Point obstacle)
        {
            return obstacle == Coordinates;
        }
    }
}
