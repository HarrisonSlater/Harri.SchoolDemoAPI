using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{ 
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
        public decimal? GPA { get; set; }

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
