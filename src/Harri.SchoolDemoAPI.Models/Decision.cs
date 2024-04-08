using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Harri.SchoolDemoAPI.Models
{ 
        /// <summary>
        /// List of valid decisions a school can make on an application
        /// </summary>
        public enum Decision
        {
            
            [EnumMember(Value = "Y")]
            Y = 1,

            [EnumMember(Value = "N")]
            N = 2
        }
}
