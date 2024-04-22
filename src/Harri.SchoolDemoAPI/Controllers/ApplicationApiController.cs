using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using Harri.SchoolDemoAPI.Models.Dto;

namespace Harri.SchoolDemoAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class ApplicationApiController : ControllerBase
    {
        /// <summary>
        /// Add a new application
        /// </summary>
        /// <remarks>Add a new application</remarks>
        /// <param name="newApplication">Create a new application</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        [Route("/application")]
        [Consumes("application/json")]
        [SwaggerOperation(OperationId = "AddApplication")]
        [SwaggerResponse(statusCode: 200, type: typeof(int), description: "Successful operation")]
        public virtual IActionResult AddApplication([FromBody]NewApplicationDto newApplication)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(int));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "876581";
            
            var example = exampleJson != null
            ? JsonSerializer.Deserialize<int>(exampleJson)
            : default(int);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Get an application
        /// </summary>
        /// <remarks>Get an application by applicationId</remarks>
        /// <param name="applicationId">ID of application to return</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Application not found</response>
        [HttpGet]
        [Route("/application/{applicationId}")]
        [SwaggerOperation(OperationId = "GetApplication")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApplicationDto), description: "Successful operation")]
        public virtual IActionResult GetApplication([FromRoute (Name = "applicationId")][Required]int applicationId)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Application));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string exampleJson = null;
            exampleJson = "{\r\n  \"major\" : \"Computer Science\",\r\n  \"decision\" : \"Y\",\r\n  \"schoolId\" : 1001,\r\n  \"applicationId\" : 876581,\r\n  \"sId\" : 1234\r\n}";
            
            var example = exampleJson != null
            ? JsonSerializer.Deserialize<ApplicationDto>(exampleJson)
            : default(ApplicationDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Update an existing application
        /// </summary>
        /// <remarks>Update an existing application by Id</remarks>
        /// <param name="application">Update an existent application</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">Application not found</response>
        [HttpPut]
        [Route("/application")]
        [Consumes("application/json")]
        [SwaggerOperation(OperationId = "UpdateApplication")]
        public virtual IActionResult UpdateApplication([FromBody] ApplicationDto application)
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
        /// Delete an application
        /// </summary>
        /// <remarks>Delete an application by applicationId</remarks>
        /// <param name="applicationId">ID of application to delete</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Application not found</response>
        [HttpDelete]
        [Route("/application/{applicationId}")]
        [SwaggerOperation(OperationId = "DeleteApplication")]
        public virtual IActionResult DeleteApplication([FromRoute(Name = "applicationId")][Required] int applicationId)
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
