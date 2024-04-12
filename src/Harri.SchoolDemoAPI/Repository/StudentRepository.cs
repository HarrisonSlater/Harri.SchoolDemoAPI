using Dapper;
using Harri.SchoolDemoAPI.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Data;

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
            throw new System.NotImplementedException();
            return 1234;
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
        public bool DeleteStudent(int id)
        {
            return false;
        }
    }
}
