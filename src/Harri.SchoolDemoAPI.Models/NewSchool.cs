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
    public class NewSchool 
    {
        /// <summary>
        /// Gets or Sets SchoolName
        /// </summary>
        /// <example>Melbourne Future School</example>
        [DataMember(Name = APIConstants.School.SchoolName)]
        [JsonPropertyName(APIConstants.School.SchoolName)]
        [DisplayName(APIConstants.School.SchoolName)]
        [ModelBinder(Name = APIConstants.School.SchoolName)]
        public string? SchoolName { get; set; }

        /// <summary>
        /// Gets or Sets State
        /// </summary>
        [DataMember(Name = APIConstants.School.State)]
        [JsonPropertyName(APIConstants.School.State)]
        [DisplayName(APIConstants.School.State)]
        [ModelBinder(Name = APIConstants.School.State)]
        public State? State { get; set; }

        /// <summary>
        /// Gets or Sets Enrollment
        /// </summary>
        /// <example>20000</example>
        [DataMember(Name = APIConstants.School.Enrollment)]
        [JsonPropertyName(APIConstants.School.Enrollment)]
        [DisplayName(APIConstants.School.Enrollment)]
        [ModelBinder(Name = APIConstants.School.Enrollment)]
        public int? Enrollment { get; set; }

    }
}
