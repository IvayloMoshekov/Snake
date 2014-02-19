using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.IO;

class Game
{
    static int points { get; set; }
    static string playerName { get; set; }
    static string saveDir = @"save";
    static string dificulty { get; set; }
    static Snake snake;
    static Element food;
    static Element bonus;
    Random random = new Random();

    private void InitializeMainComponents()
    {
        // if there is no snake - make new one
        // else the load mothod has been called and we have a snake declared
        if (snake == null)
        {
            snake = new Snake();
        }
        food = new Element(random.Next(3, Console.WindowHeight),
            random.Next(0, Console.WindowWidth), '@');
        bonus = new Element(5, 5, ' ');
        DisplayPoints();
    }

    public static void SetDificulty(string level)
    {
        dificulty = level;
    }

    public static string GetDificulty()
    {
        return dificulty;
    }

    public void Play(string level)
    {
        InitializeMainComponents();
        int bonusFoodChance = 0;
        int bonusTime = 0;
        bool bonusOn = false;
        int pointsMultiplier = 1;
        int sleepLevel = 1;

        SetLevel(level, ref pointsMultiplier, ref sleepLevel);

        try
        {
            while (true)
            {
                food.Display();
                snake.Move();
                DisplayPoints();

                GetKeyPressed();

                //SaveGame(snake);
                Element snakeHead = snake.GetSnakeHead();

                //set level
                Thread.Sleep(sleepLevel);

                //bonus
                SpawnBonus(ref bonusFoodChance, ref bonusTime, ref bonusOn);
                DisplayBonusTime(bonusTime--, ref bonusOn);

                //feed snake
                if (snakeHead.row == food.row && snakeHead.col == food.col)
                {
                    snake.Move();
                    food.ChangeCoordinates(random.Next(3, Console.WindowHeight), random.Next(0, Console.WindowWidth));
                    points += pointsMultiplier;
                }

                //check if snake gets bonus
                else if (snakeHead.row == bonus.row && snakeHead.col == bonus.col)
                {
                    snake.Move();
                    points += 10 + pointsMultiplier;
                    Console.ResetColor();
                    bonusOn = false;
                    bonusTime = 1;
                }

                //normal movement.
                else
                {
                    snake.RemoveSnakeTail();
                }
            }
        }
        catch (GameException)
        {
            GameOver();
        }
    }

    public static void SetLevel(string level, ref int pointsMultiplier, ref int sleepLevel)
    {
        SetDificulty(level);
        switch (level)
        {
            case "Hard":
                pointsMultiplier = 4;
                sleepLevel = 60;
                break;
            case "Medium":
                pointsMultiplier = 2;
                sleepLevel = 85;
                break;
            case "Easy":
                pointsMultiplier = 1;
                sleepLevel = 110;
                break;
        }
    }

