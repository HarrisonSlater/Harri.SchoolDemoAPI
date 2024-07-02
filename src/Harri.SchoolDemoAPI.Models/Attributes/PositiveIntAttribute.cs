using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    public class PositiveIntAttribute : RangeAttribute
    {
        public PositiveIntAttribute() : base(1, int.MaxValue)
        {
            ErrorMessage = "Integer should be positive";
        }
    }
}
