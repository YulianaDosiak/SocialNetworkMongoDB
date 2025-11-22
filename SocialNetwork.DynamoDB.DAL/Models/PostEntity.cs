namespace SocialNetwork.DynamoDB.DAL.Models
{
    public class PostEntity
    {
        public string PostId { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public string CreatedDateTime { get; set; }
    }
}