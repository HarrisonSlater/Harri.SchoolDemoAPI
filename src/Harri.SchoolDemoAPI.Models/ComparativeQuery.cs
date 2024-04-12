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
    /// 
    [DataContract]
    public class ComparativeQuery<T> where T : struct // struct required to fix .net framework 4.8 warnings
    {
        /// <summary>
        /// less than
        /// </summary>
        [DataMember(Name = APIConstants.Query.Lt)]
        [JsonPropertyName(APIConstants.Query.Lt)]
        [DisplayName(APIConstants.Query.Lt)]
        [ModelBinder(Name = APIConstants.Query.Lt)]
        public T? Lt { get; set; }

        /// <summary>
        /// greater than
        /// </summary>
        [DataMember(Name = APIConstants.Query.Gt)]
        [JsonPropertyName(APIConstants.Query.Gt)]
        [DisplayName(APIConstants.Query.Gt)]
        [ModelBinder(Name = APIConstants.Query.Gt)]
        public T? Gt { get; set; }

        /// <summary>
        /// equal to
        /// </summary>
        [DataMember(Name = APIConstants.Query.Eq)]
        [JsonPropertyName(APIConstants.Query.Eq)]
        [DisplayName(APIConstants.Query.Eq)]
        [ModelBinder(Name = APIConstants.Query.Eq)]
        public T? Eq { get; set; }
    }
}
