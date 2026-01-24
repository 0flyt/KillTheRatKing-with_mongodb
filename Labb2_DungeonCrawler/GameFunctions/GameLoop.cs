using Labb2_DungeonCrawler.Log;
using Labb2_DungeonCrawler.State;
using Labb2_DungeonCrawler.MongoConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MongoDB.Bson;

namespace Labb2_DungeonCrawler;

public static class GameLoop
{

    public static void GameStart()
    {
        PlayMusicLoop();
        string userName = Graphics.WriteStartScreen();
        bool isAlive = true;
        int savedXP = 0;
        int savedHP = 100;
        Console.CursorVisible = false;
        ObjectId id = ObjectId.Empty;
        //ObjectId id = ObjectId.Parse("6974f1a4483444867eae2100");

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

    private static Player InitGame(GameState gameState, string userName, int? savedHP, int? savedXP)
    {
        var player = gameState.CurrentState?
            .OfType<Player>()
            .FirstOrDefault() 
            ?? throw new ArgumentNullException("No player found.");

        //player.Name = userName;
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
        };



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
}
