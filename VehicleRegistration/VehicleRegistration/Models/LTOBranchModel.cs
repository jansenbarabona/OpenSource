using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VehicleRegistration.Models
{
    public class LTOBranchModel
    {
        [DisplayName("LTOID")]
        public int LTOID { get; set; }
        [DisplayName("LTOBranch")]
        public int LTOBranchID { get; set; }
        //
        [DisplayName("LTO Name")]
        [Required(ErrorMessage = "The Full Name field is required.")]
        public string LTOBranchName { get; set; }
        //
        [DisplayName("Email Address")]
        [EmailAddress]
        [Required(ErrorMessage = "The Email field is required.")]
        public string EmailAddress { get; set; }
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
        [DisplayName("Website")]
        //[Required(ErrorMessage = "The Website field is required.")]
        public string Website { get; set; }
        //
        [DisplayName("Address")]
        [Required(ErrorMessage = "The Address field is required.")]
        public string Address { get; set; }

        //
        [DisplayName("Select Province")]
        public int ProvinceID { get; set; }
        //
        [DisplayName("Select City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int CityID { get; set; }
        //
        [DisplayName("Select Barangay")]
        public int BarangayID { get; set; }

        //
        [DisplayName("Zip Code")]
        [Required(ErrorMessage = "The Zip Code field is required.")]
        public string ZipCode { get; set; }
        //
       public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public bool IsMain { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string PlateEmail { get; set; }
        public string PNPEmail { get; set; }
        public string Province_Name { get; set; }
        public string City_Name { get; set; }
        public string Barangay_Name { get; set; }
        public string Region_Name { get; set; }

        public List<LTOBranch> LTOBranches { get; set; }


        [DisplayName("LTO Branch")] 
        [Required]
        public List<City> CityList { get; set; }
        //
        [DisplayName("Select City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int SelectedCityID { get; set; }

        //
        [DisplayName("Barangay")]
        public List<Barangay> BarangayList { get; set; }
        public string BarangayName { get; set; }
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
        public string LTOName { get; set; }
    }
}