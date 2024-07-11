using Harri.SchoolDemoAPI.Models.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OptionalNotNullOrWhitespaceAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var optionalobj = (Optional<string?>) value;
            if (optionalobj.HasValue) {
                return !string.IsNullOrWhiteSpace(optionalobj.Value);
            }
            else
            {
                return true;
            }
        }
    }
}
