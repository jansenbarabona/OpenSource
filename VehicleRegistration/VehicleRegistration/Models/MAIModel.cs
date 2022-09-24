using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class MAIModel
    {
        //public MAI MAI { get; set; }
        //
        [DisplayName("Full Name")]
        public int MAIID { get; set; }
        //
        [DisplayName("MAI Type ID")]
        [Required(ErrorMessage = "The MAI Type field is required.")]
        public int MAITypeID { get; set; }
        //
        [DisplayName("MAI Name")]
        [Required(ErrorMessage = "The Full Name field is required.")]
        public string MAIName { get; set; }
        //
        public string MAITypeName { get; set; }
        //
        public string ProvinceName { get; set; }
        //
        public string CityName { get; set; }
        //
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "The Email Address field is required.")]
        public string EmailAddress { get; set; }
        //
        [DisplayName("Secondary Email")]
        [Required(ErrorMessage = "The Email Address field is required.")]
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
        [DisplayName("Fax Number")]
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
        [DisplayName("City")]
        public int? CityID { get; set; }
        //
        [DisplayName("Province")]
        public int? ProvinceID { get; set; }
        //
        [DisplayName("Zip Code")]
        [Required(ErrorMessage = "The Zip Code field is required.")]
        public string ZipCode { get; set; }
        //
        [DisplayName("Logo")]
        public string Logo { get; set; }
        //
        [DisplayName("Notes")]
        public string Notes { get; set; }
        //
        [DisplayName("CreatedBy")]
        public int CreatedBy { get; set; }
        //
        [DisplayName("CreatedDate")]
        public System.DateTime? CreatedDate { get; set; }
        //
        [DisplayName("Active")]
        public bool Active { get; set; }
        //
        [DisplayName("Accreditation Number")]
        [Required(ErrorMessage = "The Accreditation Number field is required.")]
        public string AccreditationNumber { get; set; }
        //
        [DisplayName("MAI Type")]
        public List<MAIType> MAITypeList { get; set; }
        //
        [DisplayName("City")]
        public List<City> CityList { get; set; }
        //
        [DisplayName("Province")]
        public List<Province> ProvinceList { get; set; }
        //
        [DisplayName("MAI Type")]
        [Required(ErrorMessage = "The MAI Type field is required.")]
        public int SelectedMAITypeID { get; set; }
        //
        [DisplayName("Province")]
        [Required(ErrorMessage = "The Province field is required.")]
        public int SelectedProvinceID { get; set; }
        //
        [DisplayName("City")]
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
        public byte[] LogoByte { get; set; }
        //
        [DisplayName("Logo")]
        public HttpPostedFileBase LogoFile { get; set; }
        //Vehicle
        public List<vwMAIVehicleMake> vwMAIVehicleMakeList { get; set; }
        public List<VehicleMake> VehicleMakeList { get; set; }
        public WalletModel WalletDetail { get; set; }
        public List<vwTransactionEntityList> EntityTransaction { get; set; }

        //public List<vwMAIList> vwMAIList { get; set; }
        //public List<City> CityList { get; set; }
    }
    public class MAINFOMODEL
    {

        public int MAIID { get; set; }
        public string MAITypeName { get; set; }
        public string MAIName { get; set; }
        public string EmailAddress { get; set; }
        public string BusinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string FaxNumber { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string ZipCode { get; set; }
        public string Logo { get; set; }
        public byte[] LogoByte { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool Active { get; set; }
        public string AccreditationNumber { get; set; }
       
    }
    public class MAI_DealerModel
    {
        public int MAIDealerID { get; set; }
        public int MAIID { get; set; }
        public int DealerID { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string DealerName { get; set; }
        public bool isChecked { get; set; }

        public List<MAI_DealerModel> DealerList { get; set; }
        public List<MAI_DealerModel> MAI_DealerList { get; set; }
    }
    public class MAI_DealerModelList
    {
        public int DealerID { get; set; }
        public string DealerName { get; set; }
    }
    
}
