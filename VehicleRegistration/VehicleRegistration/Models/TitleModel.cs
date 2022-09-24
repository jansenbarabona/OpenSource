using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public partial class TitleModel
    {
        //
        [DisplayName("Title Name")]
        public int TitleID { get; set; }
        //
        [DisplayName("Select Title Type")]
        public int TitleTypeID { get; set; }
        //
        [DisplayName("Title")]
        public string TitleName { get; set; }
        //
        [DisplayName("Title Abbreviation")]
        public string TitleAbbreviation { get; set; }
        //
        [DisplayName("CreatedBy")]
        public int CreatedBy { get; set; }
        //
        [DisplayName("CreatedDate")]
        public System.DateTime CreatedDate { get; set; }
        //
        [DisplayName("Active")]
        public bool Active { get; set; }
        //
        [DisplayName("Title Type Name")]
        public string TitleTypeName { get; set; }
        //
        [DisplayName("Select Title Type")]
        public List<TitleType> TitleTypeList { get; set; }
    }
}