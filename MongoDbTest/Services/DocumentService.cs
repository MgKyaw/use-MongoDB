using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTest.Models;

namespace MongoDbTest.Services
{
    public class DocumentService
    {
        private readonly MongoClient _client;
        private Dictionary<string, List<string>> _databasesAndCollections;

        public DocumentService(MyDatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
        }

        public async Task<BsonDocument> GetDocument(
            string databaseName,
            string collectionName,
            int index
        )
        {
            var collection = GetCollection(databaseName, collectionName);
            BsonDocument document = null;
            await collection
                .Find(doc => true)
                .Skip(index)
                .Limit(1)
                .ForEachAsync(doc => document = doc);
            return document;
        }

        public async Task<long> GetCollectionCount(string databaseName, string collectionName)
        {
            var collection = GetCollection(databaseName, collectionName);
            return await collection.EstimatedDocumentCountAsync();
        }

        private IMongoCollection<BsonDocument> GetCollection(
            string databaseName,
            string collectionName
        )
        {
            var db = _client.GetDatabase(databaseName);
            return db.GetCollection<BsonDocument>(collectionName);
        }

        public async Task<Dictionary<string, List<string>>> GetDatabasesAndCollections()
        {
            if (_databasesAndCollections != null)
                return _databasesAndCollections;

            _databasesAndCollections = new Dictionary<string, List<string>>();
            var databasesResult = _client.ListDatabaseNames();

            await databasesResult.ForEachAsync(async databaseName =>
            {
                var collectionNames = new List<string>();
                var database = _client.GetDatabase(databaseName);
                var collectionNamesResult = database.ListCollectionNames();
                await collectionNamesResult.ForEachAsync(collectionName =>
                {
                    collectionNames.Add(collectionName);
                });
                _databasesAndCollections.Add(databaseName, collectionNames);
            });

            return _databasesAndCollections;
        }
    }
}
