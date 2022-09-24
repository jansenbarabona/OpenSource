using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class VehicleMakeModel
    {
        public VehicleMakeModel()
        {
            //UserEntityList = new List<UserEntity>();
        }
        //
        [DisplayName("Vehicle Make")]
        public int VehicleMakeID { get; set; }
        //
        [DisplayName("Vehicle Make Name")]
        public string VehicleMakeName { get; set; }
        //
        [DisplayName("CreatedBy")]
        public int CreatedBy { get; set; }
        //
        [DisplayName("CreatedDat")]
        public System.DateTime CreatedDate { get; set; }
        //
        [DisplayName("Active")]
        public bool Active { get; set; }
        //
        [DisplayName("isChecked")]
        public bool isChecked { get; set; }
        public List<vwVehicleModel> VehicleModelList { get; set; }
       
        ////User Role name
        //public List<UserRole> UserRoleList { get; set; }
        //// selected int in ID
        //public int SelectedUserRoleID { get; set; }
    }

    public class VehicleModelModel
    {
        public int VehicleModelID { get; set; }
        public int VehicleMakeID { get; set; }
        [DisplayName("Model Name")]
        public string VehicleModelName { get; set; }
        public string Variant { get; set; }
        [DisplayName("Year of Make")]
        public int YearOfMake { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public List<VehicleMake> VehicleMakeList { get; set; }
        [DisplayName("Vehicle Make")]
        public int SelectedVehicleMakeID { get; set; }
        public List<VehicleClassification> VehicleClassificationList { get; set; }
        [DisplayName("Vehicle Classification")]
        public int VehicleClassificationID { get; set; }
        //public List<VehicleMake> VehicleList { get; set; }

        //[DisplayName("Dealer")]
        //[Required]
        //public int SelectedVehicleID { get; set; }
    }
    public class VehicleModelReports
    {
        public List<VehicleMake> VehicleList { get; set; }

        [DisplayName("Dealer")]
        [Required]
        public int SelectedVehicleID { get; set; }
        
    }
}