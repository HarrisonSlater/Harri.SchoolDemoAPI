using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Harri.SchoolDemoAPI.Models.Attributes.SortColumn
{
    /// <summary>
    /// Attribute for validating a string of a student column to sort on 
    /// </summary>
    // TODO Generalise this attribute + unit tests
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class ValidStudentSortColumnAttribute : ValidationAttribute
    {
        private static string[] ValidStrings = new string[] 
        {
            APIConstants.Student.SId,
            APIConstants.Student.Name,
            APIConstants.Student.GPA
        };

        public override bool IsValid(object? value)
        {
            var sortColumn = (string?)value;

            if (string.IsNullOrWhiteSpace(sortColumn)) return false;

            return ValidStrings.Contains(sortColumn, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
