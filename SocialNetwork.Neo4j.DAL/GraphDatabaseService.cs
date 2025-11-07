using Neo4j.Driver;
using System;

namespace SocialNetwork.Neo4j.DAL
{
    public class GraphDatabaseService : IDisposable
    {
        private readonly IDriver _driver;

        public GraphDatabaseService()
        {
            _driver = GraphDatabase.Driver(
                "bolt://localhost:7687",
                AuthTokens.Basic("neo4j", "12345678910")
            );
        }

        public IAsyncSession GetSession() => _driver.AsyncSession();

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}