using Labb2_DungeonCrawler.Log;
using Labb2_DungeonCrawler.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;
public class Snake : Enemy
{
    public Snake()
    {
        AttackDice = new Dice(4, 3, 2);
        DefenceDice = new Dice(8, 1, 5);
        HP = 25;
        Name = "snake";
        Symbol = 's';
        MyColor = ConsoleColor.Yellow;
    }

    public void SnakeNextMove(Player player)
    {
        var directions = new Dictionary<string, double>();
        yCordinate++;
        if (IsSpaceAvailable()) directions["south"] = GetDistanceTo(player);
        yCordinate--;

        yCordinate--;
        if (IsSpaceAvailable()) directions["north"] = GetDistanceTo(player);
        yCordinate++;

        xCordinate++;
        if (IsSpaceAvailable()) directions["west"] = GetDistanceTo(player);
        xCordinate--;

        xCordinate--;
        if (IsSpaceAvailable()) directions["east"] = GetDistanceTo(player);
        xCordinate++;
        if (directions.Any())
        {
            var bestMove = directions.OrderByDescending(d => d.Value).First().Key;
            switch(bestMove)
            {
                case "south":
                    yCordinate++;
                    break;
                case "north":
                    yCordinate--;
                    break;
                case "west":
                    xCordinate++;
                    break;
                case "east":
                    xCordinate--;
                    break;
            }
        }

    }

    public override void Update(Player player, string logMessage, MessageLog messageLog, GameState currentGameState)
    {
        if (GetDistanceTo(player) < 2)
        {
            SnakeNextMove(player);
        }
    }
}
