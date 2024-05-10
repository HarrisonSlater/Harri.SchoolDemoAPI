using FluentAssertions;
using Harri.SchoolDemoAPI.Controllers;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Harri.SchoolDemoAPI.Tests.Unit
{
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

        private static IEnumerable<TestCaseData> GetQueryStudentsTestCases()
        {
            foreach (var name in new string?[] { "Test Student", null })
            {
                yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() {  Gt = 4 } }, typeof(OkObjectResult));
                yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() {  Lt = 4 } }, typeof(OkObjectResult));
                yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() {  Gt = 4, Lt = 4 } }, typeof(OkObjectResult));
                yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() { Eq = 4 } }, typeof(OkObjectResult));

                yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() { Eq = 4, Gt = 4 } }, typeof(BadRequestResult));
                yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() { Eq = 4, Lt = 4 } }, typeof(BadRequestResult));
                yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() { Eq = 4, Gt = 4, Lt = 4 } }, typeof(BadRequestResult));

                // IsNull invalid use test cases
                foreach (var isNullValue in new bool[] { true, false })
                {
                    yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() { Gt = 4, IsNull = isNullValue } }, typeof(BadRequestResult));
                    yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() { Lt = 4, IsNull = isNullValue } }, typeof(BadRequestResult));
                    yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() { Gt = 4, Lt = 4, IsNull = isNullValue } }, typeof(BadRequestResult));
                    yield return new TestCaseData(name, new GPAQueryDto() { GPA = new() { Eq = 4, IsNull = isNullValue } }, typeof(BadRequestResult));
                }
            }
            yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = null }, typeof(OkObjectResult));
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = null }, typeof(BadRequestResult));
        }

        [TestCaseSource(nameof(GetQueryStudentsTestCases))]
        public async Task QueryStudents_ShouldReturnExpectedResponseType_ForValidAndInvalidQueries(string? name, GPAQueryDto gpaQueryDto, Type expectedResultType)
        {
            // Arrange
            _mockStudentService.Setup(x => x.QueryStudents(It.IsAny<string>(), It.IsAny<GPAQueryDto>()))
                .ReturnsAsync([new StudentDto()]);

            // Act
            var result = await _controller.QueryStudents(name, gpaQueryDto);

            // Assert
            result.Should().BeOfType(expectedResultType);
        }

        [Test]
        public async Task QueryStudents_ShouldReturnNotFound_WhenNoStudentsReturnedFromQuery()
        {
            // Arrange
            _mockStudentService.Setup(x => x.QueryStudents(It.IsAny<string>(), It.IsAny<GPAQueryDto>()))
                .ReturnsAsync([]);

            // Act
            var result = await _controller.QueryStudents("Test Student", new() { GPA = null });

            // Assert
            result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}