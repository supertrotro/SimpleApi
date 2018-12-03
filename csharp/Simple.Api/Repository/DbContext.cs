using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Simple.Api.Repository
{
    public interface IDbContext
    {
        IMongoCollection<DataItem> Items { get; }
    }

    public class DbContext:IDbContext
    {
        private readonly IMongoDatabase _db;
        private readonly IOptions<Settings> _options;
        public DbContext(IOptions<Settings> options)
        {
            _options = options;
            var client = new MongoClient(options.Value.ConnectionString);
            _db = client.GetDatabase(options.Value.Database);
            var tmp = _db.GetCollection<DataItem>(_options.Value.Collection);
            if (tmp == null)
            {
                _db.CreateCollection(_options.Value.Collection);
            }
        }
        public IMongoCollection<DataItem> Items => _db.GetCollection<DataItem>(_options.Value.Collection);
    }
}
