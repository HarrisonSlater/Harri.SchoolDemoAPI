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
    public class GPAQueryDto
    {
        /// <summary>
        /// GPA Query DTO 
        /// </summary>
        [JsonPropertyName(APIConstants.Student.GPA)]
        public ComparativeQueryDto<decimal>? GPA { get; set; }

    }
}
