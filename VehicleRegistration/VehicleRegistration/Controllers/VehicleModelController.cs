using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;

namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
    public class VehicleModelController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: VehicleMake
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                //Callback Tables vwVehicleModels
                var VehicleList = db.vwVehicleModel.Where(o => o.Active == true).ToList();

                return View(VehicleList);
            }
        }
        [HttpGet]
        public ActionResult VehicleModel_Register(int? id)
        {
            ViewBag.id = id;

            VehicleModelModel NewVehicleModel = new VehicleModelModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                NewVehicleModel.VehicleMakeList = db.VehicleMake.Where(o => o.Active == true).ToList();
                NewVehicleModel.VehicleClassificationList = db.VehicleClassification.Where(o => o.Active == true).ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var VehicleModelLoad = db.vwVehicleModel.Where(o => o.Active == true && o.VehicleModelID == id).ToList().FirstOrDefault();
                    NewVehicleModel.VehicleModelID = VehicleModelLoad.VehicleModelID;
                    NewVehicleModel.VehicleModelID = VehicleModelLoad.VehicleModelID;
                    NewVehicleModel.SelectedVehicleMakeID = VehicleModelLoad.VehicleMakeID;
                    NewVehicleModel.VehicleModelName = VehicleModelLoad.VehicleModelName;
                    NewVehicleModel.Variant = VehicleModelLoad.Variant;
                    NewVehicleModel.YearOfMake = VehicleModelLoad.YearOfMake;
                    NewVehicleModel.VehicleClassificationID = VehicleModelLoad.VehicleClassificationID;

                }
                return PartialView(NewVehicleModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VehicleModel_Register(VehicleModelModel VehicleModel, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewVehicleModel = new VehicleModel
                            {
                                VehicleMakeID = VehicleModel.SelectedVehicleMakeID,
                                VehicleModelName = VehicleModel.VehicleModelName.Trim(),
                                Variant = VehicleModel.Variant.Trim(),
                                YearOfMake = VehicleModel.YearOfMake,
                                Active = true,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                VehicleClassificationID = VehicleModel.VehicleClassificationID
                            };

                            db.VehicleModel.Add(NewVehicleModel);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.VehicleModel.Where(o => o.VehicleModelID == VehicleModel.VehicleModelID).FirstOrDefault();
                            Update.VehicleMakeID = VehicleModel.SelectedVehicleMakeID;
                            Update.VehicleModelName = VehicleModel.VehicleModelName.Trim();
                            Update.Variant = VehicleModel.Variant.Trim();
                            Update.YearOfMake = VehicleModel.YearOfMake;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            Update.VehicleClassificationID = VehicleModel.VehicleClassificationID;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var DeleteVehicleModel = db.VehicleModel.Where(o => o.VehicleModelID == VehicleModel.VehicleModelID).FirstOrDefault();
                            DeleteVehicleModel.Active = false;
                            DeleteVehicleModel.UpdatedBy = CurrentUser.Details.UserID;
                            DeleteVehicleModel.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Vehicle Model Deleted!";
                        }
                        break;
                }
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(VehicleModel);
            }
        }
        #region

        [HttpGet]
        public ActionResult VehicleModel_Reports()
        {
            VehicleModelReports model = new VehicleModelReports();
            using (db = new VRSystemEntities())
            {
                model.VehicleList = new List<VehicleMake>();
                model.VehicleList.Add(new VehicleMake() { VehicleMakeID = 0, VehicleMakeName = "ALL" });
                model.VehicleList.AddRange(db.VehicleMake.Where(o => o.Active == true)
                    .ToList());
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VehicleModel_Reports(VehicleModelReports model)
        {
            string path = VehicleModel_Reports(model.SelectedVehicleID);
            using (db = new VRSystemEntities())
            {
                model.VehicleList = new List<VehicleMake>();
                model.VehicleList.Add(new VehicleMake() { VehicleMakeID = 0, VehicleMakeName = "ALL" });
                model.VehicleList.AddRange(db.VehicleMake.Where(o => o.Active == true)
                    .ToList());
            }
            return View(model);
        }

        public string VehicleModel_Reports(int VehicleMakeID)
        {

            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "VehicleModelReports.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
           
            DataTable dt = new DataTable("DataSet1");
            dt.Columns.Add(new DataColumn("Make", typeof(string)));
            dt.Columns.Add(new DataColumn { ColumnName = "Model", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Variant", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Year", DataType = typeof(int), AllowDBNull = true });
          

            List<vwVehicleModel> Header = new List<vwVehicleModel>();
            using (db = new VRSystemEntities())
            {
                
                if (VehicleMakeID == 0)
                {
                    Header = db.vwVehicleModel.Where(o => o.Active == true).OrderBy(o => o.VehicleMakeID).ToList();
                }

                else
                {
                    Header = db.vwVehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID).ToList();
                }


                foreach (var item in Header)
                {
                    dt.Rows.Add(
                        item.VehicleMakeName,
                        item.VehicleModelName,
                        item.Variant,
                        item.YearOfMake
                        );
                }
            }
            //ReportParameter[] prm = new ReportParameter[1];
            ////prm[0] = new ReportParameter("DealerParameter", DealerName);
            ////prm[1] = new ReportParameter("DateFromParameter", DateFrom.Date.ToShortDateString());
            ////prm[2] = new ReportParameter("DateToParameter", DateTo.Date.ToShortDateString());
            //lr.SetParameters(prm);

            ReportDataSource rds = new ReportDataSource("DataSet1", dt);
            lr.DataSources.Clear();
            lr.DataSources.Add(rds);

            lr.Refresh();


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo = "";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            var timecreated = DateTime.Now.ToString("MMddyyyyhhmmss");
            ViewBag.Path = "Vehicle Model Reports - " + timecreated + ".pdf";
            var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "Vehicle Model Reports - " + timecreated + ".pdf";


            //var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "Vehicle Model Reports.pdf";
            //System.IO.File.Delete(pdfPath);
            //ViewBag.Path = "Vehicle Model Reports.pdf";
            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }
            //ViewBag.pfpath = pdfPath;
            return pdfPath;
        }
        #endregion
    }
}