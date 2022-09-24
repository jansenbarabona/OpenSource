using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class DealerModel
    {
        //public DealerModel()
        //{
        //    Dealer = new List<MAIType>();
        //}
        //public MAI MAI { get; set; }

        //
        [DisplayName("Dealer")]
        public int DealerID { get; set; }
        //
        [DisplayName("Dealer Name")]
        [Required(ErrorMessage = "The Full Name field is required.")]
        public string DealerName { get; set; }
        //
        [DisplayName("Email Address")]
        [EmailAddress]
        [Required(ErrorMessage = "The Email field is required.")]
        public string EmailAddress { get; set; }
        //
        [DisplayName("Secondary Email")]
        [EmailAddress]
        [Required(ErrorMessage = "The Email field is required.")]
        public string EmailAddress2 { get; set; }
        //
        [DisplayName("Business Phone")]
        [Required(ErrorMessage = "The Business Phone field is required.")]
        public string BusinessPhone { get; set; }
        //
        [DisplayName("Mobile Phone")]
        //[Required(ErrorMessage = "The Mobile Phone field is required.")]
        public string MobilePhone { get; set; }
        //
        [DisplayName("FaxNumber")]
        //[Required(ErrorMessage = "The Fax Number field is required.")]
        public string FaxNumber { get; set; }
        //
        [DisplayName("TIN")]
        [Required(ErrorMessage = "The TIN field is required.")]
        public string TIN { get; set; }
        //
        [DisplayName("Website")]
        //[Required(ErrorMessage = "The Website field is required.")]
        public string Website { get; set; }
        //
        [DisplayName("Address")]
        [Required(ErrorMessage = "The Address field is required.")]
        public string Address { get; set; }
        //
        [DisplayName("Select City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int CityID { get; set; }

        //
        [DisplayName("Zip Code")]
        [Required(ErrorMessage = "The Zip Code field is required.")]
        public string ZipCode { get; set; }
        //
        [DisplayName("Logo")]
        public string Logo { get; set; }
        //
        [DisplayName("Logo")]
        public byte[] LogoByte { get; set; }
        //
        [DisplayName("Notes")]
        public string Notes { get; set; }
        //
        [DisplayName("Created By")]
        public int CreatedBy { get; set; }
        //
        [DisplayName("Created Date")]
        public System.DateTime? CreatedDate { get; set; }
        //
        [DisplayName("Active")]
        public bool Active { get; set; }
        //
        [DisplayName("Select City")]
        public List<City> CityList { get; set; }
        //
        [DisplayName("Select City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int SelectedCityID { get; set; }

        //
        [DisplayName("Barangay")]
        [Required(ErrorMessage = "The Barangay field is required.")]
        public int BarangayID { get; set; }
        [DisplayName("Barangay")]
        public string BarangayName { get; set; }
        [DisplayName("Select Barangay")]
        public List<Barangay> BarangayList { get; set; }
        //
        [DisplayName("Logo")]
        //[Required(ErrorMessage = "The Logo File field is required.")]
        public HttpPostedFileBase LogoFile { get; set; }

        public List<vwDealerInsuranceModel> vwDealerInsuranceModelList { get; set; }
        //
        [DisplayName("Select Province")]
        public List<Province> ProvinceList { get; set; }
        //
        [DisplayName("Select Province")]
        [Required(ErrorMessage = "The Province field is required.")]
        public int SelectedProvinceID { get; set; }
        //
        public string ProvinceName { get; set; }
        //
        public WalletModel WalletDetail { get; set; }
        //
        public List<vwTransactionEntityList> EntityTransaction { get; set; }
        //
        public string CityName { get; set; }
    }

    public class DealerBranchModel
    {
        //
        [DisplayName("Dealer Branch")]
        [Required(ErrorMessage = "The Email field is required.")]
        public int DealerBranchID { get; set; }
        //
        [DisplayName("Dealer")]
        [Required(ErrorMessage = "The Dealer field is required.")]
        public int DealerID { get; set; }
        //
        [DisplayName("Dealer Branch Name")]
        [Required(ErrorMessage = "The Dealer Branch Name field is required.")]
        public string DealerBranchName { get; set; }
        //
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "The Email field is required.")]
        public string EmailAddress { get; set; }
        //
        [DisplayName("Business Phone")]
        [Required(ErrorMessage = "The Business Phone field is required.")]
        public string BusinessPhone { get; set; }
        //
        [DisplayName("Mobile Phone")]
        [Required(ErrorMessage = "The Mobile Phone is required.")]
        public string MobilePhone { get; set; }
        //
        [DisplayName("Fax Number")]
        [Required(ErrorMessage = "The Fax Number field is required.")]
        public string FaxNumber { get; set; }
        //
        [DisplayName("WebSite")]
        [Required(ErrorMessage = "The Website field is required.")]
        public string WebSite { get; set; }
        //
        [DisplayName("Address")]
        [Required(ErrorMessage = "The Address field is required.")]
        public string Address { get; set; }
        //
        [DisplayName("Select City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int CityID { get; set; }
        //
        [DisplayName("Zip Code")]
        [Required(ErrorMessage = "The Zip Code field is required.")]
        public string ZipCode { get; set; }
        //
        [DisplayName("Accreditation Number")]
        [Required(ErrorMessage = "The Accreditation Number field is required.")]
        public string AccreditationNumber { get; set; }
        //
        [DisplayName("Upload Version")]
        public int? UploadVersion { get; set; }
        //
        [DisplayName("Customer")]
        public int CreatedBy { get; set; }
        //
        [DisplayName("Created Date")]
        public System.DateTime CreatedDate { get; set; }
        //
        [DisplayName("Active")]
        public bool Active { get; set; }
        //
        [DisplayName("Barangay")]
        [Required(ErrorMessage = "The Barangay field is required.")]
        public int BarangayID { get; set; }
        [DisplayName("Barangay")]
        public string BarangayName { get; set; }
        [DisplayName("Select Barangay")]
        public List<Barangay> BarangayList { get; set; }
        //
        [DisplayName("Select City")]
        public List<City> CityList { get; set; }
        //
        [DisplayName("Select Province")]
        public List<Province> ProvinceList { get; set; }
        //
        [DisplayName("Dealer")]
        public List<Dealer> DealerList { get; set; }
        //
        [DisplayName("Select City")]
        public int SelectedCityID { get; set; }
        //
        [DisplayName("Select Province")]
        public int ProvinceID { get; set; }
        //
        [DisplayName("Logo")]
        public HttpPostedFileBase LogoFile { get; set; }
        //
        [DisplayName("Main Branch")]
        public bool IsMain { get; set; }
    }

    public class DealerInvoiceModel
    {
        public DealerInvoiceModel()
        {
            VehicleInfo = new vwVehicleList();
            VehicleTypeList = new List<VehicleTypeList>();
            VehicleClassificationList = new List<VehicleClassification>();
            TaxTypeList = new List<CTPLTaxType>();
        }
        public int InvoiceID { get; set; }
        public int DealerBranchID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerFullName { get; set; }
        [DisplayName("Invoice Number")]
        [Required(ErrorMessage = "The Invoice Number field is required.")]
        public string InvoiceNumber { get; set; }
        private DateTime _dateNow = DateTime.Now;
        [DisplayName("Invoice Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime InvoiceDate
        {
            get { return _dateNow; }
            set { _dateNow = value; }
        }
        public byte[] InvoiceByte { get; set; }
        [DisplayName("COC")]
        public string COC { get; set; }
        public byte[] COCByte { get; set; }
        [DisplayName("COC Authentication")]
        public string COCAuthenticationCode { get; set; }
        public List<CustomerModel> CustomerList { get; set; }
        public List<Dealer> DealerList { get; set; }
        public List<DealerBranch> DealerBranchList { get; set; }
        [DisplayName("Customer")]
        public int SelectedCustomerID { get; set; }
        public int SelectedDealer { get; set; }
        public int SelectedDealerBranchID { get; set; }
        [DisplayName("Invoice Image")]
        //[Required(ErrorMessage = "The COC File field is required.")]
        public HttpPostedFileBase InvoiceFile { get; set; }
        [DisplayName("COC Image")]
        public HttpPostedFileBase COCFile { get; set; }
        public int VehicleID { get; set; }
        //
        [DisplayName("Vehicle ID")]
        public int SelectedVehicleID { get; set; }
        public List<vwVehicleList> VehicleList { get; set; }
        [DisplayName("Cost of Vehicle")]
        [Required(ErrorMessage = "The Cost of Vehicle field is required.")]
        public decimal VehicleCost { get; set; }
        [DisplayName("Preferred Ending Plate Number")]
        public string PreferredEndingPlateNumber { get; set; }
        public vwVehicleList VehicleInfo { get; set; }
        public string InvoiceContentType { get; set; }
        public string COCContentType { get; set; }
        [DisplayName("MV Type")]
        public int VehicleTypeID { get; set; }
        [DisplayName("Premium Classification")]
        public int VehicleClassificationID { get; set; }
        [DisplayName("Tax Type")]
        public int TaxTypeID { get; set; }
        public string VehicleClassificationName { get; set; }
        public List<VehicleTypeList> VehicleTypeList { get; set; }
        public List<VehicleClassification> VehicleClassificationList { get; set; }
        public List<CTPLTaxType> TaxTypeList { get; set; }
        public string FormOrigin { get; set; }
        [DisplayName("CoC Policy Number")]
        public string COCPolicyNumber { get; set; }

        public string InvoiceImage
        {
            get
            {
                var base64 = "";
                if (InvoiceByte == null)
                {
                    base64 = "";
                }
                else
                {
                    base64 = Convert.ToBase64String(InvoiceByte);
                }
                return String.Format("data:" + InvoiceContentType + ";base64,{0}", base64);
            }
        }
        public Nullable<System.DateTime> COCInceptionDate { get; set; }
        public Nullable<System.DateTime> COCExpirationDate { get; set; }
        [DisplayName("Affidavit File")]
        //[Required(ErrorMessage = "This field is required.")]
        public HttpPostedFileBase AffidavitOfConversionFile { get; set; }
        public string AffidavitOfConversion { get; set; }
        public byte[] AffidavitOfConversionByte { get; set; }
        public string AffidavitOfConversionContentType { get; set; }
        [DisplayName("Encumbered")]
        public bool Encumbered { get; set; }
        [DisplayName("Financial Institution")]
        [Required]
        public string FinancialInstitution { get; set; }
    }

    public class DealerInsuranceModel
    {
        public int DealerInsuranceID { get; set; }
        [DisplayName("Dealer ID")]
        public int DealerID { get; set; }
        [DisplayName("Dealer Name")]
        public string DealerName { get; set; }
        [DisplayName("Insurance ID")]
        public int InsuranceID { get; set; }
        public string InsuranceName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        //
        [DisplayName("isChecked")]
        public bool isChecked { get; set; }

        public List<DealerInsuranceModel> DealerInsuranceModelList { get; set; }
        public List<vwDealerInsuranceModel> vwDealerInsuranceModeList { get; set; }
    }
    public class vwDealerInsuranceModel
    {
        public int DealerInsuranceID { get; set; }
        [DisplayName("Dealer ID")]
        public int DealerID { get; set; }
        [DisplayName("Dealer Name")]
        public int DealerName { get; set; }
        [DisplayName("Insurance ID")]
        public int InsuranceID { get; set; }
        [DisplayName("Insurance Name")]
        public string InsuranceName { get; set; }
        public bool AutoGenerateCOC { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
    }

    public class DealerTimeLineReportModel
    {
        public List<Dealer> DealerList { get; set; }

        [DisplayName("Dealer")]
        [Required]
        public int SelectedDealerID { get; set; }
        [DisplayName("Date Submitted From")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? DateFrom { get; set; }
        [DisplayName("Date Submitted To")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? DateTo { get; set; }
    }

    public class VRReportModel
    {
        public List<Dealer> DealerList { get; set; }

        [DisplayName("Dealer")]
        [Required]
        public int SelectedDealerID { get; set; }

        [DisplayName("Branch")]
        [Required]
        public int SelectedDealerBranchID { get; set; }

        public int DealerBranchID { get; set; }

        [DisplayName("Date Submitted From")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? DateFrom { get; set; }
        [DisplayName("Date Submitted To")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? DateTo { get; set; }
        public List<DealerBranch> DealerBranchList { get; internal set; }
    }

    public class vwDealerInvoiceModel
    {
        public int InvoiceID { get; set; }
        public int DealerBranchID { get; set; }
        public int CustomerID { get; set; }
        public string InvoiceNumber { get; set; }
        public byte[] InvoiceByte { get; set; }
        public string COC { get; set; }
        public bool Active { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int TitleID { get; set; }
        public int? TitleTypeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string CorpName { get; set; }
        public string TitleName { get; set; }
        public string TitleAbbreviation { get; set; }
        public string DealerBranchName { get; set; }
        public int DealerID { get; set; }
        public string DealerName { get; set; }
        public byte[] COCByte { get; set; }
        public Nullable<int> InsuranceID { get; set; }
        public string InsuranceName { get; set; }
        public int VehicleID { get; set; }
        public string CustomerName
        {
            get
            {
                var _CustomerName = "";
                if (TitleTypeID == 2)
                {
                    _CustomerName = CorpName;
                }
                else
                {
                    _CustomerName = LastName + ", " + FirstName + " " + MiddleName;
                }
                return _CustomerName;
            }
        }
    }
}