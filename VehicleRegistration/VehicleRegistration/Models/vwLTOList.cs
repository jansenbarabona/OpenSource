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
    
    public partial class vwLTOList
    {
        public int LTOID { get; set; }
        public string LTOName { get; set; }
        public string EmailAddress { get; set; }
        public string EmailAddress2 { get; set; }
        public string BusinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string FaxNumber { get; set; }
        public string TIN { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public int ProvinceID { get; set; }
        public int CityID { get; set; }
        public int BarangayID { get; set; }
        public string ZipCode { get; set; }
        public string Logo { get; set; }
        public byte[] LogoByte { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
    }
}
