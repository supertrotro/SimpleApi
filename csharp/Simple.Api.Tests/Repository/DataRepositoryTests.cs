using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using Simple.Api.Repository;
using System.Threading.Tasks;
using Xunit;

namespace Simple.Api.Tests.Repository
{
    public class DataRepositoryTests
    {
        private IDbContext _dbContext;
        private IDataRepository _repository;

        public DataRepositoryTests()
        {
            _dbContext = NSubstitute.Substitute.For<IDbContext>();
            _repository = new DataRepository(dbContext: _dbContext);
        }

        [Fact]
        public void Should_find_data_item_by_key()
        {
            //Arrange
            var cursorMock = Substitute.For<IAsyncCursor<string>>();
            cursorMock.MoveNextAsync().Returns(Task.FromResult(true), Task.FromResult(false));
            cursorMock.Current.Returns(new[] { "asd" });

            var ff = Substitute.For<IFindFluent<string, string>>();
            ff.ToCursorAsync().Returns(Task.FromResult(cursorMock));
            ff.Limit(1).Returns(ff);

            var result = ff.FirstOrDefaultAsync().Result;
            Assert.AreEqual("asd", result);
            var item = new DataItem() { Key = "124", Value = "1234" };
            var mock = NSubstitute.Substitute.For<MongoCollectionBase<DataItem>>();
            mock.Find(x => x.Key.Equals(item.Key)).FirstOrDefaultAsync().Returns(Task.FromResult(item));
            _dbContext.Items.Returns(mock);

            //Act
            var ret = _repository.GetDataAsync(item.Key);

            //Assert
            ret.Should().NotBeNull();
            ret.Result.Key.Should().Be(item.Key);
            ret.Result.Value.Should().Be(item.Value);

        }
    }
}
