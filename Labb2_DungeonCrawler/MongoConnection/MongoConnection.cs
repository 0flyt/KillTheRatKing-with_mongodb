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
        readonly static string classCollectionName = "classes";
        readonly static string highScoreCollectionName = "highscore";
        private static IMongoCollection<GameState> saveCollection;
        private static IMongoCollection<ClassModel> classCollection;
        private static IMongoCollection<HighScore> highScoreCollection;

        static void ConnectToDB()
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(dataBaseName);
            saveCollection = db.GetCollection<GameState>(saveCollectionName);
            classCollection = db.GetCollection<ClassModel>(classCollectionName);
            highScoreCollection = db.GetCollection<HighScore>(highScoreCollectionName);
        }
        public static async Task SaveHighScore( string playerName, int score)
        {
            ConnectToDB();
            var scoreModel = new HighScore { PlayerName = playerName, Score = score, IsAlive = false };
            await highScoreCollection.InsertOneAsync(scoreModel);

        }

        public static async Task<List<HighScore>> GetHighScoreFromDB()
        {
            ConnectToDB();

            return await highScoreCollection
                .Find(Builders<HighScore>.Filter.Empty)
                .SortByDescending(s => s.Score)
                .Limit(3)
                .Project(g => new HighScore
                {
                    PlayerName = g.PlayerName,
                    Score = g.Score
                })
                .ToListAsync();
        }

        public static async Task AddClassToCollection(string newClass)
        {
            ConnectToDB();
            var classModel = new ClassModel {ClassName = newClass};
            await classCollection.InsertOneAsync(classModel);
        }
        public static async Task SaveGameToDB(GameState gameState)
        {
            ConnectToDB();
            var gameStateFilter = Builders<GameState>.Filter.Eq(g => g.Id, gameState.Id);
            if(gameState.Id == ObjectId.Empty || gameState.Id == default)
            {
                await saveCollection.InsertOneAsync(gameState);
            }
            else await saveCollection.ReplaceOneAsync(gameStateFilter ,gameState);
        }

        public static async Task<GameState?> LoadGameFromDB(ObjectId id)
        {
            ConnectToDB();
            var filter = Builders<GameState>.Filter.Eq(g => g.Id, id);
            return await saveCollection.Find(filter).FirstOrDefaultAsync();         
        }

        public static async Task DeleteSaveFromDB(ObjectId id)
        {
            ConnectToDB();
            var filter = Builders<GameState>.Filter.Eq(g => g.Id, id);
            await saveCollection.DeleteOneAsync(filter);

        }
        public static async Task<List<SaveInfoDTO>> GetActiveSavesFromDB()
        {
            ConnectToDB();
            return await saveCollection
                            .Find(Builders<GameState>.Filter.Empty)
                            .Project(g => new SaveInfoDTO
                            {
                                Id = g.Id,
                                PlayerName = g.PlayerName,
                                PlayerXp = g.XpScore
                            })
                            .ToListAsync();
        }
        public static async Task<List<string>> GetClassesFromDB()
        {
            ConnectToDB();
            return await classCollection
                            .Find(Builders<ClassModel>.Filter.Empty)
                            .Project(c => c.ClassName)
                            .ToListAsync();
        }
        public static async Task<ObjectId> GetClassId(string className)
        {
            ConnectToDB();
            return await classCollection
                            .Find(Builders<ClassModel>
                            .Filter.Eq(c => c.ClassName, className))
                            .Project(c => c.Id)
                            .FirstOrDefaultAsync();
        }
    }
}
