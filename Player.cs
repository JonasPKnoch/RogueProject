using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Objects that can move and die
    /// </summary>
    interface ICreature : IGameObject
    {
        int Health { get; set; }
        int Attack { get; set; } // Used to roll to hit, and the amount of damage
        int Defense { get; set; }
        Random Rand { get; set; }
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
        public char Display { get; } = '@';
        public int Health { get; set; } = 10;

        private int attack;
        public int Attack { get
            {
                return Rand.Next(0, attack);
            }
            set { attack = value; } }
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

        public Player(Point origin, Random rand)
        {
            Point = origin;
            this.Rand = rand;
            Attack = 4;
            Defense = 4;

            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.Yellow;
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
            //System.Diagnostics.Debug.Print($"Player internal move: from {this.Point}, to {destination}");
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

        /// <summary>
        /// If the passed object is something that can attack, then it tries to hit this, reduces hit points if it does
        /// and returns true if the hitpoints drop to 0
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public bool OnCollision(IGameObject collider)
        {
            if (!(collider is ICreature)) { return false; } // shouldn't happen, but you never know
            //System.Diagnostics.Debug.Print($"Attacking player: Monster at {collider.Point}, player at {this.Point}");

            int attackRoll = ((ICreature)collider).Attack;
            Console.SetCursorPosition(64, 33);
            if (attackRoll >= Defense)
            {
                Health -= attackRoll;

                Console.Write($"Player hit for {attackRoll}  ");
                hitFlash();
            }
            else
            {
                Console.Write("Monster missed      ");
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
