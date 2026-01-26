using Labb2_DungeonCrawler.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

class Program
{
    static void Main(string[] args)
    {
        Console.SetBufferSize(Console.WindowWidth, 60);
        GameLoop.GameStart();
    }
}
