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
    /// 
    /// </summary>
    /// 
    [DataContract]
    public class ComparativeQuery<T> where T : struct // struct required to fix .net framework 4.8 warnings
    {
        /// <summary>
        /// less than
        /// </summary>
        [DataMember(Name="lt", EmitDefaultValue=true)]
        [JsonPropertyName("lt")]

        //[FromQuery(Name="lt")]
        public T? Lt { get; set; }

        /// <summary>
        /// greater than
        /// </summary>
        [DataMember(Name="gt", EmitDefaultValue=true)]
        [JsonPropertyName("gt")]

        //[FromQuery(Name = "gt")]
        public T? Gt { get; set; }

        /// <summary>
        /// equal to
        /// </summary>
        [DataMember(Name="eq", EmitDefaultValue=true)]
        [JsonPropertyName("eq")]

        //[FromQuery(Name = "eq")]
        public T? Eq { get; set; }
    }
}
