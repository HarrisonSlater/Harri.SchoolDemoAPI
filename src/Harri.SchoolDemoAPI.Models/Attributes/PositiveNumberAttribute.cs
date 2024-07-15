using System;
using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PositiveNumberAttribute : PositiveDecimalAttribute
    {
        public PositiveNumberAttribute() : base()
        {
            ErrorMessage = "Number should be positive";
        }
    }
}
