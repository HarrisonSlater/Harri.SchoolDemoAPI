using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    public class PositiveInt : RangeAttribute
    {
        public PositiveInt() : base(0, int.MaxValue)
        {
            ErrorMessage = "int should be positive";
        }
    }
}
