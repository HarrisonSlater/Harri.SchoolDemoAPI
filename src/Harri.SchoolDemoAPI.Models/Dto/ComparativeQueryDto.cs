using Harri.SchoolDemoAPI.Models.Attributes;
using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class ComparativeQueryDto<T> where T : struct // struct constraint required to fix .net framework 4.8 warnings
    {
        /// <summary>
        /// Less than
        /// </summary>
        [PositiveNumber]
        [JsonPropertyName(APIConstants.Query.Lt)]
        public T? Lt { get; set; }

        /// <summary>
        /// Greater than
        /// </summary>
        [JsonPropertyName(APIConstants.Query.Gt)]
        public T? Gt { get; set; }

        /// <summary>
        /// Equal to
        /// </summary>
        [PositiveNumber]
        [JsonPropertyName(APIConstants.Query.Eq)]
        public T? Eq { get; set; }
    }
}
