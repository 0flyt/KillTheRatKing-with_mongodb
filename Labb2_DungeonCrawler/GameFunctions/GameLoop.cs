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

namespace Labb2_DungeonCrawler;

public static class GameLoop
{
    public static async Task GameStart()
    {
        PlayMusicLoop();
        string userName = Graphics.WriteStartScreen();
        Console.Clear();
        var currentGameState = new GameState();
        var player = new Player();
        bool isAlive = true;
        int savedXP = 0;
        int savedHP = 100;
        Console.CursorVisible = false;
        
        while (true)
        {
            if (userName == "bjorn")
            {
                currentGameState = await MongoConnection.MongoConnection.LoadGameFromDB("697388421d38729a2c371a64");
                foreach (var element in currentGameState.CurrentState)
                {
                    element.SetGame(currentGameState);
                }
            }
            else
            {
                currentGameState = StartNewGame(currentGameState, player, userName);
                
            }
            player = InitializeGame(currentGameState, player, savedHP, savedXP, userName);

            //var enemys = currentGameState.CurrentState?.OfType<Enemy>().ToList();
            //if (enemys == null)
            //{
            //    throw new ArgumentNullException("no enemies found, add enemies to the map");
            //}
            //var walls = currentGameState.CurrentState?.OfType<Wall>().ToList();
            //if (walls == null)
            //{
            //    throw new ArgumentNullException("no walls found, add walls to the map");
            //}

            while (player.HP > 0)
            {

                Graphics.WriteInfo();
                var menuChoice = Console.ReadKey(true);
                if (menuChoice.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    savedXP = player.XP;
                    savedHP = player.HP;
                    currentGameState.CurrentState.Clear();
                    await MongoConnection.MongoConnection.SaveGameToDB(currentGameState);
                    break;
                }
                if(player.playerDirection.ContainsKey(menuChoice.Key) 
                    || menuChoice.Key == ConsoleKey.Z) player.Update(menuChoice);

                UpdateWalls(currentGameState);
                UpdateEnemies(currentGameState);
                HandleDeadEnemies(currentGameState, player);
                DrawAll(currentGameState, player);
                await MongoConnection.MongoConnection.SaveGameToDB(currentGameState);
            };

            if (player.HP <= 0)
            {
                HandlePlayerDeath(ref isAlive, ref savedXP, ref savedHP, player, currentGameState);
                await MongoConnection.MongoConnection.SaveGameToDB(currentGameState);
            }
        }
    }

    private static void PlayMusicLoop()
    {
        SoundPlayer musicPlayer = new SoundPlayer("ProjectFiles\\09. Björn Petersson - Uppenbarelse.wav");
        musicPlayer.PlayLooping();
    }

    private static async Task LoadGame(GameState gameState)
    {
        gameState = await MongoConnection.MongoConnection.LoadGameFromDB("69736a4e2405af0356161604");
    }

    private static GameState StartNewGame(GameState currentGameState, Player player, string userName)
    {
        currentGameState = new GameState();
        //currentGameState = MongoConnection.MongoConnection
        //                .LoadGameFromDB(new MongoDB.Bson.ObjectId("69728824e80dc413b43a363d"))
        //                .GetAwaiter()
        //                .GetResult();
        Graphics.WriteLevelSelect(userName);
        LevelElement.LevelChoice(currentGameState);
        return currentGameState;
    }
    private static Player InitializeGame(GameState currentGameState, Player player, int savedHP, int savedXP, string userName)
    {
        player = currentGameState.CurrentState?.OfType<Player>().FirstOrDefault();
        if (player == null)
        {
            throw new ArgumentNullException("no player found, add a player to the map");
        }
        player.Name = userName;
        currentGameState.PlayerName = userName;

        if (savedHP != -1)
        player.Init();
        {
            player.HP = savedHP;
            player.XP = savedXP;
        }

        foreach (var element in currentGameState.CurrentState ?? Enumerable.Empty<LevelElement>())
        {
            element.SetGame(currentGameState);
        }

        Console.Clear();

        var logMessage = player.PrintUnitInfo();
        currentGameState.MessageLog.MyLog.Add(logMessage);

        DrawAll(currentGameState, player);
        return player;
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

    private static void HandlePlayerDeath(ref bool isAlive, ref int savedXP, ref int savedHP, Player player, GameState gameState)
    {
        isAlive = false;
        savedXP = 0;
        savedHP = 100;
        Graphics.WriteEndScreen(player, gameState);

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
