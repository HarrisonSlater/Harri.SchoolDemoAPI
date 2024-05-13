using System.Runtime.Serialization;

namespace Harri.SchoolDemoAPI.Models.Enums
{
    /// <summary>
    /// List of valid decisions a school can make on an application
    /// </summary>
    public enum Decision
    {

        [EnumMember(Value = "Y")]
        Y = 1,

        [EnumMember(Value = "N")]
        N = 2
    }
}
