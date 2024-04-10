using Dapper;
using Harri.SchoolDemoAPI.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;

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
                var sql = $"SELECT sId, sName AS Name, GPA FROM SchoolDemo.Student WHERE sId = '{sId}'";
                var student = connection.Query<Student>(sql);
                return student?.FirstOrDefault();
            }
        }


        public void UpdateStudent(Student newStudent)
        {

        }
        public void DeleteStudent(int id)
        {

        }
    }
}
