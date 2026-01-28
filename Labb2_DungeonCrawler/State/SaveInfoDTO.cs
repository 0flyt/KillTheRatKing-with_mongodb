using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.State
{
    public class SaveInfoDTO
    {
        public ObjectId Id { get; set; }
        public string PlayerName { get; set; } = "";
        public string AktiveLevelName { get; set; }
        public int PlayerXp { get; set; } = 0;
    }
}
