using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.Menu;

public static class MenuHelper
{
    public static int ShowMenu(string title, List<string> options, int startIndex = 0, bool allowEscape = true)
    {
        int index = startIndex;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine(title);
            Console.WriteLine();

            for (int i = 0; i < options.Count; i++)
            {
                if (i == index)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine((i == index ? "> " : "  ") + options[i]);
            }

            Console.ResetColor();

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && index > 0) index--;
            if (key == ConsoleKey.DownArrow && index < options.Count - 1) index++;

            if (allowEscape && key == ConsoleKey.Escape)
                return -1;

        } while (key != ConsoleKey.Enter);

        return index;
    }
    public static bool ConfirmYesNo(string question)
    {
        ConsoleKey key;
        Console.Clear();
        Console.WriteLine($"{question} [Y/N]");

        do
        {
            key = Console.ReadKey(true).Key;
        } while (key != ConsoleKey.Y && key != ConsoleKey.N);

        return key == ConsoleKey.Y;
    }
}
