using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace VehicleRegistration.Models
{
    public class LTOSubmittedBatch
    {
        public List<LTODealerFilter> DealerList { get; set; }
        public List<MAIModel> MAIList { get; set; }
        public List<LTOBatchHeader> BatchList { get; set; }
        public List<LTOBatchDetailVehicle> VehicleList { get; set; }
        public int SelectedDealerID { get; set; }
        public int SelectedMAIID { get; set; }
        public int SelectedBatchID { get; set; }
        public string SelectedBatchReferenceNo { get; set; }
        public string RejectedRemarks { get; set; }
        public string CancelledRemarks { get; set; }
        public string ReprocessRemarks { get; set; }
    }

    public class LTOAssessBatch
    {
        public List<LTODealerFilter> DealerList { get; set; }
        public List<MAIModel> MAIList { get; set; }
        public List<LTOBatchHeader> BatchList { get; set; }
        public List<LTOAssessBatchDetailVehicle> VehicleList { get; set; }
        public int SelectedDealerID { get; set; }
        public int SelectedMAIID { get; set; }
        public int SelectedBatchID { get; set; }

        [Required]
        [DisplayName("Amount")]
        public decimal AssessedAmount { get; set; }
        [Required]
        [DisplayName("Message")]
        [StringLength(100, ErrorMessage = "Message cannot be longer than 100 characters.")]
        public string RejectRemarks { get; set; }
    }

    public class LTOPayment
    {
        public List<LTODealerFilter> DealerList { get; set; }
        public List<MAIModel> MAIList { get; set; }
        public List<LTOBatchHeader> BatchList { get; set; }
        public List<LTOAssessBatchDetailVehicle> VehicleList { get; set; }
        public int SelectedDealerID { get; set; }
        public int SelectedMAIID { get; set; }
        public int SelectedBatchID { get; set; }
        public LTOPaymentInfo PaymentInfo { get; set; }

        [Required]
        [DisplayName("Payment Reference")]
        public string PaymentRef { get; set; }

        [Required]
        [DisplayName("Payment File")]
        public HttpPostedFileBase PaymentFile { get; set; }

        [Required]
        [DisplayName("E-PAT File")]
        public HttpPostedFileBase EPATFile { get; set; }
        [DisplayName("LTO Region")]
        public List<LTO> LTOList { get; set; }
        [DisplayName("LTO District")]
        public List<LTOBranch> LTOBranchList { get; set; }

        public Nullable<int> LTOID { get; set; }

        public Nullable<int> LTOBranchID { get; set; }
    }

    public class LTOPaymentInfo
    {
        [DisplayName("OR/CR Ref")]
        public string PaymentRef { get; set; }
        [DisplayName("Amount")]
        public string PaymentAmount { get; set; }

        public byte[] PaymentImageByte { get; set; }
        public string PaymentImageContentType { get; set; }
        public string PaymentImage
        {
            get
            {
                var base64 = Convert.ToBase64String(PaymentImageByte);
                return String.Format("data:" + PaymentImageContentType + ";base64,{0}", base64);
            }
        }
    }
    public class LTODealerFilter
    {
        public int DealerID { get; set; }
        public string DealerName { get; set; }
    }

    public class LTOBatchHeader : BatchHeaderModel
    {
        public string EntityName { get; set; }
        public string SubEntityName { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? DateSubmitted { get; set; }

        public string PaymentRef { get; set; }
        public byte[] PaymentImageByte { get; set; }
        public string PaymentImageContentType { get; set; }
        public string PaymentImage
        {
            get
            {
                var base64 = Convert.ToBase64String(PaymentImageByte);
                return String.Format("data:" + PaymentImageContentType + ";base64,{0}", base64);
            }
        }
        public byte[] EPatImageByte { get; set; }
        public string EPatImageContentType { get; set; }
        public string EPatImage
        {
            get
            {
                var base64 = Convert.ToBase64String(EPatImageByte);
                return String.Format("data:" + EPatImageContentType + ";base64,{0}", base64);
            }
        }
    }

    public class LTOBatchDetailVehicle
    {
        public int VehicleID { get; set; }
        public string VehicleMakeName { get; set; }
        public string VehicleModelName { get; set; }
        public string Variant { get; set; }
        public int? YearOfMake { get; set; }
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string BodyIDNumber { get; set; }
        public string Remarks { get; set; }
    }
    public class LTOAssessBatchDetailVehicle
    {
        public int VehicleID { get; set; }
        public string VehicleMakeName { get; set; }
        public string VehicleModelName { get; set; }
        public string Variant { get; set; }
        public int? YearOfMake { get; set; }
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string BodyIDNumber { get; set; }
        public bool Assessed { get; set; }
        public string AssessedAmount { get; set; }
        public bool Completed { get; set; }
        public bool Rejected { get; set; }
        public bool isChecked { get; set; }
        public string Status { get; set; }
    }

    #region [ CSR ]
    public class CSRLTOSubmittedBatch
    {
        public List<LTOMAIFilter> MAIList { get; set; }
        public List<LTOBatchHeader> BatchList { get; set; }
        public List<LTOBatchDetailVehicle> VehicleList { get; set; }
        public int SelectedMAIID { get; set; }
        public int SelectedBatchID { get; set; }
        public string SelectedBatchReferenceNo { get; set; }
        public string RejectedRemarks { get; set; }
        public string CancelledRemarks { get; set; }
        public string ReprocessRemarks { get; set; }
    }
    public class LTOMAIFilter
    {
        public int MAIID { get; set; }
        public string MAIName { get; set; }
    }
    #endregion
}