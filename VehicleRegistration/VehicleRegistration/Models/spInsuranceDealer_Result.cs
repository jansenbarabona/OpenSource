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
    
    public partial class spInsuranceDealer_Result
    {
        public int InsuranceID { get; set; }
        public string InsuranceName { get; set; }
        public Nullable<int> DealerID { get; set; }
        public string DealerName { get; set; }
        public string EmailAddress { get; set; }
        public string BusinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string FaxNumber { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public Nullable<int> CityID { get; set; }
        public string ZipCode { get; set; }
        public string Logo { get; set; }
        public byte[] LogoByte { get; set; }
        public string Notes { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}