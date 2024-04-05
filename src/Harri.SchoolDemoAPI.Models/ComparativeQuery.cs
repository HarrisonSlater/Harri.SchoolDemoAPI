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
    [DataContract]
    public class ComparativeQuery<T>
    {
        /// <summary>
        /// less than
        /// </summary>
        [DataMember(Name="lt", EmitDefaultValue=true)]
        [FromQuery(Name="lt")]
        public T? Lt { get; set; }

        /// <summary>
        /// greater than
        /// </summary>
        [DataMember(Name="gt", EmitDefaultValue=true)]
        [FromQuery(Name = "gt")]
        public T? Gt { get; set; }

        /// <summary>
        /// equal to
        /// </summary>
        [DataMember(Name="eq", EmitDefaultValue=true)]
        [FromQuery(Name = "eq")]
        public T? Eq { get; set; }

    }
}
