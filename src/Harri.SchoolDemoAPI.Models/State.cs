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
        /// List of valid states
        /// </summary>
        [TypeConverter(typeof(CustomEnumConverter<State>))]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum State
        {
            
            [EnumMember(Value = "QLD")]
            QLD = 1,
            
            [EnumMember(Value = "NSW")]
            NSW = 2,

            [EnumMember(Value = "VIC")]
            VIC = 3,

            [EnumMember(Value = "ACT")]
            ACT = 4,

            [EnumMember(Value = "TAS")]
            TAS = 5,

            [EnumMember(Value = "WA")]
            WA = 6,

            [EnumMember(Value = "NT")]
            NT = 7,

            [EnumMember(Value = "SA")]
            SA = 8
        }
}
