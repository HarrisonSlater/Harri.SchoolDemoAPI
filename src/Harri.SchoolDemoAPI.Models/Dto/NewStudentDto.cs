using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class NewStudentDto
    {
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        /// <example>Garry Peterson</example>
        [Required]
        [JsonPropertyName(APIConstants.Student.Name)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or Sets GPA
        /// </summary>
        /// <example>3.9</example>
        [JsonPropertyName(APIConstants.Student.GPA)]
        public decimal? GPA { get; set; }

    }
}
