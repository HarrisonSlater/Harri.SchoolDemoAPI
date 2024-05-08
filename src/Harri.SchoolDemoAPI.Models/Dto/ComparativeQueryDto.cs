using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// 
    /// </summary>
    //TODO Validation for negative numbers
    public class ComparativeQueryDto<T> where T : struct // struct required to fix .net framework 4.8 warnings
    {
        /// <summary>
        /// less than
        /// </summary>
        [JsonPropertyName(APIConstants.Query.Lt)]
        public T? Lt { get; set; }

        /// <summary>
        /// greater than
        /// </summary>
        [JsonPropertyName(APIConstants.Query.Gt)]
        public T? Gt { get; set; }

        /// <summary>
        /// equal to
        /// </summary>
        [JsonPropertyName(APIConstants.Query.Eq)]
        public T? Eq { get; set; }
    }
}
