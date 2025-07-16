using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Repository;
using Harri.SchoolDemoAPI.Results;
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

            student!.Name.Should().NotBeEmpty();
            student!.GPA.Should().BeGreaterThan(0).And.BeLessThanOrEqualTo(4);
            student!.SId.Should().Be(1);
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
            student!.Should().BeEquivalentTo(expectedStudent, options => options.Excluding(s => s.RowVersion));
            student!.RowVersion.Should().NotBeNullOrEmpty();

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
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        private async Task<StudentDto> CreateStudentAndGetRowVersion(NewStudentDto newStudentDto)
        {
            var sId = await _studentRepository.AddStudent(newStudentDto);
            sId.Should().BeGreaterThan(0);

            var student = await _studentRepository.GetStudent(sId);

            if (student?.RowVersion is null) throw new ArgumentNullException("RowVersion cannot be null for test");
            if (student?.SId is null) throw new ArgumentNullException("sId cannot be null for test");
            return student;
        }

        private async Task AssertRowVersionIsUnmodified(StudentDto student)
        {
            var previousVersion = student.RowVersion;

            var newVersion = (await _studentRepository.GetStudent(student.SId!.Value))?.RowVersion;

            previousVersion.Should().NotBeNullOrEmpty();
            newVersion.Should().NotBeNullOrEmpty();
            previousVersion.Should().BeEquivalentTo(newVersion);
        }

        [TestCase("New Test Student 2 - Updated Name", 3.75)]
        [TestCase("New Test Student 2 - Updated Name", null)]
        public async Task UpdateStudent_ShouldUpdateExistingStudent(string? name, decimal? gpa)
        {
            // Arrange 
            var student = await CreateStudentAndGetRowVersion(new NewStudentDto() { Name = "New Test Student 2", GPA = 3.71m });
            int sId = student.SId!.Value;

            var studentUpdateDto = new UpdateStudentDto() { Name = name, GPA = gpa };

            var expectedStudent = new StudentDto()
            {
                SId = sId,
                Name = studentUpdateDto.Name,
                GPA = studentUpdateDto.GPA
            };

            // Act
            var success = await _studentRepository.UpdateStudent(sId, studentUpdateDto, student.RowVersion!);

            // Assert
            success.IsSuccess.Should().BeTrue();

            var updatedStudent = await _studentRepository.GetStudent(sId);

            updatedStudent.Should().NotBeNull();
            updatedStudent.Should().BeEquivalentTo(expectedStudent, options => options.Excluding(x => x.RowVersion));
            updatedStudent!.RowVersion.Should().NotBeNullOrEmpty()
                .And.NotBeEquivalentTo(student.RowVersion);

            await CleanUpTestStudent(sId);
        }

        [TestCase(null, null)]
        [TestCase(null, 3.75)]

        public async Task UpdateStudent_ShouldThrow_WhenIncorrectlyUpdatingExistingStudent(string? name, decimal? gpa)
        {
            // Arrange 
            var student = await CreateStudentAndGetRowVersion(new NewStudentDto() { Name = "New Test Student 2", GPA = 3.71m });
            int sId = student.SId!.Value;

            var studentUpdateDto = new UpdateStudentDto() { Name = name, GPA = gpa };

            // Act
            var action = async () => await _studentRepository.UpdateStudent(sId, studentUpdateDto, student.RowVersion!);

            // Assert
            await action.Should().ThrowAsync<SqlException>();
            await AssertRowVersionIsUnmodified(student);

            await CleanUpTestStudent(sId);
        }

        [Test]
        public async Task UpdateStudent_ShouldReturnFailure_WhenUpdatingStudentWithNonMatchingRowVersion()
        {
            // Arrange 
            var student = await CreateStudentAndGetRowVersion(new NewStudentDto() { Name = "New Test Student 3", GPA = 3.71m });
            int sId = student.SId!.Value;

            var studentUpdateDto = new UpdateStudentDto() { Name = "Test Student 3 - Updated" };

            // Act
            var result = await _studentRepository.UpdateStudent(sId, studentUpdateDto, new byte[8]);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(StudentErrors.StudentRowVersionMismatch.Error(sId));
            await AssertRowVersionIsUnmodified(student);
        }

        [Test]
        public async Task UpdateStudent_ShouldReturnFailure_WhenUpdatingNonExistantStudent()
        {
            // Arrange 
            var nonExistantSId = -1001;

            var studentUpdateDto = new UpdateStudentDto() { Name = "Test Student" };

            // Act
            var result = await _studentRepository.UpdateStudent(nonExistantSId, studentUpdateDto, new byte[8]);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(StudentErrors.StudentNotFound.Error(nonExistantSId));
        }

        private static IEnumerable<TestCaseData> PatchStudentTestCases()
        {
            yield return new TestCaseData(new StudentPatchDto() {});
            yield return new TestCaseData(new StudentPatchDto() { Name = "New Test Student 2 - Patched" });
            yield return new TestCaseData(new StudentPatchDto() { Name = "New Test Student 2 - Patched", GPA = 4.32m });
            yield return new TestCaseData(new StudentPatchDto() { GPA = 4.32m });
            yield return new TestCaseData(new StudentPatchDto() { GPA = null });
        }

        [TestCaseSource(nameof(PatchStudentTestCases))]
        public async Task UpdateStudent_ShouldPatchExistingStudent(StudentPatchDto studentPatchDto)
        {
            // Arrange 
            var student = await CreateStudentAndGetRowVersion(new NewStudentDto() { Name = "New Test Student 2", GPA = 3.71m });
            int sId = student.SId!.Value;

            var expectedStudent = new StudentDto()
            {
                SId = sId,
                Name = studentPatchDto.OptionalName.HasValue ? studentPatchDto.Name : student.Name,
                GPA = studentPatchDto.OptionalGPA.HasValue ? studentPatchDto.GPA : student.GPA,
            };

            // Act
            var success = await _studentRepository.PatchStudent(sId, studentPatchDto, student.RowVersion!);

            // Assert
            success.IsSuccess.Should().BeTrue();

            var updatedStudent = await _studentRepository.GetStudent(sId);

            updatedStudent.Should().NotBeNull();
            updatedStudent.Should().BeEquivalentTo(expectedStudent, options => options.Excluding(x => x.RowVersion));
            updatedStudent!.RowVersion.Should().NotBeNullOrEmpty()
                .And.NotBeEquivalentTo(student.RowVersion);

            await CleanUpTestStudent(sId);

        }

        private static IEnumerable<TestCaseData> PatchStudentInvalidTestCases()
        {
            yield return new TestCaseData(new StudentPatchDto() { Name = null });
            yield return new TestCaseData(new StudentPatchDto() { Name = null, GPA = 4.32m });
        }

        [TestCaseSource(nameof(PatchStudentInvalidTestCases))]
        public async Task PatchStudent_ShouldThrow_WhenIncorrectlyUpdatingExistingStudent(StudentPatchDto studentPatchDto)
        {
            // Arrange 
            var student = await CreateStudentAndGetRowVersion(new NewStudentDto() { Name = "New Test Student 2", GPA = 3.71m });
            int sId = student.SId!.Value;

            // Act
            var action = async () => await _studentRepository.PatchStudent(sId, studentPatchDto, student.RowVersion!);

            // Assert
            await action.Should().ThrowAsync<SqlException>();
            await AssertRowVersionIsUnmodified(student);

            await CleanUpTestStudent(sId);
        }

        [Test]
        public async Task PatchStudent_ShouldReturnFailure_WhenPatchinghStudentWithNonMatchingRowVersion()
        {
            // Arrange 
            var student = await CreateStudentAndGetRowVersion(new NewStudentDto() { Name = "New Test Student 3", GPA = 3.71m });
            int sId = student.SId!.Value;

            var studentPatchDto = new StudentPatchDto() { Name = "Test Student 3 - Patchd" };

            // Act
            var result = await _studentRepository.PatchStudent(sId, studentPatchDto, new byte[8]);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(StudentErrors.StudentRowVersionMismatch.Error(sId));
            await AssertRowVersionIsUnmodified(student);
        }

        [Test]
        public async Task PatchStudent_ShouldReturnFailure_WhenPatchingNonExistantStudent()
        {
            // Arrange 
            var nonExistantSId = -1001;

            var studentPatchDto = new StudentPatchDto() { Name = "Test Student" };

            // Act
            var result = await _studentRepository.PatchStudent(nonExistantSId, studentPatchDto, new byte[8]);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(StudentErrors.StudentNotFound.Error(nonExistantSId));
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

        //TODO after impelementing Applications respository, add delete conflict test (where a student is being deleted but has existing applications)

        [Test]
        public async Task GetAllStudents_ShouldGetAllStudents()
        {
            // Arrange
            // Act
            var pageSize = 10;
            var students = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Page = 1, PageSize = pageSize, OrderBy = Models.Enums.SortOrder.ASC, SortColumn = APIConstants.Student.SId });

            // Assert
            students.Should().NotBeNull();
            students.Items.Should().NotBeNullOrEmpty().And.HaveCount(pageSize);
        }
    }
}