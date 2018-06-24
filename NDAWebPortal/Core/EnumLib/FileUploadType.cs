
namespace NDAWebPortal.Core.EnumLib
{
    public enum FileUploadType : int
    {
        [EnumDisplayName(DisplayName = "Passport Photograph")]
        PassportPort = 1,
        [EnumDisplayName(DisplayName = "Signature")]
        Signature,
        [EnumDisplayName(DisplayName = "Certificate of State of Origin")]
        StateOfOrigin,
        [EnumDisplayName(DisplayName = "Certificate of Birth")]
        BirthCertificate,
        [EnumDisplayName(DisplayName = "Affidavit of Declaration of Age")]
        Affidavit,
        [EnumDisplayName(DisplayName = "PG Form 1B: Candidate Report")]
        PgForm1B,
        [EnumDisplayName(DisplayName = "PG Form 1C: Statement on Proposed Area of Research")]
        PgForm1C,
        [EnumDisplayName(DisplayName = "PG Form 2C: Medical Report")]
        PgForm2C,
        [EnumDisplayName(DisplayName = "PG Form 1E: Acknowledgement for Admission")]
        PgForm1E,
        [EnumDisplayName(DisplayName = "PG Form 1D: Application for Transcript")]
        PgForm1D
    }
}
