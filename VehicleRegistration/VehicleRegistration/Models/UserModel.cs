using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class UserModel
    {
        public UserModel()
        {
            UserEntityList = new List<UserEntity>();
            LTOUserTypeList = new List<LTOUserType>();

        }
        //public User User { get; set; }
        //
        
        [DisplayName("User")]
        public int UserID { get; set; }
        //
        [DisplayName("User Entity")]
        public int UserEntityID { get; set; }
        //
        [DisplayName("User Role")]
        public int UserRoleID { get; set; }
        //
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "The Email Address field is required.")]
        public string EmailAddress { get; set; }
        //
        [DisplayName("Password")]
        [Required(ErrorMessage = "The Password field is required.")]
        public string Password { get; set; }
        //
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName { get; set; }
        //
        [DisplayName("First Name")]
        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; }
        //
        [DisplayName("CreatedBy")]
        public int CreatedBy { get; set; }
        //
        [DisplayName("CreatedDate")]
        public System.DateTime CreatedDate { get; set; }
        //
        [DisplayName("UpdatedBy")]
        public Nullable<int> UpdatedBy { get; set; }
        //
        [DisplayName("Updated Date")]
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        //
        [DisplayName("Select Title Type")]
        public bool Active { get; set; }
        //
        [DisplayName("Select MAI Type")]
        public int SelectedMAITypeID { get; set; }
        //
        [DisplayName("Select Province")]
        public int SelectedProvinceID { get; set; }
        //
        [DisplayName("Select City")]
        public int SelectedCityID { get; set; }
        //
        [DisplayName("Logo Byte")]
        public byte[] LogoByte { get; set; }
        //
        [DisplayName("Select User Entity")]
        public List<UserEntity> UserEntityList { get; set; }
        //
        [DisplayName("Select User Entity")]
        [Required(ErrorMessage = "The Entity field is required.")]
        public int SelectedUserEntityID { get; set; }
        //
        [DisplayName("Select User Role")]
        //User Role name
        public List<UserRole> UserRoleList { get; set; }
        // selected int in ID
        //
        [DisplayName("Select User Role")]
        [Required(ErrorMessage = "The User Role field is required.")]
        public int SelectedUserRoleID { get; set; }
        //
        public Nullable<int> ReferenceID { get; set; }
        //
        
        public Nullable<int> SubReferenceID { get; set; }
        //
        [DisplayName("Reference Name")]
        public string ReferenceName { get; set; }
        //
        [DisplayName("Subreference Name")]
        public string SubReferenceName { get; set; }


        //
        //[DisplayName("Email Address")]
        ////[Required(ErrorMessage = "The Email Address field is required to reset your password.")]
        //public string EmailAddress_Password { get; set; }

        [DisplayName("New Password")]
        public string NewPassword { get; set; }
        [DisplayName("Confirm Password")]
        //[Compare("NewPassword", ErrorMessage = "New Password and Confirm Password not match.")]
        public string ConfirmPassword { get; set; }

        public List<MAI> MAIList { get; set; }
        public List<MAIType> MAITypeList { get; set; }
        public List<Dealer> DealerList { get; set; }
        public List<DealerBranch> DealerBranchList { get; set; }
        public List<Insurance> InsuranceList { get; set; }
        public List<InsuranceBranch> InsuranceBranchist { get; set; }
        [DisplayName("Select LTO Role")]
        public List<LTOUserType> LTOUserTypeList { get; set; }
        public int? SelectedLTOUserTypeID { get; set; }
    }

    public class ForgotPasswordModel
    {
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "The Email Address field is required to reset your password.")]
        public string EmailAddress_Password { get; set; }


        [DisplayName("Password")]
        public string Password { get; set; }
    }
}