using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Harri.SchoolDemoAPI.Models.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Services;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("/students")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class StudentApiController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentApiController(IStudentService studentService)
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
        /// <param name="student">All properties on student will be updated</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Student not found</response>
        [HttpPut("{sId}")]
        [SwaggerOperation(OperationId = "UpdateStudent")]
        public async Task<IActionResult> UpdateStudent([FromRoute][Required][PositiveInt] int sId, [FromBody] UpdateStudentDto student)
        {
            var success = await _studentService.UpdateStudent(sId, student);
            if (success)
            {
                return Ok(success);
            }
            else
            {
                return NotFound(success);
            }
        }

        /// <summary>
        /// Patch an existing student
        /// </summary>
        /// <remarks>Patch an existing student by sId</remarks>
        /// <param name="student">Only properties in the request will be updated</param>
        /// <returns>Modified student</returns>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request supplied</response>
        /// <response code="404">Student not found</response>
        [HttpPatch("{sId}")]
        [SwaggerOperation(OperationId = "PatchStudent")]
        public async Task<IActionResult> PatchStudent([FromRoute][Required][PositiveInt]int sId, [FromBody] StudentPatchDto student)
        {
            if (!student.OptionalName.HasValue && !student.OptionalGPA.HasValue)
            {
                return BadRequest();
            }

            var patchedStudent = await _studentService.PatchStudent(sId, student);
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
                return Conflict(success);
            }
            else if (success.Value)
            {
                return Ok(success);
            }
            else
            {
                return NotFound(success);
            }
        }
    }
}
