using ClosedXML.Excel;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;

namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    public class DealerController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Dealer
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var dealerlist = db.vwDealerList.Where(o => o.Active == true).ToList();

                return View(dealerlist);
            }
        }

        #region DealerInfo

        public ActionResult DealerInfo(int id)
        {
            ViewBag.id = id;

            DealerModel DealerInfoView = new DealerModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = true;
                ViewBag.Edit = true;
                DealerInfoView.CityList = new List<City>();
                DealerInfoView.ProvinceList = db.Province.Where(o => o.Active == true).ToList();

                //DealerInfoView.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                DealerInfoView.WalletDetail = Functions.GetWalletDetails((int)UserEntityEnum.Dealer, (int)id);
                DealerInfoView.WalletDetail.UserEntityID = (int)UserEntityEnum.Dealer;
                DealerInfoView.EntityTransaction = db.vwTransactionEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer && o.EntityID == (int)id && o.Active == true).ToList();

                var Load = db.vwDealerList.Where(o => o.Active == true && o.DealerID == id).ToList().FirstOrDefault();
                var ProvinceID = db.City.Where(o => o.CityID == Load.CityID).FirstOrDefault().ProvinceID;
                DealerInfoView.CityList = db.City.Where(o => o.ProvinceID == ProvinceID).ToList();


                DealerInfoView.WalletDetail.ReferenceID = Load.DealerID;
                DealerInfoView.WalletDetail.SubReferenceID = 0;

                DealerInfoView.DealerID = Load.DealerID;
                DealerInfoView.DealerName = Load.DealerName;
                DealerInfoView.EmailAddress = Load.EmailAddress;
                DealerInfoView.BusinessPhone = Load.BusinessPhone;
                DealerInfoView.MobilePhone = Load.MobilePhone;
                DealerInfoView.FaxNumber = Load.FaxNumber;
                DealerInfoView.Website = Load.Website;
                DealerInfoView.Address = Load.Address;
                DealerInfoView.CityName = Load.CityName;
                DealerInfoView.BarangayName = Load.BarangayName;
                DealerInfoView.ZipCode = Load.ZipCode;
                DealerInfoView.Logo = Load.Logo;
                DealerInfoView.LogoByte = Load.LogoByte;
                DealerInfoView.Notes = Load.Notes;
                //DealerInfoView.CreatedBy = Load.CreatedBy;
                //DealerInfoView.CreatedDate = Load.CreatedDate;
                //DealerInfoView.Active = Load.Active;
                DealerInfoView.SelectedProvinceID = ProvinceID;

                //DealerInfoView.SelectedProvinceID = ProvinceID;


                DealerInfoView.vwDealerInsuranceModelList = (from a in db.DealerInsurance
                                                             join b in db.Insurance on a.InsuranceID equals b.InsuranceID into temp
                                                             from temptbl in temp.DefaultIfEmpty()
                                                             select new
                                                             {
                                                                 DealerID = a.DealerID,
                                                                 InsuranceID = temptbl.InsuranceID,
                                                                 InsuranceName = temptbl.InsuranceName,
                                                             }).Where(o => o.DealerID == Load.DealerID).Select(
                    o => new vwDealerInsuranceModel()
                    {
                        DealerID = o.DealerID,
                        InsuranceID = o.InsuranceID,
                        InsuranceName = o.InsuranceName,
                    }).ToList();


            }
            return PartialView(DealerInfoView);
        }

        #endregion
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerForm(int? id)
        {
            ViewBag.id = id;

            DealerModel NewDealer = new DealerModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                NewDealer.BarangayList = new List<Barangay>();
                NewDealer.CityList = new List<City>();
                NewDealer.ProvinceList = db.Province.Where(o => o.Active == true).ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;


                    NewDealer.WalletDetail = Functions.GetWalletDetails((int)UserEntityEnum.Dealer, (int)id);
                    NewDealer.EntityTransaction = db.vwTransactionEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer && o.EntityID == (int)id && o.Active == true).ToList();

                    var Load = db.Dealer.Where(o => o.Active == true && o.DealerID == id).ToList().FirstOrDefault();
                    var ProvinceID = db.City.Where(o => o.CityID == Load.CityID).FirstOrDefault().ProvinceID;
                    NewDealer.CityList = db.City.Where(o => o.ProvinceID == ProvinceID).ToList();
                    NewDealer.BarangayList = db.Barangay.Where(o => o.CityID == Load.CityID).ToList();

                    NewDealer.DealerID = Load.DealerID;
                    NewDealer.DealerName = Load.DealerName;
                    NewDealer.EmailAddress = Load.EmailAddress;
                    NewDealer.EmailAddress2 = Load.EmailAddress2;
                    NewDealer.BusinessPhone = Load.BusinessPhone;
                    NewDealer.MobilePhone = Load.MobilePhone;
                    NewDealer.FaxNumber = Load.FaxNumber;
                    NewDealer.TIN = Load.TIN;
                    NewDealer.Website = Load.Website;
                    NewDealer.Address = Load.Address;
                    NewDealer.CityID = Load.CityID;
                    NewDealer.BarangayID = Load.BarangayID;
                    NewDealer.ZipCode = Load.ZipCode;
                    NewDealer.Logo = Load.Logo;
                    NewDealer.LogoByte = Load.LogoByte;
                    NewDealer.Notes = Load.Notes;
                    NewDealer.CreatedBy = Load.CreatedBy;
                    NewDealer.CreatedDate = Load.CreatedDate;
                    NewDealer.Active = Load.Active;
                    NewDealer.SelectedProvinceID = ProvinceID;


                    NewDealer.vwDealerInsuranceModelList = (from a in db.DealerInsurance
                                                            join b in db.Insurance on a.InsuranceID equals b.InsuranceID into temp
                                                            from temptbl in temp.DefaultIfEmpty()
                                                            select new
                                                            {
                                                                DealerID = a.DealerID,
                                                                InsuranceID = temptbl.InsuranceID,
                                                                InsuranceName = temptbl.InsuranceName,
                                                            }).Where(o => o.DealerID == Load.DealerID).Select(
                        o => new vwDealerInsuranceModel()
                        {
                            DealerID = o.DealerID,
                            InsuranceID = o.InsuranceID,
                            InsuranceName = o.InsuranceName,
                        }).ToList();
                }
                return PartialView(NewDealer);
            }
        }
        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        [ValidateAntiForgeryToken]
        public ActionResult DealerForm(DealerModel dealer, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewDealer = new Dealer
                            {
                                Address = dealer.Address.Trim(),
                                DealerName = dealer.DealerName.Trim(),
                                MobilePhone = dealer.MobilePhone?.Trim(),
                                Website = dealer.Website?.Trim(),
                                ZipCode = dealer.ZipCode?.Trim(),
                                Notes = dealer.Notes?.Trim(),
                                Logo = dealer.LogoFile != null ? dealer.LogoFile.FileName : null,
                                LogoByte = dealer.LogoFile != null ? dealer.LogoFile.ToByte() : null,
                                FaxNumber = dealer.FaxNumber?.Trim(),
                                EmailAddress = dealer.EmailAddress.Trim(),
                                EmailAddress2 = dealer.EmailAddress2.Trim(),
                                TIN = dealer.TIN.Trim(),
                                BusinessPhone = dealer.BusinessPhone.Trim(),
                                CityID = dealer.CityID,
                                BarangayID = dealer.BarangayID,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };

                            db.Dealer.Add(NewDealer);
                            db.SaveChanges();

                            Functions.Logo(submit, "", dealer.LogoFile);
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.Dealer.Where(o => o.DealerID == dealer.DealerID).FirstOrDefault();
                            Update.Address = dealer.Address.Trim();
                            Update.DealerName = dealer.DealerName.Trim();
                            Update.MobilePhone = dealer.MobilePhone?.Trim();
                            Update.Website = dealer.Website?.Trim();
                            Update.ZipCode = dealer.ZipCode?.Trim();
                            Update.Notes = dealer.Notes;
                            if (dealer.LogoFile != null)
                            {
                                Functions.Logo(submit, Update.Logo, dealer.LogoFile);

                                Update.Logo = dealer.LogoFile != null ? dealer.LogoFile.FileName : null;
                                Update.LogoByte = dealer.LogoFile.ToByte();
                            }
                            Update.FaxNumber = dealer.FaxNumber?.Trim();
                            Update.EmailAddress = dealer.EmailAddress.Trim();
                            Update.EmailAddress2 = dealer.EmailAddress2.Trim();
                            Update.TIN = dealer.TIN.Trim();
                            Update.BusinessPhone = dealer.BusinessPhone.Trim();
                            Update.CityID = dealer.CityID;
                            Update.BarangayID = dealer.BarangayID;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.Dealer.Where(o => o.DealerID == dealer.DealerID).FirstOrDefault();
                            Update.Active = false;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            Functions.Logo(submit, Update.Logo, null);
                            TempData["WarningMessage"] = "Removed Successfully!";
                        }
                        break;
                }
                return RedirectToAction("Index");
            }
            else
            {
                dealer.CityList = db.City.Where(o => o.Active == true).ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(dealer);
            }

        }

        //Dealer Branch
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerBranch()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var dealerbranchlist = new List<vwDealerBranchList>();
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                {

                    dealerbranchlist = db.vwDealerBranchList.Where(o => o.Active == true).ToList();
                }
                else
                {
                    dealerbranchlist = db.vwDealerBranchList.Where(o => o.Active == true
                    && o.DealerID == CurrentUser.Details.ReferenceID
                    ).ToList();
                }


                return View(dealerbranchlist);
            }
        }
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerBranchForm(int? id)
        {
            ViewBag.id = id;

            DealerBranchModel NewDealerBranch = new DealerBranchModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                NewDealerBranch.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                NewDealerBranch.CityList = new List<City>();
                NewDealerBranch.BarangayList = new List<Barangay>();
                NewDealerBranch.DealerList = db.Dealer.Where(o => o.Active == true).ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var Load = db.DealerBranch.Where(o => o.Active == true && o.DealerBranchID == id).ToList().FirstOrDefault();
                    var Province = db.City.Where(o => o.CityID == Load.CityID).FirstOrDefault();

                    if (Province != null)
                    {
                        NewDealerBranch.CityList = db.City.Where(o => o.ProvinceID == Province.ProvinceID).ToList();
                        NewDealerBranch.BarangayList = db.Barangay.Where(o => o.CityID == Load.CityID).ToList();
                        NewDealerBranch.ProvinceID = Province.ProvinceID;
                    }

                    NewDealerBranch.DealerID = Load.DealerID;
                    NewDealerBranch.DealerBranchID = Load.DealerBranchID;
                    NewDealerBranch.DealerBranchName = Load.DealerBranchName;
                    NewDealerBranch.CityID = Load.CityID;
                    NewDealerBranch.EmailAddress = Load.EmailAddress;
                    NewDealerBranch.BusinessPhone = Load.BusinessPhone;
                    NewDealerBranch.MobilePhone = Load.MobilePhone;
                    NewDealerBranch.FaxNumber = Load.FaxNumber;
                    NewDealerBranch.WebSite = Load.WebSite;
                    NewDealerBranch.Address = Load.Address;
                    NewDealerBranch.CityID = Load.CityID;
                    NewDealerBranch.BarangayID = Load.BarangayID;
                    NewDealerBranch.ZipCode = Load.ZipCode;
                    NewDealerBranch.AccreditationNumber = Load.AccreditationNumber;
                    NewDealerBranch.UploadVersion = Load.UploadVersion;
                    NewDealerBranch.CreatedBy = Load.CreatedBy;
                    NewDealerBranch.CreatedDate = Load.CreatedDate;
                    NewDealerBranch.Active = Load.Active;
                    NewDealerBranch.IsMain = Load.IsMain;
                }
                return PartialView(NewDealerBranch);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerBranchForm(DealerBranchModel dealer_branch, HttpPostedFileBase logo, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewDealerBranch = new DealerBranch
                            {
                                DealerID = CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia ? dealer_branch.DealerID : (int)CurrentUser.Details.ReferenceID,
                                DealerBranchName = dealer_branch.DealerBranchName.Trim(),
                                EmailAddress = dealer_branch.EmailAddress.Trim(),
                                BusinessPhone = dealer_branch.BusinessPhone.Trim(),
                                MobilePhone = dealer_branch.MobilePhone.Trim(),
                                FaxNumber = dealer_branch.FaxNumber.Trim(),
                                WebSite = dealer_branch.WebSite.Trim(),
                                Address = dealer_branch.Address.Trim(),
                                CityID = dealer_branch.CityID,
                                BarangayID = dealer_branch.BarangayID,
                                ZipCode = dealer_branch.ZipCode.Trim(),
                                AccreditationNumber = dealer_branch.AccreditationNumber.Trim(),
                                IsMain = dealer_branch.IsMain,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                                
                            };
                            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                                NewDealerBranch.UploadVersion = dealer_branch.UploadVersion;

                            db.DealerBranch.Add(NewDealerBranch);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.DealerBranch.Where(o => o.DealerBranchID == dealer_branch.DealerBranchID).FirstOrDefault();
                            Update.DealerID = dealer_branch.DealerID;
                            Update.DealerBranchName = dealer_branch.DealerBranchName.Trim();
                            Update.EmailAddress = dealer_branch.EmailAddress.Trim();
                            Update.BusinessPhone = dealer_branch.BusinessPhone.Trim();
                            Update.MobilePhone = dealer_branch.MobilePhone.Trim();
                            Update.FaxNumber = dealer_branch.FaxNumber.Trim();
                            Update.WebSite = dealer_branch.WebSite.Trim();
                            Update.Address = dealer_branch.Address.Trim();
                            Update.CityID = dealer_branch.CityID;
                            Update.BarangayID = dealer_branch.BarangayID;
                            Update.ZipCode = dealer_branch.ZipCode.Trim();
                            Update.AccreditationNumber = dealer_branch.AccreditationNumber.Trim();
                            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                                Update.UploadVersion = dealer_branch.UploadVersion;
                            Update.IsMain = dealer_branch.IsMain;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update_active = db.DealerBranch.Where(o => o.DealerBranchID == dealer_branch.DealerBranchID).FirstOrDefault();
                            Update_active.Active = false;
                            Update_active.UpdatedBy = CurrentUser.Details.UserID;
                            Update_active.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Sucessfully!";
                        }
                        break;
                }
                return RedirectToAction("DealerBranch");
            }
            else
            {
                dealer_branch.DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                dealer_branch.CityList = db.City.Where(o => o.Active == true).ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(dealer_branch);
            }

        }

        //Dealer Vehicle Make
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerVehicleMake()
        {
            var dealer = new DealerVehicleMakeModel();
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                if(CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                {
                    dealer.DealerList = db.Dealer.Where(o => o.Active == true).Select(o => new DealerFilter() { DealerID = o.DealerID, DealerName = o.DealerName }).ToList();
                }
                else
                {
                    dealer.vwDealerVehicleMakeModelList = db.vwDealerVehicleMake.Where(o => o.DealerID == CurrentUser.Details.ReferenceID && o.Active == true).ToList();

                    dealer.VehicleMakeModelList = db.VehicleMake.Where(s => !db.DealerVehicleMake.Where(es => es.VehicleMakeID == s.VehicleMakeID && es.DealerID == CurrentUser.Details.ReferenceID && es.Active == true).Any() && s.Active == true).Select(
                        o => new VehicleMakeModel()
                        {
                            VehicleMakeID = o.VehicleMakeID,
                            VehicleMakeName = o.VehicleMakeName,
                            CreatedBy = o.CreatedBy,
                            CreatedDate = o.CreatedDate,
                            Active = o.Active
                        }).ToList();
                }

                return View(dealer);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerVehicleMake(DealerVehicleMakeModel model)
        {
            using (db = new VRSystemEntities())
            {
                foreach (var item in model.VehicleMakeModelList)
                {
                    if (item.isChecked == true)
                    {
                        var insert = new DealerVehicleMake();
                        if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                        {
                            insert = new DealerVehicleMake
                            {
                                DealerID = Convert.ToInt32(model.DealerID),
                                VehicleMakeID = item.VehicleMakeID,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            db.DealerVehicleMake.Add(insert);
                            db.SaveChanges();
                            var dealername = db.Dealer.Where(o => o.DealerID == model.DealerID).FirstOrDefault().DealerName;
                            var vehiclemakename = db.VehicleMake.Where(o => o.VehicleMakeID == item.VehicleMakeID).FirstOrDefault().VehicleMakeName;
                            TempData["SuccessMessage"] = "Vehicle Make (" + vehiclemakename + ") has been added to '" + dealername + "'";
                        }
                        else
                        {
                            insert = new DealerVehicleMake
                            {
                                DealerID = Convert.ToInt32(CurrentUser.Details.ReferenceID),
                                VehicleMakeID = item.VehicleMakeID,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            db.DealerVehicleMake.Add(insert);
                            db.SaveChanges();
                            var vehiclemakename = db.VehicleMake.Where(o => o.VehicleMakeID == item.VehicleMakeID).FirstOrDefault().VehicleMakeName;
                            TempData["SuccessMessage"] = "Vehicle Make (" + vehiclemakename + ") has been added";
                        }
                    }
                }
            }
            return RedirectToAction("DealerVehicleMake");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerVehicleMake_deletes(int dealervehicle_id, int dealerid)
        {
            using (db = new VRSystemEntities())
            {
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                {
                    var Update = db.DealerVehicleMake.Where(o => o.DealerID == dealerid && o.DealerVehicleMakeID == dealervehicle_id).FirstOrDefault();
                    Update.UpdatedBy = CurrentUser.Details.UserID;
                    Update.UpdatedDate = DateTime.Now;
                    Update.Active = false;
                    db.SaveChanges();
                    var vehiclemakename = db.VehicleMake.Where(o => o.VehicleMakeID == Update.VehicleMakeID).FirstOrDefault().VehicleMakeName;
                    TempData["InfoMessage"] = "Vehicle Make ("+ vehiclemakename  + ") has been removed from the list!";
                }
                else if (CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator && CurrentUser.Details.IsMain == true)
                {
                    var Update = db.DealerVehicleMake.Where(o => o.DealerID == CurrentUser.Details.ReferenceID && o.DealerVehicleMakeID == dealervehicle_id).FirstOrDefault();
                    Update.UpdatedBy = CurrentUser.Details.UserID;
                    Update.UpdatedDate = DateTime.Now;
                    Update.Active = false;
                    db.SaveChanges();
                    var vehiclemakename = db.VehicleMake.Where(o => o.VehicleMakeID == Update.VehicleMakeID).FirstOrDefault().VehicleMakeName;
                    TempData["InfoMessage"] = "Vehicle Make (" + vehiclemakename + ") has been removed from the list!";
                }
            }
            return Json("Successful");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDealerVehicleMakeSelected(int DealerID)
        {
            using (db = new VRSystemEntities())
            {
                List<vwDealerVehicleMake> VehicleMakeList = new List<vwDealerVehicleMake>();

                VehicleMakeList = db.vwDealerVehicleMake.Where(o => o.DealerID == DealerID && o.Active == true).ToList();

                return PartialView("_VehicleMakeSelectedList", VehicleMakeList);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDealerVehicleMake(int DealerID)
        {
            using (db = new VRSystemEntities())
            {
                List<VehicleMakeModel> VehicleMakeList = new List<VehicleMakeModel>();

                VehicleMakeList = db.VehicleMake.Where(s => !db.DealerVehicleMake.Where(es => es.VehicleMakeID == s.VehicleMakeID && es.DealerID == DealerID && es.Active == true).Any() && s.Active == true).Select(
                    o => new VehicleMakeModel()
                    {
                        VehicleMakeID = o.VehicleMakeID,
                        VehicleMakeName = o.VehicleMakeName,
                        CreatedBy = o.CreatedBy,
                        CreatedDate = o.CreatedDate,
                        Active = o.Active
                    }).ToList();

                return PartialView("_VehicleMakeList", VehicleMakeList);
            }
        }

        //Dealer Invoice
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult InvoiceList()
        {
            ViewBag.Title = "Invoice List";
            using (db = new VRSystemEntities())
            {
                List<vwDealerInvoiceModel> DealerInvoiceList = new List<vwDealerInvoiceModel>();
                try
                {
                    int UserDealerBranchID = Convert.ToInt32(CurrentUser.Details.SubReferenceID);
                    DealerInvoiceList = (from a in db.vwDealerInvoice
                                         where a.DealerBranchID == UserDealerBranchID
                                         join b in db.vwCustomerList on a.CustomerID equals b.CustomerID into temp
                                         from temptbl in temp.DefaultIfEmpty()
                                         select new vwDealerInvoiceModel()
                                         {
                                             InvoiceNumber = a.InvoiceNumber,
                                             COC = a.COC,
                                             FirstName = a.FirstName,
                                             MiddleName = a.MiddleName,
                                             LastName = a.LastName,
                                             CorpName = a.CorpName,
                                             InvoiceID = a.InvoiceID,
                                             TitleTypeID = temptbl.TitleTypeID,
                                         }).ToList();
                }
                catch (Exception)
                {

                    TempData["ErrorMessage"] = "An error has occured.";
                }



                return View(DealerInvoiceList);
            }
        }
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult Invoice(int? id, int? VehicleID)
        {
            using (db = new VRSystemEntities())
            {
                var Invoice = new DealerInvoiceModel();

                DealerInvoice LoadInvoice = null;
                if (id != null)
                    LoadInvoice = db.DealerInvoice.Where(o => o.InvoiceID == id).FirstOrDefault();
                else if (VehicleID != null)
                    LoadInvoice = db.DealerInvoice.Where(o => o.VehicleID == VehicleID).FirstOrDefault();

                Invoice.FormOrigin = "Invoice";

                db.Configuration.LazyLoadingEnabled = false;

                Invoice.CustomerList = (from a in db.Customer
                                        join b in db.Title on a.TitleID equals b.TitleID into temp
                                        from temptbl in temp.DefaultIfEmpty()
                                        select new
                                        {
                                            a.DealerID,
                                            a.CustomerID,
                                            a.FirstName,
                                            a.MiddleName,
                                            a.LastName,
                                            a.CorpName,
                                            temptbl.TitleTypeID,
                                        })
                                       .Where(o => o.DealerID == CurrentUser.Details.ReferenceID)
                                       .Select(o => new CustomerModel()
                                       {
                                           CustomerID = o.CustomerID,
                                           FirstName = o.FirstName,
                                           MiddleName = o.MiddleName,
                                           LastName = o.LastName,
                                           CorpName = o.CorpName,
                                           TitleTypeID = o.TitleTypeID
                                       })
                                       .ToList();


                //Invoice.VehicleTypeList = db.VehicleType
                //    .Where(o => o.Active == true)
                //    .Select(o => new VehicleTypeList()
                //    {
                //        VehicleTypeID = o.VehicleTypeID,
                //        VehicleCode = o.VehicleCode,
                //        VehicleDesc = o.VehicleTypeDescription
                //    }).ToList();

                Invoice.VehicleClassificationList = db.VehicleClassification.Where(o => o.Active == true).ToList();

                Invoice.TaxTypeList = db.CTPLTaxType.ToList();

                if (LoadInvoice == null)
                {
                    ViewBag.Edit = false;

                    var InvoiceVehicleIDList = new List<int>();

                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        InvoiceVehicleIDList = db.vwDealerInvoice.Where(o => o.DealerID == CurrentUser.Details.ReferenceID)
                                                        .Select(o => o.VehicleID)
                                                        .ToList();
                    }
                    else
                    {
                        InvoiceVehicleIDList = db.vwDealerInvoice.Where(o => o.DealerBranchID == CurrentUser.Details.SubReferenceID)
                                                        .Select(o => o.VehicleID)
                                                        .ToList();
                    }

                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        Invoice.VehicleList = db.vwVehicleList
                                        .Where(o => o.DealerID == CurrentUser.Details.ReferenceID
                                        && o.CertificateOfStockReport != null
                                        && o.CSRNumber != null
                                        && o.Active == true
                                        && !InvoiceVehicleIDList.Contains(o.VehicleID))
                                        .ToList();
                    }
                    else
                    {
                        Invoice.VehicleList = db.vwVehicleList
                                        .Where(o => o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                        && o.CertificateOfStockReport != null
                                        && o.CSRNumber != null
                                        && o.Active == true
                                        && !InvoiceVehicleIDList.Contains(o.VehicleID))
                                        .ToList();
                    }


                }
                else
                {
                    Invoice.InvoiceID = LoadInvoice.InvoiceID;
                    Invoice.SelectedCustomerID = LoadInvoice.CustomerID;
                    Invoice.InvoiceNumber = LoadInvoice.InvoiceNumber;
                    Invoice.InvoiceDate = LoadInvoice.InvoiceDate;
                    Invoice.InvoiceByte = LoadInvoice.InvoiceByte;
                    Invoice.InvoiceContentType = LoadInvoice.InvoiceContentType;
                    Invoice.COC = LoadInvoice.COC;
                    Invoice.COCByte = LoadInvoice.COCByte;
                    Invoice.COCContentType = LoadInvoice.COCContentType;
                    Invoice.COCAuthenticationCode = LoadInvoice.COCAuthenticationCode;
                    Invoice.SelectedVehicleID = LoadInvoice.VehicleID;
                    Invoice.VehicleCost = LoadInvoice.VehicleCost;
                    Invoice.PreferredEndingPlateNumber = LoadInvoice.PreferredEndingPlateNumber;
                    Invoice.VehicleClassificationID = LoadInvoice.VehicleClassificationID;
                    //Invoice.VehicleTypeID = LoadInvoice.VehicleTypeID;
                    Invoice.TaxTypeID = LoadInvoice.TaxTypeID;
                    Invoice.Encumbered = LoadInvoice.Encumbered;
                    Invoice.FinancialInstitution = LoadInvoice.FinancialInstitution;

                    Invoice.VehicleList = db.vwVehicleList
                                          .Where(o => o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                          && o.CertificateOfStockReport != null
                                          && o.CSRNumber != null
                                          && o.Active == true
                                          )
                                          .ToList();

                    //Invoice.VehicleClassificationList = (from a in db.MVPremium
                    //                                     where a.VehicleTypeID == LoadInvoice.VehicleTypeID
                    //                                     join b in db.VehicleClassification on a.VehicleClassificationID equals b.VehicleClassificationID into temp
                    //                                     from temptbl in temp.DefaultIfEmpty()
                    //                                     select new
                    //                                     {
                    //                                         temptbl
                    //                                     })
                    //                .Select(o => o.temptbl).ToList();

                    ViewBag.Edit = true;
                }

                if (VehicleID != null || Invoice.SelectedVehicleID > 0)
                {
                    int SearchID;
                    if (VehicleID != null)
                        SearchID = (int)VehicleID;
                    else
                        SearchID = Invoice.SelectedVehicleID;

                    var VehicleInfo = db.vwVehicleList.Where(o => o.VehicleID == SearchID).FirstOrDefault();

                    if (VehicleInfo != null)
                    {
                        Invoice.VehicleInfo.VehicleMakeName = VehicleInfo.VehicleMakeName;
                        Invoice.VehicleInfo.VehicleModelName = VehicleInfo.VehicleModelName;
                        Invoice.VehicleInfo.EngineNumber = VehicleInfo.EngineNumber;
                        Invoice.VehicleInfo.ChassisNumber = VehicleInfo.ChassisNumber;
                        Invoice.SelectedVehicleID = VehicleInfo.VehicleID;
                    }

                }
                return View(Invoice);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult Invoice(DealerInvoiceModel Invoice, string submit)
        {
            if (!Invoice.Encumbered)
                ModelState.Remove("FinancialInstitution");

            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            if (db.DealerInvoice.Where(o => o.VehicleID == Invoice.SelectedVehicleID && o.Active == true).Count() == 0)
                            {
                                var NewInvoice = new DealerInvoice
                                {
                                    CustomerID = Invoice.SelectedCustomerID,
                                    InvoiceNumber = Invoice.InvoiceNumber,
                                    InvoiceDate = Invoice.InvoiceDate,
                                    InvoiceByte = null,
                                    InvoiceContentType = null,
                                    DealerBranchID = Convert.ToInt32(CurrentUser.Details.SubReferenceID),
                                    //COC = Invoice.COC.Trim(),
                                    //COCByte = Invoice.COCFile.IsImage() ? Invoice.COCFile.ToByte() : null,
                                    //COCAuthenticationCode = Invoice.COCAuthenticationCode.Trim(),
                                    VehicleID = Invoice.SelectedVehicleID,
                                    VehicleCost = Invoice.VehicleCost,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true,
                                    PreferredEndingPlateNumber = Invoice.PreferredEndingPlateNumber ?? null,
                                    //VehicleTypeID = Invoice.VehicleTypeID,
                                    //VehicleClassificationID = Invoice.VehicleClassificationID,
                                    //TaxTypeID = Invoice.TaxTypeID,
                                    //COCPolicyNumber = Invoice.COCPolicyNumber ?? null,   
                                    AffidavitOfConversion = Invoice.AffidavitOfConversionFile != null ? Invoice.AffidavitOfConversionFile.FileName : null,
                                    AffidavitOfConversionContentType = Invoice.AffidavitOfConversionFile != null ? Invoice.AffidavitOfConversionFile.ContentType : null,
                                    AffidavitOfConversionByte = Invoice.AffidavitOfConversionFile != null ? Invoice.AffidavitOfConversionFile.ToByte() : null,
                                    Encumbered = Invoice.Encumbered,
                                    FinancialInstitution = Invoice.FinancialInstitution?.Trim()

                                };

                                if (Invoice.InvoiceFile != null && Invoice.InvoiceFile.IsValidFileSize())
                                {
                                    NewInvoice.InvoiceByte = Invoice.InvoiceFile.IsImage() ? Invoice.InvoiceFile.ToByte() : null;
                                    NewInvoice.InvoiceContentType = Invoice.InvoiceFile.ContentType;
                                }
                                else
                                {
                                    TempData["WarningMessage"] = "Please upload valid file size of less than 1 MB!";
                                }

                                db.DealerInvoice.Add(NewInvoice);
                                db.SaveChanges();
                                TempData["SuccessMessage"] = "Created Successfully!";
                            }
                            else
                            {
                                TempData["WarningMessage"] = "This vehicle is already have invoice!";
                            }
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.DealerInvoice.Where(o => o.InvoiceID == Invoice.InvoiceID).FirstOrDefault();
                            Update.CustomerID = Invoice.SelectedCustomerID;
                            Update.InvoiceNumber = Invoice.InvoiceNumber.Trim();
                            Update.InvoiceDate = Invoice.InvoiceDate;
                            if (Invoice.InvoiceFile != null && Invoice.InvoiceFile.IsValidFileSize())
                            {
                                Update.InvoiceByte = Invoice.InvoiceFile.IsImage() ? Invoice.InvoiceFile.ToByte() : null;
                                Update.InvoiceContentType = Invoice.InvoiceFile.ContentType;
                            }
                            else
                            {
                                TempData["WarningMessage"] = "Please upload valid file size of less than 1 MB!";
                            }
                            //Update.COC = Invoice.COC;
                            //if (Invoice.COCFile != null)
                            //    Update.COCByte = Invoice.COCFile.ToByte();
                            //Update.COCAuthenticationCode = Invoice.COCAuthenticationCode.Trim();
                            Update.DealerBranchID = Convert.ToInt32(CurrentUser.Details.SubReferenceID);
                            Update.VehicleID = Invoice.SelectedVehicleID;
                            Update.VehicleCost = Invoice.VehicleCost;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            Update.PreferredEndingPlateNumber = Invoice.PreferredEndingPlateNumber ?? null;
                            //Update.VehicleTypeID = Invoice.VehicleTypeID;
                            //Update.VehicleClassificationID = Invoice.VehicleClassificationID;
                            //Update.TaxTypeID = Invoice.TaxTypeID;
                            //Update.COCPolicyNumber = Invoice.COCPolicyNumber ?? null;
                            Update.AffidavitOfConversion = Invoice.AffidavitOfConversionFile != null ? Invoice.AffidavitOfConversionFile.FileName : null;
                            Update.AffidavitOfConversionContentType = Invoice.AffidavitOfConversionFile != null ? Invoice.AffidavitOfConversionFile.ContentType : null;
                            Update.AffidavitOfConversionByte = Invoice.AffidavitOfConversionFile != null ? Invoice.AffidavitOfConversionFile.ToByte() : null;
                            Update.Encumbered = Invoice.Encumbered;
                            Update.FinancialInstitution = Invoice.FinancialInstitution?.Trim();

                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Delete = db.DealerInvoice.Where(o => o.InvoiceID == Invoice.InvoiceID).FirstOrDefault();
                            Delete.Active = false;
                            Delete.UpdatedBy = CurrentUser.Details.UserID;
                            Delete.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Successfully!";
                        }
                        break;
                }

                if (Invoice.FormOrigin == "VehicleInfo")
                {

                    return RedirectToAction("VehicleInfo/" + Invoice.SelectedVehicleID, "VehicleInfo");
                }

                return RedirectToAction("InvoiceList");
            }
            else
            {
                Invoice.CustomerList = (from a in db.Customer
                                        join b in db.Title on a.TitleID equals b.TitleID into temp
                                        from temptbl in temp.DefaultIfEmpty()
                                        select new
                                        {
                                            a.DealerID,
                                            a.CustomerID,
                                            a.FirstName,
                                            a.MiddleName,
                                            a.LastName,
                                            a.CorpName,
                                            temptbl.TitleTypeID,
                                        })
                                       .Where(o => o.DealerID == CurrentUser.Details.ReferenceID)
                                       .Select(o => new CustomerModel()
                                       {
                                           CustomerID = o.CustomerID,
                                           MiddleName = o.MiddleName,
                                           FirstName = o.FirstName,
                                           LastName = o.LastName,
                                           CorpName = o.CorpName,
                                           TitleTypeID = o.TitleTypeID
                                       })
                                       .ToList();

                Invoice.SelectedCustomerID = Invoice.SelectedCustomerID;
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;



                if (ViewBag.WhatForm == "VehicleInfo")
                {
                    return RedirectToAction("VehicleInfo/" + Invoice.SelectedVehicleID, "VehicleInfo");
                }
                return View(Invoice);
            }

        }

        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult GetVehicleInformation(int VehicleID)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.vwVehicleList.Where(o => o.VehicleID == VehicleID)
                    .Select(o => new
                    {
                        o.VehicleMakeName,
                        o.VehicleModelID,
                        o.VehicleModelName,
                        o.EngineNumber,
                        o.ChassisNumber
                    }).FirstOrDefault();

                var newsearchresult = db.VehicleModel.Where(o => o.VehicleModelID == searchresult.VehicleModelID)
                    .Select(o => new
                    {
                        searchresult.VehicleMakeName,
                        searchresult.VehicleModelID,
                        searchresult.VehicleModelName,
                        searchresult.EngineNumber,
                        searchresult.ChassisNumber,
                        o.VehicleClassificationID
                    }).FirstOrDefault();
                return Json(newsearchresult, JsonRequestBehavior.AllowGet);
            }
        }

        //Dealer Insurance
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerInsuranceList()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                List<DealerModel> dealerlist = new List<DealerModel>();

                dealerlist = db.Dealer.Where(o => o.Active == true).Select(
                    o => new DealerModel()
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName,
                        EmailAddress = o.EmailAddress,
                    }).ToList();

                return View(dealerlist);
            }
        }
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerInsurance(int? dealer_id)
        {
            try
            {
                var dealerinsurance = new DealerInsuranceModel();
                using (db = new VRSystemEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    dealerinsurance = db.Dealer.Where(o => o.Active == true && o.DealerID == dealer_id).Select(
                        o => new DealerInsuranceModel()
                        {
                            DealerID = o.DealerID,
                            DealerName = o.DealerName,
                        }).FirstOrDefault();

                    dealerinsurance.vwDealerInsuranceModeList = (from a in db.DealerInsurance
                                                                 join b in db.Insurance on a.InsuranceID equals b.InsuranceID into temp
                                                                 from temptbl in temp.DefaultIfEmpty()
                                                                 select new
                                                                 {
                                                                     DealerID = a.DealerID,
                                                                     InsuranceID = temptbl.InsuranceID,
                                                                     InsuranceName = temptbl.InsuranceName,
                                                                     Active = a.Active
                                                                 }).Where(o => o.DealerID == dealer_id && o.Active == true).Select(
                        o => new vwDealerInsuranceModel()
                        {
                            DealerID = o.DealerID,
                            InsuranceID = o.InsuranceID,
                            InsuranceName = o.InsuranceName,
                        }).ToList();
                    dealerinsurance.DealerInsuranceModelList = db.Insurance.Where(s => !db.DealerInsurance.Where(es => es.InsuranceID == s.InsuranceID && es.DealerID == dealer_id && es.Active == true).Any()).Select(
                        o => new DealerInsuranceModel()
                        {
                            InsuranceID = o.InsuranceID,
                            InsuranceName = o.InsuranceName,
                            CreatedBy = o.CreatedBy,
                            CreatedDate = o.CreatedDate,
                            Active = o.Active
                        }).ToList();


                    return View(dealerinsurance);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("DealerInsuranceList");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult DealerInsurance(DealerInsuranceModel model, string submit)
        {

            switch (submit)
            {
                case "Save":
                    using (db = new VRSystemEntities())
                    {
                        foreach (var item in model.DealerInsuranceModelList)
                        {
                            if (item.isChecked == true)
                            {
                                var insert = new DealerInsurance
                                {
                                    DealerID = model.DealerID,
                                    InsuranceID = item.InsuranceID,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true
                                };
                                db.DealerInsurance.Add(insert);
                                db.SaveChanges();
                                TempData["SuccessMessage"] = "Insurance has been added!";
                            }
                        }


                    }
                    break;
                case "Delete":
                    using (db = new VRSystemEntities())
                    {
                        var Update = db.DealerInsurance.Where(o => o.InsuranceID == model.InsuranceID && o.DealerID == model.DealerID).FirstOrDefault();
                        Update.UpdatedBy = CurrentUser.Details.UserID;
                        Update.UpdatedDate = DateTime.Now;
                        Update.Active = false;
                        db.SaveChanges();
                        TempData["ErrorMessage"] = "Insurance has been removed!";
                    }
                    break;
            }

            return RedirectToAction("DealerInsurance", new { dealer_id = model.DealerID });
        }

        [HttpGet]
        public ActionResult DealerTimelineReport()
        {
            DealerTimeLineReportModel model = new DealerTimeLineReportModel();
            using (db = new VRSystemEntities())
            {
                model.DealerList = new List<Dealer>();
                model.DealerList.Add(new Dealer() { DealerID = 0, DealerName = "ALL" });
                model.DealerList.AddRange(db.Dealer.Where(o => o.Active == true)
                    //.Select(o => new Dealer() { DealerID = o.DealerID, DealerName = o.DealerName })
                    .ToList()
                    );

            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DealerTimelineReport(DealerTimeLineReportModel model)
        {
            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
            {
                string path = TimelineandProgressReport((DateTime)model.DateFrom, (DateTime)model.DateTo, model.SelectedDealerID);
            }
            else if(CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
            {
                string path = TimelineandProgressReport((DateTime)model.DateFrom, (DateTime)model.DateTo, CurrentUser.Details.ReferenceID.Value);
            }
            using (db = new VRSystemEntities())
            {
                model.DealerList = new List<Dealer>();
                model.DealerList.Add(new Dealer() { DealerID = 0, DealerName = "ALL" });
                model.DealerList.AddRange(db.Dealer.Where(o => o.Active == true)
                    //.Select(o => new Dealer() { DealerID = o.DealerID, DealerName = o.DealerName })
                    .ToList());
            }
            return View(model);
        }


        public string TimelineandProgressReport(DateTime DateFrom, DateTime DateTo, int DealerID)
        {
            
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "Timeline and Progress Report.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            //else
            //{
            //    return RedirectToAction("Index");
            //}

            DataTable dt = new DataTable("DataSet1");
            dt.Columns.Add(new DataColumn("BatchID", typeof(string)));
            dt.Columns.Add(new DataColumn("Reference", typeof(string)));
            dt.Columns.Add(new DataColumn { ColumnName = "BatchCount", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "DateSubmitted", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "DownloadedDate", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "DateAssessed", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "DatePaid", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "DateCompleted", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "ForPickUpDate", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "ProcessingDays", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "PaidDays", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "DealerName", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "DealerBranch", DataType = typeof(string), AllowDBNull = true });
            //dt.Columns.Add(new DataColumn("Desc", typeof(string)) { AllowDBNull = true });
            //dt.Columns.Add(new DataColumn("DateSubmitted", typeof(DateTime)));
            //dt.Columns.Add(new DataColumn("DateAssesed", typeof(DateTime)) { AllowDBNull = true });
            //dt.Columns.Add(new DataColumn("DatePaid", typeof(DateTime)) { AllowDBNull = true });
            //dt.Columns.Add(new DataColumn("DateComplete", typeof(DateTime)) { AllowDBNull = true });
            //dt.Columns.Add(new DataColumn("ProcessingDays", typeof(string)) { AllowDBNull = true });



            List<BatchMaster> Header = new List<BatchMaster>();
            var DealerName = "";
            using (db = new VRSystemEntities())
            {
                var queryDateTo = DateTo.AddDays(1);
                if (DealerID == 0)
                {
                    Header = db.BatchMaster
                 .Where(o => (DateFrom <= o.DateSubmitted && queryDateTo >= o.DateSubmitted && o.BatchTypeID == 4) /*&& o.UserReference == DealerID*/)
                 .ToList();
                    DealerName = "ALL";
                }
             
                else
                {
                    Header = db.BatchMaster
                   .Where(o => (DateFrom <= o.DateSubmitted && queryDateTo >= o.DateSubmitted) && o.UserReference == DealerID && o.BatchTypeID == 4)
                   .ToList();
                    DealerName = db.Dealer.Where(o => o.DealerID == DealerID).Select(o => o.DealerName).FirstOrDefault();
                }
                   

                foreach (var item in Header)
                {
                    //TimeSpan? ProcessingDays;
                    int ProcessingDays;
                    int? PaidDays = null;
                    if (item.CompletedDate != null)
                    {
                        ProcessingDays = item.DateSubmitted.Value.Date.BusinessDaysUntil(item.CompletedDate.Value.Date);
                        if (item.PaymentDate != null)
                            PaidDays = item.PaymentDate.Value.Date.BusinessDaysUntil(item.CompletedDate.Value.Date);
                    }
                    else
                    {
                        ProcessingDays = item.DateSubmitted.Value.Date.BusinessDaysUntil(DateTime.Today.Date);
                        if (item.PaymentDate != null)
                            PaidDays = item.PaymentDate.Value.Date.BusinessDaysUntil(DateTime.Today.Date);
                    }

                    var Dealer = db.Dealer.Where(o => o.DealerID == item.UserReference).FirstOrDefault().DealerName;
                    var Branch = db.DealerBranch.Where(o => o.DealerID == item.UserReference && o.DealerBranchID == item.UserSubRef).FirstOrDefault().DealerBranchName;

                    dt.Rows.Add(
                        
                        item.BatchID,
                        item.ReferenceNo,
                        item.BatchCount,
                        (DateTime?)item.DateSubmitted,
                        (DateTime?)item.DownloadedDate,
                        (DateTime?)item.AssessedDate,
                        (DateTime?)item.PaymentDate,
                        (DateTime?)item.CompletedDate,
                        (DateTime?)item.ForPickUpDate,
                        ProcessingDays,
                        PaidDays,
                        Dealer,
                        Branch
                        );
                }
            }

            //string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            //lr.EnableExternalImages = true;
            //lr.EnableHyperlinks = true;

            ReportParameter[] prm = new ReportParameter[3];
            prm[0] = new ReportParameter("DealerParameter", DealerName);
            prm[1] = new ReportParameter("DateFromParameter", DateFrom.Date.ToShortDateString());
            prm[2] = new ReportParameter("DateToParameter", DateTo.Date.ToShortDateString());
            lr.SetParameters(prm);

            ReportDataSource rds = new ReportDataSource("DataSet1", dt);
            lr.DataSources.Clear();
            lr.DataSources.Add(rds);

            lr.Refresh();


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo = "";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            //return File(renderedBytes, mimeType);
            //Response.Buffer = true;
            //Response.Clear();
            //Response.ContentType = mimeType;
            //Response.AddHeader("content-disposition", "attachment; filename=Timeline and Progress Report." + fileNameExtension);
            //Response.BinaryWrite(renderedBytes); // create the file
            //Response.Flush(); // send it to the client to download

            var timecreated = DateTime.Now.ToString("MMddyyyyhhmmss");
            ViewBag.Path = "TimelineReport - " + timecreated + ".pdf";
            var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "TimelineReport - " + timecreated + ".pdf";


            //var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "Report.pdf";
            //System.IO.File.Delete(pdfPath);
            //ViewBag.Path = "Report.pdf";
            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }
            //ViewBag.pfpath = pdfPath;
            return pdfPath;
        }


        [HttpGet]
        public ActionResult VehicleRegistrationReport()
        {
            VRReportModel model = new VRReportModel();
            using (db = new VRSystemEntities())
            {
                model.DealerList = new List<Dealer>();
                model.DealerList.Add(new Dealer() { DealerID = 0, DealerName = "Select Dealer" });
                model.DealerList.AddRange(db.Dealer.Where(o => o.Active == true)
                    //.Select(o => new Dealer() { DealerID = o.DealerID, DealerName = o.DealerName })
                    .ToList()
                    );
                model.DealerBranchList = new List<DealerBranch>();
                model.DealerBranchList.Add(new DealerBranch() { DealerBranchID = 0, DealerBranchName = "Select Branch" });
                //model.DealerBranchList.AddRange(db.DealerBranch.Where(o => o.Active == true)
                //.Select(o => new Dealer() { DealerID = o.DealerID, DealerName = o.DealerName })
                //.ToList()
                // );
                model.DealerBranchList.ToList();
            }
            return View(model);
        }

        public ActionResult GetBranchList(int Did)
        {
            VRReportModel model = new VRReportModel();
            using (db = new VRSystemEntities())
            {
                GetBranch(Did);

            }
            return View(model);
        }

        public ActionResult GetBranch(decimal DealerCode)
        {
            ViewBag.DealerCode = DealerCode;
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.DealerBranch.Where(o => o.DealerID == DealerCode && o.Active == true).OrderBy(o => o.DealerBranchName).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VehicleRegistrationReport(VRReportModel model)
        {

            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
            {
                if (model.SelectedDealerID != 0)
                {
                    string path = VRRProgress((DateTime)model.DateFrom, (DateTime)model.DateTo, model.DealerBranchID);

                }
            }
            else if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
            {
                string path = VRRProgress((DateTime)model.DateFrom, (DateTime)model.DateTo, CurrentUser.Details.ReferenceID.Value);
            }

            using (db = new VRSystemEntities())
            {
                model.DealerList = new List<Dealer>();
                model.DealerList.Add(new Dealer() { DealerID = 0, DealerName = "Select Dealer" });
                model.DealerList.AddRange(db.Dealer.Where(o => o.Active == true).ToList());

                //GetBranch(model.SelectedDealerID());
                model.DealerBranchList = new List<DealerBranch>();
                //model.DealerBranchList.Add(new DealerBranch() { DealerBranchID = 0, DealerBranchName = "Select Branch" });
                model.DealerBranchList.AddRange(db.DealerBranch.Where(o => o.DealerID == model.SelectedDealerID && o.Active == true).ToList());
            }
            return View(model);
        }
        public string VRRProgress(DateTime DateFrom, DateTime DateTo, int DealerBranchID)
        {
            ViewBag.DateFrom = DateFrom.Month.ToString() + "/" + DateFrom.Day.ToString() + "/" + DateFrom.Year.ToString() + " 12:00:00 am"; ;
            ViewBag.DateTo = DateTo.Month.ToString() + "/" + DateTo.Day.ToString() + "/" + DateTo.Year.ToString() + " 11:59:59 pm";
            ViewBag.DealerBranchID = DealerBranchID;
            var lr = new LocalReport
            {
                ReportPath = Path.Combine(Server.MapPath("~/Reports/RDLC"), "VRReport.rdlc"),
                EnableExternalImages = true
            };


            if (System.IO.File.Exists(lr.ReportPath))
            {
                //lr.ReportPath = path;
            }


            DataTable dtable = new DataTable("DataSet1");

            dtable.Columns.Add(new DataColumn { ColumnName = "InvoiceDate", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "DateSubmitted", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "ReceivedDate", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "VehicleMakeName", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "VehicleModelName", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "VehicleColorName", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "Buyer", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "VehicleCost", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "Dealer", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "Branch", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "DateFrom", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "DateTo", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "DateRun", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "Count", DataType = typeof(string), AllowDBNull = true });

            var conString = System.Configuration.ConfigurationManager.ConnectionStrings["VRSystemConnectionString"];
            string strConnString = conString.ConnectionString;

            int count = 0;
            string FDate = DateFrom.Month.ToString() + "/" + DateFrom.Day.ToString() + "/" + DateFrom.Year.ToString() + " 12:00:00 am";
            string TDate = DateTo.Month.ToString() + "/" + DateTo.Day.ToString() + "/" + DateTo.Year.ToString() + " 11:59:59 pm";
            string FromD = DateFrom.Month.ToString() + "/" + DateFrom.Day.ToString() + "/" + DateFrom.Year.ToString();
            string ToD = DateTo.Month.ToString() + "/" + DateTo.Day.ToString() + "/" + DateTo.Year.ToString();

            string RDate = DateTime.Today.Month.ToString() + "/" + DateTime.Today.Day.ToString() + "/" + DateTime.Today.Year.ToString();

            using (SqlConnection sqlCon = new SqlConnection(strConnString))
            using (SqlCommand sqlCom = new SqlCommand("SELECT i.InvoiceDate, bm.DateSubmitted, bm.ReceivedDate, vm.VehicleMakeName, vmo.VehicleModelName, vc.VehicleColorName, c.FirstName +' '+ c.Lastname as Buyer, i.VehicleCost, d.DealerName, db.DealerBranchName " +
                    "From DealerInvoice i, Dealer d, DealerBranch db, BatchMaster bm, VehicleInfo vi, VehicleMake vm, VehicleModel vmo, VehicleColor vc, Customer c " +
                    "where db.DealerBranchID = '" + DealerBranchID + "' " +
                    "AND DateSubmitted BETWEEN '" + FDate + "' AND '" + TDate + "' " +
                    "AND i.DealerBranchID = db.DealerBranchID " +
                    "AND i.CustomerID = c.CustomerID " +
                    "AND i.VehicleID = vi.VehicleID " +
                    "AND vi.VehicleColorID = vc.VehicleColorID " +
                    "AND vi.VehicleMakeID = vm.VehicleMakeID " +
                    "AND vi.VehicleModelID = vmo.VehicleModelID " +
                    "AND d.DealerID = db.DealerID " +
                    "AND bm.UserSubRef = db.DealerBranchID " +
                    "ORDER BY DateSubmitted"))

            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                sqlCom.Connection = sqlCon;
                sqlCon.Open();
                sda.SelectCommand = sqlCom;
                SqlDataReader sdr = sqlCom.ExecuteReader();
                while (sdr.Read())
                {
                    string DateS = null;
                    string DateI = null;
                    string DateR = null;
                    string Buyer = null;
                    if (sdr[0].ToString() != null)
                    {


                        if (sdr[0].ToString() == null)
                        {
                            DateS = "-";
                        }
                        else
                        {


                            DateI = sdr[0].ToString();
                        }

                        if (sdr[1].ToString() == null)
                        {
                            DateS = "-";
                        }
                        else
                        {


                            DateS = sdr[1].ToString(); ;
                        }

                        if (sdr[2].ToString() == null)
                        {
                            DateR = "-";
                        }
                        else
                        {


                            DateR = sdr[2].ToString();
                        }

                        if (sdr[6].ToString() == null)
                        {
                            Buyer = "-";
                        }
                        else
                        {
                            Buyer = sdr[6].ToString();
                        }

                        decimal value1 = Convert.ToDecimal(sdr[7].ToString());
                        var value2 = String.Format("{0:0,0.00}", value1);

                        dtable.Rows.Add
                            (
                            DateI,
                            DateS,
                            DateR,
                            sdr[3].ToString(),
                            sdr[4].ToString(),
                            sdr[5].ToString(),
                            Buyer,
                            value2,
                            sdr[8].ToString(),
                            sdr[9].ToString(),
                            FromD,
                            ToD,
                            RDate,
                            "1"
                            );

                        count += 1;

                    }
                }
            }

            if (count == 0)
            {
                var DealerID = db.DealerBranch.Where(o => o.DealerBranchID == DealerBranchID).Select(o => o.DealerID).FirstOrDefault();
                var DealerName = db.Dealer.Where(o => o.DealerID == DealerID).Select(o => o.DealerName).FirstOrDefault();
                string DealerBranchName = db.DealerBranch.Where(o => o.DealerBranchID == DealerBranchID).Select(o => o.DealerBranchName).FirstOrDefault();

                //string DID = db.DealerBranch.Where(o => o.DealerBranchID == DealerBranchID).Select(o => o.DealerID).FirstOrDefault();
                dtable.Rows.Add
                            (
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "0.00",
                            DealerName,
                            DealerBranchName,
                            FromD,
                            ToD,
                            RDate,
                            "0"
                            );
            }
            string Tdate = DateTime.Now.ToShortDateString();
            string fcount = count.ToString();

            ReportDataSource rds = new ReportDataSource("DataSet1", dtable);
            lr.DataSources.Clear();
            lr.DataSources.Add(rds);

            lr.Refresh();

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo = "";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            var timecreated = DateTime.Now.ToString("MMddyyyyhhmmss");
            ViewBag.Path = "VRR - " + timecreated + ".pdf";
            var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + ViewBag.Path;


            //var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "Report.pdf";
            //System.IO.File.Delete(pdfPath);
            //ViewBag.Path = "Report.pdf";
            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }
            //ViewBag.pfpath = pdfPath;
            return pdfPath;
        }

        public FileResult ExportToExcel(string DateFrom, string DateTo, string DealerBranchID)
        {

            int BranchID = 0;
            if (DateFrom == null)
            {
                DateFrom = DateTime.Today.Month.ToString() + "/" + DateTime.Today.Day.ToString() + "/" + DateTime.Today.Year.ToString() + " 12:00:00 am";
            }

            if (DateTo == null)
            {
                DateTo = DateTime.Today.Month.ToString() + "/" + DateTime.Today.Day.ToString() + "/" + DateTime.Today.Year.ToString() + " 11:59:59 pm";
            }

            if (DealerBranchID != null)
            {
                BranchID = int.Parse(DealerBranchID);
            }

            DataTable dtable = new DataTable("DataSet1");

            dtable.Columns.Add(new DataColumn { ColumnName = "InvoiceDate", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "DateSubmitted", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "ReceivedDate", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "VehicleMakeName", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "VehicleModelName", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "VehicleColorName", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "Buyer", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "VehicleCost", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "Dealer", DataType = typeof(string), AllowDBNull = true });
            dtable.Columns.Add(new DataColumn { ColumnName = "Branch", DataType = typeof(string), AllowDBNull = true });
            //dtable.Columns.Add(new DataColumn { ColumnName = "DateFrom", DataType = typeof(string), AllowDBNull = true });
            //dtable.Columns.Add(new DataColumn { ColumnName = "DateTo", DataType = typeof(string), AllowDBNull = true });
            //dtable.Columns.Add(new DataColumn { ColumnName = "DateRun", DataType = typeof(string), AllowDBNull = true });
            //dtable.Columns.Add(new DataColumn { ColumnName = "Count", DataType = typeof(string), AllowDBNull = true });

            var conString = System.Configuration.ConfigurationManager.ConnectionStrings["VRSystemConnectionString"];
            string strConnString = conString.ConnectionString;

            int count = 0;
            //string FDate = fDate.Month.ToString() + "/" + fDate.Day.ToString() + "/" + fDate.Year.ToString() + " 12:00:00 am";
            //string TDate = tDate.Month.ToString() + "/" + tDate.Day.ToString() + "/" + tDate.Year.ToString() + " 11:59:59 pm";
            // string FromD = fDate.Month.ToString() + "/" + fDate.Day.ToString() + "/" + fDate.Year.ToString();
            //string ToD = tDate.Month.ToString() + "/" + tDate.Day.ToString() + "/" + tDate.Year.ToString();

            string RDate = DateTime.Today.Month.ToString() + "/" + DateTime.Today.Day.ToString() + "/" + DateTime.Today.Year.ToString();

            using (SqlConnection sqlCon = new SqlConnection(strConnString))
            using (SqlCommand sqlCom = new SqlCommand("SELECT i.InvoiceDate, bm.DateSubmitted, bm.ReceivedDate, vm.VehicleMakeName, vmo.VehicleModelName, vc.VehicleColorName, c.FirstName +' '+ c.Lastname as Buyer, i.VehicleCost, d.DealerName, db.DealerBranchName " +
                    "From DealerInvoice i, Dealer d, DealerBranch db, BatchMaster bm, VehicleInfo vi, VehicleMake vm, VehicleModel vmo, VehicleColor vc, Customer c " +
                    "where db.DealerBranchID = '" + DealerBranchID + "' " +
                    "AND DateSubmitted BETWEEN '" + DateFrom + "' AND '" + DateTo + "' " +
                    "AND i.DealerBranchID = db.DealerBranchID " +
                    "AND i.CustomerID = c.CustomerID " +
                    "AND i.VehicleID = vi.VehicleID " +
                    "AND vi.VehicleColorID = vc.VehicleColorID " +
                    "AND vi.VehicleMakeID = vm.VehicleMakeID " +
                    "AND vi.VehicleModelID = vmo.VehicleModelID " +
                    "AND d.DealerID = db.DealerID " +
                    "AND bm.UserSubRef = db.DealerBranchID " +
                    "ORDER BY DateSubmitted"))

            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                sqlCom.Connection = sqlCon;
                sqlCon.Open();
                sda.SelectCommand = sqlCom;
                SqlDataReader sdr = sqlCom.ExecuteReader();
                while (sdr.Read())
                {
                    string DateS = null;
                    string DateI = null;
                    string DateR = null;
                    string Buyer = null;
                    if (sdr[0].ToString() != null)
                    {


                        if (sdr[0].ToString() == null)
                        {
                            DateS = "-";
                        }
                        else
                        {


                            DateI = sdr[0].ToString();
                        }

                        if (sdr[1].ToString() == null)
                        {
                            DateS = "-";
                        }
                        else
                        {


                            DateS = sdr[1].ToString(); ;
                        }

                        if (sdr[2].ToString() == null)
                        {
                            DateR = "-";
                        }
                        else
                        {


                            DateR = sdr[2].ToString();
                        }

                        if (sdr[6].ToString() == null)
                        {
                            Buyer = "-";
                        }
                        else
                        {
                            Buyer = sdr[6].ToString();
                        }

                        decimal value1 = Convert.ToDecimal(sdr[7].ToString());
                        var value2 = String.Format("{0:0,0.00}", value1);

                        dtable.Rows.Add
                            (
                            DateI,
                            DateS,
                            DateR,
                            sdr[3].ToString(),
                            sdr[4].ToString(),
                            sdr[5].ToString(),
                            Buyer,
                            value2,
                            sdr[8].ToString(),
                            sdr[9].ToString()
                            );

                        count += 1;

                    }
                }
            }


            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dtable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelFile.xlsx");
                }
            }
        }
    }
}