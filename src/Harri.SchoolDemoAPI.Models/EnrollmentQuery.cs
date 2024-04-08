using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
        [DataMember(Name="enrollment", EmitDefaultValue=true)]
        [JsonPropertyName("enrollment")]
        //[FromQuery(Name = "enrollment")]
        public ComparativeQuery<int>? Enrollment { get; set; }

    }
}
