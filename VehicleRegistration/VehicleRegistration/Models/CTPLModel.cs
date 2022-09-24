using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class CTPLModel
    {
        [DisplayName("CTPL Code")]
        public int CTPLID { get; set; }
        [DisplayName("CTPL Term")]
        [Required(ErrorMessage = "The CTPL Term field is required.")]
        public int CTPLTermID { get; set; }
        [DisplayName("Vehicle Classification")]
        [Required(ErrorMessage = "The Vehicle Classification field is required.")]
        public int VehicleClassificationID { get; set; }
        [DisplayName("Basic Premium")]
        [Required(ErrorMessage = "The Basic Premium field is required.")]
        public decimal? BasicPremium { get; set; }
        [DisplayName("Value Added Tax")]
        [Required(ErrorMessage = "The VAT field is required.")]
        public decimal? VAT { get; set; }
        [DisplayName("Document Stamp Tax")]
        [Required(ErrorMessage = "The DST field is required.")]
        public decimal? DST { get; set; }
        [DisplayName("Local Goverment Tax")]
        [Required(ErrorMessage = "The LGT field is required.")]
        public decimal? LGT { get; set; }
        [DisplayName("Taxes")]
        [Required(ErrorMessage = "The Taxes field is required.")]
        public decimal? Taxes { get; set; }
        [DisplayName("Authentication Fee")]
        [Required(ErrorMessage = "The Authentication Fee field is required.")]
        public decimal? AuthenticationFee { get; set; }
        [DisplayName("Gross Premium")]
        [Required(ErrorMessage = "The Gross Premium field is required.")]
        public decimal? GrossPremium { get; set; }
        public bool Active { get; set; }

        public List<CTPL> CTPLList { get; set; }
        public List<vwCTPLModel> vwCTPLModelList { get; set; }
        public List<CTPLTerm> CTPLTermList { get; set; }
        public List<VehicleClassification> VehicleClassificationList { get; set; }
    }
    
    public class vwCTPLModel
    {
        public int CTPLID { get; set; }
        public int CTPLTermID { get; set; }
        public int VehicleClassificationID { get; set; }
        public string VehicleClassificationName { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal Taxes { get; set; }
        public decimal AuthenticationFee { get; set; }
        public decimal GrossPremium { get; set; }
        public bool Active { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
    public class CTPLTermModel
    {
        [DisplayName("CTPL Term Code")]
        public int CPTLTermID { get; set; }
        [DisplayName("Coverage Year")]
        [Required(ErrorMessage = "The Coverage Year field is required.")]
        public int? CoverageYear { get; set; }
        [DisplayName("Term Description")]
        [Required(ErrorMessage = "The Term Description field is required.")]
        public string TermDescription { get; set; }
        public bool Active { get; set; }

        public List<CTPLTerm> CTPLTermList { get; set; }
    }
}