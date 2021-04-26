using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Represents a game titled Rogue.
    /// </summary>
    public class RogueGame
    {
        private FileManager fm;
        private Generator gen;
        private Dungeon dungeon;

        // Starting values
        private int score = 0;
        private int level = 1;
        // Starting health
        private int playerHealth = 15;
        private string playerName;

        private Stopwatch stopwatch;

        /// <summary>
        /// The Rogue Game, is a text based adventure where you explore
        /// and fight monsters in the dungeon.
        /// </summary>
        public RogueGame()
        {
            gen = new Generator(120, 30, 5, 5);
            fm = new FileManager();

            stopwatch = new Stopwatch();
        }

        public void Start()
        {
            Console.CursorVisible = false;
            GameMenu();
        }

        // Test method to create new dungeon
        private void NewDungeon()
        {
            //dungeon = null;
            gen.Generate(8);
            dungeon = gen.GetDungeon();
            dungeon.PaintAll();
            //dungeon.PlacePlayer();
        }

        /// <summary>
        /// Represents a start menu with the following choices for
        /// the user to enter. (H) Displays high scores, (B) Displays
        /// Title Screen, (Enter) Starts the game, (Control + L) loads
        /// the game file and starts game, (Control + D) deletes the game file, 
        /// (Escape) Ends the game application
        /// </summary>
        private void GameMenu()
        {
            DrawTitleScreen();
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
                    BeginGame();
                    break;
                }
                if ((userInput.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control
                    && userInput.Key == ConsoleKey.L)
                {
                    // Margins and time to display message
                    int startLeft = 10, startTop = 30, messageTime = 500;
                    string loadedMessage = "Loading Game...";
                    string notLoadedMessage = "No Save File to Load!";
                    if (LoadGame())
                    {
                        // Loaded Game Message
                        WriteMessage(startLeft, startTop, loadedMessage, ConsoleColor.DarkGreen);
                        ClearMessage(startLeft, startTop, loadedMessage.Length, messageTime);
                        Console.Clear(); // Clears to start game
                        BeginGame(true); // Passed true if there is a game loaded
                        break;
                    }
                    else
                    {
                        // If there is No Save File to Load Message
                        WriteMessage(startLeft, startTop, notLoadedMessage, ConsoleColor.DarkRed);
                        ClearMessage(startLeft, startTop, notLoadedMessage.Length, messageTime);
                    }
                }
                if ((userInput.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control
                    && userInput.Key == ConsoleKey.D)
                {
                    // Margins and time to display message
                    int startLeft = 10, startTop = 30, messageTime = 500;
                    string deletedMessage = "Save File Deleted!";
                    string notDeletedMessage = "No Save File To Delete!";
                    ConsoleColor color = ConsoleColor.Green;

                    if (fm.DeleteSavedGame())
                    {
                        // Delete save file message
                        WriteMessage(startLeft, startTop, deletedMessage, color);
                        ClearMessage(startLeft, startTop, deletedMessage.Length, messageTime);
                    }
                    else
                    {
                        // No file to delete message
                        WriteMessage(startLeft, startTop, notDeletedMessage, color);
                        ClearMessage(startLeft, startTop, notDeletedMessage.Length, messageTime);
                    }
                }
                if (userInput.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    Environment.Exit(0);
                }
            }
        }

        // Needs work
        /// <summary>
        /// Starts the game known as Rogue. If there is a loaded game we do not
        /// get the user name, if it is a loaded game we get the information from
        /// the loaded game and pass the health. The game ends when the player health
        /// reaches zero. The user can save and or just exit.
        /// </summary>
        /// <param name="loadedGame"></param>
        private void BeginGame(bool loadedGame = false)
        {
            NewDungeon(); // Creates the first dungeon
            if (!loadedGame)
            {
                playerName = GetUserName();
                dungeon.LoadPlayerHealth(playerHealth); // Starting Health
            }
            if (loadedGame)
            {
                dungeon.LoadPlayerHealth(playerHealth);
            }

            playerHealth = dungeon.CurrentPlayerHealth();

            dungeon.PaintAll(); // Paints first dungeon
            PrintGameInfo(playerName, score, dungeon.CurrentPlayerHealth(), level);
            stopwatch.Start();

            do
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);

                dungeon.MovePlayer(userInput.Key); // Passes the key to move the player
                PrintGameInfo(playerName, score, dungeon.CurrentPlayerHealth(), level);

                //Possible if statement that would go to new level
                //Update score method and NewDungeon method called here?
                //Test example shown below
                playerHealth = dungeon.CurrentPlayerHealth(); // Testing new level creation
                if (dungeon.atExit)
                {
                    // Needs to do all these to propely go to a new level
                    // Possible way to improve this?
                    Console.Clear();
                    NewDungeon();
                    //dungeon.PaintAll();
                    dungeon.LoadPlayerHealth(playerHealth);
                    PrintGameInfo(playerName, score, dungeon.CurrentPlayerHealth(), level);
                    Debug.Print("Got HERE!!!");
                }

                //Save and exit back to menu
                if ((userInput.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control
                    && userInput.Key == ConsoleKey.S)
                {
                    Console.Clear();
                    // Passes information to be saved
                    fm.SaveGameToFile(playerName, score, level, dungeon.CurrentPlayerHealth());
                    GameMenu();
                    break;
                }
                //Just exit to menu
                if (userInput.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    GameMenu();
                    break;
                }
            } while (dungeon.CurrentPlayerHealth() > 0); // While player is alive
            GameOverScreen();
        }

        /// <summary>
        /// Prints a game over screen, until the user
        /// presses B to return to the main menu.
        /// </summary>
        private void GameOverScreen()
        {
            Thread.Sleep(450);
            Console.Clear();
            while (true)
            {
                fm.PrintFile("GameOver.txt", 45, 15);

                ConsoleKeyInfo userInput = Console.ReadKey(true);

                if (userInput.Key == ConsoleKey.B)
                {
                    Console.Clear();
                    GameMenu();
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="score"></param>
        /// <param name="health"></param>
        /// <param name="attack"></param>
        /// <param name="level"></param>
        private void PrintGameInfo(string userName, int score, int health, int level)
        {
            Console.SetCursorPosition(34, 35);
            Console.WriteLine($"Player: {userName,-15} Score: {score,-15} Health: {health,-15} Level: {level,-15}");
        }

        /// <summary>
        /// Updates the users score and current level.
        /// Score and health are incremented based off of time taken
        /// to complete the level.
        /// </summary>
        private void UpdateGamePerLevel()
        {
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            int timePassed = ts.Seconds;
            int Time = 45;

            if (timePassed < Time)
            {
                score += 160;
                playerHealth += 10;
            }

            else if (timePassed < Time * 2)
            {
                score += 80;
                playerHealth += 8;
            }

            else if (timePassed < Time * 3)
            {
                score += 40;
                playerHealth += 6;
            }

            else if (timePassed < Time * 4)
            {
                score += 20;
                playerHealth += 4;
            }

            level++;
            stopwatch.Start();
        }

        /// <summary>
        /// Loads the player information to be displayed.
        /// </summary>
        /// <returns>True if there is a game to load, false otherwise</returns>
        private bool LoadGame()
        {
            string[] test = fm.LoadGameFromFile();
            if (test == null || test.Length == 0)
            {
                return false;
            }
            else
            {
                playerName = test[0];
                level = Int32.Parse(test[1]);
                score = Int32.Parse(test[2]);
                playerHealth = Int32.Parse(test[3]);

                return true;
            }
        }

        /// <summary>
        /// Gets the username for the game, displays messages to let
        /// the user know what the input should be, and if the given
        /// input is invalid, it displays an error message.
        /// </summary>
        /// <returns>User Name</returns>
        private string GetUserName()
        {
            // Margins
            int startLeft = 7;
            int startTopIM = 33, startTopEM = 35; // IM = Input Message, EM = Error Message

            int messageClearTime = 350;
            string inputMessage = "Enter your name as 3 characters: ";
            string errorMessage = "Invalid input!";

            // Writes initial input message
            WriteMessage(startLeft, startTopIM, inputMessage, ConsoleColor.DarkGreen);

            StringBuilder sb = new StringBuilder();
            int countLetters = 0; // Used to set cursor position as letters are added.
            do
            {
                Console.SetCursorPosition(startLeft + inputMessage.Length + countLetters, startTopIM);
                char c = Console.ReadKey().KeyChar;

                // Displays error message for non letters
                if (!char.IsLetter(c))
                {
                    WriteMessage(startLeft, startTopEM, errorMessage, ConsoleColor.DarkRed);
                    ClearMessage(startLeft, startTopEM, errorMessage.Length, messageClearTime);
                }

                // Adds the letter to the string builder
                if (char.IsLetter(c))
                {
                    sb.Append(char.ToUpper(c));
                    countLetters++; // Each letter added increments the count
                }
            } while (sb.Length < 3); // Ends the loop at a 3 letter user name

            ClearMessage(startLeft, startTopIM, inputMessage.Length + sb.Length);
            return sb.ToString();
        }

        /// <summary>
        /// Writes the given message on the screen with the margins
        /// passed. Optional colorToWrite, default is white.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="message"></param>
        /// <param name="colorToWrite"></param>
        private void WriteMessage(int left, int top, string message, ConsoleColor colorToWrite = ConsoleColor.White)
        {
            Console.ForegroundColor = colorToWrite;
            Console.SetCursorPosition(left, top);
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Used to clear messages for the user at the given position and length
        /// to clear. If delayClear is passed delays the
        /// deletion by the number passed in miliseconds.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="lengthToClear"></param>
        /// <param name="delayClear"></param>
        private void ClearMessage(int left, int top, int lengthToClear, int delayClear = 0)
        {
            Thread.Sleep(delayClear);
            for (int i = 0; i < lengthToClear; i++)
            {
                Console.SetCursorPosition(left + i, top);
                Console.WriteLine(" ");
            }
        }

        private void DrawTitleScreen()
        {
            fm.PrintFile("Border.txt", 5, 2);
            fm.PrintFile("CastleTextArt.txt", 50, 2);
            fm.PrintFile("Title.txt", 10, 5, ConsoleColor.DarkGreen);
            fm.PrintFile("TitleInfo.txt", 10, 18);
        }
    }
}
