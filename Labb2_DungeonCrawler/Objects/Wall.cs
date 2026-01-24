using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

public class Wall : LevelElement
{
    public bool IsFound { get; set; } = false;
    public Wall()
    {
        Symbol = '#';
        MyColor = ConsoleColor.DarkGray;
    }
    
    public void Update()
    {
        var player = Game.CurrentState.OfType<Player>().First();
        bool isNearPlayer = GetDistanceTo(player) < 5;

        if (isNearPlayer)
        {
            IsFound = true;
            MyColor = ConsoleColor.Gray;
        }
        else if (IsFound)
        {
            MyColor = ConsoleColor.DarkGray;
        }
    }
    public bool IsToBeDrawn()
    {
        return IsFound;
    }
}


