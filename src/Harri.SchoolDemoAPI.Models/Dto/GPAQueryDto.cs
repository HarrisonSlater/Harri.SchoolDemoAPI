using Harri.SchoolDemoAPI.Models.Attributes;
using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class GPAQueryDto
    {
        /// <summary>
        /// GPA Query DTO 
        /// </summary>
        [JsonPropertyName(APIConstants.Student.GPA)]
        [ValidGPA]
        public ComparativeQueryNullableDto<decimal>? GPA { get; set; }

    }
}
