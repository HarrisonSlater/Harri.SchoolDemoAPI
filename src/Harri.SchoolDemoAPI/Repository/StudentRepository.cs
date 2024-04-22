using Dapper;
using Harri.SchoolDemoAPI.Models;
using System.Linq;
using System.Data;
using System.Security.Cryptography;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Repository
{
    //TODO use async
    public class StudentRepository : IStudentRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public StudentRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<int> AddStudent(NewStudent newStudent)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var sId = (await connection.QueryAsync<int>("[SchoolDemo].CreateNewStudent", new { sName = newStudent.Name, newStudent.GPA }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                return sId;
            }
        }

        public async Task<Student?> GetStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var student = (await connection.QueryAsync<Student?>("[SchoolDemo].GetStudent", new {sId}, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                return student;
            }
        }


        public async Task<bool> UpdateStudent(Student student)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var success = (await connection.QueryAsync<bool>("[SchoolDemo].UpdateStudent", new { sId = student.SId, sName = student.Name, student.GPA }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                return success;
            }
        }

        public async Task<bool?> DeleteStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var success = (await connection.QueryAsync<bool?>("[SchoolDemo].DeleteStudent", new { sId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                return success;
            }
        }
    }
}
