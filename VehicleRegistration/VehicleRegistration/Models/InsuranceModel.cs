using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class InsuranceModel
    {
        [DisplayName("Insurance ID")]
        public int InsuranceID { get; set; }
        //
        [DisplayName("Insurance Name")]
        [Required(ErrorMessage = "The Full Name field is required.")]
        public string InsuranceName { get; set; }
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
        [Required(ErrorMessage = "The Mobile Phone field is required.")]
        public string MobilePhone { get; set; }
        //
        [DisplayName("FaxNumber")]
        [Required(ErrorMessage = "The Fax Number field is required.")]
        public string FaxNumber { get; set; }
        //
        [DisplayName("TIN")]
        [Required(ErrorMessage = "The TIN field is required.")]
        public string TIN { get; set; }
        //
        [DisplayName("Website")]
        [Required(ErrorMessage = "The Website field is required.")]
        public string Website { get; set; }
        //
        [DisplayName("Address")]
        [Required(ErrorMessage = "The Address field is required.")]
        public string Address { get; set; }
        //
        [DisplayName("Select Province")]
        [Required(ErrorMessage = "The Province field is required.")]
        public int ProvinceID { get; set; }
        //
        [DisplayName("Province Name")]
        public string ProvinceName { get; set; }
        //
        [DisplayName("Select City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int CityID { get; set; }
        //
        [DisplayName("City Name")]
        public string CityName { get; set; }
        //
        [DisplayName("Zip Code")]
        [Required(ErrorMessage = "The Zip Code field is required.")]
        public string ZipCode { get; set; }
        //
        [DisplayName("Logo")]
        //[Required(ErrorMessage = "The Logo field is required.")]
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
        [DisplayName("Select Province")]
        public List<Province> ProvinceList { get; set; }
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
        [DisplayName("Choose Logo")]
        public HttpPostedFileBase LogoFile { get; set; }

        public List<InsuranceModel> InsuranceModelList { get; set; }
        public WalletModel WalletDetail { get; set; }
        public List<vwTransactionEntityList> EntityTransaction { get; set; }
    }

    public class InsuranceCOCSeriesModel
    {
        public int InsuranceCOCSeriesID { get; set; }
        [DisplayName("Insurance Company")]
        [Required(ErrorMessage = "field is required.")]
        public int InsuranceID { get; set; }
        [DisplayName("Effective Date")]
        [Required(ErrorMessage = "field is required.")]
        public System.DateTime EffectiveDate { get; set; }
        [DisplayName("Series From")]
        [Required(ErrorMessage = "Tield is required.")]
        public int SeriesFrom { get; set; }
        [DisplayName("Series To")]
        [Required(ErrorMessage = "field is required.")]
        public int SeriesTo { get; set; }
        public int CurrentSeries { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public List<Insurance> InsuranceList { get; set; }
        public string InsuranceName { get; set; }
    }

    public class CTPLReportModel
    {
        [DisplayName("Date From")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime DateFrom { get; set; }
        [DisplayName("Date To")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime DateTo { get; set; }
    }
}