using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class BatchModel
    {
        public BatchHeaderModel BatchHeaderModel { get; set; }
        public List<BatchDetails> BatchDetails { get; set; }
    }

    public class BatchHeaderModel
    {
        [DisplayName("Batch")]
        public int BatchID { get; set; }
        [DisplayName("BatchType")]
        [Required]
        public int BatchTypeID { get; set; }
        [DisplayName("Batch Reference #")]
        [Required]
        public string ReferenceNo { get; set; }
        [DisplayName("Description")]
        public string BatchDescription { get; set; }
        [DisplayName("Count")]
        public Nullable<int> BatchCount { get; set; }
        [DisplayName("Amount")]
        public Nullable<decimal> BatchAmount { get; set; }
        public List<BatchType> BatchTypeList { get; set; }
        public int EntityTypeID { get; set; }
        public int UserReference { get; set; }
        public int UserSubRef { get; set; }
        public bool Assessed { get; set; }
        public Nullable<decimal> AssessedAmount { get; set; }
        public bool Completed { get; set; }
        public bool Downloaded { get; set; }
        public bool ForPickup { get; set; }
        public bool Received { get; set; }
        public bool Rejected { get; set; }
        public string Remarks { get; set; }

    }
    public class BatchFilter
    {
        public string SelectedBatchFilter { get; set; }
        public List<BatchFilterList> BatchFilterList { get; set; }
    }
    public class BatchFilterList
    {
        public string BatchID { get; set; }
        public string ReferenceNo { get; set; }
    }
}