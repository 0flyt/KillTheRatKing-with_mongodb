using Labb2_DungeonCrawler.GameFunctions;
using Labb2_DungeonCrawler.Log;
using Labb2_DungeonCrawler.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

public class Player : LevelElement
{
    public ConsoleKey LastMove { get; set; }
    public Dictionary<ConsoleKey, int> playerDirection;
    public Player(string name = "player")
    {
        AttackDice = new Dice(6, 2, 2);
        DefenceDice = new Dice(6, 2, 0);
        HP = 100;
        XP = 0;
        Name = name;
        TurnsPlayed = 0;
        Symbol = '@';
        MyColor = ConsoleColor.White;
        LastMove = ConsoleKey.RightArrow;
        playerDirection = new Dictionary<ConsoleKey, int>();
        playerDirection.Add(ConsoleKey.UpArrow, -1);
        playerDirection.Add(ConsoleKey.LeftArrow, -1);
        playerDirection.Add(ConsoleKey.DownArrow, +1);
        playerDirection.Add(ConsoleKey.RightArrow, +1);
    }
    public override string PrintUnitInfo()
    {
        if (TurnsPlayed == 10 || TurnsPlayed == 100 || TurnsPlayed == 1000 || TurnsPlayed == 10000 || TurnsPlayed == 100000)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', Console.WindowWidth));
        }
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        string returnMessage = $"|{Symbol}: {Name} | HP: {HP} | XP: {XP}| Attack: {AttackDice} | Defence: {DefenceDice} | Turn: {TurnsPlayed} |";
        Console.WriteLine(returnMessage);
        return returnMessage;
    }
    private void PlayerMoveMethod(ConsoleKeyInfo userMove, string logMessage, MessageLog messageLog)
    {
        int hold;
        LastMove = userMove.Key;
        if (userMove.Key == ConsoleKey.UpArrow || userMove.Key == ConsoleKey.DownArrow)
        {
            hold = this.yCordinate;
            this.yCordinate += playerDirection[userMove.Key];
        }
        else
        {
            hold = this.xCordinate;
            this.xCordinate += playerDirection[userMove.Key];
        }
        if (!this.IsSpaceAvailable())
        {
            CollideAndConcequences(this, logMessage, messageLog);
            if (userMove.Key == ConsoleKey.UpArrow || userMove.Key == ConsoleKey.DownArrow) this.yCordinate = hold;
            else this.xCordinate = hold;
        }
    }
    private void LazerShootMethod(ConsoleKey lastMove, int lazerLength, string logMessage, MessageLog messageLog, GameState currentGameState)
    {
        if (lastMove == ConsoleKey.UpArrow || lastMove == ConsoleKey.DownArrow)
        {
            for (global::System.Int32 i = 1; i <= lazerLength; i++)
            {
                Lazer lazer = new Lazer() { yCordinate = this.yCordinate + playerDirection[lastMove]*i, xCordinate = this.xCordinate };
                if (lazer.IsSpaceAvailable()) currentGameState.CurrentState?.Add(lazer);
                else
                {
                    lazer.CollideAndConcequences(this, logMessage, messageLog);
                    break;
                }
            }
        }
        else
        {
            for (global::System.Int32 i = 1; i <= lazerLength; i++)
            {
                Lazer lazer = new Lazer() { yCordinate = this.yCordinate, xCordinate = this.xCordinate + playerDirection[lastMove]*i };
                if (lazer.IsSpaceAvailable()) currentGameState.CurrentState?.Add(lazer);
                else
                {
                    lazer.CollideAndConcequences(this, logMessage, messageLog);
                    break;
                }
            }
        }
    }
    public void Update(ConsoleKeyInfo userMove, string logMessage, MessageLog messageLog, GameState currentGameState)
    {
        logMessage = this.PrintUnitInfo();
        messageLog.MyLog.Add(logMessage);
        this.TurnsPlayed++;
        this.Erase();
        var lazers = (currentGameState.CurrentState ?? Enumerable.Empty<LevelElement>()).OfType<Lazer>().ToList();
        if (currentGameState.CurrentState != null) currentGameState.CurrentState.RemoveAll(l => l is Lazer);
        foreach (var lazer in lazers)
        {
            lazer.Erase();
        }
        if (userMove.Key == ConsoleKey.Z) LazerShootMethod(LastMove, 3, logMessage, messageLog, currentGameState);
        else PlayerMoveMethod(userMove, logMessage, messageLog);
    }
}
