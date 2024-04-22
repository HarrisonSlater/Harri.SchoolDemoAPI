using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Harri.SchoolDemoAPI.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    public class UpdateStudentDto 
    {
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        /// <example>Garry Peterson</example>
        [Required]
        [JsonPropertyName(APIConstants.Student.Name)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or Sets GPA
        /// </summary>
        /// <example>3.9</example>
        [JsonPropertyName(APIConstants.Student.GPA)]
        public decimal? GPA { get; set; }

    }
}
