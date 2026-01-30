using Labb2_DungeonCrawler.State;
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
    private static int widht = Console.WindowWidth;
    private static int height = Console.WindowHeight;

    public static void WriteInfo()
    {
        string info = "| press [L] to open message log | press [esc] to leave and play another level (HP and XP is saved) |";
        int left = (widht - info.Length) / 2;
        Console.SetCursorPosition(left, height - 4);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(info);
    }
    public static void PrintHighScore(List<HighScore> highScores)
    {
        int left = (widht - 15) / 2;
        int top = (height - highScores.Count - 2) / 2;
        Console.Clear();
        Console.SetCursorPosition(left, top);
        Console.WriteLine("Highscore:");
        
        for (int i = 0; i < highScores.Count; i++)
        {
            Console.SetCursorPosition(left, top + 2 + i);
            Console.WriteLine($"{i + 1}: {highScores[i].PlayerName} {highScores[i].Score} {(highScores[i].IsAlive ? "Alive" : "Dead")}");
        }

    }
    public static void WriteTitleScreen()
    {
        int width = 60;
        int height = 25;
        int startLeft = (Console.WindowWidth - width) / 2;
        int startTop = (Console.WindowHeight - height) / 2;
        int count = 0;
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.White;
        string pressToStart = "press any key to start";
        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(startLeft, startTop + i);
            for (global::System.Int32 j = 0; j < width; j++)
            {
                if(i == 0 || i == height-1) Console.Write('#');
                else if(j == 0 || j == width-1) Console.Write('#');
                else Console.Write(' ');
            }
        }
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.SetCursorPosition(startLeft + 30, startTop + 10);
        Console.WriteLine("kill the ratking");
        int startPositionX = startLeft + 28;
        int startPositionY = startTop + 10;
        while (!Console.KeyAvailable)
        {
            if (count % 2 == 0 )
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                for (int i = startTop + 2; i < startTop + 23; i++)
                {
                    for (global::System.Int32 j = startLeft + 3; j < startLeft + 57; j++)
                    {
                        Console.SetCursorPosition(j, i);
                        if ((i == startTop + 2 || i == startTop + 22) && (j % 4 == 0))
                        {
                            Console.Write('¤');
                        }
                        else if ((j == startLeft + 3 || j == startLeft + 56) && (i % 2 == 0)) Console.Write('¤');
                    }
                }
            }
            else if (count % 3 == 0)
            {
                for (int i = startTop + 2; i < startTop + 23; i++)
                {
                    
                    for (global::System.Int32 j = startLeft + 3; j < startLeft + 57; j++)
                    {
                        Console.SetCursorPosition(j, i);
                        if ((i == startTop + 2 || i == startTop + 22) && (j % 4 == 0))
                        {
                            Console.Write(' ');
                        }
                        else if ((j == startLeft + 3 || j == startLeft + 56) && (i % 2 == 0)) Console.Write(' ');
                        
                    }
                }
            }
            if (count % 5 == 0)
            {
                Console.SetCursorPosition(startLeft + 20, startTop + 20);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(pressToStart);
            }
            else if (count % 8 == 0)
            {
                Console.SetCursorPosition(startLeft + 20, startTop + 20);
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
                if (startPositionX < startLeft + 10)
                {
                    startPositionX = startLeft + 28;
                    startPositionY++;
                }
                if (startPositionY > startTop + 15)
                {
                    startPositionX = startLeft + 28;
                    startPositionY = startTop + 10;
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
        int leftPos;
        int topPos;
        string gameOver = "game over.";
        var roundInfo = new List<string>() { $"{player.Name} the {player.Class} died a heroic death fighting rats and", $"snakes in {player.TurnsPlayed} turns and gained {player.XP} xp", "press [enter] to accept your faith" };
        leftPos = (Console.WindowWidth - gameOver.Length) / 2;
        topPos = (Console.WindowHeight - 2) / 2;
        Console.SetCursorPosition(leftPos, topPos);
        foreach (var item in gameOver)
        {
            Console.Write(item);
            Thread.Sleep(200);
        }
        Console.ForegroundColor = ConsoleColor.DarkRed;
        leftPos = (Console.WindowWidth - roundInfo[0].Length) / 2;
        for (int i = 0; i < roundInfo.Count; i++)
        {
            Console.SetCursorPosition(leftPos, topPos + i + 2);
            foreach (var item in roundInfo[i])
            {
                Console.Write(item);
                Thread.Sleep(writingSpeed);
            }
        }
        //return Console.ReadKey(true);
    }
    public static string WriteStartScreen()
    {
        Console.Clear();
        string hello = "Hello there, whats your name?";
        int left = (widht - hello.Length) / 2;
        int top = (height - 2) / 2;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(left, top);
        foreach (var item in hello)
        {
            Console.Write(item);
            Thread.Sleep(writingSpeed);
        }
        Console.SetCursorPosition(left, top+2);
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

