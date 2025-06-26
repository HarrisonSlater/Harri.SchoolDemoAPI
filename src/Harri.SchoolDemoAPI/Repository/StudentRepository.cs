using Dapper;
using System.Linq;
using System.Data;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;
using Harri.SchoolDemoAPI.Models;
using System;
using Microsoft.IdentityModel.Tokens;

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
            if (newStudent.Name.IsNullOrEmpty()) throw new ArgumentNullException("Name must be set");

            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var query = @"INSERT INTO [SchoolDemo].Student VALUES (@Name, @GPA);
                              SELECT SCOPE_IDENTITY()";

                var sId = (await connection.QueryAsync<int>(query, new { Name = newStudent.Name, GPA = newStudent.GPA })).FirstOrDefault();
                return sId;
            }
        }

        public async Task<StudentDto?> GetStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var query = @"SELECT sID as sId, sName as Name, GPA 
                              FROM [SchoolDemo].Student
                              WHERE sId = @sId";

                var student = (await connection.QueryAsync<StudentDto?>(query, new { sId = sId })).FirstOrDefault();
                return student;
            }
        }


        public async Task<bool> UpdateStudent(int sId, UpdateStudentDto student)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var studentExistsQuery = @"(SELECT * FROM [SchoolDemo].Student WHERE sId = @sId)";

                bool studentExists = (await connection.QueryAsync(studentExistsQuery, new { sId = sId })).Any();

                if (!studentExists)
                {
                    return false;
                }

                var studentUpdateQuery = @"UPDATE [SchoolDemo].Student
                                           SET sName = @Name, GPA = @GPA
                                           WHERE sId = @sId";

                (await connection.QueryAsync(studentUpdateQuery, new { Name = student.Name, GPA = student.GPA, sId = sId })).Any();

                return true;
            }
        }

        public async Task<bool?> DeleteStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var studentExistsQuery = @"(SELECT * FROM [SchoolDemo].Student WHERE sId = @sId)";

                bool studentExists = (await connection.QueryAsync(studentExistsQuery, new { sId = sId })).Any();

                if (!studentExists)
                {
                    return false;
                }

                var deleteQuery = @"BEGIN TRY
                                        DELETE FROM [SchoolDemo].Student
                                        WHERE sID = @sId;

                                        SELECT CAST(1 AS BIT);
                                    END TRY
                                    BEGIN CATCH
                                        SELECT NULL
                                    END CATCH";

                var deleted = (await connection.QueryAsync<bool?>(deleteQuery, new { sId = sId })).FirstOrDefault();
                return deleted;
            }
        }

        public async Task<PagedList<StudentDto>> GetStudents(GetStudentsQueryDto queryDto)
        {
            GetStudentsQueryDtoGuard(queryDto);

            var builder = new SqlBuilder();

            SetWhereFilters(queryDto, builder);

            builder.OrderBy($"{GetCleanSortColumn(queryDto.SortColumn)} {queryDto.OrderBy}");

            var baseQuery = @$"SELECT sID as sId, sName as Name, GPA FROM [SchoolDemo].Student WITH (NOLOCK) /**where**/ /**orderby**/
                                OFFSET @PageSize * (@Page - 1) ROWS
                                FETCH NEXT @PageSize ROWS ONLY;";

            builder.AddParameters(new { Page = queryDto.Page, PageSize = queryDto.PageSize });

            baseQuery += $"SELECT COUNT(*) FROM [SchoolDemo].Student WITH (NOLOCK) /**where**/";

            var fullQuery = builder.AddTemplate(baseQuery);

            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var gridReader = (await connection.QueryMultipleAsync(fullQuery.RawSql, fullQuery.Parameters));
                var items = gridReader.Read<StudentDto>().ToList();
                var count = gridReader.ReadSingle<int>();

                return new PagedList<StudentDto>() { Items = items, Page = queryDto.Page.Value, PageSize = queryDto.PageSize.Value, TotalCount = count };
            }
        }

        private void GetStudentsQueryDtoGuard(GetStudentsQueryDto queryDto)
        {
            if (queryDto.OrderBy is null) throw new ArgumentNullException("OrderBy must be set");
            if (queryDto.SortColumn is null) throw new ArgumentNullException("SortColumn must be set");
            if (queryDto.Page is null || queryDto.PageSize is null) throw new ArgumentNullException("Page and PageSize must be set");
        }

        private static void SetWhereFilters(GetStudentsQueryDto queryDto, SqlBuilder builder)
        {
            if (queryDto.Name != null)
            {
                builder.Where("sName LIKE @searchString", new { searchString = $"%{queryDto.Name}%" });
            }
            if (queryDto.SId != null)
            {
                builder.Where("sID LIKE @searchSId", new { searchSId = $"%{queryDto.SId}%" });
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
        }

        internal static string GetCleanSortColumn(string sortColumn)
        {
            var cleanedColumn = sortColumn.ToLower() switch
            {
                var s when s == APIConstants.Student.SId.ToLower() => APIConstants.Student.SId,
                var s when s == APIConstants.Student.Name.ToLower() => APIConstants.Student.Name,
                var s when s == APIConstants.Student.GPA.ToLower() => APIConstants.Student.GPA,
                _ => throw new ArgumentException()
            };
            return cleanedColumn;
        }
    }
}
