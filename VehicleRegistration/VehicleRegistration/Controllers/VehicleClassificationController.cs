using System;
using System.Collections.Generic;
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
    public class VehicleClassificationController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: VehicleClassification
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var vehicleclassificationlist = db.VehicleClassification.Where(o => o.Active == true).ToList();

                return View(vehicleclassificationlist);
            }
        }
        [HttpGet]
        public ActionResult Vehicle_Classification_Registration(int? id)
        {
            ViewBag.id = id;

            VehicleClassificationModel NewVehicleClassification = new VehicleClassificationModel();


            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;


                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var VehicleClassificationLoad = db.VehicleClassification.Where(o => o.Active == true && o.VehicleClassificationID == id).ToList().FirstOrDefault();

                    NewVehicleClassification.VehicleClassificationID = VehicleClassificationLoad.VehicleClassificationID;
                    NewVehicleClassification.VehicleClassificationName = VehicleClassificationLoad.VehicleClassificationName;
                }
                return PartialView(NewVehicleClassification);
            }
        }

        /////ValidateAntiForgeryToken Here in Vehicle Classification Registration Functions///////
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vehicle_Classification_Registration(VehicleClassificationModel VehicleClassification, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewVehicleClassification = new VehicleClassification
                            {
                                VehicleClassificationName = VehicleClassification.VehicleClassificationName.Trim(),
                                Active = true,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now
                            };

                            db.VehicleClassification.Add(NewVehicleClassification);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.VehicleClassification.Where(o => o.VehicleClassificationID == VehicleClassification.VehicleClassificationID).FirstOrDefault();
                            Update.VehicleClassificationName = VehicleClassification.VehicleClassificationName.Trim();
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var DeleteVehicleClassification = db.VehicleClassification.Where(o => o.VehicleClassificationID == VehicleClassification.VehicleClassificationID).FirstOrDefault();
                            DeleteVehicleClassification.Active = false;
                            DeleteVehicleClassification.UpdatedBy = CurrentUser.Details.UserID;
                            DeleteVehicleClassification.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Vehicle Classification Deleted!";
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
                return View(VehicleClassification);
            }
        }

    }
}
