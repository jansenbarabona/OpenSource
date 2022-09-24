using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VehicleRegistration.Models
{
    public partial class TransactionEntityTypeModel
    {
   

        [DisplayName("User Entity ID")]
        public int UserEntityID { get; set; }
        [DisplayName("Transaction Entity Type ID")]
        public int TransactionEntityTypeID { get; set; }


        /// <summary>
        /// TransactionTypeID no view list
        /// </summary>
        [DisplayName("Transaction Type ID")]
        public int TransactionTypeID { get; set; }

        [DisplayName("Amount")]
        public decimal Amount { get; set; }

        [DisplayName("Effectivity Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime EffectivityDate { get; set; }


        public string Active { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        /// <summary>
        ///  Dropdown
        /// </summary>
        ///
        [DisplayName("User Entity List")]
        public List<UserEntity> UserEntityList { get; set; }
        [DisplayName("Transaction Type List")]
        public List<TransactionType> TransactionTypelist { get; set; }
        
    }

    public class TransactionEntityModel : TransactionEntityTypeModel
    {
        public int TransactionEntityID { get; set; }
        public int EntityID { get; set; }
        //public string UserEntityName { get; set; }
        //public string TransactionName { get; set; }
        //public string EntityName { get; set; }

        public List<MAIModel> MAIList { get; set; }
        public List<DealerModel> DealerList { get; set; }
        public List<InsuranceModel> InsuranceList { get; set; }
    }

    public class TransactionEntityBranchModel : TransactionEntityModel
    {
        public int TransactionEntityBranchID { get; set; }
        public int BranchID { get; set; }
        public List<DealerBranchModel> DealerBranchList { get; set; }
        public List<InsuranceBranchModel> InsuranceBranchList { get; set; }

    }
}