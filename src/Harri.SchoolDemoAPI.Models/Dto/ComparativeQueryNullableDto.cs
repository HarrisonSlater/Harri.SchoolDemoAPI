using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    public class ComparativeQueryNullableDto<T> : ComparativeQueryDto<T> where T : struct
    {
        /// <summary>
        /// Query on null value
        /// </summary>
        [JsonPropertyName(APIConstants.Query.IsNull)]
        public bool? IsNull { get; set; }
    }
}
