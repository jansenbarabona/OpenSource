using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VehicleRegistration.Models
{
    public partial class RPCModel
    {

        [Required(ErrorMessage = "This field is required.")]
        public int RPCRefID { get; set; }
        public string RPCRefName { get; set; }
        public int RPCCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string RPCNumber { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string RPCName { get; set; }
        public string RPCtype { get; set; }

        public List<Region> RegionList { get; set; }
        public List<Province> ProvinceList { get; set; }
        public List<City> CityList { get; set; }
        public List<Barangay> BarangayList { get; set; }
    }
}
