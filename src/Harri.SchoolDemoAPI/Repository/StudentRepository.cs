using Dapper;
using System.Linq;
using System.Data;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Models;

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

        public async Task<List<StudentDto>> GetStudents(GetStudentsQueryDto queryDto)
        {
            if (queryDto.OrderBy is null)
            {
                queryDto.OrderBy = SortOrder.ASC;
            }

            var builder = new SqlBuilder();
            
            if (queryDto.Name != null)
            {
                builder.Where("sName LIKE @searchString", new { searchString = $"%{queryDto.Name}%" });
            }
            if (queryDto.GPAQueryDto?.GPA != null)
            {
                var gpa = queryDto.GPAQueryDto.GPA;

                if (gpa.IsNull.HasValue && gpa.IsNull == true)
                {
                    builder.Where("GPA IS NULL");
                }
                else if (gpa.IsNull.HasValue && gpa.IsNull == false)
                {
                    builder.Where("GPA IS NOT NULL");
                }

                if (gpa.Eq.HasValue)
                {
                    builder.Where("GPA = @eq", new { eq = gpa.Eq });
                }
                if (gpa.Gt.HasValue)
                {
                    builder.Where("GPA > @gt", new { gt = gpa.Gt });
                }
                if (gpa.Lt.HasValue)
                {
                    builder.Where("GPA < @lt", new { lt = gpa.Lt });
                }
            }

            var sortColumn = queryDto.SortColumn ?? APIConstants.Student.SId;// TODO clean case
            var orderBy = queryDto.OrderBy;

            builder.OrderBy($"{sortColumn} {orderBy}");

            var baseQuery = $"SELECT sID as sId, sName as Name, GPA FROM [SchoolDemo].Student /**where**/ /**orderby**/";
            var fullQuery = builder.AddTemplate(baseQuery);

            using (var connection = _dbConnectionFactory.GetConnection())
            {
                return (await connection.QueryAsync<StudentDto>(fullQuery.RawSql, fullQuery.Parameters)).ToList();
            }
        }
    }
}
