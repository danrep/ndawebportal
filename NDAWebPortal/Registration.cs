//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NDAWebPortal
{
    using System;
    using System.Collections.Generic;
    
    public partial class Registration
    {
        public long Id { get; set; }
        public string RegistrationGuid { get; set; }
        public string AdmissionId { get; set; }
        public int CredentialId { get; set; }
        public int SessionId { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string Religion { get; set; }
        public string MaritalStatus { get; set; }
        public string PhysicalFeatures { get; set; }
        public string Nationality { get; set; }
        public string HomeAddress { get; set; }
        public string StateOfOrigin { get; set; }
        public string LgaOfOrigin { get; set; }
        public string HomeTown { get; set; }
        public string LandMarks { get; set; }
        public string BloodType { get; set; }
        public string Genotype { get; set; }
        public string Hobbies { get; set; }
        public string ForceFullName { get; set; }
        public string ForceRelationship { get; set; }
        public string ForceUnit { get; set; }
        public string SponsorFullName { get; set; }
        public string SponsorRelationship { get; set; }
        public string SponsorEmail { get; set; }
        public string SponsorPhoneNumber { get; set; }
        public Nullable<long> SponsorYearlyIncome { get; set; }
        public string NokFullName { get; set; }
        public string NokRelationship { get; set; }
        public string NokEmail { get; set; }
        public string NokPhoneNumber { get; set; }
        public string PgFullName { get; set; }
        public string PgRelationship { get; set; }
        public string PgEmail { get; set; }
        public string PgPhoneNumber { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> FirstChoiceProgramme { get; set; }
        public Nullable<int> FirstChoiceSubProgramme { get; set; }
        public Nullable<int> SecondChoiceProgramme { get; set; }
        public Nullable<int> SecondChoiceSubProgramme { get; set; }
    }
}
