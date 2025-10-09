using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SocialNetwork.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<string> Interests { get; set; } = new List<string>();
        public List<string> Friends { get; set; } = new List<string>();
        public List<Post> Posts { get; set; } = new List<Post>();
    }

    public class Post
    {
        [BsonElement("PostId")]
        public string PostId { get; set; }

        public string Content { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<string> Reactions { get; set; } = new List<string>();

        // Додаткові поля для роботи програми
        [BsonIgnore]
        public string AuthorId { get; set; }

        [BsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Comment
    {
        [BsonElement("CommentId")]
        public string CommentId { get; set; }

        [BsonElement("AuthorId")]
        public string AuthorId { get; set; }

        public string Content { get; set; }
        public List<string> Reactions { get; set; } = new List<string>();

        // Додаткові поля для роботи програми
        [BsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}