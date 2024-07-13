using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json.Serialization;
using Harri.SchoolDemoAPI.Models.Attributes;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    /// <summary>
    /// DTO for patching a student
    /// </summary>
    /// Only properties that are explicitly set are serialized using the <see cref="GetObjectToSerialize"/> method
    public class StudentPatchDto
    {
        [OptionalNotNullOrWhitespace]
        [JsonIgnore]
        [JsonPropertyName(APIConstants.Student.Name)] // Set for use in validation error message
        public Optional<string?> OptionalName { get; private set; }
        [JsonIgnore]
        public Optional<decimal?> OptionalGPA { get; private set; }

        /// <summary>
        /// Student name
        /// </summary>
        /// <example>Garry Peterson</example>
        [JsonPropertyName(APIConstants.Student.Name)]
        public string? Name
        {
            get => OptionalName.Value;
            set => OptionalName = value;
        }

        /// <summary>
        /// Student Grade Point Average
        /// </summary>
        /// <example>3.9</example>
        [JsonPropertyName(APIConstants.Student.GPA)]
        [PositiveDecimal]
        [DecimalPrecision(3, 2)]
        public decimal? GPA
        {
            get => OptionalGPA.Value;
            set => OptionalGPA = value;
        }

        public void ApplyChangesTo(StudentDto student)
        {
            if (OptionalName.HasValue) 
            {
                student.Name = OptionalName.Value;
            }

            if (OptionalGPA.HasValue)
            {
                student.GPA = OptionalGPA.Value;
            }
        }

        public object GetObjectToSerialize()
        {
            var obj = new ExpandoObject() as IDictionary<string, object>;
            if (OptionalName.HasValue) 
            { 
                obj.Add(APIConstants.Student.Name, OptionalName.Value!); 
            }

            if (OptionalGPA.HasValue)
            {
                obj.Add(APIConstants.Student.GPA, OptionalGPA.Value!);
            }

            return obj;
        }
    }
}
