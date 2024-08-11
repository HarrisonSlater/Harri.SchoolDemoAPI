
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Repository;

namespace Harri.SchoolDemoAPI.Tests.Integration.TestBase
{
    public class StudentRepositoryTestBase : IntegrationTestBase
    {
        protected static IStudentRepository _studentRepository;

        [OneTimeSetUp]
        public static void OneTimeSetUpRepositoryBase()
        {
            if (SqlConnectionStringToTest is null) throw new ArgumentException("SqlConnectionStringToTest from appsettings.json cannot be null");

            _studentRepository = new StudentRepository(new DbConnectionFactory(SqlConnectionStringToTest));
        }

        public static StudentDto GetStudentDtoFor(int id, NewStudentDto newStudent)
        {
            return new StudentDto()
            {
                SId = id,
                Name = newStudent.Name,
                GPA = newStudent.GPA
            };
        }

        public async Task CleanUpTestStudent(int sId)
        {
            await _studentRepository.DeleteStudent(sId);
        }
    }
}
