using MongoDB.Driver;
using System.Threading.Tasks;

namespace Simple.Api.Repository
{
    public interface IDataRepository
    {
        Task<DataItem> GetDataAsync(string key);
        Task CreateDataAsync(string key, string value);
        Task<bool> UpdateDataAsync(string key, string value);
        Task<bool> Delete(string key);
    }
    public class DataRepository : IDataRepository
    {
        private readonly IDbContext _dbContext;

        public DataRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DataItem> GetDataAsync(string key)
        {
            return await _dbContext
                .Items
                .Find(_ => _.Key.Equals(key))
                .FirstOrDefaultAsync();
        }

        public async Task CreateDataAsync(string key, string value)
        {
            await _dbContext.Items.InsertOneAsync(new DataItem() { Key = key, Value = value });
        }

        public async Task<bool> UpdateDataAsync(string key, string value)
        {
            var updateResult =
                await _dbContext
                    .Items
                    .ReplaceOneAsync(
                        filter: g => g.Key == key,
                        replacement: new DataItem() { Key = key, Value = value });
            return updateResult.IsAcknowledged
                   && updateResult.ModifiedCount > 0;
        }
        public async Task<bool> Delete(string key)
        {
            var filter = Builders<DataItem>.Filter.Eq(x => x.Key, key);
            var deleteResult = await _dbContext
                                        .Items
                                        .DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged
                   && deleteResult.DeletedCount > 0;
        }
    }
}
