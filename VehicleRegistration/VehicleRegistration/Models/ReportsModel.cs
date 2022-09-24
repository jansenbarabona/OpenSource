using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class LTOStatusReportModel
    {
        public LTOStatusReportModel()
        {
            DealerList = new List<Dealer>();
            DealerBranchList = new List<DealerBranchModel>();
            TableList = new List<LTOStatusReportTableModel>();
            BatchList = new List<LTOBatchHeader>();
            VehicleList = new List<LTOBatchDetailVehicle>();
        }
        [DisplayName("Dealer")]
        public int SelectedDealerID { get; set; }
        [DisplayName("Dealer Branch")]
        public int SelectedDealerBranchID { get; set; }
        public List<Dealer> DealerList { get; set; }
        public List<DealerBranchModel> DealerBranchList { get; set; }
        public List<LTOStatusReportTableModel> TableList { get; set; }
        public List<LTOBatchHeader> BatchList { get; set; }
        public List<LTOBatchDetailVehicle> VehicleList { get; set; }
    }
    public class LTOStatusReportTableModel
    {
        public int DealerID { get; set; }
        public int? DealerBranchID { get; set; }
        public string DealerName { get; set; }
        public string DealerBranchName { get; set; }
        public int TotalSubmitted { get; set; }
        public int TotalAssessment { get; set; }
        public int TotalPaid { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalActive
        {
            get
            {
                var totalcount = TotalSubmitted + TotalAssessment + TotalPaid + TotalCompleted;
                return totalcount;
            }
        }
    }
    public class DealerStatusReportModel
    {
        public DealerStatusReportModel()
        {
            DealerList = new List<DealerModel>();
            DealerBranchList = new List<DealerBranchModel>();
            TableList = new List<DealerStatusReportTableModel>();
            BatchList = new List<LTOBatchHeader>();
            VehicleList = new List<LTOBatchDetailVehicle>();
        }
        public string DealerName { get; set; }
        [DisplayName("Dealer")]
        public int SelectedDealerID { get; set; }
        [DisplayName("Dealer Branch")]
        public int SelectedDealerBranchID { get; set; }
        public List<DealerModel> DealerList { get; set; }
        public List<DealerBranchModel> DealerBranchList { get; set; }
        public List<DealerStatusReportTableModel> TableList { get; set; }
        public List<LTOBatchHeader> BatchList { get; set; }
        public List<LTOBatchDetailVehicle> VehicleList { get; set; }
    }
    public class DealerStatusReportTableModel
    {
        public int DealerID { get; set; }
        public int? DealerBranchID { get; set; }
        public string DealerName { get; set; }
        public string DealerBranchName { get; set; }
        public int TotalSubmitted { get; set; }
        public int TotalPayment { get; set; }
        public int TotalConfirmation { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalPickup { get; set; }
        public int TotalActive { get; set; }
    }
    public class DealerVehicleStatusReportModel
    {
        public DealerVehicleStatusReportModel()
        {
            DealerList = new List<Dealer>();
            DealerBranchList = new List<DealerBranchModel>();
            TableList = new List<DealerVehicleStatusReportTableModel>();
        }
        [DisplayName("Dealer")]
        public int SelectedDealerID { get; set; }
        [DisplayName("Dealer Branch")]
        public int SelectedDealerBranchID { get; set; }
        public List<Dealer> DealerList { get; set; }
        public List<DealerBranchModel> DealerBranchList { get; set; }
        public List<DealerVehicleStatusReportTableModel> TableList { get; set; }
    }
    public class DealerVehicleStatusReportTableModel
    {
        public string DealerBranchName { get; set; }
        public int TotalInventory { get; set; }
        public int TotalCSR { get; set; }
        public int TotalInvoice { get; set; }
        public int TotalCOC { get; set; }
        public int TotalPNP { get; set; }
        public int TotalSubmission { get; set; }
        public int TotalActive { get; set; }
    }
    public class CompanyListReportModel
    {
        public List<EntityList> EntityList { get; set; }

        [DisplayName("Entity")]
        [Required]
        public int SelectedEntityID { get; set; }
    }
    public class EntityModel
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string Email { get; set; }
        public string AccreditationNumber { get; set; }
        public bool Main { get; set; }
    }
    public class EntityList
    {
        public int EntityID { get; set; }
        public string EntityName { get; set; }
    }
    public class WalletReportModel
    {
        public WalletReportModel()
        {
            EntityList = new List<EntityList>();
            CompanyList = new List<CompanyList>();
            BranchList = new List<BranchList>();
            TransactionTypeList = new List<TransactionTypeList>();
        }
        public List<EntityList> EntityList { get; set; }
        public List<CompanyList> CompanyList { get; set; }
        public List<BranchList> BranchList { get; set; }
        public List<TransactionTypeList> TransactionTypeList { get; set; }

        [DisplayName("Entity")]
        [Required]
        public int SelectedEntityID { get; set; }
        [DisplayName("Company")]
        [Required]
        public int SelectedCompanyID { get; set; }
        [DisplayName("Branch")]
        [Required]
        public int SelectedBranchID { get; set; }
        [DisplayName("Transaction Type")]
        [Required]
        public int SelectedTransactionID { get; set; }
        [DisplayName("Period From")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? PeriodFrom { get; set; }
        [DisplayName("Period To")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? PeriodTo { get; set; }
    }
    public class CompanyList
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
    }
    public class BranchList
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
    }
    public class TransactionTypeList
    {
        public int TransactionTypeID { get; set; }
        public string TransactionTypeName { get; set; }
    }

    public class SOAReportModel
    {
        public SOAReportModel()
        {
            EntityList = new List<EntityList>();
            CompanyList = new List<CompanyList>();
            BranchList = new List<BranchList>();
        }
        public List<EntityList> EntityList { get; set; }
        public List<CompanyList> CompanyList { get; set; }
        public List<BranchList> BranchList { get; set; }

        [DisplayName("Entity")]
        [Required]
        public int SelectedEntityID { get; set; }
        [DisplayName("Company")]
        [Required]
        public int SelectedCompanyID { get; set; }
        [DisplayName("Branch")]
        [Required]
        public int SelectedBranchID { get; set; }
        [DisplayName("Period From")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? PeriodFrom { get; set; }
        [DisplayName("Period To")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? PeriodTo { get; set; }
    }
}