using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;

namespace VehicleRegistration.Controllers
{
    public class AirconTypeController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: AirconType
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var AirconTypeList = db.AirconType.Where(o => o.Active == true).ToList();

                return View(AirconTypeList);
            }
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult AirconTypeRegistration(int? id)
        {
            ViewBag.id = id;

            AirconTypeModel NewAirconType = new AirconTypeModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                //NewLTO.BarangayList = new List<Barangay>();
                //NewLTO.CityList = new List<City>();
                //NewLTO.ProvinceList = db.Province.Where(o => o.Active == true).ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;


                    //NewLTO.WalletDetail = Functions.GetWalletDetails((int)UserEntityEnum.LTO, (int)id);
                    //NewLTO.EntityTransaction = db.vwTransactionEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer && o.EntityID == (int)id && o.Active == true).ToList();

                    var Load = db.AirconType.Where(o => o.Active == true && o.AirconTypeID == id).ToList().FirstOrDefault();
                    //var ProvinceID = db.City.Where(o => o.CityID == Load.CityID).FirstOrDefault().ProvinceID;
                    //NewLTO.CityList = db.City.Where(o => o.ProvinceID == ProvinceID).ToList();
                    //NewLTO.BarangayList = db.Barangay.Where(o => o.CityID == Load.CityID).ToList();

                    NewAirconType.AirconTypeID = Load.AirconTypeID;
                    NewAirconType.AirconTypeReference = Load.AirconTypeReference;
                    NewAirconType.AirconTypeDescription = Load.AirconTypeDescription;

                    NewAirconType.CreatedBy = Load.CreatedBy;
                    NewAirconType.CreatedByDate = Load.CreatedByDate;
                    NewAirconType.Active = Load.Active;



                    //NewAirconType.vwDealerInsuranceModelList = (from a in db.DealerInsurance
                    //                                     join b in db.Insurance on a.InsuranceID equals b.InsuranceID into temp
                    //                                     from temptbl in temp.DefaultIfEmpty()
                    //                                     select new
                    //                                     {
                    //                                         LTOID = a.DealerID,
                    //                                         InsuranceID = temptbl.InsuranceID,
                    //                                         InsuranceName = temptbl.InsuranceName,
                    //                                     }).Where(o => o.LTOID == Load.LTOID).Select(
                    //    o => new vwDealerInsuranceModel()
                        //{
                        //    DealerID = o.LTOID,
                        //    InsuranceID = o.InsuranceID,
                        //    InsuranceName = o.InsuranceName,
                        //}).ToList();
                }
                return PartialView(NewAirconType);
            }
        }
       
        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        [ValidateAntiForgeryToken]
        public ActionResult AirconTypeRegistration(AirconType AirconType, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewAirconType = new AirconType
                            {
                                //Address = AirconType.AirconTypeID.Trim(),
                                AirconTypeReference = AirconType.AirconTypeReference.Trim(),
                                AirconTypeDescription = AirconType.AirconTypeDescription?.Trim(),

                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedByDate = DateTime.Now,
                                Active = true
                            };

                            db.AirconType.Add(NewAirconType);
                            db.SaveChanges();

                            //Functions.Logo(submit, "", lto.LogoFile);
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.AirconType.Where(o => o.AirconTypeID == AirconType.AirconTypeID).FirstOrDefault();
                            Update.AirconTypeReference = AirconType.AirconTypeReference.Trim();
                            Update.AirconTypeDescription = AirconType.AirconTypeDescription.Trim();
                           
                           
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            //Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.AirconType.Where(o => o.AirconTypeID == AirconType.AirconTypeID).FirstOrDefault();
                            Update.Active = false;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            //Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            
                            TempData["WarningMessage"] = "Removed Successfully!";
                        }
                        break;
                }
                return RedirectToAction("Index");
            }
            else
            {
                //lto.CityList = db.City.Where(o => o.Active == true).ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(AirconType);
            }

        }
    }
}