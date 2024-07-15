using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// DTO for querying students by any property
    /// </summary>
    public class StudentQueryDto
    {

        [JsonPropertyName(APIConstants.Student.Name)]
        public string? Name { get; set; }

        /// <summary>
        /// GPA Query DTO 
        /// </summary>
        [JsonPropertyName(APIConstants.Student.GPA)]
        public ComparativeQueryNullableDto<decimal>? GPA { get; set; }

    }
}