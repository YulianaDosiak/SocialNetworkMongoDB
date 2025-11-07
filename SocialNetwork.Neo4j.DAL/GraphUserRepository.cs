using Neo4j.Driver;
using System.Threading.Tasks;

namespace SocialNetwork.Neo4j.DAL
{
    public class GraphUserRepository
    {
        private readonly GraphDatabaseService _dbService;

        public GraphUserRepository(GraphDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task CreateUserNodeAsync(string mongoUserId)
        {
            IAsyncSession session = _dbService.GetSession();
            try
            {
                var query = @"
                    MERGE (u:User { mongoId: $mongoUserId })
                    RETURN u";
                await session.RunAsync(query, new { mongoUserId });
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        public async Task DeleteUserNodeAsync(string mongoUserId)
        {
            IAsyncSession session = _dbService.GetSession();
            try
            {
                var query = @"
                    MATCH (u:User { mongoId: $mongoUserId })
                    DETACH DELETE u";
                await session.RunAsync(query, new { mongoUserId });
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        public async Task CreateFriendshipAsync(string userId1, string userId2)
        {
            IAsyncSession session = _dbService.GetSession();
            try
            {
                var query = @"
                    MATCH (u1:User { mongoId: $userId1 })
                    MATCH (u2:User { mongoId: $userId2 })
                    MERGE (u1)-[:FRIENDS_WITH]-(u2)";
                await session.RunAsync(query, new { userId1, userId2 });
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        public async Task DeleteFriendshipAsync(string userId1, string userId2)
        {
            IAsyncSession session = _dbService.GetSession();
            try
            {
                var query = @"
                    MATCH (u1:User { mongoId: $userId1 })-[r:FRIENDS_WITH]-(u2:User { mongoId: $userId2 })
                    DELETE r";
                await session.RunAsync(query, new { userId1, userId2 });
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        public async Task<bool> AreFriendsAsync(string currentUserId, string otherUserId)
        {
            IAsyncSession session = _dbService.GetSession();
            try
            {
                var query = @"
                    RETURN EXISTS(
                        ( (u1:User { mongoId: $currentUserId })-[:FRIENDS_WITH]-(u2:User { mongoId: $otherUserId }) )
                    )";
                var cursor = await session.RunAsync(query, new { currentUserId, otherUserId });
                var result = await cursor.SingleAsync();
                return result[0].As<bool>();
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        public async Task<int> GetFriendshipDistanceAsync(string currentUserId, string otherUserId)
        {
            if (currentUserId == otherUserId) return 0;

            IAsyncSession session = _dbService.GetSession();
            try
            {
                var query = @"
                    MATCH p = shortestPath(
                        (u1:User { mongoId: $currentUserId })-[:FRIENDS_WITH*]-(u2:User { mongoId: $otherUserId })
                    )
                    RETURN length(p) as distance";

                var cursor = await session.RunAsync(query, new { currentUserId, otherUserId });

                if (await cursor.FetchAsync())
                {
                    return cursor.Current["distance"].As<int>();
                }

                return -1;
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }
    }
}