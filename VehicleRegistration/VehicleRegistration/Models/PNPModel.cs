using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class PNPModel
    {
        public PNPModel()
        {
            VehicleList = new List<vwVehicleListModel>();
            PNPListModel = new List<PNPListModel>();
        }
        [DisplayName("Image File")]
        [Required]
        public HttpPostedFileBase PNPFile { get; set; }
        [DisplayName("HPG Control Number")]
        [Required]
        public string HPGControlNumber { get; set; }
        [DisplayName("Image File")]
        [Required]
        public HttpPostedFileBase[] PNPFileBatch { get; set; }
        public List<PNPListModel> PNPListModel { get; set; }
        [DisplayName("Application for PNP Certificate submitted")]
        public bool AutoPNP { get; set; }
        [DisplayName("Reference Number")]
        [Required]
        public string PNPReceiptReferenceNumber { get; set; }
        [DisplayName("Receipt")]
        [Required]
        public HttpPostedFileBase PNPReceiptFile { get; set; }

        public List<vwVehicleListModel> VehicleList { get; set; }
    }
    public class PNPListModel
    {
        public HttpPostedFileBase PNPListFile { get; set; }
        public string message { get; set; }
        public bool isSuccess { get; set; }
    }
}