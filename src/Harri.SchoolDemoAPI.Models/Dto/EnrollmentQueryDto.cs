using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// TODO
    /// </summary>
    public class EnrollmentQueryDto
    {
        /// <summary>
        /// less than
        /// </summary>
        /// <example>3.5</example>
        [JsonPropertyName(APIConstants.School.Enrollment)]
        public ComparativeQueryDto<int>? Enrollment { get; set; }

    }
}
