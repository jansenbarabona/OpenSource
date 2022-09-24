using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{

    using System;
    using System.Collections.Generic;

    public class TransactionModel
    {
        /// <summary>
        /// wallet controllers.. / dropdownlist
      
        public List<Wallet> Walletlist { get; set; }

        public List<TransactionType> TransactionTypelist { get; set; }
        //public List<spEntityWallet_Result> TransactionList { get; set; }
        public WalletModel WalletDetail { get; set; }
        [DisplayName("Entity Type")]
        public List<UserEntity> UserEntityTypeList { get; set; }


        /// </summary>

        [DisplayName("Transaction ID")]
        public int TransactionID { get; set; }

        [DisplayName("Wallet ID")]
        public int WalletID { get; set; }
      

        [DisplayName("Transaction Type")]
        [Required(ErrorMessage = " field is required")]
        public int TransactionTypeID { get; set; }

        [DisplayName("Amount")]
        [Required(ErrorMessage = " field is required")]
        public decimal Amount { get; set; }

        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsCancelled { get; set; }

        [Required(ErrorMessage = " field is required")]
        public int SelectedUserEntityID { get; set; }

        [DisplayName("Entity")]
        [Required(ErrorMessage = " field is required")]
        public int SelectedEntityID { get; set; }
        public string Remarks { get; set; }
        public int VehicleID { get; set; }
        
    }

}
