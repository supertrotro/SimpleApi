using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Simple.Api.Controllers;
using Simple.Api.Repository;
using System;
using Xunit;

namespace Simple.Api.Tests.UnitTests
{
    public class DataControllerTests
    {
        private readonly IDataRepository _repository;
        private DataController _controller;

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
        public void Should_return_bad_request_exception_when_key_value_is_null_or_empty()
        {
            //Act
            var ret = _controller.GetData("");
            //Assert
            ret.Result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void Should_return_not_found_exception_when_value_is_not_exist_in_db()
        {
            //Arrange
            var key = "123";
            _repository.GetData(key).Returns(string.Empty);
            //Act
            var ret = _controller.GetData(key);
            //Assert
            ret.Result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public void Should_return_value_when_it_is_in_repository()
        {
            //Arrange
            var key = "example_key";
            var value = "example_value";
            _repository.GetData(key).Returns(value);
            //Act
            var ret = _controller.GetData(key);
            //Assert
            ret.Value.Should().Be(value);
        }

        [Fact]
        public void Should_show_an_internal_error_for_any_exception_in_procesing_request()
        {
            //Arrange
            var key = "^$$&$&";
            _repository.GetData(key).Returns(x => throw new Exception("An exeption"));
            //Act
            var ret = _controller.GetData(key);
            //Assert
            ret.Result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)ret.Result).StatusCode.Should().Be(500);
        }
    }
}