    private static void DisplayPoints()
    {
        Console.SetCursorPosition(68, 0);
        Console.Write("Points: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(points);
        Console.ResetColor();
    }

    //tries to spawn a  bonus if there isnt one already
    private void SpawnBonus(ref int bonusFoodChance, ref int bonusTime, ref bool bonusOn)
    {
        if (bonusOn == false)
        {
            bonusFoodChance = random.Next(100, 400);
            if (bonusFoodChance < 105)
            {
                bonus = new Element(random.Next(3, Console.WindowHeight),
                     random.Next(0, Console.WindowWidth), '$');
                Console.ForegroundColor = ConsoleColor.Yellow;
                bonus.Display();
                Console.ResetColor();
                bonusOn = true;
                bonusTime = 80;
            }
        }
    }

    //displays on the top of the playfield the remaining time of the bonus
    private static void DisplayBonusTime(int bonusTime, ref bool bonusOn)
    {
        if (bonusTime > 0)
        {
            Console.SetCursorPosition((Console.WindowWidth / 2) - 3, 0);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Bonus: ");
            Console.ResetColor();
            Console.WriteLine(bonusTime / 10);
        }
        //clear bonus
        else if (bonusTime == 0)
        {
            Console.SetCursorPosition((Console.WindowWidth / 2) - 3, 0);
            Console.Write("           ");
            Console.SetCursorPosition(bonus.col, bonus.row);
            Console.WriteLine(' ');
            bonus.ChangeCoordinates(0, 0);
            bonusOn = false;
        }
    }

    private static void GetPLayerName()
    {
        Console.SetCursorPosition(0, 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Enter your name: ");
        playerName = Console.ReadLine();
        playerName = playerName.Replace(" ", string.Empty);
        while (playerName.Length > 20 || playerName.Length < 1)
        {
            ClearRow(1);
            Console.SetCursorPosition(0, 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter your name ([1-20] symbols): ");
            playerName = Console.ReadLine();
            playerName = playerName.Replace(" ", string.Empty);
        }
    }

    static void SaveGame(Snake snake)
    {
        GetPLayerName();

        List<Element> snakeElements = snake.GetSnakeElements();

        string fullFilePath = Path.Combine(saveDir, playerName + ".save");
        try
        {
            if (!File.Exists(fullFilePath))
            {
                Directory.CreateDirectory(saveDir);
                FileStream fs = System.IO.File.Create(fullFilePath);
                File.SetAttributes(fullFilePath, FileAttributes.Normal);
                fs.Flush();
                fs.Close();
            }
        }
        finally
        {
            using (StreamWriter writer = new StreamWriter(fullFilePath))
            {
                //TODO: exception handling of corrupted files
                writer.WriteLine("{0} {1} {2} {3}", playerName, points, GetDificulty(), snake.GetCurrentDirection());
                foreach (var element in snakeElements)
                {
                    writer.WriteLine("{0} {1} {2}", element.row, element.col, element.symbol);
                }
            }
        }
    }

    public string[] GetPlayers(string dir)
    {
        string[] files = null;
        try
        {
            files = Directory.GetFiles(dir);
            for (int index = 0; index < files.Length; index++)
            {
                // trim all but the name
                files[index] = files[index].Replace(dir + "\\", string.Empty);
                files[index] = files[index].Replace("." + dir, string.Empty);
            }

        }
        catch (FileNotFoundException)
        {
            throw new GameException("Failed loading highscores");
        }
        catch (IOException)
        {
            throw new GameException("Failed loading highscores");
        }
        return files;
    }

    static void LoadGame(string name)
    {
        Queue<Element> newSnakeElements = new Queue<Element>();

        string fullFilePath = Path.Combine(saveDir, name + ".save");

        if (File.Exists(fullFilePath))
        {
            using (StreamReader reader = new StreamReader(fullFilePath))
            {
                string[] playerSettings = reader.ReadLine().Split(' ');
                playerName = playerSettings[0];
                points = int.Parse(playerSettings[1]);
                SetDificulty(playerSettings[2]);
                int direction = int.Parse(playerSettings[3]);

                string line = reader.ReadLine();

                while (line != null)
                {
                    string[] element = line.Split(' ');

                    newSnakeElements.Enqueue(new Element(
                        int.Parse(element[0]),
                        int.Parse(element[1]),
                        (char)element[2][0]));

                    line = reader.ReadLine();
                }
                snake = new Snake(newSnakeElements);
                snake.SetCurrentDirection(direction);
            }
        }

    }

    public int Load(string name)
    {
        string[] players = GetPlayers(saveDir);

        if (players != null && players.Length != 0)
        {
            foreach (var file in players)
            {
                if (file.Equals(name))
                {
                    LoadGame(name);
                    ClearRow(1); // if the player is found clear the msg bellow
                    break;
                }
                else
                {
                    ClearRow(1);
                    WriteMSG("Player not found", 1);
                }
            }
            //atleast one save is found
            return 1;
        }
        else
        {
            // no saves in the folder
            return 0;
        }
    }

    private static void GameOver()
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Game Over!");
        PlayList playlist = new PlayList();
        playlist.PlayGameOverSong();
        SaveScore();
    }

    private void GetKeyPressed()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo userInput = Console.ReadKey();

            switch (userInput.Key)
            {
                case ConsoleKey.LeftArrow:
                    snake.SetCurrentDirection(0);
                    break;
                case ConsoleKey.RightArrow:
                    snake.SetCurrentDirection(1);
                    break;
                case ConsoleKey.UpArrow:
                    snake.SetCurrentDirection(2);
                    break;
                case ConsoleKey.DownArrow:
                    snake.SetCurrentDirection(3);
                    break;

                case ConsoleKey.S:
                    SaveGame(snake);
                    PauseGame("Game saved! Press 'P' to continue");
                    break;
                case ConsoleKey.P:
                    PauseGame("Game paused! Press 'P' to continue");
                    break;
            }
        }
    }

