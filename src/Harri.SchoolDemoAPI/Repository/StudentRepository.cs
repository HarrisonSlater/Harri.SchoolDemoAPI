using Dapper;
using Harri.SchoolDemoAPI.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Data;
using System.Security.Cryptography;

namespace Harri.SchoolDemoAPI.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public StudentRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public int AddStudent(NewStudent newStudent)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var sId = connection.Query<int>("[SchoolDemo].CreateNewStudent", new { sName = newStudent.Name, newStudent.GPA }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return sId;
            }
        }

        public Student? GetStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var student = connection.Query<Student?>("[SchoolDemo].GetStudent", new {sId}, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return student;
            }
        }


        public bool UpdateStudent(Student newStudent)
        {
            return false;
        }
        public bool? DeleteStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var student = connection.Query<bool?>("[SchoolDemo].DeleteStudent", new { sId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return student;
            }
        }
    }
}
