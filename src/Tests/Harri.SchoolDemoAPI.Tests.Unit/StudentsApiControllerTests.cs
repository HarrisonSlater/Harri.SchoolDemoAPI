using FluentAssertions;
using Harri.SchoolDemoAPI.Controllers;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Harri.SchoolDemoAPI.Tests.Unit
{
    [TestFixture]
    public class StudentsApiControllerTests
    {
        private StudentsApiController _controller;
        private Mock<IStudentService> _mockStudentService;

        [SetUp]
        public void Setup()
        {
            _mockStudentService = new Mock<IStudentService>();
            _controller = new StudentsApiController(_mockStudentService.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [TestCase(null)]
        [TestCase("Test Student")]
        public async Task GetStudents_ShouldReturnOk(string? name)
        {
            // Arrange
            _mockStudentService.Setup(x => x.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .ReturnsAsync(new PagedList<StudentDto>() { Items = [new StudentDto()], Page = 1, PageSize = 10, TotalCount = 1000 });

            // Act
            var result = await _controller.GetStudents(null, name, new GPAQueryDto { GPA = null });

            // Assert
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public async Task GetStudents_ShouldReturnNoContent_WhenNoStudentsReturned()
        {
            // Arrange
            _mockStudentService.Setup(x => x.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .ReturnsAsync(new PagedList<StudentDto>() { Items = [] , Page = 1, PageSize = 10, TotalCount = 1000 });

            // Act
            var result = await _controller.GetStudents(null, "Test Student", new() { GPA = null });

            // Assert
            result.Should().BeOfType(typeof(NoContentResult));
        }

        [Test]
        public async Task GetStudents_ShouldReturnBadRequest_WhenPageIsOutOfBounds()
        {
            // Arrange
            _mockStudentService.Setup(x => x.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .ReturnsAsync(new PagedList<StudentDto>() { Items = [], PageSize = 10, TotalCount = 1000 });

            // Act
            var result = await _controller.GetStudents(null, "Test Student", new() { GPA = null }, page: 101);

            // Assert
            result.Should().BeOfType(typeof(BadRequestResult));
        }
    }
}