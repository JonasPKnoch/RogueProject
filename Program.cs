using System;
using System.Text;
using System.Threading;
using static DungeonGenerationDemo.Constants;

namespace DungeonGenerationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 146;
            Console.CursorVisible = false;
            RogueGame rogueGame = new RogueGame();
            rogueGame.Start();

            Console.SetCursorPosition(0, 35);
        }
    }
}
