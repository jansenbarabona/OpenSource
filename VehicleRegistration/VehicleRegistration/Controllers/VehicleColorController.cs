using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;
/// <summary>
/// ////////////////Vehicle Color List Registration Here////////////
/// </summary>
namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
    public class VehicleColorController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: VehicleColor
        public ActionResult Vehicle_Color_List()
          
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var vehiclecolorlist = db.VehicleColor.Where(o => o.Active == true).ToList();

                return View(vehiclecolorlist);
            }
        }
        /////how to True Viewbag.edit=False, Here in Vehicle Color Registration Functions///////
        [HttpGet]
        public ActionResult Vehicle_Color(int? id)
        {
            ViewBag.id = id;

            VehicleColorModel NewVehicleColor = new VehicleColorModel();


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

                    var VehicleColorLoad = db.VehicleColor.Where(o => o.Active == true && o.VehicleColorID == id).ToList().FirstOrDefault();

                    NewVehicleColor.VehicleColorID = VehicleColorLoad.VehicleColorID;
                    NewVehicleColor.VehicleColorName = VehicleColorLoad.VehicleColorName;
                }
                return PartialView(NewVehicleColor);
            }
        }

        /////ValidateAntiForgeryToken Here in Vehicle Color Registration Functions///////
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vehicle_Color(VehicleColorModel VehicleColor, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewVehicleColor = new VehicleColor
                            {
                                VehicleColorName = VehicleColor.VehicleColorName.Trim(),
                                Active = true,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now
                            };

                            db.VehicleColor.Add(NewVehicleColor);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.VehicleColor.Where(o => o.VehicleColorID == VehicleColor.VehicleColorID).FirstOrDefault();
                            Update.VehicleColorName = VehicleColor.VehicleColorName.Trim();
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var DeleteVehicleColor = db.VehicleColor.Where(o => o.VehicleColorID == VehicleColor.VehicleColorID).FirstOrDefault();
                            DeleteVehicleColor.Active = false;
                            DeleteVehicleColor.UpdatedBy = CurrentUser.Details.UserID;
                            DeleteVehicleColor.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Vehicle Color Deleted!";
                        }
                        break;
                }
                return RedirectToAction("Vehicle_Color_List");
            }
            else
            {
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(VehicleColor);
            }
        }

    }
}