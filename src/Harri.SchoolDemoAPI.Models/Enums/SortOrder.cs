using System.Runtime.Serialization;

namespace Harri.SchoolDemoAPI.Controllers
{
    /// <summary>
    /// ASC or DESC
    /// </summary>
    public enum SortOrder
    {
        [EnumMember(Value = "ASC")]
        ASC,

        [EnumMember(Value = "DESC")]
        DESC
    }
}
