using MongoDB.Driver;
using SocialNetwork.Models;
using SocialNetwork.Neo4j.DAL;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;
        private readonly GraphUserRepository _graphRepo;

        public AuthService(DatabaseService db, GraphUserRepository graphRepo)
        {
            _users = db.Users;
            _graphRepo = graphRepo;
        }

        public User Login(string email, string password)
        {
            return _users.Find(u => u.Email == email && u.Password == password).FirstOrDefault();
        }

        public async Task<User> Register(string email, string password, string firstName, string lastName)
        {
            var existingUser = _users.Find(u => u.Email == email).FirstOrDefault();
            if (existingUser != null)
            {
                Console.WriteLine("User with this email already exists.");
                return null;
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName
            };
            _users.InsertOne(user);

            await _graphRepo.CreateUserNodeAsync(user.Id);

            return user;
        }
    }
}