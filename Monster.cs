using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    class Monster : ICreature
    {

        public int Row { get; }
        public int Col { get; }
        public bool Solid { get; }

        public Point Point { get; set; }
        public char Display { get; } = 'δ';
        public int Health { get; set; } = 5;

        private int attack;
        public int Attack
        {
            get
            {
                return Rand.Next(0, attack);
            }
            set { attack = value; }
        }
        private int defense;
        public int Defense
        {
            get
            {
                return Rand.Next(0, defense);
            }
            set { defense = value; }
        }
        public Random Rand { get; set; }
        public List<IGameObject> Loot { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public Monster(Point origin, Random rand)
        {
            Point = origin;
            this.Rand = rand;
            Attack = 3;
            Defense = 3;

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
            return false;
        }

        public bool Collision(int x, int y)
        {
            return this.Collision(new Point(x, y));
        }

        public bool Collision(Point obstacle)
        {
            return obstacle == Point;
        }

        /// <summary>
        /// If the passed object is something that can attack, then it tries to hit this, reduces hit points if it does
        /// and returns true if the hitpoints drop to 0
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public bool OnCollision(IGameObject collider)
        {
            if (!(collider is ICreature)) { return false; } // shouldn't happen, but you never know

            int attackRoll = ((ICreature)collider).Attack;
            if (attackRoll >= Defense)
            {
                Health -= attackRoll;
            }
            return Health <= 0;
        }
    }
}
