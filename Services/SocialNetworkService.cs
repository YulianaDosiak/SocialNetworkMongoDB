using MongoDB.Driver;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Services
{
    public class SocialNetworkService
    {
        private readonly IMongoCollection<User> _users;

        public SocialNetworkService(DatabaseService db)
        {
            _users = db.Users;
        }

        // Додати друга
        public void AddFriend(User user, string friendId)
        {
            if (!user.Friends.Contains(friendId))
            {
                user.Friends.Add(friendId);
                _users.ReplaceOne(u => u.Id == user.Id, user);
            }
        }

        // Створити пост
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

        // Додати коментар до поста іншого користувача
        public void AddComment(User author, string postOwnerId, string postId, string content)
        {
            var postOwner = _users.Find(u => u.Id == postOwnerId).FirstOrDefault();
            if (postOwner == null) return;

            var post = postOwner.Posts.FirstOrDefault(p => p.PostId == postId);
            if (post == null) return;

            var comment = new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                AuthorId = author.Id,
                Content = content
            };

            post.Comments.Add(comment);
            _users.ReplaceOne(u => u.Id == postOwner.Id, postOwner);
        }

        // Лайк на пост
        public void LikePost(User liker, string postOwnerId, string postId)
        {
            var postOwner = _users.Find(u => u.Id == postOwnerId).FirstOrDefault();
            if (postOwner == null) return;

            var post = postOwner.Posts.FirstOrDefault(p => p.PostId == postId);
            if (post == null) return;

            if (!post.Reactions.Contains(liker.Id))
            {
                post.Reactions.Add(liker.Id);
                _users.ReplaceOne(u => u.Id == postOwner.Id, postOwner);
            }
        }

        // Отримати всі пости (stream) з усіх користувачів
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
    }
}