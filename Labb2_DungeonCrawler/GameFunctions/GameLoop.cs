using Labb2_DungeonCrawler.State;
using MongoDB.Bson;
using System.Media;

namespace Labb2_DungeonCrawler;

public static class GameLoop
{

    public static async Task GameStart()
    {
        string pasteObjectIdHere = "69750f0948246dd5a0efd7cf";


        PlayMusicLoop();
        string userName = Graphics.WriteStartScreen();
        bool isAlive = true;
        int savedXP = 0;
        int savedHP = 100;
        ObjectId id;
        Console.CursorVisible = false;
        Console.WriteLine("Press [L] to load, [D] to delete save and start a new game, anything else to just start a new game");
        var loadNewOrDelete = Console.ReadKey(true);
        if (loadNewOrDelete.Key == ConsoleKey.D)
        {
            var selectedSave = SelectSaveFromList('D');
            ConfirmSaveDelete(selectedSave);
        }
        if (loadNewOrDelete.Key == ConsoleKey.L)
        {
            var selectedSave = SelectSaveFromList('L');
            id = selectedSave.Id;
        }
        else
        {
            id = ObjectId.Empty;
        }

        while (true)
        {
            GameState gameState;
            Player player;
            if (id != ObjectId.Empty)
            {
                gameState = LoadGame(id, userName);
            }
            else
            {
                gameState = StartNewGame(userName);
            }

            player = gameState.CurrentState.OfType<Player>().First();

            player.HP = savedHP;
            player.XP = savedXP;

            RunGameLoop(gameState, player);

            savedHP = player.HP;
            savedXP = player.XP;

        }
    }

    private static void PlayMusicLoop()
    {
        SoundPlayer musicPlayer = new SoundPlayer("ProjectFiles\\09. Björn Petersson - Uppenbarelse.wav");
        musicPlayer.PlayLooping();
    }

    private static GameState StartNewGame(string userName)
    {
        var gameState = new GameState(userName);

        Graphics.WriteLevelSelect(userName);
        LevelElement.LevelChoice(gameState);

        InitGame(gameState, userName, savedHP: null, savedXP: null);

        return gameState;
    }


    //SKA DEN HÄR METODEN INTEE VARA ASYNC?
    private static GameState LoadGame(ObjectId id, string userName)
    {
        var gameState = MongoConnection.MongoConnection.LoadGameFromDB(id).GetAwaiter().GetResult();

        if (gameState == null)
        {
            throw new Exception("Save not found.");
        }

        InitGame(gameState, userName, savedHP: null, savedXP: null);

        return gameState;
    }
    private static List<SaveInfoDTO> GetSavesPlayerName()
    {
        return MongoConnection.MongoConnection.GetActiveSavesFromDB()
        .GetAwaiter()
        .GetResult();
    }
    private async static void DeleteSave(ObjectId id)
    {
        await MongoConnection.MongoConnection.DeleteSaveFromDB(id);
    }

    private static Player InitGame(GameState gameState, string userName, int? savedHP, int? savedXP)
    {
        var player = gameState.CurrentState?
            .OfType<Player>()
            .FirstOrDefault()
            ?? throw new ArgumentNullException("No player found.");

        player.Name = userName;
        player.LoadPlayerData();


        if (savedHP.HasValue && savedXP.HasValue)
        {
            player.HP = savedHP.Value;
            player.XP = savedXP.Value;
        }

        foreach (var element in gameState.CurrentState ?? Enumerable.Empty<LevelElement>())
        {
            element.SetGame(gameState);
        }

        Console.Clear();

        var logMessage = player.PrintUnitInfo();
        gameState.MessageLog.MyLog.Add(logMessage);

        UpdateWalls(gameState);
        DrawAll(gameState, player);

        return player;
    }

    private static void RunGameLoop(GameState gameState, Player player)
    {
        while (player.HP > 0)
        {

            Graphics.WriteInfo();
            var menuChoice = Console.ReadKey(true);
            if (menuChoice.Key == ConsoleKey.Escape)
            {
                MongoConnection.MongoConnection.SaveGameToDB(gameState);
                Console.Clear();
                return;
                //savedXP = player.XP;
                //savedHP = player.HP;
                //break;
            }

            if (player.playerDirection.ContainsKey(menuChoice.Key) || menuChoice.Key == ConsoleKey.Z)
            {
                player.Update(menuChoice);
            }

            UpdateWalls(gameState);
            UpdateEnemies(gameState);
            HandleDeadEnemies(gameState, player);
            DrawAll(gameState, player);
            MongoConnection.MongoConnection.SaveGameToDB(gameState);
        }
        ;



        HandlePlayerDeath(player);

    }

