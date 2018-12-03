using MongoDB.Driver;
using System.Threading.Tasks;

namespace Simple.Api.Repository
{
    public interface IDataRepository
    {
        Task<DataItem> GetDataAsync(string key);
        Task CreateDataAsync(string key, string value);
        Task<bool> UpdateDataAsync(string key, string value);
        Task<bool> DeleteAsync(string key);
    }
    public class DataRepository : IDataRepository
    {
        private readonly IMongoCollection<DataItem> _items;

        public DataRepository(IDbContext dbContext)
        {
            _items = dbContext.Items;
        }

        public async Task<DataItem> GetDataAsync(string key)
        {
            var ret = await _items.FindAsync(_ => _.Key.Equals(key));
            return ret.FirstOrDefault();
        }

        public async Task CreateDataAsync(string key, string value)
        {
            await _items.InsertOneAsync(new DataItem() { Key = key, Value = value });
        }

        public async Task<bool> UpdateDataAsync(string key, string value)
        {
            var oldItem = await GetDataAsync(key);
            if (oldItem == null)
                return false;
            var updateResult =
                await _items
                    .ReplaceOneAsync(
                        filter: g => g.Key == key,
                        replacement: new DataItem() { Key = key, Value = value, Id = oldItem.Id});
            return updateResult.IsAcknowledged
                   && updateResult.ModifiedCount > 0;
        }
        public async Task<bool> DeleteAsync(string key)
        {
            var filter = Builders<DataItem>.Filter.Eq(x => x.Key, key);
            var deleteResult = await _items.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged
                   && deleteResult.DeletedCount > 0;
        }
    }
}
