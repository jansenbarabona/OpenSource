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
    
    public partial class UserEntity
    {
        public int UserEntityID { get; set; }
        public string UserEntityName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
    }
}
