using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;

namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
    public class MVPremiumController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: MVPremium
        public ActionResult Index()
        {

            var model = new MVPremiumModel();
            using (db = new VRSystemEntities())
            {
                model.VehicleTypeList = db.VehicleType.Where(o => o.Active == true)
                    .Select(o => new VehicleTypeList()
                    {
                        VehicleTypeID = o.VehicleTypeID,
                        VehicleCode = o.VehicleCode,
                        VehicleDesc = o.VehicleTypeDescription,
                    }).ToList();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MVPremiumModel model, string submit)
        {
            using (db = new VRSystemEntities())
            {
                switch (submit)
                {
                    case "Create":
                        if (model.addVehicleClassificationList != null)
                        {
                            var insert = new List<MVPremium>();
                            foreach (var list in model.addVehicleClassificationList)
                            {
                                if (list.addIsChecked)
                                {
                                    insert.Add(new MVPremium
                                    {
                                        VehicleTypeID = model.VehicleTypeID,
                                        VehicleClassificationID = list.addVehicleClassificationID,
                                    });
                                }
                            }
                            if (insert != null)
                            {
                                db.MVPremium.AddRange(insert);
                                db.SaveChanges();
                                TempData["SuccessMessage"] = "Successfuly added!";
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "An error has occured!";
                        }
                        break;
                    case "Delete":
                        if (model.vwVehicleClassificationList != null)
                        {
                            var delete = new List<MVPremium>();
                            foreach (var list in model.vwVehicleClassificationList)
                            {
                                if (list.vwIsChecked)
                                {
                                    delete.Add(db.MVPremium.Where(
                                        o => o.VehicleTypeID == o.VehicleTypeID && 
                                        o.VehicleClassificationID == list.vwVehicleClassificationID
                                        ).FirstOrDefault());
                                }
                            }
                            if (delete != null)
                            {
                                db.MVPremium.RemoveRange(delete);
                                db.SaveChanges();
                                TempData["WarningMessage"] = "Removed Successfuly!";
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "An error has occured!";
                        }
                        break;

                }
                model.VehicleTypeList = db.VehicleType.Where(o => o.Active == true)
                    .Select(o => new VehicleTypeList()
                    {
                        VehicleTypeID = o.VehicleTypeID,
                        VehicleCode = o.VehicleCode,
                        VehicleDesc = o.VehicleTypeDescription,
                    }).ToList();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDeleteVehicleClassificationList(int VehicleTypeID)
        {
            using (var db = new VRSystemEntities())
            {
                var result = (from a in db.MVPremium
                              where a.VehicleTypeID == VehicleTypeID
                              join b in db.VehicleClassification on a.VehicleClassificationID equals b.VehicleClassificationID into temp
                              from temptbl in temp.DefaultIfEmpty()
                              select new
                              {
                                  temptbl.VehicleClassificationID,
                                  temptbl.VehicleClassificationName
                              }).Select(o => new vwVehicleClassification()
                              {
                                  vwIsChecked = false,
                                  vwVehicleClassificationID = o.VehicleClassificationID,
                                  vwVehicleClassificationName = o.VehicleClassificationName
                              }).ToList();

                return PartialView("_deleteVehicleCLassification", result);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetAddVehicleClassificationList(int VehicleTypeID)
        {
            using (var db = new VRSystemEntities())
            {
                var result = (from a in db.MVPremium
                              where a.VehicleTypeID == VehicleTypeID
                              join b in db.VehicleClassification on a.VehicleClassificationID equals b.VehicleClassificationID into temp
                              from temptbl in temp.DefaultIfEmpty()
                              select new
                              {
                                  temptbl.VehicleClassificationID,
                                  temptbl.VehicleClassificationName
                              }).Select(o => new vwVehicleClassification()
                              {
                                  vwIsChecked = false,
                                  vwVehicleClassificationID = o.VehicleClassificationID,
                                  vwVehicleClassificationName = o.VehicleClassificationName,
                              }).ToList();

                var vwClassificationID = result.Select(o => o.vwVehicleClassificationID).ToList();

                var ClassificationList = db.VehicleClassification
                    .Where(o => o.Active == true && !vwClassificationID.Contains(o.VehicleClassificationID))
                    .Select(o => new addVehicleClassification()
                    {
                        addIsChecked = false,
                        addVehicleClassificationID = o.VehicleClassificationID,
                        addVehicleClassificationName = o.VehicleClassificationName
                    }).ToList();

                return PartialView("_addVehicleCLassification", ClassificationList);
            }
        }
    }
}