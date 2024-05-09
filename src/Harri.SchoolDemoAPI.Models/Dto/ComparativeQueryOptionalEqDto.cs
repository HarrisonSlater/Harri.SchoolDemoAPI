using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Harri.SchoolDemoAPI.Models.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// Allows you to differentiate between no parameter passed in or an explicit null for Eq property
    /// </summary>
    public class ComparativeQueryOptionalEqDto<T> where T : struct // struct required to fix .net framework 4.8 warnings
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

        [JsonPropertyName(APIConstants.Query.Eq)] // Set for use in validation error message
        [JsonIgnore]
        [BindNever]
        public Optional<T?> OptionalEq { get; private set; }

        /// <summary>
        /// equal to
        /// </summary>
        [JsonPropertyName(APIConstants.Query.Eq)]
        [ModelBinder(typeof(NullableQueryStringDecimalModelBinder))]
        public T? Eq
        {
            get => OptionalEq.Value;
            set => OptionalEq = value;
        }
    }
}
