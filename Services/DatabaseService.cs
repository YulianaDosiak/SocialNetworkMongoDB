using MongoDB.Driver;
using SocialNetwork.Models;

namespace SocialNetwork.Services
{
    public class DatabaseService
    {
        private readonly IMongoDatabase _database;

        public DatabaseService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("social_network");
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}
