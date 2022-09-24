using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class VehicleTypeModel
    {
        [DisplayName("Vehicle Type ID")]
        public int VehicleTypeID { get; set; }
        [DisplayName("Vehicle Code")]
        public string VehicleCode { get; set; }
        [DisplayName("Vehicle Type Description")]
        public string VehicleTypeDescription { get; set; }

        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool Active { get; set; }
    }
}