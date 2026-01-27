using Labb2_DungeonCrawler.State;
using MongoDB.Bson;
using System.IO;
using System.Media;

namespace Labb2_DungeonCrawler;

public static class GameLoop
{
    private static SoundPlayer musicPlayer;
    private static string currentTrack;


    public static void GameStart()
    {
        //AddNewClass("Priest");
        //AddNewClass("Warrior");
        //AddNewClass("Wizard");
        //AddNewClass("Thief");
        //AddNewClass("Cat");
        while (true)
        {
            Graphics.WriteTitleScreen();
            Console.ReadKey(true);
            PlayMusicLoop("ProjectFiles\\09. Björn Petersson - Uppenbarelse.wav");
            ObjectId id;
            Console.CursorVisible = false;
            Console.Clear();
            Console.SetCursorPosition(0, 10);
            Console.WriteLine("Press [L] to load, [D] to delete save and start a new game,\nanything else to just start a new game");
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

            GameState gameState;
            Player player;
            if (id != ObjectId.Empty)
            {
                gameState = LoadGame(id);
            }
            else
            {
                gameState = StartNewGame();
            }

            player = gameState.CurrentState.OfType<Player>().First();

            gameState.XpScore = player.XP;

            ShowHighScore(gameState);
            Console.ReadKey(true);
            Console.Clear();
            
            SaveToDb(gameState);

            RunGameLoop(gameState, player);
            HandlePlayerDeath(player, id, gameState);
        }
    }

    private static void ShowHighScore(GameState gameState)
    {
        var player = gameState.CurrentState?.OfType<Player>().FirstOrDefault();
        var highScoresDead = MongoConnection.MongoConnection.GetHighScoreFromDB().GetAwaiter().GetResult();
        var highScoresAlive = MongoConnection.MongoConnection.GetActiveSavesFromDB().GetAwaiter().GetResult();

        var collectedHighScore = new List<HighScore>();

        collectedHighScore = highScoresDead;

        foreach (var item in highScoresAlive)
        {
            collectedHighScore.Add(new HighScore { IsAlive = true, PlayerName = item.PlayerName, Score = item.PlayerXp });
        }

        var sortedHighScore = collectedHighScore.OrderByDescending(s => s.Score).Take(10).ToList();
        Graphics.PrintHighScore(sortedHighScore);
    }

    private static async void AddNewClass(string newClass)
    {
        await MongoConnection.MongoConnection
            .AddClassToCollection(newClass);
    }
    private static void PlayMusicLoop(string path)
    {
        if (currentTrack == path)
            return;

        musicPlayer?.Stop();

        musicPlayer = new SoundPlayer(path);
        musicPlayer.PlayLooping();
        currentTrack = path;
    }

    private static GameState StartNewGame()
    {
        string PlayerName = Graphics.WriteStartScreen();
        var gameState = new GameState(PlayerName);
        var classChoice = SelectClass(gameState);

        gameState = SelectLevel(PlayerName, gameState);

        var player = gameState.CurrentState?
            .OfType<Player>()
            .FirstOrDefault()
            ?? throw new ArgumentNullException("No player found.");
        player.Class = classChoice;
        player.Name = PlayerName;

        InitGame(gameState);

        return gameState;
    }
    private static GameState SelectLevel(string PlayerName,GameState gameState)
    {
        Graphics.WriteLevelSelect(PlayerName);
        LevelElement.LevelChoice(gameState);
        return gameState;
    }

