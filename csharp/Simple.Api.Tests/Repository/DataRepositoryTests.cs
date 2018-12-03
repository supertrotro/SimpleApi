using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Simple.Api.Repository;
using System.Threading.Tasks;
using Xunit;

namespace Simple.Api.Tests.Repository
{
    [Trait("Category", "IntegrationTests")]
    public class DataRepositoryTests : MongoIntegrationTest
    {
        private readonly IDataRepository _repository;
        private readonly IDbContext _context;
        internal string DatabaseName = "IntegrationTest";
        internal string CollectionName = "IntegrationTestCollection";
        public DataRepositoryTests()
        {
            CreateConnection();
            var options = Substitute.For<IOptions<Settings>>();
            options.Value.Returns(new Settings()
            {
                ConnectionString = Runner.ConnectionString,
                Collection = CollectionName,
                Database = DatabaseName
            });
            _context = new DbContext(options);
            _repository = new DataRepository(_context);
        }

        [Fact]
        public async Task Should_find_data_item_by_key()
        {
            //Arrange
            var item = new DataItem() { Key = "124", Value = "1234" };
            await _context.Items.InsertOneAsync(item);

            //Act
            var ret = _repository.GetDataAsync(item.Key);

            //Assert
            ret.Should().NotBeNull();
            ret.Result.Key.Should().Be(item.Key);
            ret.Result.Value.Should().Be(item.Value);
        }
        [Fact]
        public async Task Should_create_a_data_item_into_repository()
        {
            //Arrange
            var item = new DataItem() { Key = "124", Value = "1233" };
            //Act
            await _repository.CreateDataAsync(item.Key, item.Value);
            //Assert
            var ret = await _repository.GetDataAsync(item.Key);
            ret.Value = item.Value;
        }

        [Fact]
        public async Task Should_update_data_in_repository()
        {
            //Arrange
            var key = "123";
            var value = "123";
            var newValue = "456";
            await _repository.CreateDataAsync(key, value);
            var item = await _repository.GetDataAsync(key);
            item.Should().NotBeNull();

            //Act
            await _repository.UpdateDataAsync(key, newValue);
            var newItem = await _repository.GetDataAsync(key);
            
            //Assert
            newItem.Value.Should().Be(newValue);    


        }
        [Fact]
        public async Task Should_delete_data_in_repository()
        {
            //Arrange
            var key = "123";
            var value = "123";
            await _repository.CreateDataAsync(key, value);
            var item = await _repository.GetDataAsync(key);
            item.Should().NotBeNull();
            //Act
            await _repository.DeleteAsync(key);
            var newItem = await _repository.GetDataAsync(key);
            //Assert
            newItem.Should().BeNull();
        }
    }
}
