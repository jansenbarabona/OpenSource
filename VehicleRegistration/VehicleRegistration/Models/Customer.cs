//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VehicleRegistration.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public int CustomerID { get; set; }
        public int DealerID { get; set; }
        public int TitleID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string CorpName { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string SexCode { get; set; }
        public string CivilStatusCode { get; set; }
        public string Citizenship { get; set; }
        public string HouseBldgNumber { get; set; }
        public string StreetSubdivision { get; set; }
        public string Barangay { get; set; }
        public int CityID { get; set; }
        public string ZipCode { get; set; }
        public string Height { get; set; }
        public System.DateTime Birthdate { get; set; }
        public string Birthplace { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string TIN { get; set; }
        public string AdditionalAddress { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationAddress { get; set; }
        public string OrganizationTIN { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonNumber { get; set; }
        public string ClientReferenceNumber { get; set; }
        public bool Active { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Weight { get; set; }
        public string Alias { get; set; }
    }
}
