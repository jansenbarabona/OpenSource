using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class MVPremiumModel
    {
        [DisplayName("MV Type")]
        public int VehicleTypeID { get; set; }
        [DisplayName("Premium Type")]
        public int VehicleClassificationID { get; set; }
        public List<VehicleTypeList> VehicleTypeList { get; set; }
        public List<vwVehicleClassification> vwVehicleClassificationList { get; set; }
        public List<addVehicleClassification> addVehicleClassificationList { get; set; }
    }

    public class VehicleTypeList
    {
        public int VehicleTypeID { get; set; }
        public string VehicleCode { get; set; }
        public string VehicleDesc { get; set; }
        public string VehicleName {
            get
            {
                var name = VehicleCode + " - " + VehicleDesc;
                return name;
            }
        }
    }

    public class vwVehicleClassification
    {
        public bool vwIsChecked { get; set; }
        public int vwVehicleClassificationID { get; set; }
        public string vwVehicleClassificationName { get; set; }
    }
    public class addVehicleClassification
    {
        public bool addIsChecked { get; set; }
        public int addVehicleClassificationID { get; set; }
        public string addVehicleClassificationName { get; set; }
    }
}