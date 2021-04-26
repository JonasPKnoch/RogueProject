using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// A creature that can move toward, attack and be attacked by the player 
    /// </summary>
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
        public bool JustMoved = false;

        public Monster(Point origin, Random rand)
        {
            Point = origin;
            this.Rand = rand;
            Attack = 3;
            Defense = 3;

            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Prints the Display parameter at the Point parameter using the colors BackgroundColor and Foreground color
        /// </summary>
        public void Paint()
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;

            Console.SetCursorPosition(Point.Col, Point.Row);
            Console.Write(Display);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Sets the Point parameter to the provided coordinates
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public bool Move(Point destination)
        {
            //System.Diagnostics.Debug.Print($"Monster internal move: from {this.Point}, to {destination}");
            Point = destination;
            return true;
        }

        /// <summary>
        /// Checks if the provided coordinates overlap with this object
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Collision(int x, int y)
        {
            return this.Collision(new Point(x, y));
        }

        /// <summary>
        /// Checks if the provided point overlaps with this object
        /// </summary>
        /// <param name="obstacle"></param>
        /// <returns></returns>
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
            //System.Diagnostics.Debug.Print($"Attacking monster: Player at {collider.Point}, monster at {this.Point}");

            int attackRoll = ((ICreature)collider).Attack;
            Console.SetCursorPosition(84, 33);
            if (attackRoll >= Defense)
            {
                Health -= attackRoll;

                Console.Write($"Monster hit for {attackRoll}  ");
                hitFlash();
            }
            else
            {
                Console.Write("Player missed               ");
            }
            return Health <= 0;
        }

        private void hitFlash()
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.SetCursorPosition(Point.Col, Point.Row);
            Console.Write(Display);

            Thread.Sleep(50);

            Paint();
        }
    }
}
