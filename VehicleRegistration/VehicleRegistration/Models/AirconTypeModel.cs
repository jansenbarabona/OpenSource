using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public partial class AirconTypeModel
    {
        //
        [DisplayName("Aircon Type ID")]
        public int AirconTypeID { get; set; }

        //
        [DisplayName("Aircon Type Reference")]
        [Required]
        public string AirconTypeReference { get; set; }

        //
        [DisplayName("Description")]
        [Required]
        public string AirconTypeDescription { get; set; }

        public int CreatedBy { get; set; }
        public System.DateTime CreatedByDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public bool Active { get; set; }
    }
}