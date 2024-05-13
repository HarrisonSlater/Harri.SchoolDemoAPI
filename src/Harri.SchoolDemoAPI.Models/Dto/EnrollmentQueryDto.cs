using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// TODO
    /// </summary>
    public class EnrollmentQueryDto
    {
        /// <summary>
        /// less than
        /// </summary>
        /// <example>3.5</example>
        [JsonPropertyName(APIConstants.School.Enrollment)]
        public ComparativeQueryDto<int>? Enrollment { get; set; }

    }
}
