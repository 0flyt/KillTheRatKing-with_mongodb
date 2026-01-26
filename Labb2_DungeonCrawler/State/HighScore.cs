using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.State
{
    public class HighScore
    {
        public ObjectId Id { get; set; }
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public bool IsAlive { get; set; }
    }
}
