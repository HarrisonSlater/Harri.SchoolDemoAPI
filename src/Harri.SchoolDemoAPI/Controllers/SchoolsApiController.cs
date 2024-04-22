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
using Harri.SchoolDemoAPI.Models.Enums;

namespace Harri.SchoolDemoAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class SchoolsApiController : ControllerBase
    {
        /// <summary>
        /// Get schools
        /// </summary>
        /// <remarks>Get schools by query</remarks>
        /// <param name="name">Name partial of schools to search on. Case insensitive</param>
        /// <param name="state">State to match on</param>
        /// <param name="enrollmentQuery">query object to search by enrollment (lt, gt, eq)</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid query supplied</response>
        /// <response code="404">No schools found</response>
        [HttpGet]
        [Route("/schools")]
        [SwaggerOperation(OperationId = "GetSchools")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<SchoolDto>), description: "Successful operation")]
        public virtual IActionResult GetSchools([FromQuery (Name = "name")]string? name, [FromQuery (Name = "state")]State? state, [FromQuery]EnrollmentQueryDto? enrollmentQuery)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<School>));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string exampleJson = null;
            exampleJson = "[ {\r\n  \"schoolId\" : 1001,\r\n  \"state\" : \"VIC\",\r\n  \"schoolName\" : \"Melbourne Future School\",\r\n  \"enrollment\" : 20000\r\n}, {\r\n  \"schoolId\" : 1001,\r\n  \"state\" : \"VIC\",\r\n  \"schoolName\" : \"Melbourne Future School\",\r\n  \"enrollment\" : 20000\r\n} ]";
            
            var example = exampleJson != null
            ? JsonSerializer.Deserialize<List<SchoolDto>>(exampleJson)
            : default(List<SchoolDto>);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
