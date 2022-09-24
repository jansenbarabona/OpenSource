//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VehicleRegistration.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwVehicleList
    {
        public int VehicleID { get; set; }
        public int MAIID { get; set; }
        public Nullable<int> DealerID { get; set; }
        public Nullable<int> DealerBranchID { get; set; }
        public Nullable<int> VehicleMakeID { get; set; }
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
        public Nullable<int> VehicleColorID { get; set; }
        public string PistonDisplacement { get; set; }
        public Nullable<int> VehicleFuelTypeID { get; set; }
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
        public bool Active { get; set; }
        public int CreatedBy { get; set; }
        public string VehicleBodyTypeName { get; set; }
        public string VehicleBodyAbbr { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public byte[] PNPClearance { get; set; }
        public bool LTOSubmitted { get; set; }
        public bool Assigned { get; set; }
        public string HPGControlNumber { get; set; }
        public bool AutoPNP { get; set; }
        public string PNPReceiptReferenceNumber { get; set; }
        public Nullable<bool> CSRSubmitted { get; set; }
    }
}
