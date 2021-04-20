using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DungeonGenerationDemo
{
    public class RogueGame
    {
        private FileManager fm;
        private int score = 0;
        private int level = 0;
        private string name;
        string[] labels = { "Level: ", "Player: ", "Score: ", "Health: ", "Attack: ", "Defense: "};

        private Stopwatch stopwatch;

        public RogueGame()
        {
            stopwatch = new Stopwatch();
            fm = new FileManager();
        }

        public void Start()
        {
        Start:

            DrawTitleScreen();
            StartMenu();

            // Testing Game Screen
            fm.PrintFile("GameBorder.txt", 5, 2);
            string userName = GetUserName();

            Console.SetCursorPosition(7, 33);
            Console.WriteLine($"{labels[0] + level,-20} {labels[1] + userName,-20} {labels[2] + score,-20}");

            Console.SetCursorPosition(7, 36);
            Console.WriteLine($"{labels[3] ,-20} {labels[4] ,-20} {labels[5] ,-20}");
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.D)
                {
                    Console.Clear();
                    goto Start;
                }
            }
        }

        private void StartMenu()
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
            Console.Clear();
        }

        private string GetUserName()
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

        private void ClearMessage(int left, int top, int lengthToClear)
        {
            for (int i = 0; i < lengthToClear; i++)
            {
                Console.SetCursorPosition(left + i, top);
                Console.WriteLine(" ");
            }
        }

        private void UpdateScore()
        {
            TimeSpan ts = stopwatch.Elapsed;
            int caseSwitch = ts.Seconds;

            switch (caseSwitch)
            {
                case 45:
                    score += 160;
                    break;
                case 85:
                    score += 80;
                    break;
                case 125:
                    score += 40;
                    break;
                case 165:
                    score += 20;
                    break;
                default:
                    break;
            }
        }

        private bool LoadGame()
        {
            string[] test = fm.LoadGameFromFile();
            if (test == null || test.Length == 0)
            {
                Console.WriteLine("No Save File");
                return false;
            }
            else
            {
                level = Int32.Parse(test[0]);
                name = test[1];
                score = Int32.Parse(test[2]);
                Console.SetCursorPosition(7, 33);
                Console.WriteLine($"{labels[0] + level,-20} {labels[1] + name,-20} {labels[2] + score,-20}");
                return true;
            }
        }

        private void DrawTitleScreen()
        {
            fm.PrintFile("Border.txt", 5, 2);
            fm.PrintFile("CastleTextArt.txt", 50, 2);
            fm.PrintFile("Title.txt", 10, 5);
            fm.PrintFile("TitleInfo.txt", 10, 18);
        }
    }
}
