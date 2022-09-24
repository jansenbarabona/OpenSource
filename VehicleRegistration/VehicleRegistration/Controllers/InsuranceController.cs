using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;

namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    public class InsuranceController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Insurance
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var Insurancelist = (from a in db.Insurance
                                     join b in db.City on a.CityID equals b.CityID into temp
                                     from temptbl in temp.DefaultIfEmpty()
                                     join c in db.Province on temptbl.ProvinceID equals c.ProvinceID into temp2
                                     from temptbl2 in temp2.DefaultIfEmpty()
                                     select new InsuranceModel()
                                     {
                                         InsuranceID = a.InsuranceID,
                                         InsuranceName = a.InsuranceName,
                                         EmailAddress = a.EmailAddress,
                                         Website = a.Website,
                                         BusinessPhone = a.BusinessPhone,
                                         MobilePhone = a.MobilePhone,
                                         FaxNumber = a.FaxNumber,
                                         Address = a.Address,
                                         ProvinceID = a.ProvinceID,
                                         ProvinceName = temptbl2.ProvinceName,
                                         CityID = a.CityID,
                                         CityName = temptbl.CityName,
                                         ZipCode = a.ZipCode,
                                         Logo = a.Logo,
                                         LogoByte = a.LogoByte,
                                         Notes = a.Notes,
                                         CreatedBy = a.CreatedBy,
                                         CreatedDate = a.CreatedDate,
                                         Active = a.Active
                                     }).Where(o => o.Active == true).ToList();

                return View(Insurancelist);
            }
        }
        #region InsuranceInfo

        public ActionResult InsuranceInfo(int id)
        {
            ViewBag.id = id;

            InsuranceModel InsuranceInfoView = new InsuranceModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = true;
                ViewBag.Edit = true;
                InsuranceInfoView.CityList = db.City.Where(o => o.Active == true).ToList();
                InsuranceInfoView.ProvinceList = db.Province.Where(o => o.Active == true).ToList();

                InsuranceInfoView.WalletDetail = Functions.GetWalletDetails((int)UserEntityEnum.MAI, (int)id);
                InsuranceInfoView.EntityTransaction = db.vwTransactionEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer && o.EntityID == (int)id && o.Active == true).ToList();

                var Load = db.Insurance.Where(o => o.Active == true && o.InsuranceID == id).ToList().FirstOrDefault();

                InsuranceInfoView.InsuranceID = Load.InsuranceID;
                InsuranceInfoView.InsuranceName = Load.InsuranceName;
                InsuranceInfoView.EmailAddress = Load.EmailAddress;
                InsuranceInfoView.BusinessPhone = Load.BusinessPhone;
                InsuranceInfoView.MobilePhone = Load.MobilePhone;
                InsuranceInfoView.FaxNumber = Load.FaxNumber;
                InsuranceInfoView.Website = Load.Website;
                InsuranceInfoView.Address = Load.Address;
                InsuranceInfoView.ProvinceID = Load.ProvinceID;
                InsuranceInfoView.CityID = Load.CityID;
                InsuranceInfoView.ZipCode = Load.ZipCode;
                InsuranceInfoView.Logo = Load.Logo;
                InsuranceInfoView.LogoByte = Load.LogoByte;
                InsuranceInfoView.Notes = Load.Notes;
                InsuranceInfoView.CreatedBy = Load.CreatedBy;
                InsuranceInfoView.CreatedDate = Load.CreatedDate;
                InsuranceInfoView.Active = Load.Active;


            }
            return PartialView(InsuranceInfoView);
        }

        #endregion
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult Insurance_Register(int? id)
        {
            ViewBag.id = id;

            InsuranceModel NewInsurance = new InsuranceModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                NewInsurance.CityList = db.City.Where(o => o.Active == true).ToList();
                NewInsurance.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                NewInsurance.BarangayList = new List<Barangay>();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    NewInsurance.WalletDetail = Functions.GetWalletDetails((int)UserEntityEnum.Insurance, (int)id);
                    NewInsurance.EntityTransaction = db.vwTransactionEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer && o.EntityID == (int)id && o.Active == true).ToList();

                    var Load = db.Insurance.Where(o => o.Active == true && o.InsuranceID == id).ToList().FirstOrDefault();

                    NewInsurance.CityList = db.City.Where(o => o.ProvinceID == Load.ProvinceID && o.Active == true).ToList();
                    NewInsurance.BarangayList = db.Barangay.Where(o => o.BarangayID == Load.BarangayID && o.Active == true).ToList();

                    NewInsurance.InsuranceID = Load.InsuranceID;
                    NewInsurance.InsuranceName = Load.InsuranceName;
                    NewInsurance.EmailAddress = Load.EmailAddress;
                    NewInsurance.EmailAddress2 = Load.EmailAddress2;
                    NewInsurance.BusinessPhone = Load.BusinessPhone;
                    NewInsurance.MobilePhone = Load.MobilePhone;
                    NewInsurance.FaxNumber = Load.FaxNumber;
                    NewInsurance.TIN = Load.TIN;
                    NewInsurance.Website = Load.Website;
                    NewInsurance.Address = Load.Address;
                    NewInsurance.ProvinceID = Load.ProvinceID;
                    NewInsurance.CityID = Load.CityID;
                    NewInsurance.BarangayID = Load.BarangayID;
                    NewInsurance.ZipCode = Load.ZipCode;
                    NewInsurance.Logo = Load.Logo;
                    NewInsurance.LogoByte = Load.LogoByte;
                    NewInsurance.Notes = Load.Notes;
                    NewInsurance.CreatedBy = Load.CreatedBy;
                    NewInsurance.CreatedDate = Load.CreatedDate;
                    NewInsurance.Active = Load.Active;

                }

                return PartialView(NewInsurance);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult Insurance_Register(InsuranceModel insurance, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewInsurance = new Insurance
                            {
                                Address = insurance.Address.Trim(),
                                InsuranceName = insurance.InsuranceName.Trim(),
                                MobilePhone = insurance.MobilePhone.Trim(),
                                Website = insurance.Website.Trim(),
                                ZipCode = insurance.ZipCode.Trim(),
                                Notes = insurance.Notes?.Trim(),
                                Logo = insurance.LogoFile != null ? insurance.LogoFile.FileName : null,
                                LogoByte = insurance.LogoFile != null ? insurance.LogoFile.ToByte() : null,
                                FaxNumber = insurance.FaxNumber.Trim(),
                                EmailAddress = insurance.EmailAddress.Trim(),
                                EmailAddress2 = insurance.EmailAddress2.Trim(),
                                TIN = insurance.TIN.Trim(),
                                BusinessPhone = insurance.BusinessPhone.Trim(),
                                ProvinceID = insurance.ProvinceID,
                                CityID = insurance.CityID,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };

                            db.Insurance.Add(NewInsurance);
                            db.SaveChanges();

                            Functions.Logo(submit, "", insurance.LogoFile);
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.Insurance.Where(o => o.InsuranceID == insurance.InsuranceID).FirstOrDefault();
                            Update.Address = insurance.Address.Trim();
                            Update.InsuranceName = insurance.InsuranceName.Trim();
                            Update.MobilePhone = insurance.MobilePhone.Trim();
                            Update.Website = insurance.Website.Trim();
                            Update.ZipCode = insurance.ZipCode.Trim();
                            Update.Notes = insurance.Notes?.Trim();
                            if (insurance.LogoFile != null)
                            {
                                Functions.Logo(submit, Update.Logo, insurance.LogoFile);

                                Update.Logo = insurance.LogoFile != null ? insurance.LogoFile.FileName : null;
                                Update.LogoByte = insurance.LogoFile.ToByte();
                            }
                            Update.FaxNumber = insurance.FaxNumber.Trim();
                            Update.EmailAddress = insurance.EmailAddress.Trim();
                            Update.BusinessPhone = insurance.BusinessPhone.Trim();
                            Update.ProvinceID = insurance.ProvinceID;
                            Update.CityID = insurance.CityID;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var remove = db.Insurance.Where(o => o.InsuranceID == insurance.InsuranceID).FirstOrDefault();
                            remove.Active = false;
                            remove.UpdatedBy = CurrentUser.Details.UserID;
                            remove.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            Functions.Logo(submit, remove.Logo, null);
                            TempData["WarningMessage"] = "Removed Successfully!";
                        }
                        break;
                }
                return RedirectToAction("Index");
            }
            else
            {
                insurance.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                insurance.CityList = db.City.Where(o => o.Active == true).ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(insurance);
            }

        }

        //Insurance Branch
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Insurance, UserEntityEnum.DataBridgeAsia })]
        public ActionResult InsuranceBranchList()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var InsuranceBranchlist = new List<InsuranceBranchModel>();
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                {

                    InsuranceBranchlist = (from a in db.InsuranceBranch
                                           join b in db.Insurance on a.InsuranceID equals b.InsuranceID
                                           join c in db.City on b.CityID equals c.CityID
                                           join d in db.Province on c.ProvinceID equals d.ProvinceID into temp
                                           from temptbl in temp.DefaultIfEmpty()
                                           select new InsuranceBranchModel()
                                           {
                                               InsuranceBranchID = a.InsuranceBranchID,
                                               InsuranceBranchName = a.InsuranceBranchName,
                                               InsuranceID = a.InsuranceID,
                                               InsuranceName = b.InsuranceName,
                                               EmailAddress = a.EmailAddress,
                                               WebSite = a.WebSite,
                                               BusinessPhone = a.BusinessPhone,
                                               MobilePhone = a.MobilePhone,
                                               FaxNumber = a.FaxNumber,
                                               Address = a.Address,
                                               ProvinceID = a.ProvinceID,
                                               ProvinceName = temptbl.ProvinceName,
                                               CityID = a.CityID,
                                               CityName = c.CityName,
                                               ZipCode = a.ZipCode,
                                               CreatedBy = a.CreatedBy,
                                               CreatedDate = a.CreatedDate,
                                               Active = a.Active
                                           }).Where(o => o.Active == true).ToList();
                }
                else
                {

                    InsuranceBranchlist = (from a in db.InsuranceBranch
                                           join b in db.Insurance on a.InsuranceID equals b.InsuranceID
                                           join c in db.City on b.CityID equals c.CityID
                                           join d in db.Province on c.ProvinceID equals d.ProvinceID into temp
                                           where 
                                           a.Active == true &&
                                           a.InsuranceBranchID == CurrentUser.Details.SubReferenceID &&
                                           b.Active == true &&
                                           b.InsuranceID == CurrentUser.Details.ReferenceID
                                           from temptbl in temp.DefaultIfEmpty()
                                           select new InsuranceBranchModel()
                                           {
                                               InsuranceBranchID = a.InsuranceBranchID,
                                               InsuranceBranchName = a.InsuranceBranchName,
                                               InsuranceID = a.InsuranceID,
                                               InsuranceName = b.InsuranceName,
                                               EmailAddress = a.EmailAddress,
                                               WebSite = a.WebSite,
                                               BusinessPhone = a.BusinessPhone,
                                               MobilePhone = a.MobilePhone,
                                               FaxNumber = a.FaxNumber,
                                               Address = a.Address,
                                               ProvinceID = a.ProvinceID,
                                               ProvinceName = temptbl.ProvinceName,
                                               CityID = a.CityID,
                                               CityName = c.CityName,
                                               ZipCode = a.ZipCode,
                                               CreatedBy = a.CreatedBy,
                                               CreatedDate = a.CreatedDate,
                                               Active = a.Active
                                           }).Where(o => o.Active == true).ToList();
                }

                return View(InsuranceBranchlist);
            }
        }
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Insurance, UserEntityEnum.DataBridgeAsia })]
        public ActionResult InsuranceBranch(int? id)
        {
            ViewBag.id = id;

            InsuranceBranchModel Branch = new InsuranceBranchModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                Branch.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                Branch.CityList = db.City.Where(o => o.Active == true).ToList();
                Branch.InsuranceList = db.Insurance.Where(o => o.Active == true).ToList();
                Branch.BarangayList = new List<Barangay>();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var Load = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceBranchID == id).ToList().FirstOrDefault();

                    Branch.CityList = db.City.Where(o => o.ProvinceID == Load.ProvinceID).ToList();
                    Branch.BarangayList = db.Barangay.Where(o => o.BarangayID == Load.BarangayID && o.Active == true).ToList();

                    Branch.InsuranceID = Load.InsuranceID;
                    Branch.InsuranceBranchID = Load.InsuranceBranchID;
                    Branch.InsuranceBranchName = Load.InsuranceBranchName;
                    Branch.ProvinceID = Load.ProvinceID;
                    Branch.CityID = Load.CityID;
                    Branch.BarangayID = Load.BarangayID;
                    Branch.EmailAddress = Load.EmailAddress;
                    Branch.BusinessPhone = Load.BusinessPhone;
                    Branch.MobilePhone = Load.MobilePhone;
                    Branch.FaxNumber = Load.FaxNumber;
                    Branch.WebSite = Load.WebSite;
                    Branch.Address = Load.Address;
                    Branch.CityID = Load.CityID;
                    Branch.ZipCode = Load.ZipCode;
                    Branch.CreatedBy = Load.CreatedBy;
                    Branch.CreatedDate = Load.CreatedDate;
                    Branch.IsMain = Load.IsMain;
                    Branch.Active = Load.Active;
                }
                return PartialView(Branch);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Insurance, UserEntityEnum.DataBridgeAsia })]
        public ActionResult InsuranceBranch(InsuranceBranchModel insurance_branch, HttpPostedFileBase logo, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var Branch = new InsuranceBranch
                            {
                                InsuranceID = insurance_branch.InsuranceID,
                                InsuranceBranchName = insurance_branch.InsuranceBranchName.Trim(),
                                EmailAddress = insurance_branch.EmailAddress.Trim(),
                                BusinessPhone = insurance_branch.BusinessPhone.Trim(),
                                MobilePhone = insurance_branch.MobilePhone.Trim(),
                                FaxNumber = insurance_branch.FaxNumber.Trim(),
                                WebSite = insurance_branch.WebSite.Trim(),
                                Address = insurance_branch.Address.Trim(),
                                CityID = insurance_branch.CityID,
                                BarangayID = insurance_branch.BarangayID,
                                ProvinceID = insurance_branch.ProvinceID,
                                ZipCode = insurance_branch.ZipCode.Trim(),
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                IsMain = insurance_branch.IsMain,
                                Active = true
                            };
                            db.InsuranceBranch.Add(Branch);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.InsuranceBranch.Where(o => o.InsuranceBranchID == insurance_branch.InsuranceBranchID).FirstOrDefault();
                            Update.InsuranceID = insurance_branch.InsuranceID;
                            Update.InsuranceBranchName = insurance_branch.InsuranceBranchName.Trim();
                            Update.EmailAddress = insurance_branch.EmailAddress.Trim();
                            Update.BusinessPhone = insurance_branch.BusinessPhone.Trim();
                            Update.MobilePhone = insurance_branch.MobilePhone.Trim();
                            Update.FaxNumber = insurance_branch.FaxNumber.Trim();
                            Update.WebSite = insurance_branch.WebSite.Trim();
                            Update.Address = insurance_branch.Address.Trim();
                            Update.CityID = insurance_branch.CityID;
                            Update.BarangayID = insurance_branch.BarangayID;
                            Update.ProvinceID = insurance_branch.ProvinceID;
                            Update.ZipCode = insurance_branch.ZipCode.Trim();
                            Update.IsMain = insurance_branch.IsMain;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update_active = db.InsuranceBranch.Where(o => o.InsuranceBranchID == insurance_branch.InsuranceBranchID).FirstOrDefault();
                            Update_active.Active = false;
                            Update_active.UpdatedBy = CurrentUser.Details.UserID;
                            Update_active.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Sucessfully!";
                        }
                        break;
                }
                return RedirectToAction("InsuranceBranchList");
            }
            else
            {
                insurance_branch.InsuranceList = db.Insurance.Where(o => o.Active == true).ToList();
                insurance_branch.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                insurance_branch.CityList = db.City.Where(o => o.Active == true).ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(insurance_branch);
            }

        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Insurance })]
        public ActionResult InsuranceDealerList(int? insurance_id)
        {
            List<spInsuranceDealer_Result> InsuranceDealerList = new List<spInsuranceDealer_Result>();

            using (db = new VRSystemEntities())
            {
                InsuranceDealerList = db.spInsuranceDealer(insurance_id).ToList();
            }
            return View(InsuranceDealerList);

            //using (db = new VRSystemEntities())
            //{
            //    db.Configuration.LazyLoadingEnabled = false;
            //    List<Insurance> insurancelist = new List<Insurance>();

            //    insurancelist = db.Insurance.Where(o => o.Active == true).ToList();

            //    return View(insurancelist);
            //}
        }
        //public ActionResult InsuranceDealer(int? insurance_id)
        //{

        //    List<spInsuranceDealer_Result> InsuranceDealerList = new List<spInsuranceDealer_Result>();

        //    using (db = new VRSystemEntities())
        //    {
        //        InsuranceDealerList = db.spInsuranceDealer(insurance_id).ToList();
        //    }
        //    return View(InsuranceDealerList);
        //}

        [HttpGet]
        public ActionResult InsuranceCOCSeriesList()
        {
            using (db = new VRSystemEntities())
            {
                var COCQuery = from coc in db.InsuranceCOCSeries
                               join insurance in db.Insurance on coc.InsuranceID equals insurance.InsuranceID
                               where coc.Active == true
                               select new { COC = coc, Insurance = insurance};

                var COCList = COCQuery.Select(
                    o => new InsuranceCOCSeriesModel()
                    {
                        InsuranceCOCSeriesID = o.COC.InsuranceCOCSeriesID,
                        InsuranceID = o.COC.InsuranceID,
                        InsuranceName = o.Insurance.InsuranceName,
                        EffectiveDate = o.COC.EffectiveDate,
                        SeriesFrom = o.COC.SeriesFrom,
                        SeriesTo = o.COC.SeriesTo,
                        CurrentSeries = o.COC.CurrentSeries,
                        CreatedBy = o.COC.CreatedBy,
                        CreatedDate = o.COC.CreatedDate,
                        Active = o.COC.Active
                    }).ToList(); ;

                return View(COCList);
            }
        }

        [HttpGet]
        public ActionResult InsuranceCOCSeries(int? id)
        {
            var InsuranceCOCSeries = new InsuranceCOCSeriesModel();

            using (db = new VRSystemEntities())
            {
                InsuranceCOCSeries.InsuranceList = db.Insurance.Where(o => o.Active == true).ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var Load = db.InsuranceCOCSeries.Where(o => o.InsuranceCOCSeriesID == id).FirstOrDefault();

                    InsuranceCOCSeries.InsuranceCOCSeriesID = Load.InsuranceCOCSeriesID;
                    InsuranceCOCSeries.InsuranceID = Load.InsuranceID;
                    InsuranceCOCSeries.EffectiveDate = Load.EffectiveDate;
                    InsuranceCOCSeries.SeriesFrom = Load.SeriesFrom;
                    InsuranceCOCSeries.SeriesTo = Load.SeriesTo;
                    InsuranceCOCSeries.Active = Load.Active;
                }
            }
            return View(InsuranceCOCSeries);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsuranceCOCSeries(InsuranceCOCSeriesModel COCSeriesModel, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var Insert = new InsuranceCOCSeries
                            {
                                InsuranceID = COCSeriesModel.InsuranceID,
                                EffectiveDate = COCSeriesModel.EffectiveDate,
                                SeriesFrom = COCSeriesModel.SeriesFrom,
                                SeriesTo = COCSeriesModel.SeriesTo,
                                CurrentSeries = 0,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };

                            db.InsuranceCOCSeries.Add(Insert);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.InsuranceCOCSeries.Where(o => o.InsuranceCOCSeriesID == COCSeriesModel.InsuranceCOCSeriesID).FirstOrDefault();
                            Update.InsuranceID = COCSeriesModel.InsuranceID;
                            Update.EffectiveDate = COCSeriesModel.EffectiveDate;
                            Update.SeriesFrom = COCSeriesModel.SeriesFrom;
                            Update.SeriesTo = COCSeriesModel.SeriesTo;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var remove = db.InsuranceCOCSeries.Where(o => o.InsuranceCOCSeriesID == COCSeriesModel.InsuranceCOCSeriesID).FirstOrDefault();
                            remove.Active = false;
                            remove.UpdatedBy = CurrentUser.Details.UserID;
                            remove.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Successfully!";
                        }
                        break;
                }
                return RedirectToAction("InsuranceCOCSeriesList");
            }
            else
            {
                COCSeriesModel.InsuranceList = db.Insurance.Where(o => o.Active == true).ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(COCSeriesModel);
            }
        }


        [HttpGet]
        public ActionResult CTPL()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CTPL(CTPLReportModel model)
        {
            var AutoGen = db.Insurance.Where(o => o.InsuranceID == CurrentUser.Details.ReferenceID).FirstOrDefault().AutoGenerateCOC;
            if (ModelState.IsValid && AutoGen)
            {
                string path = Server.MapPath("~/Reports/Excel/CTPL UPLOAD DATA.xlsx");

                using (XLWorkbook wb = new XLWorkbook(path))
                {
                    var ws = wb.Worksheet(1);

                    var cnt_row = 2;
                    var queryDateTo = model.DateTo.AddDays(1);

                    var InvoiceList = db.DealerInvoice.Where(o => o.InsuranceID == CurrentUser.Details.ReferenceID && (model.DateFrom <= DbFunctions.TruncateTime(o.COCInceptionDate) && DbFunctions.TruncateTime(o.COCInceptionDate) <= model.DateTo)).ToList();
                    foreach (var item in InvoiceList)
                    {
                        var Vehicleinfo = db.vwVehicleList.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                        var CustomerInfo = db.Customer.Where(o => o.CustomerID == item.CustomerID).FirstOrDefault();
                        var CTPLInfo = db.CTPL.Where(o => o.VehicleClassificationID == 7 && o.Active == true).FirstOrDefault();
                        var Dealer = db.Dealer.Where(o => o.DealerID == Vehicleinfo.DealerID).FirstOrDefault().DealerName;
                        var Branch = db.DealerBranch.Where(o => o.DealerID == Vehicleinfo.DealerID && o.DealerBranchID == Vehicleinfo.DealerBranchID).FirstOrDefault().DealerBranchName;
                        //Dealer
                        ws.Cell(cnt_row, 1).Value = Dealer;
                        //Dealer Branch
                        ws.Cell(cnt_row, 2).Value = Branch;
                        //Issuance Date   
                        ws.Cell(cnt_row, 3).SetValue<string>(item.COCInceptionDate?.ToString("yyyy.MM.dd"));
                        //COCNO
                        ws.Cell(cnt_row, 4).Value = item.COC;
                        //PolicyNo    
                        ws.Cell(cnt_row, 5).Value = item.COCPolicyNumber;
                        //Inception Date  
                        ws.Cell(cnt_row, 6).SetValue<string>(item.COCInceptionDate?.ToString("yyyy.MM.dd"));
                        //Expiry Date 
                        ws.Cell(cnt_row, 7).SetValue<string>(item.COCExpirationDate?.ToString("yyyy.MM.dd"));
                        //FirstName 
                        ws.Cell(cnt_row, 8).Value = CustomerInfo.FirstName;
                        //MidName 
                        ws.Cell(cnt_row, 9).Value = CustomerInfo.MiddleName;
                        //LastName 
                        ws.Cell(cnt_row, 10).Value = CustomerInfo.LastName;
                        //CompanyName 
                        ws.Cell(cnt_row, 11).Value = CustomerInfo.CorpName;
                        //Vehicle Year    
                        ws.Cell(cnt_row, 12).Value = Vehicleinfo.Year;
                        //Make 
                        ws.Cell(cnt_row, 13).Value = Vehicleinfo.VehicleMakeName;
                        //Model Variant
                        ws.Cell(cnt_row, 14).Value = Vehicleinfo.VehicleModelName + " - "+ Vehicleinfo.Variant;
                        //Plate Number 
                        ws.Cell(cnt_row, 15).Value = "";
                        //Type Of Body    
                        ws.Cell(cnt_row, 16).Value = Vehicleinfo.VehicleBodyTypeName;
                        //Color 
                        ws.Cell(cnt_row, 17).Value = Vehicleinfo.VehicleColorName;
                        //M.V.File Number    
                        ws.Cell(cnt_row, 18).Value = "";
                        //Serial / Chasis Number 
                        ws.Cell(cnt_row, 19).Value = Vehicleinfo.ChassisNumber;
                        //Motor Number 
                        ws.Cell(cnt_row, 20).Value = Vehicleinfo.EngineNumber;
                        //Authorized Capacity 
                        ws.Cell(cnt_row, 21).Value = "";
                        //Unladen Weight 
                        ws.Cell(cnt_row, 22).Value = Vehicleinfo.GrossVehicleWeight;
                        //Agent Code 
                        ws.Cell(cnt_row, 23).Value = "";
                        //Sum Insured 
                        ws.Cell(cnt_row, 24).Value = "";
                        //Net Premium 
                        ws.Cell(cnt_row, 25).Value = CTPLInfo.BasicPremium;
                        //DST 
                        ws.Cell(cnt_row, 26).Value = CTPLInfo.DST;
                        //VAT 
                        ws.Cell(cnt_row, 27).Value = CTPLInfo.VAT;
                        //LGT 
                        ws.Cell(cnt_row, 28).Value = CTPLInfo.LGT;
                        //Authentication Charges 
                        ws.Cell(cnt_row, 29).Value = CTPLInfo.AuthenticationFee;
                        //Gross Premium (W + X + Y + Z + AA)
                        ws.Cell(cnt_row, 30).Value = CTPLInfo.GrossPremium;

                        cnt_row++;
                    }

                    //wb.Save();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        TempData["SuccessMessage"] = "Downloaded is successful!";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CTPL UPLOAD DATA.xlsx");
                    }
                }
            }
            else if (!AutoGen)
            {
                TempData["ErrorMessage"] = "This insurance is not autogenrate.";
                return View(model);
            }
            else
            {
                TempData["ErrorMessage"] = "An error has occured.";
                return View(model);
            }
        }
    }
}