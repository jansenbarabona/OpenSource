using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class MAIVehicleMakeModel
    {
        public MAIVehicleMakeModel()
        {
            vwMAIVehicleMakeModelList = new List<Models.vwMAIVehicleMake>();
            VehicleMakeModelList = new List<VehicleMakeModel>();
        }
        
        public List<vwMAIVehicleMake> vwMAIVehicleMakeModelList { get; set; }
        public List<VehicleMakeModel> VehicleMakeModelList { get; set; }
        
    }
}