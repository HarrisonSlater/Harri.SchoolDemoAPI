using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Harri.SchoolDemoAPI.Models.Enums;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class SchoolDto
    {
        /// <summary>
        /// Gets or Sets SchoolId
        /// </summary>
        /// <example>1001</example>
        [Required]
        [JsonPropertyName(APIConstants.School.SchoolId)]
        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or Sets SchoolName
        /// </summary>
        /// <example>Melbourne Future School</example>
        [JsonPropertyName(APIConstants.School.SchoolName)]
        public string? SchoolName { get; set; }

        /// <summary>
        /// Gets or Sets State
        /// </summary>
        [JsonPropertyName(APIConstants.School.State)]
        public State? State { get; set; }

        /// <summary>
        /// Gets or Sets Enrollment
        /// </summary>
        /// <example>20000</example>
        [JsonPropertyName(APIConstants.School.Enrollment)]
        public int? Enrollment { get; set; }

    }
}
