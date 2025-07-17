using Dapper;
using System.Linq;
using System.Data;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;
using Harri.SchoolDemoAPI.Models;
using System;
using Microsoft.IdentityModel.Tokens;
using Harri.SchoolDemoAPI.Results;

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
                var query = @"INSERT INTO [SchoolDemo].Student (sName, GPA)
                              VALUES (@Name, @GPA);
                              SELECT SCOPE_IDENTITY()";

                var sId = (await connection.QueryAsync<int>(query, new { Name = newStudent.Name, GPA = newStudent.GPA })).FirstOrDefault();
                return sId;
            }
        }

        public async Task<StudentDto?> GetStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                return await GetStudentInternal(sId, connection);
            }
        }
        private async Task<StudentDto?> GetStudentInternal(int sId, IDbConnection connection)
        {
            var query = @"SELECT sID as SId, sName as Name, GPA, rowVer as RowVersion
                          FROM [SchoolDemo].Student
                          WHERE sId = @sId";
            var student = await connection.QuerySingleOrDefaultAsync<StudentDto?>(query, new { sId = sId });
            return student;
        }

        public async Task<Result> UpdateStudent(int sId, UpdateStudentDto student, byte[] rowVersion)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var existingStudent = await GetStudentInternal(sId, connection);

                return await UpdateStudentInternal(sId, student, rowVersion, existingStudent, connection);
            }
        }

        private static async Task<Result> UpdateStudentInternal(int sId, UpdateStudentDto student, byte[] rowVersion, StudentDto? existingStudent, IDbConnection connection)
        {
            var studentRowVer = existingStudent?.RowVersion;
            if (studentRowVer is null) return Result.Failure(StudentErrors.StudentNotFound.Error(sId));
            else if (!studentRowVer.SequenceEqual(rowVersion)) return Result.Failure(StudentErrors.StudentRowVersionMismatch.Error(sId));

            var studentUpdateQuery = @"UPDATE [SchoolDemo].Student
                                           SET sName = @Name, GPA = @GPA
                                           WHERE sId = @sId
                                           AND rowVer = @RowVersion";
            var updatedRows = (await connection.ExecuteAsync(studentUpdateQuery, new { Name = student.Name, GPA = student.GPA, sId = sId, RowVersion = rowVersion }));
            if (updatedRows == 0)
            {
                // The only way this can happen is if this student was changed between GetStudentInternal and studentUpdateQuery 
                return Result.Failure(StudentErrors.StudentUpdateConflict.Error(sId));
            }
            else
            {
                return Result.Success();
            }
        }

        public async Task<ResultWith<StudentDto>> PatchStudent(int sId, PatchStudentDto student, byte[] rowVersion)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var existingStudent = await GetStudentInternal(sId, connection);
                student.ApplyChangesTo(existingStudent);

                var result = await UpdateStudentInternal(sId, existingStudent?.AsUpdateStudentDto(), rowVersion, existingStudent, connection);

                return ResultWith<StudentDto>.FromResult(result, existingStudent);
            }
        }

        public async Task<Result> DeleteStudent(int sId)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var studentExistsQuery = @"(SELECT * FROM [SchoolDemo].Student WHERE sId = @sId)";

                bool studentExists = (await connection.QueryAsync(studentExistsQuery, new { sId = sId })).Any();

                if (!studentExists)
                {
                    return Result.Failure(StudentErrors.StudentNotFound.Error(sId));
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
                if (deleted is true)
                {
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(StudentErrors.StudentDeleteConflict.Error(sId));
                }
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
