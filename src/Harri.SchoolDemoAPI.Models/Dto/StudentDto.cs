using Harri.SchoolDemoAPI.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{ 
    /// <summary>
    /// Response DTO for a complete student
    /// </summary>
    public class StudentDto
    {
        /// <summary>
        /// Student ID
        /// </summary>
        /// <example>1234</example>
        [Required]
        [JsonPropertyName(APIConstants.Student.SId)]
        public int? SId { get; set; }

        /// <summary>
        /// Student name
        /// </summary>
        /// <example>Garry Peterson</example>
        [Required]
        [JsonPropertyName(APIConstants.Student.Name)]
        public string? Name { get; set; }

        /// <summary>
        /// Student Grade Point Average
        /// </summary>
        /// <example>3.9</example>
        [JsonPropertyName(APIConstants.Student.GPA)]
        [PositiveDecimal]
        [DecimalPrecision(3, 2)]
        public decimal? GPA { get; set; }

        [JsonIgnore]
        public byte[]? RowVersion { get; set; }

        public UpdateStudentDto AsUpdateStudentDto()
        {
            return new UpdateStudentDto()
            {
                Name = Name,
                GPA = GPA,
            };
        }
    }
}
