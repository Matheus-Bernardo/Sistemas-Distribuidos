namespace GerenciadorDeProdutos.Repositories;
using MongoDB.Driver;
public class MongoRepository<T>:IMongoRepository<T>
{
    private readonly IMongoCollection<T> _collection;
    public MongoRepository(IMongoDatabase db, string collectionName)
    {
        _collection = db.GetCollection<T>(collectionName);
    }
    public async Task<List<T>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
    public async Task<T?> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task CreateAsync(T entity) => await _collection.InsertOneAsync(entity);
    public async Task UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
        await _collection.ReplaceOneAsync(filter, entity);
    }
    public async Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
        await _collection.DeleteOneAsync(filter);
    }
}