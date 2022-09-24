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
    [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia})]
    public class VehicleMakeController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: VehicleMake
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var vehiclelist = db.VehicleMake.Where(o => o.Active == true).ToList();

                return View(vehiclelist);
            }
        }
       
        [HttpGet]
        public ActionResult Vehicle_Register(int? id)
        {
            ViewBag.id = id;

            VehicleMakeModel NewVehicleMake = new VehicleMakeModel();
           

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

                    var VehicleMakeLoad = db.VehicleMake.Where(o => o.Active == true && o.VehicleMakeID == id).ToList().FirstOrDefault();

                    NewVehicleMake.VehicleMakeID = VehicleMakeLoad.VehicleMakeID;
                    NewVehicleMake.VehicleMakeName = VehicleMakeLoad.VehicleMakeName;
                }
                return PartialView(NewVehicleMake);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vehicle_Register(VehicleMakeModel VehicleMake, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewVehicleMake = new VehicleMake
                            {
                                VehicleMakeName = VehicleMake.VehicleMakeName.Trim(),
                                Active = true,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now
                            };

                            db.VehicleMake.Add(NewVehicleMake);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.VehicleMake.Where(o => o.VehicleMakeID == VehicleMake.VehicleMakeID).FirstOrDefault();
                            Update.VehicleMakeName = VehicleMake.VehicleMakeName.Trim();
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var DeleteVehicleMake = db.VehicleMake.Where(o => o.VehicleMakeID == VehicleMake.VehicleMakeID).FirstOrDefault();
                            DeleteVehicleMake.Active = false;
                            DeleteVehicleMake.UpdatedBy = CurrentUser.Details.UserID;
                            DeleteVehicleMake.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Vehicle Make Deleted!";
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
                return View(VehicleMake);
            }
        }

        public ActionResult VehicleInfo(int? id)
        {
            ViewBag.id = id;

            VehicleMakeModel NewVehicleMake = new VehicleMakeModel();
            //NewVehicleMake.VehicleModelList = new List<vwVehicleModel>();

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

                    var VehicleMakeLoad = db.VehicleMake.Where(o => o.Active == true && o.VehicleMakeID == id).ToList().FirstOrDefault();

                    //NewVehicleMake.VehicleModelList = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeLoad.VehicleMakeID).ToList();
                    NewVehicleMake.VehicleMakeID = VehicleMakeLoad.VehicleMakeID;
                    NewVehicleMake.VehicleMakeName = VehicleMakeLoad.VehicleMakeName;

                    NewVehicleMake.VehicleModelList = db.vwVehicleModel.Where(o => o.VehicleMakeID == VehicleMakeLoad.VehicleMakeID && o.Active == true).ToList();
                }
                return PartialView(NewVehicleMake);
            }
        }
    }
}