using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
