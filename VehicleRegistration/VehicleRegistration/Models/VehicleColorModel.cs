using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
 

    public partial class VehicleColorModel
    {
        [DisplayName("Vehicle ID")]
        public int VehicleColorID { get; set; }

        [DisplayName("Vehicle Color Name")]
        public string VehicleColorName { get; set; }

        /// <summary>
        /// ///////
        /// </summary>

        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}