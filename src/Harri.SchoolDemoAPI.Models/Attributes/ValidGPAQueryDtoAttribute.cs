using Harri.SchoolDemoAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ValidGPAAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var gpa = (ComparativeQueryNullableDto<decimal>?)value;

            if (gpa is null) return true;

            if (gpa.Eq.HasValue && (gpa.Gt.HasValue || gpa.Lt.HasValue))
            {
                return false;
            }
            if (gpa.IsNull.HasValue && (gpa.Eq.HasValue || gpa.Gt.HasValue || gpa.Lt.HasValue))
            {
                return false;
            }
            return true;
        }
    }
}
