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
    
    public partial class DealerBranch
    {
        public int DealerBranchID { get; set; }
        public int DealerID { get; set; }
        public string DealerBranchName { get; set; }
        public string EmailAddress { get; set; }
        public string BusinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string FaxNumber { get; set; }
        public string WebSite { get; set; }
        public string Address { get; set; }
        public int CityID { get; set; }
        public string ZipCode { get; set; }
        public string AccreditationNumber { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public bool IsMain { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int BarangayID { get; set; }
        public Nullable<int> UploadVersion { get; set; }
    }
}
