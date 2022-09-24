using System;
using ExcelDataReader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;
using ClosedXML.Excel;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Drawing;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Web.Caching;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Ajax.Utilities;


namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.PNP })]
    public class PNPController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: PNP
        public ActionResult ApprovalList()
        {
            using (db = new VRSystemEntities())
            {
                PNPModel pnp = new PNPModel();

                pnp.VehicleList = db.vwVehicleList
                                    .Where(o =>
                                        o.Active == true &&
                                        o.CertificateOfStockReport != null &&
                                        o.CertificateOfConformity != null &&
                                        o.PNPClearance != null ||
                                        o.AutoPNP == true
                                        )
                                    .Select(o => new vwVehicleListModel
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        Year = o.Year,
                                        EngineNumber = o.EngineNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        PNPReceiptReferenceNumber = o.PNPReceiptReferenceNumber,
                                        isChecked = false
                                    }).ToList();

                return View(pnp);
            }
        }
        public ActionResult CompletedList()
        {
            using (db = new VRSystemEntities())
            {
                PNPModel pnp = new PNPModel();

                pnp.VehicleList = db.vwVehicleList
                                    .Where(o =>
                                        o.Active == true &&
                                        o.CertificateOfStockReport != null &&
                                        o.CertificateOfConformity != null &&
                                        o.PNPClearance != null ||
                                        o.AutoPNP == true
                                        )
                                    .Select(o => new vwVehicleListModel
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        Year = o.Year,
                                        EngineNumber = o.EngineNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        PNPReceiptReferenceNumber = o.PNPReceiptReferenceNumber,
                                        isChecked = false
                                    }).ToList();

                return View(pnp);
            }
        }
    }
}