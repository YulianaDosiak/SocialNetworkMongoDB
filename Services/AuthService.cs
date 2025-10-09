using MongoDB.Driver;
using SocialNetwork.Models;
using System.Linq;

namespace SocialNetwork.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;

        public AuthService(DatabaseService db)
        {
            _users = db.Users;
        }

        public User Login(string email, string password)
        {
            return _users.Find(u => u.Email == email && u.Password == password).FirstOrDefault();
        }
    }
}