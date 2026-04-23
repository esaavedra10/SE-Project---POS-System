using MongoExample.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MongoExample.Services;

public class ProductsServices
{
    private readonly IMongoCollection<Products> _productsCollection;

    public ProductsServices(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _productsCollection = database.GetCollection<Products>("Products");
    }

    public async Task<List<Products>> GetAsync() =>
        await _productsCollection.Find(_ => true).ToListAsync();

    public async Task<Products?> GetAsync(string id) =>
        await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Products?> GetByNameAsync(string name) =>
        await _productsCollection.Find(x => x.name == name).FirstOrDefaultAsync();

    public async Task<Products?> GetBySkuAsync(string sku) =>
        await _productsCollection.Find(x => x.sku == sku).FirstOrDefaultAsync();

    public async Task CreateAsync(Products product) =>
        await _productsCollection.InsertOneAsync(product);

    public async Task UpdateAsync(string id, Products product)
    {
        product.Id = id;
        await _productsCollection.ReplaceOneAsync(x => x.Id == id, product);
    }

    public async Task RemoveAsync(string id) =>
        await _productsCollection.DeleteOneAsync(x => x.Id == id);
}