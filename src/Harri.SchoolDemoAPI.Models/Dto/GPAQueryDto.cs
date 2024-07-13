using Harri.SchoolDemoAPI.Models.Attributes;
using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// Wrapper DTO for quering on student GPA
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
