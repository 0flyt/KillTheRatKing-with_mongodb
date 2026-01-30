using Labb2_DungeonCrawler.MusicAndSounds;
using Labb2_DungeonCrawler.Menu;
using Labb2_DungeonCrawler.State;
using MongoDB.Bson;
using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace Labb2_DungeonCrawler;

public static class GameLoop
{
    private static AudioFileReader musicTrack;
    private static WaveOutEvent musicPlayer;
    private static string currentTrack;


    public static async Task GameStart()
    {
        var levelPaths = new List<string>() { "ProjectFiles\\Level1.txt", "ProjectFiles\\Level2.txt", "ProjectFiles\\Level3.txt" };
        List<LevelModel> levels = LevelModel.LoadLevels(levelPaths);
        Graphics.WriteTitleScreen();
        PlayMusicLoop("ProjectFiles\\09. Björn Petersson - Uppenbarelse.wav");
        Console.ReadKey(true);

        while (true)
        {
            Console.CursorVisible = false;

            var saves = await GetSavesPlayerName();
            bool hasSaves = saves.Any();
       
            var mainMenuOptions = new List<MenuOption>
            {
                new MenuOption("Continue", hasSaves),
                new MenuOption("Load Save", hasSaves),
                new MenuOption("New Game"),
                new MenuOption("High Score")
            };
            int mainChoice = MenuHelper.ShowMenu("=== Main Menu ===", mainMenuOptions);

            ObjectId id = ObjectId.Empty;

            switch (mainChoice)
            {
                case -1: continue;

                case 0:
                    if (!hasSaves)
                    {
                        Console.Clear();
                        Console.WriteLine("No save!");
                        Console.ReadKey(true);
                        continue;
                    }
                    id = saves.First().Id;
                    break;

                case 1:
                    var selectedSave = await SelectSaveFromList();
                    if (selectedSave == null) continue;
                    id = selectedSave.Id;
                    break;
                    
                case 2:
                    id = ObjectId.Empty;
                    break;

                case 3:
                    await ShowHighScore();
                    continue;

                default:
                    continue;
            }

            GameState gameState;
            Player player;
            if (id != ObjectId.Empty)
            {
                gameState = await LoadGame(id);
            }
            else
            {
                gameState = await StartNewGame(levels);
            }

            player = gameState.CurrentState.OfType<Player>().First();

            gameState.XpScore = player.XP;
            
            await SaveToDb(gameState);

            await RunGameLoop(gameState, player, levels);
            await HandlePlayerDeath(player, id, gameState);
        }
    }

    private static async Task ShowHighScore()
    {
        var highScoresDead = await MongoConnection.MongoConnection.GetHighScoreFromDB();
        var highScoresAlive = await MongoConnection.MongoConnection.GetActiveSavesFromDB();

        var collectedHighScore = new List<HighScore>();

        collectedHighScore.AddRange(highScoresDead);

        foreach (var item in highScoresAlive)
        {
            collectedHighScore.Add(new HighScore 
            { 
                IsAlive = true, 
                PlayerName = item.PlayerName, 
                Score = item.PlayerXp 
            });
        }

        var sortedHighScore = collectedHighScore.OrderByDescending(s => s.Score).Take(10).ToList();
        Graphics.PrintHighScore(sortedHighScore);

        Console.ReadKey(true);
    }

    private static void PlayMusicLoop(string path)
    {
        StopMusic();

        musicTrack = new AudioFileReader(path);
        var loop = new LoopStream(musicTrack);

        musicPlayer = new WaveOutEvent();
        musicPlayer.Init(musicTrack);
        musicPlayer.Play();
    }
    private static void StopMusic()
    {
        if (musicPlayer == null)
            return;

        musicPlayer.Stop();
        musicPlayer.Dispose();
        musicPlayer = null;

        musicTrack?.Dispose();
        musicTrack = null;
    }