    private static string SelectClass(GameState gameState)
    {
        var classes = GetClassesNames();
        int index = 0;
        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.ResetColor();
            Console.WriteLine("select class:");
            for (int i = 0; i < classes.Count; i++)
            {
                if (i == index)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($">    {classes[i]}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"    {classes[i]}");
                }
            }

            Console.ResetColor();

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && index > 0)
                index--;
            else if (key == ConsoleKey.DownArrow && index < classes.Count - 1)
                index++;

        } while (key != ConsoleKey.Enter);

        Console.ResetColor();
        Console.Clear();

        gameState.ClassId = MongoConnection.MongoConnection.GetClassId(classes[index]).GetAwaiter().GetResult();

        return classes[index];
    }

    private static GameState LoadGame(ObjectId id)
    {
        var gameState = MongoConnection.MongoConnection.LoadGameFromDB(id).GetAwaiter().GetResult();

        if (gameState == null)
        {
            throw new Exception("Save not found.");
        }


        InitGame(gameState);

        return gameState;
    }
    private static List<string> GetClassesNames()
    {
        return MongoConnection.MongoConnection
                                .GetClassesFromDB()
                                .GetAwaiter()
                                .GetResult();
    }
    private static void PrintMessageLog(GameState gameState)
    {
        Console.Clear();

        var messages = gameState.MessageLog.MyLog;

        foreach (var message in messages)
        {
            if (message.Count() > 0 && message?[1] == '@')
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(message);
            }
            else if (message.Count() >= 1 && message?[0] == '|')
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(message);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(message);
            }
        }
        Console.ResetColor();
        Console.ReadKey(true);
        Console.Clear();
    }
    private static List<SaveInfoDTO> GetSavesPlayerName()
    {
        return MongoConnection.MongoConnection.GetActiveSavesFromDB()
        .GetAwaiter()
        .GetResult();
    }
    private async static Task DeleteSave(ObjectId id)
    {
        await MongoConnection.MongoConnection.DeleteSaveFromDB(id);
    }

    private static Player InitGame(GameState gameState)
    {
        var player = gameState.CurrentState?
            .OfType<Player>()
            .FirstOrDefault()
            ?? throw new ArgumentNullException("No player found.");

        player.LoadPlayerData();

        foreach (var element in gameState.CurrentState ?? Enumerable.Empty<LevelElement>())
        {
            element.SetGame(gameState);
        }

        Console.Clear();

        var logMessage = player.PrintUnitInfo();
        //gameState.MessageLog.MyLog.Add(logMessage);

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
                SaveToDb(gameState);
                Console.Clear();
                int hpHold = player.HP;
                int xpHold = player.XP;
                gameState = SelectLevel(player.Name, gameState);
                string nameHold = player.Name;
                string classHold = player.Class;
                player = gameState.CurrentState?
                    .OfType<Player>()
                    .FirstOrDefault()
                    ?? throw new ArgumentNullException("No player found.");
                player.Name = nameHold;
                player.Class = classHold;
                player.XP = xpHold;
                player.HP = hpHold;
                player = InitGame(gameState);
            }

            if (menuChoice.Key == ConsoleKey.L)
            {
                PrintMessageLog(gameState);
                player.PrintUnitInfo();
            }

            if (player.playerDirection.ContainsKey(menuChoice.Key) || menuChoice.Key == ConsoleKey.Z)
            {
                player.Update(menuChoice);
            }

            UpdateWalls(gameState);
            UpdateEnemies(gameState);
            HandleDeadEnemies(gameState, player);
            DrawAll(gameState, player);
            if (player.TurnsPlayed % 10 == 0)
            {
                SaveToDb(gameState);
            }
        };
    }

    private static void SaveToDb(GameState gameState)
    {
        MongoConnection.MongoConnection.SaveGameToDB(gameState).GetAwaiter().GetResult();
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
            gameState.XpScore += 23;
        }
        var deadSneaks = gameState.CurrentState?.OfType<Snake>().Where(e => e.HP <= 0).ToList() ?? new List<Snake>();
        foreach (var snake in deadSneaks)
        {
            player.XP += 57;
            gameState.XpScore += 57;
        }
        var deadKings = gameState.CurrentState?.OfType<TheRatKing>().Where(e => e.HP <= 0).ToList() ?? new List<TheRatKing>();
        foreach (var king in deadKings)
        {
            player.XP += 132;
            gameState.XpScore += 132;
        }

        gameState.CurrentState?.RemoveAll(e => e is Enemy enemy && enemy.HP <= 0);
    }

    private static void HandlePlayerDeath(Player player, ObjectId id, GameState gameState)
    {
        MongoConnection.MongoConnection.SaveHighScore(player.Name, player.XP).GetAwaiter().GetResult();
        DeleteSave(id).GetAwaiter().GetResult();
        PlayMusicLoop("ProjectFiles\\03-3.wav");

        Graphics.WriteEndScreen(player);

        ConsoleKeyInfo menuChoice;
        do
        {
            menuChoice = Console.ReadKey(true);
        }
        while (menuChoice.Key != ConsoleKey.Enter);
        

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
            DeleteSave(selectedSave.Id).GetAwaiter().GetResult();        
        }
        Console.ResetColor();
    }
}
