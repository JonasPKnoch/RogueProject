using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Represents a file manager for a text-based game.
    /// </summary>
    /// <author>Isac Zuniga</author>
    public class FileManager
    {
        private readonly string folderPath = "GameFiles/";
        private readonly string savedGameFile = "Saved Game.txt";
        private readonly string saveScoresFile = "Scores.csv";

        private readonly DataColumn nameColumn;
        private readonly DataColumn scoreColumn;
        private DataTable highScores;

        /// <summary>
        /// The file manager, reads, writes, and displays text files
        /// to screen.
        /// </summary>
        public FileManager()
        {
            nameColumn = new DataColumn()
            {
                ColumnName = "Player name",
                DataType = typeof(string)
            };
            scoreColumn = new DataColumn()
            {
                ColumnName = "High Score",
                DataType = typeof(int)
            };
            highScores = new DataTable();
            highScores.Columns.Add(nameColumn);
            highScores.Columns.Add(scoreColumn);
        }

        /// <summary>
        /// Deletes the save file if it exists
        /// </summary>
        /// <returns>True if deletd, fasle otherwise</returns>
        public bool DeleteSavedGame()
        {
            try
            {
                if (File.Exists($"{folderPath}{savedGameFile}"))
                {
                    File.Delete($"{folderPath}{savedGameFile}");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Problem deleting the file: {savedGameFile}");
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Saves the current player information to a file.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="playerName"></param>
        /// <param name="score"></param>
        public void SaveGameToFile(string playerName, int score, int level, int playerHealth)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter($"{folderPath}{savedGameFile}"))
                {
                    writer.Write($"{playerName},{score},{level},{playerHealth}");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Problem writing to the file: {savedGameFile}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Loads the game from the file, adding the information to a
        /// string array.
        /// </summary>
        /// <returns>Saved Game information</returns>
        public string[] LoadGameFromFile()
        {
            if (!File.Exists($"{folderPath}{savedGameFile}"))
            {
                return null;
            }
            else
            {
                try
                {
                    using (StreamReader reader = new StreamReader($"{folderPath}{savedGameFile}"))
                    {
                        while (!reader.EndOfStream)
                        {
                            string[] lines = reader.ReadLine().Split(',');
                            return lines;
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Problem reading the file: {savedGameFile}");
                    Console.WriteLine(ex.StackTrace);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the High Scores from the save file, then prints them along
        /// with a title and border. The scores are sorted in descending order.
        /// </summary>
        public void PrintHighScores()
        {
            PrintFile("Border.txt", 5, 2);
            PrintFile("TitleScores.txt", 8, 3);

            try
            {
                using (StreamReader reader = new StreamReader($"GameFiles/{saveScoresFile}"))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        string[] row = reader.ReadLine().Split(',');
                        highScores.Rows.Add(row);
                    }
                }

                var rows = from row in highScores.AsEnumerable()
                           orderby row["High Score"] descending
                           select row;

                highScores = rows.AsDataView().ToTable();

                int topPosition = 20;
                foreach (DataRow row in highScores.Rows)
                {
                    Console.SetCursorPosition(8, topPosition);
                    Console.WriteLine($"{row[0],-20} {row[1],12}");
                    topPosition += 2;
                }
                highScores.Clear();

            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Problem accessing the file: {saveScoresFile}");
                Console.WriteLine(ex.StackTrace);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Problem reading the file: {saveScoresFile}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Reads the given file from the folder
        /// and sets the position to the console to write.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="startLeft"></param>
        /// <param name="startTop"></param>
        public void PrintFile(string fileName, int startLeft, int startTop, ConsoleColor printColor = ConsoleColor.White)
        {
            try
            {
                using (StreamReader reader = new StreamReader($"{folderPath}{fileName}"))
                {
                    Console.ForegroundColor = printColor;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        Console.SetCursorPosition(startLeft, startTop);
                        Console.WriteLine(line);
                        startTop++; //Increments position for next line
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Problem accessing the file: {fileName}");
                Console.WriteLine(ex.StackTrace);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Problem reading the file: {fileName}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Reads scores from a csv file and adds the information
        /// to a DataTable. It is updated as needed, formated to
        /// be written to the csv, then the Data is written to the file.
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="playerScore"></param>
        public void SaveHighScore(string playerName, int playerScore)
        {

            try
            {
                using (StreamReader reader = new StreamReader($"{folderPath}{saveScoresFile}"))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        string[] row = reader.ReadLine().Split(',');
                        highScores.Rows.Add(row);
                    }
                }

                UpdateHighScore(playerName, playerScore, scoreColumn.ColumnName, highScores);

                string line = FormatToCSV(highScores);

                using StreamWriter writer = new StreamWriter($"{folderPath}{saveScoresFile}");
                writer.Write(line);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Problem accessing the file: {saveScoresFile}");
                Console.WriteLine(ex.StackTrace);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Problem reading/writing the file: {saveScoresFile}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Updates the highScores DataTable by adding the score
        /// if there is less than 10. If there are 10 scores in the
        /// table, it will then check if the new score is higher
        /// than one present in the DataTable. If it is higher
        /// it will remove the lowest score from the DataTable.
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="playerScore"></param>
        /// <param name="scoreColumn"></param>
        /// <param name="highScores"></param>
        private static void UpdateHighScore(string playerName,
        int playerScore, string scoreColumn, DataTable highScores)
        {
            int min = int.MaxValue;

            if (highScores.Rows.Count == 10)
            {
                foreach (DataRow scores in highScores.Rows)
                {
                    int level = scores.Field<int>(scoreColumn);
                    min = Math.Min(min, level);
                }

                for (int i = 0; i < highScores.Rows.Count; i++)
                {
                    if (playerScore > min)
                    {
                        highScores.Rows.Add(playerName, playerScore);
                        break;
                    }
                }

                for (int i = 0; i < highScores.Rows.Count; i++)
                {
                    DataRow dr = highScores.Rows[i];
                    if ((int)dr[scoreColumn] == min && playerScore > min)
                    {
                        dr.Delete();
                        break;
                    }
                }
                highScores.AcceptChanges();
            }
            else
            {
                highScores.Rows.Add(playerName, playerScore);
            }
        }

        private static string FormatToCSV(DataTable table)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append(table.Columns[i].ColumnName);
                sb.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    sb.Append(row[i].ToString());
                    sb.Append(i == table.Columns.Count - 1 ? "\n" : ",");
                }
            }
            return sb.ToString();
        }
    }
}