    private static void DrawAll(GameState gameState, Player player)
    {
        foreach (var element in gameState.CurrentState ?? Enumerable.Empty<LevelElement>())
        {
            if (element is Player)
            {
                element.Draw();
            }
            else if (element.GetDistanceTo(player) < 5)
            {
                element.Draw();
            }
        }
    }

    private static void UpdateWalls(GameState gameState)
    {
        var walls = gameState.CurrentState?.OfType<Wall>().ToList() ?? new List<Wall>();

        foreach (var wall in walls ?? Enumerable.Empty<Wall>())
        {
            wall.Update();
            if (wall.IsToBeDrawn()) wall.Draw();
        }
    }

    private static void UpdateEnemies(GameState gameState)
    {
        var enemys = gameState.CurrentState?.OfType<Enemy>().ToList() ?? new List<Enemy>();

        foreach (var enemy in enemys)
        {
            enemy.Erase();
            enemy.Update();
        }
    }

    private static void HandleDeadEnemies(GameState gameState, Player player)
    {
        var deadRats = gameState.CurrentState?.OfType<Rat>().Where(e => e.HP <= 0).ToList() ?? new List<Rat>();
        foreach (var rat in deadRats)
        {
            player.XP += 23;
        }
        var deadSneaks = gameState.CurrentState?.OfType<Snake>().Where(e => e.HP <= 0).ToList() ?? new List<Snake>();
        foreach (var snake in deadSneaks)
        {
            player.XP += 57;
        }
        var deadKings = gameState.CurrentState?.OfType<TheRatKing>().Where(e => e.HP <= 0).ToList() ?? new List<TheRatKing>();
        foreach (var king in deadKings)
        {
            player.XP += 132;
        }

        gameState.CurrentState?.RemoveAll(e => e is Enemy enemy && enemy.HP <= 0);
    }

    private static void HandlePlayerDeath(Player player)
    {
        //isAlive = false;
        //savedXP = 0;
        //savedHP = 100;
        Graphics.WriteEndScreen(player);

        ConsoleKeyInfo menuChoice;
        do
        {
            menuChoice = Console.ReadKey(true);
        }
        while (menuChoice.Key != ConsoleKey.Escape && menuChoice.Key != ConsoleKey.Enter);
        if (menuChoice.Key == ConsoleKey.Enter) Console.Clear();
        //else if (menuChoice.Key == ConsoleKey.Escape) 
    }
    static SaveInfoDTO SelectSaveFromList(char purpose)
    {
        var saves = GetSavesPlayerName();
        int index = 0;
        ConsoleKey key;
        var selectedColor = new ConsoleColor();
        var notSelectedColor = new ConsoleColor();
        if (purpose == 'D')
        {
            notSelectedColor = ConsoleColor.Green;
            selectedColor = ConsoleColor.Red;
        }
        else
        {
            notSelectedColor = ConsoleColor.Red;
            selectedColor = ConsoleColor.Green;
        }
        do
        {
            Console.Clear();

            for (int i = 0; i < saves.Count; i++)
            {
                if (i == index)
                {
                    Console.ForegroundColor = selectedColor;
                    Console.WriteLine($">    {saves[i].PlayerName}, {saves[i].Id.ToString().Substring(saves[i].Id.ToString().Length - 5)}");
                }
                else
                {
                    Console.ForegroundColor = notSelectedColor;
                    Console.WriteLine($"    {saves[i].PlayerName}, {saves[i].Id.ToString().Substring(saves[i].Id.ToString().Length - 5)}");
                }
            }

            Console.ResetColor();

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && index > 0)
                index--;
            else if (key == ConsoleKey.DownArrow && index < saves.Count - 1)
                index++;

        } while (key != ConsoleKey.Enter);

        Console.ResetColor();
        Console.Clear();

        return saves[index];
    }
    static void ConfirmSaveDelete(SaveInfoDTO selectedSave)
    {
        var key = new ConsoleKey();
        Console.Clear();
        Console.SetCursorPosition(15, 10);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"are you sure you want to delete {selectedSave.PlayerName}?\n                         [Y]es | [N]o");
        do
        {
            key = Console.ReadKey(true).Key;
        }
        while (key != ConsoleKey.Y && key != ConsoleKey.N);
        if (key == ConsoleKey.Y)
        {
            DeleteSave(selectedSave.Id);        
        }
        Console.ResetColor();
    }
}