    private static async Task<GameState> StartNewGame(List<LevelModel> levels)
    {
        string PlayerName = Graphics.WriteStartScreen();
        var gameState = new GameState(PlayerName);
        var classChoice = await SelectClass(gameState);

        gameState = SelectLevel(PlayerName, gameState, levels);

        var player = gameState.CurrentState?
            .OfType<Player>()
            .FirstOrDefault()
            ?? throw new ArgumentNullException("No player found.");
        player.Class = classChoice;
        player.Name = PlayerName;

        InitGame(gameState);

        return gameState;
    }
    private static GameState SelectLevel(string PlayerName,GameState gameState, List<LevelModel> levels)
    {
        LevelElement.LevelChoice(PlayerName, gameState, levels);
        return gameState;
    }

    private static async Task<string> SelectClass(GameState gameState)
    {
        var classes = await GetClassesNames();
        var options = classes.Select(c => new MenuOption(c)).ToList();

        int index = MenuHelper.ShowMenu("=== Choose Class ===", options, false);

        gameState.ClassId = await MongoConnection.MongoConnection.GetClassId(classes[index]);

        return classes[index];
    }

    private static async Task<GameState> LoadGame(ObjectId id)
    {
        var gameState = await MongoConnection.MongoConnection.LoadGameFromDB(id);

        if (gameState == null)
        {
            throw new Exception("Save not found.");
        }


        InitGame(gameState);

        return gameState;
    }
    private static async Task<List<string>> GetClassesNames()
    {
        return await MongoConnection.MongoConnection.GetClassesFromDB();
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
    private static Task<List<SaveInfoDTO>> GetSavesPlayerName()
    {
        return MongoConnection.MongoConnection.GetActiveSavesFromDB();
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

        UpdateWalls(gameState);
        DrawAll(gameState, player);

        return player;
    }

    private static async Task RunGameLoop(GameState gameState, Player player, List<LevelModel> levels)
    {
        while (player.HP > 0)
        {
            Graphics.WriteInfo();
            var menuChoice = Console.ReadKey(true);
            if (menuChoice.Key == ConsoleKey.Escape)
            {
                await SaveToDb(gameState);
                Console.Clear();
                int hpHold = player.HP;
                int xpHold = player.XP;
                gameState = SelectLevel(player.Name, gameState, levels);
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
                await player.Update(menuChoice);
            }

            UpdateWalls(gameState);
            UpdateEnemies(gameState);
            HandleDeadEnemies(gameState, player);
            DrawAll(gameState, player);
            if (player.TurnsPlayed % 10 == 0)
            {
                await SaveToDb(gameState);
            }
        };
    }

    private static async Task SaveToDb(GameState gameState)
    {
        await MongoConnection.MongoConnection.SaveGameToDB(gameState);
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

    private static async Task HandlePlayerDeath(Player player, ObjectId id, GameState gameState)
    {
        await MongoConnection.MongoConnection.SaveHighScore(player.Name, player.XP);
        await DeleteSave(id);
        PlayMusicLoop("ProjectFiles\\03-3.wav");

        Graphics.WriteEndScreen(player);

        ConsoleKeyInfo menuChoice;
        do
        {
            menuChoice = Console.ReadKey(true);
        }
        while (menuChoice.Key != ConsoleKey.Enter);
        

    }
    static async Task<SaveInfoDTO> SelectSaveFromList()
    {
        var saves = await GetSavesPlayerName();

        if (saves.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("No saves. Press any key to return...");
            Console.ReadKey(true);
            return null;
        }

        var options = saves.Select(s => new MenuOption($"{s.PlayerName}, level {s.AktiveLevelName}, {s.PlayerXp} xp, {s.CreatedAt}", isEnabled: true)).ToList();

        int selectedIndex = MenuHelper.ShowMenu("=== Select Save ===", options);
        if (selectedIndex == -1) return null;

        return saves[selectedIndex];

    }
    static async Task ConfirmSaveDelete(SaveInfoDTO selectedSave)
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
            await DeleteSave(selectedSave.Id);        
        }
        Console.ResetColor();
    }
}
