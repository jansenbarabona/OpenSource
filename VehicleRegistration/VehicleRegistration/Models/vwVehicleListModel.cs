using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class vwVehicleListModel
    {
        public int VehicleID { get; set; }
        public int MAIID { get; set; }
        public Nullable<int> DealerID { get; set; }
        public Nullable<int> DealerBranchID { get; set; }
        public int VehicleMakeID { get; set; }
        public Nullable<int> VehicleModelID { get; set; }
        public string EngineNumber { get; set; }
        public string CPNumber { get; set; }
        public Nullable<System.DateTime> DateIssued1 { get; set; }
        public string ChassisNumber { get; set; }
        public string CPNumber2 { get; set; }
        public Nullable<System.DateTime> DateIssued2 { get; set; }
        public string BodyIDNumber { get; set; }
        public string BIRCCMV { get; set; }
        public Nullable<System.DateTime> DateIssued3 { get; set; }
        public int VehicleColorID { get; set; }
        public string PistonDisplacement { get; set; }
        public int VehicleFuelTypeID { get; set; }
        public string Cylinders { get; set; }
        public string Series { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<decimal> GrossVehicleWeight { get; set; }
        public Nullable<int> FrontTiresNumber { get; set; }
        public Nullable<int> RearTiresNumber { get; set; }
        public string TaxType { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public string AirconType { get; set; }
        public string ConductionSticker { get; set; }
        public string COCNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> DatePrepared { get; set; }
        public string CSRNumber { get; set; }
        public byte[] BOCCertificateOfPayment { get; set; }
        public byte[] CertificateOfStockReport { get; set; }
        public byte[] CertificateOfConformity { get; set; }
        public byte[] StencilOfEngine { get; set; }
        public byte[] StencilOfChasis { get; set; }
        public string MAIName { get; set; }
        public string DealerName { get; set; }
        public string DealerBranchName { get; set; }
        public string VehicleMakeName { get; set; }
        public string VehicleModelName { get; set; }
        public string Variant { get; set; }
        public Nullable<int> YearOfMake { get; set; }
        public string VehicleColorName { get; set; }
        public string VehicleFuelTypeName { get; set; }
        public string PNPReceiptReferenceNumber { get; set; }
        public bool Active { get; set; }
        public int CreatedBy { get; set; }
        public bool isChecked { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class VehicleListModel
    {
        public VehicleListModel()
        {
            VehicleList = new List<vwVehicleListModel>();
            BOCInfo = new BOCModel();
            CSRInfo = new CSRModel();
            DealerInfo = new AssignDealerModel();
            SOEInfo = new SOEModel();
            SOCInfo = new SOCModel();
            COCInfo = new COCModel();
            BatchFilter = new BatchFilter();
            BatchHeader = new BatchHeaderModel();
        }
        [DisplayName("Dealer")]
        [Required(ErrorMessage = "The Dealer field is required.")]
        public int SelectedDealer { get; set; }
      
        public string BuyerType { get; set; }
        [DisplayName("Dealer Branch")]
        [Required(ErrorMessage = "The Dealer Branch field is required.")]
        public int SelectedDealerBranch { get; set; }
        public List<vwVehicleListModel> VehicleList { get; set; }
        public List<MAI_DealerModel> MAI_DealerList { get; set; }
        public List<DealerBranchModel> MAI_DealerBranchList { get; set; }
        public VehicleInfoModel VehicleInfo { get; set; }
        public BOCModel BOCInfo { get; set; }
        public CSRModel CSRInfo { get; set; }
        public CSRModelBatchUpload CSRBatchInfo { get; set; }
        public AssignDealerModel DealerInfo { get; set; }
        public SOEModel SOEInfo { get; set; }
        public SOCModel SOCInfo { get; set; }
        public PNPModel PNPInfo { get; set; }
        public COCModel COCInfo { get; set; }
        public BatchFilter BatchFilter { get; set; }
        public BatchHeaderModel BatchHeader { get; set; }

        //[DisplayName("C.P. Number")]
        //[Required(ErrorMessage = "The CP Number field is required.")]
        //public string CPNumber { get; set; }
        ////
        //[DisplayName("Date Issued")]
        //[Required(ErrorMessage = "The Date Issued field is required.")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public Nullable<System.DateTime> DateIssued1 { get; set; }
        ////
        //[DisplayName("BOC File")]
        //[Required(ErrorMessage = "The BOC File field is required.")]
        //public HttpPostedFileBase BOCFile2 { get; set; }
        ////
        //[DisplayName("C.P. Number")]
        ////[Required(ErrorMessage = "The CP Number field is required.")]
        //public string CPNumber2 { get; set; }
        ////
        //[DisplayName("Date Issued")]
        ////[Required(ErrorMessage = "The Date Issued field is required.")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public Nullable<System.DateTime> DateIssued2 { get; set; }
        ////
        //[DisplayName("BOC File")]
        //public HttpPostedFileBase BOC2File2 { get; set; }
    }
}