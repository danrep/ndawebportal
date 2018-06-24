namespace NDAWebPortal.Core.EnumLib
{
    public enum UserRoles : int
    {
        [EnumDisplayName(DisplayName = "System Administrator")]
        SystemAdministrator = 1,
        [EnumDisplayName(DisplayName = "Admissions")]
        Admissions,
        [EnumDisplayName(DisplayName = "Bank Representative")]
        Bank,
        [EnumDisplayName(DisplayName = "Students")]
        Students = 10
    }
}
