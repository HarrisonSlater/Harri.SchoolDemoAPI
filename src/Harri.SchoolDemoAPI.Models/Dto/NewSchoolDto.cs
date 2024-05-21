using System.Text.Json.Serialization;
using Harri.SchoolDemoAPI.Models.Enums;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class NewSchoolDto
    {
        /// <summary>
        /// Gets or Sets SchoolName
        /// </summary>
        /// <example>Melbourne Future School</example>
        [JsonPropertyName(APIConstants.School.SchoolName)]
        public string? SchoolName { get; set; }

        /// <summary>
        /// Gets or Sets State
        /// </summary>
        [JsonPropertyName(APIConstants.School.State)]
        public State? State { get; set; }

        /// <summary>
        /// Gets or Sets Enrollment
        /// </summary>
        /// <example>20000</example>
        [JsonPropertyName(APIConstants.School.Enrollment)]
        public int? Enrollment { get; set; }

    }
}
