using Labb2_DungeonCrawler.GameFunctions;
using Labb2_DungeonCrawler.Log;
using Labb2_DungeonCrawler.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

public abstract class Enemy : LevelElement
{
    public virtual void Update()
    {

    }
    public override string PrintUnitInfo()
    {
        Console.SetCursorPosition(0, 3);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 3);
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        string returnMessage = string.Empty;
        if (HP > 0)
        {
            returnMessage = $"|{Symbol}: {Name} | HP: {HP} | Attack: {AttackDice} | Defence: {DefenceDice} |";
            Console.WriteLine(returnMessage);
        }
        else
        {
            returnMessage = $"{Name} is now killed and dead";
            Console.WriteLine(returnMessage);
        }
        return returnMessage;
    }
}
