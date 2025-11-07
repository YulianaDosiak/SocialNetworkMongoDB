using MongoDB.Driver;
using SocialNetwork.Models;
using SocialNetwork.Neo4j.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Services
{
    public class SocialNetworkService
    {
        private readonly IMongoCollection<User> _users;
        private readonly GraphUserRepository _graphRepo;

        public SocialNetworkService(DatabaseService db, GraphUserRepository graphRepo)
        {
            _users = db.Users;
            _graphRepo = graphRepo;
        }

        public async Task AddFriend(User user, string friendId)
        {
            if (!user.Friends.Contains(friendId))
            {
                user.Friends.Add(friendId);
                _users.ReplaceOne(u => u.Id == user.Id, user);

                await _graphRepo.CreateFriendshipAsync(user.Id, friendId);
            }
        }

        public void CreatePost(User user, string content)
        {
            var post = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                Content = content
            };

            user.Posts.Add(post);
            _users.ReplaceOne(u => u.Id == user.Id, user);
        }

        public List<Post> GetStream()
        {
            var users = _users.Find(_ => true).ToList();

            var allPosts = new List<Post>();
            foreach (var user in users)
            {
                foreach (var post in user.Posts)
                {
                    post.AuthorId = user.Id;
                    allPosts.Add(post);
                }
            }

            return allPosts.OrderByDescending(p => p.CreatedAt).ToList();
        }

        public async Task<bool> AreUsersFriends(string currentUserId, string otherUserId)
        {
            return await _graphRepo.AreFriendsAsync(currentUserId, otherUserId);
        }

        public async Task<int> GetFriendshipDistance(string currentUserId, string otherUserId)
        {
            return await _graphRepo.GetFriendshipDistanceAsync(currentUserId, otherUserId);
        }
    }
}