    private void PauseGame(string message)
    {
        bool pause = true;
        // clear the row
        ClearRow(1);
        WriteMSG(message, 1);

        while (pause)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
                switch (userInput.Key)
                {
                    case ConsoleKey.P:
                        pause = false;
                        break;
                }
            }
        }
        // clear the row
        ClearRow(1);
    }

    private static void ClearRow(int row)
    {
        // clear the row
        Console.SetCursorPosition(0, row);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.ResetColor();
    }

    private static void WriteMSG(string message, int row)
    {
        Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, row);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(message);
    }

    static void SaveScore()
    {
        try
        {
            string[,] scores = LoadScore();
            scores = AddScore(scores);
            using (StreamWriter writer = new StreamWriter("../../hightScores.txt"))
            {
                for (int index = 0; index < scores.GetLength(0); index++)
                {
                    if (scores[index, 0] != null)
                    {
                        writer.WriteLine(string.Format("{0} {1} {2}", scores[index, 0], scores[index, 1], scores[index, 2]));
                    }
                }

            }
        }
        catch (FileNotFoundException)
        {
            throw new GameException("Failed saving game");
        }
        catch (IOException)
        {
            throw new GameException("Failed saving game");
        }
    }

    private static string[,] AddScore(string[,] scores)
    {
        GetPLayerName();
        string pName = playerName;
        int pPoints = points;
        string pDif = dificulty;

        for (int index = 0; index < scores.GetLength(0); index++)
        {
            if (scores[index, 1] != null)
            {
                if (pPoints > int.Parse(scores[index, 1]))
                {
                    for (int replace = index; replace < scores.GetLength(0); replace++)
                    {
                        string tempPlayerName = scores[replace, 0];
                        string tempPoints = scores[replace, 1];
                        string tempDif = scores[replace, 2];
                        scores[replace, 0] = pName;
                        scores[replace, 1] = pPoints.ToString();
                        scores[replace, 2] = pDif;
                        pName = tempPlayerName;
                        if (!int.TryParse(tempPoints, out pPoints))
                        {
                            return scores;
                        }
                        pDif = tempDif;
                    }
                }
            }
            else
            {
                scores[index, 0] = pName;
                scores[index, 1] = pPoints.ToString();
                scores[index, 2] = pDif;
                break;
            }
        }
        return scores;
    }

    public static string[,] LoadScore()
    {
        string[,] scores = new string[5, 3];
        try
        {      
            using (StreamReader reader = new StreamReader("../../hightScores.txt"))
            {
                string line = reader.ReadLine();
                int count = 0;
                while (line != null)
                {
                    string[] player = line.Split(' ');
                    if (count == 5)
                    {
                        break;
                    }
                    scores[count, 0] = player[0];
                    scores[count, 1] = player[1];
                    scores[count, 2] = player[2];
                    count++;
                    line = reader.ReadLine();
                }
            }          
        }
        catch (FileNotFoundException)
        {
            throw new GameException("Failed loading game");
        }
        catch (IOException)
        {
            throw new GameException("Failed loading game");
        }
        return scores;
    }
}