using MongoDB.Bson;
using MongoDB.Driver;

namespace src.Interfaces
{
    public interface IConnection
    {
        bool IsConnected (string connection);

        IMongoCollection<BsonDocument> GetCollection(string collectionName);

        List<BsonDocument> GetAll();

        BsonDocument Insert(BsonDocument document);

        string GetDatabases();

        string GetCollectionsFromDataBase(string dataBaseName);

        BsonDocument Update(IDictionary<string, int> queryFilter, IDictionary<string, string> dataUpdate);

        BsonDocument SearchQuery(IDictionary<string, int> query);

        DeleteResult DeleteWithFilter(Dictionary<string, int> query);
    }
}