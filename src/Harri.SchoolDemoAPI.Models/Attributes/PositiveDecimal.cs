using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    public class PositiveDecimal : RangeAttribute
    {
        public PositiveDecimal() : base(0, double.MaxValue)
        {
            ErrorMessage = "decimal should be positive";
        }
    }
}
