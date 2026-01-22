using Labb2_DungeonCrawler.Log;
using Labb2_DungeonCrawler.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

public class TheKingsTail : Enemy
{
    private int lifeTime = 1;
    private static int tailLength = 3;
    public TheKingsTail()
    {
        Symbol = '¤';
        MyColor = ConsoleColor.DarkYellow;
        Name = "TheKingsTail";
        AttackDice = new Dice(0, 0, 20);
        DefenceDice = new Dice(20, 20, 20);
        HP = 200;
    }
    public override void Update()
    {
        this.lifeTime--;
        if (lifeTime <= 0)
        {
            this.Erase();
            Game.CurrentState.Remove(this);
        }
    }
    public void AddRatTails
        (
            int random0To3, 
            int y, 
            int x
        )
    {
        var player = Game.CurrentState.OfType<Player>().FirstOrDefault();
        if (player == null) return;

        var messageLog = Game.MessageLog;
        switch (random0To3)
        {
            case 0:
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail = new TheKingsTail() { yCordinate = y - i, xCordinate = x };
                    tail.SetGame(Game);
                    if (tail.IsSpaceAvailable()) Game.CurrentState.Add(tail);
                    else
                    {
                        tail.CollideAndConcequences(player);
                        break;
                    }
                }
                for(int i = 1; i <= tailLength; i++)
                { 
                    TheKingsTail tail2 = new TheKingsTail() { yCordinate = y + i, xCordinate = x };
                    tail2.SetGame(Game);
                    if (tail2.IsSpaceAvailable()) Game.CurrentState.Add(tail2);
                    else
                    {
                        tail2.CollideAndConcequences(player);
                        break;
                    }
                }
                break;
            case 1:
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail = new TheKingsTail() { yCordinate = y + i, xCordinate = x };
                    tail.SetGame(Game);
                    if (tail.IsSpaceAvailable()) Game.CurrentState.Add(tail);
                    else
                    {
                        tail.CollideAndConcequences(player);
                        break;
                    }
                }
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail2 = new TheKingsTail() { yCordinate = y, xCordinate = x - i };
                    tail2.SetGame(Game);
                    if (tail2.IsSpaceAvailable()) Game.CurrentState.Add(tail2);
                    else
                    {
                        tail2.CollideAndConcequences(player);
                        break;
                    }
                }
                break;
            case 2:
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail = new TheKingsTail() { yCordinate = y, xCordinate = x - i };
                    tail.SetGame(Game);
                    if (tail.IsSpaceAvailable()) Game.CurrentState.Add(tail);
                    else
                    {
                        tail.CollideAndConcequences(player);
                        break;
                    }
                }
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail2 = new TheKingsTail() { yCordinate = y, xCordinate = x + i };
                    tail2.SetGame(Game);
                    if (tail2.IsSpaceAvailable()) Game.CurrentState.Add(tail2);
                    else
                    {
                        tail2.CollideAndConcequences(player);
                        break;
                    }
                }
                break;
            case 3:
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail = new TheKingsTail() { yCordinate = y, xCordinate = x + i };
                    tail.SetGame(Game);
                    if (tail.IsSpaceAvailable()) Game.CurrentState.Add(tail);
                    else
                    {
                        tail.CollideAndConcequences(player);
                        break;
                    }
                }
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail2 = new TheKingsTail() { yCordinate = y - i, xCordinate = x };
                    tail2.SetGame(Game);
                    if (tail2.IsSpaceAvailable()) Game.CurrentState.Add(tail2);
                    else
                    {
                        tail2.CollideAndConcequences(player);
                        break;
                    }
                }
                break;
        }
    }
}
