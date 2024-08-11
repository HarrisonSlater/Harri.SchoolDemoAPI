using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Repository;
using Harri.SchoolDemoAPI.Tests.Integration.TestBase;
using Microsoft.Data.SqlClient;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    public class StudentRepositoryTests : StudentRepositoryTestBase
    {
        // This test assumes a restored clean database
        [Test]
        public async Task GetStudent_ShouldReturnStudent()
        {
            var student = await _studentRepository.GetStudent(1);
            student.Should().NotBeNull();

            student.Name.Should().NotBeEmpty();
            student.GPA.Should().BeGreaterThan(0).And.BeLessThanOrEqualTo(4);
            student.SId.Should().Be(1);
        }

        [Test]
        public async Task GetStudent_ShouldReturnNull()
        {
            var student = await _studentRepository.GetStudent(-1001);
            student.Should().BeNull();
        }

        [TestCase("New Test Student", 3.81)]
        [TestCase("New Test Student", null)]
        public async Task AddStudent_ShouldAddNewStudent(string? name, decimal? gpa)
        {
            // Arrange
            var newStudent = new NewStudentDto() { Name = name, GPA = gpa };

            // Act 
            var id = await _studentRepository.AddStudent(newStudent);

            var expectedStudent = new StudentDto()
            {
                SId = id,
                Name = newStudent.Name,
                GPA = newStudent.GPA
            };
            // Assert
            id.Should().BeGreaterThan(0);

            var student = await _studentRepository.GetStudent(id);
            student.Should().NotBeNull();
            student.Should().BeEquivalentTo(expectedStudent);

            await CleanUpTestStudent(student!.SId!.Value);
        }

        [TestCase(null, null)]
        [TestCase(null, 3.81)]
        public async Task AddStudent_ShouldNotAddNewStudent(string? name, decimal? gpa)
        {
            // Arrange
            var newStudent = new NewStudentDto() { Name = name, GPA = gpa };

            // Act 
            var action = async () => await _studentRepository.AddStudent(newStudent);

            // Assert
            await action.Should().ThrowAsync<SqlException>();
        }

        [TestCase("New Test Student 2 - Updated Name", 3.75)]
        [TestCase("New Test Student 2 - Updated Name", null)]
        public async Task UpdateStudent_ShouldUpdateExistingStudent(string? name, decimal? gpa)
        {
            // Arrange 
            var newStudent = new NewStudentDto() { Name = "New Test Student 2", GPA = 3.71m };
            var sId = await _studentRepository.AddStudent(newStudent);
            sId.Should().BeGreaterThan(0);

            var studentUpdateDto = new UpdateStudentDto() { Name = name, GPA = gpa };

            var expectedStudent = new StudentDto()
            {
                SId = sId,
                Name = studentUpdateDto.Name,
                GPA = studentUpdateDto.GPA
            };

            // Act
            var success = await _studentRepository.UpdateStudent(sId, studentUpdateDto);

            // Assert
            success.Should().BeTrue();

            var updatedStudent = await _studentRepository.GetStudent(sId);

            updatedStudent.Should().NotBeNull();
            updatedStudent.Should().BeEquivalentTo(expectedStudent);

            await CleanUpTestStudent(sId);
        }

        [TestCase(null, null)]
        [TestCase(null, 3.75)]

        public async Task UpdateStudent_ShouldThrow_WhenUpdatingExistingStudent(string? name, decimal? gpa)
        {
            // Arrange 
            var newStudent = new NewStudentDto() { Name = "New Test Student 2", GPA = 3.71m };
            var sId = await _studentRepository.AddStudent(newStudent);
            sId.Should().BeGreaterThan(0);

            var studentUpdateDto = new UpdateStudentDto() { Name = name, GPA = gpa };

            // Act
            var action = async () => await _studentRepository.UpdateStudent(sId, studentUpdateDto);

            // Assert
            await action.Should().ThrowAsync<SqlException>();
        }

        [Test]
        public async Task UpdateStudent_ShouldReturnFalse_WhenUpdatingNonExistantStudent()
        {
            // Arrange 
            var nonExistantSId = -1001;

            var studentUpdateDto = new UpdateStudentDto() { Name = "Test Student" };

            // Act
            var success = await _studentRepository.UpdateStudent(nonExistantSId, studentUpdateDto);

            // Assert
            success.Should().BeFalse();
        }

        [Test]
        public async Task DeleteStudent_ShouldDeleteStudent()
        {
            // Arrange
            var newStudent = new NewStudentDto()
            {
                Name = "New Test Student - To be deleted",
                GPA = 3.81m
            };
            var sId = await _studentRepository.AddStudent(newStudent);
            sId.Should().BeGreaterThan(0);

            var student = await _studentRepository.GetStudent(sId);
            student.Should().NotBeNull();

            // Act
            var success = await _studentRepository.DeleteStudent(sId);

            // Assert
            success.Should().Be(true);

            var studentResponse = await _studentRepository.GetStudent(sId);
            studentResponse.Should().BeNull();
        }

        [Test]
        public async Task DeleteStudent_ShouldReturnFalse_WhenDeletingNonExistantStudent()
        {
            // Arrange
            var nonExistantSId = -1001;

            // Act
            var success = await _studentRepository.DeleteStudent(nonExistantSId);

            // Assert
            success.Should().Be(false);
        }

        [Test]
        public async Task GetAllStudents_ShouldGetAllStudents()
        {
            // Arrange
            // Act
            var students = await _studentRepository.GetStudents();

            // Assert
            students.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(900);
        }
    }
}