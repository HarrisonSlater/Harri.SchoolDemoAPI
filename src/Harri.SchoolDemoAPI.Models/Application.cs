using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Harri.SchoolDemoAPI.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Application 
    {
        /// <summary>
        /// Gets or Sets ApplicationId
        /// </summary>
        /// <example>876581</example>
        [Required]
        [DataMember(Name= APIConstants.Application.ApplicationId)]
        [JsonPropertyName(APIConstants.Application.ApplicationId)]
        [DisplayName(APIConstants.Application.ApplicationId)]
        [ModelBinder(Name = APIConstants.Application.ApplicationId)]

        public int? ApplicationId { get; set; }

        /// <summary>
        /// Gets or Sets SId
        /// </summary>
        /// <example>1234</example>
        [DataMember(Name = APIConstants.Student.SId)]
        [JsonPropertyName(APIConstants.Student.SId)]
        [DisplayName(APIConstants.Student.SId)]
        [ModelBinder(Name = APIConstants.Student.SId)]

        public int? SId { get; set; }

        /// <summary>
        /// Gets or Sets SchoolId
        /// </summary>
        /// <example>1001</example>
        [DataMember(Name = APIConstants.School.SchoolId)]
        [JsonPropertyName(APIConstants.School.SchoolId)]
        [DisplayName(APIConstants.School.SchoolId)]
        [ModelBinder(Name = APIConstants.School.SchoolId)]

        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or Sets Major
        /// </summary>
        /// <example>Computer Science</example>
        [DataMember(Name = APIConstants.Application.Major)]
        [JsonPropertyName(APIConstants.Application.Major)]
        [DisplayName(APIConstants.Application.Major)]
        [ModelBinder(Name = APIConstants.Application.Major)]

        public string? Major { get; set; }

        /// <summary>
        /// Gets or Sets Decision
        /// </summary>
        [DataMember(Name = APIConstants.Application.Decision)]
        [JsonPropertyName(APIConstants.Application.Decision)]
        [DisplayName(APIConstants.Application.Decision)]
        [ModelBinder(Name = APIConstants.Application.Decision)]
        public Decision? Decision { get; set; }

    }
}
