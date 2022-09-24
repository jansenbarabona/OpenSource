using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class VehicleClassificationModel
    {
        [DisplayName("Vehicle Classification ID")]
        public int VehicleClassificationID { get; set; }
        [DisplayName("Vehicle Classification Name")]
        public string VehicleClassificationName { get; set; }


        public bool Active { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}