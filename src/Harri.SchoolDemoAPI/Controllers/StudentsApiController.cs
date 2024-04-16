using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Harri.SchoolDemoAPI.Models;
using System.Text.Json;

namespace Harri.SchoolDemoAPI.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class StudentsApiController : ControllerBase
    { 
        /// <summary>
        /// Get students
        /// </summary>
        /// <remarks>Get students by query</remarks>
        /// <param name="name">Name partial of students to search on. Case insensitive</param>
        /// <param name="GPAQuery">query object to search by GPA (lt, gt, eq)</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid query supplied</response>
        /// <response code="404">No students found</response>
        [HttpGet]
        [Route("/students")]
        [SwaggerOperation(OperationId = "GetStudents")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Student>), description: "Successful operation")]
        public virtual IActionResult GetStudents([FromQuery (Name = "name")]string? name, [FromQuery] GPAQuery? GPAQuery)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<Student>));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string exampleJson = null;
            exampleJson = "[ {\r\n  \"name\" : \"Garry Peterson\",\r\n  \"GPA\" : 3.9,\r\n  \"sId\" : 1234\r\n}, {\r\n  \"name\" : \"Garry Peterson\",\r\n  \"GPA\" : 3.9,\r\n  \"sId\" : 1234\r\n} ]";
            
            var example = exampleJson != null
            ? JsonSerializer.Deserialize<List<Student>>(exampleJson)
            : default(List<Student>);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
