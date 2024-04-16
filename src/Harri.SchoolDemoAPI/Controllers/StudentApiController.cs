using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Harri.SchoolDemoAPI.Attributes;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Repository;
using Harri.SchoolDemoAPI.Models.Attributes;
using Microsoft.Extensions.Configuration;

namespace Harri.SchoolDemoAPI.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class StudentApiController : ControllerBase
    {
        private readonly IStudentRepository studentService;

        public StudentApiController(IStudentRepository studentService)
        {
            this.studentService = studentService;
        }

        /// <summary>
        /// Add a new student
        /// </summary>
        /// <remarks>Add a new student</remarks>
        /// <param name="newStudent">Create a new student</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        [Route("/student")]
        [Consumes("application/json")]
        [SwaggerOperation(OperationId = "AddStudent")]
        [SwaggerResponse(statusCode: 200, type: typeof(int), description: "Successful operation")]
        public virtual IActionResult AddStudent([FromBody]NewStudent newStudent)
        {
            var result = studentService.AddStudent(newStudent);

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
        [HttpGet]
        [Route("/student/{sId}")]
        [SwaggerOperation(OperationId = "GetStudent")]
        [SwaggerResponse(statusCode: 200, type: typeof(Student), description: "Successful operation")]
        public virtual IActionResult GetStudent([FromRoute(Name = "sId")][Required][PositiveInt] int sId)
        {
            var result = studentService.GetStudent(sId);
            if (result == null) {
                return NotFound();
            }
            return new ObjectResult(result);
        }

        /// <summary>
        /// Update an existing student
        /// </summary>
        /// <remarks>Update an existing student by Id</remarks>
        /// <param name="student">Update an existent student</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Student not found</response>
        [HttpPut]
        [Route("/student")]
        [Consumes("application/json")]
        [SwaggerOperation(OperationId = "UpdateStudent")]
        public virtual IActionResult UpdateStudent([FromBody] Student student)
        {
            var success = studentService.UpdateStudent(student);
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
        /// Delete a student
        /// </summary>
        /// <remarks>Delete a student by sId</remarks>
        /// <param name="sId">ID of student to delete</param>
        /// <response code="200">Successful operation</response>
        /// <response code="409">Conflict. Student has applications which must be deleted first</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Student not found</response>
        [HttpDelete]
        [Route("/student/{sId}")]
        [SwaggerOperation(OperationId = "DeleteStudent")]
        public virtual IActionResult DeleteStudent([FromRoute (Name = "sId")][Required][PositiveInt] int sId)
        {
            var success = studentService.DeleteStudent(sId);
            if (success is null) {
                // Return conflict when student cannot be deleted due to foreign key references in other tables
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
