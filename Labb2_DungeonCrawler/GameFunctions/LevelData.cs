using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler;

public abstract class LevelData : LevelElement
{
    public static List<LevelElement>? Elements { get; set; }
 
	public static List<LevelElement> Load(string fileName)
	{
        int row = 4;
        Elements = new List<LevelElement>();
        foreach (var line in File.ReadAllLines(fileName))
        {
            for (int i = 0; i < line.Length; i++)
            {
                switch(line[i])
                {
                    case '#':
                        Elements.Add(new Wall() { yCordinate = row, xCordinate = i });
                        break;
                    case '@':
                        Elements.Add(new Player() { yCordinate = row, xCordinate = i });
                        break;
                    case 'r':
                        Elements.Add(new Rat() { yCordinate = row, xCordinate = i });
                        break;
                    case 's':
                        Elements.Add(new Snake() { yCordinate = row, xCordinate = i });
                        break;
                    case 'R':
                        Elements.Add(new TheRatKing() { yCordinate = row, xCordinate = i });
                        break;
                    default:
                        break;
                }
            }
            row++;
        }
        return Elements;
    }

}
