using Labb2_DungeonCrawler.Log;
using Labb2_DungeonCrawler.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

public class TheRatKing : Enemy
{

    private Random random;
    
    public TheRatKing()
    {
        random = new Random();
        AttackDice = new Dice(6, 3, 2);
        DefenceDice = new Dice(6, 1, 1);
        HP = 85;
        Name = "TheRatKing";
        Symbol = 'R';
        MyColor = ConsoleColor.DarkRed;
        
    }
    
    public override void Update()
    {
        var player = Game.CurrentState.OfType<Player>().First();
        int move = random.Next(4);
        this.TurnsPlayed++;
        if (TurnsPlayed % 2 == 0)
        {
            switch (move)
            {
                case 0:
                    xCordinate--;
                    if (IsSpaceAvailable()) break;
                    else
                    {
                        CollideAndConcequences(player);
                        xCordinate++;
                        break;
                    }
                case 1:
                    xCordinate++;
                    if (IsSpaceAvailable()) break;
                    else
                    {
                        CollideAndConcequences(player);
                        xCordinate--;
                        break;
                    }
                case 2:
                    yCordinate--;
                    if (IsSpaceAvailable()) break;
                    else
                    {
                        CollideAndConcequences(player);
                        yCordinate++;
                        break;
                    }
                case 3:
                    yCordinate++;
                    if (IsSpaceAvailable()) break;
                    else
                    {
                        CollideAndConcequences(player);
                        yCordinate--;
                        break;
                    }

            }
            if (TurnsPlayed % 3 == 0 && HP > 0)
            {
                var tailSpawner = new TheKingsTail() { xCordinate = xCordinate, yCordinate = yCordinate };
                tailSpawner.SetGame(Game);
                tailSpawner.AddRatTails(move, yCordinate, xCordinate);
            }
        }
    }
}
