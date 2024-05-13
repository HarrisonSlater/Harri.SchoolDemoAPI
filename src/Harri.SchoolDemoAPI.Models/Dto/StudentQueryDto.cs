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
    /// 
    /// </summary>
    public class StudentQueryDto
    {

        [JsonPropertyName(APIConstants.Student.Name)]
        public string? Name { get; set; }

        /// <summary>
        /// GPA Query DTO 
        /// </summary>
        [JsonPropertyName(APIConstants.Student.GPA)]
        public ComparativeQueryNullableDto<decimal>? GPA { get; set; }

    }
}