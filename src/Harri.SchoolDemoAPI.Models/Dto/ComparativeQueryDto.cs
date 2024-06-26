using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// 
    /// </summary>
    //TODO Validation for negative numbers
    public class ComparativeQueryDto<T> where T : struct // struct required to fix .net framework 4.8 warnings
    {
        /// <summary>
        /// Less than
        /// </summary>
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
        [JsonPropertyName(APIConstants.Query.Eq)]
        public T? Eq { get; set; }
    }
}
