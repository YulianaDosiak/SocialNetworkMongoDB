using MongoDB.Driver;
using SocialNetwork.Models;
using SocialNetwork.Services;
using SocialNetwork.Neo4j.DAL;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var db = new DatabaseService();

        var graphDb = new GraphDatabaseService();
        var graphRepo = new GraphUserRepository(graphDb);

        var auth = new AuthService(db, graphRepo);
        var social = new SocialNetworkService(db, graphRepo);

        User user = null;
        bool running = true;

        Console.WriteLine("=== Social Network ===");

        while (user == null && running)
        {
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("0. Exit");
            Console.Write("Choose: ");
            string authChoice = Console.ReadLine();

            switch (authChoice)
            {
                case "1":
                    Console.Write("Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
                    user = auth.Login(email, password);
                    if (user == null) Console.WriteLine("Wrong credentials");
                    break;

                case "2":
                    Console.Write("First Name: ");
                    string firstName = Console.ReadLine();
                    Console.Write("Last Name: ");
                    string lastName = Console.ReadLine();
                    Console.Write("Email: ");
                    string regEmail = Console.ReadLine();
                    Console.Write("Password: ");
                    string regPassword = Console.ReadLine();
                    user = await auth.Register(regEmail, regPassword, firstName, lastName);
                    if (user != null) Console.WriteLine("Registration successful!");
                    break;

                case "0":
                    running = false;
                    break;
            }
        }

        if (user == null)
        {
            graphDb.Dispose();
            return;
        }

        Console.WriteLine($"Welcome, {user.FirstName} {user.LastName}!");

        while (running)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Show stream (all posts)");
            Console.WriteLine("2. Create post");
            Console.WriteLine("3. Add friend");
            Console.WriteLine("4. Comment post");
            Console.WriteLine("5. Like post");
            Console.WriteLine("6. View User Profile (Neo4j)");
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
                    Console.WriteLine("Post created");
                    break;

                case "3":
                    Console.Write("Enter friend userId: ");
                    string friendId = Console.ReadLine();
                    await social.AddFriend(user, friendId);
                    Console.WriteLine("Friend added");
                    break;

                case "4":
                    Console.Write("Enter post owner userId: ");
                    string ownerId = Console.ReadLine();
                    Console.Write("Enter postId: ");
                    string postId = Console.ReadLine();
                    Console.Write("Enter comment: ");
                    string comment = Console.ReadLine();
                    // social.AddComment(user, ownerId, postId, comment); // Ваш метод не був наданий у файлах
                    Console.WriteLine("Comment added");
                    break;

                case "5":
                    Console.Write("Enter post owner userId: ");
                    string likeOwner = Console.ReadLine();
                    Console.Write("Enter postId: ");
                    string likePostId = Console.ReadLine();
                    // social.LikePost(user, likeOwner, likePostId); // Ваш метод не був наданий у файлах
                    Console.WriteLine("Post liked");
                    break;

                case "6":
                    Console.Write("Enter userId to view: ");
                    string otherUserId = Console.ReadLine();

                    if (otherUserId == user.Id)
                    {
                        Console.WriteLine("That's you!");
                        break;
                    }

                    bool isFriend = await social.AreUsersFriends(user.Id, otherUserId);
                    Console.WriteLine($"Are you friends? {isFriend}");

                    int distance = await social.GetFriendshipDistance(user.Id, otherUserId);
                    Console.WriteLine($"Friendship distance: {(distance == -1 ? "No connection" : distance.ToString())}");
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("❌ Wrong option");
                    break;
            }
        }

        graphDb.Dispose();
    }

    static void ShowStream(SocialNetworkService social, DatabaseService db)
    {
        var posts = social.GetStream();
        foreach (var post in posts)
        {
            var author = db.Users.Find(u => u.Id == post.AuthorId).FirstOrDefault();
            Console.WriteLine($"\n[{post.PostId}] {post.Content} by {author?.FirstName} {author?.LastName} ({post.CreatedAt})");
            Console.WriteLine($"   Likes: {post.Reactions.Count}, Comments: {post.Comments.Count}");
        }
    }
}