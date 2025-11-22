using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SocialNetwork.DynamoDB.DAL.Repositories;
using SocialNetwork.Services;

namespace SocialNetwork
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8001"
            };

            var client = new AmazonDynamoDBClient("admin", "1234", config);

            await CreateTableIfNotExists(client);

            IPostRepository repository = new PostRepository(client);
            var service = new SocialNetworkService(repository);
            Console.WriteLine("Creating Post...");
            string postId = Guid.NewGuid().ToString();
            string authorId = "User1";
            await service.CreatePost(postId, "Hello DynamoDB", authorId);

            Console.WriteLine("Adding Comments...");
            await service.AddComment(postId, "First Comment", "User2");
            await Task.Delay(1000);
            await service.AddComment(postId, "Second Comment", "User3");

            Console.WriteLine("Reading Comments (Sorted)...");
            await service.ShowComments(postId);

            Console.WriteLine("Editing Post...");
            await service.EditPost(postId, "Hello Updated DynamoDB");

            Console.WriteLine("Done! Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task CreateTableIfNotExists(IAmazonDynamoDB client)
        {
            string tableName = "SocialNetwork";

            var tables = await client.ListTablesAsync();
            if (tables.TableNames.Contains(tableName))
            {
                return;
            }

            Console.WriteLine("Creating table...");

            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "PK", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "SK", AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = "PK", KeyType = KeyType.HASH },
                    new KeySchemaElement { AttributeName = "SK", KeyType = KeyType.RANGE }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            await client.CreateTableAsync(request);

            Console.WriteLine("Waiting for table to be active...");
            await Task.Delay(2000);
            Console.WriteLine("Table created.");
        }
    }
}