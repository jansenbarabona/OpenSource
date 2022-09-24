using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Reporting.WebForms;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;

namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    public class ReportsController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Reports
        [HttpGet]
        public ActionResult LTOStatusReport()
        {
            LTOStatusReportModel model = new LTOStatusReportModel();
            using (db = new VRSystemEntities())
            {
                model.DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                model.DealerBranchList.Add(new DealerBranchModel() { DealerBranchID = 0, DealerBranchName = "ALL" });
                model.DealerBranchList.AddRange(db.DealerBranch.Where(o => o.Active == true).Select(o => new DealerBranchModel
                {
                    DealerBranchID = o.DealerBranchID,
                    DealerBranchName = o.DealerBranchName
                }).OrderBy(o => o.DealerBranchName).ToList());

                ViewBag.EntityType = "Databridge";
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetLTOStatusReportTable(int DealerID, int DealerBranchID)
        {
            using (db = new VRSystemEntities())
            {
                ViewBag.EntityType = "Databridge";
                List<LTOStatusReportTableModel> model = new List<LTOStatusReportTableModel>();

                var DealerBranchList = new List<DealerBranch>();
                if (DealerBranchID != 0)
                {
                    DealerBranchList = db.DealerBranch.Where(o => o.DealerID == DealerID && o.DealerBranchID == DealerBranchID && o.Active == true).ToList();
                }
                else
                {
                    DealerBranchList = db.DealerBranch.Where(o => o.DealerID == DealerID && o.Active == true).ToList();
                }

                foreach (var item in DealerBranchList)
                {
                    model.Add(new LTOStatusReportTableModel()
                    {
                        DealerID = item.DealerID,
                        DealerBranchID = item.DealerBranchID,
                        DealerName = db.Dealer.Where(o => o.Active == true && o.DealerID == item.DealerID).FirstOrDefault().DealerName,
                        DealerBranchName = item.DealerBranchName,
                        TotalSubmitted = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == false &&
                                o.Assessed == false &&
                                (o.Rejected == false || o.Reprocessed == true)
                                ).Count(),
                        TotalAssessment = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == false &&
                                o.PaymentRef == null &&
                                o.Completed == false
                                ).Count(),
                        TotalPaid = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == false
                                ).Count(),
                        TotalCompleted = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == true &&
                                o.ForPickUp == false
                                ).Count()
                        //TotalActive = db.BatchMaster
                        //        .Where(o =>
                        //        o.Active == true &&
                        //        o.UserReference == DealerID &&
                        //        o.UserSubRef == item.DealerBranchID &&
                        //        o.BatchTypeID == (int)BatchTypeList.LTO &&
                        //        o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                        //        o.ForPickUp == false
                        //        ).Count()
                    });
                }
                return PartialView("_LTOStatusReportTable", model);
            }
        }

        #region [DealerStatusReport]
        [HttpGet]
        public ActionResult DealerStatusReport()
        {
            DealerStatusReportModel model = new DealerStatusReportModel();
            using (db = new VRSystemEntities())
            {
                model.DealerList.Add(new DealerModel() { DealerID = 0, DealerName = "ALL" });
                model.DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new DealerModel
                {
                    DealerID = o.DealerID,
                    DealerName = o.DealerName
                }).OrderBy(o => o.DealerName).ToList());
                //model.DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                model.DealerBranchList.Add(new DealerBranchModel() { DealerBranchID = 0, DealerBranchName = "ALL" });
                model.DealerBranchList.AddRange(db.DealerBranch.Where(o => o.Active == true).Select(o => new DealerBranchModel
                {
                    DealerBranchID = o.DealerBranchID,
                    DealerBranchName = o.DealerBranchName

                }).OrderBy(o => o.DealerBranchName).ToList());

                ViewBag.EntityType = "Databridge";


                var DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                var DealerListID = DealerList.Select(o => o.DealerID).ToList();
                var DealerBranchList = db.DealerBranch.Where(o => DealerListID.Contains(o.DealerID) && o.Active == true).ToList();
                foreach (var item in DealerBranchList)
                {
                    model.TableList.Add(new DealerStatusReportTableModel()
                    {
                        DealerID = item.DealerID,
                        DealerBranchID = item.DealerBranchID,
                        DealerName = DealerList.Where(o => o.DealerID == item.DealerID).FirstOrDefault().DealerName,
                        DealerBranchName = item.DealerBranchName,
                        TotalSubmitted = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Assessed == false
                                ).Count(),
                        TotalPayment = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef == null &&
                                o.Completed == false
                                ).Count(),
                        TotalConfirmation = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == false
                                ).Count(),
                        TotalCompleted = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == true &&
                                o.ForPickUp == false
                                ).Count(),
                        TotalPickup = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.Completed == true &&
                                o.ForPickUp == true &&
                                o.Received == false
                                ).Count(),
                        TotalActive = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.Received == false
                                ).Count()
                    });
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDealerStatusReportTable(int DealerID, int DealerBranchID)
        {
            ViewBag.EntityType = "Databridge";
            //List<Dealer> DealerList = new List<Dealer>();
            //var DealerName = "";
            using (db = new VRSystemEntities())
            {
                List<DealerStatusReportTableModel> model = new List<DealerStatusReportTableModel>();
                var DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                var DealerListID = DealerList.Select(o => o.DealerID).ToList();

                var DealerBranchList = new List<DealerBranch>();
                if (DealerBranchID != 0)
                {
                    DealerBranchList = db.DealerBranch.Where(o => o.DealerID == DealerID && o.DealerBranchID == DealerBranchID && o.Active == true).ToList();

                }
                else
                {
                    if (DealerID != 0)
                        DealerBranchList = db.DealerBranch.Where(o => o.DealerID == DealerID && o.Active == true).ToList();
                    else
                        DealerBranchList = db.DealerBranch.Where(o => DealerListID.Contains(o.DealerID) && o.Active == true).ToList();
                    //DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                }


                foreach (var item in DealerBranchList)
                {
                    model.Add(new DealerStatusReportTableModel()
                    {
                        DealerID = item.DealerID,
                        DealerBranchID = item.DealerBranchID,
                        DealerName = DealerList.Where(o => o.DealerID == item.DealerID).FirstOrDefault().DealerName,
                        DealerBranchName = item.DealerBranchName,
                        TotalSubmitted = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Assessed == false
                                ).Count(),
                        TotalPayment = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef == null &&
                                o.Completed == false
                                ).Count(),
                        TotalConfirmation = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == false
                                ).Count(),
                        TotalCompleted = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == true &&
                                o.ForPickUp == false
                                ).Count(),
                        TotalPickup = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.Completed == true &&
                                o.ForPickUp == true &&
                                o.Received == false
                                ).Count(),
                        TotalActive = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.UserReference == item.DealerID &&
                                o.UserSubRef == item.DealerBranchID &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.Received == false
                                ).Count()
                    });
                }
                return PartialView("_DealerStatusReportTable", model);
            }
        }

        #endregion

        // _DealerVehicleStatusReportTable
        [HttpGet]
        public ActionResult DealerVehicleStatusReport()
        {
            DealerVehicleStatusReportModel model = new DealerVehicleStatusReportModel();
            using (db = new VRSystemEntities())
            {
                model.DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                model.DealerBranchList.Add(new DealerBranchModel() { DealerBranchID = 0, DealerBranchName = "ALL" });
                model.DealerBranchList.AddRange(db.DealerBranch.Where(o => o.Active == true).Select(o => new DealerBranchModel
                {
                    DealerBranchID = o.DealerBranchID,
                    DealerBranchName = o.DealerBranchName
                }).OrderBy(o => o.DealerBranchName).ToList());
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDealerVehicleStatusReportTable(int DealerID, int DealerBranchID)
        {
            using (db = new VRSystemEntities())
            {
                List<DealerVehicleStatusReportTableModel> model = new List<DealerVehicleStatusReportTableModel>();

                var DealerBranchList = new List<DealerBranch>();
                if (DealerBranchID != 0)
                {
                    DealerBranchList = db.DealerBranch.Where(o => o.DealerID == DealerID && o.DealerBranchID == DealerBranchID && o.Active == true).ToList();
                }
                else
                {
                    DealerBranchList = db.DealerBranch.Where(o => o.DealerID == DealerID && o.Active == true).ToList();
                }

                foreach (var item in DealerBranchList)
                {
                    var VehicleSoldID = db.DealerInvoice.Where(o => o.InvoiceByte != null && o.DealerBranchID == item.DealerBranchID)
                                        .Select(o => o.VehicleID).ToList();
                    model.Add(new DealerVehicleStatusReportTableModel()
                    {
                        DealerBranchName = item.DealerBranchName,

                        TotalInventory = db.VehicleInfo
                                .Where(o =>
                                o.Active == true &&
                                o.DealerID == DealerID &&
                                o.DealerBranchID == item.DealerBranchID &&
                                o.Assigned == true
                                ).Count(),
                        TotalCSR = db.VehicleInfo
                                .Where(o =>
                                o.Active == true &&
                                o.DealerID == DealerID &&
                                o.DealerBranchID == item.DealerBranchID &&
                                o.CertificateOfStockReport == null
                                ).Count(),
                        TotalInvoice = db.VehicleInfo
                                .Where(o =>
                                o.Active == true &&
                                o.DealerID == DealerID &&
                                o.DealerBranchID == item.DealerBranchID &&
                                o.CertificateOfStockReport != null &&
                                !VehicleSoldID.Contains(o.VehicleID)
                                ).Count(),
                        TotalCOC = db.VehicleInfo
                                .Where(o =>
                                o.Active == true &&
                                o.DealerID == DealerID &&
                                o.DealerBranchID == item.DealerBranchID &&
                                o.CertificateOfStockReport != null &&
                                VehicleSoldID.Contains(o.VehicleID) &&
                                o.CertificateOfConformity == null
                                ).Count(),
                        TotalPNP = db.VehicleInfo
                                .Where(o =>
                                o.Active == true &&
                                o.DealerID == DealerID &&
                                o.DealerBranchID == item.DealerBranchID &&
                                o.CertificateOfStockReport != null &&
                                VehicleSoldID.Contains(o.VehicleID) &&
                                o.CertificateOfConformity != null &&
                                (o.PNPClearance == null && o.AutoPNP == false)
                                ).Count(),
                        TotalSubmission = db.VehicleInfo
                                .Where(o =>
                                o.Active == true &&
                                o.DealerID == DealerID &&
                                o.DealerBranchID == item.DealerBranchID &&
                                o.CertificateOfStockReport != null &&
                                VehicleSoldID.Contains(o.VehicleID) &&
                                o.CertificateOfConformity != null &&
                                (o.PNPClearance != null || o.AutoPNP == true) &&
                                o.LTOSubmitted == false
                                ).Count(),
                        TotalActive = db.VehicleInfo
                                .Where(o =>
                                o.Active == true &&
                                o.DealerID == DealerID &&
                                o.DealerBranchID == item.DealerBranchID &&
                                o.Assigned == true &&
                                o.LTOSubmitted == false
                                ).Count()
                    });
                }
                return PartialView("_DealerVehicleStatusReportTable", model);
            }
        }

        #region [LTO]

        [HttpGet]
        public ActionResult PlateSection()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOPayment();

                var DealerList = new List<LTODealerFilter>();
                DealerList.Add(new LTODealerFilter() { DealerID = 0, DealerName = "ALL" });
                DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                {
                    DealerID = o.DealerID,
                    DealerName = o.DealerName
                }).OrderBy(o => o.DealerName).ToList());

                model.DealerList = DealerList;

                model.BatchList = db
                    .BatchMaster
                    .Where(o =>
                        o.Active == true &&
                        o.BatchTypeID == (int)BatchTypeList.LTO &&
                        o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                        o.DateSubmitted != null &&
                        o.Downloaded == true &&
                        o.Assessed == true &&
                        o.PaymentRef != null &&
                        o.Completed == false
                        )
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.PaymentDate,
                            BatchCount = o.BatchCount,
                            Assessed = o.Assessed,
                            //AssessedAmount = o.AssessedAmount,
                            PaymentRef = o.PaymentRef,
                            PaymentImageContentType = o.PaymentFileType,
                            EPatImageContentType = o.PaymentEPATFileType
                        })
                        .OrderByDescending(o => o.DateSubmitted)
                        .ToList();

                model.VehicleList = new List<LTOAssessBatchDetailVehicle>();

                return View(model);
            }
        }
        [HttpPost]
        public ActionResult PlateSection(LTOPayment model)
        {
            using (db = new VRSystemEntities())
            {
                var Update = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                db.SaveChanges();
                Functions.EmailPlateLTO(Update);
                Functions.LTOEmailStatus(model.SelectedBatchID, LTOStatus.Paid);
                TempData["SuccessMessage"] = "Plate Section and Status has been email to LTO!";
            }

            return RedirectToAction("PlateSection");
        }
        #endregion

        ////////////////////

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetBatchListReport(int DealerID, int DealerBranchID, string Status)
        {
            using (db = new VRSystemEntities())
            {
                ViewBag.EntityType = "Databridge";
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                switch (Status)
                {
                    case "Submitted":
                        BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                   && o.UserReference == DealerID
                                                   && o.UserSubRef == DealerBranchID
                                                   && o.BatchTypeID == (int)BatchTypeList.LTO
                                                   && o.DateSubmitted != null
                                                   && o.Assessed == false
                                                   && o.Active == true)
                                              .Select(o => new LTOBatchHeader
                                              {
                                                  BatchID = o.BatchID,
                                                  EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                  ReferenceNo = o.ReferenceNo,
                                                  BatchDescription = o.BatchDescription,
                                                  DateSubmitted = o.DateSubmitted,
                                                  BatchCount = o.BatchCount
                                              })
                                              .OrderByDescending(o => o.DateSubmitted)
                                              .ToList();
                        break;
                    case "Assessment":
                        BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                   && o.UserReference == DealerID
                                                   && o.UserSubRef == DealerBranchID
                                                   && o.BatchTypeID == (int)BatchTypeList.LTO
                                                   && o.Downloaded == true
                                                   && o.Assessed == false
                                                   && o.Active == true)
                                              .Select(o => new LTOBatchHeader
                                              {
                                                  BatchID = o.BatchID,
                                                  EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                  ReferenceNo = o.ReferenceNo,
                                                  BatchDescription = o.BatchDescription,
                                                  DateSubmitted = o.DateSubmitted,
                                                  BatchCount = o.BatchCount
                                              })
                                              .OrderByDescending(o => o.DateSubmitted)
                                              .ToList();
                        break;
                    case "Payment":
                        BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                        && o.UserReference == DealerID
                                        && o.UserSubRef == DealerBranchID
                                        && o.BatchTypeID == (int)BatchTypeList.LTO
                                        && o.Assessed == true
                                        && o.PaymentRef == null
                                        && o.Active == true)
                                   .Select(o => new LTOBatchHeader
                                   {
                                       BatchID = o.BatchID,
                                       EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                       ReferenceNo = o.ReferenceNo,
                                       BatchDescription = o.BatchDescription,
                                       DateSubmitted = o.DateSubmitted,
                                       BatchCount = o.BatchCount,
                                       Assessed = o.Assessed,
                                                  //AssessedAmount = o.AssessedAmount
                                              })
                                   .OrderByDescending(o => o.DateSubmitted)
                                   .ToList();
                        break;
                    case "Paid":
                    case "Confirmation":
                        BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                        && o.UserReference == DealerID
                                                        && o.UserSubRef == DealerBranchID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTO
                                                        && o.Assessed == true
                                                        && o.PaymentRef != null
                                                        && o.Completed == false
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                      EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                      ReferenceNo = o.ReferenceNo,
                                                      BatchDescription = o.BatchDescription,
                                                      DateSubmitted = o.PaymentDate,
                                                      BatchCount = o.BatchCount,
                                                      Assessed = o.Assessed,
                                                      AssessedAmount = o.AssessedAmount,
                                                      PaymentRef = o.PaymentRef,
                                                      PaymentImageContentType = o.PaymentFileType,
                                                      EPatImageContentType = o.PaymentEPATFileType
                                                  })
                                                  .OrderByDescending(o => o.DateSubmitted)
                                                  .ToList();
                        break;
                    case "Completed":
                        BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                           && o.UserReference == DealerID
                                           && o.UserSubRef == DealerBranchID
                                           && o.BatchTypeID == (int)BatchTypeList.LTO
                                           && o.Assessed == true
                                           && o.PaymentRef != null
                                           && o.Completed == true
                                           && o.ForPickUp == false
                                           && o.Active == true)
                                     .Select(o => new LTOBatchHeader
                                     {
                                         BatchID = o.BatchID,
                                         EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                         ReferenceNo = o.ReferenceNo,
                                         BatchDescription = o.BatchDescription,
                                         DateSubmitted = o.DateSubmitted,
                                         BatchCount = o.BatchCount,
                                         Assessed = o.Assessed,
                                         AssessedAmount = o.AssessedAmount,
                                         PaymentRef = o.PaymentRef,
                                         PaymentImageContentType = o.PaymentFileType,
                                         EPatImageContentType = o.PaymentEPATFileType
                                     })
                                     .OrderByDescending(o => o.DateSubmitted)
                                     .ToList();
                        break;
                    case "Pickup":BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                        && o.UserReference == DealerID
                                                        && o.UserSubRef == DealerBranchID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTO
                                                        && o.Completed == true
                                                        && o.ForPickUp == true
                                                        && o.Received == false
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                      EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                      ReferenceNo = o.ReferenceNo,
                                                      BatchDescription = o.BatchDescription,
                                                      DateSubmitted = o.ForPickUpDate,
                                                      BatchCount = o.BatchCount,
                                                      Assessed = o.Assessed,
                                                      AssessedAmount = o.AssessedAmount,
                                                      PaymentRef = o.PaymentRef,
                                                      PaymentImageContentType = o.PaymentFileType,
                                                      EPatImageContentType = o.PaymentEPATFileType
                                                  })
                                                  .OrderByDescending(o => o.DateSubmitted)
                                                  .ToList();
                        break;
                }
                return PartialView("_BatchList", BatchList);
            }
        }

        #region [Company List Report]
        [HttpGet]
        public ActionResult CompanyListReport()
        {
            CompanyListReportModel model = new CompanyListReportModel();
            using (db = new VRSystemEntities())
            {
                model.EntityList = new List<EntityList>();
                model.EntityList.Add(new EntityList() { EntityID = 1, EntityName = "MAI" });
                model.EntityList.Add(new EntityList() { EntityID = 2, EntityName = "Dealer" });
                model.EntityList.Add(new EntityList() { EntityID = 3, EntityName = "Insurance" });
                model.EntityList.Add(new EntityList() { EntityID = 4, EntityName = "LTO" });
                model.EntityList.Add(new EntityList() { EntityID = 5, EntityName = "PNP" });

            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyListReport(CompanyListReportModel model)
        {
            string path = CompanyListReports(model.SelectedEntityID);
            using (db = new VRSystemEntities())
            {
                model.EntityList = new List<EntityList>();
                model.EntityList.Add(new EntityList() { EntityID = 1, EntityName = "MAI" });
                model.EntityList.Add(new EntityList() { EntityID = 2, EntityName = "Dealer" });
                model.EntityList.Add(new EntityList() { EntityID = 3, EntityName = "Insurance" });
                model.EntityList.Add(new EntityList() { EntityID = 4, EntityName = "LTO" });
                model.EntityList.Add(new EntityList() { EntityID = 5, EntityName = "PNP" });
            }
            return View(model);
        }


        public string CompanyListReports(int EntityID)
        {

            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "Company List Report.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            //else
            //{
            //    return RedirectToAction("Index");
            //}

            DataTable dt = new DataTable("DataSet1");
            dt.Columns.Add(new DataColumn("Reference", typeof(string)));
            dt.Columns.Add(new DataColumn { ColumnName = "SubReference", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Email", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "AccreditationNumber", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Main", DataType = typeof(string), AllowDBNull = true });



            List<EntityModel> Entity = new List<EntityModel>();
            using (db = new VRSystemEntities())
            {
                if (EntityID == 1)
                {
                    Entity = db.MAI.Where(o => o.Active == true).Select(o => new EntityModel()
                    {
                        CompanyID = o.MAIID,
                        CompanyName = o.MAIName,
                        BranchID = o.MAIID,
                        BranchName = o.MAIName,
                        Email = o.EmailAddress,
                        AccreditationNumber = o.AccreditationNumber,
                        Main = true,
                    }).ToList();
                }
                else if (EntityID == 2)
                {
                    Entity = (from a in db.Dealer
                              join b in db.DealerBranch on a.DealerID equals b.DealerID into temp
                              from temptbl in temp.DefaultIfEmpty()
                              select new
                              {
                                  c = a,
                                  b = temptbl
                              }).Where(o => o.c.Active == true && o.b.Active == true)
                              .Select(o => new EntityModel()
                              {
                                  CompanyID = o.c.DealerID,
                                  CompanyName = o.c.DealerName,
                                  BranchID = o.b.DealerBranchID,
                                  BranchName = o.b.DealerBranchName,
                                  Email = o.b.EmailAddress,
                                  AccreditationNumber = o.b.AccreditationNumber,
                                  Main = o.b.IsMain,
                              }).ToList();
                }
                else if (EntityID == 3)
                {
                    Entity = (from a in db.Insurance
                              join b in db.InsuranceBranch on a.InsuranceID equals b.InsuranceID into temp
                              from temptbl in temp.DefaultIfEmpty()
                              select new
                              {
                                  c = a,
                                  b = temptbl
                              })
                              .Where(o => o.c.Active == true && o.b.Active == true)
                              .Select(o => new EntityModel()
                              {
                                  CompanyID = o.c.InsuranceID,
                                  CompanyName = o.c.InsuranceName,
                                  BranchID = o.b.InsuranceBranchID,
                                  BranchName = o.b.InsuranceBranchName,
                                  Email = o.b.EmailAddress,
                                  AccreditationNumber = "",
                                  Main = o.b.IsMain,
                              }).ToList();
                }
                else if (EntityID == 4)
                {
                    Entity = (from a in db.LTO
                              join b in db.LTOBranch on a.LTOID equals b.LTOID into temp
                              from temptbl in temp.DefaultIfEmpty()
                              select new
                              {
                                  c = a,
                                  b = temptbl
                              })
                              .Where(o => o.c.Active == true && o.b.Active == true)
                              .Select(o => new EntityModel()
                              {
                                  CompanyID = o.c.LTOID,
                                  CompanyName = o.c.LTOName,
                                  BranchID = o.b.LTOBranchID,
                                  BranchName = o.b.LTOBranchName,
                                  Email = o.b.EmailAddress,
                                  AccreditationNumber = "",
                                  Main = o.b.IsMain,
                              }).ToList();
                }
                else if (EntityID == 5)
                {

                }


                foreach (var item in Entity)
                {
                    var IsMain = "No";
                    if (item.Main)
                    {
                        IsMain = "Yes";
                    }

                    dt.Rows.Add(
                        item.CompanyName,
                        item.BranchName,
                        item.Email,
                        item.AccreditationNumber,
                        IsMain);
                }
            }

            //string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            //lr.EnableExternalImages = true;
            //lr.EnableHyperlinks = true;

            //ReportParameter[] prm = new ReportParameter[3];
            //prm[0] = new ReportParameter("DealerParameter", DealerName);
            //prm[1] = new ReportParameter("DateFromParameter", DateFrom.Date.ToShortDateString());
            //prm[2] = new ReportParameter("DateToParameter", DateTo.Date.ToShortDateString());
            //lr.SetParameters(prm);

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
            ViewBag.Path = "CompanyListReport - " + timecreated + ".pdf";
            var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "CompanyListReport - " + timecreated + ".pdf";


            //var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "Company List Report.pdf";
            //System.IO.File.Delete(pdfPath);
            //ViewBag.Path = "Company List Report.pdf";
            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }
            //ViewBag.pfpath = pdfPath;
            return pdfPath;
        }

        #endregion


        #region [Wallet Report]
        [HttpGet]
        public ActionResult WalletReport()
        {
            WalletReportModel model = new WalletReportModel();
            using (db = new VRSystemEntities())
            {
                model.EntityList = new List<EntityList>();
                model.EntityList.AddRange(db.UserEntity.Where(o => o.Active == true && o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).Select(o => new EntityList
                {
                    EntityID = o.UserEntityID,
                    EntityName = o.UserEntityName
                }).ToList());

                model.TransactionTypeList.Add(new TransactionTypeList() { TransactionTypeID = 0, TransactionTypeName = "ALL" });
                model.TransactionTypeList.AddRange(db.TransactionType.Where(o => o.Active == true).Select(o => new TransactionTypeList
                {
                    TransactionTypeID = o.TransactionTypeID,
                    TransactionTypeName = o.TransactionName
                }).OrderBy(o => o.TransactionTypeName).ToList());
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WalletReport(WalletReportModel model)
        {
            string path = WalletReports(model);
            using (db = new VRSystemEntities())
            {
                model = new WalletReportModel();
                model.EntityList = new List<EntityList>();
                model.EntityList.AddRange(db.UserEntity.Where(o => o.Active == true && o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).Select(o => new EntityList
                {
                    EntityID = o.UserEntityID,
                    EntityName = o.UserEntityName
                }).ToList());

                model.TransactionTypeList.Add(new TransactionTypeList() { TransactionTypeID = 0, TransactionTypeName = "ALL" });
                model.TransactionTypeList.AddRange(db.TransactionType.Where(o => o.Active == true).Select(o => new TransactionTypeList
                {
                    TransactionTypeID = o.TransactionTypeID,
                    TransactionTypeName = o.TransactionName
                }).OrderBy(o => o.TransactionTypeName).ToList());
            }
            return View(model);
        }
        public string WalletReports(WalletReportModel model)
        {

            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "Wallet Report.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            //else
            //{
            //    return RedirectToAction("Index");
            //}

            DataTable dt = new DataTable("DataSet1");
            dt.Columns.Add(new DataColumn("CompanyName", typeof(string)));
            dt.Columns.Add(new DataColumn { ColumnName = "TransactionName", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "TransactionTotalAmount", DataType = typeof(decimal), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "TransactionTotalCount", DataType = typeof(string), AllowDBNull = true });


            using (db = new VRSystemEntities())
            {
                var SelectedCompanyID = model.SelectedCompanyID;
                var SelectedEntityID = model.SelectedEntityID;
                var SelectedTransactionID = model.SelectedTransactionID;
                var PeriodFrom = model.PeriodFrom;
                var PeriodTo = model.PeriodTo;

                var Result = (from a in db.Transaction
                              join b in db.TransactionType on a.TransactionTypeID equals b.TransactionTypeID
                              join c in db.TransactionEntryType on b.TransactionEntryTypeID equals c.TransactionEntryTypeID
                              join d in db.Wallet on a.WalletID equals d.WalletID
                              join e in db.User on a.CreatedBy equals e.UserID
                              where d.EntityID == SelectedCompanyID
                              && d.UserEntityID == SelectedEntityID
                              && (DbFunctions.TruncateTime(a.CreatedDate) >= PeriodFrom && PeriodTo >= DbFunctions.TruncateTime(a.CreatedDate))
                              select new
                              {
                                  TransactionTypeID = b.TransactionTypeID,
                                  CompanyID = d.EntityID,
                                  TransactionEntry = c.TransactionEntryTypeName,
                                  TransactionName = b.TransactionName,
                                  Amount = a.Amount * c.TransactionEntryMultiplier,
                                  Remarks = a.Remarks,
                                  Date = a.CreatedDate,
                                  Count = 1
                              })
                              .GroupBy(o => o.TransactionName)
                              .Select(o => new {
                                  CompanyID = o.FirstOrDefault().CompanyID,
                                  TransactionTypeID = o.FirstOrDefault().TransactionTypeID,
                                  TransactionName = o.FirstOrDefault().TransactionName,
                                  TotalAmount = o.Sum(t => t.Amount),
                                  TotalCount = o.Sum(c => c.Count)
                              }).ToList();

                if (SelectedTransactionID != 0)
                {
                    Result = Result.Where(o => o.TransactionTypeID == SelectedTransactionID).ToList();
                }
                

                foreach (var item in Result)
                {
                    CompanyList companyinfo = new CompanyList();
                    if (model.SelectedEntityID == (int)UserEntityEnum.MAI)
                    {
                        companyinfo = db.MAI
                            .Where(o => o.Active == true && o.MAIID == item.CompanyID)
                            .Select(o => new CompanyList() { 
                                CompanyName = o.MAIName
                            }).FirstOrDefault();
                    }
                    else if (model.SelectedEntityID == (int)UserEntityEnum.Dealer)
                    {
                        companyinfo = db.Dealer
                            .Where(o => o.Active == true && o.DealerID == item.CompanyID)
                            .Select(o => new CompanyList()
                            {
                                CompanyName = o.DealerName
                            }).FirstOrDefault();
                    }
                    else if (model.SelectedEntityID == (int)UserEntityEnum.Insurance)
                    {
                        companyinfo = db.Insurance
                            .Where(o => o.Active == true && o.InsuranceID == item.CompanyID)
                            .Select(o => new CompanyList()
                            {
                                CompanyName = o.InsuranceName
                            }).FirstOrDefault();
                    }
                    else if (model.SelectedEntityID == (int)UserEntityEnum.LTO)
                    {
                        companyinfo = db.LTO
                            .Where(o => o.Active == true && o.LTOID == item.CompanyID)
                            .Select(o => new CompanyList()
                            {
                                CompanyName = o.LTOName
                            }).FirstOrDefault();
                    }
                    //else if (model.SelectedEntityID == (int)UserEntityEnum.PNP)
                    //{

                    //    companyinfo = db.MAI
                    //        .Where(o => o.Active == true && o.MAIID == item.CompanyID)
                    //        .Select(o => new CompanyList()
                    //        {
                    //            CompanyName = o.MAIName
                    //        }).FirstOrDefault();
                    //}

                    dt.Rows.Add(
                        companyinfo.CompanyName,
                        item.TransactionName,
                        item.TotalAmount,
                        item.TotalCount);
                }
            }

            //string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            //lr.EnableExternalImages = true;
            //lr.EnableHyperlinks = true;

            ReportParameter[] prm = new ReportParameter[2];
            prm[0] = new ReportParameter("PeriodFrom", model.PeriodFrom?.ToShortDateString());
            prm[1] = new ReportParameter("PeriodTo", model.PeriodTo?.ToShortDateString());
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
            ViewBag.Path = "Wallet Report - " + timecreated + ".pdf";
            var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "Wallet Report - " + timecreated + ".pdf";


            //var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "Company List Report.pdf";
            //System.IO.File.Delete(pdfPath);
            //ViewBag.Path = "Company List Report.pdf";
            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }
            //ViewBag.pfpath = pdfPath;
            return pdfPath;
        }
        #endregion

        #region [SOA Report]
        [HttpGet]
        public ActionResult StatementOfAccount()
        {
            SOAReportModel model = new SOAReportModel();
            using (db = new VRSystemEntities())
            {
                model.EntityList = new List<EntityList>();
                model.EntityList.AddRange(db.UserEntity.Where(o => o.Active == true && o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).Select(o => new EntityList
                {
                    EntityID = o.UserEntityID,
                    EntityName = o.UserEntityName
                }).ToList());
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StatementOfAccount(SOAReportModel model)
        {
            string path = StatementOfAccountReport(model);
            using (db = new VRSystemEntities())
            {
                model.EntityList = new List<EntityList>();
                model.EntityList.AddRange(db.UserEntity.Where(o => o.Active == true && o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).Select(o => new EntityList
                {
                    EntityID = o.UserEntityID,
                    EntityName = o.UserEntityName
                }).ToList());

                model.CompanyList = new List<CompanyList>();
                if (model.SelectedEntityID == (int)UserEntityEnum.MAI)
                {
                    model.CompanyList = db.MAI.Where(o => o.Active == true).Select(o => new CompanyList { 
                        CompanyID = o.MAIID,
                        CompanyName = o.MAIName
                    }).ToList();
                }
                else if (model.SelectedEntityID == (int)UserEntityEnum.Dealer)
                {
                    model.CompanyList = db.Dealer.Where(o => o.Active == true).Select(o => new CompanyList
                    {
                        CompanyID = o.DealerID,
                        CompanyName = o.DealerName
                    }).ToList();
                }
                else if (model.SelectedEntityID == (int)UserEntityEnum.Insurance)
                {
                    model.CompanyList = db.Insurance.Where(o => o.Active == true).Select(o => new CompanyList
                    {
                        CompanyID = o.InsuranceID,
                        CompanyName = o.InsuranceName
                    }).ToList();
                }
                else if (model.SelectedEntityID == (int)UserEntityEnum.LTO)
                {
                    model.CompanyList = db.LTO.Where(o => o.Active == true).Select(o => new CompanyList
                    {
                        CompanyID = o.LTOID,
                        CompanyName = o.LTOName
                    }).ToList();
                }

            }
            return View(model);
        }
        public string StatementOfAccountReport(SOAReportModel model)
        {

            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "SOAReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            //else
            //{
            //    return RedirectToAction("Index");
            //}

            DataTable dt = new DataTable("DataSet1");
            dt.Columns.Add(new DataColumn { ColumnName = "Branch", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "BeginningBalance", DataType = typeof(decimal), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "TransDate", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "TransType", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "TransAmount", DataType = typeof(decimal), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "VehicleMake", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "VehicleModel", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "VehicleColor", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Customer", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "VehicleAmount", DataType = typeof(decimal), AllowDBNull = true });


            var CompanyName = new CompanyList();
            using (db = new VRSystemEntities())
            {
                var SelectedEntityID = model.SelectedEntityID;
                var SelectedCompanyID = model.SelectedCompanyID;
                var PeriodFrom = model.PeriodFrom;
                var PeriodTo = model.PeriodTo;

                var Result = (from a in db.Transaction
                              join b in db.TransactionType on a.TransactionTypeID equals b.TransactionTypeID
                              join c in db.TransactionEntryType on b.TransactionEntryTypeID equals c.TransactionEntryTypeID
                              join d in db.Wallet on a.WalletID equals d.WalletID
                              join e in db.User on a.CreatedBy equals e.UserID
                              join f in db.vwVehicleList on a.VehicleID equals f.VehicleID
                              join g in db.DealerInvoice on a.VehicleID equals g.VehicleID
                              join h in db.Customer on g.CustomerID equals h.CustomerID
                              join i in db.Title on h.TitleID equals i.TitleID
                              where d.EntityID == SelectedCompanyID
                              && d.UserEntityID == SelectedEntityID
                              && (DbFunctions.TruncateTime(a.CreatedDate) >= PeriodFrom && PeriodTo >= DbFunctions.TruncateTime(a.CreatedDate))
                              select new
                              {
                                  Branch = g.DealerBranchID,
                                  BeginningBalance = 0,
                                  TransDate = a.CreatedDate,
                                  TransType = b.TransactionName,
                                  TransAmount = a.Amount,
                                  VehicleMake = f.VehicleMakeName,
                                  VehicleModel = f.VehicleModelName,
                                  VehicleColor = f.VehicleColorName,
                                  Buyer = h.CustomerID,
                                  BuyerTitleType = i.TitleTypeID,
                                  VehicleValue = g.VehicleCost
                              }).ToList();

                foreach (var item in Result)
                {
                    BranchList branchinfo = new BranchList();
                    if (model.SelectedEntityID == (int)UserEntityEnum.MAI)
                    {
                        branchinfo = new BranchList()
                        {
                            BranchName = ""
                        };
                    }
                    else if (model.SelectedEntityID == (int)UserEntityEnum.Dealer)
                    {
                        CompanyName = db.Dealer.Where(o => o.Active == true && o.DealerID == SelectedCompanyID)
                            .Select(o => new CompanyList()
                            {
                                CompanyName = o.DealerName
                            }).FirstOrDefault();;
                        branchinfo = db.DealerBranch
                            .Where(o => o.Active == true && o.DealerID == SelectedCompanyID && o.DealerBranchID == item.Branch)
                            .Select(o => new BranchList()
                            {
                                BranchName = o.DealerBranchName
                            }).FirstOrDefault();
                    }
                    else if (model.SelectedEntityID == (int)UserEntityEnum.Insurance)
                    {
                        branchinfo = db.InsuranceBranch
                            .Where(o => o.Active == true && o.InsuranceID == SelectedCompanyID && o.InsuranceBranchID == item.Branch)
                            .Select(o => new BranchList()
                            {
                                BranchName = o.InsuranceBranchName
                            }).FirstOrDefault();
                    }
                    else if (model.SelectedEntityID == (int)UserEntityEnum.LTO)
                    {
                        branchinfo = db.LTOBranch
                            .Where(o => o.Active == true && o.LTOID == SelectedCompanyID && o.LTOBranchID == item.Branch)
                            .Select(o => new BranchList()
                            {
                                BranchName = o.LTOBranchName
                            }).FirstOrDefault();
                    }
                    var buyerinfo = db.Customer.Where(o => o.CustomerID == item.Buyer).Select(o => new { o.CorpName, o.LastName, o.FirstName, o.MiddleName}).FirstOrDefault();
                    var buyername = buyerinfo.CorpName;
                    if (item.BuyerTitleType == 1)
                    {
                        buyername = buyerinfo.LastName + ", " + buyerinfo.FirstName + " " + buyerinfo.MiddleName;
                    }

                    dt.Rows.Add(
                        branchinfo.BranchName,
                        (decimal)item.BeginningBalance,
                        item.TransDate,
                        item.TransType,
                        (decimal)item.TransAmount,
                        item.VehicleMake,
                        item.VehicleModel,
                        item.VehicleColor,
                        buyername,
                        (decimal)item.VehicleValue);
                }
            }

            //string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            //lr.EnableExternalImages = true;
            //lr.EnableHyperlinks = true;

            ReportParameter[] prm = new ReportParameter[4];
            prm[0] = new ReportParameter("PeriodFrom", model.PeriodFrom?.ToShortDateString());
            prm[1] = new ReportParameter("PeriodTo", model.PeriodTo?.ToShortDateString());
            prm[2] = new ReportParameter("RunDate", DateTime.Today.ToShortDateString());
            prm[3] = new ReportParameter("CompanyName", CompanyName.CompanyName.ToString()) ;
            lr.SetParameters(prm);

            ReportDataSource rds = new ReportDataSource("DataSet1", dt);
            lr.DataSources.Clear();
            lr.DataSources.Add(rds);

            lr.Refresh();


            string reportType = "Excel";
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
            ViewBag.Path = "SOA Report - " + timecreated + ".xls";
            var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "SOA Report - " + timecreated + ".xls";


            //var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + "Company List Report.pdf";
            //System.IO.File.Delete(pdfPath);
            //ViewBag.Path = "Company List Report.pdf";
            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }
            //ViewBag.pfpath = pdfPath;
            return pdfPath;
        }
        #endregion


        public ActionResult GetCompany(int EntityID)
        {
            var jsonResult = Json("", JsonRequestBehavior.AllowGet);
            using (var db = new VRSystemEntities())
            {
                if (EntityID == (int)UserEntityEnum.MAI)
                {
                    var searchresult = db.MAI.Where(o => o.Active == true && o.MAITypeID == EntityID)
                        .Select(o => new CompanyList()
                        {
                            CompanyID = o.MAIID,
                            CompanyName = o.MAIName
                        }).ToList();
                    jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (EntityID == (int)UserEntityEnum.Dealer)
                {
                    var searchresult = db.Dealer.Where(o => o.Active == true)
                        .Select(o => new CompanyList()
                        {
                            CompanyID = o.DealerID,
                            CompanyName = o.DealerName
                        }).ToList();
                    jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (EntityID == (int)UserEntityEnum.Insurance)
                {
                    var searchresult = db.Insurance.Where(o => o.Active == true)
                        .Select(o => new CompanyList()
                        {
                            CompanyID = o.InsuranceID,
                            CompanyName = o.InsuranceName
                        }).ToList();
                    jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (EntityID == (int)UserEntityEnum.LTO)
                {
                    var searchresult = db.LTO.Where(o => o.Active == true)
                        .Select(o => new CompanyList()
                        {
                            CompanyID = o.LTOID,
                            CompanyName = o.LTOName
                        }).ToList();
                    jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (EntityID == (int)UserEntityEnum.PNP)
                {
                    //var searchresult = db.PNP.Where(o => o.Active == true && o.MAITypeID == EntityID).ToList();
                    //jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }

                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        public ActionResult GetBranch(int EntityID, int CompanyID)
        {
            var jsonResult = Json("", JsonRequestBehavior.AllowGet);
            using (var db = new VRSystemEntities())
            {
                if (EntityID == (int)UserEntityEnum.MAI)
                {
                    //var searchresult = db.MAI.Where(o => o.Active == true && o.MAITypeID == EntityID).ToList();
                    //jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (EntityID == (int)UserEntityEnum.Dealer)
                {
                    var searchresult = db.DealerBranch.Where(o => o.Active == true && o.DealerID == CompanyID)
                        .Select(o => new BranchList()
                        {
                            BranchID = o.DealerBranchID,
                            BranchName = o.DealerBranchName
                        }).ToList();
                    jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (EntityID == (int)UserEntityEnum.Insurance)
                {
                    var searchresult = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceID == CompanyID)
                        .Select(o => new BranchList()
                        {
                            BranchID = o.InsuranceBranchID,
                            BranchName = o.InsuranceBranchName
                        }).ToList();
                    jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (EntityID == (int)UserEntityEnum.LTO)
                {
                    var searchresult = db.LTOBranch.Where(o => o.Active == true && o.LTOID == CompanyID)
                        .Select(o => new BranchList()
                        {
                            BranchID = o.LTOBranchID,
                            BranchName = o.LTOBranchName
                        }).ToList();
                    jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }
                else if (EntityID == (int)UserEntityEnum.PNP)
                {
                    //var searchresult = db.PNP.Where(o => o.Active == true && o.MAITypeID == EntityID).ToList();
                    //jsonResult = Json(searchresult, JsonRequestBehavior.AllowGet);
                }

                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
    }
}