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
    
    public partial class City
    {
        public int CityID { get; set; }
        public int ProvinceID { get; set; }
        public string CityName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string CityNumber { get; set; }
    }
}
