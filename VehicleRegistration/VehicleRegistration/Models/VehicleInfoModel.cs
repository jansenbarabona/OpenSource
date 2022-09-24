using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel;
using VehicleRegistration.Tools;
using System.Security.RightsManagement;

namespace VehicleRegistration.Models
{
    public class VehicleInfoModel
    {
        public VehicleInfoModel()
        {
            MAIList = new List<MAI>();
            VehicleMakeList = new List<VehicleMake>();
            vwDealerVehicleMakeList = new List<vwDealerVehicleMake>();
            VehicleModelList = new List<VehicleModelModel>();
            VehicleColorList = new List<VehicleColor>();
            VehicleFuelTypeList = new List<VehicleFuelType>();
            DealerInsuranceList = new List<vwDealerInsuranceModel>();
            VehicleAirconTypeList = new List<AirconType>();
            BOCInfo = new BOCModel();
            CSRInfo = new CSRModel();
            COCInfo = new COCModel();
            PNPInfo = new PNPModel();
            BatchHeader = new BatchHeaderModel();
        }

        //
        [DisplayName("Vehicle")]
        public int VehicleID { get; set; }
        //
        [DisplayName("MAI")]
        public int MAIID { get; set; }
        //
        [DisplayName("Accredited MAI")]
        public string MAIName { get; set; }
        //
        [DisplayName("Dealer")]
        public Nullable<int> DealerID { get; set; }
        //
        [DisplayName("Dealer Branch")]
        public Nullable<int> DealerBranchID { get; set; }
        //
        [DisplayName("Dealer")]
        public string DealerName { get; set; }
        //
        [DisplayName("Branch")]
        public string DealerBranchName { get; set; }
        //
        [DisplayName("Vehicle Make")]
        public  int VehicleMakeID { get; set; }
        //
        [DisplayName("Vehicle Make")]
        public string VehicleMakeName { get; set; }
        //
        [DisplayName("Vehicle Model")]
        public Nullable<int> VehicleModelID { get; set; }
        //
        [DisplayName("Model")]
        public string VehicleModelName { get; set; }
        //
        [DisplayName("Engine Number")]
        [Required(ErrorMessage = "The Engine Number field is required.")]
        public string EngineNumber { get; set; }
        //
        [DisplayName("C.P. Number")]
        //[Required(ErrorMessage = "The CP Number field is required.")]
        public string CPNumber { get; set; }
        //
        [DisplayName("Date Issued")]
        //[Required(ErrorMessage = "The Date Issued field is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateIssued1 { get; set; }
        //
        [DisplayName("Chassis Number")]
        [Required(ErrorMessage = "The Chassis Number field is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string ChassisNumber { get; set; }
        //
        [DisplayName("C.P. Number")]
        //[Required(ErrorMessage = "The CP Number field is required.")]
        public string CPNumber2 { get; set; }
        //
        [DisplayName("Date Issued")]
        //[Required(ErrorMessage = "The Date Issued field is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateIssued2 { get; set; }
        //
        [DisplayName("Body ID Number")]
        [Required(ErrorMessage = "The Body ID Number field is required.")]
        public string BodyIDNumber { get; set; }
        //
        [DisplayName("BIRCCMV")]
        //[Required(ErrorMessage = "The BIR CCMV field is required.")]
        public string BIRCCMV { get; set; }
        //
        [DisplayName("Date Issued")]
        //[Required(ErrorMessage = "The Date Issued field is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateIssued3 { get; set; }
        //
        [DisplayName("Vehicle Color")]
        [Required(ErrorMessage = "The Vehicle Color field is required.")]
        public int VehicleColorID { get; set; }
        //
        [DisplayName("Vehicle Body Type")]
        [Required(ErrorMessage = "The Vehicle Body Type field is required.")]
        public int VehicleBodyTypeID { get; set; }
        //
        [DisplayName("Piston Displacement")]
        [Required(ErrorMessage = "The Piston Displacement field is required.")]
        public string PistonDisplacement { get; set; }
        //
        [DisplayName("Vehicle Fuel Type")]
        [Required(ErrorMessage = "The Vehicle Fuel field is required.")]
        public int VehicleFuelTypeID { get; set; }
        //
        [DisplayName("Cylinders")]
        [Required(ErrorMessage = "The Cylinders field is required.")]
        public string Cylinders { get; set; }
        //
        [DisplayName("Series")]
        //[Required(ErrorMessage = "The Series field is required.")]
        public string Series { get; set; }
        //
        [DisplayName("Year")]
        [Required(ErrorMessage = "The Year Tires field is required.")]
        public Nullable<int> Year { get; set; }
        //
        [DisplayName("Gross Vehicle Weight")]
        [Required(ErrorMessage = "The Vehicle Weight field is required.")]
        public Nullable<decimal> GrossVehicleWeight { get; set; }
        //
        [DisplayName("Front Tires Number")]
        [Required(ErrorMessage = "The Number of Front Tires field is required.")]
        public Nullable<int> FrontTiresNumber { get; set; }
        //
        [DisplayName("Rear Tires Number")]
        [Required(ErrorMessage = "The Number of Rear Tires field is required.")]
        public Nullable<int> RearTiresNumber { get; set; }
        //
        [DisplayName("Tax Type")]
        //[Required(ErrorMessage = "The Tax Type field is required.")]
        public string TaxType { get; set; }
        //
        [DisplayName("Tax Amount")]
        //[Required(ErrorMessage = "The Tax Amount field is required.")]
        public Nullable<decimal> TaxAmount { get; set; }
        //
        [DisplayName("Aircon Type")]
        [Required(ErrorMessage = "The Aircon Type field is required.")]
        public string AirconType { get; set; }
        //
        [DisplayName("Conduction Sticker")]
        [Required(ErrorMessage = "The Conduction Sticker field is required.")]
        public string ConductionSticker { get; set; }
        //
        [DisplayName("HPG Control Number")]
        //[Required(ErrorMessage = "The CoC Number field is required.")]
        public string HPGControlNumber { get; set; }
        //
        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        //
        [DisplayName("Date Prepared")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DatePrepared { get; set; }
        //
        [DisplayName("Transaction ID")]
        //[Required(ErrorMessage = "The Transaction ID field is required.")]
        public string TransactionID { get; set; }
        //
        [DisplayName("CSR Number")]
        //[Required(ErrorMessage = "The CRS Number field is required.")]
        public string CSRNumber { get; set; }
        //
        [DisplayName("Report Entry ID")]
        public string ReportEntryID { get; set; }
        //
        [DisplayName("Address")]
        //[Required(ErrorMessage = "The Address field is required.")]
        public string Address { get; set; }
        //
        [DisplayName("Report Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ReportDate { get; set; }
        //
        [DisplayName("Accreditation Number")]
        //[Required(ErrorMessage = "The Accreditation Number field is required.")]
        public string AccreditationNumber { get; set; }
        //
        [DisplayName("Item Type")]
        //[Required(ErrorMessage = "The Item Type field is required.")]
        public string ItemType { get; set; }
        //
        public string BOCContentType { get; set; }
        public string BOCContentType2 { get; set; }
        public string CSRContentType { get; set; }
        public string SOEContentType { get; set; }
        public string SOCContentType { get; set; }
        public string PNPContentType { get; set; }

        [DisplayName("BOC Certificate Of Payment")]
        public byte[] BOCCertificateOfPayment { get; set; }
        public string BOCImage
        {
            get
            {
                var base64 = Convert.ToBase64String(BOCCertificateOfPayment);
                return String.Format("data:" + BOCContentType + ";base64,{0}", base64);
            }
        }
        //
        [DisplayName("BOC Certificate Of Payment 2")]
        public byte[] BOCCertificateOfPayment2 { get; set; }
        public string BOCImage2
        {
            get
            {
                var base64 = Convert.ToBase64String(BOCCertificateOfPayment2);
                return String.Format("data:" + BOCContentType2 + ";base64,{0}", base64);
            }
        }
        //
        [DisplayName("Certificate Of Stock Report")]
        public byte[] CertificateOfStockReport { get; set; }
        public string CSRImage
        {
            get
            {
                var base64 = Convert.ToBase64String(CertificateOfStockReport);
                return String.Format("data:" + CSRContentType + ";base64,{0}", base64);
            }
        }
        //
        [DisplayName("Stencil Of Engine")]
        public byte[] StencilOfEngine { get; set; }
        public string SOEImage
        {
            get
            {
                var base64 = Convert.ToBase64String(StencilOfEngine);
                return String.Format("data:" + SOEContentType + ";base64,{0}", base64);
            }
        }
        //
        [DisplayName("Stencil Of Chasis")]
        public byte[] StencilOfChasis { get; set; }
        public string SOCImage
        {
            get
            {
                var base64 = Convert.ToBase64String(StencilOfChasis);
                return String.Format("data:" + SOCContentType + ";base64,{0}", base64);
            }
        }
        //
        [DisplayName("PNP Clearance")]
        public byte[] PNPClearance { get; set; }
        public string PNPImage
        {
            get
            {
                var base64 = Convert.ToBase64String(PNPClearance);
                return String.Format("data:" + PNPContentType + ";base64,{0}", base64);
            }
        }
        //
        [DisplayName("MAI")]
        public List<MAI> MAIList { get; set; }
        //
        [DisplayName("Dealer")]
        public List<Dealer> DealerList { get; set; }
        //
        [DisplayName("Dealer Branch")]
        public List<DealerBranch> DealerBranchList { get; set; }
        //
        [DisplayName("Vehicle Make")]
        public List<VehicleMake> VehicleMakeList { get; set; }
        //
        [DisplayName("Dealer Vehicle Make")]
        public List<vwMAIVehicleMake> vwMAIVehicleMakeList { get; set; }
        //
        [DisplayName("Dealer Vehicle Make")]
        public List<vwDealerVehicleMake> vwDealerVehicleMakeList { get; set; }
        //
        [DisplayName("Vehicle Model")]
        public List<VehicleModelModel> VehicleModelList { get; set; }
        //
        [DisplayName("Vehicle Color")]
        public List<VehicleColor> VehicleColorList { get; set; }
        //
        [DisplayName("Vehicle Fuel Type")]
        public List<VehicleFuelType> VehicleFuelTypeList { get; set; }
        //
        [DisplayName("Vehicle Body Type")]
        public List<VehicleBodyType> VehicleBodyTypeList { get; set; }
        //
        [DisplayName("Vehicle Aircon Type")]
        public List<AirconType> VehicleAirconTypeList { get; set; }
        //
        [DisplayName("MAI")]
        public int SelectedMAID { get; set; }
        //
        [DisplayName("Dealer")]
        //[Required(ErrorMessage = "The Dealer field is required.")]
        public int? SelectedDealer { get; set; }
        //
        [DisplayName("Dealer Branch")]
        //[Required(ErrorMessage = "The Dealer Branch field is required.")]
        public int? SelectedDealerBranch { get; set; }
        //
        [DisplayName("Vehicle Make")]
        //[Required(ErrorMessage = "The Vehicle Make field is required.")]
        public int? SelectedVehicleMakeID { get; set; }
        //
        [DisplayName("Vehicle Model")]
        //[Required(ErrorMessage = "The Vehicle Model field is required.")]
        public int? SelectedVehicleModelID { get; set; }
        //
        [DisplayName("Vehicle Color")]
        //[Required(ErrorMessage = "The Vehicle Color field is required.")]
        public int? SelectedVehicleColorID { get; set; }
        //
        [DisplayName("Color")]
        public string VehicleColorName { get; set; }
        //
        [DisplayName("Vehicle Body")]
        //[Required(ErrorMessage = "The Vehicle Body field is required.")]
        public int? SelectedVehicleBodyTypeID { get; set; }
        //
        [DisplayName("Body Type")]
        public string VehicleBodyTypeName { get; set; }
        //
        [DisplayName("Vehicle Fuel Type")]
        //[Required(ErrorMessage = "The Vehicle Color field is required.")]
        public int? SelectedVehicleFuelTypeID { get; set; }
        //
        [DisplayName("Fuel Type")]
        public string VehicleFuelName { get; set; }
        //
        [DisplayName("BOC File")]
        //[Required(ErrorMessage = "The BOC File field is required.")]
        public HttpPostedFileBase BOCFile { get; set; }
        public HttpPostedFileBase BOCFile2 { get; set; }
        //
        [DisplayName("BOC File 2")]
        //[Required(ErrorMessage = "The BOC File field is required.")]
        public HttpPostedFileBase BOC2File { get; set; }
        public HttpPostedFileBase BOC2File2 { get; set; }
        //
        [DisplayName("CSR File")]
        //[Required(ErrorMessage = "The CSR File field is required.")]
        public HttpPostedFileBase CSRFile { get; set; }
        public HttpPostedFileBase CSRFile2 { get; set; }
        //
        [DisplayName("Stencil of Engine File")]
        //[Required(ErrorMessage = "The Stencil of Engine File field is required.")]
        public HttpPostedFileBase SOEFile { get; set; }
        //
        [DisplayName("Stencil of Chasis File")]
        //[Required(ErrorMessage = "The Stencil of Chasis File field is required.")]
        public HttpPostedFileBase SOCFile { get; set; }
        //
        [DisplayName("PNP Clearance File")]
        //[Required(ErrorMessage = "The Stencil of Chasis File field is required.")]
        public HttpPostedFileBase PNPFile { get; set; }
        [DisplayName("LTO Submitted")]
        public bool LTOSubmitted { get; set; }
        [DisplayName("CSR Submitted")]
        public bool? CSRSubmitted { get; set; }
        [DisplayName("Allocate")]
        public bool Assigned { get; set; }

        //COCInfo
        [DisplayName("COC Number")]
        [Required(ErrorMessage = "The COC Number field is required.")]
        public string COCNo { get; set; }
        [DisplayName("COC Verification")]
        public bool CoCVerified { get; set; }
        [DisplayName("COC Authentication")]
        public string COCAuthentication { get; set; }
        [DisplayName("COC Policy Number")]
        public string COCPolicyNumber { get; set; }
        [DisplayName("COC Date")]
        public System.DateTime COCDate { get; set; }
        [DisplayName("COC File")]
        //[Required(ErrorMessage = "The COC File field is required.")]
        public HttpPostedFileBase COCFile { get; set; }
        [DisplayName("Certificate Of Conformity")]
        public byte[] CertificateOfConformity { get; set; }
        public string COCContentType { get; set; }
        public string COCImage
        {
            get
            {
                var base64 = Convert.ToBase64String(CertificateOfConformity);
                return String.Format("data:" + COCContentType + ";base64,{0}", base64);
            }
        }
        //

        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public List<vwDealerInsuranceModel> DealerInsuranceList { get; set; }
        [DisplayName("Insurance")]
        public int DealerInsuranceID { get; set; }
        public BOCModel BOCInfo { get; set; }
        public CSRModel CSRInfo { get; set; }
        public COCModel COCInfo { get; set; }
        public PNPModel PNPInfo { get; set; }
        public AssignDealerModel DealerInfo { get; set; }
        public DealerInvoiceModel InvoiceInfo { get; set; }

        //Batch Info
        public int? BatchID { get; set; }
        [DisplayName("Reference No")]
        public string BatchReferenceNo { get; set; }
        [DisplayName("Description")]
        public string BatchDescription { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsAssessed { get; set; }
        public bool IsPaid { get; set; }
        public bool IsBatchCompleted { get; set; }
        public BatchHeaderModel BatchHeader { get; set; }
        public string RejectedRemarks { get; set; }

        //Auto PNP
        public bool AutoPNP { get; set; }
        public string PNPReceiptReferenceNumber { get; set; }
        public byte[] PNPReceipt { get; set; }
        public string PNPReceiptContentType { get; set; }
        public string PNPReceiptImage
        {
            get
            {
                var base64 = Convert.ToBase64String(PNPReceipt);
                return String.Format("data:" + PNPReceiptContentType + ";base64,{0}", base64);
            }
        }
        //BatchDetail
        public bool isRejected { get; set; }
        public bool isCompleted { get; set; }
    }

    public class BOCModel
    {
        public BOCModel()
        {
            BatchHeader = new BatchHeaderModel();
        }
        //BOC 1
        [DisplayName("Engine C.P. Number")]
        [Required(ErrorMessage = "The CP Number field is required.")]
        public string CPNumber { get; set; }

        [DisplayName("Engine COP File")]
        [Required(ErrorMessage = "The COP File field is required.")]
        public HttpPostedFileBase BOCFile { get; set; }

        [DisplayName("Engine Date Issued")]
        [Required(ErrorMessage = "The Date Issued field is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateIssued1 { get; set; }
        [DisplayName("Engine Informal Entry Number")]
        public string InformalEntryNumberEngine { get; set; }

        //BOC2

        [DisplayName("Chassis C.P. Number")]
        public string CPNumber2 { get; set; }

        [DisplayName("Chassis Date Issued")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateIssued2 { get; set; }

        [DisplayName("Chassis BOC File 2")]
        public HttpPostedFileBase BOC2File { get; set; }

        [DisplayName("Chassis Informal Entry Number")]
        public string InformalEntryNumberChassis { get; set; }

        public BatchHeaderModel BatchHeader { get; set; }

    }

    public class CSRModel
    {
        public CSRModel()
        {
            BatchHeader = new BatchHeaderModel();
        }
        [DisplayName("Transaction ID")]
        //[Required(ErrorMessage = "The Transaction ID field is required.")]
        public string TransactionID { get; set; }

        [DisplayName("CSR Number")]
        [Required(ErrorMessage = "The CRS Number field is required.")]
        public string CSRNumber { get; set; }

        [DisplayName("Item Type")]
        //[Required(ErrorMessage = "The Item Type field is required.")]
        public string ItemType { get; set; }

        [DisplayName("Report Entry ID")]
        public string ReportEntryID { get; set; }

        [DisplayName("Report Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ReportDate { get; set; }

        [DisplayName("BIRCCMV")]
        //[Required(ErrorMessage = "The BIR CCMV field is required.")]
        public string BIRCCMV { get; set; }

        [DisplayName("Date Issued")]
        //[Required(ErrorMessage = "The Date Issued field is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateIssued3 { get; set; }

        [DisplayName("Tax Type")]
        //[Required(ErrorMessage = "The Tax Type field is required.")]
        public string TaxType { get; set; }

        [DisplayName("Tax Amount")]
        //[Required(ErrorMessage = "The Tax Amount field is required.")]
        public Nullable<decimal> TaxAmount { get; set; }

        [DisplayName("CSR File")]
        [Required(ErrorMessage = "The CSR File field is required.")]
        public HttpPostedFileBase CSRFile { get; set; }
        //public HttpPostedFileBase[] CSRFileArray { get; set; }
        public BatchHeaderModel BatchHeader { get; set; }

        public byte[] CSRByte { get; set; }
        public string CSRContentType { get; set; }
        public string CSRImage
        {
            get
            {
                var base64 = "";
                if (CSRByte != null)
                {
                    base64 = Convert.ToBase64String(CSRByte);
                }
                return String.Format("data:" + CSRContentType + ";base64,{0}", base64);
            }
        }

        //public List<CSRListModel> CSRListModel { get; set; }

    }

    public class CSRModelBatchUpload
    {
        public CSRModelBatchUpload()
        {
            CSRListModel = new List<CSRListModel>();
        }
        public HttpPostedFileBase[] CSRFileArray { get; set; }
        public List<CSRListModel> CSRListModel { get; set; }
    }


    public class CSRListModel
    {
        public HttpPostedFileBase CSRListFile { get; set; }
        public string message { get; set; }
        public bool isSuccess { get; set; }
    }

    public class AssignDealerModel
    {
        [DisplayName("Dealer")]
        [Required(ErrorMessage = "The Dealer field is required.")]
        public int SelectedDealer { get; set; }

        [DisplayName("Dealer Branch")]
        [Required(ErrorMessage = "The Dealer Branch field is required.")]
        public int SelectedDealerBranch { get; set; }
        public List<MAI_DealerModel> MAI_DealerList { get; set; }
        public List<DealerBranchModel> MAI_DealerBranchList { get; set; }
    }

    public class SOEModel
    {
        public SOEModel()
        {
            SOEListModel = new List<SOEListModel>();
        }
        [DisplayName("Image File")]
        [Required(ErrorMessage = "The file field is required.")]
        public HttpPostedFileBase[] SOEFile { get; set; }
        public List<SOEListModel> SOEListModel { get; set; }
    }
    public class SOEListModel
    {
        public HttpPostedFileBase SOEListFile { get; set; }
        public string message { get; set; }
        public bool isSuccess { get; set; }
    }
    public class SOCModel
    {
        public SOCModel()
        {
            SOCListModel = new List<SOCListModel>();
        }
        [DisplayName("Image File")]
        [Required(ErrorMessage = "The file field is required.")]
        public HttpPostedFileBase[] SOCFile { get; set; }
        public List<SOCListModel> SOCListModel { get; set; }
    }
    public class SOCListModel
    {
        public HttpPostedFileBase SOCListFile { get; set; }
        public string message { get; set; }
        public bool isSuccess { get; set; }
    }

    public class COCModel
    {
        public COCModel()
        {
            DealerInsuranceList = new List<vwDealerInsuranceModel>();
            VehicleTypeList = new List<VehicleTypeList>();
            VehicleClassificationList = new List<VehicleClassification>();
            TaxTypeList = new List<CTPLTaxType>();
            AutoGenerateCoC = true;
        }
        [DisplayName("Insurance")]
        [Required(ErrorMessage = "The insurance field is required!")]
        public int DealerInsuranceID { get; set; }
        [DisplayName("CoC Number")]
        [Required]
        public string COC { get; set; }
        [DisplayName("Authentication Code")]
        [Required]
        public string COCAuthenticationCode { get; set; }
        [DisplayName("Policy Number")]
        [Required]
        public string COCPolicyNumber { get; set; }
        [DisplayName("COC Image")]
        [Required]
        public HttpPostedFileBase COCFile { get; set; }
        public byte[] COCByte { get; set; }
        public List<vwDealerInsuranceModel> DealerInsuranceList { get; set; }

        [DisplayName("Inception Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> InceptionDate { get; set; }
        [DisplayName("Expiry Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ExpiryDate { get; set; }

        [DisplayName("Insured Name")]
        [Required]
        public string COCInsuredName { get; set; }
        [DisplayName("Insured Address")]
        [Required]
        public string COCInsuredAddress { get; set; }
        [DisplayName("Automated")]
        [Required]
        public bool AutoGenerateCoC { get; set; }

        [DisplayName("MV Type")]
        [Required]
        public int VehicleTypeID { get; set; }

        [DisplayName("Premium Classification")]
        [Required]
        public int VehicleClassificationID { get; set; }
        [DisplayName("Tax Type")]
        [Required]
        public int TaxTypeID { get; set; }
        public string VehicleClassificationName { get; set; }
        public List<VehicleTypeList> VehicleTypeList { get; set; }
        public List<VehicleClassification> VehicleClassificationList { get; set; }
        public List<CTPLTaxType> TaxTypeList { get; set; }
    }

    public class UploadVehicleInfoModel
    {
        public UploadVehicleInfoModel()
        {
            Table = new DataTable();
            BatchHeader = new BatchHeaderModel();
            VehicleTable = new List<UploadTable>();
            VehicleMakeList = new List<vwDealerVehicleMake>();
            VehicleModelList = new List<VehicleModelModel>();
            VehicleBodyTypeList = new List<VehicleBodyType>();
            VehicleColorList = new List<VehicleColor>();
            VehicleFuelTypeList = new List<VehicleFuelType>();
            VehicleAirconTypeList = new List<AirconType>();
        }
        public DataTable Table { get; set; }
        public BatchHeaderModel BatchHeader { get; set; }
        public List<UploadTable> VehicleTable { get; set; }
        public List<vwDealerVehicleMake> VehicleMakeList { get; set; }
        public List<VehicleModelModel> VehicleModelList { get; set; }
        public List<VehicleBodyType> VehicleBodyTypeList { get; set; }
        public List<VehicleColor> VehicleColorList { get; set; }
        public List<VehicleFuelType> VehicleFuelTypeList { get; set; }
        public List<AirconType> VehicleAirconTypeList { get; set; }
    }

    public class UploadVehicleInfoModelV2 : UploadVehicleInfoModel
    {
        public UploadVehicleInfoModelV2()
        {
            VehicleTableV2 = new List<UploadTableV2>();
            SexList = new List<Sex>();
            CivilStatusList = new List<CivilStatus>();
            CityList = new List<City>();
            ProvinceList = new List<Province>();
            TitleList = new List<Title>();
            BarangayList = new List<Barangay>();
        }
        public List<UploadTableV2> VehicleTableV2 { get; set; }
        public List<Sex> SexList { get; set; }
        public List<CivilStatus> CivilStatusList { get; set; }
        public List<City> CityList { get; set; }
        public List<Province> ProvinceList { get; set; }
        public List<Barangay> BarangayList { get; set; }
        public List<Title> TitleList { get; set; }
    }
    public class IsExistEngineORChassis
    {
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
    }

    public class UploadModel
    {
        public UploadModel()
        {
            BatchHeader = new BatchHeaderModel();
            VehicleTable = new List<UploadTable>();
            VehicleMakeList = new List<vwDealerVehicleMake>();
            VehicleModelList = new List<VehicleModelModel>();
            VehicleBodyTypeList = new List<VehicleBodyType>();
            VehicleColorList = new List<VehicleColor>();
            VehicleFuelTypeList = new List<VehicleFuelType>();
        }
        public List<UploadTable> VehicleTable { get; set; }
        public List<vwDealerVehicleMake> VehicleMakeList { get; set; }
        public List<VehicleModelModel> VehicleModelList { get; set; }
        public List<VehicleBodyType> VehicleBodyTypeList { get; set; }
        public List<VehicleColor> VehicleColorList { get; set; }
        public List<VehicleFuelType> VehicleFuelTypeList { get; set; }
        public BatchHeaderModel BatchHeader { get; set; }
    }

    public class UploadTable
    {
        public UploadTable()
        {
            VehicleModelList = new List<VehicleModelModel>();
        }
        public int VehicleMakeID { get; set; }
        public int VehicleModelID { get; set; }
        public int VehicleBodyTypeID { get; set; }
        [DisplayName("Vehicle Make")]
        [Required]
        public int SelectedVehicleMakeID { get; set; }
        [DisplayName("Vehicle Model")]
        [Required]
        public int? SelectedVehicleModelID { get; set; }
        [DisplayName("Vehicle Body")]
        [Required]
        public int SelectedVehicleBodyTypeID { get; set; }
        [Required]
        public string EngineNumber { get; set; }
        [Required]
        public string ChassisNumber { get; set; }

        [Required]
        public int VehicleColorID { get; set; }
        [Required]
        public string AirconType { get; set; }
        [Required]
        public string ConductionSticker { get; set; }
        [Required]
        public string PistonDisplacement { get; set; }
        [Required]
        public int VehicleFuelTypeID { get; set; }
        [Required]
        public string Cylinders { get; set; }
        [Required]
        public string BodyIDNumber { get; set; }
        [Required]
        public string Year { get; set; }
        [Required]
        public string GrossVehicleWeight { get; set; }
        [Required]
        public string FrontTiresNumber { get; set; }
        [Required]
        public string RearTiresNumber { get; set; }
        [Required]
        public string COCNo { get; set; }
        [DisplayName("Vehicle Color")]
        [Required]
        public int SelectedVehicleColorID { get; set; }
        [DisplayName("Vehicle Fuel Type")]
        [Required]
        public int SelectedVehicleFuelTypeID { get; set; }
        //[Required(ErrorMessage = "The CSR Number field is required.")]
        public string CSRNumber { get; set; }
        //[Required(ErrorMessage = "The HPG Number field is required.")]
        public string HPGNumber { get; set; }
        public List<VehicleModelModel> VehicleModelList { get; set; }
    }

    public class UploadTableV2 : UploadTable
    {
        [Required(ErrorMessage = "The Invoice Number field is required.")]
        public string InvoiceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "The Invoice Date field is required.")]
        public System.DateTime? InvoiceDate { get; set; }
        [Required(ErrorMessage = "The Invoice Amount field is required.")]
        public decimal InvoiceAmount { get; set; }

        public int TitleID { get; set; }
        public string TitleName { get; set; }

        [DisplayName("Last Name")]
        //[Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName { get; set; }

        [DisplayName("First Name")]
        //[Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; }

        [DisplayName("MiddleName")]
        //[Required(ErrorMessage = "The Middle Name field is required. Put - if not applicable")]
        public string MiddleName { get; set; }

        [DisplayName("Birthdate")]
        //[DateRange(ErrorMessage = "The Birthdate field must be in date range.")]
        //[Required(ErrorMessage = "The Birthdate field is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? Birthdate { get; set; }

        [DisplayName("Civil Status")]
        //[Required(ErrorMessage = "The Civil Status field is required.")]
        public string CivilStatusCode { get; set; }

        [DisplayName("Sex")]
        //[Required(ErrorMessage = "The Gender field is required.")]
        public string SexCode { get; set; }

        [DisplayName("Contact Number")]
        //[Required(ErrorMessage = "The Contact Number field is required. Put - if not applicable")]
        public string ContactNumber { get; set; }

        [DisplayName("Email Address")]
        //[Required(ErrorMessage = "The Email Address field is required. Put - if not applicable")]
        public string EmailAddress { get; set; }

        [DisplayName("TIN #")]
        [RegularExpression(@"^\d{3}\-\d{3}\-\d{3}\-\d{3}$", ErrorMessage = "XXX-XXX-XXX-XXX format is required")]
        //[Required(ErrorMessage = "The TIN field is required.")]
        public string TIN { get; set; }

        [DisplayName("Organization Name")]
        //[Required(ErrorMessage = "The Organization Name field is required.")]
        public string OrgName { get; set; }

        [DisplayName("Organization MNEMONIC")]
        //[Required(ErrorMessage = "The Organization MNEMONIC field is required. Put - if not applicable")]
        public string OrgMnemonic { get; set; }

        [DisplayName("Primary Contact")]
        //[Required(ErrorMessage = "The Primary Contact field is required. Put - if not applicable")]
        public string PrimaryContact { get; set; }

        [DisplayName("Contact Details")]
        //[Required(ErrorMessage = "The Contact Details field is required. Put - if not applicable")]
        public string ContactDetails { get; set; }

        [DisplayName("Email Address")]
        //[Required(ErrorMessage = "The Email Address field is required. Put - if not applicable")]
        public string EmailAddressOrg { get; set; }

        [DisplayName("Phone No.")]
        //[Required(ErrorMessage = "The Phone Number field is required. Put - if not applicable")]
        public string PhoneNo { get; set; }

        [DisplayName("TIN #")]
        [RegularExpression(@"^\d{3}\-\d{3}\-\d{3}\-\d{3}$", ErrorMessage = "XXX-XXX-XXX-XXX format is required")]
        //[Required(ErrorMessage = "The TIN field is required.")]
        public string TINOrg { get; set; }

        [DisplayName("Province")]
        [Required(ErrorMessage = "The Province field is required.")]
        public int ProvinceID { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int CityID { get; set; }

        [DisplayName("Barangay")]
        [Required(ErrorMessage = "The Barangay field is required.")]
        public int BarangayID { get; set; }

        [DisplayName("House Bldg. Number")]
        [Required(ErrorMessage = "The House/Bldg Number field is required.")]
        [MaxLength(5, ErrorMessage = "Max length is 5")]
        public string HouseBldgNumber { get; set; }

        [DisplayName("Street")]
        [MaxLength(30, ErrorMessage = "Max length is 30")]
        [Required(ErrorMessage = "The Street field is required.")]
        public string Street { get; set; }

        [DisplayName("Zip Code")]
        [Required(ErrorMessage = "The Zip Code field is required. Put - if not applicable")]
        public string ZipCode { get; set; }

        public List<CityList_upload> Citylist { get; set; }
        public List<BarangayList_upload> Barangaylist { get; set; }
    }
    public class CityList_upload
    {
        public int CityID { get; set; } 
        public string CityName { get; set; }
    }
    public class BarangayList_upload
    {
        public int BarangayID { get; set; }
        public string BarangayName { get; set; }
    }
}