using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class TransactionTypeModel
    {
        public int TransactionTypeID { get; set; }
        [DisplayName("Transaction Name")]
        public string TransactionName { get; set; }
        public bool Type { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        [DisplayName("Debit")]
        public bool IsDebit { get; set; }
        public bool IsActive { get; set; }
        public int TransactionEntryTypeID { get; set; }
        [DisplayName("Entry Type")]
        public List<TransactionEntryType> EntryTypeList { get; set; }
    }
}