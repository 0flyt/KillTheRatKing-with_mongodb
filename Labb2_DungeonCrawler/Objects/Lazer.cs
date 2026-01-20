using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

public class Lazer : LevelElement
{
    public Lazer()
    {
        Name = "lazer";
        Symbol = '░';
        MyColor = ConsoleColor.Magenta;
        AttackDice = new Dice(0, 0, 20);
    }
}
