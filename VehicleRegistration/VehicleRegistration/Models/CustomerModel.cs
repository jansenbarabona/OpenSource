using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VehicleRegistration.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VehicleRegistration.Models
{
    public class CustomerModel
    {
        public CustomerModel()
        {
            SexList = new List<Sex>();
            CivilStatusList = new List<CivilStatus>();
            CityList = new List<City>();
            ProvinceList = new List<Province>();
            TitleList = new List<Title>();
            TitleTypeList = new List<TitleType>();

        }
        [DisplayName("Customer")]
        public int CustomerID { get; set; }
        //
        [DisplayName("Dealer")]
        public int DealerID { get; set; }
        //
        public int TitleID { get; set; }
        //
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName { get; set; }
        //
        [DisplayName("First Name")]
        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; }
        //
        [DisplayName("MiddleName")]
        [Required(ErrorMessage = "The Middle Name field is required. Put - if not applicable")]
        public string MiddleName { get; set; }
        //
        [DisplayName("Organization Name")]
        [Required(ErrorMessage = "The Organization Name field is required.")]
        public string CorpName { get; set; }
        //
        [DisplayName("Organization MNEMONIC")]
        [Required(ErrorMessage = "The Organization MNEMONIC field is required. Put - if not applicable")]
        public string Alias { get; set; }
        //
        [DisplayName("Father's Name")]
        [Required(ErrorMessage = "The Father's Name field is required. Put - if not applicable")]
        public string FathersName { get; set; }
        //
        [DisplayName("Mother's Name")]
        [Required(ErrorMessage = "The Mother's Name field is required. Put - if not applicable")]
        public string MothersName { get; set; }
        //
        [DisplayName("Sex")]
        //[Required(ErrorMessage = "The Gender field is required.")]
        public string SexCode { get; set; }
        //
        [DisplayName("Civil Status")]
        //[Required(ErrorMessage = "The Civil Status field is required.")]
        public string CivilStatusCode { get; set; }
        //
        [DisplayName("Citizenship")]
        [Required(ErrorMessage = "The Citizenship field is required. Put - if not applicable")]
        public string Citizenship { get; set; }
        //
        [DisplayName("House Bldg. Number")]
        [Required(ErrorMessage = "The House/Bldg Number field is required.")]
        [MaxLength(5, ErrorMessage = "Max length is 5")]
        public string HouseBldgNumber { get; set; }
        //
        [DisplayName("Street/Subdivision")]
        [MaxLength(30, ErrorMessage = "Max length is 30")]
        [Required(ErrorMessage = "The Street/Subdivision field is required.")]
        public string StreetSubdivision { get; set; }
        //
        [DisplayName("Barangay")]
        [Required(ErrorMessage = "The Barangay field is required.")]
        public string Barangay { get; set; }
        //
        [DisplayName("City")]
        public int CityID { get; set; }
        //
        [DisplayName("Zip Code")]
        [Required(ErrorMessage = "The Zip Code field is required. Put - if not applicable")]
        public string ZipCode { get; set; }
        //
        [DisplayName("Height")]
        [Required(ErrorMessage = "The Height field is required. Put - if not applicable")]
        public string Height { get; set; }
        //
        [DisplayName("Weight")]
        [Required(ErrorMessage = "The Weight field is required. Put - if not applicable")]
        public string Weight { get; set; }
        //[DataType(DataType.Date)]
        [DisplayName("Birthdate")]
        [DateRange(ErrorMessage = "The Birthdate field must be in date range.")]
        [Required(ErrorMessage = "The Birthdate field is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? Birthdate { get; set; }
        //
        [DisplayName("Birth Place")]
        [Required(ErrorMessage = "The Birthplace field is required. Put - if not applicable")]
        public string Birthplace { get; set; }
        //
        [DisplayName("Contact Number")]
        [Required(ErrorMessage = "The Contact Number field is required. Put - if not applicable")]
        public string ContactNumber { get; set; }
        //
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "The Email Address field is required. Put - if not applicable")]
        public string EmailAddress { get; set; }
        //
        [DisplayName("TIN #")]
        [RegularExpression(@"^\d{3}\-\d{3}\-\d{3}\-\d{3}$", ErrorMessage = "XXX-XXX-XXX-XXX format is required")]
        [Required(ErrorMessage = "The TIN field is required.")]
        public string TIN { get; set; }
        //
        [DisplayName("Additional Address")]
        public string AdditionalAddress { get; set; }
        //
        [DisplayName("Organization Name")]
        [Required(ErrorMessage = "The Organization Name field is required. Put - if not applicable")]
        public string OrganizationName { get; set; }
        //
        [DisplayName("Organization Address")]
        [Required(ErrorMessage = "The Organization Address field is required. Put - if not applicable")]
        public string OrganizationAddress { get; set; }
        //
        [DisplayName("Organization TIN")]
        [RegularExpression(@"^\d{3}\-\d{3}\-\d{3}\-\d{3}$", ErrorMessage = "XXX-XXX-XXX-XXX format is required")]
        [Required(ErrorMessage = "The Organization TIN field is required.")]
        public string OrganizationTIN { get; set; }
        //
        [DisplayName("Contact Person")]
        [Required(ErrorMessage = "The Contact Person field is required. Put - if not applicable")]
        public string ContactPerson { get; set; }
        //
        [DisplayName("Contact Person Number")]
        [Required(ErrorMessage = "The Contact Person Number field is required. Put - if not applicable")]
        public string ContactPersonNumber { get; set; }
        //
        [DisplayName("Sex")]
        public List<Sex> SexList { get; set; }
        //
        [DisplayName("Civil Status")]
        public List<CivilStatus> CivilStatusList { get; set; }
        //
        [DisplayName("Barangay")]
        public List<Barangay> BarangayList { get; set; }
        //
        [DisplayName("City")]
        public List<City> CityList { get; set; }
        //
        [DisplayName("Province")]
        public List<Province> ProvinceList { get; set; }
        //
        [DisplayName("Title")]
        public List<Title> TitleList { get; set; }
        //
        [DisplayName("Title Type")]
        public List<TitleType> TitleTypeList { get; set; }
        //
        [DisplayName("Sex")]
        [Required(ErrorMessage = "The Gender field is required.")]
        public string SelectedSexCode { get; set; }
        //
        [DisplayName("Civil Status")]
        [Required(ErrorMessage = "The Civil Status field is required.")]
        public string SelectedCivilStatusCode { get; set; }
        //
        [DisplayName("Barangay")]
        [Required(ErrorMessage = "The Barangay field is required.")]
        public int SelectedBarangayID { get; set; }
        //
        [DisplayName("City")]
        [Required(ErrorMessage = "The City field is required.")]
        public int SelectedCityID { get; set; }
        //
        [DisplayName("Province")]
        [Required(ErrorMessage = "The Province field is required.")]
        public int SelectedProvinceID { get; set; }
        //
        [DisplayName("Title")]
        public int SelectedTitleID { get; set; }
        //
        [DisplayName("Reference Number")]
        public string ClientReferenceNumber { get; set; }

        public int TitleTypeID { get; set; }
        public string FullName
        {
            get
            {
                if (TitleTypeID == 2)
                {
                    return CorpName;
                }
                else
                {
                    return LastName + ", " + FirstName + " " + MiddleName;
                }
            }
        }
    }
}