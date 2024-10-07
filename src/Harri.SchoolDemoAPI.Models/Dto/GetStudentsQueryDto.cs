using Harri.SchoolDemoAPI.Models.Attributes;
using Harri.SchoolDemoAPI.Models.Attributes.SortColumn;
using Harri.SchoolDemoAPI.Models.Enums;
using System.Text.Json.Serialization;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// DTO for querying students by any property, and ordering on any property
    /// </summary>
    public class GetStudentsQueryDto
    {
        public int? SID;

        [JsonPropertyName(APIConstants.Student.SId)]
        public int? SId { get; set; }

        [JsonPropertyName(APIConstants.Student.Name)]
        public string? Name { get; set; }

        [JsonPropertyName(APIConstants.Query.GpaQuery)]
        public GPAQueryDto? GPAQueryDto { get; set; }

        [JsonPropertyName(APIConstants.Query.OrderBy)]
        public SortOrder? OrderBy { get; set; }

        [JsonPropertyName(APIConstants.Query.SortColumn)]
        [ValidStudentSortColumn]
        public string? SortColumn { get; set; }

        [JsonPropertyName(APIConstants.Query.Page)]
        [PositiveInt]
        public int? Page { get; set; }

        [JsonPropertyName(APIConstants.Query.PageSize)]
        [PositiveInt]
        public int? PageSize { get; set; }
    }
}