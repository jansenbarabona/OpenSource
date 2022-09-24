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
    public class VehicleTypeController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: VehicleType
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var vehicltypelist = db.VehicleType.Where(o => o.Active == true).ToList();

                return View(vehicltypelist);
            }
        }
        [HttpGet]
        public ActionResult Vehicle_Type_Registration(int? id)
        {
            ViewBag.id = id;

            VehicleTypeModel NewVehicleType= new VehicleTypeModel();


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

                    var VehicleTypeLoad = db.VehicleType.Where(o => o.Active == true && o.VehicleTypeID == id).ToList().FirstOrDefault();

                    NewVehicleType.VehicleTypeID = VehicleTypeLoad.VehicleTypeID;
                    NewVehicleType.VehicleCode = VehicleTypeLoad.VehicleCode;
                    NewVehicleType.VehicleTypeDescription = VehicleTypeLoad.VehicleTypeDescription;
                }
                return PartialView(NewVehicleType);
            }
        }
        /////ValidateAntiForgeryToken Here in Vehicle Color Registration Functions///////
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vehicle_Type_Registration(VehicleTypeModel VehicleType, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewVehicleType = new VehicleType
                            {
                                VehicleTypeID = VehicleType.VehicleTypeID,
                                VehicleCode = VehicleType.VehicleCode,
                                VehicleTypeDescription = VehicleType.VehicleTypeDescription,
                                Active = true,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now
                            };

                            db.VehicleType.Add(NewVehicleType);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.VehicleType.Where(o => o.VehicleTypeID == VehicleType.VehicleTypeID).FirstOrDefault();
                            Update.VehicleCode = VehicleType.VehicleCode;
                            Update.VehicleTypeDescription = VehicleType.VehicleTypeDescription;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var DeleteVehicleType = db.VehicleType.Where(o => o.VehicleTypeID == VehicleType.VehicleTypeID).FirstOrDefault();
                            DeleteVehicleType.Active = false;
                            DeleteVehicleType.UpdatedBy = CurrentUser.Details.UserID;
                            DeleteVehicleType.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Vehicle Type Deleted!";
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
                return View(VehicleType);
            }
        }
    }
}