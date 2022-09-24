using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class vwMAIVehicleMakeModel
    {
        public int MAIID { get; set; }
        public string MAIName { get; set; }
        public int VehicleMakeID { get; set; }
        public string VehicleMakeName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}