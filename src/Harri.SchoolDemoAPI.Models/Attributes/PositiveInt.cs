using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
