using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    public class NonNegativeInt : RangeAttribute
    {
        public NonNegativeInt() : base(0, int.MaxValue)
        {
            ErrorMessage = "    ";
        }
    }
}
