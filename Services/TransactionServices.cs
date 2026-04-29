using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoExample.Models;

namespace MongoExample.Services;

public class TransactionServices
{
    private readonly IMongoCollection<Transactions> _transactionsCollection;

    public TransactionServices(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _transactionsCollection = database.GetCollection<Transactions>("Transactions");
    }

    public async Task<List<Transactions>> GetAsync() =>
        await _transactionsCollection.Find(_ => true).ToListAsync();

    public async Task<Transactions?> GetAsync(string id) =>
        await _transactionsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Transactions?> GetByTransactionNumberAsync(string transactionNumber) =>
        await _transactionsCollection.Find(x => x.transactionNumber == transactionNumber).FirstOrDefaultAsync();

    public async Task<List<Transactions>> GetByDateRangeAsync(DateTime startInclusive, DateTime endExclusive) =>
        await _transactionsCollection
            .Find(x => x.createdAt >= startInclusive && x.createdAt < endExclusive)
            .SortBy(x => x.createdAt)
            .ToListAsync();

    public async Task CreateAsync(Transactions transaction) =>
        await _transactionsCollection.InsertOneAsync(transaction);

    public async Task UpdateAsync(string id, Transactions transaction)
    {
        transaction.Id = id;
        await _transactionsCollection.ReplaceOneAsync(x => x.Id == id, transaction);
    }

    public async Task RemoveAsync(string id) =>
        await _transactionsCollection.DeleteOneAsync(x => x.Id == id);
}