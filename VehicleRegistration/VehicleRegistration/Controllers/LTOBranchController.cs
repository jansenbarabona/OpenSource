using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;




namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    //[AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
    public class LTOBranchController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: LTOBranch
        public ActionResult Index()
        {
            db.Configuration.LazyLoadingEnabled = false;



            var LTOBranch = (from l in db.LTO
                             join lb in db.LTOBranch on l.LTOID equals lb.LTOID
                             join ct in db.City on lb.CityID equals ct.CityID
                             join pv in db.Province on lb.ProvinceID equals pv.ProvinceID
                             join br in db.Barangay on lb.BarangayID equals br.BarangayID
                             join rg in db.Region on pv.RegionID equals rg.RegionID
                             where lb.Active
                             select new LTOBranchModel
                             {
                                 LTOBranchID = lb.LTOBranchID,
                                 Region_Name = rg.RegionName,
                                 LTOName = l.LTOName,
                                 LTOBranchName = lb.LTOBranchName,
                                 EmailAddress = lb.EmailAddress,
                                 BusinessPhone = lb.BusinessPhone,
                                 MobilePhone = lb.MobilePhone,
                                 FaxNumber = lb.FaxNumber,
                                 Website = lb.WebSite,
                                 Address = lb.Address,
                                 Province_Name = pv.ProvinceName,
                                 City_Name = ct.CityName,
                                 Barangay_Name = br.BarangayName,
                                 ZipCode = lb.ZipCode,
                                 CreatedBy = lb.CreatedBy,
                                 CreatedDate = lb.CreatedDate,
                                 Active = lb.Active,
                                 IsMain = lb.IsMain,
                                 UpdatedBy = lb.UpdatedBy,
                                 UpdatedDate = lb.UpdatedDate,
                                 PlateEmail = lb.PlateEmail,
                                 PNPEmail = lb.PNPEmail

                             }).ToList();

            return View(LTOBranch);
        }

        public ActionResult LTOBranch(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var LTOBranch = db.LTOBranch.Where(o => o.Active != false && o.LTOID == id).ToList();
            return View(LTOBranch);
        }

        public ActionResult AddEdit(int id = 0)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var LTOBranch = db.LTOBranch.Where(o => o.Active != false).ToList();
            return View(LTOBranch);
        }
        // GET: LTOBranch/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LTOBranch/Create
        public ActionResult Create()
        {



            return View();
        }

        // POST: LTOBranch/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LTOBranch/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LTOBranch/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LTOBranch/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LTOBranch/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Download()
        {
            string path = LTOBranchListReport();
           // var timecreated = DateTime.Now.ToString("MMddyyyyhhmmss");
           string pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/"));
           // ViewBag.Path = "LTOBranchList" + timecreated + ".pdf";
            byte[] fileBytes = System.IO.File.ReadAllBytes(pdfPath + ViewBag.Path);
            string fileName = ViewBag.Path;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public string LTOBranchListReport()
        {

            var lr = new LocalReport
            {
                ReportPath = Path.Combine(Server.MapPath("~/Reports/RDLC"),
              "LTOBranch.rdlc"),
                EnableExternalImages = true
            };
            //LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "LTOBranch.rdlc");

            DataTable dt = new DataTable("DataSet1");
            dt.Columns.Add(new DataColumn("LTOBranchID", typeof(string)));
            dt.Columns.Add(new DataColumn("Region", typeof(string)));
            dt.Columns.Add(new DataColumn("LTOBranchName", typeof(string)));
            dt.Columns.Add(new DataColumn { ColumnName = "EmailAddress", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "BusinessPhone", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "MobilePhone", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "FaxNumber", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Website", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Address", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Province", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "City", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Barangay", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "ZipCode", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Active", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "IsMain", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "PlateEmail", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "PNPEmail", DataType = typeof(string), AllowDBNull = true });
            using (db = new VRSystemEntities())
            {

                //var LTOBranchList = db.LTOBranch.Where(o => o.Active == true).ToList();
                var LTOBranch = (from l in db.LTO
                                 join lb in db.LTOBranch on l.LTOID equals lb.LTOID
                                 join ct in db.City on lb.CityID equals ct.CityID
                                 join pv in db.Province on lb.ProvinceID equals pv.ProvinceID
                                 join br in db.Barangay on lb.BarangayID equals br.BarangayID
                                 join rg in db.Region on pv.RegionID equals rg.RegionID
                                 where lb.Active
                                 select new LTOBranchModel
                                 {
                                     LTOBranchID = lb.LTOBranchID,
                                     Region_Name = rg.RegionName,
                                     LTOBranchName = lb.LTOBranchName,
                                     EmailAddress = lb.EmailAddress,
                                     BusinessPhone = lb.BusinessPhone,
                                     MobilePhone = lb.MobilePhone,
                                     FaxNumber = lb.FaxNumber,
                                     Website = lb.WebSite,
                                     Address = lb.Address,
                                     Province_Name = pv.ProvinceName,
                                     City_Name = ct.CityName,
                                     Barangay_Name = br.BarangayName,
                                     ZipCode = lb.ZipCode,
                                     Active = lb.Active,
                                     IsMain = lb.IsMain,
                                     PlateEmail = lb.PlateEmail,
                                     PNPEmail = lb.PNPEmail

                                 }).ToList();

                foreach (var item in LTOBranch)
                {
                    //TimeSpan? ProcessingDays;

                    dt.Rows.Add(
                        item.LTOBranchID,
                        item.Region_Name,
                        item.LTOBranchName,
                        item.EmailAddress,
                        item.BusinessPhone,
                        item.MobilePhone,
                        item.FaxNumber,
                        item.Website,
                        item.Address,
                        item.Province_Name,
                        item.City_Name,
                        item.Barangay_Name,
                        item.ZipCode,
                        item.Active,
                        item.IsMain,
                        item.PlateEmail,
                        item.PNPEmail
                        );
                }
            }


            ReportDataSource rds = new ReportDataSource("DataSet1", dt);
            lr.DataSources.Clear();
            lr.DataSources.Add(rds);

            lr.Refresh();


            //string reportType = "PDF";
            string mimeType;
            string encoding;
            string extension;

            //string deviceInfo = "";

            Warning[] warnings;
            string[] streams;

            var renderedBytes = lr.Render
            (
            "PDF",
            @"<DeviceInfo><OutputFormat>PDF</OutputFormat><HumanReadablePDF>False</HumanReadablePDF></DeviceInfo>",
            out mimeType,
            out encoding,
            out extension,
            out streams,
            out warnings
            );

            var timecreated = DateTime.Now.ToString("MMddyyyyhhmmss");
            ViewBag.Path = "LTOBranchListReport" + timecreated + ".pdf";

            //var timecreated = DateTime.Now.ToString("MMddyyyyhhmmss");
            //var Pathpdf = "LTOBranchListReport" + CurrentUser.Details.UserEntityID + ".pdf";
            var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + ViewBag.Path;

            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }

            return pdfPath;
        }
    }
}
