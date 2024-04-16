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
    [DataContract]
    public class NewStudent 
    {
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        /// <example>Garry Peterson</example>
        [Required]
        [DataMember(Name = APIConstants.Student.Name)]
        [JsonPropertyName(APIConstants.Student.Name)]
        [DisplayName(APIConstants.Student.Name)]
        [ModelBinder(Name = APIConstants.Student.Name)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or Sets GPA
        /// </summary>
        /// <example>3.9</example>
        [DataMember(Name = APIConstants.Student.GPA)]
        [JsonPropertyName(APIConstants.Student.GPA)]
        [DisplayName(APIConstants.Student.GPA)]
        [ModelBinder(Name = APIConstants.Student.GPA)]
        public decimal? GPA { get; set; }

    }
}
