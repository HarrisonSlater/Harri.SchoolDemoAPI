using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Harri.SchoolDemoAPI.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Harri.SchoolDemoAPI.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    public class StudentDto 
    {
        /// <summary>
        /// Gets or Sets SId
        /// </summary>
        /// <example>1234</example>
        /// 
        [JsonPropertyName(APIConstants.Student.SId)]
        public int? SId { get; set; }

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
