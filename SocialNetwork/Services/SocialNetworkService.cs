using System;
using System.Threading.Tasks;
using SocialNetwork.DynamoDB.DAL.Models;
using SocialNetwork.DynamoDB.DAL.Repositories;

namespace SocialNetwork.Services
{
    public class SocialNetworkService
    {
        private readonly IPostRepository _postRepository;

        public SocialNetworkService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task CreatePost(string postId, string content, string authorId)
        {
            var post = new PostEntity
            {
                PostId = postId,
                Content = content,
                AuthorId = authorId,
                CreatedDateTime = DateTime.UtcNow.ToString("s")
            };

            await _postRepository.CreatePostAsync(post);
            Console.WriteLine($"Post created: {post.PostId}");
        }

        public async Task AddComment(string postId, string content, string authorId)
        {
            var comment = new CommentEntity
            {
                CommentId = Guid.NewGuid().ToString(),
                PostId = postId,
                Content = content,
                AuthorId = authorId,
                ModifiedDateTime = DateTime.UtcNow.ToString("s")
            };

            await _postRepository.CreateCommentAsync(comment);
            Console.WriteLine("Comment added.");
        }

        public async Task EditPost(string postId, string newContent)
        {
            await _postRepository.UpdatePostContentAsync(postId, newContent);
            Console.WriteLine("Post updated.");
        }

        public async Task ShowComments(string postId)
        {
            var comments = await _postRepository.GetCommentsForPostAsync(postId);
            Console.WriteLine($"Comments for Post {postId}:");
            foreach (var c in comments)
            {
                Console.WriteLine($"[{c.ModifiedDateTime}] {c.AuthorId}: {c.Content}");
            }
        }
    }
}