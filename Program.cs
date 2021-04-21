using System;
using System.Text;
using System.Threading;
using static DungeonGenerationDemo.Constants;

namespace DungeonGenerationDemo
{
    class Program
    {
        static FileManager fm = new FileManager();
        static Generator gen = new Generator(100, 30);
        static Dungeon dungeon = gen.GetDungeon();
        static string[] labels = { "Level: ", "Player: ", "HP: ", "Score: "};

        static void Main(string[] args)
        {
            Console.WindowWidth = 146;
            Console.CursorVisible = false;
            
            gen.Generate(6);
           
            DrawTitleScreen();
            StartMenu();

            Console.SetCursorPosition(0, 35);
        }    

        //Jason L
        private static void StartGame()
        {
            string userName = GetUserName();
            Console.SetCursorPosition(7, 33);
            Console.WriteLine($"{labels[0] ,-20} {labels[1] + userName,-12} {labels[2],-6} {labels[3] ,-20}");

            dungeon.PlacePlayer(); // TODO: test code
            dungeon.PaintAll();

            ConsoleKey key;

            do
            {
                dungeon.DisplayPlayerHealth(45, 33);
                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape) { break; }

                if (dungeon.MovePlayer(key)) { break; }

            } while (key != ConsoleKey.Escape);

            dungeon.DisplayPlayerHealth(45, 33);
        }

        //Isac Z
        private static void StartMenu()
        {
            while (true)
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);

                if (userInput.Key == ConsoleKey.H)
                {
                    fm.PrintHighScores();
                }
                if (userInput.Key == ConsoleKey.B)
                {
                    DrawTitleScreen();
                }
                if (userInput.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    StartGame();
                    break;
                }
                //Testing Control + L for load
                if ((userInput.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control && userInput.Key == ConsoleKey.L)
                {
                    break;
                }
                if (userInput.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    Environment.Exit(0);
                }
            }
        }

        //Isac Z
        private static string GetUserName()
        {
            // Testing user input
            Console.SetCursorPosition(7, 33);
            string inputMessage = "Enter your name as 3 characters: ";
            string errorMessage = "Invalid input!";
            Console.Write(inputMessage);

            StringBuilder sb = new StringBuilder();
            int countLet = 0;
            do
            {
                Console.SetCursorPosition(7 + inputMessage.Length + countLet, 33);

                char c = Console.ReadKey().KeyChar;
                if (!char.IsLetter(c))
                {
                    Console.SetCursorPosition(7, 35);
                    Console.WriteLine(errorMessage);
                    Thread.Sleep(350);
                    ClearMessage(7, 35, errorMessage.Length);
                }

                if (char.IsLetter(c))
                {
                    sb.Append(char.ToUpper(c));
                    countLet++;
                }
            } while (sb.Length < 3);

            return sb.ToString();
        }

        //Isac Z
        private static void ClearMessage(int left, int top, int lengthToClear)
        {
            for (int i = 0; i < lengthToClear; i++)
            {
                Console.SetCursorPosition(left + i, top);
                Console.WriteLine(" ");
            }
        }

        //Isac Z
        private static void DrawTitleScreen()
        {
            fm.PrintFile("Border.txt", 5, 2);
            fm.PrintFile("CastleTextArt.txt", 50, 2);
            fm.PrintFile("Title.txt", 10, 5);
            fm.PrintFile("TitleInfo.txt", 10, 18);
        }       
    }
}
