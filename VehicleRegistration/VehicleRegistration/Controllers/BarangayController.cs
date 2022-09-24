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
    [SessionExpire]
    [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
    public class BarangayController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Barangay
        public ActionResult Index()
        {
            VRSystemEntities db = new VRSystemEntities();
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var barangaylist = db.Barangay.Where(o => o.Active == true).ToList();

                return View(barangaylist);
            }
          
        }
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult Barangay_Registration(int? id)
        {
            ViewBag.id = id;

            BarangayModel NewBarangay = new BarangayModel();

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

                    var Load = db.Barangay.Where(o => o.Active == true && o.BarangayID == id).ToList().FirstOrDefault();
                    //var ProvinceID = db.City.Where(o => o.CityID == Load.CityID).FirstOrDefault().ProvinceID;
                    //NewLTO.CityList = db.City.Where(o => o.ProvinceID == ProvinceID).ToList();
                    //NewLTO.BarangayList = db.Barangay.Where(o => o.CityID == Load.CityID).ToList();

                    NewBarangay.BarangayID = Load.BarangayID;
                    NewBarangay.BarangayName = Load.BarangayName;
                    NewBarangay.CityID = Load.CityID;

                    NewBarangay.CreatedBy = Load.CreatedBy;
                    NewBarangay.CreatedDate = Load.CreatedDate;
                    NewBarangay.Active = Load.Active;

                }
                return PartialView(NewBarangay);
            }
        }

        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        [ValidateAntiForgeryToken]
        public ActionResult Barangay_Registration(Barangay Barangay, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewBarangay = new Barangay
                            {
                                
                                BarangayID = Barangay.BarangayID,
                                BarangayName = Barangay.BarangayName,
                                CityID = Barangay.CityID,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };

                            db.Barangay.Add(NewBarangay);
                            db.SaveChanges();

                            //Functions.Logo(submit, "", lto.LogoFile);
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.Barangay.Where(o => o.BarangayID == Barangay.BarangayID).FirstOrDefault();
                            Update.BarangayID = Barangay.BarangayID;
                            Update.BarangayName = Barangay.BarangayName.Trim();
                            Update.CityID = Barangay.CityID;


                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.Barangay.Where(o => o.BarangayID == Barangay.BarangayID).FirstOrDefault();
                            Update.Active = false;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
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
                return View(Barangay);
            }

        }
    }
}