using System.Text.Json.Serialization;
using Harri.SchoolDemoAPI.Models.Enums;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class NewApplicationDto
    {
        /// <summary>
        /// Gets or Sets SId
        /// </summary>
        /// <example>1234</example>
        [JsonPropertyName(APIConstants.Student.SId)]
        public int? SId { get; set; }

        /// <summary>
        /// Gets or Sets SchoolId
        /// </summary>
        /// <example>1001</example>
        [JsonPropertyName(APIConstants.School.SchoolId)]
        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or Sets Major
        /// </summary>
        /// <example>Computer Science</example>
        [JsonPropertyName(APIConstants.Application.Major)]
        public string? Major { get; set; }

        /// <summary>
        /// Gets or Sets Decision
        /// </summary>
        [JsonPropertyName(APIConstants.Application.Decision)]
        public Decision? Decision { get; set; }
    }
}
