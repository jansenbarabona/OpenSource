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
    
    public partial class Transaction
    {
        public int TransactionID { get; set; }
        public int WalletID { get; set; }
        public int TransactionTypeID { get; set; }
        public decimal Amount { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsCancelled { get; set; }
        public string Remarks { get; set; }
        public int VehicleID { get; set; }
    }
}
