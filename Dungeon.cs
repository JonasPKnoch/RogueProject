﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    class Dungeon
    {
        public char[,] Map;
        public Generator gen;

        public Dungeon(char[,] map)
        {
            Map = map;
        }

        public Dungeon()
        {
            Generator gen = new Generator(Console.WindowWidth - 10, Console.WindowHeight - 5);
            gen.Generate(4);
            gen.Draw();


        }

        public void Paint()
        {
            for(int i = 0; i < Map.Length; i++)
            {
                for(int j = 0; j < Map.Length; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.WriteLine(Map[i, j]);
                }
            }
        }
    }
}
