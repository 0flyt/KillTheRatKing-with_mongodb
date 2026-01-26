using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Labb2_DungeonCrawler;

public static class Graphics
{
    private static int writingSpeed = 1; 
    public static void WriteInfo()
    {
        Console.SetCursorPosition(4, 40);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("| press [L] to open message log | press [esc] to leave and play another level (HP and XP is saved)");
    }
    public static void WriteTitleScreen()
    {
        Console.SetCursorPosition(0, 0);
        int count = 0;
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.White;
        string pressToStart = "press any key to start";
        for (int i = 0; i < 25; i++)
        {
            for (global::System.Int32 j = 0; j < 60; j++)
            {
                if(i == 0 || i == 24) Console.Write('#');
                else if(j == 0 || j == 59) Console.Write('#');
                else Console.Write(' ');
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.SetCursorPosition(30, 10);
        Console.WriteLine("kill the ratking");
        int startPositionX = 28;
        int startPositionY = 10;
        while (!Console.KeyAvailable)
        {
            if (count % 2 == 0 )
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                for (int i = 2; i < 23; i++)
                {
                    for (global::System.Int32 j = 3; j < 57; j++)
                    {
                        Console.SetCursorPosition(j, i);
                        if ((i == 2 || i == 22) && (j % 4 == 0))
                        {
                            Console.Write('¤');
                        }
                        else if ((j == 3 || j == 56) && (i % 2 == 0)) Console.Write('¤');
                    }
                }
            }
            else if (count % 3 == 0)
            {
                for (int i = 2; i < 23; i++)
                {
                    
                    for (global::System.Int32 j = 3; j < 57; j++)
                    {
                        Console.SetCursorPosition(j, i);
                        if ((i == 2 || i == 22) && (j % 4 == 0))
                        {
                            Console.Write(' ');
                        }
                        else if ((j == 3 || j == 56) && (i % 2 == 0)) Console.Write(' ');
                        
                    }
                }
            }
            if (count % 5 == 0)
            {
                Console.SetCursorPosition(20, 20);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(pressToStart);
            }
            else if (count % 8 == 0)
            {
                Console.SetCursorPosition(20, 20);
                foreach (var item in pressToStart)
                {
                    Console.Write(' ');
                }
            }
            if (count % 2 == 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(startPositionX, startPositionY);
                Console.Write("░");
                startPositionX--;
                if (startPositionX < 10)
                {
                    startPositionX = 28;
                    startPositionY++;
                }
                if (startPositionY > 15)
                {
                    startPositionX = 28;
                    startPositionY = 10;
                }
            }
            count++;
            Thread.Sleep(200);
        }

    }
    public static void WriteEndScreen(Player player)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        string gameOver = "game over.";
        string roundInfo = $"{player.Name} the {player.Class} died a heroic death fighting rats and\nsnakes in {player.TurnsPlayed} turns and gained {player.XP} xp\npress [enter] to accept your faith";
        Console.SetCursorPosition(15, 10);
        foreach (var item in gameOver)
        {
            Console.Write(item);
            Thread.Sleep(200);
        }
        Console.SetCursorPosition(0, 12);
        Console.ForegroundColor = ConsoleColor.DarkRed;
        foreach (var item in roundInfo)
        {
            Console.Write(item);
            Thread.Sleep(writingSpeed);
        }
        //return Console.ReadKey(true);
    }
    public static string WriteStartScreen()
    {
        Console.Clear();
        string hello = "Hello there, whats your name?";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(15, 10);
        foreach (var item in hello)
        {
            Console.Write(item);
            Thread.Sleep(writingSpeed);
        }
        Console.SetCursorPosition(15, 12);
        Console.CursorVisible = true;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Name: ");
        string result = Console.ReadLine();
        Console.CursorVisible = false;
        return result;
    }
    public static void WriteLevelSelect(string userName)
    {
        string levelSelect1 = "press [1] to play level 1";
        string levelSelect2 = "press [2] to play level 2";
        string levelSelect3 = "press [3] to play level 3";
        string levelSelect4 = "press [4] to generate a random level";
        string gameStart1 = $"Alright {userName}...";
        Console.SetCursorPosition(15, 14);
        Console.ForegroundColor = ConsoleColor.Green;
        foreach (var item in gameStart1)
        {
            Console.Write(item);
            Thread.Sleep(writingSpeed);
        }
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(15, 16);
        foreach (var item in levelSelect1)
        {
            Console.Write(item);
            Thread.Sleep(writingSpeed);
        }
        Console.SetCursorPosition(15, 17);
        foreach (var item in levelSelect2)
        {
            Console.Write(item);
            Thread.Sleep(writingSpeed);
        }
        Console.SetCursorPosition(15, 18);
        foreach (var item in levelSelect3)
        {
            Console.Write(item);
            Thread.Sleep(writingSpeed);
        }
        Console.SetCursorPosition(15, 19);
        foreach (var item in levelSelect4)
        {
            Console.Write(item);
            Thread.Sleep(writingSpeed);
        }
    }
}

