using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;


namespace VehicleRegistration.Models
{
    public partial class VehicleFuelTypeModel
    {
        [DisplayName("Vehicle Fuel Type ID")]
        public int VehicleFuelTypeID { get; set; }

        [DisplayName("Vehicle Fuel Type Name")]
        public string VehicleFuelTypeName { get; set; }
        /// <summary>
        /// //////////////
        /// </summary>
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}