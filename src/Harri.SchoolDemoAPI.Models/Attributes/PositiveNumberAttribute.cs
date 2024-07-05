using System.ComponentModel.DataAnnotations;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    public class PositiveNumberAttribute : PositiveDecimalAttribute
    {
        public PositiveNumberAttribute() : base()
        {
            ErrorMessage = "Number should be positive";
        }
    }
}
