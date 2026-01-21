using Labb2_DungeonCrawler.Log;
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
    public override void Update(Player player, string logMessage, MessageLog messageLog)
    {
        this.lifeTime--;
        if (lifeTime <= 0)
        {
            this.Erase();
            LevelData.Elements.Remove(this);
        }
    }
    public static void AddRatTails(int random0To3, int y, int x, Player player, string logMessage, MessageLog messageLog)
    {
        switch (random0To3)
        {
            case 0:
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail = new TheKingsTail() { yCordinate = y - i, xCordinate = x };
                    if (tail.IsSpaceAvailable()) LevelData.Elements.Add(tail);
                    else
                    {
                        tail.CollideAndConcequences(player, logMessage, messageLog);
                        break;
                    }
                }
                for(int i = 1; i <= tailLength; i++)
                { 
                    TheKingsTail tail2 = new TheKingsTail() { yCordinate = y + i, xCordinate = x };
                    if (tail2.IsSpaceAvailable()) LevelData.Elements.Add(tail2);
                    else
                    {
                        tail2.CollideAndConcequences(player, logMessage, messageLog);
                        break;
                    }
                }
                break;
            case 1:
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail = new TheKingsTail() { yCordinate = y + i, xCordinate = x };
                    if (tail.IsSpaceAvailable()) LevelData.Elements.Add(tail);
                    else
                    {
                        tail.CollideAndConcequences(player, logMessage, messageLog);
                        break;
                    }
                }
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail2 = new TheKingsTail() { yCordinate = y, xCordinate = x - i };
                    if (tail2.IsSpaceAvailable()) LevelData.Elements.Add(tail2);
                    else
                    {
                        tail2.CollideAndConcequences(player, logMessage, messageLog);
                        break;
                    }
                }
                break;
            case 2:
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail = new TheKingsTail() { yCordinate = y, xCordinate = x - i };
                    if (tail.IsSpaceAvailable()) LevelData.Elements.Add(tail);
                    else
                    {
                        tail.CollideAndConcequences(player, logMessage, messageLog);
                        break;
                    }
                }
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail2 = new TheKingsTail() { yCordinate = y, xCordinate = x + i };
                    if (tail2.IsSpaceAvailable()) LevelData.Elements.Add(tail2);
                    else
                    {
                        tail2.CollideAndConcequences(player, logMessage, messageLog);
                        break;
                    }
                }
                break;
            case 3:
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail = new TheKingsTail() { yCordinate = y, xCordinate = x + i };
                    if (tail.IsSpaceAvailable()) LevelData.Elements.Add(tail);
                    else
                    {
                        tail.CollideAndConcequences(player, logMessage, messageLog);
                        break;
                    }
                }
                for (int i = 1; i <= tailLength; i++)
                {
                    TheKingsTail tail2 = new TheKingsTail() { yCordinate = y - i, xCordinate = x };
                    if (tail2.IsSpaceAvailable()) LevelData.Elements.Add(tail2);
                    else
                    {
                        tail2.CollideAndConcequences(player, logMessage, messageLog);
                        break;
                    }
                }
                break;
        }
    }
}
