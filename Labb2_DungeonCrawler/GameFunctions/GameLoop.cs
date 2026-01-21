using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Media;
using Labb2_DungeonCrawler.State;
using Labb2_DungeonCrawler.Log;

namespace Labb2_DungeonCrawler;

public abstract class GameLoop:LevelElement
{
    public static void GameStart()
    {
        SoundPlayer musicPlayer = new SoundPlayer("ProjectFiles\\09. Björn Petersson - Uppenbarelse.wav");
        musicPlayer.PlayLooping();
        var currentGameState = new GameState();
        string logMessage = string.Empty;
        while (true)
        {
            bool isAlive = true;
            int savedXP = -1;
            int savedHP = -1;
            ConsoleKeyInfo menuChoice;
            Console.CursorVisible = false;
            string userName = Graphics.WriteStartScreen();
            while(true)
            {
                //LevelData.Elements?.Clear();
                currentGameState.CurrentState?.Clear();
                Graphics.WriteLevelSelect(userName);
                LevelChoice(currentGameState);
                //var player = LevelData.Elements?.OfType<Player>().FirstOrDefault();
                var player = currentGameState.CurrentState?.OfType<Player>().FirstOrDefault();
                if (player == null)
                {
                    throw new ArgumentNullException("no player found, add a player to the map");
                }
                //var enemys = LevelData.Elements?.OfType<Enemy>().ToList();
                var enemys = currentGameState.CurrentState?.OfType<Enemy>().ToList();
                if (enemys == null)
                {
                    throw new ArgumentNullException("no enemies found, add enemies to the map");
                }
                var walls = currentGameState.CurrentState?.OfType<Wall>().ToList();
                if (walls == null)
                {
                    throw new ArgumentNullException("no walls found, add walls to the map");
                }
                if (savedHP != -1)
                {
                    player.HP = savedHP;
                    player.XP = savedXP;
                }
                player.Name = userName; 
                Console.Clear();
                logMessage = player.PrintUnitInfo();
                currentGameState.MessageLog.MyLog.Add(logMessage);
                Graphics.WriteInfo();
                foreach (var wall in walls ?? Enumerable.Empty<Wall>())
                {
                    wall.Update(player);
                    if (wall.IsToBeDrawn()) wall.Draw();
                }
                foreach (var element in currentGameState.CurrentState ?? Enumerable.Empty<LevelElement>())
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
                do
                {
                    Graphics.WriteInfo();
                    enemys = currentGameState.CurrentState?.OfType<Enemy>().ToList() ?? new List<Enemy>();
                    walls = currentGameState.CurrentState?.OfType<Wall>().ToList() ?? new List<Wall>();
                    menuChoice = Console.ReadKey(true);                                       
                    if (menuChoice.Key == ConsoleKey.Escape)
                    {
                        Console.Clear();
                        savedXP = player.XP;
                        savedHP = player.HP;
                        break;
                    }
                    if(player.playerDirection.ContainsKey(menuChoice.Key) 
                        || menuChoice.Key == ConsoleKey.Z) player.Update(menuChoice, logMessage, currentGameState.MessageLog);
                    foreach (var wall in walls)
                    {
                        wall.Update(player);
                        if (wall.IsToBeDrawn()) wall.Draw();
                    }
                    foreach (var enemy in enemys)
                    {
                        enemy.Erase();
                        enemy.Update(player, logMessage, currentGameState.MessageLog, currentGameState);
                    }
                    var deadRats = currentGameState.CurrentState?.OfType<Rat>().Where(e => e.HP <= 0).ToList() ?? new List<Rat>();
                    foreach (var rat in deadRats)
                    {
                        player.XP += 23;
                    }
                    var deadSneaks = currentGameState.CurrentState?.OfType<Snake>().Where(e => e.HP <= 0).ToList() ?? new List<Snake>();
                    foreach (var snake in deadSneaks)
                    {
                        player.XP += 57;
                    }
                    var deadKings = currentGameState.CurrentState?.OfType<TheRatKing>().Where(e => e.HP <= 0).ToList() ?? new List<TheRatKing>();
                    foreach (var king in deadKings)
                    {
                        player.XP += 132;
                    }

                    currentGameState.CurrentState?.RemoveAll(e => e is Enemy enemy && enemy.HP <= 0);

                    foreach (var element in currentGameState.CurrentState ?? Enumerable.Empty<LevelElement>())
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

                } while (player.HP > 0);

                if (player.HP <= 0)
                {
                    isAlive = false;
                    savedXP = 0;
                    savedHP = 100;
                    Graphics.WriteEndScreen(player);

                    do
                    {
                        menuChoice = Console.ReadKey(true); 
                    }
                    while (menuChoice.Key != ConsoleKey.Escape && menuChoice.Key != ConsoleKey.Enter);
                    if (menuChoice.Key == ConsoleKey.Enter) Console.Clear();
                    else if (menuChoice.Key == ConsoleKey.Escape) break;
                }
            }
            if (isAlive == false) break;
        }
    }
}
