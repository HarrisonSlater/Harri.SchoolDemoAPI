using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using Harri.SchoolDemoAPI.Attributes;
using Harri.SchoolDemoAPI.Models;

namespace Harri.SchoolDemoAPI.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class StudentApiController : ControllerBase
    { 
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
        [ValidateModelState]
        [SwaggerOperation("AddStudent")]
        [SwaggerResponse(statusCode: 200, type: typeof(int), description: "Successful operation")]
        public virtual IActionResult AddStudent([FromBody]NewStudent newStudent)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(int));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "1234";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<int>(exampleJson)
            : default(int);
            //TODO: Change the data returned
            return new ObjectResult(example);
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
        [ValidateModelState]
        [SwaggerOperation("DeleteStudent")]
        public virtual IActionResult DeleteStudent([FromRoute (Name = "sId")][Required]int sId)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 409 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(409);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            throw new NotImplementedException();
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
        [ValidateModelState]
        [SwaggerOperation("GetStudent")]
        [SwaggerResponse(statusCode: 200, type: typeof(Student), description: "Successful operation")]
        public virtual IActionResult GetStudent([FromRoute (Name = "sId")][Required]int sId)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Student));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string exampleJson = null;
            exampleJson = "{\r\n  \"name\" : \"Garry Peterson\",\r\n  \"GPA\" : 3.9,\r\n  \"sId\" : 1234\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<Student>(exampleJson)
            : default(Student);
            //TODO: Change the data returned
            return new ObjectResult(example);
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
        [ValidateModelState]
        [SwaggerOperation("UpdateStudent")]
        public virtual IActionResult UpdateStudent([FromBody]Student student)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            throw new NotImplementedException();
        }
    }
}
