using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;

namespace VehicleRegistration.Controllers
{
    public class GetDataController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: GetList
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProvinceCity(int ProviceCode)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.City.Where(o => o.Active == true && o.ProvinceID == ProviceCode).OrderBy(o => o.CityName).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCityBarangay(int CityCode)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.Barangay.Where(o => o.Active == true && o.CityID == CityCode).OrderBy(o => o.BarangayName).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTitleTypeID(int TitleID)
        {
            using (var db = new VRSystemEntities())
            {
                var TitleTypeID = db.Title.Where(o => o.Active == true && o.TitleID == TitleID).FirstOrDefault().TitleTypeID;
                return Json(new { TitleTypeID }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetVehicleMake(decimal ID)
        {
            using (var db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                {
                    var searchresult = db.vwDealerVehicleMake.Where(o => o.Active == true && o.DealerID == ID).ToList();
                    return Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                {
                    var searchresult = db.vwMAIVehicleMake.Where(o => o.Active == true && o.MAIID == ID).ToList();
                    return Json(searchresult, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetVehicleModel(decimal VehicleMakeID)
        {
            using (var db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                var searchresult = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID).OrderBy(o => o.VehicleModelName).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDealerBranch(decimal DealerID)
        {
            using (var db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                var searchresult = db.DealerBranch.Where(o => o.Active == true && o.DealerID == DealerID).OrderBy(o => o.DealerBranchName).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPremiumType(int VehicleTypeID)
        {
            using (var db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var searchresult = (from a in db.MVPremium
                                    where a.VehicleTypeID == VehicleTypeID
                                    join b in db.VehicleClassification on a.VehicleClassificationID equals b.VehicleClassificationID into temp
                                    from temptbl in temp.DefaultIfEmpty()
                                    select new
                                    {
                                        temptbl
                                    })
                                    .Select(o => o.temptbl).ToList();

                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInsuranceAutoGenerated(int InsuranceID)
        {
            using (var db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var searchresult = db.Insurance.Where(o => o.InsuranceID == InsuranceID && o.Active == true).Select(o => o.AutoGenerateCOC).FirstOrDefault();

                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCSRFile(int VehicleID)
        {
            using (var db = new VRSystemEntities())
            {
                var seasrch = db
                    .VehicleInfo
                    .Where(o =>
                        o.Active == true &&
                        o.VehicleID == VehicleID
                        )
                        .Select(o => new
                        {
                            Byte = o.CertificateOfStockReport,
                            ContentType = o.CSRContentType
                        })
                        .FirstOrDefault();

                var searchresult = "//:0";
                if (seasrch.Byte != null)
                    searchresult = String.Format("data:" + seasrch.ContentType + ";base64,{0}", Convert.ToBase64String(seasrch.Byte));

                var jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        public ActionResult GetInvoiceFile(int VehicleID)
        {
            using (var db = new VRSystemEntities())
            {
                var seasrch = db
                    .DealerInvoice
                    .Where(o =>
                        o.Active == true &&
                        o.VehicleID == VehicleID
                        )
                        .Select(o => new
                        {
                            Byte = o.InvoiceByte,
                            ContentType = o.InvoiceContentType
                        })
                        .FirstOrDefault();

                //string path = Server.MapPath("~/scripts/Img/PDF.pdf");
                //System.IO.File.WriteAllBytes(path, seasrch.Byte);
                //string url_parth = Url.Content("~/scripts/Img/PDF.pdf");

                var searchresult = "//:0";
                if (seasrch.Byte != null)
                    searchresult = String.Format("data:" + seasrch.ContentType + ";base64,{0}", Convert.ToBase64String(seasrch.Byte));

                var jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        public ActionResult GetCOCFile(int VehicleID)
        {
            using (var db = new VRSystemEntities())
            {
                var seasrch = db
                    .VehicleInfo
                    .Where(o =>
                        o.Active == true &&
                        o.VehicleID == VehicleID
                        )
                        .Select(o => new
                        {
                            Byte = o.CertificateOfConformity,
                            ContentType = o.COCContentType
                        })
                        .FirstOrDefault();

                var searchresult = "//:0";
                if (seasrch.Byte != null)
                    searchresult = String.Format("data:" + seasrch.ContentType + ";base64,{0}", Convert.ToBase64String(seasrch.Byte));

                var jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        public ActionResult GetPNPFile(int VehicleID)
        {
            using (var db = new VRSystemEntities())
            {
                var seasrch = db
                    .VehicleInfo
                    .Where(o =>
                        o.Active == true &&
                        o.VehicleID == VehicleID
                        )
                        .Select(o => new
                        {
                            AutoPNP = o.AutoPNP,
                            Byte = o.PNPClearance,
                            ContentType = o.PNPContentType,
                            Byte2 = o.PNPReceipt,
                            ContentType2 = o.PNPReceiptContentType
                        })
                        .FirstOrDefault();

                var searchresult = "//:0";
                if (seasrch.AutoPNP == false && seasrch.Byte != null)
                    searchresult = String.Format("data:" + seasrch.ContentType + ";base64,{0}", Convert.ToBase64String(seasrch.Byte));
                else if(seasrch.AutoPNP == true && seasrch.Byte2 != null)
                    searchresult = String.Format("data:" + seasrch.ContentType2 + ";base64,{0}", Convert.ToBase64String(seasrch.Byte2));

                var jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
                //return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPaymentReceiptFile(int BatchID)
        {
            using (var db = new VRSystemEntities())
            {
                var seasrch = db
                    .BatchMaster
                    .Where(o =>
                        o.Active == true &&
                        o.BatchID == BatchID
                        )
                        .Select(o => new LTOBatchHeader
                        {
                            PaymentRef = o.PaymentRef,
                            PaymentImageByte = o.PaymentFileByte,
                            PaymentImageContentType = o.PaymentFileType
                        })
                        .FirstOrDefault();
                var searchresult = "//:0";
                if (seasrch.PaymentImageByte != null)
                    searchresult = String.Format("data:" + seasrch.PaymentImageContentType + ";base64,{0}", Convert.ToBase64String(seasrch.PaymentImageByte));

                var jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        public ActionResult GetEPatReceiptFile(int BatchID)
        {
            using (var db = new VRSystemEntities())
            {
                var seasrch = db
                    .BatchMaster
                    .Where(o =>
                        o.Active == true &&
                        o.BatchID == BatchID
                        )
                        .Select(o => new LTOBatchHeader
                        {
                            EPatImageByte = o.PaymentEPATFileByte,
                            EPatImageContentType = o.PaymentEPATFileType,
                        })
                        .FirstOrDefault();
                var searchresult = "//:0";
                if (seasrch.EPatImageByte != null)
                    searchresult = String.Format("data:" + seasrch.EPatImageContentType + ";base64,{0}", Convert.ToBase64String(seasrch.EPatImageByte));

                var jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
                //return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLTO_SubReferenceID(int reference_id)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.LTOBranch.Where(o => o.Active == true && o.LTOID == reference_id).Select(
                    o => new {
                        o.LTOBranchID,
                        o.LTOBranchName
                    }).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }
    }
}