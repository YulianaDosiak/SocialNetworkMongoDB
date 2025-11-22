using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.DynamoDB.DAL.Models;

namespace SocialNetwork.DynamoDB.DAL.Repositories
{
    public interface IPostRepository
    {
        Task CreatePostAsync(PostEntity post);
        Task CreateCommentAsync(CommentEntity comment);
        Task UpdatePostContentAsync(string postId, string newContent);
        Task<List<CommentEntity>> GetCommentsForPostAsync(string postId);
    }
}