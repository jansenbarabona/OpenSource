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
    public class CTPLController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: CTPL
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult Index()
        {
            CTPLModel ctplList = new CTPLModel();
            try
            {

                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)


                    ctplList.vwCTPLModelList = (from a in db.CTPL
                                                join b in db.VehicleClassification
                                                on a.VehicleClassificationID equals b.VehicleClassificationID into temp
                                                from temptbl in temp.DefaultIfEmpty()
                                                select new
                                                {
                                                    a,
                                                    temptbl.VehicleClassificationName,
                                                }).Where(o => o.a.Active == true).Select(
                        o => new vwCTPLModel()
                        {
                            CTPLID = o.a.CTPLID,
                            CTPLTermID = o.a.CTPLTermID,
                            VehicleClassificationID = o.a.VehicleClassificationID,
                            VehicleClassificationName = o.VehicleClassificationName,
                            BasicPremium = o.a.BasicPremium,
                            Taxes = o.a.Taxes,
                            AuthenticationFee = o.a.AuthenticationFee,
                            GrossPremium = o.a.GrossPremium,
                        }).ToList();

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error has occured.";
            }
            return View(ctplList);
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult CTPL(int? id)
        {
            CTPLModel ctpl = new CTPLModel();

            using (db = new VRSystemEntities())
            {
                ctpl.CTPLTermList = db.CTPLTerm.Where(o => o.Active == true).ToList();
                ctpl.VehicleClassificationList = db.VehicleClassification.Where(o => o.Active == true).ToList();
                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var ctplLoad = db.CTPL.Where(o => o.CTPLID == id && o.Active == true).FirstOrDefault();

                    ctpl.CTPLID = ctplLoad.CTPLID;
                    ctpl.CTPLTermID = ctplLoad.CTPLTermID;
                    ctpl.VehicleClassificationID = ctplLoad.VehicleClassificationID;
                    ctpl.BasicPremium = ctplLoad.BasicPremium;
                    ctpl.VAT = ctplLoad.VAT;
                    ctpl.DST = ctplLoad.DST;
                    ctpl.LGT = ctplLoad.LGT;
                    ctpl.Taxes = ctplLoad.Taxes;
                    ctpl.AuthenticationFee = ctplLoad.AuthenticationFee;
                    ctpl.GrossPremium = ctplLoad.GrossPremium;
                }

                return View(ctpl);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult CTPL(CTPLModel ctpl, string submit)
        {

            ctpl.CTPLTermList = db.CTPLTerm.Where(o => o.Active == true).ToList();
            ctpl.VehicleClassificationList = db.VehicleClassification.Where(o => o.Active == true).ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    switch (submit)
                    {
                        case "Create":
                            using (db = new VRSystemEntities())
                            {

                                var newctpl = new CTPL
                                {
                                    CTPLTermID = Convert.ToInt32(ctpl.CTPLTermID),
                                    VehicleClassificationID = Convert.ToInt32(ctpl.VehicleClassificationID),
                                    BasicPremium = Convert.ToDecimal(ctpl.BasicPremium),
                                    Taxes = Convert.ToDecimal(ctpl.Taxes),
                                    VAT = Convert.ToDecimal(ctpl.VAT),
                                    DST = Convert.ToDecimal(ctpl.DST),
                                    LGT = Convert.ToDecimal(ctpl.LGT),
                                    AuthenticationFee = Convert.ToDecimal(ctpl.AuthenticationFee),
                                    GrossPremium = Convert.ToDecimal(ctpl.GrossPremium),
                                    Active = true,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now
                                };
                                db.CTPL.Add(newctpl);
                                db.SaveChanges();

                            }
                            TempData["SuccessMessage"] = "Created Successfully!";
                            break;

                        case "Save":
                            using (db = new VRSystemEntities())
                            {
                                var Update = db.CTPL.Where(o => o.CTPLID == ctpl.CTPLID).FirstOrDefault();
                                Update.CTPLTermID = Convert.ToInt32(ctpl.CTPLTermID);
                                Update.VehicleClassificationID = Convert.ToInt32(ctpl.VehicleClassificationID);
                                Update.BasicPremium = Convert.ToDecimal(ctpl.BasicPremium);
                                Update.Taxes = Convert.ToDecimal(ctpl.Taxes);
                                Update.VAT = Convert.ToDecimal(ctpl.VAT);
                                Update.DST = Convert.ToDecimal(ctpl.DST);
                                Update.LGT = Convert.ToDecimal(ctpl.LGT);
                                Update.AuthenticationFee = Convert.ToDecimal(ctpl.AuthenticationFee);
                                Update.GrossPremium = Convert.ToDecimal(ctpl.GrossPremium);
                                Update.UpdatedBy = CurrentUser.Details.UserID;
                                Update.UpdatedDate = DateTime.Now;
                                db.SaveChanges();
                            }
                            TempData["SuccessMessage"] = "Updated Successfully!";
                            break;
                        case "Delete":
                            using (db = new VRSystemEntities())
                            {
                                var Delete = db.CTPL.Where(o => o.CTPLID == ctpl.CTPLID).FirstOrDefault();
                                Delete.Active = false;
                                Delete.UpdatedBy = CurrentUser.Details.UserID;
                                Delete.UpdatedDate = DateTime.Now;
                                db.SaveChanges();
                            }
                            TempData["WarningMessage"] = "Removed Successfully!";
                            break;
                    }
                    return RedirectToAction("index");
                }
                else
                {
                    if (submit == "Create")
                        ViewBag.Edit = false;

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex;
            }
            return View(ctpl);
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult CTPLTermList()
        {
            CTPLTermModel ctptermlList = new CTPLTermModel();
            try
            {

                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                    ctptermlList.CTPLTermList = db.CTPLTerm.Where(o => o.Active == true).ToList();

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error has occured.";
            }
            return View(ctptermlList);
        }
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult CTPLTerm(int? id)
        {
            CTPLTermModel ctplterm = new CTPLTermModel();

            using (db = new VRSystemEntities())
            {
                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var ctpltermLoad = db.CTPLTerm.Where(o => o.CPTLTermID == id && o.Active == true).FirstOrDefault();

                    ctplterm.CPTLTermID = ctpltermLoad.CPTLTermID;
                    ctplterm.CoverageYear = ctpltermLoad.CoverageYear;
                    ctplterm.TermDescription = ctpltermLoad.TermDescription;
                    ctplterm.Active = ctpltermLoad.Active;
                }

                return View(ctplterm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult CTPLTerm(CTPLTermModel ctplterm, string submit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (submit)
                    {
                        case "Create":
                            using (db = new VRSystemEntities())
                            {

                                var newctplterm = new CTPLTerm
                                {
                                    CoverageYear = Convert.ToInt32(ctplterm.CoverageYear),
                                    TermDescription = ctplterm.TermDescription,
                                    Active = true,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now
                                };
                                db.CTPLTerm.Add(newctplterm);
                                db.SaveChanges();

                            }
                            TempData["SuccessMessage"] = "Created Successfully!";
                            break;

                        case "Save":
                            using (db = new VRSystemEntities())
                            {
                                var Update = db.CTPLTerm.Where(o => o.CPTLTermID == ctplterm.CPTLTermID).FirstOrDefault();
                                Update.CoverageYear = Convert.ToInt32(ctplterm.CoverageYear);
                                Update.TermDescription = ctplterm.TermDescription;
                                Update.UpdatedBy = CurrentUser.Details.UserID;
                                Update.UpdatedDate = DateTime.Now;
                                db.SaveChanges();
                            }
                            TempData["SuccessMessage"] = "Updated Successfully!";
                            break;
                        case "Delete":
                            using (db = new VRSystemEntities())
                            {
                                var Delete = db.CTPLTerm.Where(o => o.CPTLTermID == ctplterm.CPTLTermID).FirstOrDefault();
                                Delete.Active = false;
                                Delete.UpdatedBy = CurrentUser.Details.UserID;
                                Delete.UpdatedDate = DateTime.Now;
                                db.SaveChanges();
                            }
                            TempData["WarningMessage"] = "Removed Successfully!";
                            break;
                    }
                    return RedirectToAction("CTPLTermList");
                }
                else
                {
                    if (submit == "Create")
                        ViewBag.Edit = false;

                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error has occured.";
            }
            return View(ctplterm);
        }
    }
}