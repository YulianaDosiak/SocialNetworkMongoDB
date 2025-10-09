using MongoDB.Driver;
using SocialNetwork.Models;
using SocialNetwork.Services;
using System;
using System.Linq;

class Program
{
    static void Main()
    {
        var db = new DatabaseService();
        var auth = new AuthService(db);
        var social = new SocialNetworkService(db);

        Console.WriteLine("=== Social Network ===");

        // Авторизація
        Console.Write("Email: ");
        string email = Console.ReadLine();
        Console.Write("Password: ");
        string password = Console.ReadLine();

        var user = auth.Login(email, password);
        if (user == null)
        {
            Console.WriteLine("❌ Wrong credentials");
            return;
        }

        Console.WriteLine($"✅ Welcome, {user.FirstName} {user.LastName}!");

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Show stream (all posts)");
            Console.WriteLine("2. Create post");
            Console.WriteLine("3. Add friend");
            Console.WriteLine("4. Comment post");
            Console.WriteLine("5. Like post");
            Console.WriteLine("0. Exit");
            Console.Write("Choose: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ShowStream(social, db);
                    break;

                case "2":
                    Console.Write("Enter post content: ");
                    string content = Console.ReadLine();
                    social.CreatePost(user, content);
                    Console.WriteLine("✅ Post created");
                    break;

                case "3":
                    Console.Write("Enter friend userId: ");
                    string friendId = Console.ReadLine();
                    social.AddFriend(user, friendId);
                    Console.WriteLine("✅ Friend added");
                    break;

                case "4":
                    Console.Write("Enter post owner userId: ");
                    string ownerId = Console.ReadLine();
                    Console.Write("Enter postId: ");
                    string postId = Console.ReadLine();
                    Console.Write("Enter comment: ");
                    string comment = Console.ReadLine();
                    social.AddComment(user, ownerId, postId, comment);
                    Console.WriteLine("✅ Comment added");
                    break;

                case "5":
                    Console.Write("Enter post owner userId: ");
                    string likeOwner = Console.ReadLine();
                    Console.Write("Enter postId: ");
                    string likePostId = Console.ReadLine();
                    social.LikePost(user, likeOwner, likePostId);
                    Console.WriteLine("✅ Post liked");
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("❌ Wrong option");
                    break;
            }
        }
    }

    static void ShowStream(SocialNetworkService social, DatabaseService db)
    {
        var posts = social.GetStream();
        foreach (var post in posts)
        {
            // Знаходимо автора поста через AuthorId
            var author = db.Users.Find(u => u.Id == post.AuthorId).FirstOrDefault();

            Console.WriteLine($"\n[{post.PostId}] {post.Content} by {author?.FirstName} {author?.LastName} ({post.CreatedAt})");
            Console.WriteLine($"   Likes: {post.Reactions.Count}, Comments: {post.Comments.Count}");

            // Вивід коментарів з іменами авторів
            foreach (var c in post.Comments)
            {
                var commentAuthor = db.Users.Find(u => u.Id == c.AuthorId).FirstOrDefault();
                Console.WriteLine($"      - {commentAuthor?.FirstName} {commentAuthor?.LastName}: {c.Content} ({c.CreatedAt})");
            }
        }
    }
}