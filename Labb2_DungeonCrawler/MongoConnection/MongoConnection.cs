using Labb2_DungeonCrawler.State;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.MongoConnection
{
    public static class MongoConnection
    {
        readonly static string connectionString = "mongodb://localhost:27017/";
        readonly static string dataBaseName = "KillTheRatKing";
        readonly static string saveCollectionName = "saves";
        private static IMongoCollection<GameState> collection;
        static void ConnectToDB()
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(dataBaseName);
            collection = db.GetCollection<GameState>(saveCollectionName);
        }
        public static async void SaveGameToDB(GameState gameState)
        {
            ConnectToDB();
            var gameStateFilter = Builders<GameState>.Filter.Eq(g => g.Id, gameState.Id);
            if(gameState.Id == null || gameState.Id == default)
            {
                await collection.InsertOneAsync(gameState);
            }
            else await collection.ReplaceOneAsync(gameStateFilter ,gameState);
            //om filtert "returnerar" en save så replace annars insert, den här ska bara triggas när man gjort en Load
        }

        public static async Task<GameState?> LoadGameFromDB(ObjectId id)
        {
            ConnectToDB();
            var filter = Builders<GameState>.Filter.Eq(g => g.Id, id);
            return await collection.Find(filter).FirstOrDefaultAsync();         
        }

    }
}
