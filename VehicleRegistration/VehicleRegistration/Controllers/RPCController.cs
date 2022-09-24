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
    public class RPCController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: RPC
        public ActionResult RPCList(string RPCtype)
        {
            RPCModel RPC = new RPCModel();
            RPC.RPCtype = RPCtype;
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                switch (RPCtype)
                {
                    case "Region":
                        RPC.RegionList = db.Region.Where(o => o.Active == true).ToList();
                        break;
                    case "Province":
                        RPC.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                        break;
                    case "City":
                        RPC.CityList = db.City.Where(o => o.Active == true).ToList();
                        break;
                    case "Barangay":
                        RPC.BarangayList = db.Barangay.Where(o => o.Active == true).ToList();
                        break;
                    default:
                        return RedirectToAction("Dashboard", "Home");
                }
                return View(RPC);
            }
        }
        [HttpGet]
        public ActionResult RPC(bool Edit, string RPCtype, int? RPCCode)
        {
            ViewBag.Edit = Edit;
            try
            {
                RPCModel RPC = new RPCModel();
                RPC.RPCtype = RPCtype;
                using (db = new VRSystemEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;


                    if (Edit)
                    {
                        switch (RPCtype)
                        {
                            case "Region":
                                var Load1 = db.Region.Where(o => o.Active == true && o.RegionID == RPCCode).ToList().FirstOrDefault();

                                RPC.ProvinceList = db.Province.Where(o => o.Active == true && o.RegionID == Load1.RegionID).ToList();
                                RPC.RPCCode = Load1.RegionID;
                                RPC.RPCName = Load1.RegionName;
                                RPC.RPCNumber = Load1.RegionNumber;

                                break;
                            case "Province":
                                var Load2 = db.Province.Where(o => o.Active == true && o.ProvinceID == RPCCode).ToList().FirstOrDefault();

                                RPC.RegionList = db.Region.Where(o => o.Active == true).ToList();
                                RPC.CityList = db.City.Where(o => o.Active == true && o.ProvinceID == Load2.ProvinceID).ToList();
                                RPC.RPCRefID = Load2.RegionID;
                                RPC.RPCCode = Load2.ProvinceID;
                                RPC.RPCNumber = Load2.ProvinceNumber;
                                RPC.RPCName = Load2.ProvinceName;

                                break;
                            case "City":
                                var Load3 = db.City.Where(o => o.Active == true && o.CityID == RPCCode).ToList().FirstOrDefault();

                                RPC.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                                RPC.RPCRefID = Load3.ProvinceID;
                                RPC.RPCCode = Load3.CityID;
                                RPC.RPCNumber = Load3.CityNumber;
                                RPC.RPCName = Load3.CityName;

                                break;
                            case "Barangay":
                                var Load4 = db.Barangay.Where(o => o.Active == true && o.CityID == RPCCode).ToList().FirstOrDefault();

                                RPC.CityList = db.City.Where(o => o.Active == true).ToList();
                                RPC.RPCRefID = Load4.CityID;
                                RPC.RPCCode = Load4.BarangayID;
                                RPC.RPCName = Load4.BarangayName;

                                break;
                            default:
                                return RedirectToAction("Dashboard", "Home");
                        }
                    }
                    else
                    {
                        switch (RPCtype)
                        {
                            case "Region":
                                break;
                            case "Province":
                                RPC.RegionList = db.Region.Where(o => o.Active == true).ToList();
                                break;
                            case "City":
                                RPC.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                                break;
                            case "Barangay":
                                RPC.CityList = db.City.Where(o => o.Active == true).ToList();
                                break;
                            default:
                                return RedirectToAction("Dashboard", "Home");
                        }
                    }
                    return PartialView(RPC);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("RPCList", new { RPCtype = RPCtype});
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RPC(RPCModel RPC, bool Edit, string RPCtype, string submit)
        {
            RPC.RPCtype = RPCtype;
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            switch (RPCtype)
                            {
                                case "Region":
                                    var Region = new Region
                                    {
                                        RegionID = RPC.RPCCode,
                                        RegionNumber = RPC.RPCNumber.Trim(),
                                        RegionName = RPC.RPCName.Trim(),
                                        CreatedBy = CurrentUser.Details.UserID,
                                        CreatedDate = DateTime.Now,
                                        Active = true
                                    };

                                    db.Region.Add(Region);
                                    db.SaveChanges();
                                    break;
                                case "Province":
                                    var Province = new Province
                                    {
                                        ProvinceID = RPC.RPCCode,
                                        ProvinceNumber = RPC.RPCNumber.Trim(),
                                        ProvinceName = RPC.RPCName.Trim(),
                                        CreatedBy = CurrentUser.Details.UserID,
                                        CreatedDate = DateTime.Now,
                                        Active = true
                                    };

                                    db.Province.Add(Province);
                                    db.SaveChanges();
                                    break;
                                case "City":
                                    var City = new City
                                    {
                                        CityID = RPC.RPCCode,
                                        CityNumber = RPC.RPCNumber.Trim(),
                                        CityName = RPC.RPCName.Trim(),
                                        CreatedBy = CurrentUser.Details.UserID,
                                        CreatedDate = DateTime.Now,
                                        Active = true
                                    };

                                    db.City.Add(City);
                                    db.SaveChanges();
                                    break;
                                case "Barangay":
                                    var Barangay = new Barangay
                                    {
                                        BarangayID = RPC.RPCCode,
                                        BarangayName = RPC.RPCName.Trim(),
                                        CreatedBy = CurrentUser.Details.UserID,
                                        CreatedDate = DateTime.Now,
                                        Active = true
                                    };

                                    db.Barangay.Add(Barangay);
                                    db.SaveChanges();
                                    break;
                            }
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            switch (RPCtype)
                            {
                                case "Region":
                                    var Update1 = db.Region.Where(o => o.RegionID == RPC.RPCCode).FirstOrDefault();
                                    Update1.RegionName = RPC.RPCName.Trim();
                                    Update1.RegionNumber = RPC.RPCNumber.Trim();
                                    Update1.UpdatedBy = CurrentUser.Details.UserID;
                                    Update1.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    break;
                                case "Province":
                                    var Update2 = db.Province.Where(o => o.ProvinceID == RPC.RPCCode).FirstOrDefault();
                                    Update2.RegionID = RPC.RPCRefID;
                                    Update2.ProvinceNumber = RPC.RPCNumber.Trim();
                                    Update2.ProvinceName = RPC.RPCName.Trim();
                                    Update2.UpdatedBy = CurrentUser.Details.UserID;
                                    Update2.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    break;
                                case "City":
                                    var Update3 = db.City.Where(o => o.CityID == RPC.RPCCode).FirstOrDefault();
                                    Update3.ProvinceID = RPC.RPCRefID;
                                    Update3.CityNumber = RPC.RPCNumber.Trim();
                                    Update3.CityName = RPC.RPCName.Trim();
                                    Update3.UpdatedBy = CurrentUser.Details.UserID;
                                    Update3.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    break;
                                case "Barangay":
                                    var Update4 = db.Barangay.Where(o => o.BarangayID == RPC.RPCCode).FirstOrDefault();
                                    Update4.CityID = RPC.RPCRefID;
                                    Update4.BarangayName = RPC.RPCName.Trim();
                                    Update4.UpdatedBy = CurrentUser.Details.UserID;
                                    Update4.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    break;
                            }
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            switch (RPCtype)
                            {
                                case "Region":
                                    var Update1 = db.Region.Where(o => o.RegionID == RPC.RPCCode).FirstOrDefault();
                                    Update1.Active = false;
                                    Update1.UpdatedBy = CurrentUser.Details.UserID;
                                    Update1.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    break;
                                case "Province":
                                    var Update2 = db.Province.Where(o => o.RegionID == RPC.RPCRefID && o.ProvinceID == RPC.RPCCode).FirstOrDefault();
                                    Update2.Active = false;
                                    Update2.UpdatedBy = CurrentUser.Details.UserID;
                                    Update2.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    break;
                                case "City":
                                    var Update3 = db.City.Where(o => o.ProvinceID == RPC.RPCRefID && o.CityID == RPC.RPCCode).FirstOrDefault();
                                    Update3.Active = false;
                                    Update3.UpdatedBy = CurrentUser.Details.UserID;
                                    Update3.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    break;
                                case "Barangay":
                                    var Update4 = db.Barangay.Where(o => o.CityID == RPC.RPCRefID && o.CityID == RPC.RPCCode).FirstOrDefault();
                                    Update4.Active = false;
                                    Update4.UpdatedBy = CurrentUser.Details.UserID;
                                    Update4.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    break;
                            }
                            TempData["WarningMessage"] = "Removed Successfully!";
                        }
                        break;
                }
                return RedirectToAction("RPCList", new { RPCtype = RPCtype });
            }
            else
            {
                switch (RPCtype)
                {
                    case "Region":
                        var Load1 = db.Region.Where(o => o.Active == true).ToList().FirstOrDefault();
                        RPC.ProvinceList = db.Province.Where(o => o.Active == true && o.RegionID == Load1.RegionID).ToList();
                        break;
                    case "Province":
                        var Load2 = db.Province.Where(o => o.Active == true).ToList().FirstOrDefault();
                        RPC.CityList = db.City.Where(o => o.Active == true && o.ProvinceID == Load2.ProvinceID).ToList();
                        break;
                    case "City":
                        var Load3 = db.City.Where(o => o.Active == true).ToList().FirstOrDefault();
                        RPC.BarangayList = db.Barangay.Where(o => o.Active == true && o.CityID == Load3.CityID).ToList();
                        break;
                }
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(RPC);
            }

        }
    }
}