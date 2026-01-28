using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.State
{
    public class LevelModel
    {
        public string Name { get; set; }
        public int InitialEnemies { get; set; }
        public int DeadEnemies { get; set; }
        public List<LevelElement> Elements { get; set; }
        public bool IsAccessable { get; set; }


        public void LoadLevelFromFile(string path)
        {
            int row = 4;
            Elements = new List<LevelElement>();
            foreach (var line in File.ReadAllLines(path))
            {
                for (int i = 0; i < line.Length; i++)
                {
                    switch (line[i])
                    {
                        case '#':
                            Elements.Add(new Wall() { yCordinate = row, xCordinate = i });
                            break;
                        case '@':
                            Elements.Add(new Player() { yCordinate = row, xCordinate = i });
                            break;
                        case 'r':
                            Elements.Add(new Rat() { yCordinate = row, xCordinate = i });
                            InitialEnemies++;
                            break;
                        case 's':
                            Elements.Add(new Snake() { yCordinate = row, xCordinate = i });
                            InitialEnemies++;
                            break;
                        case 'R':
                            Elements.Add(new TheRatKing() { yCordinate = row, xCordinate = i });
                            InitialEnemies++;
                            break;
                        default:
                            break;
                    }
                }
                row++;
            }
        }
    }
}

