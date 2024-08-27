using Harri.SchoolDemoAPI.Models.Attributes;
using Harri.SchoolDemoAPI.Models.Enums;
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

        [JsonPropertyName(APIConstants.Query.GpaQuery)]
        public GPAQueryDto? GPAQueryDto { get; set; }

        [JsonPropertyName(APIConstants.Query.OrderBy)]
        public SortOrder? OrderBy { get; set; }
    }
}