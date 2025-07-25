using Harri.SchoolDemoAPI.Client;
using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Tests.E2E.TestBase;

namespace Harri.SchoolDemoAPI.Tests.E2E
{
    public class StudentApiTests : StudentApiTestBase
    {
        // This test assumes a restored clean database
        [Test]
        public async Task GetStudent_ShouldReturnStudent()
        {
            var student = await _client.GetStudent(1);
            student.Should().NotBeNull();

            student.Name.Should().NotBeEmpty();
            student.GPA.Should().BeGreaterThan(0).And.BeLessThanOrEqualTo(4);
            student.SId.Should().Be(1);
        }

        [Test]
        public async Task AddStudent_ShouldAddNewStudent()
        {
            // Arrange
            var newStudent = new NewStudentDto() { Name = "New Test Student", GPA = 3.81m };

            // Act 
            var id = await _client.AddStudent(newStudent);

            var expectedStudent = new StudentDto()
            {
                SId = id.Value,
                Name = newStudent.Name,
                GPA = newStudent.GPA
            };
            // Assert
            id.Should().NotBeNull();
            id.Should().BeGreaterThan(0);

            var student = await _client.GetStudent(id.Value);
            student.Should().NotBeNull();
            student.Should().BeEquivalentTo(expectedStudent, options => options.Excluding(s => s.RowVersion));
            student.RowVersion.Should().NotBeNullOrEmpty();

            await CleanUpTestStudent(student.SId.Value);
        }

        [Test]
        public async Task UpdateStudent_ShouldUpdateExistingStudent()
        {
            // Arrange 
            var newStudent = new NewStudentDto() { Name = "New Test Student 2", GPA = 3.71m };
            var sId = await _client.AddStudent(newStudent);
            sId.Should().NotBeNull();

            var retrievedStudent = await _client.GetStudent(sId.Value);

            var studentUpdateDto = new UpdateStudentDto() { Name = "New Test Student 2 - Updated Name", GPA = 3.75m };

            var expectedStudent = new StudentDto()
            {
                SId = sId.Value,
                Name = studentUpdateDto.Name,
                GPA = studentUpdateDto.GPA
            };

            // Act
            var success = await _client.UpdateStudent(sId.Value, studentUpdateDto, retrievedStudent.RowVersion!);

            // Assert
            success.Should().BeTrue();

            var updatedStudent = await _client.GetStudent(sId.Value);

            updatedStudent.Should().NotBeNull();
            updatedStudent.Should().BeEquivalentTo(expectedStudent, options => options.Excluding(s => s.RowVersion));
            updatedStudent.RowVersion.Should().NotBeNullOrEmpty();

            await CleanUpTestStudent(sId.Value);
        }

        private static IEnumerable<TestCaseData> PatchStudentTestCases()
        {
            var newStudent1 = new NewStudentDto() { Name = "Test Student 3", GPA = 3.55m };
            var newStudent1PatchDto = new PatchStudentDto() { GPA = 3.99m };
            var expectedStudent1 = new StudentDto() { Name = newStudent1.Name, GPA = newStudent1PatchDto.GPA, };
            yield return new TestCaseData(newStudent1, newStudent1PatchDto, expectedStudent1);

            var newStudent2 = new NewStudentDto() { Name = "Test Student 4", GPA = 3.45m };
            var newStudent2PatchDto = new PatchStudentDto() { Name = "Test Student 4 - PATCHED" };
            var expectedStudent2 = new StudentDto() { Name = newStudent2PatchDto.Name, GPA = newStudent2.GPA, };
            yield return new TestCaseData(newStudent2, newStudent2PatchDto, expectedStudent2);

            var newStudent3 = new NewStudentDto() { Name = "Test Student 5", GPA = 3.35m };
            var newStudent3PatchDto = new PatchStudentDto() { Name = "Test Student 5 - PATCHED", GPA = 4 };
            var expectedStudent3 = new StudentDto() { Name = newStudent3PatchDto.Name, GPA = newStudent3PatchDto.GPA, };
            yield return new TestCaseData(newStudent3, newStudent3PatchDto, expectedStudent3);
        }

        [TestCaseSource(nameof(PatchStudentTestCases))]
        public async Task PatchStudent_ShouldUpdateOnlyGPAOnExistingStudent(NewStudentDto newStudent, PatchStudentDto studentPatchDto, StudentDto expectedStudent)
        {
            // Arrange
            var sId = await _client.AddStudent(newStudent);
            sId.Should().NotBeNull();
            expectedStudent.SId = sId;

            var retrievedStudent = await _client.GetStudent(sId.Value);
            // Act
            var patchedStudent = await _client.PatchStudent(sId.Value, studentPatchDto, retrievedStudent.RowVersion!);
            patchedStudent.Should().NotBeNull();

            // Assert
            patchedStudent.Should().BeEquivalentTo(expectedStudent);

            var student = await _client.GetStudent(sId.Value);
            student.Should().NotBeNull();
            student.Should().BeEquivalentTo(expectedStudent, options => options.Excluding(s => s.RowVersion));
            student.RowVersion.Should().NotBeNullOrEmpty();

            await CleanUpTestStudent(student.SId.Value);
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
            var sId = await _client.AddStudent(newStudent);
            sId.Should().NotBeNull();

            var student = await _client.GetStudent(sId.Value);
            student.Should().NotBeNull();

            // Act
            var success = await _client.DeleteStudent(sId.Value);

            // Assert
            success.Should().Be(true);

            var studentResponse = await _client.GetStudentRestResponse(sId.Value);
            studentResponse.Data.Should().BeNull();
            studentResponse.IsSuccessStatusCode.Should().BeFalse();
            studentResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}