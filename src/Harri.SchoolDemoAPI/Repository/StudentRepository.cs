using Dapper;
using System.Linq;
using System.Data;
using System.Security.Cryptography;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Harri.SchoolDemoAPI.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public StudentRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<int> AddStudent(NewStudentDto newStudent)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var sId = (await connection.QueryAsync<int>("[SchoolDemo].CreateNewStudent", new { sName = newStudent.Name, newStudent.GPA }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                return sId;
            }
        }

        public async Task<StudentDto?> GetStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var student = (await connection.QueryAsync<StudentDto?>("[SchoolDemo].GetStudent", new { sId }, commandType: CommandType.StoredProcedure)).FirstOrDefault<StudentDto>();
                return student;
            }
        }


        public async Task<bool> UpdateStudent(int sId, UpdateStudentDto student)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var success = (await connection.QueryAsync<bool>("[SchoolDemo].UpdateStudent", new { sId = sId, sName = student.Name, student.GPA }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
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

        public async Task<List<StudentDto>> GetAllStudents()
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                return (await connection.QueryAsync<StudentDto>("SELECT sID as sId, sName as Name, GPA FROM [SchoolDemo].Student ORDER BY sId")).ToList();
            }
        }

        public async Task<List<StudentDto>> QueryStudents(string? name, GPAQueryDto? gpaQuery)
        {
            throw new System.NotImplementedException();
        }
    }
}
