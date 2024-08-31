using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Harri.SchoolDemoAPI.Models.Attributes.SortColumn
{
    /// <summary>
    /// Attribute for validating a string of a student column to sort on 
    /// </summary>
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

            if (sortColumn is null) return true;

            return ValidStrings.Contains(sortColumn, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
