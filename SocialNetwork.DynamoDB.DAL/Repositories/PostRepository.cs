using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SocialNetwork.DynamoDB.DAL.Models;

namespace SocialNetwork.DynamoDB.DAL.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IAmazonDynamoDB _client;
        private readonly string _tableName = "SocialNetwork";

        public PostRepository(IAmazonDynamoDB client)
        {
            _client = client;
        }

        public async Task CreatePostAsync(PostEntity post)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "PK", new AttributeValue { S = $"POST#{post.PostId}" } },
                    { "SK", new AttributeValue { S = "METADATA" } },
                    { "Content", new AttributeValue { S = post.Content } },
                    { "AuthorId", new AttributeValue { S = post.AuthorId } },
                    { "CreatedDateTime", new AttributeValue { S = post.CreatedDateTime } }
                }
            };
            await _client.PutItemAsync(request);
        }

        public async Task CreateCommentAsync(CommentEntity comment)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "PK", new AttributeValue { S = $"POST#{comment.PostId}" } },
                    { "SK", new AttributeValue { S = $"COMMENT#{comment.ModifiedDateTime}#{comment.CommentId}" } },
                    { "Content", new AttributeValue { S = comment.Content } },
                    { "AuthorId", new AttributeValue { S = comment.AuthorId } },
                    { "ModifiedDateTime", new AttributeValue { S = comment.ModifiedDateTime } },
                    { "CommentId", new AttributeValue { S = comment.CommentId } }
                }
            };
            await _client.PutItemAsync(request);
        }

        public async Task UpdatePostContentAsync(string postId, string newContent)
        {
            var request = new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "PK", new AttributeValue { S = $"POST#{postId}" } },
                    { "SK", new AttributeValue { S = "METADATA" } }
                },
                UpdateExpression = "SET Content = :c",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":c", new AttributeValue { S = newContent } }
                }
            };
            await _client.UpdateItemAsync(request);
        }

        public async Task<List<CommentEntity>> GetCommentsForPostAsync(string postId)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "PK = :pk AND begins_with(SK, :sk)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":pk", new AttributeValue { S = $"POST#{postId}" } },
                    { ":sk", new AttributeValue { S = "COMMENT#" } }
                },
                ScanIndexForward = true
            };

            var response = await _client.QueryAsync(request);
            var comments = new List<CommentEntity>();

            foreach (var item in response.Items)
            {
                comments.Add(new CommentEntity
                {
                    PostId = postId,
                    CommentId = item.ContainsKey("CommentId") ? item["CommentId"].S : null,
                    Content = item.ContainsKey("Content") ? item["Content"].S : null,
                    AuthorId = item.ContainsKey("AuthorId") ? item["AuthorId"].S : null,
                    ModifiedDateTime = item.ContainsKey("ModifiedDateTime") ? item["ModifiedDateTime"].S : null
                });
            }

            return comments;
        }
    }
}