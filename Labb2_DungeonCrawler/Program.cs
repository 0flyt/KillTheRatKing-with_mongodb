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
        ConsoleKeyInfo proceed;
        do
        {
            Console.SetBufferSize(Console.WindowWidth, 60);
            Graphics.WriteTitleScreen();
            proceed = Console.ReadKey(true);
            GameLoop.GameStart();
            proceed = Console.ReadKey(true);
        }
        while (proceed.Key != ConsoleKey.Escape);

    }
}
