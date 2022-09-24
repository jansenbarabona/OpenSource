using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class InsuranceBranchModel
    {
        [DisplayName("Insurance Branch")]
        public int InsuranceBranchID { get; set; }
        [DisplayName("Insurance Name")]
        [Required(ErrorMessage = "The Insurance Name field is required.")]
        public int InsuranceID { get; set; }
        [DisplayName("Insurance Branch Name")]
        [Required(ErrorMessage = "The Insurance Branch Name field is required.")]
        public string InsuranceBranchName { get; set; }
        [DisplayName("Email")]
        [Required(ErrorMessage = "The Email field is required.")]
        public string EmailAddress { get; set; }
        [DisplayName("Business Phone")]
        [Required(ErrorMessage = "The Business Phone field is required.")]
        public string BusinessPhone { get; set; }
        [DisplayName("Mobile Phone")]
        [Required(ErrorMessage = "The Mobile Phone field is required.")]
        public string MobilePhone { get; set; }
        [DisplayName("Fax Number")]
        [Required(ErrorMessage = "The Fax Number field is required.")]
        public string FaxNumber { get; set; }
        [DisplayName("Web Site")]
        [Required(ErrorMessage = "The Web Site field is required.")]
        public string WebSite { get; set; }
        [DisplayName("Address")]
        [Required(ErrorMessage = "The Address field is required.")]
        public string Address { get; set; }
        [DisplayName("Province")]
        [Required(ErrorMessage = "The Province field is required.")]
        public int ProvinceID { get; set; }
        [DisplayName("City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int CityID { get; set; }
        [DisplayName("Zip Code")]
        [Required(ErrorMessage = "The Zip Code field is required.")]
        public string ZipCode { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
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
        [DisplayName("Insurance Name")]
        public string InsuranceName { get; set; }
        [DisplayName("Province Name")]
        public string ProvinceName { get; set; }
        [DisplayName("City Name")]
        public string CityName { get; set; }

        public List<Insurance> InsuranceList { get; set; }
        public List<Province> ProvinceList { get; set; }
        public List<City> CityList { get; set; }
        public bool IsMain { get; set; }
    }
}