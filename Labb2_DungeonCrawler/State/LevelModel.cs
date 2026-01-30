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
        public int AliveEnemies { get; set; }
        public int DeadEnemies { get; set; }
        public List<LevelElement> Elements { get; set; }
        public bool IsAccessable { get; set; }
        public static List<LevelModel> LoadLevels(List<string> paths)
        {
            var result = new List<LevelModel>();
            for (int i = 0; i < paths.Count; i++)
            {
                var level = new LevelModel() { Elements = LevelData.Load(paths[i]), Name = $"Level {i + 1}" };
                if (i == 0) level.IsAccessable = true;
                result.Add(level);
            }
            return result;
        }
    }

}
