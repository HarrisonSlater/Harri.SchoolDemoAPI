using System.Runtime.Serialization;

namespace Harri.SchoolDemoAPI.Models.Enums
{
    /// <summary>
    /// Ascending or descending
    /// </summary>
    public enum SortOrder
    {
        [EnumMember(Value = "ASC")]
        ASC,

        [EnumMember(Value = "DESC")]
        DESC
    }
}
