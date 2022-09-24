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
    public class VehicleFuelTypeController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();

        // GET: VehicleFuelType
        public ActionResult Index()

        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var vehiclefueltypelist = db.VehicleFuelType.Where(o => o.Active == true).ToList();

                return View(vehiclefueltypelist);
            }
        }
        /////how to True Viewbag.edit=False, Here in Vehicle Fuel Type Registration Functions///////
        [HttpGet]
        public ActionResult Vehicle_Fuel_Registration(int? id)
        {
            ViewBag.id = id;

            VehicleFuelTypeModel NewVehicleFuelType = new VehicleFuelTypeModel();


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

                    var VehicleFuelTypeLoad = db.VehicleFuelType.Where(o => o.Active == true && o.VehicleFuelTypeID == id).ToList().FirstOrDefault();

                    NewVehicleFuelType.VehicleFuelTypeID = VehicleFuelTypeLoad.VehicleFuelTypeID;
                    NewVehicleFuelType.VehicleFuelTypeName = VehicleFuelTypeLoad.VehicleFuelTypeName;
                }
                return PartialView(NewVehicleFuelType);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vehicle_Fuel_Registration(VehicleFuelTypeModel VehicleFuelType, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewVehicleFuelType = new VehicleFuelType
                            {
                                VehicleFuelTypeName = VehicleFuelType.VehicleFuelTypeName.Trim(),
                                Active = true,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now
                            };

                            db.VehicleFuelType.Add(NewVehicleFuelType);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.VehicleFuelType.Where(o => o.VehicleFuelTypeID == VehicleFuelType.VehicleFuelTypeID).FirstOrDefault();
                            Update.VehicleFuelTypeName = VehicleFuelType.VehicleFuelTypeName.Trim();
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var DeleteVehicleFuelType = db.VehicleFuelType.Where(o => o.VehicleFuelTypeID == VehicleFuelType.VehicleFuelTypeID).FirstOrDefault();
                            DeleteVehicleFuelType.Active = false;
                            DeleteVehicleFuelType.UpdatedBy = CurrentUser.Details.UserID;
                            DeleteVehicleFuelType.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Vehicle Fuel Type Deleted!";
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
                return View(VehicleFuelType);
            }
        }
    }
}