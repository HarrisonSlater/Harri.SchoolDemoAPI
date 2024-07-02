using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    public class PositiveDecimalAttribute : RangeAttribute
    {
        public PositiveDecimalAttribute() : base(1, double.MaxValue)
        {
            ErrorMessage = "Decimal should be positive";
        }
    }
}
