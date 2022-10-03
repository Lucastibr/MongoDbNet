using MongoDB.Bson;
using MongoDB.Driver;
using src.Interfaces;

namespace src;

public class Connection : IConnection
{
    private readonly MongoClient _mongoClient;

    public Connection(string urlConnectionString)
    {
        _mongoClient = new MongoClient(urlConnectionString);
    }

    public string DatabaseName { get; set; }

    public string CollectionName { get; set; }

    /// <summary>
    /// Checks if the connection to the bank was successful
    /// </summary>
    /// <param name="dataBaseName"></param>
    /// <returns></returns>
    public bool IsConnected(string dataBaseName)
    {
        if(string.IsNullOrWhiteSpace(dataBaseName))
            throw new Exception($"Param {dataBaseName} can't be empty");

        DatabaseName = dataBaseName;

        var dbList = _mongoClient.GetDatabase(dataBaseName);

        var isConnected = dbList.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);

        return isConnected;
    }

    /// <summary>
    /// Initial method to connect to the collection you will work on
    /// </summary>
    /// <param name="dataBaseName"></param>
    /// <param name="collectionName"></param>
    /// <returns></returns>
    public IMongoCollection<BsonDocument> GetCollection(string collectionName)
    {
         if(string.IsNullOrWhiteSpace(DatabaseName))
            throw new Exception($"Param {DatabaseName} can't be empty");

        if(string.IsNullOrWhiteSpace(collectionName))
            throw new Exception($"Param {collectionName} can't be empty");

        CollectionName = collectionName;

        var dbClient = _mongoClient.GetDatabase(DatabaseName);

        var collection = dbClient.GetCollection<BsonDocument>(collectionName);

        var validate = collection.CollectionNamespace.CollectionName == collectionName;

        if(!validate)
            ErrorConnectionFromCollection();

        return collection;
    }

    /// <summary>
    /// Fetch all data in the collection
    /// </summary>
    /// <returns></returns>
    public List<BsonDocument> GetAll()
    {
        var collection = GetCollection(CollectionName);

        var result = collection.Find(new BsonDocument()).ToList();

        return result;
    }

    /// <summary>
    /// Insert a new data into the collection
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public BsonDocument Insert(BsonDocument document)
    {
        if(!CheckCollectionParam(CollectionName))
            ErrorConnectionFromCollection();

        var collection = GetCollection(CollectionName);

        collection.InsertOne(document);

        return document;
    }

    /// <summary>
    /// Search the data according to the parameter
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public BsonDocument SearchQuery(IDictionary<string, int> query)
    {
        if(!CheckCollectionParam(CollectionName))
            ErrorConnectionFromCollection();

        var queryData = GetCollection(CollectionName);

        var filter = Builders<BsonDocument>.Filter.Eq(query.Keys.ElementAt(0), query.Values.ElementAt(0));

        var search = queryData.Find(filter).FirstOrDefault();

        return search;
    }

    /// <summary>
    /// Updates data based on method parameters
    /// </summary>
    /// <param name="queryFilter"></param>
    /// <param name="dataUpdate"></param>
    /// <returns></returns>
    public BsonDocument Update(IDictionary<string, int> queryFilter, IDictionary<string, string> dataUpdate)
    {
        if(!CheckCollectionParam(CollectionName))
            ErrorConnectionFromCollection();

        var collection = GetCollection(CollectionName);

        var filter = Builders<BsonDocument>.Filter.Eq(queryFilter.Keys.ElementAt(0), queryFilter.Values.ElementAt(0));

        var update = Builders<BsonDocument>.Update.Set(dataUpdate.Keys.ElementAt(0), dataUpdate.Values.ElementAt(0));

        collection.UpdateOne(filter, update);

        return new BsonDocument
        {
            {dataUpdate.Keys.ElementAt(0), dataUpdate.Values.ElementAt(0)}
        };
    }

    /// <summary>
    /// Deletes data from the collection based on the given filter
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public DeleteResult DeleteWithFilter(Dictionary<string, int> query)
    {
        if(!CheckCollectionParam(CollectionName))
            ErrorConnectionFromCollection();

        var queryData = GetCollection(CollectionName);

        var filter = Builders<BsonDocument>.Filter.Eq(query.Keys.ElementAt(0), query.Values.ElementAt(0));

        var search = queryData.DeleteOne(filter);

        return search;
    }

    /// <summary>
    /// Search all databases
    /// </summary>
    /// <returns></returns>
    public string GetDatabases()
    {
        if(!CheckCollectionParam(CollectionName))
            ErrorConnectionFromCollection();

        var collection = _mongoClient.ListDatabaseNames().ToList();
        var dataBaseNames = collection.ToList();

        return string.Join(", ", dataBaseNames);
    }

    /// <summary>
    /// Fetch all collections from the database
    /// </summary>
    /// <param name="dataBaseName"></param>
    /// <returns></returns>
    public string GetCollectionsFromDataBase(string dataBaseName)
    {
        if(!CheckCollectionParam(CollectionName))
            ErrorConnectionFromCollection();

        var database = _mongoClient.GetDatabase(dataBaseName);

        var collections = database.ListCollectionNames().ToList();

        var collectionName = collections.ToList();

        return string.Join(",", collectionName);
    }

    /// <summary>
    /// Check if collection param is null or white space
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool CheckCollectionParam(string collection) => string.IsNullOrWhiteSpace(collection);
    
    public Exception ErrorConnectionFromCollection()
    {
        throw new Exception($"Collection not found in database {DatabaseName}");
    }
}