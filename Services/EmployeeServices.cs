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

    /// <summary>
    /// Validates credentials and ensures the employee is a manager (PID starts with "M", same rule as AuthController).
    /// </summary>
    public async Task<Employees?> ValidateManagerCredentialsAsync(string employeeId, string password)
    {
        var employee = await GetByEmployeeIdAsync(employeeId);
        if (employee == null || employee.password != password)
        {
            return null;
        }

        var pid = employee.PID?.Trim();
        if (string.IsNullOrEmpty(pid))
        {
            return null;
        }

        if (!pid.ToUpperInvariant().StartsWith("M"))
        {
            return null;
        }

        return employee;
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