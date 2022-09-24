using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace VehicleRegistration.Models
{
    public class MAIUploadModel
    {
        //
        [DisplayName("Select MAI")]
        public List<MAIModel> MAIList { get; set; }
        //
        [DisplayName("Upload File")]
        public HttpPostedFileBase UploadFile { get; set; }
        //
        [DisplayName("Uploaded Data Table")]
        public DataTable UploadedDataTable { get; set; }
    }
}