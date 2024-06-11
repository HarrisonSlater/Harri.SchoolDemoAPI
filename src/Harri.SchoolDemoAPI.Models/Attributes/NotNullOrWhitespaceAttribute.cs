using System;
using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotNullOrWhitespaceAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }
    }
}
