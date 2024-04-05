using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Harri.SchoolDemoAPI.Converters;

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
        [DataMember(Name="applicationId")]
        public int? ApplicationId { get; set; }

        /// <summary>
        /// Gets or Sets SId
        /// </summary>
        /// <example>1234</example>
        [DataMember(Name="sId")]
        public int? SId { get; set; }

        /// <summary>
        /// Gets or Sets SchoolId
        /// </summary>
        /// <example>1001</example>
        [DataMember(Name="schoolId")]
        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or Sets Major
        /// </summary>
        /// <example>Computer Science</example>
        [DataMember(Name="major")]
        public string? Major { get; set; }

        /// <summary>
        /// Gets or Sets Decision
        /// </summary>
        [DataMember(Name="decision")]
        public Decision? Decision { get; set; }

    }
}
