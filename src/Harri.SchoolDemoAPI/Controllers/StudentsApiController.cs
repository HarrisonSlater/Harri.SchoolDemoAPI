using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Harri.SchoolDemoAPI.Models.Attributes;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Services;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Attributes.SortColumn;
using Harri.SchoolDemoAPI.Results;
using System;
using Microsoft.Net.Http.Headers;

namespace Harri.SchoolDemoAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("/students")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Tags("Student")]
    public class StudentsApiController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsApiController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Add a new student
        /// </summary>
        /// <remarks>Add a new student</remarks>
        /// <param name="newStudent">Create a new student</param>
        /// <returns>New student sId</returns>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        [SwaggerOperation(OperationId = "AddStudent")]
        [SwaggerResponse(statusCode: 200, type: typeof(int), description: "Successful operation")]
        public async Task<IActionResult> AddStudent([FromBody]NewStudentDto newStudent)
        {
            var result = await _studentService.AddStudent(newStudent);

            return new ObjectResult(result);
        }

        /// <summary>
        /// Get a student
        /// </summary>
        /// <remarks>Get a student by sId</remarks>
        /// <param name="sId">ID of student to return</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Student not found</response>
        [HttpGet("{sId}")]
        [SwaggerOperation(OperationId = "GetStudent")]
        [SwaggerResponse(statusCode: 200, type: typeof(StudentDto), description: "Successful operation")]
        public async Task<IActionResult> GetStudent([FromRoute(Name = "sId")][Required][PositiveInt] int sId)
        {
            //TODO contract tests for etag

            var result = await _studentService.GetStudent(sId);
            if (result is null) {
                return NotFound();
            }
            return new ObjectResult(result);
        }

        /// <summary>
        /// Update an existing student
        /// </summary>
        /// <remarks>Update an existing student by sId</remarks>
        /// <param name="sId">Student ID</param>
        /// <param name="student">All properties on student will be updated</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Student not found</response>
        [HttpPut("{sId}")]
        [SwaggerOperation(OperationId = "UpdateStudent")]
        public async Task<IActionResult> UpdateStudent([FromRoute][Required][PositiveInt] int sId, [FromBody] UpdateStudentDto student)
        {
            var rowVersion = Convert.FromBase64String(Request.Headers[HeaderNames.IfMatch]);

            var result = await _studentService.UpdateStudent(sId, student, rowVersion);
            if (result.IsSuccess)
            {
                return Ok();
            }

            return result.MatchError<IActionResult>(error => error.Code switch
            {
                StudentErrors.StudentNotFound.ErrorCode => NotFound(),
                StudentErrors.StudentUpdateConflict.ErrorCode => Conflict(),
                _ => throw new InvalidOperationException($"Unexpected error code: {error.Code}")
            });
        }

        /// <summary>
        /// Patch an existing student
        /// </summary>
        /// <remarks>Patch an existing student by sId</remarks>
        /// <param name="sId">Student ID</param>
        /// <param name="student">Only properties in the request will be updated</param>
        /// <returns>Modified student</returns>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request supplied</response>
        /// <response code="404">Student not found</response>
        [HttpPatch("{sId}")]
        [SwaggerOperation(OperationId = "PatchStudent")]
        public async Task<IActionResult> PatchStudent([FromRoute][Required][PositiveInt]int sId, [FromBody] StudentPatchDto student)
        {
            var rowVersion = Convert.FromBase64String(Request.Headers[HeaderNames.IfMatch]);// TODO

            if (!student.OptionalName.HasValue && !student.OptionalGPA.HasValue)
            {
                return BadRequest();
            }

            var patchedStudent = await _studentService.PatchStudent(sId, student, rowVersion);
            if (patchedStudent is not null)
            {
                return Ok(patchedStudent);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Delete a student
        /// </summary>
        /// <remarks>Delete a student by sId</remarks>
        /// <param name="sId">ID of student to delete</param>
        /// <response code="200">Successful operation</response>
        /// <response code="409">Conflict. Student has applications which must be deleted first</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Student not found</response>
        [HttpDelete("{sId}")]
        [SwaggerOperation(OperationId = "DeleteStudent")]
        public async Task<IActionResult> DeleteStudent([FromRoute][Required][PositiveInt] int sId)
        {
            var success = await _studentService.DeleteStudent(sId);
            if (success is null) {
                // Return conflict when student cannot be deleted due to applications referencing that student exist
                return Conflict();
            }
            else if (success.Value)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get students
        /// </summary>
        /// <remarks>Get all  students by optional query</remarks>
        /// <param name="sId">Student ID partial to search on</param>
        /// <param name="name">Name partial of students to search on. Case insensitive</param>
        /// <param name="gpaQuery">Query object to search by GPA (lt, gt, eq). See <see cref="ComparativeQueryDto{T}"></see></param>
        /// <param name="orderBy">ASC or DESC. Default sort order is ASC</param>
        /// <param name="sortColumn">String name of column to sort on</param>
        /// <param name="page">Which page you are requesting. Indexed from 1</param>
        /// <param name="pageSize">How many students per page. Default is <see cref="APIDefaults.Query.PageSize"/></param>
        /// <response code="200">Successful operation</response>
        /// <response code="204">No students found</response>
        /// <response code="400">Invalid query supplied</response>
        [HttpGet]
        [Route("/students")]
        [SwaggerOperation(OperationId = "GetStudents")]
        [SwaggerResponse(statusCode: 200, type: typeof(PagedList<StudentDto>), description: "Successful operation")]
        [Tags("Students")]
        public async Task<IActionResult> GetStudents(
            [FromQuery(Name = APIConstants.Student.SId)][PositiveInt] int? sId,
            [FromQuery(Name = APIConstants.Student.Name)] string? name,
            [FromQuery] GPAQueryDto gpaQuery,
            [FromQuery(Name = APIConstants.Query.OrderBy)] SortOrder orderBy = APIDefaults.Query.OrderBy,
            [FromQuery(Name = APIConstants.Query.SortColumn)][ValidStudentSortColumn] string sortColumn = APIDefaults.Query.SortColumn,
            [FromQuery(Name = APIConstants.Query.Page)][PositiveInt] int page = APIDefaults.Query.Page,
            [FromQuery(Name = APIConstants.Query.PageSize)][PositiveInt] int pageSize = APIDefaults.Query.PageSize)
        {
            var students = await _studentService.GetStudents(new GetStudentsQueryDto() 
            {
                SId = sId,
                Name = name,
                GPAQueryDto = gpaQuery,
                OrderBy = orderBy,
                SortColumn = sortColumn,
                Page = page,
                PageSize = pageSize
            });

            // If there are students found but the requested page is out of bounds
            if (students.TotalCount > 0 && page > students.TotalPageCount)
            {
                return BadRequest();
            }

            if (students.Items.IsNullOrEmpty())
            {
                return NoContent();
            }
            else
            {
                return Ok(students);
            }
        } 
    }
}
