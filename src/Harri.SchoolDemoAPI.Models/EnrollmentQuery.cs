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
    /// TODO
    /// </summary>
    public class EnrollmentQuery
    {
        /// <summary>
        /// less than
        /// </summary>
        /// <example>3.5</example>
        [DataMember(Name = APIConstants.School.Enrollment)]
        [JsonPropertyName(APIConstants.School.Enrollment)]
        [DisplayName(APIConstants.School.Enrollment)]
        [ModelBinder(Name = APIConstants.School.Enrollment)]
        public ComparativeQuery<int>? Enrollment { get; set; }

    }
}
