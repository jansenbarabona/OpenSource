using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Tools;
using VehicleRegistration.Models;
using System.Configuration;

namespace VehicleRegistration.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Pricing()
        {
            return View();
        }
        public ActionResult Documentation()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
   
    
        [SessionExpire]
        public ActionResult Dashboard()
        {
            VRSystemEntities db = new VRSystemEntities();

            switch (CurrentUser.Details.UserEntityID)
            {

                case (int)UserEntityEnum.DataBridgeAsia:
                     
                    using (db = new VRSystemEntities())
                    {
                        ViewBag.DataBridge_DealerActiveCount = db.BatchMaster
                        .Where(o =>
                        o.Active == true &&
                        o.BatchTypeID == (int)BatchTypeList.LTO &&
                        o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                        o.Received == false &&
                        o.Rejected == false &&
                        o.Reprocessed == false &&
                        o.Cancelled == false
                        )
                        .Count();
                        ViewBag.DataBridge_LTOActiveCount = db.BatchMaster
                        .Where(o =>
                        o.Active == true &&
                        o.BatchTypeID == (int)BatchTypeList.LTO &&
                        o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                        o.ForPickUp == false &&
                        o.Rejected == false &&
                        o.Reprocessed == false &&
                        o.Cancelled == false &&
                        !(
                        o.Assessed == true &&
                        o.PaymentRef == null
                        )
                        )
                        .Count();
                    
                    }
            
                    break;
                case (int)UserEntityEnum.Dealer:
                    {
                        using (db = new VRSystemEntities())
                        {
                            if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                            {

                                ViewBag.VehicleNumber = db.VehicleInfo
                                    .Where(o => o.Active == true
                                    && o.DealerID == CurrentUser.Details.ReferenceID
                                    && o.Assigned == true
                                    ).Count();

                                var VehicleSoldIDNoFile = db.DealerInvoice.Where(o => o.InvoiceByte == null)
                                                    .Select(o => o.VehicleID).ToList();
                                var VehicleSoldID = db.DealerInvoice.Where(o => o.InvoiceByte != null)
                                                    .Select(o => o.VehicleID).ToList();

                                ViewBag.VehiclesForCSR = db.VehicleInfo.Where(o =>
                                                             o.CertificateOfStockReport == null
                                                          && o.Active == true
                                                          && o.DealerID == CurrentUser.Details.ReferenceID
                                                          ).Count();

                                ViewBag.VehiclesNotSold = db.VehicleInfo.Where(o =>
                                                            o.CertificateOfStockReport != null
                                                         && o.Active == true
                                                         && o.DealerID == CurrentUser.Details.ReferenceID
                                                         && (VehicleSoldIDNoFile.Contains(o.VehicleID) || !VehicleSoldID.Contains(o.VehicleID))
                                                         ).Distinct().Count();

                                ViewBag.VehiclesForCOC = db.VehicleInfo.Where(o =>
                                                            o.CertificateOfConformity == null
                                                         && o.Active == true
                                                         && o.DealerID == CurrentUser.Details.ReferenceID
                                                         && VehicleSoldID.Contains(o.VehicleID)
                                                         ).Count();

                                ViewBag.VehiclesForPNP = db.VehicleInfo.Where(o =>
                                                            o.CertificateOfConformity != null
                                                         && (o.PNPClearance == null && o.AutoPNP == false)
                                                         && o.Active == true
                                                         && o.DealerID == CurrentUser.Details.ReferenceID
                                                         ).Count();

                                ////////LTO PORTION
                                ViewBag.DealerSubmissionCount = (from a in db.vwVehicleList
                                                                 join b in db.DealerInvoice on a.VehicleID equals b.VehicleID into temp
                                                                 from temptbl in temp.DefaultIfEmpty()
                                                                 select new
                                                                 {
                                                                     a,
                                                                     temptbl.InvoiceByte
                                                                 }).Where(o =>
                                                                 o.a.CertificateOfStockReport != null &&
                                                                 o.InvoiceByte != null &&
                                                                 o.a.CertificateOfConformity != null &&
                                                                 (o.a.PNPClearance != null || o.a.AutoPNP == true) &&
                                                                 o.a.LTOSubmitted == false &&
                                                                 o.a.DealerID == (int)CurrentUser.Details.ReferenceID &&
                                                                 o.a.Active == true)
                                                                 .Count();

                                ViewBag.DealerSubmittedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                           && o.UserReference == CurrentUser.Details.ReferenceID
                                                           && o.DateSubmitted != null
                                                           && o.Assessed == false
                                                           && o.BatchTypeID == (int)BatchTypeList.LTO
                                                           && o.Active == true)
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.DateSubmitted,
                                                          BatchCount = o.BatchCount
                                                      }).Count();

                                ViewBag.DealerDIYRejectedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                           && o.UserReference == CurrentUser.Details.ReferenceID
                                                           && o.DateSubmitted == null
                                                           && o.Assessed == false
                                                           && o.Rejected == true
                                                           && o.BatchTypeID == (int)BatchTypeList.LTO
                                                           && o.Active == true)
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.DateSubmitted,
                                                          BatchCount = o.BatchCount
                                                      }).Count();

                                ViewBag.DealerForPaymentCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
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
                                                          DateSubmitted = o.AssessedDate,
                                                          BatchCount = o.BatchCount,
                                                          Assessed = o.Assessed,
                                                          AssessedAmount = o.AssessedAmount,
                                                          PaymentRef = o.PaymentRef,
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                ViewBag.DealerPendingCompletionCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
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
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                ViewBag.DealerCompletedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                             && o.UserReference == CurrentUser.Details.ReferenceID
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
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                ViewBag.DealerForPickUpCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
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
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                ViewBag.DealerReceivedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
                                                            && o.BatchTypeID == (int)BatchTypeList.LTO
                                                            && o.Completed == true
                                                            && o.ForPickUp == true
                                                            && o.Received == true
                                                            && o.Active == true)
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.ReceivedDate,
                                                          BatchCount = o.BatchCount,
                                                          Assessed = o.Assessed,
                                                          AssessedAmount = o.AssessedAmount,
                                                          PaymentRef = o.PaymentRef,
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                var RejectBatchID = (from detail in db.BatchDetails
                                                     join master in db.BatchMaster
                                                     on detail.BatchID equals master.BatchID
                                                     where detail.Rejected == true
                                                     && master.UserReference == CurrentUser.Details.ReferenceID
                                                     && master.BatchTypeID == (int)BatchTypeList.LTO
                                                     select master.BatchID).ToList();

                                ViewBag.DealerRejectedCount = db.BatchMaster.Where(o => RejectBatchID.Distinct().Contains(o.BatchID))
                                .Select(o => new LTOBatchHeader
                                {
                                    BatchID = o.BatchID,
                                })
                                .Count();
                            }
                            else
                            {

                                ViewBag.VehicleNumber = db.VehicleInfo
                                    .Where(o => o.Active == true
                                    && o.DealerID == CurrentUser.Details.ReferenceID
                                    && o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                    && o.Assigned == true
                                    ).Count();

                                var VehicleSoldIDNoFile = db.DealerInvoice.Where(o => o.InvoiceByte == null && o.DealerBranchID == CurrentUser.Details.SubReferenceID)
                                                    .Select(o => o.VehicleID).ToList();
                                var VehicleSoldID = db.DealerInvoice.Where(o => o.InvoiceByte != null && o.DealerBranchID == CurrentUser.Details.SubReferenceID)
                                                    .Select(o => o.VehicleID).ToList();

                                ViewBag.VehiclesForCSR = db.VehicleInfo.Where(o =>
                                                             o.CertificateOfStockReport == null
                                                          && o.Active == true
                                                          && o.DealerID == CurrentUser.Details.ReferenceID
                                                          && o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                                          ).Count();

                                ViewBag.VehiclesNotSold = db.VehicleInfo.Where(o =>
                                                            o.CertificateOfStockReport != null
                                                         && o.Active == true
                                                         && o.DealerID == CurrentUser.Details.ReferenceID
                                                         && o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                                         && (VehicleSoldIDNoFile.Contains(o.VehicleID) || !VehicleSoldID.Contains(o.VehicleID))
                                                         ).Distinct().Count();

                                ViewBag.VehiclesForCOC = db.VehicleInfo.Where(o =>
                                                            o.CertificateOfConformity == null
                                                         && o.Active == true
                                                         && o.DealerID == CurrentUser.Details.ReferenceID
                                                         && o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                                         && VehicleSoldID.Contains(o.VehicleID)
                                                         ).Count();

                                ViewBag.VehiclesForPNP = db.VehicleInfo.Where(o =>
                                                            o.CertificateOfConformity != null
                                                         && (o.PNPClearance == null && o.AutoPNP == false)
                                                         && o.Active == true
                                                         && o.DealerID == CurrentUser.Details.ReferenceID
                                                         && o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                                         ).Count();

                                ////////LTO PORTION
                                ViewBag.DealerSubmissionCount = (from a in db.vwVehicleList
                                                                 join b in db.DealerInvoice on a.VehicleID equals b.VehicleID into temp
                                                                 from temptbl in temp.DefaultIfEmpty()
                                                                 select new
                                                                 {
                                                                     a,
                                                                     temptbl.InvoiceByte
                                                                 }).Where(o =>
                                                                 o.a.CertificateOfStockReport != null &&
                                                                 o.InvoiceByte != null &&
                                                                 o.a.CertificateOfConformity != null &&
                                                                 (o.a.PNPClearance != null || o.a.AutoPNP == true) &&
                                                                 o.a.LTOSubmitted == false &&
                                                                 o.a.DealerID == (int)CurrentUser.Details.ReferenceID &&
                                                                 o.a.DealerBranchID == (int)CurrentUser.Details.SubReferenceID &&
                                                                 o.a.Active == true)
                                                                 .Count();

                                ViewBag.DealerSubmittedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                           && o.UserReference == CurrentUser.Details.ReferenceID
                                                           && o.UserSubRef == CurrentUser.Details.SubReferenceID
                                                           && o.DateSubmitted != null
                                                           && o.Assessed == false
                                                           && o.BatchTypeID == (int)BatchTypeList.LTO
                                                           && o.Active == true)
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.DateSubmitted,
                                                          BatchCount = o.BatchCount
                                                      }).Count();

                                ViewBag.DealerDIYRejectedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                           && o.UserReference == CurrentUser.Details.ReferenceID
                                                           && o.UserSubRef == CurrentUser.Details.SubReferenceID
                                                           && o.DateSubmitted != null
                                                           && o.Assessed == false
                                                           && o.Rejected == true
                                                           && o.BatchTypeID == (int)BatchTypeList.LTO
                                                           && o.Active == true)
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.DateSubmitted,
                                                          BatchCount = o.BatchCount
                                                      }).Count();

                                ViewBag.DealerForPaymentCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
                                                            && o.UserSubRef == CurrentUser.Details.SubReferenceID
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
                                                          DateSubmitted = o.AssessedDate,
                                                          BatchCount = o.BatchCount,
                                                          Assessed = o.Assessed,
                                                          AssessedAmount = o.AssessedAmount,
                                                          PaymentRef = o.PaymentRef,
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                ViewBag.DealerPendingCompletionCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
                                                            && o.UserSubRef == CurrentUser.Details.SubReferenceID
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
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                ViewBag.DealerCompletedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                             && o.UserReference == CurrentUser.Details.ReferenceID
                                                            && o.UserSubRef == CurrentUser.Details.SubReferenceID
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
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                ViewBag.DealerForPickUpCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
                                                            && o.UserSubRef == CurrentUser.Details.SubReferenceID
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
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                ViewBag.DealerReceivedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
                                                            && o.UserSubRef == CurrentUser.Details.SubReferenceID
                                                            && o.BatchTypeID == (int)BatchTypeList.LTO
                                                            && o.Completed == true
                                                            && o.ForPickUp == true
                                                            && o.Received == true
                                                            && o.Active == true)
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.ReceivedDate,
                                                          BatchCount = o.BatchCount,
                                                          Assessed = o.Assessed,
                                                          AssessedAmount = o.AssessedAmount,
                                                          PaymentRef = o.PaymentRef,
                                                          PaymentImageByte = o.PaymentFileByte,
                                                          PaymentImageContentType = o.PaymentFileType
                                                      }).Count();

                                var RejectBatchID = (from detail in db.BatchDetails
                                                     join master in db.BatchMaster
                                                     on detail.BatchID equals master.BatchID
                                                     where detail.Rejected == true
                                                     && master.UserReference == CurrentUser.Details.ReferenceID
                                                     && master.UserSubRef == CurrentUser.Details.SubReferenceID
                                                     && master.BatchTypeID == (int)BatchTypeList.LTO
                                                     select master.BatchID).ToList();

                                ViewBag.DealerRejectedCount = db.BatchMaster.Where(o => RejectBatchID.Distinct().Contains(o.BatchID))
                                .Select(o => new LTOBatchHeader
                                {
                                    BatchID = o.BatchID,
                                })
                                .Count();
                            }
                        }
                    }
                    break;
                case (int)UserEntityEnum.Insurance:
                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {


                        using (db = new VRSystemEntities())
                        {
                            ViewBag.EntityName = CurrentUser.Details.CompanyName;
                        }

                        using (db = new VRSystemEntities())
                        {
                            ViewBag.VehicleNumber = db.VehicleInfo
                                .Where(o => o.Active == true
                                && o.DealerID == CurrentUser.Details.ReferenceID
                                && o.Assigned == true
                                ).Count();

                        }

                        using (db = new VRSystemEntities())
                        {
                            ViewBag.Allocated = db.VehicleInfo
                                  .Where(o => o.DealerID == CurrentUser.Details.ReferenceID
                                  && o.Assigned == false
                                  && o.Active == true
                                  )
                                  .Count();

                        }
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.Vehicleofclients = db.Customer
                                .Where(o => o.Active == true
                                && o.DealerID == CurrentUser.Details.ReferenceID
                                ).Count();

                        }
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.numberofcsr = db.vwVehicleList.Where(o =>
                                                      o.CertificateOfStockReport == null
                                                      && o.Active == true
                                                      && o.DealerID == CurrentUser.Details.ReferenceID
                                                      && o.BOCCertificateOfPayment != null
                                                      && o.Assigned == true).Count();

                        }
                        using (db = new VRSystemEntities())
                        {
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

                            ViewBag.NumberForCOP = db.vwVehicleList.Where(o =>
                               o.CertificateOfConformity == null
                               &&
                               o.Active == true
                               &&
                               InvoiceVehicleIDList.Contains(o.VehicleID)
                            ).Count();

                        }

                        using (db = new VRSystemEntities())
                        {
                            ViewBag.NumberForLTO = db.VehicleInfo
                                  .Where(o => o.DealerID == CurrentUser.Details.ReferenceID
                                  && o.Assigned == true
                                  && o.Active == true
                                  && o.BOCCertificateOfPayment != null
                                  && o.StencilOfChasis != null
                                  && o.StencilOfEngine != null
                                  && o.CertificateOfStockReport != null
                                  && o.CertificateOfConformity != null
                                  && o.PNPClearance != null
                                  && o.LTOSubmitted == false
                                  )
                                  .Count();

                        }

                        //using (db = new VRSystemEntities())
                        //{
                        //    vwDealerList DealerInfoView = new vwDealerList();
                        //    db.Configuration.LazyLoadingEnabled = true;
                        //    ViewBag.Edit = true;
                        //    var Load = db.vwDealerList.Where(o => o.Active == true).ToList().FirstOrDefault();


                        //    DealerInfoView.Address = Load.Address;


                        //    return View(DealerInfoView);
                        //}

                        ////// mai dashboard /////
                        //using (db = new VRSystemEntities())
                        //{
                        //    ViewBag.MAIVehicleNumber = db.VehicleInfo
                        //        .Where(o => o.Active == true
                        //        && o.VehicleMakeID == CurrentUser.Details.ReferenceID
                        //        ).Count();

                        //}
                        //using (db = new VRSystemEntities())
                        //{
                        //    ViewBag.MAIBOC = db.VehicleInfo
                        //        .Where(o => o.Active == true
                        //        && o.VehicleBodyTypeID == CurrentUser.Details.ReferenceID
                        //        ).Count();

                        //}
                        //using (db = new VRSystemEntities())
                        //{
                        //    ViewBag.mainumberofcsr = db.MAI
                        //        .Where(o => o.Active == true
                        //        && o.MAIID == CurrentUser.Details.ReferenceID
                        //        ).Count();

                        //}
                    }
                    else
                    {
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.VehicleNumber = db.VehicleInfo
                                .Where(o => o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                && o.Active == true
                                && o.DealerID == CurrentUser.Details.ReferenceID
                                && o.Assigned == true
                                ).Count();
                        }
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.Vehicleofclients = db.Customer
                                .Where(o => o.CustomerID == CurrentUser.Details.SubReferenceID
                                && o.Active == true
                                && o.CustomerID == CurrentUser.Details.ReferenceID
                                ).Count();
                        }
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.MAIVehicleNumber = db.VehicleInfo
                                .Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                && o.Active == true
                                && o.VehicleMakeID == CurrentUser.Details.ReferenceID
                                && o.Assigned == true
                                ).Count();
                        }
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.MAIBOC = db.VehicleInfo
                                .Where(o => o.VehicleModelID == CurrentUser.Details.SubReferenceID
                                && o.Active == true
                                && o.VehicleBodyTypeID == CurrentUser.Details.ReferenceID
                                ).Count();
                        }

                        using (db = new VRSystemEntities())
                        {
                            ViewBag.mainumberofcsr = db.MAI
                                .Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                && o.Active == true
                                && o.MAIID == CurrentUser.Details.ReferenceID
                                ).Count();
                        }

                    }

                    if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                    {
                        //COUNT

                    }

                    break;
                case (int)UserEntityEnum.MAI:
                    {
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.MAIVehicleAllocation = db.VehicleInfo.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      && (o.DealerBranchID == null || o.DealerID == null)
                                      && o.BOCCertificateOfPayment != null
                                      && o.StencilOfEngine != null
                                      && o.StencilOfChasis != null
                                      && o.Active == true
                                ).Count();

                            ViewBag.MAIVehicleNumber = db.VehicleInfo.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                                        && o.Active == true).Count();

                            ViewBag.MAIBOC = db.VehicleInfo.Where(o => (o.DealerBranchID == null
                                      || o.DealerID == null)
                                      && o.BOCCertificateOfPayment == null
                                      && o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                      && o.Active == true).Count();

                            ViewBag.MAIForSOE = db.VehicleInfo.Where(o => (o.DealerBranchID == null
                                          || o.DealerID == null
                                          )
                                          &&
                                          o.BOCCertificateOfPayment != null
                                          &&
                                          o.StencilOfEngine == null
                                          &&
                                          o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                          &&
                                          o.Active == true).Count();

                            ViewBag.MAIForSOC = db.VehicleInfo.Where(o => (o.DealerBranchID == null
                                          || o.DealerID == null)
                                          && o.BOCCertificateOfPayment != null
                                          && o.StencilOfEngine != null
                                          && o.StencilOfChasis == null
                                          && o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                          && o.Active == true).Count();

                            //ViewBag.mainumberofcsr = db.vwVehicleList.Where(o =>
                            //                         o.MAIID == CurrentUser.Details.SubReferenceID
                            //                         && o.CertificateOfStockReport == null
                            //                         && o.Active == true
                            //                         && o.StencilOfEngine != null
                            //                         && o.StencilOfChasis != null
                            //                         && o.BOCCertificateOfPayment != null
                            //                         && o.CSRSubmitted != true
                            //                         ).Select(o => new vwVehicleListModel
                            //                         {
                            //                             VehicleID = o.VehicleID,
                            //                         }).Count();

                            ViewBag.MAIForDealer = db.VehicleInfo.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      //&& (o.DealerBranchID == null || o.DealerID == null)
                                      && o.Assigned == false
                                      && o.BOCCertificateOfPayment != null
                                      && o.CertificateOfStockReport != null
                                      && o.StencilOfEngine != null
                                      && o.StencilOfChasis != null
                                      && o.Active == true).Count();


                            //LTO Details
                            ViewBag.MAISubmissionCount = db.vwVehicleList.Where(o =>
                                                     o.MAIID == CurrentUser.Details.SubReferenceID
                                                     && o.CertificateOfStockReport == null
                                                     && o.Active == true
                                                     && o.StencilOfEngine != null
                                                     && o.StencilOfChasis != null
                                                     && o.BOCCertificateOfPayment != null
                                                     && o.CSRSubmitted != true
                                                     ).Select(o => new vwVehicleListModel
                                                     {
                                                         VehicleID = o.VehicleID,
                                                     }).Count();

                            ViewBag.MAISubmittedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                       && o.UserReference == CurrentUser.Details.ReferenceID
                                                       && o.Assessed == false
                                                       && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                       && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                  }).Count();

                            ViewBag.MAIForPaymentCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Assessed == true
                                                        && o.PaymentRef == null
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                  }).Count();

                            ViewBag.MAIPendingCompletionCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Assessed == true
                                                        && o.PaymentRef != null
                                                        && o.Completed == false
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                  }).Count();

                            ViewBag.MAICompletedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                         && o.UserReference == CurrentUser.Details.ReferenceID
                                                         && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                         && o.Assessed == true
                                                         && o.PaymentRef != null
                                                         && o.Completed == true
                                                         && o.ForPickUp == false
                                                         && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                  }).Count();

                            ViewBag.MAIForPickUpCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Completed == true
                                                        && o.ForPickUp == true
                                                        && o.Received == false
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                  }).Count();

                            ViewBag.MAIReceivedCount = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Completed == true
                                                        && o.ForPickUp == true
                                                        && o.Received == true
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID
                                                  }).Count();

                            var RejectBatchID = (from detail in db.BatchDetails
                                                 join master in db.BatchMaster
                                                 on detail.BatchID equals master.BatchID
                                                 where detail.Rejected == true
                                                    && master.EntityTypeID == (int)UserEntityEnum.MAI
                                                    && master.UserReference == CurrentUser.Details.ReferenceID
                                                    && master.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                 select master.BatchID).ToList();

                            ViewBag.MAIRejectedCount = db.BatchMaster.Where(o => RejectBatchID.Distinct().Contains(o.BatchID))
                            .Select(o => new LTOBatchHeader
                            {
                                BatchID = o.BatchID,
                            })
                            .Count();

                            //After LTO
                            ViewBag.MAIForCSRCount = (from v in db.VehicleInfo
                                                 join bd in db.BatchDetails
                                                 on v.VehicleID equals bd.VehicleID
                                                 join bm in db.BatchMaster
                                                 on bd.BatchID equals bm.BatchID
                                                 where bm.Received == true &&
                                                 bm.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                                                 v.DealerID == null &&
                                                 v.CertificateOfStockReport == null
                                                 select v.VehicleID
                                                 ).Count();

                            ViewBag.MAIForPNPCount = db.VehicleInfo.Where(o =>
                                                        o.CertificateOfStockReport != null
                                                     && (o.PNPClearance == null && o.AutoPNP == false)
                                                     && o.Assigned == false
                                                     && o.Active == true
                                                     ).Count();
                        }
                    }

                    break;
                case (int)UserEntityEnum.LTO:
                    {
                        using (db = new VRSystemEntities())
                        {
                            if ((CurrentUser.Details.SubReferenceID == Convert.ToInt32(ConfigurationManager.AppSettings["DIYLTOBranchID"].ToString()) && CurrentUser.Details.LTOUserTypeID == (int)LTOUserTypeEnum.VehicleRegistration) || CurrentUser.Details.LTOUserTypeID == (int)LTOUserTypeEnum.CSR)
                            {


                                ViewBag.LTOSubmittedCount = db.BatchMaster
                                    .Where(o =>
                                    o.Active == true &&
                                    o.BatchTypeID == (int)BatchTypeList.LTO &&
                                    o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                    o.DateSubmitted != null &&
                                    o.Downloaded == false &&
                                    o.Assessed == false &&
                                    (o.Rejected == false || o.Reprocessed == true)
                                    )
                                    .Count();
                                ViewBag.LTOAssessmentCount = db.BatchMaster
                                    .Where(o =>
                                    o.Active == true &&
                                    o.BatchTypeID == (int)BatchTypeList.LTO &&
                                    o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                    o.DateSubmitted != null &&
                                    o.Downloaded == true &&
                                    o.Assessed == false &&
                                    o.PaymentRef == null &&
                                    o.Completed == false
                                    )
                                    .Count();
                            }
                            ViewBag.LTOPaidCount = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == false &&
                                o.LTOBranchID == CurrentUser.Details.SubReferenceID
                                )
                                .Count();
                            ViewBag.LTOCompletedCount = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.BatchTypeID == (int)BatchTypeList.LTO &&
                                o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == true &&
                                o.ForPickUp == false &&
                                o.LTOBranchID == CurrentUser.Details.SubReferenceID
                                )
                                .Count();
                        }


                        //LTO CSR
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.LTOCSRSubmittedCount = db.BatchMaster
                               .Where(o =>
                               o.Active == true &&
                               o.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                               o.EntityTypeID == (int)UserEntityEnum.MAI &&
                               o.DateSubmitted != null &&
                               o.Downloaded == false &&
                               o.Assessed == false &&
                               (o.Rejected == false || o.Reprocessed == true)
                               )
                               .Count();
                            ViewBag.LTOCSRAssessmentCount = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                                o.EntityTypeID == (int)UserEntityEnum.MAI &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == false &&
                                o.PaymentRef == null &&
                                o.Completed == false
                                )
                                .Count();
                            ViewBag.LTOCSRPaidCount = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                                o.EntityTypeID == (int)UserEntityEnum.MAI &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == false
                                )
                                .Count();
                            ViewBag.LTOCSRCompletedCount = db.BatchMaster
                                .Where(o =>
                                o.Active == true &&
                                o.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                                o.EntityTypeID == (int)UserEntityEnum.MAI &&
                                o.DateSubmitted != null &&
                                o.Downloaded == true &&
                                o.Assessed == true &&
                                o.PaymentRef != null &&
                                o.Completed == true &&
                                o.ForPickUp == false
                                )
                                .Count();
                        }
                    }
                    break;
                case (int)UserEntityEnum.PNP:
                    {
                        using (db = new VRSystemEntities())
                        {
                            ViewBag.PNPApprovalList = db.VehicleInfo
                                .Where(o =>
                                    o.Active == true &&
                                    o.CertificateOfStockReport != null &&
                                    o.CertificateOfConformity != null &&
                                    o.PNPClearance == null &&
                                    o.AutoPNP == false
                                    )
                                .Count();
                            ViewBag.PNPCompletedList = db.VehicleInfo
                                .Where(o =>
                                    o.Active == true &&
                                    o.CertificateOfStockReport != null &&
                                    o.CertificateOfConformity != null &&
                                    o.PNPClearance != null ||
                                    o.AutoPNP == true
                                    )
                                .Count();
                        }
                    }
                    break;


            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}