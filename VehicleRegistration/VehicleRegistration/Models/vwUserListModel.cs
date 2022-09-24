using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class vwUserListModel
    {
        public int UserID { get; set; }
        public int UserEntityID { get; set; }
        public int UserRoleID { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Nullable<int> ReferenceID { get; set; }
        public Nullable<int> SubReferenceID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool Active { get; set; }
        public string UserEntityName { get; set; }
        public string UserRoleName { get; set; }
        public Nullable<bool> IsMain { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }

        public string EntityLogo { get; set; }
        public Nullable<int> LTOUserTypeID { get; set; }
        public Nullable<int> UploadVersion { get; set; }
    }
}