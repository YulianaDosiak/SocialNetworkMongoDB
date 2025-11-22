namespace SocialNetwork.DynamoDB.DAL.Models
{
    public class CommentEntity
    {
        public string CommentId { get; set; }
        public string PostId { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public string ModifiedDateTime { get; set; }
    }
}