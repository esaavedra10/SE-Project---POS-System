using MongoExample.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MongoExample.Services;

public class MongoDBService
{
    private readonly IMongoCollection<Employees> _employeeCollection;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _employeeCollection = database.GetCollection<Employees>(mongoDBSettings.Value.CollectionName);
    }

    public async Task<List<Employees>> GetAsync()
    {
        return await _employeeCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Employees?> GetAsync(string id)
    {
        return await _employeeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Employees employee)
    {
        await _employeeCollection.InsertOneAsync(employee);
    }

    public async Task UpdateAsync(string id, Employees employee)
    {
        await _employeeCollection.ReplaceOneAsync(x => x.Id == id, employee);
    }

    public async Task RemoveAsync(string id)
    {
        await _employeeCollection.DeleteOneAsync(x => x.Id == id);
    }
}