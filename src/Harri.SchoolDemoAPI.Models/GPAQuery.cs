using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Harri.SchoolDemoAPI.Converters;
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
        [DataMember(Name="GPA", EmitDefaultValue=true)]
        [FromQuery(Name = "GPA")]
        public ComparativeQuery<decimal>? GPA { get; set; }

    }
}
