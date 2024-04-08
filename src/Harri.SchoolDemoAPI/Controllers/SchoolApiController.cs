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
using System.Text.Json;

namespace Harri.SchoolDemoAPI.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class SchoolApiController : ControllerBase
    {
        /// <summary>
        /// Add a new school
        /// </summary>
        /// <remarks>Add a new school</remarks>
        /// <param name="newSchool">Create a new school</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        [Route("/school")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation(OperationId = "AddSchool")]
        [SwaggerResponse(statusCode: 200, type: typeof(int), description: "Successful operation")]
        public virtual IActionResult AddSchool([FromBody]NewSchool newSchool)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(int));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "1001";
            
            var example = exampleJson != null
            ? JsonSerializer.Deserialize<int>(exampleJson)
            : default(int);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Get a school
        /// </summary>
        /// <remarks>Get a school by schoolId</remarks>
        /// <param name="schoolId">ID of school to return</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">School not found</response>
        [HttpGet]
        [Route("/school/{schoolId}")]
        [ValidateModelState]
        [SwaggerOperation(OperationId = "GetSchool")]
        [SwaggerResponse(statusCode: 200, type: typeof(School), description: "Successful operation")]
        public virtual IActionResult GetSchool([FromRoute(Name = "schoolId")][Required] int schoolId)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(School));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string exampleJson = null;
            exampleJson = "{\r\n  \"schoolId\" : 1001,\r\n  \"state\" : \"VIC\",\r\n  \"schoolName\" : \"Melbourne Future School\",\r\n  \"enrollment\" : 20000\r\n}";

            var example = exampleJson != null
            ? JsonSerializer.Deserialize<School>(exampleJson)
            : default(School);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Update an existing school
        /// </summary>
        /// <remarks>Update an existing school by Id</remarks>
        /// <param name="school">Update an existent school</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">School not found</response>
        [HttpPut]
        [Route("/school")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation(OperationId = "UpdateSchool")]
        public virtual IActionResult UpdateSchool([FromBody] School school)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a school
        /// </summary>
        /// <remarks>Delete a school by schoolId</remarks>
        /// <param name="schoolId">ID of school to delete</param>
        /// <response code="200">Successful operation</response>
        /// <response code="409">Conflict. School has applications which must be deleted first</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">School not found</response>
        [HttpDelete]
        [Route("/school/{schoolId}")]
        [ValidateModelState]
        [SwaggerOperation(OperationId = "DeleteSchool")]
        public virtual IActionResult DeleteSchool([FromRoute (Name = "schoolId")][Required]int schoolId)
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
    }
}
