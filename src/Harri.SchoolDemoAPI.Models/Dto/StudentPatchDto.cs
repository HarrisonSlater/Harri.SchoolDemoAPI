using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Harri.SchoolDemoAPI.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    public class StudentPatchDto
    {
        [OptionalNotNullOrWhitespace]
        [JsonIgnore]
        [JsonPropertyName(APIConstants.Student.Name)] // Set for use in validation error message
        public Optional<string?> OptionalName { get; set; }
        [JsonIgnore]
        public Optional<decimal?> OptionalGPA { get; set; }

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

        //TODO use custom json converter for this?
        public object GetObjectToSerialize()
        {
            var obj = new ExpandoObject() as IDictionary<string, Object>;
            if (OptionalName.HasValue) 
            { 
                obj.Add(APIConstants.Student.Name, OptionalName.Value); 
            }

            if (OptionalGPA.HasValue)
            {
                obj.Add(APIConstants.Student.GPA, OptionalGPA.Value);
            }

            return obj;
        }
    }
}
