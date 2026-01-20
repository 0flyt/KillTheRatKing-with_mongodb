using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.State
{
    internal class GameState
    {
        public ObjectId Id { get; set; }
        public List<LevelElement> CurrentState { get; set; }  
    }
}
