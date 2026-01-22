using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

public class Wall : LevelElement
{
    private bool IsFound = false;
    public Wall()
    {
        Symbol = '#';
        MyColor = ConsoleColor.DarkGray;
    }
    
    public void Update()
    {
        var player = Game.CurrentState.OfType<Player>().First();

        if (GetDistanceTo(player) < 5)
        {
            MyColor = ConsoleColor.Gray;
            IsFound = true;
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


