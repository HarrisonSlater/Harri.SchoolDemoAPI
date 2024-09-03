using FluentAssertions;
using Harri.SchoolDemoAPI.Controllers;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Services;
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
        }

        [TestCase(null)]
        [TestCase("Test Student")]
        public async Task GetStudents_ShouldReturnOk(string? name)
        {
            // Arrange
            _mockStudentService.Setup(x => x.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .ReturnsAsync(new List<StudentDto>() { new StudentDto() });

            // Act
            var result = await _controller.GetStudents(name, new GPAQueryDto { GPA = null }, null, null);

            // Assert
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public async Task GetStudents_ShouldReturnNotFound_WhenNoStudentsReturned()
        {
            // Arrange
            _mockStudentService.Setup(x => x.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .ReturnsAsync([]);

            // Act
            var result = await _controller.GetStudents("Test Student", new() { GPA = null }, null, null);

            // Assert
            result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}