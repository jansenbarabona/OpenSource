using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class DealerVehicleMakeModel
    {
        public DealerVehicleMakeModel()
        {
            vwDealerVehicleMakeModelList = new List<Models.vwDealerVehicleMake>();
            VehicleMakeModelList = new List<VehicleMakeModel>();
        }
        [DisplayName("Dealer Vehicle Make Model")]
        public List<vwDealerVehicleMake> vwDealerVehicleMakeModelList { get; set; }
        [DisplayName("Vehicle Make Model")]
        public List<VehicleMakeModel> VehicleMakeModelList { get; set; }
        public int DealerID { get; set; }
        public List<DealerFilter> DealerList { get; set; }
    }
    public class DealerFilter
    {
        public int DealerID { get; set; }
        public string DealerName { get; set; }
    }
}