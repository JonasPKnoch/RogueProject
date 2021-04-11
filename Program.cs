using System;

namespace DungeonGenerationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Generator gen = new Generator(150, 150);
            gen.Generate(35);
            gen.Draw();

            Console.SetCursorPosition(0, 150); 
        }
    }
}
