using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;

namespace VehicleRegistration.Controllers
{
    public class BatchController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Batch
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        public ActionResult GetBatchFilter(string batchID)
        {
            VehicleListModel filterResult = new VehicleListModel();

            switch(batchID.ToLower())
            {
                case "all":
                    switch (CurrentUser.Details.UserEntityID)
                    {
                        case (int)UserEntityEnum.Dealer:
                            {
                                if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                                {
                                    filterResult.VehicleList = db.vwVehicleList
                                      .Where(o => o.DealerID == CurrentUser.Details.ReferenceID
                                      && o.Active == true
                                      ).Select(o => new vwVehicleListModel()
                                      {
                                          VehicleID = o.VehicleID,
                                          VehicleMakeName = o.VehicleMakeName,
                                          VehicleModelName = o.VehicleModelName,
                                          Variant = o.Variant,
                                          Year = o.Year,
                                          EngineNumber = o.EngineNumber,
                                          BodyIDNumber = o.BodyIDNumber,
                                          isChecked = false
                                      }).ToList();
                                }
                                else
                                {
                                    filterResult.VehicleList = db.vwVehicleList
                                      .Where(o => o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                      && o.DealerID == CurrentUser.Details.ReferenceID
                                      && o.Active == true
                                      ).Select(o => new vwVehicleListModel()
                                      {
                                          VehicleID = o.VehicleID,
                                          VehicleMakeName = o.VehicleMakeName,
                                          VehicleModelName = o.VehicleModelName,
                                          Variant = o.Variant,
                                          Year = o.Year,
                                          EngineNumber = o.EngineNumber,
                                          BodyIDNumber = o.BodyIDNumber,
                                          isChecked = false
                                      }).ToList();
                                }
                            }

                            break;
                        case (int)UserEntityEnum.MAI:
                            
                            filterResult.VehicleList = db.vwVehicleList
                          .Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                          && o.Active == true
                          ).Select(o => new vwVehicleListModel()
                          {
                              VehicleID = o.VehicleID,
                              VehicleMakeName = o.VehicleMakeName,
                              VehicleModelName = o.VehicleModelName,
                              Variant = o.Variant,
                              Year = o.Year,
                              EngineNumber = o.EngineNumber,
                              BodyIDNumber = o.BodyIDNumber,
                              isChecked = false
                          }).ToList();
                            break;
                    }
                    break;
                default:
                    var newBatchID = Convert.ToInt32(batchID);
                    filterResult.VehicleList = (from a in db.BatchMaster
                                                where a.BatchID == newBatchID && 
                                                      a.UserReference == CurrentUser.Details.ReferenceID && 
                                                      a.UserSubRef == CurrentUser.Details.SubReferenceID
                                                join b in db.BatchDetails on a.BatchID equals b.BatchID
                                                join c in db.vwVehicleList on b.VehicleID equals c.VehicleID into temp
                                                from temptbl in temp.DefaultIfEmpty()
                                                select new vwVehicleListModel()
                                                {
                                                    VehicleID = temptbl.VehicleID,
                                                    VehicleMakeName = temptbl.VehicleMakeName,
                                                    VehicleModelName = temptbl.VehicleModelName,
                                                    Variant = temptbl.Variant,
                                                    Year = temptbl.Year,
                                                    EngineNumber = temptbl.EngineNumber,
                                                    BodyIDNumber = temptbl.BodyIDNumber,
                                                    isChecked = false
                                                }).ToList();
                    var BatchTypeID = db.BatchMaster.FirstOrDefault(o => o.BatchID == newBatchID).BatchTypeID;
                    switch (BatchTypeID)
                    {
                        case (int)BatchTypeList.NewUpload:
                            {
                                ViewBag.HasCheckBox = true;
                                break;
                            }
                    }

                    break;
            }

            return PartialView("_VehicleList", filterResult);
            //return Json(filterResult, JsonRequestBehavior.AllowGet);
        }
    }
}