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
    public class ApplicationsApiController : ControllerBase
    { 
        /// <summary>
        /// Get applications
        /// </summary>
        /// <remarks>Get applications by query</remarks>
        /// <param name="sId">ID of student to match applications on</param>
        /// <param name="schoolId">ID of school to match applications on</param>
        /// <param name="major">Major to match exactly</param>
        /// <param name="decision">decision to match on. May be &#39;Y&#39;, &#39;N&#39; or null</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid query supplied</response>
        /// <response code="404">No applications found</response>
        [HttpGet]
        [Route("/applications")]
        [ValidateModelState]
        [SwaggerOperation("GetApplications")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Application>), description: "Successful operation")]
        public virtual IActionResult GetApplications([FromQuery (Name = "sId")]int? sId, [FromQuery (Name = "schoolId")]int? schoolId, [FromQuery (Name = "major")]string? major, [FromQuery (Name = "decision")]Decision? decision)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<Application>));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string exampleJson = null;
            exampleJson = "[ {\r\n  \"major\" : \"Computer Science\",\r\n  \"decision\" : \"Y\",\r\n  \"schoolId\" : 1001,\r\n  \"applicationId\" : 876581,\r\n  \"sId\" : 1234\r\n}, {\r\n  \"major\" : \"Computer Science\",\r\n  \"decision\" : \"Y\",\r\n  \"schoolId\" : 1001,\r\n  \"applicationId\" : 876581,\r\n  \"sId\" : 1234\r\n} ]";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<List<Application>>(exampleJson)
            : default(List<Application>);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
