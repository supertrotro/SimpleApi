using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Simple.Api.Controllers;
using Simple.Api.Repository;
using Xunit;

namespace Simple.Api.Tests.Controller
{
    public class DataControllerTests
    {
        private readonly IDataRepository _repository;
        private readonly DataController _controller;
        private string example_key = "example_key";
        private string example_value = "example_value";

        public DataControllerTests()
        {
            _repository = NSubstitute.Substitute.For<IDataRepository>();
            _controller = new DataController(_repository, NSubstitute.Substitute.For<ILogger<DataController>>());
        }

        [Fact]
        public void Should_show_healthy_message()
        {
            //Act
            var ret = _controller.Index();
            //Asser
            ret.Should().NotBeNullOrEmpty();
            ret.Should().Be(DataController.HealthyMessage);
        }

        [Fact]
        public async Task Should_return_bad_request_exception_when_key_value_is_null_or_empty()
        {
            //Act
            var ret =await _controller.GetDataAsync("");
            //Assert
            ret.Result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Should_return_not_found_exception_when_value_is_not_exist_in_db()
        {
            //Arrange
            var key = "123";
            //Act
            var ret = await _controller.GetDataAsync(key);
            //Assert
            ret.Result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task Should_return_value_when_it_is_in_repository()
        {
            //Arrange

            _repository.GetDataAsync(example_key).Returns(Task.FromResult(new DataItem(){
                Key = example_key,
                Value = example_value}));
            //Act
            var ret = await _controller.GetDataAsync(example_key);
            //Assert
            ret.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult) ret.Result).Value.Should().Be(example_value);
        }

        [Fact]
        public async Task Should_show_an_internal_error_for_any_exception_in_getting_value_from_repository()
        {
            //Arrange
            var key = "^$$&$&";
             _repository.GetDataAsync(key).Returns(async x => throw new Exception("An exeption"));
            //Act
            var ret = await _controller.GetDataAsync(key);
            //Assert
            ret.Result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)ret.Result).StatusCode.Should().Be(500);
        }

        [Fact]
        public async  Task Should_show_a_bad_request_exception_for_a_request_with_an_empty_key()
        {
            //Act
            var ret = await _controller.PostDataAsync(string.Empty, "");
            //Arrange

            ret.Should().BeOfType<BadRequestResult>();

        }
        [Fact]
        public async Task Should_show_an_internal_exception_for_any_exception_in_saving_data_into_repository()
        {
            //Arrange
            _repository.CreateDataAsync(example_key, example_value).Returns(async x => throw new Exception("Something wrong"));
            //Act
           var ret = await _controller.PostDataAsync(example_key, example_value);
            
            //Assert
            ret.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult) ret).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task Should_show_sucess_message_when_value_is_saved_into_repository()
        {
            //Arrange
            _repository.CreateDataAsync(example_key, example_value).Returns(Task.FromResult(true));
            //Act
            var ret = await _controller.PostDataAsync(example_key, example_value);
            //Assert
            ret.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task Should_show_conflict_exception_when_posting_an_existing_data()
        {
            //Arrange
            _repository.GetDataAsync(example_key).Returns(Task.FromResult(new DataItem()
            {
                Key = example_key,
                Value = "Something else"
            }));
            //Act
            var ret = await _controller.PostDataAsync(example_key, example_value);
            //Assert
            ret.Should().BeOfType<ConflictObjectResult>();
        }
    }
}
