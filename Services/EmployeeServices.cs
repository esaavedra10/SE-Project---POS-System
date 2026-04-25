using MongoExample.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoExample.Services;

public class EmployeeServices
{
    private readonly IMongoCollection<Employees> _employeeCollection;

    public EmployeeServices(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _employeeCollection = database.GetCollection<Employees>("Employees");
    }

    public async Task<List<Employees>> GetAsync() =>
        await _employeeCollection.Find(_ => true).ToListAsync();

    public async Task<Employees?> GetAsync(string id) =>
        await _employeeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Employees?> GetByNameAsync(string name)
    {
        var filter = new BsonDocument("name", name);
        return await _employeeCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Employees?> GetByEmployeeIdAsync(string employeeId) =>
        await _employeeCollection.Find(x => x.EID == employeeId).FirstOrDefaultAsync();

    public async Task CreateAsync(Employees employee) =>
        await _employeeCollection.InsertOneAsync(employee);

    public async Task UpdateAsync(string id, Employees employee)
    {
        employee.Id = id;
        await _employeeCollection.ReplaceOneAsync(x => x.Id == id, employee);
    }

    public async Task RemoveAsync(string id) =>
        await _employeeCollection.DeleteOneAsync(x => x.Id == id);
}