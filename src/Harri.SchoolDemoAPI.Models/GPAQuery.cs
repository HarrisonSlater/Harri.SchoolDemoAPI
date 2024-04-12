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
    public class GPAQuery 
    {
        /// <summary>
        /// less than
        /// </summary>
        /// <example>3.5</example>
        [DataMember(Name = APIConstants.Student.GPA)]
        [JsonPropertyName(APIConstants.Student.GPA)]
        [DisplayName(APIConstants.Student.GPA)]
        [ModelBinder(Name = APIConstants.Student.GPA)]
        public ComparativeQuery<decimal>? GPA { get; set; }

    }
}
