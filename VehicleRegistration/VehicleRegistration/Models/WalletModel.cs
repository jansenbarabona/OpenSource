using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class WalletModel
    {
        public WalletModel()
        {
            TransactionList = new List<spEntityWallet_Result>();
        }
        public decimal AvailableBalance { get; set; }
        public List<spEntityWallet_Result> TransactionList { get; set; }
        public string AvailableBalanceStr
        {
            get
            {
                return AvailableBalance.ToString("#,##0.00");
            }
        }
        public decimal Threshold { get; set; }
        public decimal ThresholdInput { get; set; }
        public int UserEntityID { get; set; }
        public int ReferenceID { get; set; }
        public int SubReferenceID { get; set; }
    }
    public class Receipt
    {
        public Receipt()
        {
            merchant_code = "0623202001";
        }
        public string merchant_code { get; set; }
        public string trans_id { get; set; }
        public string account_no { get; set; }
        public string payor_name { get; set; }
        public string address { get; set; }
        public string TIN { get; set; }
        public string transaction_type { get; set; }
        public string payment_option { get; set; }
        public string email { get; set; }
        public string amount { get; set; }
        public string special_discount { get; set; }
        public string discount_id { get; set; }
        public string particulars { get; set; }
    }
    public class ReceiptResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public string merchantname { get; set; }
        public string or_number { get; set; }
        public string transid { get; set; }
    }
}