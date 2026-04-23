using MongoExample.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoExample.Services;

public class EmployeesServices
{
    private readonly IMongoCollection<Employees> _employeeCollection;

    public EmployeesServices(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _employeeCollection = database.GetCollection<Employees>(mongoDBSettings.Value.CollectionName);
    }

    public async Task<List<Employees>> GetAsync() =>
        await _employeeCollection.Find(_ => true).ToListAsync();

    public async Task<Employees?> GetAsync(string id) =>
        await _employeeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Employees?> GetByNameAsync(string name)
    {
        var filter = Builders<Employees>.Filter.Regex(
            "name",
            new BsonRegularExpression($"^{System.Text.RegularExpressions.Regex.Escape(name)}$", "i")
        );

        return await _employeeCollection.Find(filter).FirstOrDefaultAsync();
    }

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