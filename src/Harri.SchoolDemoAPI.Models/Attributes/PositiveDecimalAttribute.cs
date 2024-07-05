using System;
using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    public class PositiveDecimalAttribute : ValidationAttribute
    {
        public PositiveDecimalAttribute() : base()
        {
            ErrorMessage = "Decimal should be positive";
        }

        public override bool IsValid(object value)
        {
            if (value is null) return true;

            var dec = (decimal)value;
            return dec > 0;
        }
    }
}
