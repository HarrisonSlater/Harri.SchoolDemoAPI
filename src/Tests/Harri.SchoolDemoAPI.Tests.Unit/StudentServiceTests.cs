using FluentAssertions;
using Harri.SchoolDemoAPI.Controllers;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Repository;
using Harri.SchoolDemoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Harri.SchoolDemoAPI.Tests.Unit
{
    [TestFixture]
    public class StudentServiceTests
    {
        private IStudentService _studentService;
        private Mock<IStudentRepository> _mockStudentRepo;

        [SetUp]
        public void Setup()
        {
            _mockStudentRepo = new Mock<IStudentRepository>();
            _studentService = new StudentService(_mockStudentRepo.Object);
        }

        private static IEnumerable<TestCaseData> PatchStudentTestCases()
        {
            yield return new TestCaseData(new StudentPatchDto(), new StudentDto(), new StudentDto());

            var existingName = "Existing Student Name";
            yield return new TestCaseData(new StudentPatchDto() { GPA = 3.9m }, new StudentDto() { }, new StudentDto() { GPA = 3.9m });
            yield return new TestCaseData(new StudentPatchDto() { GPA = 3.9m }, new StudentDto() { Name = existingName }, new StudentDto() { Name = existingName, GPA = 3.9m });
            yield return new TestCaseData(new StudentPatchDto() { GPA = 3.9m }, new StudentDto() { Name = existingName, GPA = 1 }, new StudentDto() { Name = existingName, GPA = 3.9m });

            yield return new TestCaseData(new StudentPatchDto() { Name = "Patched Name", GPA = 3.9m }, new StudentDto() { }, new StudentDto() { Name = "Patched Name", GPA = 3.9m });
            yield return new TestCaseData(new StudentPatchDto() { Name = "Patched Name", GPA = 3.9m }, new StudentDto() { Name = existingName }, new StudentDto() { Name = "Patched Name", GPA = 3.9m });
            yield return new TestCaseData(new StudentPatchDto() { Name = "Patched Name", GPA = 3.9m }, new StudentDto() { Name = existingName, GPA = 1 }, new StudentDto() { Name = "Patched Name", GPA = 3.9m });

            yield return new TestCaseData(new StudentPatchDto() { Name = "Patched Name" }, new StudentDto() { }, new StudentDto() { Name = "Patched Name" });
            yield return new TestCaseData(new StudentPatchDto() { Name = "Patched Name" }, new StudentDto() { Name = existingName }, new StudentDto() { Name = "Patched Name" });
            yield return new TestCaseData(new StudentPatchDto() { Name = "Patched Name" }, new StudentDto() { Name = existingName, GPA = 1 }, new StudentDto() { Name = "Patched Name", GPA = 1 });
        }

        [TestCaseSource(nameof(PatchStudentTestCases))]
        public async Task PatchStudent_ShouldApplyPatchCorrectly(StudentPatchDto patchDto, StudentDto existingStudent, StudentDto expectedPatchedStudent)
        {
            // Arrange
            var sid = 123;

            _mockStudentRepo.Setup(s => s.GetStudent(123)).ReturnsAsync(existingStudent);

            // Act
            var response = await _studentService.PatchStudent(sid, patchDto);

            // Assert
            response.Should().NotBeNull();

            response.Should().BeEquivalentTo(expectedPatchedStudent);

            _mockStudentRepo.Verify(x => x.UpdateStudent(sid, It.Is<UpdateStudentDto>(s => s.Name == existingStudent.Name && s.GPA == existingStudent.GPA)));
        }

        [Test]
        public async Task PatchStudent_WhenStudentNotFound_ShouldReturnNull()
        {
            // Arrange
            var sid = 123;

            _mockStudentRepo.Setup(s => s.GetStudent(123)).ReturnsAsync((StudentDto?)null);

            // Act
            var response = await _studentService.PatchStudent(sid, new StudentPatchDto() { Name = "Student Name Patch"});

            // Assert
            response.Should().BeNull();
        }
    }
}
