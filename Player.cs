﻿using System;
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
    class Player : ITempTile
    {
        public Point Coordinates { get; }
        public char Display { get; } = '☺';
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
