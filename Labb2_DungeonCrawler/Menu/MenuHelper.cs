using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.Menu;

public static class MenuHelper
{
    public static int ShowMenu(string title, List<MenuOption> options, bool allowEscape = true)
    {
        int index = 0;
        ConsoleKey key;

        index = options.FindIndex(o => o.IsEnabled);
        if (index == -1) return -1;

        do
        {
            Console.Clear();
            Console.ResetColor();
            Console.WriteLine(title);
            Console.WriteLine();

            for (int i = 0; i < options.Count; i++)
            {
                var option = options[i];

                if (i == index)
                    Console.ForegroundColor = option.IsEnabled ? ConsoleColor.Green : ConsoleColor.DarkGray;
                else
                    Console.ForegroundColor = option.IsEnabled ? ConsoleColor.Gray : ConsoleColor.DarkGray;

                Console.WriteLine((i == index ? "> " : "  ") + option.Text);
            }

            Console.ResetColor();

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                do
                {
                    index = (index == 0) ? options.Count - 1 : index - 1;
                } while (!options[index].IsEnabled);
            }
            else if (key == ConsoleKey.DownArrow)
            {
                do
                {
                    index = (index == options.Count - 1) ? 0 : index + 1;
                } while (!options[index].IsEnabled);
            }

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
