
namespace NDAWebPortal.Core.EnumLib
{
    public enum AdmissionStates : int
    {
        [EnumDisplayName(DisplayName = "Pre-Bank Confirmed")]
        PreBank = 1,
        [EnumDisplayName(DisplayName = "Bank Confirmed")]
        Bank,
        [EnumDisplayName(DisplayName = "Post-Bank Confirmed")]
        PostBank,
        [EnumDisplayName(DisplayName = "School Approved")]
        SchoolApproved,
        [EnumDisplayName(DisplayName = "School Denied")]
        SchoolDenied,
        [EnumDisplayName(DisplayName = "School Approval Pending")]
        SchoolApprovalPending
    }
}
