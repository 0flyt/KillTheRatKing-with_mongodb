using Labb2_DungeonCrawler.Log;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.State
{
    public class GameState
    {

        public ObjectId Id { get; set; }
        public MessageLog MessageLog { get; set; }
        public List<LevelElement> CurrentState { get; set; }  
        public GameState()
        {
            MessageLog = new MessageLog();
            CurrentState = new List<LevelElement>();
        }
    }
}
