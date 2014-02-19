using System;
using System.Collections.Generic;
using System.Text;

class Launcher
{
    static void Main(string[] args)
    {
        SetConsoleDefaultSettings();
        DisplayMainMenu("S N A K E", 13);
    }

    static void SetConsoleDefaultSettings()
    {
        Console.Title = "Snake";
        Console.BufferHeight = 40;
        Console.BufferWidth = 80;
        Console.CursorVisible = false;
        Console.SetWindowSize(80, 40);
    }

    static void DisplayMainMenu(string title, int underlineLength)
    {
        int selected = 0;
        bool switchToNextMenu = false;
        Game game = new Game();
        List<string> mainMenu = new List<string>() { 
            "New game", "Load game", "Highscore", "Instructions", "Exit game" 
        };
        int menuSize = mainMenu.Count - 1;

        DisplayMenuHeader(title, underlineLength);
        DisplayMenuContent(mainMenu, selected);

        while (!switchToNextMenu)
        {
            ConsoleKeyInfo userInput = Console.ReadKey();

            if (userInput.Key == ConsoleKey.DownArrow)
            {
                if (selected == menuSize)
                {
                    selected = -1;
                }
                Console.Clear();
                DisplayMenuHeader(title, underlineLength);
                DisplayMenuContent(mainMenu, ++selected);
            }
            else if (userInput.Key == ConsoleKey.UpArrow)
            {

                if (selected == 0)
                {
                    selected = menuSize + 1;
                }
                Console.Clear();
                DisplayMenuHeader(title, underlineLength);
                DisplayMenuContent(mainMenu, --selected);
            }
            else if (userInput.Key == ConsoleKey.Enter)
            {
                switch (selected)
                {
                    case 0:
                        Console.Clear();
                        DisplayLevelMenu();
                        switchToNextMenu = true;
                        break;
                    case 1:
                        Console.Clear();
                        DisplayLoadGames();
                        switchToNextMenu = true;

                        break;
                    case 2:
                        //view highscore
                        Console.Clear();
                        DisplayHightScores();
                        switchToNextMenu = true;
                        break;
                    case 3:
                        Console.Clear();
                        DisplayInstructions();
                        switchToNextMenu = true;
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }

    private static void DisplayWindowFrame()
    {
        Console.SetCursorPosition(0, 2);
        Console.Write(new string('-', Console.WindowWidth));
    }

    private static void DisplayMenuHeader(string msg, int underlineLength)
    {
        int middle = Console.WindowWidth / 2;
        Console.SetCursorPosition(middle - 7, 6);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
        Console.SetCursorPosition(middle - 9, 7);
        Console.WriteLine(new string('_', underlineLength));
    }

    private static void DisplayMenuContent(List<string> menuContent, int selected)
    {
        int middle = Console.WindowWidth / 2;
        int topPosition = 8;

        for (int i = 0; i < menuContent.Count; i++)
        {
            if (i == selected)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(middle - 7, topPosition += 3);
                Console.WriteLine(menuContent[selected]);
                Console.ResetColor();
            }
            else
            {
                Console.SetCursorPosition(middle - 7, topPosition += 3);
                Console.WriteLine(menuContent[i]);
            }
        }
    }

    private static void DisplayLevelMenu()
    {
        bool gameStarted = false;
        int selected = 0;
        Game game = new Game();
        List<string> gameLevels = new List<string>();
        gameLevels.Add("Easy");
        gameLevels.Add("Medium");
        gameLevels.Add("Hard");

        DisplayMenuHeader("Choose difficulty", 20);
        DisplayMenuContent(gameLevels, selected);

        while (!gameStarted)
        {
            ConsoleKeyInfo userInput = Console.ReadKey();

            if (userInput.Key == ConsoleKey.DownArrow)
            {
                if (selected == 2)
                {
                    selected = -1;
                }
                Console.Clear();
                DisplayMenuHeader("Choose difficulty", 20);
                DisplayMenuContent(gameLevels, ++selected);
            }
            else if (userInput.Key == ConsoleKey.UpArrow)
            {

                if (selected == 0)
                {
                    selected = 3;
                }
                Console.Clear();
                DisplayMenuHeader("Choose difficulty", 20);
                DisplayMenuContent(gameLevels, --selected);
            }
            else if (userInput.Key == ConsoleKey.Enter)
            {
                switch (selected)
                {
                    //easy    
                    case 0:
                        Console.Clear();
                        gameStarted = true;
                        DisplayWindowFrame();
                        game.Play(gameLevels[0]);
                        break;

                    //medium    
                    case 1:
                        Console.Clear();
                        gameStarted = true;
                        DisplayWindowFrame();
                        game.Play(gameLevels[1]);
                        break;

                    //hard  
                    case 2:
                        Console.Clear();
                        gameStarted = true;
                        DisplayWindowFrame();
                        game.Play(gameLevels[2]);
                        break;
                }
            }
            else if (userInput.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                DisplayMainMenu("S N A K E", 13);
                gameStarted = true;
            }
        }
    }

    private static void DisplayLoadGames()
    {
        int selected = 0;
        bool gameStarted = false;
        Game game = new Game();
        List<string> gameSaves = new List<string>();

        string[] players = game.GetPlayers("save");
        for (int index = 0; index < players.Length; index++)
        {
            gameSaves.Add(players[index]);
        }

        DisplayMenuHeader("Choose a saved game", 26);
        DisplayMenuContent(gameSaves, selected);

        while (!gameStarted)
        {
            ConsoleKeyInfo userInput = Console.ReadKey();

            if (userInput.Key == ConsoleKey.DownArrow)
            {
                if (selected == players.Length - 1)
                {
                    selected = -1;
                }
                Console.Clear();
                DisplayMenuHeader("Choose a saved game", 26);
                DisplayMenuContent(gameSaves, ++selected);
            }
            else if (userInput.Key == ConsoleKey.UpArrow)
            {

                if (selected == 0)
                {
                    selected = players.Length;
                }
                Console.Clear();
                DisplayMenuHeader("Choose a saved game", 26);
                DisplayMenuContent(gameSaves, --selected);
            }
            else if (userInput.Key == ConsoleKey.Enter)
            {
                game.Load(players[selected]);
                gameStarted = true;
                Console.Clear();
                DisplayWindowFrame();
                game.Play(Game.GetDificulty());
            }
            else if (userInput.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                DisplayMainMenu("S N A K E", 13);
                gameStarted = true;
            }
        }

    }

    private static void DisplayHightScores()
    {
        bool doneWatching = false;
        Game game = new Game();
        List<string> gameSaves = new List<string>();

        string[,] scores = Game.LoadScore();

        // add the high scores to the gameSaves
        for (int index = 0; index < scores.GetLength(0); index++)
        {
            if (scores[index, 0] == null)
            {
                if (index == 0)
                {
                    gameSaves.Add("No high scores, yet!");
                }
                break;
            }
            else
            {
                int countElements = 20 - scores[index, 0].Length;
                gameSaves.Add(string.Format("{0}{1}\t{2}\t{3}", scores[index, 0], new string(' ', countElements), scores[index, 1], scores[index, 2]));
            }
        }

        DisplayMenuHeader("Highest scores", 20);
        // print the scores
        DisplayMenuContent(gameSaves, -1);


        while (!doneWatching)
        {
            ConsoleKeyInfo userInput = Console.ReadKey();
            if (userInput.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                DisplayMainMenu("S N A K E", 13);
                doneWatching = true;
            }
        }
    }

    private static void DisplayInstructions()
    {
        DisplayMenuHeader("Instructions", 16);
        bool doneWatching = false;
        int middle = Console.WindowWidth / 2;
        int topPosition = 10;

        Console.SetCursorPosition(middle - 6, 10);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Navigation");

        Dictionary<char, string> content = new Dictionary<char, string>();
        content.Add('\u0018', "    Move up");
        content.Add('\u0019', "    Move down");
        content.Add('\u001A', "    Move left");
        content.Add('\u001B', "    Move right");
        content.Add('@', "    Gain points");
        content.Add('$', "    Earn bonus");
        content.Add('S', "    Save game");
        content.Add('P', "    Pause game");

        foreach (KeyValuePair<char, string> item in content)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(middle - 9, topPosition += 2);
            Console.Write(item.Key);
            Console.ResetColor();
            Console.WriteLine(item.Value);

            if (topPosition == 18)
            {
                topPosition += 2;
            }
        }

        Console.SetCursorPosition(middle - 6, 20);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Gameplay");


        while (!doneWatching)
        {
            ConsoleKeyInfo userInput = Console.ReadKey();
            if (userInput.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                DisplayMainMenu("S N A K E", 13);
                doneWatching = true;
            }
        }
    }
}