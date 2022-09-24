using System;
using ExcelDataReader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;
using ClosedXML.Excel;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Drawing;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Web.Caching;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Ajax.Utilities;
using System.Configuration;
using ZohoMail;

namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    public class VehicleInfoController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Vehicle
        //[AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia, UserEntityEnum.DataBridgeAsia })]
        //[AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                List<vwVehicleListModel> VehicleList = new List<vwVehicleListModel>();

                switch (CurrentUser.Details.UserEntityID)
                {
                    case (int)UserEntityEnum.Dealer:
                        {
                            if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                            {
                                VehicleList = db.vwVehicleList
                                  .Where(o => o.DealerID == CurrentUser.Details.ReferenceID
                                  && o.Active == true
                                  && o.Assigned == true
                                  )
                                   .Select(o => new vwVehicleListModel()
                                   {
                                       VehicleID = o.VehicleID,
                                       DealerBranchName = o.DealerBranchName,
                                       VehicleMakeName = o.VehicleMakeName,
                                       VehicleModelName = o.VehicleModelName,
                                       Variant = o.Variant,
                                       Year = o.Year,
                                       EngineNumber = o.EngineNumber,
                                       ChassisNumber = o.ChassisNumber,
                                       BodyIDNumber = o.BodyIDNumber,
                                       CreatedDate = o.CreatedDate
                                   })
                                  .ToList();
                            }
                            else
                            {
                                VehicleList = db.vwVehicleList
                                  .Where(o => o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                  && o.DealerID == CurrentUser.Details.ReferenceID
                                  && o.Active == true
                                  && o.Assigned == true
                                  )
                                  .Select(o => new vwVehicleListModel()
                                  {
                                      VehicleID = o.VehicleID,
                                      DealerBranchName = o.DealerBranchName,
                                      VehicleMakeName = o.VehicleMakeName,
                                      VehicleModelName = o.VehicleModelName,
                                      Variant = o.Variant,
                                      Year = o.Year,
                                      EngineNumber = o.EngineNumber,
                                      ChassisNumber = o.ChassisNumber,
                                      BodyIDNumber = o.BodyIDNumber,
                                      CreatedDate = o.CreatedDate
                                  })
                                  .ToList();
                            }
                        }

                        break;
                    case (int)UserEntityEnum.MAI:
                        VehicleList = db.vwVehicleList
                      .Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                      && o.Active == true
                      )
                      .Select(o => new vwVehicleListModel()
                      {
                          VehicleID = o.VehicleID,
                          DealerBranchName = o.DealerBranchName,
                          VehicleMakeName = o.VehicleMakeName,
                          VehicleModelName = o.VehicleModelName,
                          Variant = o.Variant,
                          Year = o.Year,
                          EngineNumber = o.EngineNumber,
                          ChassisNumber = o.ChassisNumber,
                          BodyIDNumber = o.BodyIDNumber,
                          CreatedDate = o.CreatedDate
                      })
                      .ToList();
                        break;
                    case (int)UserEntityEnum.DataBridgeAsia:
                        VehicleList = db.vwVehicleList
                      .Where(o =>
                       o.Active == true
                      )
                      .Select(o => new vwVehicleListModel()
                      {
                          VehicleID = o.VehicleID,
                          DealerBranchName = o.DealerBranchName,
                          VehicleMakeName = o.VehicleMakeName,
                          VehicleModelName = o.VehicleModelName,
                          Variant = o.Variant,
                          Year = o.Year,
                          EngineNumber = o.EngineNumber,
                          ChassisNumber = o.ChassisNumber,
                          BodyIDNumber = o.BodyIDNumber,
                          CreatedDate = o.CreatedDate
                      })
                      .ToList();
                        break;
                }

                return View(VehicleList);
            }
        }

        #region [ DEALER ]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForCSR()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                VehicleListModel model = new VehicleListModel();
                switch (CurrentUser.Details.UserEntityID)
                {
                    case (int)UserEntityEnum.Dealer:

                        if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                        {
                            model.VehicleList = db.vwVehicleList
                                .Where(o => o.Active == true
                                && o.CertificateOfStockReport == null
                                && o.DealerID == CurrentUser.Details.ReferenceID)
                                .Select(o => new vwVehicleListModel
                                {
                                    VehicleID = o.VehicleID,
                                    VehicleMakeName = o.VehicleMakeName,
                                    VehicleModelName = o.VehicleModelName,
                                    Variant = o.Variant,
                                    Year = o.Year,
                                    EngineNumber = o.EngineNumber,
                                    ChassisNumber = o.ChassisNumber,
                                    BodyIDNumber = o.BodyIDNumber,
                                    isChecked = false
                                }).ToList();
                        }
                        else
                        {
                            model.VehicleList = db.vwVehicleList
                                .Where(o => o.Active == true
                                && o.CertificateOfStockReport == null
                                && o.DealerID == CurrentUser.Details.ReferenceID
                                && o.DealerBranchID == CurrentUser.Details.SubReferenceID)
                                .Select(o => new vwVehicleListModel
                                {
                                    VehicleID = o.VehicleID,
                                    VehicleMakeName = o.VehicleMakeName,
                                    VehicleModelName = o.VehicleModelName,
                                    Variant = o.Variant,
                                    Year = o.Year,
                                    EngineNumber = o.EngineNumber,
                                    ChassisNumber = o.ChassisNumber,
                                    BodyIDNumber = o.BodyIDNumber,
                                    isChecked = false
                                }).ToList();
                        }

                        break;
                    case (int)UserEntityEnum.MAI:
                        model.VehicleList = (from v in db.vwVehicleList
                                             join bd in db.BatchDetails
                                             on v.VehicleID equals bd.VehicleID
                                             join bm in db.BatchMaster
                                             on bd.BatchID equals bm.BatchID
                                             where bm.Received == true &&
                                             bm.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                                             v.DealerID == null &&
                                             v.CertificateOfStockReport == null
                                             select v)
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList();
                        break;
                }
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForCSR(VehicleListModel model, string submit)
        {
            using (db = new VRSystemEntities())
            {
                model.CSRInfo.BatchHeader.BatchTypeID = (int)BatchTypeList.CSR;
                switch (submit)
                {
                    case "CSR":
                        if (!CheckAvailableBalance(model.VehicleList.Where(o => o.isChecked == true).Count(), (UserEntityEnum)CurrentUser.Details.UserEntityID,
                            CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI ?
                            (int)CurrentUser.Details.SubReferenceID : (int)CurrentUser.Details.ReferenceID))
                        {
                            TempData["ErrorMessage"] = "Wallet balance is not enough";
                            break;
                        }
                        else if (db.BatchMaster.Where(o => o.ReferenceNo == model.CSRInfo.BatchHeader.ReferenceNo && o.BatchTypeID == model.CSRInfo.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                        {
                            //BatchMaster Validation
                            TempData["ErrorMessage"] = "Batch reference number is existed!";
                        }

                        //Batch Header temp data
                        TempData["BatchHeader"] = model.CSRInfo.BatchHeader;

                        List<int> VehicleIDList = new List<int>();
                        foreach (var item in model.VehicleList)
                        {
                            if (item.isChecked == true)
                            {
                                var validateCheck = db.vwVehicleList.Where(o => o.CertificateOfStockReport == null
                                    && o.Active == true
                                    && o.VehicleID == item.VehicleID
                                    && o.MAIID == CurrentUser.Details.SubReferenceID
                                    && o.BOCCertificateOfPayment != null
                                    && o.StencilOfEngine != null
                                    && o.StencilOfChasis != null
                                    ).FirstOrDefault();

                                if (validateCheck != null)
                                {
                                    VehicleIDList.Add(item.VehicleID);
                                }
                            }
                        }

                        if (VehicleIDList != null)
                        {
                            if (CSRSumbit(VehicleIDList))
                            {
                                TempData["SuccessMessage"] = "CSR Application is successful!";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "An error has occured!";
                            }
                        }
                        break;

                    case "DealerCSR":
                        model.CSRInfo.BatchHeader.BatchTypeID = (int)BatchTypeList.CSR;
                        //BatchMaster Validation
                        if (db.BatchMaster.Where(o => o.ReferenceNo == model.CSRInfo.BatchHeader.ReferenceNo && o.BatchTypeID == model.CSRInfo.BatchHeader.BatchTypeID).FirstOrDefault() == null)
                        {
                            BatchHeaderModel BatchHeader = (BatchHeaderModel)TempData["BatchHeader"];
                            //BatchList
                            List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                            foreach (var item in model.VehicleList)
                            {
                                if (item.isChecked == true)
                                {
                                    ModelState.Clear();
                                    if (TryValidateModel(model.CSRInfo))
                                    {

                                        var validateCheck = db.vwVehicleList.Where(o => o.CertificateOfStockReport == null
                                            && o.Active == true
                                            && o.VehicleID == item.VehicleID
                                            && o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                            //&& o.BOCCertificateOfPayment != null
                                            //&& o.StencilOfEngine != null
                                            //&& o.StencilOfChasis != null
                                            ).FirstOrDefault();

                                        if (validateCheck != null)
                                        {
                                            var UpdateCSR = db.VehicleInfo.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                                            UpdateCSR.TransactionID = model.CSRInfo.TransactionID;
                                            UpdateCSR.CSRNumber = model.CSRInfo.CSRNumber;

                                            UpdateCSR.ReportEntryID = model.CSRInfo.ReportEntryID;
                                            UpdateCSR.ReportDate = model.CSRInfo.ReportDate;
                                            UpdateCSR.ItemType = model.CSRInfo.ItemType;

                                            UpdateCSR.BIRCCMV = model.CSRInfo.BIRCCMV;
                                            UpdateCSR.DateIssued3 = model.CSRInfo.DateIssued3;

                                            UpdateCSR.TaxType = model.CSRInfo.TaxType;
                                            UpdateCSR.TaxAmount = model.CSRInfo.TaxAmount;

                                            if (model.CSRInfo.CSRFile.IsAllowedContentType())
                                            {
                                                UpdateCSR.CertificateOfStockReport = model.CSRInfo.CSRFile.ToByte();
                                                UpdateCSR.CSRContentType = model.CSRInfo.CSRFile.ContentType;
                                            }
                                            UpdateCSR.UpdatedBy = CurrentUser.Details.UserID;
                                            UpdateCSR.UpdatedDate = DateTime.Now;

                                            BatchDetailsList.Add(new BatchDetails()
                                            {
                                                VehicleID = UpdateCSR.VehicleID,
                                                TransactionID = null
                                            });

                                            TempData["SuccessMessage"] = "Upload successful!";
                                        }
                                    }
                                }
                            }

                            if (BatchDetailsList != null)
                            {
                                db.SaveChanges();
                                var batchID = BatchHeaderInsert(BatchHeader, BatchDetailsList.Count);
                                if (batchID != null)
                                {
                                    //Batch Details Insert
                                    BatchDetailsList.ForEach(o => o.BatchID = Convert.ToInt32(batchID));
                                    BatchDetailsInsert(BatchDetailsList);
                                }
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Batch reference number is existed!";
                        }
                        break;
                    case "Report":

                        string path = Server.MapPath("~/Reports/Excel/ForCSR-Report.xlsx");
                        var cnt_itm = 0;
                        DataTable vehicleinforeport = new DataTable();
                        vehicleinforeport.Columns.AddRange(new DataColumn[40] {
                                            new DataColumn("CONDUCTION STICKER NUMBER"),
                                            new DataColumn("ENGINE NUMBER"),
                                            new DataColumn("FUEL TYPE"),
                                            new DataColumn("CYLINDERS"),
                                            new DataColumn("PISTON DISPLACEMENT"),
                                            new DataColumn("BOC CP NUMBER"),
                                            new DataColumn("CP DATE"),
                                            new DataColumn("INFORMAL ENTRY NUMBER"),
                                            new DataColumn("CHASSIS NUMBER"),
                                            new DataColumn("BOC CP NUMBER 2"),
                                            new DataColumn("CP DATE 2"),
                                            new DataColumn("INFORMAL ENTRY NUMBER 2"),
                                            new DataColumn("BODY ID NUMBER"),
                                            new DataColumn("MAKE"),
                                            new DataColumn("SERIES"),
                                            new DataColumn("BODY TYPE"),
                                            new DataColumn("YEAR"),
                                            new DataColumn("COLOR"),
                                            new DataColumn("GVW"),
                                            new DataColumn("AIRCON_REF"),
                                            new DataColumn("TAX_TYPE"),
                                            new DataColumn("TAX_AMOUNT"),
                                            new DataColumn("TIRE_SIZE_FRONT"),
                                            new DataColumn("TIRE_SIZE_REAR"),
                                            new DataColumn("COC_NO"),
                                            new DataColumn("BIR CP NUMBER"),
                                            new DataColumn("CP DATE 3"),
                                            new DataColumn("INFORMAL ENTRY NUMBER 3"),
                                            new DataColumn("SALES TYPE"),
                                            new DataColumn("DEALER'S/OPERATOR'S  NAME (TRANSFER,TRANSFER TO)"),
                                            new DataColumn("ACCREDITATION NUMBER"),
                                            new DataColumn("INVOICE NUMBER"),
                                            new DataColumn("INVOICE DATE"),
                                            new DataColumn("DEALER'S/OPERATOR'S  NAME (TRANSFER FROM)"),
                                            new DataColumn("ACCREDITATION NUMBER 2"),
                                            new DataColumn("INVOICE NUMBER 2"),
                                            new DataColumn("INVOICE DATE 2"),
                                            new DataColumn("BUYER NO(S)."),
                                            new DataColumn("INVOICE NUMBER 3"),
                                            new DataColumn("INVOICE DATE 4")
                                            });
                        foreach (var item in model.VehicleList)
                        {
                            if (item.isChecked == true)
                            {
                                var vehiclelist = db.vwVehicleList.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                                cnt_itm++;
                                vehicleinforeport.Rows.Add(
                                    vehiclelist.ConductionSticker,
                                    vehiclelist.EngineNumber,
                                    vehiclelist.VehicleFuelTypeName,
                                    vehiclelist.Cylinders,
                                    vehiclelist.PistonDisplacement,
                                    vehiclelist.CPNumber,
                                    vehiclelist.DateIssued1,
                                    "",
                                    vehiclelist.ChassisNumber,
                                    vehiclelist.CPNumber2,
                                    vehiclelist.DateIssued2,
                                    "",
                                    vehiclelist.BodyIDNumber,
                                    vehiclelist.VehicleMakeName,
                                    vehiclelist.Series,
                                    vehiclelist.BodyIDNumber,
                                    "",
                                    vehiclelist.Year,
                                    vehiclelist.VehicleColorName,
                                    vehiclelist.GrossVehicleWeight,
                                    "",
                                    vehiclelist.TaxType,
                                    vehiclelist.TaxAmount,
                                    vehiclelist.FrontTiresNumber,
                                    vehiclelist.RearTiresNumber,
                                    vehiclelist.COCNo,
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    ""
                                    );
                            }
                        }

                        using (XLWorkbook wb = new XLWorkbook(path))
                        {
                            var ws = wb.Worksheet(1);
                            //BUSINESS NAME
                            ws.Cell(2, 3).Value = CurrentUser.Details.FirstName;
                            //BUSINESS ADDRESS
                            ws.Cell(3, 3).Value = "address";
                            //ACCREDITATION NO.
                            ws.Cell(4, 3).Value = "ACCREDITATION";
                            //REPORT TYPE
                            ws.Cell(12, 3).Value = "1";
                            //TOTAL ITEMS
                            ws.Cell(14, 3).Value = cnt_itm;
                            //ACCREDITATION NO.
                            ws.Cell(24, 1).InsertData(vehicleinforeport.Rows);

                            //wb.Save();
                            using (MemoryStream stream = new MemoryStream())
                            {
                                wb.SaveAs(stream);

                                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ForCSR-Report.xlsx");
                            }
                        }
                        break;
                }

                model = new VehicleListModel();
                switch (CurrentUser.Details.UserEntityID)
                {
                    case (int)UserEntityEnum.Dealer:
                        model.VehicleList = db.vwVehicleList.Where(o =>
                   o.CertificateOfStockReport == null
                   && o.Active == true
                   && o.DealerBranchID == CurrentUser.Details.SubReferenceID
                //&& o.BOCCertificateOfPayment != null
                //&& o.StencilOfEngine != null
                //&& o.StencilOfChasis != null
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList();
                        break;
                    case (int)UserEntityEnum.MAI:
                        model.VehicleList = db.vwVehicleList.Where(o =>
                   o.CertificateOfStockReport == null
                   && o.Active == true
                   && o.MAIID == CurrentUser.Details.SubReferenceID
                   && o.BOCCertificateOfPayment != null
                   && o.StencilOfEngine != null
                   && o.StencilOfChasis != null
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList();
                        break;
                }
            }
            return View(model);
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult ForInvoice()
        {
            using (db = new VRSystemEntities())
            {

                VehicleListModel model = new VehicleListModel();

                if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                {
                    var VehicleSoldIDNoFile = db.DealerInvoice.Where(o => o.InvoiceByte == null)
                                        .Select(o => o.VehicleID).ToList();
                    var VehicleSoldID = db.DealerInvoice.Where(o => o.InvoiceByte != null)
                                                   .Select(o => o.VehicleID).ToList();

                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o => o.CertificateOfStockReport != null
                                          && (VehicleSoldIDNoFile.Contains(o.VehicleID) || !VehicleSoldID.Contains(o.VehicleID))
                                          && o.DealerID == (int)CurrentUser.Details.ReferenceID
                                          && o.Active == true)
                        .Distinct()
                        .Select(o => new vwVehicleListModel
                        {
                            VehicleID = o.VehicleID,
                            VehicleMakeName = o.VehicleMakeName,
                            VehicleModelName = o.VehicleModelName,
                            Variant = o.Variant,
                            Year = o.Year,
                            EngineNumber = o.EngineNumber,
                            ChassisNumber = o.ChassisNumber,
                            BodyIDNumber = o.BodyIDNumber,
                            isChecked = false
                        }
                        ).ToList()
                    };
                }
                else
                {
                    var VehicleSoldIDNoFile = db.DealerInvoice.Where(o => o.InvoiceByte == null && o.DealerBranchID == CurrentUser.Details.SubReferenceID)
                                        .Select(o => o.VehicleID).ToList();
                    var VehicleSoldID = db.DealerInvoice.Where(o => o.InvoiceByte != null && o.DealerBranchID == CurrentUser.Details.SubReferenceID)
                                                   .Select(o => o.VehicleID).ToList();

                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o => o.CertificateOfStockReport != null
                                          && (VehicleSoldIDNoFile.Contains(o.VehicleID) || !VehicleSoldID.Contains(o.VehicleID))
                                          && o.DealerID == (int)CurrentUser.Details.ReferenceID
                                          && o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                          && o.Active == true)
                        .Distinct()
                        .Select(o => new vwVehicleListModel
                        {
                            VehicleID = o.VehicleID,
                            VehicleMakeName = o.VehicleMakeName,
                            VehicleModelName = o.VehicleModelName,
                            Variant = o.Variant,
                            Year = o.Year,
                            EngineNumber = o.EngineNumber,
                            ChassisNumber = o.ChassisNumber,
                            BodyIDNumber = o.BodyIDNumber,
                            isChecked = false
                        }
                        ).ToList()
                    };
                }
                return View(model);
            }
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        //[AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        public ActionResult ForCOP()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

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

                VehicleListModel model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o =>
                   o.CertificateOfConformity == null
                   &&
                   o.Active == true
                   &&
                   InvoiceVehicleIDList.Contains(o.VehicleID)
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                return View(model);
            }
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult AllocatedVehicle()
        {
            using (db = new VRSystemEntities())
            {
                List<vwVehicleListModel> VehicleList = new List<vwVehicleListModel>();

                switch (CurrentUser.Details.UserEntityID)
                {
                    case (int)UserEntityEnum.Dealer:
                        {
                            if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                            {
                                VehicleList = db.vwVehicleList
                                  .Where(o => o.DealerID == CurrentUser.Details.ReferenceID
                                  && o.Assigned == false
                                  && o.Active == true
                                  )
                                   .Select(o => new vwVehicleListModel()
                                   {
                                       VehicleID = o.VehicleID,
                                       DealerBranchName = o.DealerBranchName,
                                       VehicleMakeName = o.VehicleMakeName,
                                       VehicleModelName = o.VehicleModelName,
                                       Variant = o.Variant,
                                       Year = o.Year,
                                       EngineNumber = o.EngineNumber,
                                       ChassisNumber = o.ChassisNumber,
                                       BodyIDNumber = o.BodyIDNumber,
                                       CreatedDate = o.CreatedDate
                                   })
                                  .ToList();
                            }
                            else
                            {
                                VehicleList = db.vwVehicleList
                                  .Where(o => o.DealerBranchID == CurrentUser.Details.SubReferenceID
                                  && o.DealerID == CurrentUser.Details.ReferenceID
                                  && o.Assigned == false
                                  && o.Active == true
                                  )
                                   .Select(o => new vwVehicleListModel()
                                   {
                                       VehicleID = o.VehicleID,
                                       DealerBranchName = o.DealerBranchName,
                                       VehicleMakeName = o.VehicleMakeName,
                                       VehicleModelName = o.VehicleModelName,
                                       Variant = o.Variant,
                                       Year = o.Year,
                                       EngineNumber = o.EngineNumber,
                                       ChassisNumber = o.ChassisNumber,
                                       BodyIDNumber = o.BodyIDNumber,
                                       CreatedDate = o.CreatedDate
                                   })
                                  .ToList();
                            }
                        }

                        break;
                }

                return View(VehicleList);
            }
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForPNP()
        {
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel();

                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                {
                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o =>
                                          o.CertificateOfStockReport != null &&
                                          (o.PNPClearance == null && o.AutoPNP == false) &&
                                          o.Assigned == false && 
                                          o.Active == true
                    )
                        .Select(o => new vwVehicleListModel
                        {
                            VehicleID = o.VehicleID,
                            VehicleMakeName = o.VehicleMakeName,
                            VehicleModelName = o.VehicleModelName,
                            Variant = o.Variant,
                            Year = o.Year,
                            EngineNumber = o.EngineNumber,
                            ChassisNumber = o.ChassisNumber,
                            BodyIDNumber = o.BodyIDNumber,
                            isChecked = false
                        }
                        ).ToList()
                    };
                }
                else
                {

                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        model.VehicleList = db.vwVehicleList
                            .Where(o => o.CertificateOfStockReport != null
                            && o.CertificateOfConformity != null
                            && (o.PNPClearance == null && o.AutoPNP == false)
                            && o.DealerID == (int)CurrentUser.Details.ReferenceID
                            && o.Active == true)
                            .Select(o => new vwVehicleListModel
                            {
                                VehicleID = o.VehicleID,
                                VehicleMakeName = o.VehicleMakeName,
                                VehicleModelName = o.VehicleModelName,
                                Variant = o.Variant,
                                Year = o.Year,
                                EngineNumber = o.EngineNumber,
                                ChassisNumber = o.ChassisNumber,
                                BodyIDNumber = o.BodyIDNumber,
                                isChecked = false
                            }).ToList();
                    }
                    else
                    {
                        model.VehicleList = db.vwVehicleList
                            .Where(o => o.CertificateOfStockReport != null
                            && o.CertificateOfConformity != null
                            && (o.PNPClearance == null && o.AutoPNP == false)
                            && o.DealerID == (int)CurrentUser.Details.ReferenceID
                            && o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                            && o.Active == true)
                            .Select(o => new vwVehicleListModel
                            {
                                VehicleID = o.VehicleID,
                                VehicleMakeName = o.VehicleMakeName,
                                VehicleModelName = o.VehicleModelName,
                                Variant = o.Variant,
                                Year = o.Year,
                                EngineNumber = o.EngineNumber,
                                ChassisNumber = o.ChassisNumber,
                                BodyIDNumber = o.BodyIDNumber,
                                isChecked = false
                            }).ToList();
                    }
                }

                return View(model);
            }
        }
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult UploadForPNP()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult UploadForPNP(VehicleListModel vehicle)
        {
            ModelState.Remove("PNPInfo.PNPFile");
            ModelState.Remove("PNPInfo.HPGControlNumber");
            ModelState.Remove("PNPInfo.PNPReceiptReferenceNumber");
            ModelState.Remove("PNPInfo.PNPReceiptFile");
            if (ModelState.IsValid)
            {
                if (vehicle.PNPInfo.PNPFileBatch != null && vehicle.PNPInfo.PNPFileBatch.Length > 0)
                {
                    using (db = new VRSystemEntities())
                    {
                        foreach (var image in vehicle.PNPInfo.PNPFileBatch)
                        {
                            if (image.IsAllowedContentType())
                            {
                                List<VehicleInfo> UpdateList = new List<VehicleInfo>();
                                //int VehicleID = 0;
                                //int.TryParse(System.IO.Path.GetFileNameWithoutExtension(image.FileName), out VehicleID);
                                string HPGControlNumber = System.IO.Path.GetFileNameWithoutExtension(image.FileName);
                                UpdateList = db.VehicleInfo.Where(o =>
                                     //o.BOCCertificateOfPayment != null
                                     //&&
                                     //o.StencilOfEngine != null
                                     //&&
                                     //o.StencilOfChasis != null
                                     //&&
                                     o.CertificateOfStockReport != null
                                     &&
                                     o.CertificateOfConformity != null
                                     &&
                                     o.PNPClearance == null
                                     &&
                                     o.DealerID == (int)CurrentUser.Details.ReferenceID
                                     &&
                                     o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                     &&
                                     o.HPGControlNumber == HPGControlNumber
                                     &&
                                     o.Active == true
                                     ).ToList();


                                if (UpdateList != null)
                                {
                                    foreach(var Update in UpdateList)
                                    {
                                        Update.PNPClearance = image.ToByte();
                                        Update.PNPContentType = image.ContentType;
                                        Update.UpdatedBy = CurrentUser.Details.UserID;
                                        Update.UpdatedDate = DateTime.Now;
                                        db.SaveChanges();

                                        vehicle.PNPInfo.PNPListModel.Add(new PNPListModel
                                        {
                                            PNPListFile = image,
                                            isSuccess = true
                                        });
                                    }
                                }
                                else
                                {
                                    vehicle.PNPInfo.PNPListModel.Add(new PNPListModel
                                    {
                                        PNPListFile = image,
                                        message = "HPG Number doesn't exist",
                                        isSuccess = false
                                    });
                                }
                            }
                            else
                            {
                                vehicle.PNPInfo.PNPListModel.Add(new PNPListModel
                                {
                                    PNPListFile = image,
                                    message = "File extension is not valid",
                                    isSuccess = false
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please upload your image file!";
            }
            return View(vehicle);
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult ForLTO()
        {
            ViewBag.HasCheckBox = true;
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel();

                //model = new VehicleListModel
                //{
                //    VehicleList = db.vwVehicleList.Where(o =>
                //                      //o.BOCCertificateOfPayment != null
                //                      //&&
                //                      //o.StencilOfChasis != null
                //                      //&&
                //                      //o.StencilOfEngine != null
                //                      //&&
                //                      o.CertificateOfStockReport != null
                //                      &&
                //                      o.CertificateOfConformity != null
                //                      &&
                //                      o.PNPClearance != null
                //                      &&
                //                      o.LTOSubmitted == false
                //                      &&
                //                      o.DealerID == (int)CurrentUser.Details.ReferenceID
                //                      &&
                //                      o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                //                      &&
                //                      o.Active == true
                //)
                //    .Select(o => new vwVehicleListModel
                //    {
                //        VehicleID = o.VehicleID,
                //        VehicleMakeName = o.VehicleMakeName,
                //        VehicleModelName = o.VehicleModelName,
                //        Variant = o.Variant,
                //        YearOfMake = o.YearOfMake,
                //        EngineNumber = o.EngineNumber,
                //        BodyIDNumber = o.BodyIDNumber,
                //        isChecked = false
                //    }
                //    ).ToList()
                //};

                //model.BatchFilter.BatchFilterList = db.BatchMaster
                //    .Where(o =>
                //        o.BatchTypeID == (int)BatchTypeList.BOC &&
                //        o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                //        o.UserReference == CurrentUser.Details.ReferenceID &&
                //        o.UserSubRef == CurrentUser.Details.SubReferenceID)
                //    .Select(o => new BatchFilterList { BatchID = o.BatchID.ToString(), ReferenceNo = o.ReferenceNo }).ToList();

                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult ForLTO(VehicleListModel model, string submit)
        {
            ViewBag.HasCheckBox = true;
            using (db = new VRSystemEntities())
            {
                if (model.VehicleList.Where(o => o.isChecked == true).Count() > 25)
                {
                    TempData["WarningMessage"] = "25 Vehicles are allowed per batch only!";
                }
                else
                {
                    switch (submit.ToLower())
                    {
                        case "lto":

                            if (ModelState.IsValid)
                            {
                                if (!CheckAvailableBalance(model.VehicleList.Where(o => o.isChecked == true).Count(), (UserEntityEnum)CurrentUser.Details.UserEntityID, (int)CurrentUser.Details.ReferenceID))
                                {
                                    TempData["ErrorMessage"] = "Wallet balance is not enough";
                                    return View(model);
                                }

                                List<int> VehicleIDList = new List<int>();
                                foreach (var item in model.VehicleList.Where(o => o.isChecked == true))
                                {
                                    if (item.isChecked == true)
                                    {

                                        VehicleIDList.Add(item.VehicleID);
                                        //var Update = db.VehicleInfo.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                                        //Update.LTOSubmitted = true;
                                        //Update.UpdatedBy = CurrentUser.Details.UserID;
                                        //Update.UpdatedDate = DateTime.Now;
                                        //db.SaveChanges();
                                        //TempData["SuccessMessage"] = "Vehicle Information has been submitted!";
                                    }
                                }

                                if (VehicleIDList != null)
                                {
                                    TempData["BatchHeader"] = model.BatchHeader;
                                    if (LTOSubmit(VehicleIDList))
                                    {
                                        TempData["SuccessMessage"] = "LTO Application is successful!";
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "An error has occured!";
                                    }
                                }
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "There's something error!";
                            }
                            break;
                    }
                }
                model = new VehicleListModel();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetVehicleListByBuyerType(string buyertype)
        {
            ViewBag.HasCheckBox = true;
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel();
                switch (buyertype)
                {
                    case "IND":

                        if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                        {
                            model.VehicleList = (from a in db.vwVehicleList
                                                 join b in db.DealerInvoice on a.VehicleID equals b.VehicleID
                                                 join c in db.vwCustomerList on b.CustomerID equals c.CustomerID into temp
                                                 from temptbl in temp.DefaultIfEmpty()
                                                 where
                                                    a.CertificateOfStockReport != null &&
                                                    a.CertificateOfConformity != null &&
                                                    b.InvoiceByte != null &&
                                                    (a.PNPClearance != null || a.AutoPNP == true) &&
                                                    a.LTOSubmitted == false &&
                                                    a.DealerID == (int)CurrentUser.Details.ReferenceID &&
                                                    a.Active == true &&
                                                    temptbl.TitleTypeID == 1
                                                 select new
                                                 {
                                                     a.CertificateOfStockReport,
                                                     a.CertificateOfConformity,
                                                     a.PNPClearance,
                                                     a.LTOSubmitted,
                                                     a.DealerID,
                                                     a.DealerBranchID,
                                                     a.Active,
                                                     a.VehicleID,
                                                     a.VehicleMakeName,
                                                     a.VehicleModelName,
                                                     a.Variant,
                                                     a.Year,
                                                     a.EngineNumber,
                                                     a.ChassisNumber,
                                                     a.BodyIDNumber,
                                                     temptbl.TitleTypeID
                                                 })
                                            .Select(o => new vwVehicleListModel
                                            {
                                                VehicleID = o.VehicleID,
                                                VehicleMakeName = o.VehicleMakeName,
                                                VehicleModelName = o.VehicleModelName,
                                                Variant = o.Variant,
                                                Year = o.Year,
                                                EngineNumber = o.EngineNumber,
                                                ChassisNumber = o.ChassisNumber,
                                                BodyIDNumber = o.BodyIDNumber,
                                                isChecked = false
                                            }).ToList();
                        }
                        else
                        {
                            model.VehicleList = (from a in db.vwVehicleList
                                                 join b in db.DealerInvoice on a.VehicleID equals b.VehicleID
                                                 join c in db.vwCustomerList on b.CustomerID equals c.CustomerID into temp
                                                 from temptbl in temp.DefaultIfEmpty()
                                                 where
                                                    a.CertificateOfStockReport != null &&
                                                    a.CertificateOfConformity != null &&
                                                    b.InvoiceByte != null &&
                                                    (a.PNPClearance != null || a.AutoPNP == true) &&
                                                    a.LTOSubmitted == false &&
                                                    a.DealerID == (int)CurrentUser.Details.ReferenceID &&
                                                    a.DealerBranchID == (int)CurrentUser.Details.SubReferenceID &&
                                                    a.Active == true &&
                                                    temptbl.TitleTypeID == 1
                                                 select new
                                                 {
                                                     a.CertificateOfStockReport,
                                                     a.CertificateOfConformity,
                                                     a.PNPClearance,
                                                     a.LTOSubmitted,
                                                     a.DealerID,
                                                     a.DealerBranchID,
                                                     a.Active,
                                                     a.VehicleID,
                                                     a.VehicleMakeName,
                                                     a.VehicleModelName,
                                                     a.Variant,
                                                     a.Year,
                                                     a.EngineNumber,
                                                     a.ChassisNumber,
                                                     a.BodyIDNumber,
                                                     temptbl.TitleTypeID
                                                 })
                                            .Select(o => new vwVehicleListModel
                                            {
                                                VehicleID = o.VehicleID,
                                                VehicleMakeName = o.VehicleMakeName,
                                                VehicleModelName = o.VehicleModelName,
                                                Variant = o.Variant,
                                                Year = o.Year,
                                                EngineNumber = o.EngineNumber,
                                                ChassisNumber = o.ChassisNumber,
                                                BodyIDNumber = o.BodyIDNumber,
                                                isChecked = false
                                            }).ToList();
                        }
                        break;
                    case "ORG":

                        if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                        {
                            model.VehicleList = (from a in db.vwVehicleList
                                                 join b in db.DealerInvoice on a.VehicleID equals b.VehicleID
                                                 join c in db.vwCustomerList on b.CustomerID equals c.CustomerID into temp
                                                 from temptbl in temp.DefaultIfEmpty()
                                                 where
                                                    a.CertificateOfStockReport != null &&
                                                    b.InvoiceByte != null &&
                                                    a.CertificateOfConformity != null &&
                                                    (a.PNPClearance != null || a.AutoPNP == true) &&
                                                    a.LTOSubmitted == false &&
                                                    a.DealerID == (int)CurrentUser.Details.ReferenceID &&
                                                    a.Active == true &&
                                                    temptbl.TitleTypeID == 2
                                                 select new
                                                 {
                                                     a.CertificateOfStockReport,
                                                     a.CertificateOfConformity,
                                                     a.PNPClearance,
                                                     a.LTOSubmitted,
                                                     a.DealerID,
                                                     a.DealerBranchID,
                                                     a.Active,
                                                     a.VehicleID,
                                                     a.VehicleMakeName,
                                                     a.VehicleModelName,
                                                     a.Variant,
                                                     a.Year,
                                                     a.EngineNumber,
                                                     a.ChassisNumber,
                                                     a.BodyIDNumber,
                                                     temptbl.TitleTypeID
                                                 })
                                            .Select(o => new vwVehicleListModel
                                            {
                                                VehicleID = o.VehicleID,
                                                VehicleMakeName = o.VehicleMakeName,
                                                VehicleModelName = o.VehicleModelName,
                                                Variant = o.Variant,
                                                Year = o.Year,
                                                EngineNumber = o.EngineNumber,
                                                ChassisNumber = o.ChassisNumber,
                                                BodyIDNumber = o.BodyIDNumber,
                                                isChecked = false
                                            }).ToList();
                        }
                        else
                        {
                            model.VehicleList = (from a in db.vwVehicleList
                                                 join b in db.DealerInvoice on a.VehicleID equals b.VehicleID
                                                 join c in db.vwCustomerList on b.CustomerID equals c.CustomerID into temp
                                                 from temptbl in temp.DefaultIfEmpty()
                                                 where
                                                    a.CertificateOfStockReport != null &&
                                                    b.InvoiceByte != null &&
                                                    a.CertificateOfConformity != null &&
                                                    (a.PNPClearance != null || a.AutoPNP == true) &&
                                                    a.LTOSubmitted == false &&
                                                    a.DealerID == (int)CurrentUser.Details.ReferenceID &&
                                                    a.DealerBranchID == (int)CurrentUser.Details.SubReferenceID &&
                                                    a.Active == true &&
                                                    temptbl.TitleTypeID == 2
                                                 select new
                                                 {
                                                     a.CertificateOfStockReport,
                                                     a.CertificateOfConformity,
                                                     a.PNPClearance,
                                                     a.LTOSubmitted,
                                                     a.DealerID,
                                                     a.DealerBranchID,
                                                     a.Active,
                                                     a.VehicleID,
                                                     a.VehicleMakeName,
                                                     a.VehicleModelName,
                                                     a.Variant,
                                                     a.Year,
                                                     a.EngineNumber,
                                                     a.ChassisNumber,
                                                     a.BodyIDNumber,
                                                     temptbl.TitleTypeID
                                                 })
                                            .Select(o => new vwVehicleListModel
                                            {
                                                VehicleID = o.VehicleID,
                                                VehicleMakeName = o.VehicleMakeName,
                                                VehicleModelName = o.VehicleModelName,
                                                Variant = o.Variant,
                                                Year = o.Year,
                                                EngineNumber = o.EngineNumber,
                                                ChassisNumber = o.ChassisNumber,
                                                BodyIDNumber = o.BodyIDNumber,
                                                isChecked = false
                                            }).ToList();
                        }
                        break;
                }
                return PartialView("_VehicleList", model);
            }
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult ForLTO_Submitted()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOSubmittedBatch();

                var DealerList = new List<LTODealerFilter>();

                DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                {
                    DealerID = o.DealerID,
                    DealerName = o.DealerName
                }).OrderBy(o => o.DealerName).ToList());

                model.DealerList = DealerList;

                if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                {
                    ViewBag.IsMain = true;
                    ViewBag.IsAdmin = true;
                    model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                       && o.UserReference == CurrentUser.Details.ReferenceID
                                                       && o.DateSubmitted != null
                                                       && o.Assessed == false
                                                       && o.BatchTypeID == (int)BatchTypeList.LTO
                                                       && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                      EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                      SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                                                      ReferenceNo = o.ReferenceNo,
                                                      BatchDescription = o.BatchDescription,
                                                      DateSubmitted = o.DateSubmitted,
                                                      BatchCount = o.BatchCount
                                                  })
                                                  .OrderByDescending(o => o.DateSubmitted)
                                                  .ToList();
                }
                else
                {
                    model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                      SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                                                      ReferenceNo = o.ReferenceNo,
                                                      BatchDescription = o.BatchDescription,
                                                      DateSubmitted = o.DateSubmitted,
                                                      BatchCount = o.BatchCount
                                                  })
                                                  .OrderByDescending(o => o.DateSubmitted)
                                                  .ToList();
                }

                model.VehicleList = new List<LTOBatchDetailVehicle>();

                return View(model);
            }
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer,UserEntityEnum.MAI })]
        public ActionResult ForLTO_Payment()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOPayment();

                if ((int)UserEntityEnum.MAI == CurrentUser.Details.UserEntityID)
                {
                    ViewBag.EntityType = "MAI";
                    var MAIList = new List<MAIModel>();

                    MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                    {
                        MAIID = o.MAIID,
                        MAIName = o.MAIName
                    }).OrderBy(o => o.MAIName).ToList());

                    model.MAIList = MAIList;

                    model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Assessed == true
                                                        && o.PaymentRef == null
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                      EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                                                      ReferenceNo = o.ReferenceNo,
                                                      BatchDescription = o.BatchDescription,
                                                      DateSubmitted = o.AssessedDate,
                                                      BatchCount = o.BatchCount,
                                                      Assessed = o.Assessed,
                                                      AssessedAmount = o.AssessedAmount,
                                                      PaymentRef = o.PaymentRef,
                                                      PaymentImageContentType = o.PaymentFileType,
                                                      EPatImageContentType = o.PaymentEPATFileType
                                                  })
                                                  .OrderByDescending(o => o.DateSubmitted)
                                                  .ToList();

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }
                else
                {
                    var DealerList = new List<LTODealerFilter>();

                    DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).OrderBy(o => o.DealerName).ToList());

                    model.DealerList = DealerList;

                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        ViewBag.IsMain = true;
                        ViewBag.IsAdmin = true;
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                            && o.UserReference == CurrentUser.Details.ReferenceID
                                                            && o.BatchTypeID == (int)BatchTypeList.LTO
                                                            && o.Assessed == true
                                                            && o.PaymentRef == null
                                                            && o.Active == true)
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.AssessedDate,
                                                          BatchCount = o.BatchCount,
                                                          Assessed = o.Assessed,
                                                          AssessedAmount = o.AssessedAmount,
                                                          PaymentRef = o.PaymentRef,
                                                          PaymentImageContentType = o.PaymentFileType,
                                                          EPatImageContentType = o.PaymentEPATFileType
                                                      })
                                                      .OrderByDescending(o => o.DateSubmitted)
                                                      .ToList();
                    }
                    else
                    {
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.AssessedDate,
                                                          BatchCount = o.BatchCount,
                                                          Assessed = o.Assessed,
                                                          AssessedAmount = o.AssessedAmount,
                                                          PaymentRef = o.PaymentRef,
                                                          PaymentImageContentType = o.PaymentFileType,
                                                          EPatImageContentType = o.PaymentEPATFileType
                                                      })
                                                      .OrderByDescending(o => o.DateSubmitted)
                                                      .ToList();
                    }

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();

                    //LTO Region and District
                    model.LTOList = db.LTO.Where(o => o.Active == true).ToList();
                    model.LTOID = Convert.ToInt32(ConfigurationManager.AppSettings["DIYLTOID"].ToString());
                    model.LTOBranchList = db.LTOBranch.Where(o => o.Active == true && o.LTOID == model.LTOID).ToList();
                    model.LTOBranchID = Convert.ToInt32(ConfigurationManager.AppSettings["DIYLTOBranchID"].ToString());
                }
                return View(model);
            }
        }

        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer,UserEntityEnum.MAI })]
        public ActionResult ForLTO_Payment(LTOPayment model)
        {
            using (db = new VRSystemEntities())
            {
                model.SelectedDealerID = 0;
                //model.SelectedBatchID = 0;
                if (ModelState.IsValid)
                {
                    if ((model.PaymentFile.IsAllowedContentType() && model.PaymentFile.IsValidFileSize()) && (model.EPATFile.IsAllowedContentType() && model.EPATFile.IsValidFileSize()))
                    {
                        var Update = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                        Update.PaymentRef = model.PaymentRef;
                        Update.PaymentFileByte = model.PaymentFile.ToByte();
                        Update.PaymentFileType = model.PaymentFile.ContentType;
                        Update.PaymentEPATFileByte = model.EPATFile.ToByte();
                        Update.PaymentEPATFileType = model.EPATFile.ContentType;
                        Update.PaymentBy = CurrentUser.Details.UserID;
                        Update.PaymentDate = DateTime.Now;
                        Update.LTOBranchID = model.LTOBranchID;
                        db.SaveChanges();
                        Functions.EmailPlateLTO(Update);
                        Functions.LTOEmailStatus(model.SelectedBatchID, LTOStatus.Paid);
                        if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                        {

                            TempData["SuccessMessage"] = "CSR successfuly submitted!";

                        }
                        if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                        {

                            TempData["SuccessMessage"] = "OR/CR successfuly submitted!";
                        }
                    }
                    else if (model.PaymentFile == null || model.EPATFile == null)
                    {
                        TempData["WarningMessage"] = "Please upload Payment and E-PAT file.";
                    }
                    else if (!model.PaymentFile.IsAllowedContentType() || !model.EPATFile.IsAllowedContentType())
                    {
                        TempData["WarningMessage"] = "Only .JPG, .JPEG, .PNG & PDF file type's are allowed.";
                    }
                    else if (!model.PaymentFile.IsValidFileSize() || !model.EPATFile.IsValidFileSize())
                    {
                        TempData["WarningMessage"] = "Please upload valid file size of less than 1 MB.";
                    }

                }
                else
                {
                    TempData["ErrorMessage"] = "An error has occured!";
                }
            }

            return RedirectToAction("ForLTO_Payment");
            //using (db = new VRSystemEntities())
            //{
            //    model = new LTOPayment();

            //    var DealerList = new List<LTODealerFilter>();

            //    DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
            //    {
            //        DealerID = o.DealerID,
            //        DealerName = o.DealerName
            //    }).OrderBy(o => o.DealerName).ToList());

            //    model.DealerList = DealerList;
            //    model.SelectedDealerID = 0;
            //    model.SelectedBatchID = 0;
            //    model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
            //                                        && o.UserReference == CurrentUser.Details.ReferenceID
            //                                        && o.BatchTypeID == (int)BatchTypeList.LTO
            //                                        && o.Assessed == true
            //                                        && o.PaymentRef == null
            //                                        && o.Active == true)
            //                                  .Select(o => new LTOBatchHeader
            //                                  {
            //                                      BatchID = o.BatchID,
            //                                      EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
            //                                      ReferenceNo = o.ReferenceNo,
            //                                      BatchDescription = o.BatchDescription,
            //                                      DateSubmitted = o.AssessedDate,
            //                                      BatchCount = o.BatchCount,
            //                                      Assessed = o.Assessed,
            //                                      AssessedAmount = o.AssessedAmount,
            //                                      PaymentRef = o.PaymentRef,
            //                                      PaymentImageContentType = o.PaymentFileType,
            //                                      EPatImageContentType = o.PaymentEPATFileType
            //                                  })
            //                                  .OrderByDescending(o => o.DateSubmitted)
            //                                  .ToList();

            //    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();



            //    return View(model);
            //}
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForLTO_Processed()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOPayment();

                if ((int)UserEntityEnum.MAI == CurrentUser.Details.UserEntityID)
                {
                    ViewBag.EntityType = "MAI";
                    var MAIList = new List<MAIModel>();

                    MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                    {
                        MAIID = o.MAIID,
                        MAIName = o.MAIName
                    }).OrderBy(o => o.MAIName).ToList());

                    model.MAIList = MAIList;

                    model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Assessed == true
                                                        && o.PaymentRef != null
                                                        && o.Completed == false
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                      EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
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

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }
                else
                {
                    var DealerList = new List<LTODealerFilter>();

                    DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).OrderBy(o => o.DealerName).ToList());

                    model.DealerList = DealerList;

                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        ViewBag.IsMain = true;
                        ViewBag.IsAdmin = true;
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
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
                    }
                    else
                    {
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
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
                    }

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }
                return View(model);
            }
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForLTO_Completed()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOPayment();

                if ((int)UserEntityEnum.MAI == CurrentUser.Details.UserEntityID)
                {
                    ViewBag.EntityType = "MAI";
                    var MAIList = new List<MAIModel>();

                    MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                    {
                        MAIID = o.MAIID,
                        MAIName = o.MAIName
                    }).OrderBy(o => o.MAIName).ToList());

                    model.MAIList = MAIList;

                    model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.UserSubRef == CurrentUser.Details.SubReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Assessed == true
                                                        && o.PaymentRef != null
                                                        && o.Completed == true
                                                        && o.ForPickUp == false
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                      EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
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

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }
                else
                {
                    var DealerList = new List<LTODealerFilter>();

                    DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).OrderBy(o => o.DealerName).ToList());

                    model.DealerList = DealerList;

                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        ViewBag.IsMain = true;
                        ViewBag.IsAdmin = true;
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
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
                    }
                    else
                    {
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
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
                    }

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }
                return View(model);
            }
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForLTO_ForPickUp()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOPayment();

                if ((int)UserEntityEnum.MAI == CurrentUser.Details.UserEntityID)
                {
                    ViewBag.EntityType = "MAI";
                    var MAIList = new List<MAIModel>();

                    //DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                    //{
                    //    DealerID = o.DealerID,
                    //    DealerName = o.DealerName
                    //}).OrderBy(o => o.DealerName).ToList());

                    model.MAIList = MAIList;

                    model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Completed == true
                                                        && o.ForPickUp == true
                                                        && o.Received == false
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                      EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
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

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }
                else
                {
                    var DealerList = new List<LTODealerFilter>();

                    //DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                    //{
                    //    DealerID = o.DealerID,
                    //    DealerName = o.DealerName
                    //}).OrderBy(o => o.DealerName).ToList());

                    model.DealerList = DealerList;

                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        ViewBag.IsMain = true;
                        ViewBag.IsAdmin = true;
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
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
                    }
                    else
                    {
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
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
                    }

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }

                return View(model);
            }
        }

        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForLTO_ForPickUp(LTOPayment model)
        {
            using (db = new VRSystemEntities())
            {
                model.SelectedDealerID = 0;

                if (model.SelectedBatchID != 0)
                {
                    var Update = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                    Update.Received = true;
                    Update.ReceivedBy = CurrentUser.Details.UserID;
                    Update.ReceivedDate = DateTime.Now;
                    db.SaveChanges();
                    if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                    {
                        TempData["SuccessMessage"] = "CSR received!";

                    }
                    if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                    {

                        TempData["SuccessMessage"] = "OR/CR received!";
                    }
                    
                }
                else
                {
                    TempData["ErrorMessage"] = "An error has occured!";
                }

                return RedirectToAction("ForLTO_ForPickUp");
            }
        }
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForLTO_Received()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOPayment();
                if ((int)UserEntityEnum.MAI == CurrentUser.Details.UserEntityID)
                {
                    ViewBag.EntityType = "MAI";
                    var MAIList = new List<MAIModel>();

                    //DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                    //{
                    //    DealerID = o.DealerID,
                    //    DealerName = o.DealerName
                    //}).OrderBy(o => o.DealerName).ToList());

                    model.MAIList = MAIList;
                    
                    model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                        && o.UserReference == CurrentUser.Details.ReferenceID
                                                        && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                        && o.Completed == true
                                                        && o.ForPickUp == true
                                                        && o.Received == true
                                                        && o.Active == true)
                                                  .Select(o => new LTOBatchHeader
                                                  {
                                                      BatchID = o.BatchID,
                                                      EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                                                      ReferenceNo = o.ReferenceNo,
                                                      BatchDescription = o.BatchDescription,
                                                      DateSubmitted = o.ReceivedDate,
                                                      BatchCount = o.BatchCount,
                                                      Assessed = o.Assessed,
                                                      AssessedAmount = o.AssessedAmount,
                                                      PaymentRef = o.PaymentRef,
                                                      PaymentImageContentType = o.PaymentFileType,
                                                      EPatImageContentType = o.PaymentEPATFileType
                                                  })
                                                  .OrderByDescending(o => o.DateSubmitted)
                                                  .ToList();

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }
                else
                {
                    var DealerList = new List<LTODealerFilter>();

                    model.DealerList = DealerList;

                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        ViewBag.IsMain = true;
                        ViewBag.IsAdmin = true;
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.ReceivedDate,
                                                          BatchCount = o.BatchCount,
                                                          Assessed = o.Assessed,
                                                          AssessedAmount = o.AssessedAmount,
                                                          PaymentRef = o.PaymentRef,
                                                          PaymentImageContentType = o.PaymentFileType,
                                                          EPatImageContentType = o.PaymentEPATFileType
                                                      })
                                                      .OrderByDescending(o => o.DateSubmitted)
                                                      .ToList();
                    }
                    else
                    {
                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
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
                                                          SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.ReceivedDate,
                                                          BatchCount = o.BatchCount,
                                                          Assessed = o.Assessed,
                                                          AssessedAmount = o.AssessedAmount,
                                                          PaymentRef = o.PaymentRef,
                                                          PaymentImageContentType = o.PaymentFileType,
                                                          EPatImageContentType = o.PaymentEPATFileType
                                                      })
                                                      .OrderByDescending(o => o.DateSubmitted)
                                                      .ToList();
                    }

                    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();
                }
                return View(model);
            }
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForLTO_Rejected()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOSubmittedBatch();

                if ((int)UserEntityEnum.MAI == CurrentUser.Details.UserEntityID)
                {
                    ViewBag.EntityType = "MAI";
                    var MAIList = new List<MAIModel>();

                    MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                    {
                        MAIID = o.MAIID,
                        MAIName = o.MAIName
                    }).OrderBy(o => o.MAIName).ToList());

                    model.MAIList = MAIList;

                    var RejectBatchID = (from detail in db.BatchDetails
                                         join master in db.BatchMaster
                                         on detail.BatchID equals master.BatchID
                                         where (detail.Rejected == true && detail.Completed == false) && master.Completed == false
                                         && master.EntityTypeID == (int)UserEntityEnum.MAI
                                         && master.UserReference == CurrentUser.Details.ReferenceID
                                         && master.BatchTypeID == (int)BatchTypeList.LTOCSR
                                         select master.BatchID).ToList();


                    model.BatchList = db.BatchMaster.Where(o => RejectBatchID.Distinct().Contains(o.BatchID))
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.DateSubmitted,
                            BatchCount = RejectBatchID.Count(),
                            Remarks = o.RejectedRemarks
                        })
                        .ToList();

                    model.VehicleList = new List<LTOBatchDetailVehicle>();
                }
                else
                {
                    var DealerList = new List<LTODealerFilter>();

                    DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).OrderBy(o => o.DealerName).ToList());

                    model.DealerList = DealerList;



                    if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                    {
                        ViewBag.IsMain = true;
                        ViewBag.IsAdmin = true;
                        var RejectBatchID = (from detail in db.BatchDetails
                                             join master in db.BatchMaster
                                             on detail.BatchID equals master.BatchID
                                             where (detail.Rejected == true && detail.Completed == false) && master.Completed == false
                                             && master.EntityTypeID == (int)UserEntityEnum.Dealer
                                             && master.UserReference == CurrentUser.Details.ReferenceID
                                             && master.BatchTypeID == (int)BatchTypeList.LTO
                                             select master.BatchID).ToList();

                        model.BatchList = db.BatchMaster.Where(o => RejectBatchID.Distinct().Contains(o.BatchID))
                            .Select(o => new LTOBatchHeader
                            {
                                BatchID = o.BatchID,
                                EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                                ReferenceNo = o.ReferenceNo,
                                BatchDescription = o.BatchDescription,
                                DateSubmitted = o.DateSubmitted,
                                BatchCount = RejectBatchID.Count(),
                                Remarks = o.RejectedRemarks
                            })
                            .ToList();
                    }
                    else
                    {
                        var RejectBatchID = (from detail in db.BatchDetails
                                             join master in db.BatchMaster
                                             on detail.BatchID equals master.BatchID
                                             where (detail.Rejected == true && detail.Completed == false) && master.Completed == false
                                             && master.EntityTypeID == (int)UserEntityEnum.Dealer
                                             && master.UserReference == CurrentUser.Details.ReferenceID
                                             && master.UserSubRef == CurrentUser.Details.SubReferenceID
                                             && master.BatchTypeID == (int)BatchTypeList.LTO
                                             select master.BatchID).ToList();

                        model.BatchList = db.BatchMaster.Where(o => RejectBatchID.Distinct().Contains(o.BatchID))
                            .Select(o => new LTOBatchHeader
                            {
                                BatchID = o.BatchID,
                                EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                                ReferenceNo = o.ReferenceNo,
                                BatchDescription = o.BatchDescription,
                                DateSubmitted = o.DateSubmitted,
                                BatchCount = RejectBatchID.Count(),
                                Remarks = o.RejectedRemarks
                            })
                            .ToList();
                    }

                    model.VehicleList = new List<LTOBatchDetailVehicle>();
                }
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer, UserEntityEnum.MAI })]
        public ActionResult ForLTO_Rejected(LTOSubmittedBatch model, string submit)
        {
            using (db = new VRSystemEntities())
            {
                var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                switch (submit)
                {
                    case "Reprocess":
                        //Rejected update
                        UpdateHeader.Rejected = false;
                        UpdateHeader.RejectedBy = null;
                        UpdateHeader.RejectedDate = null;
                        UpdateHeader.RejectedRemarks = null;
                        //Reprocess
                        UpdateHeader.Reprocessed = true;
                        UpdateHeader.ReprocessedBy = CurrentUser.Details.UserID;
                        UpdateHeader.ReprocessedDate = DateTime.Now;
                        UpdateHeader.ReprocessedRemarks = model.ReprocessRemarks?.Trim();
                        db.SaveChanges();

                        TempData["InfoMessage"] = "Reprocessed Successfully";
                        break;
                    case "Cancel":

                        UpdateHeader.Cancelled = true;
                        UpdateHeader.CancelledBy = CurrentUser.Details.UserID;
                        UpdateHeader.CancelledDate = DateTime.Now;
                        UpdateHeader.CancelledRemarks = model.CancelledRemarks?.Trim();

                        foreach (var list in model.VehicleList)
                        {
                            var UpdateDetails = db.VehicleInfo.Where(o => o.VehicleID == list.VehicleID).First();
                            UpdateDetails.LTOSubmitted = false;
                            UpdateDetails.UpdatedBy = CurrentUser.Details.UserID;
                            UpdateDetails.UpdatedDate = DateTime.Now;
                        }

                        db.SaveChanges();
                        TempData["InfoMessage"] = "Cancelled Successfully";
                        break;
                    default:
                        TempData["ErrorMessage"] = "An error has occured.";
                        break;
                }
            }
            return RedirectToAction("ForLTO_Rejected");
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult ForLTO_DIY_Rejected()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOSubmittedBatch();

                ViewBag.DIYRejected = true;
                var DealerList = new List<LTODealerFilter>();

                DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                {
                    DealerID = o.DealerID,
                    DealerName = o.DealerName
                }).OrderBy(o => o.DealerName).ToList());

                model.DealerList = DealerList;

                if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                {
                    ViewBag.IsMain = true;
                    ViewBag.IsAdmin = true;
                    var RejectBatchID = (from detail in db.BatchDetails
                                         join master in db.BatchMaster
                                         on detail.BatchID equals master.BatchID
                                         where master.Active == true
                                         && master.DateSubmitted == null
                                         && master.Assessed == false
                                         && master.Rejected == true
                                         && master.EntityTypeID == (int)UserEntityEnum.Dealer
                                         && master.UserReference == CurrentUser.Details.ReferenceID
                                         && master.BatchTypeID == (int)BatchTypeList.LTO
                                         select master.BatchID).ToList();

                    model.BatchList = db.BatchMaster.Where(o => RejectBatchID.Distinct().Contains(o.BatchID))
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                            SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.DateSubmitted,
                            BatchCount = RejectBatchID.Count(),
                            Remarks = o.RejectedRemarks
                        })
                        .ToList();
                }
                else
                {
                    var RejectBatchID = (from detail in db.BatchDetails
                                         join master in db.BatchMaster
                                         on detail.BatchID equals master.BatchID
                                         where master.Active == true
                                         && master.DateSubmitted == null
                                         && master.Assessed == false
                                         && master.Rejected == true
                                         && master.EntityTypeID == (int)UserEntityEnum.Dealer
                                         && master.UserReference == CurrentUser.Details.ReferenceID
                                         && master.UserSubRef == CurrentUser.Details.SubReferenceID
                                         && master.BatchTypeID == (int)BatchTypeList.LTO
                                         select master.BatchID).ToList();

                    model.BatchList = db.BatchMaster.Where(o => RejectBatchID.Distinct().Contains(o.BatchID))
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                            SubEntityName = db.DealerBranch.Where(p => p.DealerID == o.UserReference && p.DealerBranchID == o.UserSubRef).FirstOrDefault().DealerBranchName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.DateSubmitted,
                            BatchCount = RejectBatchID.Count(),
                            Remarks = o.RejectedRemarks
                        })
                        .ToList();
                }

                model.VehicleList = new List<LTOBatchDetailVehicle>();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult ForLTO_DIY_Rejected(LTOSubmittedBatch model, string submit)
        {
            using (db = new VRSystemEntities())
            {
                var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                switch (submit)
                {
                    case "ReSubmit":
                        //Rejected update
                        UpdateHeader.Rejected = false;
                        UpdateHeader.RejectedBy = null;
                        UpdateHeader.RejectedDate = null;
                        UpdateHeader.RejectedRemarks = null;
                        //ReSubmit
                        UpdateHeader.DateSubmitted = DateTime.Now;
                        db.SaveChanges();

                        TempData["InfoMessage"] = "Re-Submit Successfully";
                        break;
                }
            }
            return RedirectToAction("ForLTO_DIY_Rejected");
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        [HttpGet]
        public ActionResult ForCOC()
        {
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel();

                if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                {
                    model.VehicleList = (from a in db.DealerInvoice
                                         join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                         from temptbl in temp.DefaultIfEmpty()
                                         select new
                                         {
                                             temptbl,
                                         })
                                         .Where(o => o.temptbl.CertificateOfStockReport != null
                                         && o.temptbl.CertificateOfConformity == null
                                         && o.temptbl.DealerID == (int)CurrentUser.Details.ReferenceID
                                         && o.temptbl.Active == true)
                                         .Select(o => new vwVehicleListModel
                                         {
                                             VehicleID = o.temptbl.VehicleID,
                                             VehicleMakeName = o.temptbl.VehicleMakeName,
                                             VehicleModelName = o.temptbl.VehicleModelName,
                                             Variant = o.temptbl.Variant,
                                             Year = o.temptbl.Year,
                                             EngineNumber = o.temptbl.EngineNumber,
                                             ChassisNumber = o.temptbl.ChassisNumber,
                                             BodyIDNumber = o.temptbl.BodyIDNumber,
                                             isChecked = false
                                         }).ToList();
                }
                else
                {
                    model.VehicleList = (from a in db.DealerInvoice
                                         join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                         from temptbl in temp.DefaultIfEmpty()
                                         select new
                                         {
                                             temptbl,
                                         })
                                         .Where(o => o.temptbl.CertificateOfStockReport != null
                                         && o.temptbl.CertificateOfConformity == null
                                         && o.temptbl.DealerID == (int)CurrentUser.Details.ReferenceID
                                         && o.temptbl.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                         && o.temptbl.Active == true)
                                         .Select(o => new vwVehicleListModel
                                         {
                                             VehicleID = o.temptbl.VehicleID,
                                             VehicleMakeName = o.temptbl.VehicleMakeName,
                                             VehicleModelName = o.temptbl.VehicleModelName,
                                             Variant = o.temptbl.Variant,
                                             Year = o.temptbl.Year,
                                             EngineNumber = o.temptbl.EngineNumber,
                                             ChassisNumber = o.temptbl.ChassisNumber,
                                             BodyIDNumber = o.temptbl.BodyIDNumber,
                                             isChecked = false
                                         }).ToList();
                }

                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult ForCOC(VehicleListModel model)
        {
            if (ModelState.IsValid)
            {
                if (!CheckAvailableBalance(model.VehicleList.Where(o => o.isChecked == true).Count(), UserEntityEnum.Insurance, model.COCInfo.DealerInsuranceID))
                {
                    TempData["ErrorMessage"] = "Insurance balance is not enough";
                    return View(model);
                }

                foreach (var item in model.VehicleList)
                {
                    if (item.isChecked == true)
                    {
                        if (GetCOCManual(model.COCInfo, item.VehicleID, model.COCInfo.DealerInsuranceID))
                        {
                            TempData["SuccessMessage"] = "COC Registration successful!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "An error has occured.";
                        }
                        //if (GetCOC(item.VehicleID, model.COCInfo.DealerInsuranceID))
                        //{
                        //    TempData["SuccessMessage"] = "COC Registration successful!";
                        //}
                        //else
                        //{
                        //    TempData["ErrorMessage"] = "An error has occured.";
                        //}
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "There's something error!";
            }
            using (db = new VRSystemEntities())
            {
                model = new VehicleListModel
                {
                    VehicleList = (from a in db.DealerInvoice
                                   join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                   from temptbl in temp.DefaultIfEmpty()
                                   select new
                                   {
                                       temptbl,
                                   }).Where(o =>
                                   o.temptbl.CertificateOfStockReport != null
                                   &&
                                   o.temptbl.CertificateOfConformity == null
                                   &&
                                   o.temptbl.DealerID == (int)CurrentUser.Details.ReferenceID
                                   &&
                                   o.temptbl.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                   &&
                                   o.temptbl.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.temptbl.VehicleID,
                        VehicleMakeName = o.temptbl.VehicleMakeName,
                        VehicleModelName = o.temptbl.VehicleModelName,
                        Variant = o.temptbl.Variant,
                        Year = o.temptbl.Year,
                        EngineNumber = o.temptbl.EngineNumber,
                        ChassisNumber = o.temptbl.ChassisNumber,
                        BodyIDNumber = o.temptbl.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                model.COCInfo.DealerInsuranceList = (from a in db.DealerInsurance
                                                     where a.Active == true
                                                     join b in db.Insurance on a.InsuranceID equals b.InsuranceID into temp
                                                     from temptbl in temp.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         DealerID = a.DealerID,
                                                         InsuranceID = temptbl.InsuranceID,
                                                         InsuranceName = temptbl.InsuranceName,
                                                         AutoGenerated = temptbl.AutoGenerateCOC
                                                     }).Where(o => o.DealerID == (int)CurrentUser.Details.ReferenceID).Select(
                        o => new vwDealerInsuranceModel()
                        {
                            DealerID = o.DealerID,
                            InsuranceID = o.InsuranceID,
                            InsuranceName = o.InsuranceName,
                            AutoGenerateCOC = o.AutoGenerated
                        }).ToList();
            }
            return View(model);
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult UploadForCSR()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult UploadForCSR(VehicleListModel vehicle)
        {
            if (ModelState.IsValid)
            {
                if (vehicle.CSRBatchInfo.CSRFileArray != null && vehicle.CSRBatchInfo.CSRFileArray.Length > 0)
                {
                    using (db = new VRSystemEntities())
                    {
                        foreach (var image in vehicle.CSRBatchInfo.CSRFileArray)
                        {
                            if (image.IsAllowedContentType())
                            {
                                VehicleInfo Update = new VehicleInfo();
                                var newimagename = System.IO.Path.GetFileNameWithoutExtension(image.FileName);
                                if ((int)UserEntityEnum.MAI == (int)CurrentUser.Details.UserEntityID)
                                {
                                    Update = db.VehicleInfo.Where(o => (o.DealerBranchID == null
                                         || o.DealerID == null)
                                         &&
                                         o.BOCCertificateOfPayment != null
                                         &&
                                         o.CertificateOfStockReport == null
                                         &&
                                         o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                         &&
                                         o.Active == true
                                         &&
                                         o.CSRNumber == newimagename
                                         ).FirstOrDefault();
                                }
                                else if ((int)UserEntityEnum.Dealer == (int)CurrentUser.Details.UserEntityID)
                                {
                                    Update = db.VehicleInfo.Where(o =>
                                         o.CertificateOfStockReport == null
                                         &&
                                         o.DealerID == (int)CurrentUser.Details.ReferenceID
                                         &&
                                         o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                         &&
                                         o.Active == true
                                         &&
                                         o.CSRNumber == newimagename
                                         ).FirstOrDefault();
                                }


                                if (Update != null)
                                {
                                    Update.CertificateOfStockReport = image.ToByte();
                                    Update.CSRContentType = image.ContentType;
                                    Update.UpdatedBy = CurrentUser.Details.UserID;
                                    Update.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();

                                    vehicle.CSRBatchInfo.CSRListModel.Add(new CSRListModel
                                    {
                                        CSRListFile = image,
                                        isSuccess = true
                                    });
                                }
                                else
                                {
                                    vehicle.CSRBatchInfo.CSRListModel.Add(new CSRListModel
                                    {
                                        CSRListFile = image,
                                        message = "CSR number doesn't exist",
                                        isSuccess = false
                                    });
                                }
                            }
                            else
                            {
                                vehicle.CSRBatchInfo.CSRListModel.Add(new CSRListModel
                                {
                                    CSRListFile = image,
                                    message = "File extension is not valid!",
                                    isSuccess = false
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please upload your image file!";
            }
            return View(vehicle);
        }


        #endregion

        #region [ MAI ]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        [HttpGet]
        public ActionResult ForBOC()
        {
            ViewBag.HasCheckBox = true;
            using (db = new VRSystemEntities())
            {
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                {
                    VehicleListModel model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o => (o.DealerBranchID == null
                                          || o.DealerID == null
                                          )
                                          &&
                                          o.BOCCertificateOfPayment == null
                                          &&
                                          o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                          &&
                                          o.Active == true
               )
                   .Select(o => new vwVehicleListModel
                   {
                       VehicleID = o.VehicleID,
                       VehicleMakeName = o.VehicleMakeName,
                       VehicleModelName = o.VehicleModelName,
                       Variant = o.Variant,
                       Year = o.Year,
                       EngineNumber = o.EngineNumber,
                       ChassisNumber = o.ChassisNumber,
                       BodyIDNumber = o.BodyIDNumber,
                       isChecked = false
                   }).ToList()
                    };

                    model.BatchFilter.BatchFilterList = db.BatchMaster
                        .Where(o =>
                            o.BatchTypeID == (int)BatchTypeList.NewUpload &&
                            o.EntityTypeID == (int)UserEntityEnum.MAI &&
                            o.UserReference == CurrentUser.Details.ReferenceID &&
                            o.UserSubRef == CurrentUser.Details.SubReferenceID)
                        .Select(o => new BatchFilterList { BatchID = o.BatchID.ToString(), ReferenceNo = o.ReferenceNo }).ToList();
                    return View(model);
                }
                else
                {
                    VehicleListModel model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o =>
                                          o.BOCCertificateOfPayment == null
                                          &&
                                          o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                          &&
                                          o.DealerID == (int)CurrentUser.Details.ReferenceID
                                          &&
                                          o.Active == true
               )
                   .Select(o => new vwVehicleListModel
                   {
                       VehicleID = o.VehicleID,
                       VehicleMakeName = o.VehicleMakeName,
                       VehicleModelName = o.VehicleModelName,
                       Variant = o.Variant,
                       Year = o.Year,
                       EngineNumber = o.EngineNumber,
                       ChassisNumber = o.ChassisNumber,
                       BodyIDNumber = o.BodyIDNumber,
                       isChecked = false
                   }
                   ).ToList()
                    };

                    model.BatchFilter.BatchFilterList = db.BatchMaster
                        .Where(o =>
                            o.BatchTypeID == (int)BatchTypeList.NewUpload &&
                            o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                            o.UserReference == CurrentUser.Details.ReferenceID &&
                            o.UserSubRef == CurrentUser.Details.SubReferenceID)
                        .Select(o => new BatchFilterList { BatchID = o.BatchID.ToString(), ReferenceNo = o.ReferenceNo }).ToList();
                    return View(model);
                }

                //model.VehicleInfo = new VehicleInfoModel();


            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        public ActionResult ForBOC(VehicleListModel model)
        {
            ViewBag.HasCheckBox = true;
            model.BOCInfo.BatchHeader.BatchTypeID = (int)BatchTypeList.BOC;
            using (db = new VRSystemEntities())
            {
                //BatchList
                List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                //BatchMaster Validation
                if (db.BatchMaster.Where(o => o.ReferenceNo == model.BOCInfo.BatchHeader.ReferenceNo && o.BatchTypeID == model.BOCInfo.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                {
                    TempData["ErrorMessage"] = "Batch reference number is existed!";
                }
                else if (ModelState.IsValid)
                {
                    List<VehicleInfo> UpdateList = new List<VehicleInfo>();
                    foreach (var item in model.VehicleList)
                    {
                        if (item.isChecked == true)
                        {
                            VehicleInfo Update = null;
                            switch (CurrentUser.Details.UserEntityID)
                            {
                                case (int)UserEntityEnum.Dealer:
                                    {
                                        Update = db.VehicleInfo.Where(o =>
                                                         o.BOCCertificateOfPayment == null
                                                         &&
                                                         o.VehicleID == item.VehicleID
                                                         &&
                                                         o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                                         &&
                                                         o.DealerID == (int)CurrentUser.Details.ReferenceID
                                                         &&
                                                         o.Active == true).FirstOrDefault();
                                        break;
                                    }
                                case (int)UserEntityEnum.MAI:
                                    {
                                        Update = db.VehicleInfo.Where(o => (o.DealerBranchID == null
                                         || o.DealerID == null
                                         )
                                         &&
                                         o.BOCCertificateOfPayment == null
                                         &&
                                         o.VehicleID == item.VehicleID
                                         &&
                                         o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                         &&
                                         o.Active == true).FirstOrDefault();
                                        break;
                                    }
                            }

                            if (Update != null)
                            {
                                Update.CPNumber = model.BOCInfo.CPNumber.Trim();
                                Update.DateIssued1 = model.BOCInfo.DateIssued1;
                                Update.BOCCertificateOfPayment = model.BOCInfo.BOCFile.ToByte();
                                Update.BOCContentType = model.BOCInfo.BOCFile != null ? model.BOCInfo.BOCFile.ContentType : null;
                                Update.CPNumber2 = model.BOCInfo.CPNumber2.Trim();
                                Update.DateIssued2 = model.BOCInfo.DateIssued2;
                                Update.BOCCertificateOfPayment2 = model.BOCInfo.BOC2File.ToByte();
                                Update.BOCContentType2 = model.BOCInfo.BOC2File != null ? model.BOCInfo.BOC2File.ContentType : null;
                                Update.UpdatedBy = CurrentUser.Details.UserID;
                                Update.UpdatedDate = DateTime.Now;
                                Update.InformalEntryNumberEngine = model.BOCInfo.InformalEntryNumberEngine.Trim();
                                Update.InformalEntryNumberChassis = model.BOCInfo.InformalEntryNumberChassis.Trim();
                                UpdateList.Add(Update);

                                //Batch add to list
                                BatchDetailsList.Add(new BatchDetails
                                {
                                    VehicleID = Update.VehicleID,
                                    TransactionID = null
                                });
                            }
                        }
                    }

                    if (UpdateList.Count > 0)
                    {
                        db.SaveChanges();
                        //Batch Header Insert
                        var batchID = BatchHeaderInsert(model.BOCInfo.BatchHeader, BatchDetailsList.Count);
                        if (batchID != null)
                        {
                            //Batch Details Insert
                            BatchDetailsList.ForEach(o => o.BatchID = Convert.ToInt32(batchID));
                            BatchDetailsInsert(BatchDetailsList);
                        }
                        TempData["SuccessMessage"] = "BOC Payment Information has been assign!";
                    }

                    //BatchMaster(model.BOCInfo.BatchHeader, model.VehicleList, null);
                }
                else
                {
                    TempData["ErrorMessage"] = "There's something error!";
                }

                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                {
                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o => (o.DealerBranchID == null
                                          || o.DealerID == null
                                          )
                                          &&
                                          o.BOCCertificateOfPayment == null
                                          &&
                                          o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                          &&
                                          o.Active == true
               )
                   .Select(o => new vwVehicleListModel
                   {
                       VehicleID = o.VehicleID,
                       VehicleMakeName = o.VehicleMakeName,
                       VehicleModelName = o.VehicleModelName,
                       Variant = o.Variant,
                       Year = o.Year,
                       EngineNumber = o.EngineNumber,
                       ChassisNumber = o.ChassisNumber,
                       BodyIDNumber = o.BodyIDNumber,
                       isChecked = false
                   }).ToList()
                    };

                    model.BatchFilter.BatchFilterList = db.BatchMaster
                        .Where(o =>
                            o.BatchTypeID == (int)BatchTypeList.NewUpload &&
                            o.EntityTypeID == (int)UserEntityEnum.MAI &&
                            o.UserReference == CurrentUser.Details.ReferenceID &&
                            o.UserSubRef == CurrentUser.Details.SubReferenceID)
                        .Select(o => new BatchFilterList { BatchID = o.BatchID.ToString(), ReferenceNo = o.ReferenceNo }).ToList();
                }
                else
                {
                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o =>
                                          o.BOCCertificateOfPayment == null
                                          &&
                                          o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                          &&
                                          o.DealerID == (int)CurrentUser.Details.ReferenceID
                                          &&
                                          o.Active == true
               )
                   .Select(o => new vwVehicleListModel
                   {
                       VehicleID = o.VehicleID,
                       VehicleMakeName = o.VehicleMakeName,
                       VehicleModelName = o.VehicleModelName,
                       Variant = o.Variant,
                       Year = o.Year,
                       EngineNumber = o.EngineNumber,
                       ChassisNumber = o.ChassisNumber,
                       BodyIDNumber = o.BodyIDNumber,
                       isChecked = false
                   }
                   ).ToList()
                    };

                    model.BatchFilter.BatchFilterList = db.BatchMaster
                        .Where(o =>
                            o.BatchTypeID == (int)BatchTypeList.NewUpload &&
                            o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                            o.UserReference == CurrentUser.Details.ReferenceID &&
                            o.UserSubRef == CurrentUser.Details.SubReferenceID)
                        .Select(o => new BatchFilterList { BatchID = o.BatchID.ToString(), ReferenceNo = o.ReferenceNo }).ToList();
                }
            }
            return View(model);
        }


        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        public ActionResult ForSOE()
        {
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel();
                if ((int)UserEntityEnum.MAI == (int)CurrentUser.Details.UserEntityID)
                {
                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o => (o.DealerBranchID == null
                                          || o.DealerID == null
                                          )
                                          &&
                                          o.BOCCertificateOfPayment != null
                                          &&
                                          o.StencilOfEngine == null
                                          &&
                                          o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                          &&
                                          o.Active == true
                    )
                        .Select(o => new vwVehicleListModel
                        {
                            VehicleID = o.VehicleID,
                            VehicleMakeName = o.VehicleMakeName,
                            VehicleModelName = o.VehicleModelName,
                            Variant = o.Variant,
                            Year = o.Year,
                            EngineNumber = o.EngineNumber,
                            ChassisNumber = o.ChassisNumber,
                            BodyIDNumber = o.BodyIDNumber,
                            isChecked = false
                        }
                        ).ToList()
                    };
                }
                else if ((int)UserEntityEnum.Dealer == (int)CurrentUser.Details.UserEntityID)
                {
                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o =>
                                          o.BOCCertificateOfPayment != null
                                          &&
                                          o.StencilOfEngine == null
                                          &&
                                          o.DealerID == (int)CurrentUser.Details.ReferenceID
                                          &&
                                          o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                          &&
                                          o.Active == true
                    )
                        .Select(o => new vwVehicleListModel
                        {
                            VehicleID = o.VehicleID,
                            VehicleMakeName = o.VehicleMakeName,
                            VehicleModelName = o.VehicleModelName,
                            Variant = o.Variant,
                            Year = o.Year,
                            EngineNumber = o.EngineNumber,
                            ChassisNumber = o.ChassisNumber,
                            BodyIDNumber = o.BodyIDNumber,
                            isChecked = false
                        }
                        ).ToList()
                    };
                }

                //model.VehicleInfo = new VehicleInfoModel();

                return View(model);
            }
        }
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        public ActionResult UploadForSOE()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        public ActionResult UploadForSOE(VehicleListModel vehicle)
        {
            if (ModelState.IsValid)
            {
                if (vehicle.SOEInfo.SOEFile != null && vehicle.SOEInfo.SOEFile.Length > 0)
                {
                    using (db = new VRSystemEntities())
                    {
                        foreach (var image in vehicle.SOEInfo.SOEFile)
                        {
                            if (image.IsAllowedContentType())
                            {
                                VehicleInfo Update = new VehicleInfo();
                                var newimagename = System.IO.Path.GetFileNameWithoutExtension(image.FileName);
                                if ((int)UserEntityEnum.MAI == (int)CurrentUser.Details.UserEntityID)
                                {
                                    Update = db.VehicleInfo.Where(o => (o.DealerBranchID == null
                                         || o.DealerID == null)
                                         &&
                                         o.BOCCertificateOfPayment != null
                                         &&
                                         o.StencilOfEngine == null
                                         &&
                                         o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                         &&
                                         o.Active == true
                                         &&
                                         o.EngineNumber == newimagename
                                         ).FirstOrDefault();
                                }
                                else if ((int)UserEntityEnum.Dealer == (int)CurrentUser.Details.UserEntityID)
                                {
                                    Update = db.VehicleInfo.Where(o =>
                                         o.BOCCertificateOfPayment != null
                                         &&
                                         o.StencilOfEngine == null
                                         &&
                                         o.DealerID == (int)CurrentUser.Details.ReferenceID
                                         &&
                                         o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                         &&
                                         o.Active == true
                                         &&
                                         o.EngineNumber == newimagename
                                         ).FirstOrDefault();
                                }


                                if (Update != null)
                                {
                                    Update.StencilOfEngine = image.ToByte();
                                    Update.SOEContentType = image.ContentType;
                                    Update.UpdatedBy = CurrentUser.Details.UserID;
                                    Update.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();

                                    vehicle.SOEInfo.SOEListModel.Add(new SOEListModel
                                    {
                                        SOEListFile = image,
                                        isSuccess = true
                                    });
                                }
                                else
                                {
                                    vehicle.SOEInfo.SOEListModel.Add(new SOEListModel
                                    {
                                        SOEListFile = image,
                                        message = "Engine number doesn't exist",
                                        isSuccess = false
                                    });
                                }
                            }
                            else
                            {
                                vehicle.SOEInfo.SOEListModel.Add(new SOEListModel
                                {
                                    SOEListFile = image,
                                    message = "File extension is not valid!",
                                    isSuccess = false
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please upload your image file!";
            }
            return View(vehicle);
        }


        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        public ActionResult ForSOC()
        {
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel();
                if ((int)UserEntityEnum.MAI == (int)CurrentUser.Details.UserEntityID)
                {
                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o => (o.DealerBranchID == null
                                          || o.DealerID == null)
                                          && o.BOCCertificateOfPayment != null
                                          && o.StencilOfEngine != null
                                          && o.StencilOfChasis == null
                                          && o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                          && o.Active == true
                    )
                        .Select(o => new vwVehicleListModel
                        {
                            VehicleID = o.VehicleID,
                            VehicleMakeName = o.VehicleMakeName,
                            VehicleModelName = o.VehicleModelName,
                            Variant = o.Variant,
                            Year = o.Year,
                            EngineNumber = o.EngineNumber,
                            ChassisNumber = o.ChassisNumber,
                            BodyIDNumber = o.BodyIDNumber,
                            isChecked = false
                        }
                        ).ToList()
                    };
                }
                else if ((int)UserEntityEnum.Dealer == (int)CurrentUser.Details.UserEntityID)
                {
                    model = new VehicleListModel
                    {
                        VehicleList = db.vwVehicleList.Where(o =>
                                          o.BOCCertificateOfPayment != null
                                          &&
                                          o.StencilOfChasis == null
                                          &&
                                          o.DealerID == (int)CurrentUser.Details.ReferenceID
                                          &&
                                          o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                          &&
                                          o.Active == true
                    )
                        .Select(o => new vwVehicleListModel
                        {
                            VehicleID = o.VehicleID,
                            VehicleMakeName = o.VehicleMakeName,
                            VehicleModelName = o.VehicleModelName,
                            Variant = o.Variant,
                            Year = o.Year,
                            EngineNumber = o.EngineNumber,
                            ChassisNumber = o.ChassisNumber,
                            BodyIDNumber = o.BodyIDNumber,
                            isChecked = false
                        }
                        ).ToList()
                    };
                }

                //model.VehicleInfo = new VehicleInfoModel();

                return View(model);
            }
        }
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        public ActionResult UploadForSOC()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI, UserEntityEnum.Dealer })]
        public ActionResult UploadForSOC(VehicleListModel vehicle)
        {
            if (ModelState.IsValid)
            {
                if (vehicle.SOCInfo.SOCFile != null && vehicle.SOCInfo.SOCFile.Length > 0)
                {
                    using (db = new VRSystemEntities())
                    {
                        foreach (var image in vehicle.SOCInfo.SOCFile)
                        {
                            if (image.IsAllowedContentType())
                            {
                                VehicleInfo Update = new VehicleInfo();
                                var newimagename = System.IO.Path.GetFileNameWithoutExtension(image.FileName);
                                if ((int)UserEntityEnum.MAI == (int)CurrentUser.Details.UserEntityID)
                                {
                                    Update = db.VehicleInfo.Where(o => (o.DealerBranchID == null
                                         || o.DealerID == null)
                                         && o.BOCCertificateOfPayment != null
                                         && o.StencilOfEngine != null
                                         && o.StencilOfChasis == null
                                         && o.MAIID == (int)CurrentUser.Details.SubReferenceID
                                         && o.Active == true
                                         && o.ChassisNumber == newimagename
                                         ).FirstOrDefault();
                                }
                                else if ((int)UserEntityEnum.Dealer == (int)CurrentUser.Details.UserEntityID)
                                {
                                    Update = db.VehicleInfo.Where(o =>
                                         o.BOCCertificateOfPayment != null
                                         &&
                                         o.StencilOfChasis == null
                                         &&
                                         o.DealerID == (int)CurrentUser.Details.ReferenceID
                                         &&
                                         o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                         &&
                                         o.Active == true
                                         &&
                                         o.ChassisNumber == newimagename
                                         ).FirstOrDefault();
                                }


                                if (Update != null)
                                {
                                    Update.StencilOfChasis = image.ToByte();
                                    Update.SOCContentType = image.ContentType;
                                    Update.UpdatedBy = CurrentUser.Details.UserID;
                                    Update.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();

                                    vehicle.SOCInfo.SOCListModel.Add(new SOCListModel
                                    {
                                        SOCListFile = image,
                                        isSuccess = true
                                    });
                                }
                                else
                                {
                                    vehicle.SOCInfo.SOCListModel.Add(new SOCListModel
                                    {
                                        SOCListFile = image,
                                        message = "Chassis number doesn't exist",
                                        isSuccess = false
                                    });
                                }
                            }
                            else
                            {
                                vehicle.SOCInfo.SOCListModel.Add(new SOCListModel
                                {
                                    SOCListFile = image,
                                    message = "File extension is not valid!",
                                    isSuccess = false
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please upload your image file!";
            }
            return View(vehicle);
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        [HttpGet]
        public ActionResult ForDealers()
        {
            using (db = new VRSystemEntities())
            {
                ViewBag.HasCheckBox = true;
                VehicleListModel model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      //&& (o.DealerBranchID == null || o.DealerID == null)
                                      && o.Assigned == false
                                      && o.BOCCertificateOfPayment != null
                                      && o.CertificateOfStockReport != null
                                      && o.StencilOfEngine != null
                                      && o.StencilOfChasis != null
                                      && o.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                var query = from a in db.MAIDealer
                            join b in db.Dealer on a.DealerID equals b.DealerID into temp
                            from temptbl in temp.DefaultIfEmpty()
                            select new
                            {
                                MAIID = a.MAIID,
                                MAIDealerID = a.MAIDealerID,
                                DealerID = temptbl.DealerID,
                                DealerName = temptbl.DealerName,
                                Active = a.Active
                            };
                model.DealerInfo.MAI_DealerList = query.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        MAIDealerID = o.MAIDealerID,
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                model.DealerInfo.MAI_DealerBranchList = new List<DealerBranchModel>();
                return View(model);
            }
        }
        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        public ActionResult ForDealers(VehicleListModel model)
        {
            using (db = new VRSystemEntities())
            {
                ViewBag.HasCheckBox = true;
                if (ModelState.IsValid)
                {
                    foreach (var item in model.VehicleList)
                    {
                        if (item.isChecked == true)
                        {
                            if (AssigningDealers(item.VehicleID, model.DealerInfo.SelectedDealer, model.DealerInfo.SelectedDealerBranch))
                            {
                                TempData["SuccessMessage"] = "Dealer has been assign!";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "An error has occured!";
                            }
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "There's something error!";
                }

                model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      //&& (o.DealerBranchID == null || o.DealerID == null)
                                      && o.Assigned == false
                                      && o.BOCCertificateOfPayment != null
                                      && o.CertificateOfStockReport != null
                                      && o.StencilOfEngine != null
                                      && o.StencilOfChasis != null
                                      && o.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                var query = from a in db.MAIDealer
                            join b in db.Dealer on a.DealerID equals b.DealerID into temp
                            from temptbl in temp.DefaultIfEmpty()
                            select new
                            {
                                MAIID = a.MAIID,
                                MAIDealerID = a.MAIDealerID,
                                DealerID = temptbl.DealerID,
                                DealerName = temptbl.DealerName,
                                Active = a.Active
                            };
                model.DealerInfo.MAI_DealerList = query.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        MAIDealerID = o.MAIDealerID,
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                model.DealerInfo.MAI_DealerBranchList = new List<DealerBranchModel>();
            }
            return View(model);
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        [HttpGet]
        public ActionResult ForAllocate()
        {
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      && (o.DealerBranchID == null || o.DealerID == null)
                                      && o.BOCCertificateOfPayment != null
                                      && o.StencilOfEngine != null
                                      && o.StencilOfChasis != null
                                      && o.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                var query = from a in db.MAIDealer
                            join b in db.Dealer on a.DealerID equals b.DealerID into temp
                            from temptbl in temp.DefaultIfEmpty()
                            select new
                            {
                                MAIID = a.MAIID,
                                MAIDealerID = a.MAIDealerID,
                                DealerID = temptbl.DealerID,
                                DealerName = temptbl.DealerName,
                                Active = a.Active
                            };
                model.DealerInfo.MAI_DealerList = query.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        MAIDealerID = o.MAIDealerID,
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                model.DealerInfo.MAI_DealerBranchList = new List<DealerBranchModel>();
                return View(model);
            }
        }
        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        public ActionResult ForAllocate(VehicleListModel model)
        {
            using (db = new VRSystemEntities())
            {
                if (ModelState.IsValid)
                {
                    foreach (var item in model.VehicleList)
                    {
                        if (item.isChecked == true)
                        {

                            if (AllocateVehicle(item.VehicleID, model.DealerInfo.SelectedDealer, model.DealerInfo.SelectedDealerBranch))
                            {
                                TempData["SuccessMessage"] = "Allocated Successful!";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "An error has occured!";
                            }
                            //var Update = db.VehicleInfo.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                            //Update.DealerID = model.DealerInfo.SelectedDealer;
                            //Update.DealerBranchID = model.DealerInfo.SelectedDealerBranch;
                            //Update.UpdatedBy = CurrentUser.Details.UserID;
                            //Update.UpdatedDate = DateTime.Now;
                            //db.SaveChanges();
                            //TempData["SuccessMessage"] = "Dealer and Dealer Branch has been assign!";
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "There's something error!";
                }

                model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      && (o.DealerBranchID == null || o.DealerID == null)
                                      && o.BOCCertificateOfPayment != null
                                      && o.StencilOfEngine != null
                                      && o.StencilOfChasis != null
                                      && o.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                var query = from a in db.MAIDealer
                            join b in db.Dealer on a.DealerID equals b.DealerID into temp
                            from temptbl in temp.DefaultIfEmpty()
                            select new
                            {
                                MAIID = a.MAIID,
                                MAIDealerID = a.MAIDealerID,
                                DealerID = temptbl.DealerID,
                                DealerName = temptbl.DealerName,
                                Active = a.Active
                            };
                model.DealerInfo.MAI_DealerList = query.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        MAIDealerID = o.MAIDealerID,
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                model.DealerInfo.MAI_DealerBranchList = new List<DealerBranchModel>();
            }
            return View(model);
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        public ActionResult ReAllocatedVehicle()
        {
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      && (o.DealerBranchID != null || o.DealerID != null)
                                      && o.BOCCertificateOfPayment != null
                                      && o.StencilOfEngine != null
                                      && o.StencilOfChasis != null
                                      && o.Assigned == false
                                      && o.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                var query = from a in db.MAIDealer
                            join b in db.Dealer on a.DealerID equals b.DealerID into temp
                            from temptbl in temp.DefaultIfEmpty()
                            select new
                            {
                                MAIID = a.MAIID,
                                MAIDealerID = a.MAIDealerID,
                                DealerID = temptbl.DealerID,
                                DealerName = temptbl.DealerName,
                                Active = a.Active
                            };
                model.DealerInfo.MAI_DealerList = query.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        MAIDealerID = o.MAIDealerID,
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                model.DealerInfo.MAI_DealerBranchList = new List<DealerBranchModel>();
                return View(model);
            }
        }
        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        public ActionResult ReAllocatedVehicle(VehicleListModel model)
        {
            using (db = new VRSystemEntities())
            {
                if (ModelState.IsValid)
                {
                    foreach (var item in model.VehicleList)
                    {
                        if (item.isChecked == true)
                        {

                            if (AllocateVehicle(item.VehicleID, model.DealerInfo.SelectedDealer, model.DealerInfo.SelectedDealerBranch))
                            {
                                TempData["SuccessMessage"] = "Re-Allocated Vehicle Successful!";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "An error has occured!";
                            }
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "There's something error!";
                }

                model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      && (o.DealerBranchID != null || o.DealerID != null)
                                      && o.BOCCertificateOfPayment != null
                                      && o.StencilOfEngine != null
                                      && o.StencilOfChasis != null
                                      && o.Assigned == false
                                      && o.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                var query = from a in db.MAIDealer
                            join b in db.Dealer on a.DealerID equals b.DealerID into temp
                            from temptbl in temp.DefaultIfEmpty()
                            select new
                            {
                                MAIID = a.MAIID,
                                MAIDealerID = a.MAIDealerID,
                                DealerID = temptbl.DealerID,
                                DealerName = temptbl.DealerName,
                                Active = a.Active
                            };
                model.DealerInfo.MAI_DealerList = query.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        MAIDealerID = o.MAIDealerID,
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                model.DealerInfo.MAI_DealerBranchList = new List<DealerBranchModel>();
            }
            return View(model);
        }


        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        public ActionResult ForPullOut()
        {
            using (db = new VRSystemEntities())
            {
                VehicleListModel model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      && (o.DealerBranchID != null || o.DealerID != null)
                                      && o.CertificateOfConformity == null
                                      && o.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };

                return View(model);
            }
        }
        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        public ActionResult ForPullOut(VehicleListModel model, string submit)
        {
            using (db = new VRSystemEntities())
            {
                if (ModelState.IsValid)
                {
                    switch (submit)
                    {
                        case "pull":

                            foreach (var item in model.VehicleList)
                            {
                                if (item.isChecked == true)
                                {
                                    if (db.DealerInvoice.Where(o => o.VehicleID == item.VehicleID).Count() == 0)
                                    {
                                        var UpdateDealer = db.VehicleInfo.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                                        UpdateDealer.DealerID = null;
                                        UpdateDealer.DealerBranchID = null;
                                        UpdateDealer.Assigned = false;
                                        UpdateDealer.UpdatedBy = CurrentUser.Details.UserID;
                                        UpdateDealer.UpdatedDate = DateTime.Now;
                                        db.SaveChanges();
                                        TempData["SuccessMessage"] = "Pull out vehicle successful!";
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "An error has occured.";
                                    }
                                }
                            }
                            break;
                        case "pulloutReport":
                            return PullOutReport(model.VehicleList);
                            break;
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "There's something error!";
                }

                model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      && (o.DealerBranchID != null || o.DealerID != null)
                                      && o.CertificateOfConformity == null
                                      && o.Active == true
                )
                    .Select(o => new vwVehicleListModel
                    {
                        VehicleID = o.VehicleID,
                        VehicleMakeName = o.VehicleMakeName,
                        VehicleModelName = o.VehicleModelName,
                        Variant = o.Variant,
                        Year = o.Year,
                        EngineNumber = o.EngineNumber,
                        ChassisNumber = o.ChassisNumber,
                        BodyIDNumber = o.BodyIDNumber,
                        isChecked = false
                    }
                    ).ToList()
                };
            }
            return View(model);
        }
        #endregion


        [HttpGet]
        public ActionResult Vehicle(int? id)
        {
            VehicleInfoModel Vehicle = new VehicleInfoModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                {
                    int refid = Convert.ToInt32(CurrentUser.Details.ReferenceID);
                    Vehicle.vwDealerVehicleMakeList = db.vwDealerVehicleMake.Where(o => o.DealerID == refid && o.Active == true).ToList();
                }
                Vehicle.DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                Vehicle.DealerBranchList = new List<DealerBranch>();
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                {
                    Vehicle.vwMAIVehicleMakeList = db.vwMAIVehicleMake.Where(o => o.Active && o.MAIID == CurrentUser.Details.SubReferenceID).ToList();
                }
                else
                {
                    Vehicle.vwMAIVehicleMakeList = new List<vwMAIVehicleMake>();
                }

                Vehicle.VehicleModelList = new List<VehicleModelModel>();
                Vehicle.VehicleColorList = db.VehicleColor.Where(o => o.Active == true).ToList();
                Vehicle.VehicleFuelTypeList = db.VehicleFuelType.Where(o => o.Active == true).ToList();
                Vehicle.VehicleBodyTypeList = db.VehicleBodyType.Where(o => o.Active == true).ToList();
                Vehicle.VehicleAirconTypeList = db.AirconType.Where(o => o.Active == true).ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var VehicleLoad = db.VehicleInfo.Where(o => o.Active == true && o.VehicleID == id).ToList().FirstOrDefault();
                    Vehicle.VehicleModelList = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleLoad.VehicleMakeID)
                        .Select(o => new VehicleModelModel()
                        {
                            VehicleModelID = o.VehicleModelID,
                            VehicleMakeID = o.VehicleMakeID,
                            VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                            Variant = o.Variant,
                            YearOfMake = o.YearOfMake,
                            Active = o.Active,
                            VehicleClassificationID = o.VehicleClassificationID,
                        }).OrderBy(o => o.VehicleModelID).ToList();
                    Vehicle.MAIID = VehicleLoad.MAIID;
                    Vehicle.SelectedMAID = VehicleLoad.MAIID;
                    Vehicle.DealerID = VehicleLoad.DealerID;
                    Vehicle.DealerBranchID = VehicleLoad.DealerBranchID;
                    Vehicle.SelectedVehicleMakeID = VehicleLoad.VehicleMakeID;
                    Vehicle.SelectedVehicleModelID = VehicleLoad.VehicleModelID;
                    Vehicle.EngineNumber = VehicleLoad.EngineNumber;
                    Vehicle.CPNumber = VehicleLoad.CPNumber;
                    Vehicle.DateIssued1 = VehicleLoad.DateIssued1;
                    Vehicle.ChassisNumber = VehicleLoad.ChassisNumber;
                    Vehicle.CPNumber2 = VehicleLoad.CPNumber2;
                    Vehicle.DateIssued2 = VehicleLoad.DateIssued2;
                    Vehicle.BodyIDNumber = VehicleLoad.BodyIDNumber;
                    Vehicle.BIRCCMV = VehicleLoad.BIRCCMV;
                    Vehicle.DateIssued3 = VehicleLoad.DateIssued3;
                    Vehicle.SelectedVehicleColorID = VehicleLoad.VehicleColorID;
                    Vehicle.PistonDisplacement = VehicleLoad.PistonDisplacement;
                    Vehicle.SelectedVehicleFuelTypeID = VehicleLoad.VehicleFuelTypeID;
                    Vehicle.Cylinders = VehicleLoad.Cylinders;
                    Vehicle.Series = VehicleLoad.Series;
                    Vehicle.Year = VehicleLoad.Year;
                    Vehicle.GrossVehicleWeight = VehicleLoad.GrossVehicleWeight;
                    Vehicle.FrontTiresNumber = VehicleLoad.FrontTiresNumber;
                    Vehicle.RearTiresNumber = VehicleLoad.RearTiresNumber;
                    Vehicle.TaxType = VehicleLoad.TaxType;
                    Vehicle.TaxAmount = VehicleLoad.TaxAmount;
                    Vehicle.AirconType = VehicleLoad.AirconType;
                    Vehicle.ConductionSticker = VehicleLoad.ConductionSticker;
                    Vehicle.COCNo = VehicleLoad.COCNo;
                    Vehicle.Remarks = VehicleLoad.Remarks;
                    Vehicle.DatePrepared = VehicleLoad.DatePrepared;
                    Vehicle.CSRNumber = VehicleLoad.CSRNumber;
                    Vehicle.TransactionID = VehicleLoad.TransactionID;
                    Vehicle.Address = VehicleLoad.Address;
                    Vehicle.AccreditationNumber = VehicleLoad.AccreditationNumber;
                    Vehicle.ItemType = VehicleLoad.ItemType;
                    Vehicle.SelectedVehicleBodyTypeID = VehicleLoad.VehicleBodyTypeID;

                    //Images
                    Vehicle.BOCCertificateOfPayment = VehicleLoad.BOCCertificateOfPayment;
                    Vehicle.BOCContentType = VehicleLoad.BOCContentType;
                    Vehicle.BOCCertificateOfPayment2 = VehicleLoad.BOCCertificateOfPayment2;
                    Vehicle.BOCContentType2 = VehicleLoad.BOCContentType2;
                    Vehicle.CertificateOfStockReport = VehicleLoad.CertificateOfStockReport;
                    Vehicle.CSRContentType = VehicleLoad.CSRContentType;
                    Vehicle.CertificateOfConformity = VehicleLoad.CertificateOfConformity;
                    Vehicle.COCContentType = VehicleLoad.COCContentType;
                    Vehicle.StencilOfEngine = VehicleLoad.StencilOfEngine;
                    Vehicle.SOEContentType = VehicleLoad.SOEContentType;
                    Vehicle.StencilOfChasis = VehicleLoad.StencilOfChasis;
                    Vehicle.SOCContentType = VehicleLoad.SOCContentType;
                    Vehicle.PNPClearance = VehicleLoad.PNPClearance;
                    Vehicle.PNPContentType = VehicleLoad.PNPContentType;
                }

                return View(Vehicle);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vehicle(VehicleInfoModel Vehicle, string submit, int? id)
        {
            var createdbydealer = false;
            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
            {
                Vehicle.SelectedMAID = Convert.ToInt32(CurrentUser.Details.SubReferenceID);
            }
            else if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
            {
                createdbydealer = true;
                Vehicle.SelectedMAID = 0;
                Vehicle.SelectedDealer = Convert.ToInt32(CurrentUser.Details.ReferenceID);
                Vehicle.SelectedDealerBranch = Convert.ToInt32(CurrentUser.Details.SubReferenceID);

                ModelState.Remove("BodyIDNumber");
                ModelState.Remove("SelectedVehicleColorID");
                ModelState.Remove("AirconType");
                ModelState.Remove("SelectedVehicleFuelTypeID");
                ModelState.Remove("ConductionSticker");
                ModelState.Remove("PistonDisplacement");
                ModelState.Remove("Cylinders");
                ModelState.Remove("Year");
                ModelState.Remove("GrossVehicleWeight");
                ModelState.Remove("FrontTiresNumber");
                ModelState.Remove("RearTiresNumber");
                ModelState.Remove("COCNo");
            }

            var IsExisted = Functions.IsExistedEngineORChassis(Vehicle.EngineNumber?.Trim(), Vehicle.ChassisNumber?.Trim());

            if (ModelState.IsValid && ((IsExisted.EngineNumber == null || IsExisted.EngineNumber == Vehicle.EngineNumber.Trim()) || (IsExisted.ChassisNumber == null || IsExisted.ChassisNumber == Vehicle.ChassisNumber.Trim())))
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            try
                            {
                                var NewVehicle = new VehicleInfo
                                {
                                    MAIID = Vehicle.SelectedMAID,
                                    DealerID = Vehicle.SelectedDealer,
                                    DealerBranchID = Vehicle.SelectedDealerBranch,
                                    VehicleMakeID = Vehicle.SelectedVehicleMakeID,
                                    VehicleModelID = Vehicle.SelectedVehicleModelID,
                                    EngineNumber = Vehicle.EngineNumber?.Trim(),
                                    CPNumber = Vehicle.CPNumber?.Trim(),
                                    DateIssued1 = Vehicle.DateIssued1,
                                    ChassisNumber = Vehicle.ChassisNumber?.Trim(),
                                    CPNumber2 = Vehicle.CPNumber2?.Trim(),
                                    DateIssued2 = Vehicle.DateIssued2,
                                    BodyIDNumber = Vehicle.BodyIDNumber?.Trim(),
                                    BIRCCMV = Vehicle.BIRCCMV?.Trim(),
                                    DateIssued3 = Vehicle.DateIssued3,
                                    VehicleColorID = Vehicle.SelectedVehicleColorID?? 0,
                                    VehicleBodyTypeID = Vehicle.SelectedVehicleBodyTypeID,
                                    PistonDisplacement = Vehicle.PistonDisplacement?.Trim(),
                                    VehicleFuelTypeID = Vehicle.SelectedVehicleFuelTypeID?? 0,
                                    Cylinders = Vehicle.Cylinders?.Trim(),
                                    //Series = Vehicle.Series?.Trim(),
                                    Year = Vehicle.Year,
                                    GrossVehicleWeight = Vehicle.GrossVehicleWeight,
                                    FrontTiresNumber = Vehicle.FrontTiresNumber,
                                    RearTiresNumber = Vehicle.RearTiresNumber,
                                    TaxType = Vehicle.TaxType?.Trim(),
                                    TaxAmount = Vehicle.TaxAmount,
                                    AirconType = Vehicle.AirconType?.Trim(),
                                    ConductionSticker = Vehicle.ConductionSticker?.Trim(),
                                    COCNo = Vehicle.COCNo?.Trim(),
                                    Remarks = Vehicle.Remarks?.Trim(),
                                    DatePrepared = null,
                                    TransactionID = Vehicle.TransactionID,
                                    CSRNumber = Vehicle.CSRNumber?.Trim(),
                                    ReportEntryID = Vehicle.ReportEntryID,
                                    Address = Vehicle.Address?.Trim(),
                                    ReportDate = null,
                                    AccreditationNumber = Vehicle.AccreditationNumber?.Trim(),
                                    ItemType = Vehicle.ItemType?.Trim(),
                                    Assigned = createdbydealer,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true,
                                    //Images
                                    BOCCertificateOfPayment = Vehicle.BOCFile != null ? Vehicle.BOCFile.ToByte() : null,
                                    BOCContentType = Vehicle.BOCFile != null ? Vehicle.BOCFile.ContentType : null,
                                    BOCCertificateOfPayment2 = Vehicle.BOC2File != null ? Vehicle.BOC2File.ToByte() : null,
                                    BOCContentType2 = Vehicle.BOC2File != null ? Vehicle.BOC2File.ContentType : null,
                                    CertificateOfStockReport = Vehicle.CSRFile != null ? Vehicle.CSRFile.ToByte() : null,
                                    CSRContentType = Vehicle.CSRFile != null ? Vehicle.CSRFile.ContentType : null,
                                    CertificateOfConformity = Vehicle.COCFile != null ? Vehicle.COCFile.ToByte() : null,
                                    COCContentType = Vehicle.COCFile != null ? Vehicle.COCFile.ContentType : null,
                                    StencilOfEngine = Vehicle.SOEFile != null ? Vehicle.SOEFile.ToByte() : null,
                                    SOEContentType = Vehicle.SOEFile != null ? Vehicle.SOEFile.ContentType : null,
                                    StencilOfChasis = Vehicle.SOCFile != null ? Vehicle.SOCFile.ToByte() : null,
                                    SOCContentType = Vehicle.SOCFile != null ? Vehicle.SOCFile.ContentType : null,
                                    PNPClearance = Vehicle.PNPFile != null ? Vehicle.PNPFile.ToByte() : null,
                                    PNPContentType = Vehicle.PNPFile != null ? Vehicle.PNPFile.ContentType : null,
                                };

                                db.VehicleInfo.Add(NewVehicle);
                                db.SaveChanges();
                                TempData["SuccessMessage"] = "Vehicle Info successfuly added!";
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var UpdateVehicle = db.VehicleInfo.Where(o => o.VehicleID == id).FirstOrDefault();
                            UpdateVehicle.MAIID = Vehicle.SelectedMAID;
                            UpdateVehicle.DealerID = Vehicle.SelectedDealer;
                            UpdateVehicle.DealerBranchID = Vehicle.SelectedDealerBranch;

                            UpdateVehicle.ChassisNumber = Vehicle.ChassisNumber?.Trim();
                            UpdateVehicle.EngineNumber = Vehicle.EngineNumber?.Trim();

                            UpdateVehicle.VehicleMakeID = Vehicle.SelectedVehicleMakeID;
                            UpdateVehicle.VehicleModelID = Vehicle.SelectedVehicleModelID;
                            UpdateVehicle.VehicleBodyTypeID = Vehicle.SelectedVehicleBodyTypeID;
                            UpdateVehicle.VehicleColorID = Vehicle.SelectedVehicleColorID;
                            UpdateVehicle.GrossVehicleWeight = Vehicle.GrossVehicleWeight;

                            //UpdateVehicle.Series = Vehicle.Series?.Trim();
                            //UpdateVehicle.Remarks = Vehicle.Remarks.Trim();

                            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                            {

                                UpdateVehicle.CPNumber = Vehicle.CPNumber?.Trim();
                                UpdateVehicle.DateIssued1 = Vehicle.DateIssued1;
                                UpdateVehicle.CPNumber2 = Vehicle.CPNumber2?.Trim();
                                UpdateVehicle.DateIssued2 = Vehicle.DateIssued2;
                                UpdateVehicle.BodyIDNumber = Vehicle.BodyIDNumber?.Trim();
                                UpdateVehicle.BIRCCMV = Vehicle.BIRCCMV?.Trim();
                                UpdateVehicle.DateIssued3 = Vehicle.DateIssued3;
                                UpdateVehicle.PistonDisplacement = Vehicle.PistonDisplacement?.Trim();
                                UpdateVehicle.VehicleFuelTypeID = Vehicle.SelectedVehicleFuelTypeID ?? 0;
                                UpdateVehicle.Cylinders = Vehicle.Cylinders?.Trim();

                                UpdateVehicle.Year = Vehicle.Year;
                                UpdateVehicle.FrontTiresNumber = Vehicle.FrontTiresNumber;
                                UpdateVehicle.RearTiresNumber = Vehicle.RearTiresNumber;
                                UpdateVehicle.TaxType = Vehicle.TaxType?.Trim();
                                UpdateVehicle.TaxAmount = Vehicle.TaxAmount;
                                UpdateVehicle.AirconType = Vehicle.AirconType?.Trim();
                                UpdateVehicle.ConductionSticker = Vehicle.ConductionSticker?.Trim();
                                UpdateVehicle.COCNo = Vehicle.COCNo?.Trim();

                                UpdateVehicle.TransactionID = Vehicle.TransactionID;
                                UpdateVehicle.CSRNumber = Vehicle.CSRNumber?.Trim();
                                UpdateVehicle.ReportEntryID = Vehicle.ReportEntryID;
                                UpdateVehicle.Address = Vehicle.Address?.Trim();
                                UpdateVehicle.AccreditationNumber = Vehicle.AccreditationNumber?.Trim();
                                UpdateVehicle.ItemType = Vehicle.ItemType;
                            }

                            UpdateVehicle.UpdatedBy = CurrentUser.Details.UserID;
                            UpdateVehicle.UpdatedDate = DateTime.Now;
                            //Images
                            if (Vehicle.BOCFile != null)
                            {
                                UpdateVehicle.BOCCertificateOfPayment = Vehicle.BOCFile.ToByte();
                                UpdateVehicle.BOCContentType = Vehicle.BOCFile.ContentType;
                            }

                            if (Vehicle.BOC2File != null)
                            {
                                UpdateVehicle.BOCCertificateOfPayment2 = Vehicle.BOC2File.ToByte();
                                UpdateVehicle.BOCContentType2 = Vehicle.BOC2File.ContentType;
                            }

                            if (Vehicle.CSRFile != null)
                            {
                                UpdateVehicle.CertificateOfStockReport = Vehicle.CSRFile.ToByte();
                                UpdateVehicle.CSRContentType = Vehicle.CSRFile.ContentType;
                            }

                            if (Vehicle.COCFile != null)
                            {
                                UpdateVehicle.CertificateOfConformity = Vehicle.COCFile.ToByte();
                                UpdateVehicle.COCContentType = Vehicle.COCFile.ContentType;
                            }

                            if (Vehicle.SOEFile != null)
                            {
                                UpdateVehicle.StencilOfEngine = Vehicle.SOEFile.ToByte();
                                UpdateVehicle.SOEContentType = Vehicle.SOEFile.ContentType;
                            }

                            if (Vehicle.SOCFile != null)
                            {
                                UpdateVehicle.StencilOfChasis = Vehicle.SOCFile.ToByte();
                                UpdateVehicle.SOCContentType = Vehicle.SOCFile.ContentType;
                            }

                            if (Vehicle.PNPFile != null)
                            {
                                UpdateVehicle.PNPClearance = Vehicle.PNPFile.ToByte();
                                UpdateVehicle.PNPContentType = Vehicle.PNPFile.ContentType;
                            }

                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Vehicle Info update successfuly";
                            return RedirectToAction("VehicleInfo/" + id);
                        }
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var UpdateVehicle = db.VehicleInfo.Where(o => o.VehicleID == id).FirstOrDefault();
                            UpdateVehicle.Active = false;
                            UpdateVehicle.UpdatedBy = CurrentUser.Details.UserID;
                            UpdateVehicle.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                        }
                        break;
                }
                return RedirectToAction("Index");
            }
            else
            {
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                {
                    int refid = Convert.ToInt32(CurrentUser.Details.ReferenceID);
                    Vehicle.vwDealerVehicleMakeList = db.vwDealerVehicleMake.Where(o => o.DealerID == refid).ToList();
                }

                Vehicle.MAIList = db.MAI.Where(o => o.Active == true).ToList();
                Vehicle.DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                Vehicle.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerID == Vehicle.SelectedDealer).ToList();
                Vehicle.VehicleModelList = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == Vehicle.SelectedVehicleMakeID)
                    .Select(o => new VehicleModelModel()
                    {
                        VehicleModelID = o.VehicleModelID,
                        VehicleMakeID = o.VehicleMakeID,
                        VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                        Variant = o.Variant,
                        YearOfMake = o.YearOfMake,
                        Active = o.Active,
                        VehicleClassificationID = o.VehicleClassificationID,
                    }).OrderBy(o => o.VehicleModelID).ToList();
                Vehicle.VehicleColorList = db.VehicleColor.Where(o => o.Active == true).ToList();
                Vehicle.VehicleFuelTypeList = db.VehicleFuelType.Where(o => o.Active == true).ToList();
                Vehicle.VehicleBodyTypeList = db.VehicleBodyType.Where(o => o.Active == true).ToList();
                Vehicle.VehicleAirconTypeList = db.AirconType.Where(o => o.Active == true).ToList();

                if (IsExisted.EngineNumber != null || IsExisted.ChassisNumber != null)
                {
                    if (IsExisted.EngineNumber != null && IsExisted.ChassisNumber != null)
                    {
                        TempData["ErrorMessage"] = "Vehicle Engine Number '" + IsExisted.EngineNumber + "' and Chassis Number '" + IsExisted.ChassisNumber + "' is already existing!";
                    }
                    else if (IsExisted.EngineNumber != null)
                    {
                        TempData["ErrorMessage"] = "Vehicle Engine Number '" + IsExisted.EngineNumber + "' is already existing!";
                    }
                    else if (IsExisted.ChassisNumber != null)
                    {
                        TempData["ErrorMessage"] = "Vehicle Chassis Number '" + IsExisted.ChassisNumber + "' is already existing!";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "An error has occured.";
                }

                if (submit == "Create")
                    ViewBag.Edit = false;

                return View(Vehicle);
            }

        }


        public ActionResult Upload()
        {
            var model = new UploadModel();

            int refid = Convert.ToInt32(CurrentUser.Details.ReferenceID);
            model.VehicleMakeList = db.vwDealerVehicleMake.Where(o => o.DealerID == refid && o.Active == true).ToList();
            model.VehicleModelList = new List<VehicleModelModel>();
            model.VehicleBodyTypeList = db.VehicleBodyType.Where(o => o.Active == true).ToList();
            model.VehicleColorList = db.VehicleColor.Where(o => o.Active == true).ToList();
            model.VehicleFuelTypeList = db.VehicleFuelType.Where(o => o.Active == true).ToList();

            model.VehicleTable = new List<UploadTable> { new UploadTable{
                VehicleMakeID = 0,
                VehicleModelID = 0,
                VehicleBodyTypeID = 0,
                VehicleColorID = 0,
                VehicleFuelTypeID = 0,
                AirconType = "",
                BodyIDNumber = "",
                ChassisNumber = "",
                COCNo ="",
                ConductionSticker = "",
                Cylinders = "",
                EngineNumber = "",
                FrontTiresNumber = "",
                RearTiresNumber = "",
                GrossVehicleWeight = "",
                PistonDisplacement = "",
                Year =""
            }};
            return View(model);
        }

        public ActionResult UploadVehicleInfo()
        {
            var model = new UploadVehicleInfoModel();
            model.Table = null;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVehicleInfo(UploadVehicleInfoModel UploadVehicle, HttpPostedFileBase upload, string submit)
        {
            UploadVehicle.BatchHeader.BatchTypeID = (int)BatchTypeList.NewUpload;

            //NEW
            var cnt = 0;
            var TableErrorHolder = "";
            int refid = Convert.ToInt32(CurrentUser.Details.ReferenceID);
            List<VehicleInfoModel> UploadVehicleList = new List<VehicleInfoModel>();
            switch (submit)
            {
                case "Save":
                    if (db.BatchMaster.Where(o => o.ReferenceNo == UploadVehicle.BatchHeader.ReferenceNo && o.BatchTypeID == UploadVehicle.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                    {
                        ModelState.AddModelError("BatchHeader.ReferenceNo", "Batch reference number is existed");
                    }
                    foreach (var item in UploadVehicle.VehicleTable)
                    {
                        List<VehicleModelModel> VehicleModelList_rslt = new List<VehicleModelModel>();
                        if (db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == item.SelectedVehicleMakeID).Select(o => o.VehicleModelID).Count() > 0)
                            VehicleModelList_rslt = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == item.SelectedVehicleMakeID)
                            .Select(o => new VehicleModelModel()
                            {
                                VehicleModelID = o.VehicleModelID,
                                VehicleMakeID = o.VehicleMakeID,
                                VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                                Variant = o.Variant,
                                YearOfMake = o.YearOfMake,
                                Active = o.Active,
                                VehicleClassificationID = o.VehicleClassificationID,
                            }).OrderBy(o => o.VehicleModelID).ToList();

                        item.VehicleModelList = VehicleModelList_rslt;

                        var IsExisted = Functions.IsExistedEngineORChassis(item.EngineNumber, item.ChassisNumber);
                        if (UploadVehicleList.Where(o => o.EngineNumber == item.EngineNumber).Count() > 0)
                            IsExisted.EngineNumber = item.EngineNumber;
                        if (UploadVehicleList.Where(o => o.ChassisNumber == item.ChassisNumber).Count() > 0)
                            IsExisted.ChassisNumber = item.ChassisNumber;

                        if (IsExisted.EngineNumber != null || IsExisted.ChassisNumber != null)
                        {
                            if (IsExisted.EngineNumber != null)
                                ModelState.AddModelError("VehicleTable[" + cnt + "].EngineNumber", "Vehicle Engine Number is existed");
                            if (IsExisted.ChassisNumber != null)
                                ModelState.AddModelError("VehicleTable[" + cnt + "].ChassisNumber", "Vehicle Chassis Number is existed");
                        }
                        else
                        {
                            int? GVW = null;
                            if (item.GrossVehicleWeight != null)
                               GVW = Convert.ToInt32(item.GrossVehicleWeight);
                            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                            {
                                UploadVehicleList.Add(new VehicleInfoModel()
                                {
                                    VehicleMakeID = item.SelectedVehicleMakeID,
                                    SelectedVehicleMakeID = item.SelectedVehicleMakeID,
                                    VehicleModelID = item.SelectedVehicleModelID,
                                    SelectedVehicleModelID = item.SelectedVehicleModelID,
                                    VehicleBodyTypeID = item.SelectedVehicleBodyTypeID,
                                    SelectedVehicleBodyTypeID = item.SelectedVehicleBodyTypeID,
                                    EngineNumber = item.EngineNumber,
                                    ChassisNumber = item.ChassisNumber,
                                    BodyIDNumber = item.BodyIDNumber,
                                    VehicleColorID = item.SelectedVehicleColorID,
                                    SelectedVehicleColorID = item.SelectedVehicleColorID,
                                    AirconType = item.AirconType,
                                    ConductionSticker = item.ConductionSticker,
                                    VehicleFuelTypeID = item.SelectedVehicleFuelTypeID,
                                    SelectedVehicleFuelTypeID = item.SelectedVehicleFuelTypeID,
                                    PistonDisplacement = item.PistonDisplacement,
                                    Cylinders = item.Cylinders,
                                    //Series = item.Series,
                                    Year = Convert.ToInt32(item.Year),
                                    GrossVehicleWeight = GVW,
                                    FrontTiresNumber = Convert.ToInt32(item.FrontTiresNumber),
                                    RearTiresNumber = Convert.ToInt32(item.RearTiresNumber),
                                    COCNo = item.COCNo,
                                    MAIID = Convert.ToInt32(CurrentUser.Details.SubReferenceID),
                                    SelectedMAID = Convert.ToInt32(CurrentUser.Details.SubReferenceID),
                                    DatePrepared = DateTime.Now,
                                    ReportDate = DateTime.Now,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true
                                });
                            }
                            else if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                            {
                                ModelState.Remove("VehicleTable[" + cnt + "].BodyIDNumber");
                                ModelState.Remove("VehicleTable[" + cnt + "].SelectedVehicleColorID");
                                ModelState.Remove("VehicleTable[" + cnt + "].AirconType");
                                ModelState.Remove("VehicleTable[" + cnt + "].SelectedVehicleFuelTypeID");
                                ModelState.Remove("VehicleTable[" + cnt + "].ConductionSticker");
                                ModelState.Remove("VehicleTable[" + cnt + "].PistonDisplacement");
                                ModelState.Remove("VehicleTable[" + cnt + "].Cylinders");
                                ModelState.Remove("VehicleTable[" + cnt + "].Year");
                                ModelState.Remove("VehicleTable[" + cnt + "].GrossVehicleWeight");
                                ModelState.Remove("VehicleTable[" + cnt + "].FrontTiresNumber");
                                ModelState.Remove("VehicleTable[" + cnt + "].RearTiresNumber");
                                ModelState.Remove("VehicleTable[" + cnt + "].COCNo");

                                UploadVehicleList.Add(new VehicleInfoModel()
                                {
                                    VehicleMakeID = item.SelectedVehicleMakeID,
                                    SelectedVehicleMakeID = item.SelectedVehicleMakeID,
                                    VehicleModelID = item.SelectedVehicleModelID,
                                    SelectedVehicleModelID = item.SelectedVehicleModelID,
                                    VehicleBodyTypeID = item.SelectedVehicleBodyTypeID,
                                    SelectedVehicleBodyTypeID = item.SelectedVehicleBodyTypeID,
                                    EngineNumber = item.EngineNumber,
                                    ChassisNumber = item.ChassisNumber,
                                    //BodyIDNumber = item.BodyIDNumber,
                                    VehicleColorID = item.SelectedVehicleColorID,
                                    SelectedVehicleColorID = item.SelectedVehicleColorID,
                                    //AirconType = item.AirconType,
                                    //ConductionSticker = item.ConductionSticker,
                                    //VehicleFuelTypeID = item.SelectedVehicleFuelTypeID,
                                    //SelectedVehicleFuelTypeID = item.SelectedVehicleFuelTypeID,
                                    //PistonDisplacement = item.PistonDisplacement,
                                    //Cylinders = item.Cylinders,
                                    //Series = item.Series,
                                    //Year = Convert.ToInt32(item.Year),
                                    GrossVehicleWeight = GVW,
                                    //FrontTiresNumber = Convert.ToInt32(item.FrontTiresNumber),
                                    //RearTiresNumber = Convert.ToInt32(item.RearTiresNumber),
                                    //COCNo = item.COCNo,
                                    CSRNumber = item.CSRNumber,
                                    HPGControlNumber = item.HPGNumber,
                                    MAIID = 0,
                                    SelectedMAID = 0,
                                    DealerID = CurrentUser.Details.ReferenceID,
                                    SelectedDealer = Convert.ToInt32(CurrentUser.Details.ReferenceID),
                                    DealerBranchID = CurrentUser.Details.SubReferenceID,
                                    SelectedDealerBranch = Convert.ToInt32(CurrentUser.Details.SubReferenceID),
                                    DatePrepared = DateTime.Now,
                                    ReportDate = DateTime.Now,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true
                                });
                            }
                        }
                        cnt++;
                    }

                    //if (TableErrorHolder != "")
                    //{
                    //    ModelState.AddModelError("TableError", TableErrorHolder);
                    //}

                    if (ModelState.IsValid)
                    {
                        if (InsertVehicleInfo(UploadVehicle.BatchHeader, UploadVehicleList.ToArray()))
                            TempData["SuccessMessage"] = "Successfully Save!";
                        else
                            TempData["ErrorMessage"] = "An error has occured.";

                        var model = new UploadVehicleInfoModel();
                        return View(model);
                    }
                    else
                    {
                        UploadVehicle.VehicleMakeList = db.vwDealerVehicleMake.Where(o => o.DealerID == refid && o.Active == true).ToList();
                        UploadVehicle.VehicleBodyTypeList = db.VehicleBodyType.Where(o => o.Active == true).ToList();
                        UploadVehicle.VehicleColorList = db.VehicleColor.Where(o => o.Active == true).ToList();
                        UploadVehicle.VehicleFuelTypeList = db.VehicleFuelType.Where(o => o.Active == true).ToList();
                        UploadVehicle.VehicleAirconTypeList = db.AirconType.Where(o => o.Active == true).ToList();

                        return View(UploadVehicle);
                    }

                    break;
                case "Upload":
                    if (ModelState.IsValid)
                    {
                        ////BatchMaster Validation
                        //if (db.BatchMaster.Where(o => o.ReferenceNo == UploadVehicle.BatchHeader.ReferenceNo && o.BatchTypeID == UploadVehicle.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                        //{
                        //    //TempData["ErrorMessage"] = "Batch reference number is existed!";
                        //    //return View(UploadVehicle);
                        //    ModelState.AddModelError("BatchHeader.ReferenceNo", "Batch reference number is existed");
                        //}
                        if (upload != null && upload.ContentLength > 0)
                        {
                            // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                            // to get started. This is how we avoid dependencies on ACE or Interop:
                            Stream stream = upload.InputStream;

                            // We return the interface, so that
                            IExcelDataReader reader = null;

                            try
                            {
                                if (upload.FileName.EndsWith(".xls"))
                                {
                                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                                }
                                else if (upload.FileName.EndsWith(".xlsx"))
                                {
                                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "This file is not supported";
                                    return View(UploadVehicle);
                                }
                            }
                            catch (Exception)
                            {
                                TempData["ErrorMessage"] = "An error has occured while reading the file. Please check your file.";
                                return View(UploadVehicle); 
                            }
                         

                            DataTable NewDataTable;
                            try
                            {

                                NewDataTable = reader.AsDataSet().Tables[0].FormatDataTable();

                            }
                            catch (Exception)
                            {
                                TempData["ErrorMessage"] = "Unable to upload file!";
                                return View(UploadVehicle);
                            }

                            var NewModel = new UploadVehicleInfoModel();
                            NewModel.Table = NewDataTable;

                            if (NewDataTable.Rows.Count == 0)
                            {
                                TempData["ErrorMessage"] = "There's no data in that file!";
                                reader.Close();
                                reader.Dispose();

                                return View(NewModel);
                            }
                            //NEW - END
                            //List<VehicleInfoModel> UploadVehicleList = new List<VehicleInfoModel>();
                            foreach (DataRow item in NewDataTable.Rows)
                            {
                                //NEW
                                var VehicleMakeName = item.ItemArray[0].ToString()?.ToLower().Trim();
                                var VehicleMakeID_rslt = db.VehicleMake.Where(o => o.Active == true && o.VehicleMakeName.ToLower().Contains(VehicleMakeName)).Select(o => o.VehicleMakeID).FirstOrDefault();
                                if (VehicleMakeID_rslt == 0)
                                    ModelState.AddModelError("VehicleTable[" + cnt + "].SelectedVehicleMakeID", "Vehicle Make data are not match from our data");

                                var VehicleModelName = item.ItemArray[2].ToString()?.ToLower().Trim();
                                //var VehicleModelID_rslt = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt && o.VehicleModelName.ToLower().Contains(VehicleModelName)).Select(o => o.VehicleModelID).FirstOrDefault();
                                var VehicleModelID_rslt = (db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt)
                                    .Select(o => new VehicleModelModel()
                                    {
                                        VehicleModelID = o.VehicleModelID,
                                        VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                                    }).OrderBy(o => o.VehicleModelID)).Where(o => o.VehicleModelName.ToLower().Contains(VehicleModelName)).Select(o => o.VehicleModelID).FirstOrDefault();
                                if (VehicleModelID_rslt == 0)
                                    ModelState.AddModelError("VehicleTable[" + cnt + "].SelectedVehicleModelID", "Vehicle Model data are not match from our data");

                                var VehicleBodyTypeName = item.ItemArray[4].ToString()?.ToLower().Trim();
                                var VehicleBodyTypeID_rslt = db.VehicleBodyType.Where(o => o.Active == true && o.VehicleBodyTypeName.ToLower().Contains(VehicleBodyTypeName)).Select(o => o.VehicleBodyTypeID).FirstOrDefault();
                                if (VehicleBodyTypeID_rslt == 0)
                                    ModelState.AddModelError("VehicleTable[" + cnt + "].SelectedVehicleBodyTypeID", "Vehicle Body Type data are not match from our data");

                                var EngineNumber_rslt = item.ItemArray[6].ToString()?.Trim();
                                var ChassisNumber_rslt = item.ItemArray[7].ToString()?.Trim();
                                var IsExisted = Functions.IsExistedEngineORChassis(EngineNumber_rslt, ChassisNumber_rslt);
                                if (UploadVehicle.VehicleTable.Where(o => o.EngineNumber == EngineNumber_rslt).Count() > 0)
                                    IsExisted.EngineNumber = EngineNumber_rslt;
                                if (UploadVehicle.VehicleTable.Where(o => o.ChassisNumber == ChassisNumber_rslt).Count() > 0)
                                    IsExisted.ChassisNumber = ChassisNumber_rslt;

                                if (IsExisted.EngineNumber != null)
                                    ModelState.AddModelError("VehicleTable[" + cnt + "].EngineNumber", "Vehicle Engine Number is existed");
                                if (IsExisted.ChassisNumber != null)
                                    ModelState.AddModelError("VehicleTable[" + cnt + "].ChassisNumber", "Vehicle Chassis Number is existed");

                                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                                {
                                    var VehicleColorName = item.ItemArray[8].ToString()?.ToLower().Trim();
                                    var VehicleColorID_rslt = db.VehicleColor.Where(o => o.Active == true && o.VehicleColorName.ToLower().Contains(VehicleColorName)).Select(o => o.VehicleColorID).FirstOrDefault();
                                    if (VehicleColorName == "")
                                        VehicleColorID_rslt = 0;

                                    List<VehicleModelModel> VehicleModelList_rslt = new List<VehicleModelModel>();
                                    if (db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt).Select(o => o.VehicleModelID).Count() > 0)
                                        VehicleModelList_rslt = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt)
                                        .Select(o => new VehicleModelModel()
                                        {
                                            VehicleModelID = o.VehicleModelID,
                                            VehicleMakeID = o.VehicleMakeID,
                                            VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                                            Variant = o.Variant,
                                            YearOfMake = o.YearOfMake,
                                            Active = o.Active,
                                            VehicleClassificationID = o.VehicleClassificationID,
                                        }).OrderBy(o => o.VehicleModelID).ToList();

                                    UploadVehicle.VehicleTable.Add(new UploadTable()
                                    {
                                        VehicleMakeID = VehicleMakeID_rslt,
                                        SelectedVehicleMakeID = VehicleMakeID_rslt,
                                        VehicleModelID = VehicleModelID_rslt,
                                        SelectedVehicleModelID = VehicleModelID_rslt,
                                        VehicleBodyTypeID = VehicleBodyTypeID_rslt,
                                        SelectedVehicleBodyTypeID = VehicleBodyTypeID_rslt,
                                        EngineNumber = EngineNumber_rslt,
                                        ChassisNumber = ChassisNumber_rslt,
                                        VehicleColorID = VehicleColorID_rslt,
                                        SelectedVehicleColorID = VehicleColorID_rslt,
                                        GrossVehicleWeight = item.ItemArray[10].ToString().Trim(),
                                        CSRNumber = item.ItemArray[11].ToString().Trim(),
                                        HPGNumber = item.ItemArray[12].ToString().Trim(),
                                        VehicleModelList = VehicleModelList_rslt
                                    });
                                }
                                else
                                {
                                    var VehicleColorName = item.ItemArray[9].ToString()?.ToLower().Trim();
                                    var VehicleColorID_rslt = db.VehicleColor.Where(o => o.Active == true && o.VehicleColorName.ToLower().Contains(VehicleColorName)).Select(o => o.VehicleColorID).FirstOrDefault();
                                    //if (VehicleColorID_rslt == 0)
                                    //    ModelState.AddModelError("VehicleTable[" + cnt + "].SelectedVehicleColorID", "Vehicle Color data are not match from our data");

                                    var VehicleAirconTypeName = item.ItemArray[11].ToString()?.ToLower().Trim();
                                    var VehicleAirconTypeID_rslt = db.AirconType.Where(o => o.Active == true && o.AirconTypeDescription.ToLower().Contains(VehicleAirconTypeName)).Select(o => o.AirconTypeReference).FirstOrDefault();
                                    //if (VehicleAirconTypeID_rslt == "")
                                    //    ModelState.AddModelError("VehicleTable[" + cnt + "].AirconType", "Aircon Type data are not match from our data");

                                    var VehicleFuelTypeName = item.ItemArray[13].ToString()?.ToLower().Trim();
                                    var VehicleFuelTypeID_rslt = db.VehicleFuelType.Where(o => o.Active == true && o.VehicleFuelTypeName.ToLower().Contains(VehicleFuelTypeName)).Select(o => o.VehicleFuelTypeID).FirstOrDefault();
                                    //if (VehicleFuelTypeID_rslt == 0)
                                    //    ModelState.AddModelError("VehicleTable[" + cnt + "].SelectedVehicleFuelTypeID", "Vehicle Fuel Type data are not match from our data");

                                    List<VehicleModelModel> VehicleModelList_rslt = new List<VehicleModelModel>();
                                    if (db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt).Select(o => o.VehicleModelID).Count() > 0)
                                        VehicleModelList_rslt = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt)
                                        .Select(o => new VehicleModelModel()
                                        {
                                            VehicleModelID = o.VehicleModelID,
                                            VehicleMakeID = o.VehicleMakeID,
                                            VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                                            Variant = o.Variant,
                                            YearOfMake = o.YearOfMake,
                                            Active = o.Active,
                                            VehicleClassificationID = o.VehicleClassificationID,
                                        }).OrderBy(o => o.VehicleModelID).ToList();

                                    UploadVehicle.VehicleTable.Add(new UploadTable()
                                    {
                                        VehicleMakeID = VehicleMakeID_rslt,
                                        SelectedVehicleMakeID = VehicleMakeID_rslt,
                                        VehicleModelID = VehicleModelID_rslt,
                                        SelectedVehicleModelID = VehicleModelID_rslt,
                                        VehicleBodyTypeID = VehicleBodyTypeID_rslt,
                                        SelectedVehicleBodyTypeID = VehicleBodyTypeID_rslt,
                                        EngineNumber = EngineNumber_rslt,
                                        ChassisNumber = ChassisNumber_rslt,
                                        BodyIDNumber = item.ItemArray[8].ToString().Trim(),
                                        VehicleColorID = VehicleColorID_rslt,
                                        SelectedVehicleColorID = VehicleColorID_rslt,
                                        AirconType = VehicleAirconTypeID_rslt,
                                        VehicleFuelTypeID = VehicleFuelTypeID_rslt,
                                        SelectedVehicleFuelTypeID = VehicleFuelTypeID_rslt,
                                        ConductionSticker = item.ItemArray[15].ToString()?.Trim(),
                                        PistonDisplacement = item.ItemArray[16].ToString().Trim(),
                                        Cylinders = item.ItemArray[17].ToString().Trim(),
                                        Year = item.ItemArray[18].ToString().Trim(),
                                        GrossVehicleWeight = item.ItemArray[19].ToString().Trim(),
                                        FrontTiresNumber = item.ItemArray[20].ToString().Trim(),
                                        RearTiresNumber = item.ItemArray[21].ToString().Trim(),
                                        COCNo = item.ItemArray[22].ToString()?.Trim(),
                                        VehicleModelList = VehicleModelList_rslt
                                    });
                                }
                                cnt++;
                            }

                            //NEW 
                            if (db.BatchMaster.Where(o => o.ReferenceNo == UploadVehicle.BatchHeader.ReferenceNo && o.BatchTypeID == UploadVehicle.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                            {
                                ModelState.AddModelError("BatchHeader.ReferenceNo", "Batch reference number is existed");
                            }

                            UploadVehicle.VehicleMakeList = db.vwDealerVehicleMake.Where(o => o.DealerID == refid && o.Active == true).ToList();
                            UploadVehicle.VehicleBodyTypeList = db.VehicleBodyType.Where(o => o.Active == true).ToList();
                            UploadVehicle.VehicleColorList = db.VehicleColor.Where(o => o.Active == true).ToList();
                            UploadVehicle.VehicleFuelTypeList = db.VehicleFuelType.Where(o => o.Active == true).ToList();
                            UploadVehicle.VehicleAirconTypeList = db.AirconType.Where(o => o.Active == true).ToList();

                            reader.Close();
                            reader.Dispose();

                            return View(UploadVehicle);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Please upload your file!";
                        }
                    }
                    return View(UploadVehicle);
                    break;
            }
            return View(UploadVehicle);
        }

        [HttpPost]
        public FileResult VehicleInfo_template()
        {
            DataTable dealer_dt = new DataTable();
            DataTable dealerbranch_dt = new DataTable();
            DataTable make_dt = new DataTable();
            DataTable model_dt = new DataTable();
            DataTable fuel_dt = new DataTable();
            DataTable color_dt = new DataTable();
            DataTable body_dt = new DataTable();
            DataTable aircon_dt = new DataTable();

            string path = "";
            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
            {
                path = Server.MapPath("~/UploadTemplate/VehicleInfo-Template-MAI.xlsx");
                var query = from a in db.MAIDealer
                            join b in db.Dealer on a.DealerID equals b.DealerID into temp
                            from temptbl in temp.DefaultIfEmpty()
                            select new
                            {
                                MAIID = a.MAIID,
                                MAIDealerID = a.MAIDealerID,
                                DealerID = temptbl.DealerID,
                                DealerName = temptbl.DealerName,
                                Active = a.Active
                            };
                var dealer = query.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                var dealerBranch = db.DealerBranch.Where(o => o.Active == true).Select(x => new { x.DealerID, x.DealerBranchName, x.DealerBranchID }).OrderBy(o => o.DealerID).ToList();
                var vehicleMake = db.vwMAIVehicleMake.Where(o => o.Active == true && o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(x => new { x.VehicleMakeID, x.VehicleMakeName }).ToList();
                var vehicleModel = db.VehicleModel.Where(o => o.Active == true).Select(x => new { x.VehicleMakeID, x.VehicleModelID, x.VehicleModelName, x.Variant }).OrderBy(o => o.VehicleMakeID).ThenBy(o => o.VehicleModelName).ToList();
                var vehicleFuel = db.VehicleFuelType.Where(o => o.Active == true).Select(x => new { x.VehicleFuelTypeID, x.VehicleFuelTypeName }).ToList();
                var vehicleColor = db.VehicleColor.Where(o => o.Active == true).Select(x => new { x.VehicleColorID, x.VehicleColorName }).ToList();
                var vehicleBody = db.VehicleBodyType.Where(o => o.Active == true).Select(x => new { x.VehicleBodyTypeID, x.VehicleBodyTypeName }).ToList();
                var vehicleAircon = db.AirconType.Where(o => o.Active == true).Select(x => new { x.AirconTypeReference, x.AirconTypeDescription }).ToList();

                //dealer_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("DealerID"),
                //                            new DataColumn("DealerName") });
                //dealerbranch_dt.Columns.AddRange(new DataColumn[3] { new DataColumn("DealerID"),
                //                            new DataColumn("DealerBranchName"),
                //                            new DataColumn("DealerBranchID")});
                make_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleMakeID"),
                                            new DataColumn("VehicleMakeName")});
                model_dt.Columns.AddRange(new DataColumn[3] { new DataColumn("VehicleMakeID"),
                                            new DataColumn("VehicleModelName"),
                                            new DataColumn("VehicleModelID")});
                fuel_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleFuelTypeName"),
                                            new DataColumn("VehicleFuelTypeID")});
                color_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleColorName"),
                                            new DataColumn("VehicleColorID")});
                body_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleBodyTypeName"),
                                            new DataColumn("VehicleBodyTypeID")});
                aircon_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("AirconTypeDescription"),
                                            new DataColumn("AirconTypeReference")});

                foreach (var list in vehicleMake)
                {
                    make_dt.Rows.Add(list.VehicleMakeName, list.VehicleMakeID);
                }
                foreach (var list in vehicleModel)
                {
                    model_dt.Rows.Add(list.VehicleMakeID, list.VehicleModelName + " - " + list.Variant, list.VehicleModelID);
                }
                foreach (var list in vehicleFuel)
                {
                    fuel_dt.Rows.Add(list.VehicleFuelTypeName, list.VehicleFuelTypeID);
                }
                foreach (var list in vehicleColor)
                {
                    color_dt.Rows.Add(list.VehicleColorName, list.VehicleColorID);
                }
                foreach (var list in vehicleBody)
                {
                    body_dt.Rows.Add(list.VehicleBodyTypeName, list.VehicleBodyTypeID);
                }
                foreach (var list in vehicleAircon)
                {
                    aircon_dt.Rows.Add(list.AirconTypeDescription, list.AirconTypeReference);
                }
            }
            else if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
            {
                path = Server.MapPath("~/UploadTemplate/VehicleInfo-Template-Dealer.xlsx");

                var vehicleMake = db.vwDealerVehicleMake.Where(o => o.DealerID == CurrentUser.Details.ReferenceID && o.Active == true).Select(x => new { x.VehicleMakeID, x.VehicleMakeName }).ToList();
                var vehicleModel = db.VehicleModel.Where(o => o.Active == true).Select(x => new { x.VehicleMakeID, x.VehicleModelID, x.VehicleModelName, x.Variant }).OrderBy(o => o.VehicleMakeID).ThenBy(o => o.VehicleModelName).ToList();
                var vehicleFuel = db.VehicleFuelType.Where(o => o.Active == true).Select(x => new { x.VehicleFuelTypeID, x.VehicleFuelTypeName }).ToList();
                var vehicleColor = db.VehicleColor.Where(o => o.Active == true).Select(x => new { x.VehicleColorID, x.VehicleColorName }).ToList();
                var vehicleBody = db.VehicleBodyType.Where(o => o.Active == true).Select(x => new { x.VehicleBodyTypeID, x.VehicleBodyTypeName }).ToList();
                var vehicleAircon = db.AirconType.Where(o => o.Active == true).Select(x => new { x.AirconTypeReference, x.AirconTypeDescription }).ToList();

                make_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleMakeID"),
                                            new DataColumn("VehicleMakeName")});
                model_dt.Columns.AddRange(new DataColumn[3] { new DataColumn("VehicleMakeID"),
                                            new DataColumn("VehicleModelName"),
                                            new DataColumn("VehicleModelID")});
                fuel_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleFuelTypeName"),
                                            new DataColumn("VehicleFuelTypeID")});
                color_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleColorName"),
                                            new DataColumn("VehicleColorID")});
                body_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleBodyTypeName"),
                                            new DataColumn("VehicleBodyTypeID")});
                aircon_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("AirconTypeDescription"),
                                            new DataColumn("AirconTypeReference")});

                foreach (var list in vehicleMake)
                {
                    make_dt.Rows.Add(list.VehicleMakeName, list.VehicleMakeID);
                }
                foreach (var list in vehicleModel)
                {
                    model_dt.Rows.Add(list.VehicleMakeID, list.VehicleModelName + " - " + list.Variant, list.VehicleModelID);
                }
                foreach (var list in vehicleFuel)
                {
                    fuel_dt.Rows.Add(list.VehicleFuelTypeName, list.VehicleFuelTypeID);
                }
                foreach (var list in vehicleColor)
                {
                    color_dt.Rows.Add(list.VehicleColorName, list.VehicleColorID);
                }
                foreach (var list in vehicleBody)
                {
                    body_dt.Rows.Add(list.VehicleBodyTypeName, list.VehicleBodyTypeID);
                }
                foreach (var list in vehicleAircon)
                {
                    aircon_dt.Rows.Add(list.AirconTypeDescription, list.AirconTypeReference);
                }
            }

            using (XLWorkbook wb = new XLWorkbook(path))
            {

                if (wb.Worksheets.Count() > 1)
                {
                    wb.Worksheet("List").Delete();
                }

                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                {
                    var ws = wb.Worksheets.Add("List").Hide();
                    if (make_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 1).InsertTable(make_dt.AsEnumerable());
                    }
                    if (model_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 4).InsertTable(model_dt.AsEnumerable());
                    }
                    if (fuel_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 8).InsertTable(fuel_dt.AsEnumerable());
                    }
                    if (color_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 11).InsertTable(color_dt.AsEnumerable());
                    }
                    if (body_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 14).InsertTable(body_dt.AsEnumerable());
                    }
                    if (body_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 17).InsertTable(aircon_dt.AsEnumerable());
                    }
                }
                else if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                {
                    var ws = wb.Worksheets.Add("List").Hide();
                    if (make_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 1).InsertTable(make_dt.AsEnumerable());
                    }
                    if (model_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 4).InsertTable(model_dt.AsEnumerable());
                    }
                    if (fuel_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 8).InsertTable(fuel_dt.AsEnumerable());
                    }
                    if (color_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 11).InsertTable(color_dt.AsEnumerable());
                    }
                    if (body_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 14).InsertTable(body_dt.AsEnumerable());
                    }
                    if (body_dt.Rows.Count > 0)
                    {
                        ws.Cell(1, 17).InsertTable(aircon_dt.AsEnumerable());
                    }
                }

                //wb.Save();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VehicleInfo-Template.xlsx");
                }
            }
        }

        #region [ Upload V2 ]
        public ActionResult UploadVehicleInfoV2()
        {
            var model = new UploadVehicleInfoModelV2();
            model.Table = null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVehicleInfoV2(UploadVehicleInfoModelV2 UploadVehicle, HttpPostedFileBase upload, string submit)
        {
            UploadVehicle.BatchHeader.BatchTypeID = (int)BatchTypeList.NewUpload;

            //NEW
            var cnt = 0;
            var TableErrorHolder = "";
            int refid = Convert.ToInt32(CurrentUser.Details.ReferenceID);
            List<VehicleInfo> UploadVehicleList = new List<VehicleInfo>();
            List<Customer> NewCustomerList = new List<Customer>();
            List<DealerInvoice> NewInvoiceList = new List<DealerInvoice>();
            switch (submit)
            {
                case "Save":
                    if (db.BatchMaster.Where(o => o.ReferenceNo == UploadVehicle.BatchHeader.ReferenceNo && o.BatchTypeID == UploadVehicle.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                    {
                        ModelState.AddModelError("BatchHeader.ReferenceNo", "Batch reference number is existed");
                    }
                    foreach (var item in UploadVehicle.VehicleTableV2)
                    {
                        //List<VehicleModelModel> VehicleModelList_rslt = new List<VehicleModelModel>();
                        //if (db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == item.SelectedVehicleMakeID).Select(o => o.VehicleModelID).Count() > 0)
                        //    VehicleModelList_rslt = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == item.SelectedVehicleMakeID)
                        //    .Select(o => new VehicleModelModel()
                        //    {
                        //        VehicleModelID = o.VehicleModelID,
                        //        VehicleMakeID = o.VehicleMakeID,
                        //        VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                        //        Variant = o.Variant,
                        //        YearOfMake = o.YearOfMake,
                        //        Active = o.Active,
                        //        VehicleClassificationID = o.VehicleClassificationID,
                        //    }).OrderBy(o => o.VehicleModelID).ToList();

                        var CityList_rslt = db.City.Where(o => o.Active == true && o.ProvinceID == item.ProvinceID).Select(o => new CityList_upload() { CityID = o.CityID, CityName = o.CityName }).ToList();
                        var BarangayList_rslt = db.Barangay.Where(o => o.Active == true && o.CityID == item.CityID).Select(o => new BarangayList_upload() { BarangayID = o.BarangayID, BarangayName = o.BarangayName }).ToList();
                        
                        //item.VehicleModelList = VehicleModelList_rslt;
                        item.Citylist = CityList_rslt;
                        item.Barangaylist = BarangayList_rslt;

                        var IsExisted = Functions.IsExistedEngineORChassis(item.EngineNumber, item.ChassisNumber);
                        if (UploadVehicleList.Where(o => o.EngineNumber == item.EngineNumber).Count() > 0)
                            IsExisted.EngineNumber = item.EngineNumber;
                        if (UploadVehicleList.Where(o => o.ChassisNumber == item.ChassisNumber).Count() > 0)
                            IsExisted.ChassisNumber = item.ChassisNumber;

                        if (IsExisted.EngineNumber != null || IsExisted.ChassisNumber != null)
                        {
                            if (IsExisted.EngineNumber != null)
                                ModelState.AddModelError("VehicleTableV2[" + cnt + "].EngineNumber", "Vehicle Engine Number is existed");
                            if (IsExisted.ChassisNumber != null)
                                ModelState.AddModelError("VehicleTableV2[" + cnt + "].ChassisNumber", "Vehicle Chassis Number is existed");
                        }
                        else
                        {
                            //int? GVW = null;
                            //if (item.GrossVehicleWeight != null)
                            //    GVW = Convert.ToInt32(item.GrossVehicleWeight);
                            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                            {
                                ModelState.Remove("VehicleTableV2[" + cnt + "].SelectedVehicleMakeID");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].SelectedVehicleModelID");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].SelectedVehicleBodyTypeID");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].SelectedVehicleColorID");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].BodyIDNumber");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].SelectedVehicleColorID");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].AirconType");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].SelectedVehicleFuelTypeID");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].ConductionSticker");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].PistonDisplacement");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].Cylinders");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].Year");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].GrossVehicleWeight");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].FrontTiresNumber");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].RearTiresNumber");
                                ModelState.Remove("VehicleTableV2[" + cnt + "].COCNo");

                                var titletype = db.Title.Where(o => o.Active == true && o.TitleID == item.TitleID).FirstOrDefault().TitleTypeID;
                                if (titletype == 1)
                                {
                                    NewCustomerList.Add(new Customer()
                                    {
                                        DealerID = Convert.ToInt32(CurrentUser.Details.ReferenceID),
                                        TitleID = item.TitleID,
                                        //
                                        LastName = item.LastName?.Trim(),
                                        FirstName = item.FirstName?.Trim(),
                                        MiddleName = item.MiddleName?.Trim(),

                                        Birthdate = item.Birthdate ?? DateTime.Now,
                                        Birthplace = "-",
                                        FathersName = "-",
                                        MothersName = "-",
                                        SexCode = item.SexCode,
                                        CivilStatusCode = item.CivilStatusCode,
                                        Citizenship = "-",
                                        Height = "-",
                                        Weight = "-",
                                        //
                                        ContactNumber = item.ContactNumber?.Trim(),
                                        EmailAddress = item.EmailAddress?.Trim(),
                                        TIN = item.TIN?.Trim(),
                                        HouseBldgNumber = item.HouseBldgNumber?.Trim(),
                                        StreetSubdivision = item.Street?.Trim(),
                                        Barangay = item.BarangayID.ToString(),
                                        CityID = item.CityID,
                                        ZipCode = item.ZipCode?.Trim(),
                                        AdditionalAddress = "",
                                        //
                                        Active = true,
                                        CreatedBy = CurrentUser.Details.UserID,
                                        CreatedDate = DateTime.Now,
                                    });
                                }
                                else
                                {
                                    NewCustomerList.Add(new Customer()
                                    {
                                        DealerID = Convert.ToInt32(CurrentUser.Details.ReferenceID),
                                        TitleID = item.TitleID,
                                        //
                                        Birthdate = DateTime.Now,
                                        //
                                        CorpName = item.OrgName?.Trim(),
                                        Alias = item.OrgMnemonic?.Trim(),

                                        ContactPerson = item.PrimaryContact?.Trim(),
                                        ContactPersonNumber = item.ContactDetails?.Trim(),

                                        OrganizationName = item.OrgName?.Trim(),
                                        OrganizationAddress = "",
                                        OrganizationTIN = item.TINOrg?.Trim(),
                                        //
                                        ContactNumber = item.PhoneNo?.Trim(),
                                        EmailAddress = item.EmailAddressOrg?.Trim(),
                                        TIN = item.TIN?.Trim(),
                                        HouseBldgNumber = item.HouseBldgNumber?.Trim(),
                                        StreetSubdivision = item.Street?.Trim(),
                                        Barangay = item.BarangayID.ToString(),
                                        CityID = item.CityID,
                                        ZipCode = item.ZipCode?.Trim(),
                                        AdditionalAddress = "",
                                        //
                                        Active = true,
                                        CreatedBy = CurrentUser.Details.UserID,
                                        CreatedDate = DateTime.Now,
                                    }); ;
                                }

                                NewInvoiceList.Add(new DealerInvoice()
                                {
                                    InvoiceNumber = item.InvoiceNumber,
                                    VehicleCost = item.InvoiceAmount,
                                    InvoiceDate = Convert.ToDateTime(item.InvoiceDate),

                                    DealerBranchID = Convert.ToInt32(CurrentUser.Details.SubReferenceID),

                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true,
                                });

                                UploadVehicleList.Add(new VehicleInfo()
                                {
                                    //VehicleMakeID = item.SelectedVehicleMakeID,
                                    //VehicleModelID = item.SelectedVehicleModelID,
                                    //VehicleBodyTypeID = item.SelectedVehicleBodyTypeID,
                                    EngineNumber = item.EngineNumber,
                                    ChassisNumber = item.ChassisNumber,
                                    //VehicleColorID = item.SelectedVehicleColorID,
                                    //GrossVehicleWeight = GVW,
                                    CSRNumber = item.CSRNumber,
                                    HPGControlNumber = item.HPGNumber,
                                    MAIID = 0,

                                    DealerID = CurrentUser.Details.ReferenceID,
                                    DealerBranchID = CurrentUser.Details.SubReferenceID,
                                    Assigned = true,

                                    DatePrepared = DateTime.Now,
                                    ReportDate = DateTime.Now,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true
                                });
                            }
                        }
                        cnt++;
                    }


                    if (ModelState.IsValid)
                    {
                        //if (InsertVehicleInfo(UploadVehicle.BatchHeader, UploadVehicleList.ToArray()))
                        //    TempData["SuccessMessage"] = "Successfully Save!";
                        //else
                        //    TempData["ErrorMessage"] = "An error has occured.";

                        using (db = new VRSystemEntities())
                        {
                            db.VehicleInfo.AddRange(UploadVehicleList);
                            db.Customer.AddRange(NewCustomerList);
                            db.SaveChanges();
                        }

                        var i = 0;
                        foreach (var invoice in NewInvoiceList) {
                            NewInvoiceList[i].VehicleID = UploadVehicleList[i].VehicleID;
                            NewInvoiceList[i].CustomerID = NewCustomerList[i].CustomerID;
                            i++;
                        }
                        using (db = new VRSystemEntities())
                        {
                            db.DealerInvoice.AddRange(NewInvoiceList);
                            db.SaveChanges();
                        }

                        using (db = new VRSystemEntities())
                        {
                            var batchID = BatchHeaderInsert(UploadVehicle.BatchHeader, UploadVehicleList.Count);
                            bool batchdetails = false;
                            if (batchID != null)
                            {
                                //BatchList
                                List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                                //Batch add to list
                                foreach (var list in UploadVehicleList)
                                {
                                    BatchDetailsList.Add(new BatchDetails
                                    {
                                        BatchID = Convert.ToInt32(batchID),
                                        VehicleID = list.VehicleID,
                                        TransactionID = null
                                    });
                                }
                                //Batch Details Insert
                                batchdetails = BatchDetailsInsert(BatchDetailsList);
                            }

                            if (batchID != null && batchdetails)
                                TempData["SuccessMessage"] = "Successfully Save!";
                            else
                                TempData["ErrorMessage"] = "An error has occured.";
                        }
                        var model = new UploadVehicleInfoModelV2();
                        return View(model);
                    }
                    else
                    {
                        //UploadVehicle.VehicleMakeList = db.vwDealerVehicleMake.Where(o => o.DealerID == refid && o.Active == true).ToList();
                        //UploadVehicle.VehicleBodyTypeList = db.VehicleBodyType.Where(o => o.Active == true).ToList();
                        //UploadVehicle.VehicleColorList = db.VehicleColor.Where(o => o.Active == true).ToList();
                        //UploadVehicle.VehicleFuelTypeList = db.VehicleFuelType.Where(o => o.Active == true).ToList();
                        //UploadVehicle.VehicleAirconTypeList = db.AirconType.Where(o => o.Active == true).ToList();
                        UploadVehicle.TitleList = db.Title.Where(o => o.Active == true).ToList();
                        UploadVehicle.CivilStatusList = db.CivilStatus.Where(o => o.Active == true).ToList();
                        UploadVehicle.SexList = db.Sex.ToList();
                        UploadVehicle.ProvinceList = db.Province.Where(o => o.Active == true).ToList();

                        return View(UploadVehicle);
                    }

                    break;
                case "Upload":
                    if (ModelState.IsValid)
                    {
                        if (upload != null && upload.ContentLength > 0)
                        {
                            // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                            // to get started. This is how we avoid dependencies on ACE or Interop:
                            Stream stream = upload.InputStream;

                            // We return the interface, so that
                            IExcelDataReader reader = null;

                            try
                            {
                                if (upload.FileName.EndsWith(".xls"))
                                {
                                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                                }
                                else if (upload.FileName.EndsWith(".xlsx"))
                                {
                                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "This file is not supported";
                                    return View(UploadVehicle);
                                }
                            }
                            catch (Exception)
                            {
                                TempData["ErrorMessage"] = "An error has occured while reading the file. Please check your file.";
                                return View(UploadVehicle);
                            }


                            DataTable NewDataTable;
                            try
                            {

                                NewDataTable = reader.AsDataSet().Tables[0].FormatDataTable();

                            }
                            catch (Exception)
                            {
                                TempData["ErrorMessage"] = "Unable to upload file!";
                                return View(UploadVehicle);
                            }

                            var NewModel = new UploadVehicleInfoModelV2();
                            NewModel.Table = NewDataTable;

                            if (NewDataTable.Rows.Count == 0)
                            {
                                TempData["ErrorMessage"] = "There's no data in that file!";
                                reader.Close();
                                reader.Dispose();

                                return View(NewModel);
                            }
                            //NEW - END
                            //List<VehicleInfoModel> UploadVehicleList = new List<VehicleInfoModel>();
                            foreach (DataRow item in NewDataTable.Rows.Cast<DataRow>().Skip(1))
                            {
                                ////NEW
                                //var VehicleMakeName = item.ItemArray[0].ToString()?.ToLower().Trim();
                                //var VehicleMakeID_rslt = db.VehicleMake.Where(o => o.Active == true && o.VehicleMakeName.ToLower().Contains(VehicleMakeName)).Select(o => o.VehicleMakeID).FirstOrDefault();
                                //if (VehicleMakeID_rslt == 0)
                                //    ModelState.AddModelError("VehicleTable[" + cnt + "].SelectedVehicleMakeID", "Vehicle Make data are not match from our data");

                                //var VehicleModelName = item.ItemArray[2].ToString()?.ToLower().Trim();
                                ////var VehicleModelID_rslt = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt && o.VehicleModelName.ToLower().Contains(VehicleModelName)).Select(o => o.VehicleModelID).FirstOrDefault();
                                //var VehicleModelID_rslt = (db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt)
                                //    .Select(o => new VehicleModelModel()
                                //    {
                                //        VehicleModelID = o.VehicleModelID,
                                //        VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                                //    }).OrderBy(o => o.VehicleModelID)).Where(o => o.VehicleModelName.ToLower().Contains(VehicleModelName)).Select(o => o.VehicleModelID).FirstOrDefault();
                                //if (VehicleModelID_rslt == 0)
                                //    ModelState.AddModelError("VehicleTable[" + cnt + "].SelectedVehicleModelID", "Vehicle Model data are not match from our data");

                                //var VehicleBodyTypeName = item.ItemArray[4].ToString()?.ToLower().Trim();
                                //var VehicleBodyTypeID_rslt = db.VehicleBodyType.Where(o => o.Active == true && o.VehicleBodyTypeName.ToLower().Contains(VehicleBodyTypeName)).Select(o => o.VehicleBodyTypeID).FirstOrDefault();
                                //if (VehicleBodyTypeID_rslt == 0)
                                //    ModelState.AddModelError("VehicleTable[" + cnt + "].SelectedVehicleBodyTypeID", "Vehicle Body Type data are not match from our data");

                                var EngineNumber_rslt = item.ItemArray[0].ToString()?.Trim();
                                var ChassisNumber_rslt = item.ItemArray[1].ToString()?.Trim();
                                var IsExisted = Functions.IsExistedEngineORChassis(EngineNumber_rslt, ChassisNumber_rslt);
                                if (UploadVehicle.VehicleTable.Where(o => o.EngineNumber == EngineNumber_rslt).Count() > 0)
                                    IsExisted.EngineNumber = EngineNumber_rslt;
                                if (UploadVehicle.VehicleTable.Where(o => o.ChassisNumber == ChassisNumber_rslt).Count() > 0)
                                    IsExisted.ChassisNumber = ChassisNumber_rslt;

                                if (IsExisted.EngineNumber != null)
                                    ModelState.AddModelError("VehicleTable[" + cnt + "].EngineNumber", "Vehicle Engine Number is existed");
                                if (IsExisted.ChassisNumber != null)
                                    ModelState.AddModelError("VehicleTable[" + cnt + "].ChassisNumber", "Vehicle Chassis Number is existed");

                                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                                {
                                    //var VehicleColorName = item.ItemArray[8].ToString()?.ToLower().Trim();
                                    //var VehicleColorID_rslt = db.VehicleColor.Where(o => o.Active == true && o.VehicleColorName.ToLower().Contains(VehicleColorName)).Select(o => o.VehicleColorID).FirstOrDefault();
                                    //if (VehicleColorName == "")
                                    //    VehicleColorID_rslt = 0;

                                    //List<VehicleModelModel> VehicleModelList_rslt = new List<VehicleModelModel>();
                                    //if (db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt).Select(o => o.VehicleModelID).Count() > 0)
                                    //    VehicleModelList_rslt = db.VehicleModel.Where(o => o.Active == true && o.VehicleMakeID == VehicleMakeID_rslt)
                                    //    .Select(o => new VehicleModelModel()
                                    //    {
                                    //        VehicleModelID = o.VehicleModelID,
                                    //        VehicleMakeID = o.VehicleMakeID,
                                    //        VehicleModelName = o.VehicleModelName + " - " + o.Variant,
                                    //        Variant = o.Variant,
                                    //        YearOfMake = o.YearOfMake,
                                    //        Active = o.Active,
                                    //        VehicleClassificationID = o.VehicleClassificationID,
                                    //    }).OrderBy(o => o.VehicleModelID).ToList();


                                    var TitleName = item.ItemArray[7].ToString()?.ToLower().Trim();
                                    var Title_rslt = 0;
                                    if (TitleName != "")
                                        Title_rslt = db.Title.Where(o => o.Active == true && o.TitleName.ToLower().Contains(TitleName)).Select(o => o.TitleID).FirstOrDefault();

                                    var CivilName = item.ItemArray[12].ToString()?.ToLower().Trim();
                                    var Civil_rslt = "";
                                    if (CivilName != "")
                                        Civil_rslt = db.CivilStatus.Where(o => o.Active == true && o.CivilStatusName.ToLower().Contains(CivilName)).Select(o => o.CivilStatusCode).FirstOrDefault();

                                    var SexName = item.ItemArray[13].ToString()?.ToLower().Trim();
                                    var Sex_rslt = "";
                                    if (SexName != "")
                                        Sex_rslt = db.Sex.Where(o => o.SexName.ToLower().Contains(SexName)).Select(o => o.SexCode).FirstOrDefault();

                                    var ProvinceName = item.ItemArray[24].ToString()?.ToLower().Trim();
                                    var Province_rslt = 0;
                                    if (ProvinceName != "")
                                        Province_rslt = db.Province.Where(o => o.Active == true && o.ProvinceName.ToLower().Contains(ProvinceName)).Select(o => o.ProvinceID).FirstOrDefault();

                                    var CityName = item.ItemArray[25].ToString()?.ToLower().Trim();
                                    var City_rslt = 0;
                                    if (CityName != "")
                                        City_rslt = db.City.Where(o => o.Active == true && o.ProvinceID == Province_rslt && o.CityName.ToLower().Contains(CityName)).Select(o => o.CityID).FirstOrDefault();
                                    var CityList_rslt = db.City.Where(o => o.Active == true && o.ProvinceID == Province_rslt).Select(o => new CityList_upload() { CityID = o.CityID, CityName = o.CityName }).ToList();

                                    var BarangayName = item.ItemArray[26].ToString()?.ToLower().Trim();
                                    var Barangay_rslt = 0;
                                    if (BarangayName != "")
                                        Barangay_rslt = db.Barangay.Where(o => o.Active == true && o.CityID == City_rslt && o.BarangayName.ToLower().Contains(BarangayName)).Select(o => o.BarangayID).FirstOrDefault();
                                    var BarangayList_rslt = db.Barangay.Where(o => o.Active == true && o.CityID == City_rslt).Select(o => new BarangayList_upload() { BarangayID = o.BarangayID, BarangayName = o.BarangayName }).ToList();

                                    DateTime? invoicedatefnl;
                                    DateTime? birthdatefnl;
                                    Decimal invoiceamount;
                                    try
                                    { 
                                        invoicedatefnl = Convert.ToDateTime(item.ItemArray[5].ToString().Trim());
                                    }
                                    catch
                                    {
                                        invoicedatefnl = null;
                                    }
                                    try 
                                    {
                                        birthdatefnl = Convert.ToDateTime(item.ItemArray[11].ToString().Trim());
                                    }
                                    catch
                                    {
                                        birthdatefnl = null;
                                    }
                                    try
                                    {
                                        invoiceamount = Convert.ToDecimal(item.ItemArray[6].ToString().Trim());
                                    }
                                    catch
                                    {
                                        invoiceamount = 0;
                                    }
                                    UploadVehicle.VehicleTableV2.Add(new UploadTableV2()
                                    {
                                        //VehicleMakeID = VehicleMakeID_rslt,
                                        //SelectedVehicleMakeID = VehicleMakeID_rslt,
                                        //VehicleModelID = VehicleModelID_rslt,
                                        //SelectedVehicleModelID = VehicleModelID_rslt,
                                        //VehicleBodyTypeID = VehicleBodyTypeID_rslt,
                                        //SelectedVehicleBodyTypeID = VehicleBodyTypeID_rslt,
                                        EngineNumber = EngineNumber_rslt,
                                        ChassisNumber = ChassisNumber_rslt,
                                        //VehicleColorID = VehicleColorID_rslt,
                                        //SelectedVehicleColorID = VehicleColorID_rslt,
                                        //GrossVehicleWeight = item.ItemArray[10].ToString().Trim(),
                                        CSRNumber = item.ItemArray[2].ToString().Trim(),
                                        HPGNumber = item.ItemArray[3].ToString().Trim(),
                                        InvoiceNumber = item.ItemArray[4].ToString().Trim(),
                                        InvoiceDate = invoicedatefnl,
                                        InvoiceAmount = invoiceamount,
                                        TitleID = Title_rslt,
                                        LastName = item.ItemArray[8].ToString().Trim(),
                                        FirstName = item.ItemArray[9].ToString().Trim(),
                                        MiddleName = item.ItemArray[10].ToString().Trim(),
                                        Birthdate = birthdatefnl,
                                        CivilStatusCode = Civil_rslt,
                                        SexCode = Sex_rslt,
                                        ContactNumber = item.ItemArray[14].ToString().Trim(),
                                        EmailAddress = item.ItemArray[15].ToString().Trim(),
                                        TIN = item.ItemArray[16].ToString().Trim(),
                                        OrgName = item.ItemArray[17].ToString().Trim(),
                                        OrgMnemonic = item.ItemArray[18].ToString().Trim(),
                                        PrimaryContact = item.ItemArray[19].ToString().Trim(),
                                        ContactDetails = item.ItemArray[20].ToString().Trim(),
                                        EmailAddressOrg = item.ItemArray[21].ToString().Trim(),
                                        PhoneNo = item.ItemArray[22].ToString().Trim(),
                                        TINOrg = item.ItemArray[23].ToString().Trim(),
                                        ProvinceID = Province_rslt,
                                        CityID = City_rslt,
                                        BarangayID = Barangay_rslt,
                                        HouseBldgNumber = item.ItemArray[27].ToString().Trim(),
                                        Street = item.ItemArray[28].ToString().Trim(),
                                        ZipCode = item.ItemArray[29].ToString().Trim(),
                                        //VehicleModelList = VehicleModelList_rslt,
                                        Citylist = CityList_rslt,
                                        Barangaylist = BarangayList_rslt,
                                    });
                                }
                                cnt++;
                            }

                            //NEW 
                            if (db.BatchMaster.Where(o => o.ReferenceNo == UploadVehicle.BatchHeader.ReferenceNo && o.BatchTypeID == UploadVehicle.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                            {
                                ModelState.AddModelError("BatchHeader.ReferenceNo", "Batch reference number is existed");
                            }

                            //UploadVehicle.VehicleMakeList = db.vwDealerVehicleMake.Where(o => o.DealerID == refid && o.Active == true).ToList();
                            //UploadVehicle.VehicleBodyTypeList = db.VehicleBodyType.Where(o => o.Active == true).ToList();
                            //UploadVehicle.VehicleColorList = db.VehicleColor.Where(o => o.Active == true).ToList();
                            //UploadVehicle.VehicleFuelTypeList = db.VehicleFuelType.Where(o => o.Active == true).ToList();
                            //UploadVehicle.VehicleAirconTypeList = db.AirconType.Where(o => o.Active == true).ToList();
                            UploadVehicle.TitleList = db.Title.Where(o => o.Active == true).ToList();
                            UploadVehicle.CivilStatusList = db.CivilStatus.Where(o => o.Active == true).ToList();
                            UploadVehicle.SexList = db.Sex.ToList();
                            UploadVehicle.ProvinceList = db.Province.Where(o => o.Active == true).ToList();

                            reader.Close();
                            reader.Dispose();

                            return View(UploadVehicle);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Please upload your file!";
                        }
                    }
                    return View(UploadVehicle);
                    break;
            }
            return View(UploadVehicle);
        }

        [HttpPost]
        public FileResult VehicleInfo_templateV2()
        {
            //DataTable dealer_dt = new DataTable();
            //DataTable dealerbranch_dt = new DataTable();

            //DataTable make_dt = new DataTable();
            //DataTable model_dt = new DataTable();
            //DataTable color_dt = new DataTable();
            //DataTable body_dt = new DataTable();

            DataTable Title_dt = new DataTable();
            DataTable Civil_dt = new DataTable();
            DataTable Sex_dt = new DataTable();

            DataTable Province_dt = new DataTable();
            DataTable City_dt = new DataTable();
            DataTable Barangay_dt = new DataTable();

            string path = "";
            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
            {
                path = Server.MapPath("~/UploadTemplate/UploadInfo-Template.xlsx");

                //var vehicleMake = db.vwDealerVehicleMake.Where(o => o.DealerID == CurrentUser.Details.ReferenceID && o.Active == true).Select(x => new { x.VehicleMakeID, x.VehicleMakeName }).ToList();
                //var vehicleModel = db.VehicleModel.Where(o => o.Active == true).Select(x => new { x.VehicleMakeID, x.VehicleModelID, x.VehicleModelName, x.Variant }).OrderBy(o => o.VehicleMakeID).ThenBy(o => o.VehicleModelName).ToList();
                //var vehicleBody = db.VehicleBodyType.Where(o => o.Active == true).Select(x => new { x.VehicleBodyTypeID, x.VehicleBodyTypeName }).ToList(); 
                //var vehicleColor = db.VehicleColor.Where(o => o.Active == true).Select(x => new { x.VehicleColorID, x.VehicleColorName }).ToList();

                var Buyer_Title = db.Title.Where(o => o.Active == true).Select(x => new { x.TitleID, x.TitleTypeID, x.TitleName }).ToList();
                var Buyer_Civil = db.CivilStatus.Where(o => o.Active == true).Select(x => new { x.CivilStatusCode, x.CivilStatusName }).ToList();
                var Buyer_Sex = db.Sex.ToList();

                var Buyer_Province = db.Province.Where(o => o.Active == true).Select(x => new { x.ProvinceID, x.ProvinceName }).OrderBy(o => o.ProvinceName).ToList();
                var Buyer_City = db.vwProvinceCity.Select(x => new { x.ProvinceName, x.CityID, x.CityName }).OrderBy(o => o.ProvinceName).ThenBy(o => o.CityName).ToList();
                var Buyer_Barangay = (from a in db.Barangay join b in db.City on a.CityID equals b.CityID where a.Active == true && b.Active == true select new {  b.CityName, a.BarangayName, a.BarangayID }).OrderBy(o => o.CityName).ThenBy(o => o.BarangayName).ToList();

                //make_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleMakeID"),
                //                            new DataColumn("VehicleMakeName")});
                //model_dt.Columns.AddRange(new DataColumn[3] { new DataColumn("VehicleMakeID"),
                //                            new DataColumn("VehicleModelName"),
                //                            new DataColumn("VehicleModelID")});
                //body_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleBodyTypeName"),
                //                            new DataColumn("VehicleBodyTypeID")});
                //color_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("VehicleColorName"),
                //                            new DataColumn("VehicleColorID")});

                Title_dt.Columns.AddRange(new DataColumn[3] { new DataColumn("TitleTypeID"),
                                            new DataColumn("TitleName"),
                                            new DataColumn("TitleID")});
                Civil_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("CivilStatusName"),
                                            new DataColumn("CivilStatusCode")});
                Sex_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("SexName"),
                                            new DataColumn("SexCode")});

                Province_dt.Columns.AddRange(new DataColumn[2] { new DataColumn("ProvinceName"),
                                            new DataColumn("ProvinceID")});
                City_dt.Columns.AddRange(new DataColumn[3] { new DataColumn("ProvinceName"),
                                            new DataColumn("CityName"),
                                            new DataColumn("CityID")});
                Barangay_dt.Columns.AddRange(new DataColumn[3] { new DataColumn("CityName"),
                                            new DataColumn("BarangayName"),
                                            new DataColumn("BarangayID")});

                //foreach (var list in vehicleMake)
                //{
                //    make_dt.Rows.Add(list.VehicleMakeName, list.VehicleMakeID);
                //}
                //foreach (var list in vehicleModel)
                //{
                //    model_dt.Rows.Add(list.VehicleMakeID, list.VehicleModelName + " - " + list.Variant, list.VehicleModelID);
                //}
                //foreach (var list in vehicleBody)
                //{
                //    body_dt.Rows.Add(list.VehicleBodyTypeName, list.VehicleBodyTypeID);
                //}
                //foreach (var list in vehicleColor)
                //{
                //    color_dt.Rows.Add(list.VehicleColorName, list.VehicleColorID);
                //}


                foreach (var list in Buyer_Title)
                {
                    Title_dt.Rows.Add(list.TitleTypeID, list.TitleName, list.TitleID);
                }
                foreach (var list in Buyer_Civil)
                {
                    Civil_dt.Rows.Add(list.CivilStatusName, list.CivilStatusCode);
                }
                foreach (var list in Buyer_Sex)
                {
                    Sex_dt.Rows.Add(list.SexName, list.SexCode);
                }

                foreach (var list in Buyer_Province)
                {
                    Province_dt.Rows.Add(list.ProvinceName, list.ProvinceID);
                }
                foreach (var list in Buyer_City)
                {
                    City_dt.Rows.Add(list.ProvinceName, list.CityName, list.CityID);
                }
                foreach (var list in Buyer_Barangay)
                {
                    Barangay_dt.Rows.Add(list.CityName, list.BarangayName, list.BarangayID);
                }
            }

            using (XLWorkbook wb = new XLWorkbook(path))
            {

                if (wb.Worksheets.Count() > 1)
                {
                    wb.Worksheet("List").Delete();
                }

                var ws = wb.Worksheets.Add("List").Hide();
                //if (make_dt.Rows.Count > 0)
                //{
                //    ws.Cell(1, 1).InsertTable(make_dt.AsEnumerable());
                //}
                //if (model_dt.Rows.Count > 0)
                //{
                //    ws.Cell(1, 4).InsertTable(model_dt.AsEnumerable());
                //}
                //if (body_dt.Rows.Count > 0)
                //{
                //    ws.Cell(1, 8).InsertTable(body_dt.AsEnumerable());
                //}
                //if (color_dt.Rows.Count > 0)
                //{
                //    ws.Cell(1, 11).InsertTable(color_dt.AsEnumerable());
                //}

                if (Title_dt.Rows.Count > 0)
                {
                    ws.Cell(1, 1).InsertTable(Title_dt.AsEnumerable());
                }
                if (Civil_dt.Rows.Count > 0)
                {
                    ws.Cell(1, 5).InsertTable(Civil_dt.AsEnumerable());
                }
                if (Sex_dt.Rows.Count > 0)
                {
                    ws.Cell(1, 8).InsertTable(Sex_dt.AsEnumerable());
                }

                if (Province_dt.Rows.Count > 0)
                {
                    ws.Cell(1, 11).InsertTable(Province_dt.AsEnumerable());
                }
                if (City_dt.Rows.Count > 0)
                {
                    ws.Cell(1, 14).InsertTable(City_dt.AsEnumerable());
                }
                if (Barangay_dt.Rows.Count > 0)
                {
                    ws.Cell(1, 18).InsertTable(Barangay_dt.AsEnumerable());
                }

                //wb.Save();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UploadInfo-Template.xlsx");
                }
            }
        }
        #endregion

        #region [ Private Functions ]
        private bool InsertVehicleInfo(BatchHeaderModel BatchHeader, params VehicleInfoModel[] NewVehicleList)
        {
            bool success = false;
            int VehicleNumber = 0;
            using (db = new VRSystemEntities())
            {
                var InsertVehicle = new List<VehicleInfo>();
                foreach (var NewVehicle in NewVehicleList)
                {
                    try
                    {
                        var createdbydealer = false;
                        if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                        {
                            createdbydealer = true;
                        };

                        InsertVehicle.Add(new VehicleInfo
                        {
                            VehicleMakeID = NewVehicle.SelectedVehicleMakeID,
                            VehicleModelID = NewVehicle.SelectedVehicleModelID,
                            VehicleBodyTypeID = NewVehicle.SelectedVehicleBodyTypeID,
                            VehicleColorID = NewVehicle.SelectedVehicleColorID ?? 0,
                            AirconType = NewVehicle.AirconType,
                            ConductionSticker = NewVehicle.ConductionSticker,
                            EngineNumber = NewVehicle.EngineNumber,
                            PistonDisplacement = NewVehicle.PistonDisplacement,
                            VehicleFuelTypeID = NewVehicle.SelectedVehicleFuelTypeID ?? 0,
                            Cylinders = NewVehicle.Cylinders,
                            ChassisNumber = NewVehicle.ChassisNumber,
                            Series = NewVehicle.Series,
                            BodyIDNumber = NewVehicle.BodyIDNumber,
                            Year = NewVehicle.Year,
                            GrossVehicleWeight = NewVehicle.GrossVehicleWeight,
                            FrontTiresNumber = NewVehicle.FrontTiresNumber,
                            RearTiresNumber = NewVehicle.RearTiresNumber,
                            COCNo = NewVehicle.COCNo,
                            CSRNumber = NewVehicle.CSRNumber,
                            HPGControlNumber = NewVehicle.HPGControlNumber,
                            MAIID = NewVehicle.SelectedMAID,

                            DealerID = NewVehicle.SelectedDealer,
                            DealerBranchID = NewVehicle.SelectedDealerBranch,
                            Assigned = createdbydealer,

                            ReportDate = DateTime.Now,
                            DatePrepared = DateTime.Now,
                            CreatedBy = CurrentUser.Details.UserID,
                            CreatedDate = DateTime.Now,
                            Active = true
                        });
                        VehicleNumber++;

                        //if (TryValidateModel(NewVehicle))
                        //{
                        //    var createdbydealer = false;
                        //    if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                        //    {
                        //        createdbydealer = true;
                        //    };

                        //    InsertVehicle.Add(new VehicleInfo
                        //    {
                        //        VehicleMakeID = NewVehicle.SelectedVehicleMakeID,
                        //        VehicleModelID = NewVehicle.SelectedVehicleModelID,
                        //        VehicleBodyTypeID = NewVehicle.SelectedVehicleBodyTypeID,
                        //        VehicleColorID = NewVehicle.SelectedVehicleColorID?? 0,
                        //        AirconType = NewVehicle.AirconType,
                        //        ConductionSticker = NewVehicle.ConductionSticker,
                        //        EngineNumber = NewVehicle.EngineNumber,
                        //        PistonDisplacement = NewVehicle.PistonDisplacement,
                        //        VehicleFuelTypeID = NewVehicle.SelectedVehicleFuelTypeID?? 0,
                        //        Cylinders = NewVehicle.Cylinders,
                        //        ChassisNumber = NewVehicle.ChassisNumber,
                        //        Series = NewVehicle.Series,
                        //        BodyIDNumber = NewVehicle.BodyIDNumber,
                        //        Year = NewVehicle.Year,
                        //        GrossVehicleWeight = NewVehicle.GrossVehicleWeight,
                        //        FrontTiresNumber = NewVehicle.FrontTiresNumber,
                        //        RearTiresNumber = NewVehicle.RearTiresNumber,
                        //        COCNo = NewVehicle.COCNo,
                        //        MAIID = NewVehicle.SelectedMAID,

                        //        DealerID = NewVehicle.SelectedDealer,
                        //        DealerBranchID = NewVehicle.SelectedDealerBranch,
                        //        Assigned = createdbydealer,

                        //        ReportDate = DateTime.Now,
                        //        DatePrepared = DateTime.Now,
                        //        CreatedBy = CurrentUser.Details.UserID,
                        //        CreatedDate = DateTime.Now,
                        //        Active = true
                        //    });
                        //    VehicleNumber++;
                        //}
                        //else
                        //break;
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                }

                if (VehicleNumber == NewVehicleList.Count())
                {
                    db.VehicleInfo.AddRange(InsertVehicle);
                    db.SaveChanges();
                    var test = InsertVehicle[1].VehicleID;
                    //Batch Header Insert
                    var batchID = BatchHeaderInsert(BatchHeader, InsertVehicle.Count);
                    if (batchID != null)
                    {
                        //BatchList
                        List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                        //Batch add to list
                        foreach (var list in InsertVehicle)
                        {
                            BatchDetailsList.Add(new BatchDetails
                            {
                                BatchID = Convert.ToInt32(batchID),
                                VehicleID = list.VehicleID,
                                TransactionID = null
                            });
                        }
                        //Batch Details Insert
                        BatchDetailsInsert(BatchDetailsList);
                    }
                    success = true;
                }
            }

            return success;
        }
        #endregion

        public ActionResult VehicleBodyList()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var vehiclebodylist = db.VehicleBodyType.Where(o => o.Active == true).ToList();

                return View(vehiclebodylist);
            }
        }
        [HttpGet]
        public ActionResult VehicleBody(int? id)
        {

            VehicleBodyType vehiclebody = new VehicleBodyType();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var Load = db.VehicleBodyType.Where(o => o.Active == true && o.VehicleBodyTypeID == id).ToList().FirstOrDefault();

                    vehiclebody.VehicleBodyTypeID = Load.VehicleBodyTypeID;
                    vehiclebody.VehicleBodyAbbr = Load.VehicleBodyAbbr;
                    vehiclebody.VehicleBodyTypeName = Load.VehicleBodyTypeName;
                }
                return View(vehiclebody);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VehicleBody(VehicleBodyType Vehiclebody, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewVehiclebody = new VehicleBodyType
                            {
                                VehicleBodyTypeID = Vehiclebody.VehicleBodyTypeID,
                                VehicleBodyAbbr = Vehiclebody.VehicleBodyAbbr.Trim(),
                                VehicleBodyTypeName = Vehiclebody.VehicleBodyTypeName.Trim(),
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            db.VehicleBodyType.Add(NewVehiclebody);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.VehicleBodyType.Where(o => o.VehicleBodyTypeID == Vehiclebody.VehicleBodyTypeID).FirstOrDefault();
                            Update.VehicleBodyTypeID = Vehiclebody.VehicleBodyTypeID;
                            Update.VehicleBodyAbbr = Vehiclebody.VehicleBodyAbbr.Trim();
                            Update.VehicleBodyTypeName = Vehiclebody.VehicleBodyTypeName.Trim();
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update_active = db.VehicleBodyType.Where(o => o.VehicleBodyTypeID == Vehiclebody.VehicleBodyTypeID).FirstOrDefault();
                            Update_active.Active = false;
                            Update_active.UpdatedBy = CurrentUser.Details.UserID;
                            Update_active.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Sucessfully!";
                        }
                        break;
                }
                return RedirectToAction("VehicleBodyList");
            }
            else
            {
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(Vehiclebody);
            }

        }

        [HttpGet]
        public ActionResult VehicleInfo(int id)
        {
            ViewBag.WhatForm = "VehicleInfo";
            var Vehicle = GetVehicleInfo(id);
            switch (CurrentUser.Details.UserEntityID)
            {
                case (int)UserEntityEnum.Dealer:
                    {
                        if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                        {
                            if (CurrentUser.Details.ReferenceID != Vehicle.DealerID)
                            {
                                TempData["ErrorMessage"] = "Vehicle does not exist";
                                return RedirectToAction("Index");
                            }

                        }
                        else
                        {
                            if (CurrentUser.Details.ReferenceID != Vehicle.DealerID && CurrentUser.Details.SubReferenceID != Vehicle.DealerBranchID)
                            {
                                TempData["ErrorMessage"] = "Vehicle does not exist";
                                return RedirectToAction("Index");
                            }
                        }
                        break;
                    }
                case (int)UserEntityEnum.MAI:
                    {
                        if (CurrentUser.Details.SubReferenceID != Vehicle.MAIID)
                        {
                            TempData["ErrorMessage"] = "Vehicle does not exist";
                            return RedirectToAction("Index");
                        }
                        break;
                    }
            }

            return View(Vehicle);
        }

        [HttpPost]
        public ActionResult VehicleInfo(VehicleInfoModel LoadVehicle, string submit)
        {
            ViewBag.WhatForm = "VehicleInfo";
            var Vehicle = GetVehicleInfo(LoadVehicle.VehicleID);

            switch (CurrentUser.Details.UserEntityID)
            {
                case (int)UserEntityEnum.Dealer:
                    {
                        if (CurrentUser.Details.IsMain == true && CurrentUser.Details.UserRoleID == (int)UserRoleEnum.Administrator)
                        {
                            if (CurrentUser.Details.ReferenceID != Vehicle.DealerID)
                            {
                                TempData["ErrorMessage"] = "Vehicle does not exist";
                                return RedirectToAction("Index");
                            }

                        }
                        else
                        {
                            if (CurrentUser.Details.ReferenceID != Vehicle.DealerID && CurrentUser.Details.SubReferenceID != Vehicle.DealerBranchID)
                            {
                                TempData["ErrorMessage"] = "Vehicle does not exist";
                                return RedirectToAction("Index");
                            }
                        }
                        break;
                    }
                case (int)UserEntityEnum.MAI:
                    {
                        if (CurrentUser.Details.SubReferenceID != Vehicle.MAIID)
                        {
                            TempData["ErrorMessage"] = "Vehicle does not exist";
                            return RedirectToAction("Index");
                        }
                        break;
                    }
            }

            using (db = new VRSystemEntities())
            {
                try
                {
                    List<int> VehicleIDList = new List<int>();
                    VehicleIDList.Add(Vehicle.VehicleID);
                    switch (submit)
                    {
                        case "Reject":
                            {
                                var UpdateBatchDetail = db.BatchDetails.Where(o => o.BatchID == Vehicle.BatchID && o.VehicleID == Vehicle.VehicleID).FirstOrDefault();

                                if (UpdateBatchDetail != null)
                                {
                                    UpdateBatchDetail.Rejected = true;
                                    UpdateBatchDetail.RejectedRemarks = LoadVehicle.RejectedRemarks;
                                    UpdateBatchDetail.RejectedBy = CurrentUser.Details.UserID;
                                    UpdateBatchDetail.RejectedDate = DateTime.Now;
                                    db.SaveChanges();

                                    TempData["WarningMessage"] = "Vehicle Rejected!";
                                }
                                break;
                            }

                        case "Complete":
                            {
                                var UpdateBatchDetail = db.BatchDetails.Where(o => o.BatchID == Vehicle.BatchID && o.VehicleID == Vehicle.VehicleID).FirstOrDefault();

                                if (UpdateBatchDetail != null)
                                {
                                    UpdateBatchDetail.Completed = true;
                                    UpdateBatchDetail.CompletedBy = CurrentUser.Details.UserID;
                                    UpdateBatchDetail.CompletedDate = DateTime.Now;

                                    db.SaveChanges();
                                    TempData["SuccessMessage"] = "Marked as Complete!";
                                }
                                break;
                            }
                        case "BOC":
                            {
                                LoadVehicle.BatchHeader.BatchTypeID = (int)BatchTypeList.BOC;
                                ModelState.Clear();
                                if (db.BatchMaster.Where(o => o.ReferenceNo == LoadVehicle.BOCInfo.BatchHeader.ReferenceNo && o.BatchTypeID == LoadVehicle.BOCInfo.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                                {
                                    TempData["ErrorMessage"] = "Batch reference number is existed!";
                                }
                                else if (TryValidateModel(LoadVehicle.BOCInfo))
                                {
                                    //var newvehiclelist = new List<vwVehicleListModel>();
                                    //newvehiclelist.Add(new vwVehicleListModel() {
                                    //    isChecked = true,
                                    //    VehicleID = LoadVehicle.VehicleID
                                    //});
                                    //BatchMaster(LoadVehicle.BOCInfo.BatchHeader, newvehiclelist, null);

                                    var UpdateBOC = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();

                                    if (LoadVehicle.BOCInfo.BOCFile.IsAllowedContentType())
                                    {
                                        UpdateBOC.BOCCertificateOfPayment = LoadVehicle.BOCInfo.BOCFile.ToByte();
                                        UpdateBOC.BOCContentType = LoadVehicle.BOCInfo.BOCFile.ContentType;
                                    }

                                    if (LoadVehicle.BOCInfo.BOC2File.IsAllowedContentType())
                                    {
                                        UpdateBOC.BOCCertificateOfPayment2 = LoadVehicle.BOCInfo.BOC2File.ToByte();
                                        UpdateBOC.BOCContentType2 = LoadVehicle.BOCInfo.BOC2File.ContentType;
                                    }

                                    UpdateBOC.CPNumber = LoadVehicle.BOCInfo.CPNumber;
                                    UpdateBOC.DateIssued1 = LoadVehicle.BOCInfo.DateIssued1;

                                    UpdateBOC.CPNumber2 = LoadVehicle.BOCInfo.CPNumber2;
                                    UpdateBOC.DateIssued2 = LoadVehicle.BOCInfo.DateIssued2;

                                    UpdateBOC.InformalEntryNumberEngine = LoadVehicle.BOCInfo.InformalEntryNumberEngine;
                                    UpdateBOC.InformalEntryNumberChassis = LoadVehicle.BOCInfo.InformalEntryNumberChassis;

                                    UpdateBOC.UpdatedBy = CurrentUser.Details.UserID;
                                    UpdateBOC.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();

                                    //Batch Header Insert
                                    var batchID = BatchHeaderInsert(LoadVehicle.BOCInfo.BatchHeader, 1);
                                    if (batchID != null)
                                    {
                                        //BatchList
                                        List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                                        //Batch add to list
                                        BatchDetailsList.Add(new BatchDetails
                                        {
                                            BatchID = Convert.ToInt32(batchID),
                                            VehicleID = UpdateBOC.VehicleID,
                                            TransactionID = null
                                        });
                                        BatchDetailsInsert(BatchDetailsList);
                                    }

                                    TempData["SuccessMessage"] = "Upload successful!";
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "Upload error!";
                                }
                                Vehicle = GetVehicleInfo(LoadVehicle.VehicleID);
                                return View(Vehicle);
                            }
                        case "Allocate":
                            {
                                ModelState.Clear();
                                if (TryValidateModel(LoadVehicle.DealerInfo))
                                {
                                    if (AllocateVehicle(LoadVehicle.VehicleID, LoadVehicle.DealerInfo.SelectedDealer, LoadVehicle.DealerInfo.SelectedDealerBranch))
                                    {
                                        TempData["SuccessMessage"] = "Allocated Successful!";
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "An error has occured!";
                                    }
                                }
                                Vehicle = GetVehicleInfo(LoadVehicle.VehicleID);
                                return View(Vehicle);

                            }
                        case "CSR":
                            {
                                if (!CheckAvailableBalance(1, (UserEntityEnum)CurrentUser.Details.UserEntityID,
                            CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI ?
                            (int)CurrentUser.Details.SubReferenceID : (int)CurrentUser.Details.ReferenceID))
                                {
                                    TempData["ErrorMessage"] = "Wallet balance is not enough";
                                    return View(Vehicle);
                                }
                                else if (db.BatchMaster.Where(o => o.ReferenceNo == LoadVehicle.BOCInfo.BatchHeader.ReferenceNo && o.BatchTypeID == LoadVehicle.BOCInfo.BatchHeader.BatchTypeID).FirstOrDefault() != null)
                                {
                                    //BatchMaster Validation
                                    TempData["ErrorMessage"] = "Batch reference number is existed!";
                                }

                                TempData["BatchHeader"] = LoadVehicle.CSRInfo.BatchHeader;
                                if (CSRSumbit(VehicleIDList))
                                {
                                    TempData["SuccessMessage"] = "CSR Application is successful!";
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "An error has occured!";
                                }
                                Vehicle = GetVehicleInfo(LoadVehicle.VehicleID);
                                return View(Vehicle);
                            }
                        case "DealerCSR":
                            {
                                ModelState.Clear();
                                if (TryValidateModel(LoadVehicle.CSRInfo) || ViewBag.CSR_Edit)
                                {
                                    if (LoadVehicle.CSRInfo.CSRFile.IsAllowedContentType() && LoadVehicle.CSRInfo.CSRFile.IsValidFileSize())
                                    {
                                        var UpdateCSR = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                        if (LoadVehicle.CSRInfo.CSRFile.IsAllowedContentType())
                                        {
                                            UpdateCSR.CertificateOfStockReport = LoadVehicle.CSRInfo.CSRFile.ToByte();
                                            UpdateCSR.CSRContentType = LoadVehicle.CSRInfo.CSRFile.ContentType;
                                        }

                                        UpdateCSR.TransactionID = LoadVehicle.CSRInfo.TransactionID;
                                        UpdateCSR.CSRNumber = LoadVehicle.CSRInfo.CSRNumber;

                                        UpdateCSR.ReportEntryID = LoadVehicle.CSRInfo.ReportEntryID;
                                        UpdateCSR.ReportDate = LoadVehicle.CSRInfo.ReportDate;
                                        UpdateCSR.ItemType = LoadVehicle.CSRInfo.ItemType;

                                        UpdateCSR.BIRCCMV = LoadVehicle.CSRInfo.BIRCCMV;
                                        UpdateCSR.DateIssued3 = LoadVehicle.CSRInfo.DateIssued3;

                                        UpdateCSR.TaxType = LoadVehicle.CSRInfo.TaxType;
                                        UpdateCSR.TaxAmount = LoadVehicle.CSRInfo.TaxAmount;

                                        UpdateCSR.UpdatedBy = CurrentUser.Details.UserID;
                                        UpdateCSR.UpdatedDate = DateTime.Now;
                                        db.SaveChanges();

                                        //BatchList
                                        //List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                                        //var batchID = BatchHeaderInsert(LoadVehicle.CSRInfo.BatchHeader, VehicleIDList.Count);
                                        //if (batchID != null)
                                        //{
                                        //    //Batch Details Insert
                                        //    BatchDetailsList.Add(new BatchDetails
                                        //    {
                                        //        VehicleID = UpdateCSR.VehicleID,
                                        //        BatchID = Convert.ToInt32(batchID),
                                        //        TransactionID = null
                                        //    });
                                        //    BatchDetailsInsert(BatchDetailsList);
                                        //}
                                        TempData["SuccessMessage"] = "CSR information has been submitted!";
                                    }
                                    else if (!LoadVehicle.CSRInfo.CSRFile.IsValidFileSize())
                                    {
                                        TempData["WarningMessage"] = "Please upload valid file size of less than 1 MB!";
                                    }
                                    else if (!LoadVehicle.CSRInfo.CSRFile.IsAllowedContentType())
                                    {
                                        TempData["WarningMessage"] = "Only .JPG, .JPEG, .PNG & PDF file type's are allowed.";
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "An error has occured.";
                                    }
                                }
                                Vehicle = GetVehicleInfo(LoadVehicle.VehicleID);
                                return View(Vehicle);
                            }
                        case "Dealer":
                            {
                                ModelState.Clear();
                                if (TryValidateModel(LoadVehicle.DealerInfo))
                                {

                                    if (AssigningDealers(LoadVehicle.VehicleID, LoadVehicle.DealerInfo.SelectedDealer, LoadVehicle.DealerInfo.SelectedDealerBranch))
                                    {
                                        TempData["SuccessMessage"] = "Dealer has been assign!";
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "An error has occured!";
                                    }
                                    //var UpdateDealer = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                    //UpdateDealer.DealerID = LoadVehicle.DealerInfo.SelectedDealer;
                                    //UpdateDealer.DealerBranchID = LoadVehicle.DealerInfo.SelectedDealerBranch;
                                    //UpdateDealer.Assigned = true;
                                    //UpdateDealer.UpdatedBy = CurrentUser.Details.UserID;
                                    //UpdateDealer.UpdatedDate = DateTime.Now;
                                    //db.SaveChanges();

                                    //TempData["SuccessMessage"] = "Dealer Assigned!";

                                }
                                Vehicle = GetVehicleInfo(LoadVehicle.VehicleID);
                                return View(Vehicle);

                            }
                        case "upload":
                            {
                                if (LoadVehicle.BOCFile.IsAllowedContentType())
                                {
                                    var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                    UploadFile.BOCCertificateOfPayment = LoadVehicle.BOCFile.ToByte();
                                    UploadFile.BOCContentType = LoadVehicle.BOCFile.ContentType;
                                    UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                    UploadFile.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    Vehicle.BOCCertificateOfPayment = UploadFile.BOCCertificateOfPayment;
                                    Vehicle.BOCContentType = UploadFile.BOCContentType;
                                    TempData["SuccessMessage"] = "Upload successful!";
                                    return View(Vehicle);
                                }
                                else if (LoadVehicle.BOCFile != null)
                                {
                                    ModelState.AddModelError("BOCFile", "Only .JPG, .JPEG, .PNG & PDF file types are allowed.");
                                }

                                if (LoadVehicle.BOC2File.IsAllowedContentType())
                                {
                                    var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                    UploadFile.BOCCertificateOfPayment2 = LoadVehicle.BOC2File.ToByte();
                                    UploadFile.BOCContentType2 = LoadVehicle.BOC2File.ContentType;
                                    UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                    UploadFile.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    Vehicle.BOCCertificateOfPayment2 = UploadFile.BOCCertificateOfPayment2;
                                    Vehicle.BOCContentType2 = UploadFile.BOCContentType2;
                                    TempData["SuccessMessage"] = "Upload successful!";
                                    return View(Vehicle);
                                }
                                else if (LoadVehicle.BOC2File != null)
                                {
                                    ModelState.AddModelError("BOC2File", "Only .JPG, .JPEG, .PNG & PDF file types are allowed.");
                                }

                                if (LoadVehicle.CSRFile.IsAllowedContentType())
                                {
                                    var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                    UploadFile.CertificateOfStockReport = LoadVehicle.CSRFile.ToByte();
                                    UploadFile.CSRContentType = LoadVehicle.CSRFile.ContentType;
                                    UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                    UploadFile.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    Vehicle.CertificateOfStockReport = UploadFile.CertificateOfStockReport;
                                    Vehicle.CSRContentType = UploadFile.CSRContentType;
                                    TempData["SuccessMessage"] = "Upload successful!";
                                    return View(Vehicle);
                                }
                                else if (LoadVehicle.CSRFile != null)
                                {
                                    ModelState.AddModelError("CSRFile", "Only .JPG, .JPEG, .PNG & PDF file types are allowed.");
                                }

                                if (LoadVehicle.COCFile.IsAllowedContentType())
                                {
                                    var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                    UploadFile.CertificateOfConformity = LoadVehicle.COCFile.ToByte();
                                    UploadFile.COCContentType = LoadVehicle.COCFile.ContentType;
                                    UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                    UploadFile.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    Vehicle.CertificateOfConformity = UploadFile.CertificateOfConformity;
                                    Vehicle.COCContentType = UploadFile.COCContentType;
                                    TempData["SuccessMessage"] = "Upload successful!";
                                    return View(Vehicle);
                                }
                                else if (LoadVehicle.COCFile != null)
                                {
                                    ModelState.AddModelError("COCFile", "Only .JPG, .JPEG, .PNG & PDF file types are allowed.");
                                }

                                if (LoadVehicle.SOEFile.IsAllowedContentType())
                                {
                                    var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                    UploadFile.StencilOfEngine = LoadVehicle.SOEFile.ToByte();
                                    UploadFile.SOEContentType = LoadVehicle.SOEFile.ContentType;
                                    UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                    UploadFile.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    Vehicle.StencilOfEngine = UploadFile.StencilOfEngine;
                                    Vehicle.SOEContentType = UploadFile.SOEContentType;
                                    TempData["SuccessMessage"] = "Upload successful!";
                                    return View(Vehicle);
                                }
                                else if (LoadVehicle.SOEFile != null)
                                {
                                    ModelState.AddModelError("SOEFile", "Only .JPG, .JPEG, .PNG & PDF file types are allowed.");
                                }

                                if (LoadVehicle.SOCFile.IsAllowedContentType())
                                {
                                    var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                    UploadFile.StencilOfChasis = LoadVehicle.SOCFile.ToByte();
                                    UploadFile.SOCContentType = LoadVehicle.SOCFile.ContentType;
                                    UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                    UploadFile.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    Vehicle.StencilOfChasis = UploadFile.StencilOfChasis;
                                    Vehicle.SOCContentType = UploadFile.SOCContentType;
                                    TempData["SuccessMessage"] = "Upload successful!";
                                    return View(Vehicle);
                                }
                                else if (LoadVehicle.SOCFile != null)
                                {
                                    ModelState.AddModelError("SOCFile", "Only .JPG, .JPEG, .PNG & PDF file types are allowed.");
                                }

                                //if (LoadVehicle.PNPFile.IsAllowedContentType())
                                //{
                                //    var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                //    UploadFile.PNPClearance = LoadVehicle.PNPFile.ToByte();
                                //    UploadFile.PNPContentType = LoadVehicle.PNPFile.ContentType;
                                //    UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                //    UploadFile.UpdatedDate = DateTime.Now;
                                //    db.SaveChanges();
                                //    Vehicle.PNPClearance = UploadFile.PNPClearance;
                                //    Vehicle.PNPContentType = UploadFile.PNPContentType;
                                //    TempData["SuccessMessage"] = "Upload successful!";
                                //    return View(Vehicle);
                                //}
                                //else if (LoadVehicle.PNPFile != null)
                                //{
                                //    ModelState.AddModelError("PNPFile", "Only .JPG, .JPEG, .PNG & PDF file types are allowed.");
                                //}

                                break;
                            }
                        case "LTO":
                            {
                                if (!CheckAvailableBalance(1, UserEntityEnum.Dealer, (int)CurrentUser.Details.ReferenceID))
                                {
                                    TempData["ErrorMessage"] = "Wallet balance is not enough";
                                    return View(Vehicle);
                                }


                                TempData["BatchHeader"] = LoadVehicle.BatchHeader;
                                if (LTOSubmit(VehicleIDList))
                                {
                                    TempData["SuccessMessage"] = "LTO Registration successful!";
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "An error has occured.";
                                }
                            }
                            break;
                        case "COC":
                            {
                                var AutoGenerate = db.Insurance.Where(o => o.InsuranceID == LoadVehicle.COCInfo.DealerInsuranceID).FirstOrDefault().AutoGenerateCOC;

                                if (AutoGenerate)
                                {
                                    if (LoadVehicle.COCInfo.AutoGenerateCoC)
                                    {
                                        if (!CheckAvailableBalance(1, UserEntityEnum.Insurance, LoadVehicle.COCInfo.DealerInsuranceID))
                                        {
                                            TempData["ErrorMessage"] = "Wallet balance is not enough";
                                            return View(Vehicle);
                                        }

                                        if (GetCOC(LoadVehicle.COCInfo, Vehicle.VehicleID, LoadVehicle.COCInfo.DealerInsuranceID))
                                        {
                                            TempData["SuccessMessage"] = "COC Registration successful!";
                                        }
                                    }
                                    else if (!LoadVehicle.COCInfo.AutoGenerateCoC)
                                    {
                                        if (GetCOCAutoManual(LoadVehicle.COCInfo, Vehicle.VehicleID, LoadVehicle.COCInfo.DealerInsuranceID))
                                        {
                                            TempData["SuccessMessage"] = "COC Registration successful!";
                                        }
                                        else
                                        {
                                            var Invoice = db.DealerInvoice.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();

                                            if (Invoice.COC != null || Invoice.COC != string.Empty)
                                            {
                                                TempData["InfoMessage"] = "Please click 'Authenticate' button to verify the COC";
                                            }
                                            else
                                            {
                                                TempData["ErrorMessage"] = "An error has occured.";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "An error has occured.";
                                    }
                                }
                                else
                                {
                                    if (GetCOCManual(LoadVehicle.COCInfo, Vehicle.VehicleID, LoadVehicle.COCInfo.DealerInsuranceID))
                                    {
                                        TempData["SuccessMessage"] = "COC Registration successful!";
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "An error has occured.";
                                    }
                                }
                            }
                            break;
                        case "PNP":
                            {
                                //AUTO PNP
                                if (LoadVehicle.PNPInfo.AutoPNP == true)
                                {
                                    if (LoadVehicle.PNPInfo.PNPReceiptFile.IsAllowedContentType() && LoadVehicle.PNPInfo.PNPReceiptFile.IsValidFileSize())
                                    {
                                        var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                        UploadFile.AutoPNP = LoadVehicle.PNPInfo.AutoPNP;
                                        UploadFile.PNPReceiptReferenceNumber = LoadVehicle.PNPInfo.PNPReceiptReferenceNumber;
                                        UploadFile.PNPReceipt = LoadVehicle.PNPInfo.PNPReceiptFile.ToByte();
                                        UploadFile.PNPReceiptContentType = LoadVehicle.PNPInfo.PNPReceiptFile.ContentType;
                                        UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                        UploadFile.UpdatedDate = DateTime.Now;
                                        db.SaveChanges();

                                        ////PNP Transaction
                                        //TransactionModel NewTransaction = new TransactionModel()
                                        //{
                                        //    SelectedUserEntityID = (int)UserEntityEnum.Dealer,
                                        //    SelectedEntityID = (int)CurrentUser.Details.ReferenceID,
                                        //    TransactionTypeID = 7,
                                        //    Amount = 350,
                                        //    CreatedBy = CurrentUser.Details.UserID,
                                        //    CreatedDate = DateTime.Now,
                                        //    Remarks = string.Concat("Vehicle ID: ", UploadFile.VehicleID),
                                        //    VehicleID = UploadFile.VehicleID
                                        //};

                                        //Functions.InsertTransaction(NewTransaction);

                                        Vehicle.AutoPNP = UploadFile.AutoPNP;
                                        Vehicle.PNPReceiptReferenceNumber = UploadFile.PNPReceiptReferenceNumber;
                                        Vehicle.PNPReceipt = UploadFile.PNPReceipt;
                                        Vehicle.PNPReceiptContentType = UploadFile.PNPReceiptContentType;
                                        TempData["SuccessMessage"] = "Application for PNP Certificate submitted";
                                        return View(Vehicle);
                                    }
                                    else if (!LoadVehicle.PNPInfo.PNPReceiptFile.IsAllowedContentType())
                                    {
                                        TempData["WarningMessage"] = "Only .JPG, .JPEG, .PNG & PDF file type's are allowed.";
                                        return View(Vehicle);
                                    }
                                    else if (!LoadVehicle.PNPInfo.PNPReceiptFile.IsValidFileSize())
                                    {
                                        TempData["WarningMessage"] = "Please upload valid file size of less than 1 MB!";
                                        return View(Vehicle);
                                    }
                                }


                                //REGULAR PNP
                                if (LoadVehicle.PNPInfo.PNPFile.IsAllowedContentType() && LoadVehicle.PNPInfo.PNPFile.IsValidFileSize())
                                {
                                    var UploadFile = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                                    UploadFile.PNPClearance = LoadVehicle.PNPInfo.PNPFile.ToByte();
                                    UploadFile.PNPContentType = LoadVehicle.PNPInfo.PNPFile.ContentType;
                                    UploadFile.HPGControlNumber = LoadVehicle.PNPInfo.HPGControlNumber;
                                    UploadFile.UpdatedBy = CurrentUser.Details.UserID;
                                    UploadFile.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    Vehicle.PNPClearance = UploadFile.PNPClearance;
                                    Vehicle.PNPContentType = UploadFile.PNPContentType;
                                    Vehicle.HPGControlNumber = UploadFile.HPGControlNumber;

                                    Vehicle.PNPClearance = UploadFile.PNPClearance;
                                    Vehicle.PNPContentType = UploadFile.PNPContentType;
                                    Vehicle.HPGControlNumber = UploadFile.HPGControlNumber;
                                    TempData["SuccessMessage"] = "PNP Clearance is submitted!";
                                    return View(Vehicle);
                                }
                                else if (!LoadVehicle.PNPInfo.PNPFile.IsValidFileSize())
                                {
                                    TempData["WarningMessage"] = "Please upload valid file size of less than 1 MB!";
                                }
                                else if (!LoadVehicle.PNPInfo.PNPFile.IsAllowedContentType())
                                {
                                    TempData["WarningMessage"] = "Only .JPG, .JPEG, .PNG & PDF file type's are allowed.";
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "An error has occured.";
                                }
                            }
                            break;
                        case "coc_verification":
                            {
                                CoCVerification(Vehicle.COCNo, Vehicle.VehicleID);
                            }
                            break;
                        case "coc_authentication":
                            {
                                if (cocGetAuthentication(Vehicle.VehicleID))
                                {
                                    if (CoCReport(Vehicle.VehicleID))
                                        TempData["SuccessMessage"] = "Authenticated!";
                                    else
                                        TempData["ErrorMessage"] = "Report generation failed!";
                                }
                                //else
                                //    TempData["ErrorMessage"] = "Authentication failed";
                            }
                            break;
                        case "pull":
                            {
                                if (db.DealerInvoice.Where(o => o.VehicleID == LoadVehicle.VehicleID).Count() == 0)
                                {
                                    var UpdateDealer = db.VehicleInfo.Where(o => o.VehicleID == LoadVehicle.VehicleID).FirstOrDefault();
                                    UpdateDealer.DealerID = null;
                                    UpdateDealer.DealerBranchID = null;
                                    UpdateDealer.Assigned = false;
                                    UpdateDealer.UpdatedBy = CurrentUser.Details.UserID;
                                    UpdateDealer.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    TempData["SuccessMessage"] = "Pull out vehicle successful!";
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "An error has occured.";
                                }
                            }
                            break;
                        case "pulloutReport":
                            {
                                List<vwVehicleListModel> model = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                      && (o.DealerBranchID != null || o.DealerID != null)
                                      && o.CertificateOfConformity == null
                                      && o.Active == true)
                                      .Select(o => new vwVehicleListModel
                                      {
                                          VehicleID = o.VehicleID,
                                          VehicleMakeName = o.VehicleMakeName,
                                          VehicleModelName = o.VehicleModelName,
                                          Variant = o.Variant,
                                          Year = o.Year,
                                          EngineNumber = o.EngineNumber,
                                          ChassisNumber = o.ChassisNumber,
                                          BodyIDNumber = o.BodyIDNumber,
                                          isChecked = false
                                      }).ToList();
                                return PullOutReport(model);
                            }
                        default: break;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                Vehicle = GetVehicleInfo(LoadVehicle.VehicleID);
                return View(Vehicle);
            }

        }

        public VehicleInfoModel GetVehicleInfo(int VehicleID)
        {
            using (db = new VRSystemEntities())
            {
                VehicleInfoModel Vehicle = new VehicleInfoModel();

                var VehicleLoad = db.VehicleInfo.Where(o => o.Active == true && o.VehicleID == VehicleID).ToList().FirstOrDefault();
                var MAILoad = db.MAI.Where(o => o.MAIID == VehicleLoad.MAIID).FirstOrDefault();

                Vehicle.VehicleID = VehicleLoad.VehicleID;
                if (MAILoad != null)
                {
                    Vehicle.MAIID = MAILoad.MAIID;
                    Vehicle.MAIName = MAILoad.MAIName;
                    Vehicle.Address = MAILoad.Address;
                    Vehicle.AccreditationNumber = MAILoad.AccreditationNumber;
                }

                Vehicle.DealerID = VehicleLoad.DealerID;
                Vehicle.DealerName = db.Dealer.Where(o => o.Active == true & o.DealerID == VehicleLoad.DealerID).Select(o => o.DealerName).FirstOrDefault();

                Vehicle.DealerBranchID = VehicleLoad.DealerBranchID;
                Vehicle.DealerBranchName = db.DealerBranch.Where(o => o.DealerBranchID == VehicleLoad.DealerBranchID).Select(o => o.DealerBranchName).FirstOrDefault();

                Vehicle.SelectedVehicleMakeID = VehicleLoad.VehicleMakeID;
                Vehicle.VehicleMakeName = db.VehicleMake.Where(o => o.VehicleMakeID == VehicleLoad.VehicleMakeID).Select(o => o.VehicleMakeName).FirstOrDefault();

                Vehicle.SelectedVehicleModelID = VehicleLoad.VehicleModelID;
                Vehicle.VehicleModelName = db.VehicleModel.Where(o => o.VehicleModelID == VehicleLoad.VehicleModelID).Select(o => o.VehicleModelName).FirstOrDefault();

                Vehicle.EngineNumber = VehicleLoad.EngineNumber;
                Vehicle.CPNumber = VehicleLoad.CPNumber;
                Vehicle.DateIssued1 = VehicleLoad.DateIssued1;
                Vehicle.ChassisNumber = VehicleLoad.ChassisNumber;
                Vehicle.CPNumber2 = VehicleLoad.CPNumber2;
                Vehicle.DateIssued2 = VehicleLoad.DateIssued2;
                Vehicle.BodyIDNumber = VehicleLoad.BodyIDNumber;
                Vehicle.BIRCCMV = VehicleLoad.BIRCCMV;
                Vehicle.DateIssued3 = VehicleLoad.DateIssued3;

                Vehicle.SelectedVehicleColorID = VehicleLoad.VehicleColorID;
                Vehicle.VehicleColorName = db.VehicleColor.Where(o => o.VehicleColorID == VehicleLoad.VehicleColorID).Select(o => o.VehicleColorName).FirstOrDefault();

                Vehicle.PistonDisplacement = VehicleLoad.PistonDisplacement;

                Vehicle.SelectedVehicleFuelTypeID = VehicleLoad.VehicleFuelTypeID;
                Vehicle.VehicleFuelName = db.VehicleFuelType.Where(o => o.VehicleFuelTypeID == VehicleLoad.VehicleFuelTypeID).Select(o => o.VehicleFuelTypeName).FirstOrDefault();

                Vehicle.Cylinders = VehicleLoad.Cylinders;
                Vehicle.Series = VehicleLoad.Series;
                Vehicle.Year = VehicleLoad.Year;
                Vehicle.GrossVehicleWeight = VehicleLoad.GrossVehicleWeight;
                Vehicle.FrontTiresNumber = VehicleLoad.FrontTiresNumber;
                Vehicle.RearTiresNumber = VehicleLoad.RearTiresNumber;
                Vehicle.TaxType = VehicleLoad.TaxType;
                Vehicle.TaxAmount = VehicleLoad.TaxAmount;
                Vehicle.AirconType = db.AirconType.Where(o => o.Active == true && o.AirconTypeReference == VehicleLoad.AirconType).Select(o => o.AirconTypeDescription).FirstOrDefault();
                Vehicle.ConductionSticker = VehicleLoad.ConductionSticker;
                Vehicle.COCNo = VehicleLoad.COCNo;
                Vehicle.HPGControlNumber = VehicleLoad.HPGControlNumber;
                Vehicle.PNPInfo.HPGControlNumber = VehicleLoad.HPGControlNumber;
                Vehicle.PNPReceiptReferenceNumber = VehicleLoad.PNPReceiptReferenceNumber;
                Vehicle.AutoPNP = VehicleLoad.AutoPNP;
                Vehicle.Remarks = VehicleLoad.Remarks;
                Vehicle.DatePrepared = VehicleLoad.DatePrepared;
                Vehicle.CSRNumber = VehicleLoad.CSRNumber;
                Vehicle.TransactionID = VehicleLoad.TransactionID;
                Vehicle.ReportDate = VehicleLoad.ReportDate;
                Vehicle.ReportEntryID = VehicleLoad.ReportEntryID;
                //Vehicle.Address = MAILoad.Address;
                //Vehicle.AccreditationNumber = MAILoad.AccreditationNumber;
                Vehicle.ItemType = VehicleLoad.ItemType;

                Vehicle.SelectedVehicleBodyTypeID = VehicleLoad.VehicleBodyTypeID;
                Vehicle.VehicleBodyTypeName = db.VehicleBodyType.Where(o => o.VehicleBodyTypeID == VehicleLoad.VehicleBodyTypeID).Select(o => o.VehicleBodyTypeName).FirstOrDefault();
                Vehicle.LTOSubmitted = VehicleLoad.LTOSubmitted;

                Vehicle.CSRSubmitted = VehicleLoad.CSRSubmitted;
                Vehicle.Assigned = VehicleLoad.Assigned;
                //Images
                Vehicle.BOCCertificateOfPayment = VehicleLoad.BOCCertificateOfPayment;
                Vehicle.BOCCertificateOfPayment2 = VehicleLoad.BOCCertificateOfPayment2;
                Vehicle.CertificateOfStockReport = VehicleLoad.CertificateOfStockReport;
                Vehicle.CertificateOfConformity = VehicleLoad.CertificateOfConformity;
                Vehicle.StencilOfChasis = VehicleLoad.StencilOfChasis;
                Vehicle.StencilOfEngine = VehicleLoad.StencilOfEngine;
                Vehicle.PNPClearance = VehicleLoad.PNPClearance;
                Vehicle.PNPReceipt = VehicleLoad.PNPReceipt;
                //Image ContentType
                Vehicle.BOCContentType = VehicleLoad.BOCContentType;
                Vehicle.BOCContentType2 = VehicleLoad.BOCContentType2;
                Vehicle.CSRContentType = VehicleLoad.CSRContentType;
                Vehicle.COCContentType = VehicleLoad.COCContentType;
                Vehicle.SOCContentType = VehicleLoad.SOCContentType;
                Vehicle.SOEContentType = VehicleLoad.SOEContentType;
                Vehicle.PNPContentType = VehicleLoad.PNPContentType;
                Vehicle.PNPReceiptContentType = VehicleLoad.PNPReceiptContentType;

                Vehicle.DealerInfo = new AssignDealerModel();
                Vehicle.DealerInfo.MAI_DealerList = new List<MAI_DealerModel>();
                Vehicle.DealerInfo.MAI_DealerBranchList = new List<DealerBranchModel>();

                var query = from a in db.MAIDealer
                            join b in db.Dealer on a.DealerID equals b.DealerID into temp
                            from temptbl in temp.DefaultIfEmpty()
                            select new
                            {
                                MAIID = a.MAIID,
                                MAIDealerID = a.MAIDealerID,
                                DealerID = temptbl.DealerID,
                                DealerName = temptbl.DealerName,
                                Active = a.Active
                            };
                Vehicle.DealerInfo.MAI_DealerList = query.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        MAIDealerID = o.MAIDealerID,
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                //if (Vehicle.DealerID == null && Vehicle.DealerBranchID == null)
                //{


                //}

                //CSR
                if (VehicleLoad.TransactionID != null)
                {
                    ViewBag.CSR_Edit = true;
                }
                else
                {
                    ViewBag.CSR_Edit = false;
                }
                Vehicle.CSRInfo.CSRByte = VehicleLoad.CertificateOfStockReport;
                Vehicle.CSRInfo.CSRContentType = VehicleLoad.CSRContentType;
                Vehicle.CSRInfo.TransactionID = VehicleLoad.TransactionID;
                Vehicle.CSRInfo.CSRNumber = VehicleLoad.CSRNumber;
                Vehicle.CSRInfo.ReportEntryID = VehicleLoad.ReportEntryID;
                Vehicle.CSRInfo.ReportDate = VehicleLoad.ReportDate;
                Vehicle.CSRInfo.ItemType = VehicleLoad.ItemType;
                Vehicle.CSRInfo.BIRCCMV = VehicleLoad.BIRCCMV;
                Vehicle.CSRInfo.DateIssued3 = VehicleLoad.DateIssued3;
                Vehicle.CSRInfo.TaxType = VehicleLoad.TaxType;
                Vehicle.CSRInfo.TaxAmount = VehicleLoad.TaxAmount;

                //CSR-MAI
                ViewBag.MAICSR_Received = (from bd in db.BatchDetails
                                           join bm in db.BatchMaster
                                           on bd.BatchID equals bm.BatchID
                                           where
                                           bm.Received == true &&
                                           bm.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                                           bd.VehicleID == VehicleLoad.VehicleID
                                           select bm).Select(o => o.Received).FirstOrDefault();
                ///CSR\\\

                //INVOICE
                Vehicle.InvoiceInfo = new DealerInvoiceModel();
                Vehicle.InvoiceInfo.VehicleList = db.vwVehicleList.Where(o => o.VehicleID == VehicleID).ToList();
                Vehicle.InvoiceInfo.CustomerList = (from a in db.Customer
                                                    join b in db.Title on a.TitleID equals b.TitleID into temp
                                                    where a.Active == true
                                                    from temptbl in temp.DefaultIfEmpty()
                                                    select new
                                                    {
                                                        a.DealerID,
                                                        a.CustomerID,
                                                        a.FirstName,
                                                        a.LastName,
                                                        a.MiddleName,
                                                        a.CorpName,
                                                        temptbl.TitleTypeID,
                                                    })
                                       .Where(o => o.DealerID == CurrentUser.Details.ReferenceID)
                                       .Select(o => new CustomerModel()
                                       {
                                           CustomerID = o.CustomerID,
                                           FirstName = o.FirstName,
                                           LastName = o.LastName,
                                           MiddleName = o.MiddleName,
                                           CorpName = o.CorpName,
                                           TitleTypeID = o.TitleTypeID
                                       })
                                       .ToList();


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
                if (newsearchresult != null)
                {
                    Vehicle.InvoiceInfo.VehicleInfo.VehicleMakeName = newsearchresult.VehicleMakeName;
                    Vehicle.InvoiceInfo.VehicleInfo.VehicleModelName = newsearchresult.VehicleModelName;
                }

                Vehicle.InvoiceInfo.VehicleInfo.EngineNumber = searchresult.EngineNumber;
                Vehicle.InvoiceInfo.VehicleInfo.ChassisNumber = searchresult.ChassisNumber;
                Vehicle.InvoiceInfo.SelectedVehicleID = VehicleID;
                //Vehicle.InvoiceInfo.VehicleClassificationID = newsearchresult.VehicleClassificationID;

                //Vehicle.InvoiceInfo.VehicleTypeList = db.VehicleType
                //    .Where(o => o.Active == true)
                //    .Select(o => new VehicleTypeList()
                //    {
                //        VehicleTypeID = o.VehicleTypeID,
                //        VehicleCode = o.VehicleCode,
                //        VehicleDesc = o.VehicleTypeDescription
                //    }).ToList();

                //Vehicle.InvoiceInfo.VehicleClassificationList = db.VehicleClassification.Where(o => o.Active == true).ToList();

                //Vehicle.InvoiceInfo.TaxTypeList = db.CTPLTaxType.ToList();

                Vehicle.InvoiceInfo.FormOrigin = "VehicleInfo";

                //if (db.DealerInvoice.Where(o => o.VehicleID == Vehicle.VehicleID).Count() > 0)
                //{
                //    ViewBag.HasInvoice = true;
                //}
                //else
                //{
                //    ViewBag.HasInvoice = false;
                //}


                if (db.DealerInvoice.Where(o => o.VehicleID == Vehicle.VehicleID).Count() > 0)
                {
                    var queryinvoice = from a in db.DealerInvoice
                                       join b in db.Customer on a.CustomerID equals b.CustomerID
                                       join c in db.City on b.CityID equals c.CityID
                                       join d in db.Title on b.TitleID equals d.TitleID
                                       join e in db.VehicleClassification on a.VehicleClassificationID equals e.VehicleClassificationID into temp
                                       from temptbl in temp.DefaultIfEmpty()
                                       select new
                                       {
                                           a,
                                           FullName = b.LastName + ", " + b.FirstName +" "+ b.MiddleName,
                                           b.CorpName,
                                           b.OrganizationAddress,
                                           b.FirstName,
                                           b.MiddleName,
                                           b.LastName,
                                           b.HouseBldgNumber,
                                           b.StreetSubdivision,
                                           b.Barangay,
                                           c.CityName,
                                           b.ZipCode,
                                           d.TitleTypeID,
                                           temptbl.VehicleClassificationName
                                       };
                    var LoadInvoice = queryinvoice.Where(o => o.a.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                    if (LoadInvoice != null)
                    {
                        var FullName = "";
                        var FullAddress = "";
                        if (LoadInvoice.TitleTypeID == 2)
                        {
                            FullName = LoadInvoice.CorpName;
                            FullAddress = LoadInvoice.OrganizationAddress;
                        }
                        else
                        {
                            FullName = LoadInvoice.LastName + ", " + LoadInvoice.FirstName + " " + LoadInvoice.MiddleName;
                            FullAddress = LoadInvoice.HouseBldgNumber + ", " + LoadInvoice.StreetSubdivision + ", " + LoadInvoice.Barangay + ", " + LoadInvoice.CityName + ", " + LoadInvoice.ZipCode;
                        }

                        Vehicle.InvoiceInfo.InvoiceID = LoadInvoice.a.InvoiceID;
                        Vehicle.InvoiceInfo.SelectedCustomerID = LoadInvoice.a.CustomerID;
                        Vehicle.InvoiceInfo.CustomerFullName = FullName;
                        Vehicle.InvoiceInfo.InvoiceNumber = LoadInvoice.a.InvoiceNumber;
                        Vehicle.InvoiceInfo.InvoiceDate = LoadInvoice.a.InvoiceDate.Date;
                        Vehicle.InvoiceInfo.InvoiceByte = LoadInvoice.a.InvoiceByte;
                        Vehicle.InvoiceInfo.InvoiceContentType = LoadInvoice.a.InvoiceContentType;
                        Vehicle.InvoiceInfo.COC = LoadInvoice.a.COC;
                        Vehicle.InvoiceInfo.COCByte = LoadInvoice.a.COCByte;
                        Vehicle.InvoiceInfo.COCContentType = LoadInvoice.a.COCContentType;
                        Vehicle.InvoiceInfo.COCAuthenticationCode = LoadInvoice.a.COCAuthenticationCode;
                        Vehicle.InvoiceInfo.SelectedVehicleID = LoadInvoice.a.VehicleID;
                        Vehicle.InvoiceInfo.COCPolicyNumber = LoadInvoice.a.COCPolicyNumber;
                        Vehicle.InvoiceInfo.COCInceptionDate = LoadInvoice.a.COCInceptionDate;
                        Vehicle.InvoiceInfo.VehicleCost = LoadInvoice.a.VehicleCost;
                        Vehicle.InvoiceInfo.PreferredEndingPlateNumber = LoadInvoice.a.PreferredEndingPlateNumber;
                        Vehicle.InvoiceInfo.VehicleClassificationID = LoadInvoice.a.VehicleClassificationID;
                        Vehicle.InvoiceInfo.VehicleClassificationName = LoadInvoice.VehicleClassificationName;
                        Vehicle.InvoiceInfo.VehicleTypeID = LoadInvoice.a.VehicleTypeID;
                        Vehicle.InvoiceInfo.TaxTypeID = LoadInvoice.a.TaxTypeID;
                        Vehicle.InvoiceInfo.COCInceptionDate = LoadInvoice.a.COCInceptionDate;
                        Vehicle.InvoiceInfo.Encumbered = LoadInvoice.a.Encumbered;
                        Vehicle.InvoiceInfo.FinancialInstitution = LoadInvoice.a.FinancialInstitution;

                        Vehicle.InvoiceInfo.VehicleClassificationList = (from a in db.MVPremium
                                                                         where a.VehicleTypeID == LoadInvoice.a.TaxTypeID
                                                                         join b in db.VehicleClassification on a.VehicleClassificationID equals b.VehicleClassificationID into temp
                                                                         from temptbl in temp.DefaultIfEmpty()
                                                                         select new
                                                                         {
                                                                             temptbl
                                                                         }).Select(o => o.temptbl).ToList();
                        if (LoadInvoice.a.COCInceptionDate != null && LoadInvoice.a.COCExpirationDate != null)
                        {
                            Vehicle.COCInfo.InceptionDate = LoadInvoice.a.COCInceptionDate;
                            Vehicle.COCInfo.ExpiryDate = LoadInvoice.a.COCExpirationDate;
                        }

                        //Vehicle.COCInfo.COCInsuredName = FullName;
                        //Vehicle.COCInfo.COCInsuredAddress = FullAddress;
                    }
                    if (LoadInvoice.a.InvoiceByte != null)
                        ViewBag.HasInvoice = true;
                    else
                        ViewBag.HasInvoice = false;

                    ViewBag.Edit = true;
                }
                else
                {
                    ViewBag.HasInvoice = false;
                    ViewBag.Edit = false;
                }
                ///Invoice\\\


                //COC
                Vehicle.DealerInsuranceList = (from a in db.DealerInsurance
                                               where a.Active == true
                                               join b in db.Insurance on a.InsuranceID equals b.InsuranceID into temp
                                               from temptbl in temp.DefaultIfEmpty()
                                               select new
                                               {
                                                   DealerID = a.DealerID,
                                                   InsuranceID = temptbl.InsuranceID,
                                                   InsuranceName = temptbl.InsuranceName,
                                                   AutoGenerated = temptbl.AutoGenerateCOC
                                               }).Where(o => o.DealerID == VehicleLoad.DealerID).Select(
                        o => new vwDealerInsuranceModel()
                        {
                            DealerID = o.DealerID,
                            InsuranceID = o.InsuranceID,
                            InsuranceName = o.InsuranceName,
                            AutoGenerateCOC = o.AutoGenerated
                        }).ToList();

                Vehicle.COCInfo.DealerInsuranceList = (from a in db.DealerInsurance
                                                       where a.Active == true
                                                       join b in db.Insurance on a.InsuranceID equals b.InsuranceID into temp
                                                       from temptbl in temp.DefaultIfEmpty()
                                                       select new
                                                       {
                                                           DealerID = a.DealerID,
                                                           InsuranceID = temptbl.InsuranceID,
                                                           InsuranceName = temptbl.InsuranceName,
                                                           AutoGenerated = temptbl.AutoGenerateCOC
                                                       }).Where(o => o.DealerID == VehicleLoad.DealerID).Select(
                        o => new vwDealerInsuranceModel()
                        {
                            DealerID = o.DealerID,
                            InsuranceID = o.InsuranceID,
                            InsuranceName = o.InsuranceName,
                            AutoGenerateCOC = o.AutoGenerated
                        }).ToList();


                Vehicle.COCInfo.VehicleTypeList = db.VehicleType.Where(o => o.Active == true)
                    .Select(o => new VehicleTypeList()
                    {
                        VehicleTypeID = o.VehicleTypeID,
                        VehicleCode = o.VehicleCode,
                        VehicleDesc = o.VehicleTypeDescription
                    }).ToList();

                Vehicle.COCInfo.VehicleClassificationList = (from a in db.MVPremium
                                                             where a.VehicleTypeID == Vehicle.InvoiceInfo.VehicleTypeID
                                                             join b in db.VehicleClassification on a.VehicleClassificationID equals b.VehicleClassificationID into temp
                                                             from temptbl in temp.DefaultIfEmpty()
                                                             select new
                                                             {
                                                                 temptbl
                                                             })
                                .Select(o => o.temptbl).ToList();
                Vehicle.COCInfo.TaxTypeList = db.CTPLTaxType.ToList();
                ///COC\\\

                //Batch Info
                var Batch = (from BM in db.BatchMaster
                             join BD in db.BatchDetails on BM.BatchID equals BD.BatchID
                             where (BM.BatchTypeID == (int)BatchTypeList.LTO || BM.BatchTypeID == (int)BatchTypeList.LTOCSR) &&
                                   BM.Active == true &&
                                   BD.VehicleID == Vehicle.VehicleID
                             select new
                             {
                                 BM,
                                 BD
                             }).FirstOrDefault();

                if (Batch != null)
                {
                    Vehicle.BatchID = Batch.BM.BatchID;
                    Vehicle.BatchReferenceNo = Batch.BM.ReferenceNo;
                    Vehicle.BatchDescription = Batch.BM.BatchDescription;
                    Vehicle.IsSubmitted = Batch.BM.ReferenceNo == null ? false : true;
                    Vehicle.IsAssessed = Batch.BM.Assessed;
                    Vehicle.IsBatchCompleted = Batch.BM.Completed;
                    Vehicle.isRejected = Batch.BD.Rejected;
                    Vehicle.isCompleted = Batch.BD.Completed;

                    if (Batch.BM.PaymentRef != null)
                    {
                        Vehicle.IsPaid = true;
                    }
                }

                return Vehicle;
            }
        }


        public ActionResult PullOutReport(List<vwVehicleListModel> model)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "PullOutReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return RedirectToAction("Index");
            }

            DataTable dt = new DataTable("PullOutList");
            dt.Columns.Add(new DataColumn("CompanyName", typeof(string)));
            dt.Columns.Add(new DataColumn("BranchName", typeof(string)));
            dt.Columns.Add(new DataColumn("VehicleModel", typeof(string)));
            dt.Columns.Add(new DataColumn("VehicleVariant", typeof(string)));
            dt.Columns.Add(new DataColumn("VehicleColor", typeof(string)));
            dt.Columns.Add(new DataColumn("VehicleEngine", typeof(string)));
            dt.Columns.Add(new DataColumn("VehicleChassis", typeof(string)));

            MAI MAIInfo = new MAI();
            using (db = new VRSystemEntities())
            {
                MAIInfo = db.MAI.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID).FirstOrDefault();
                foreach (var item in model)
                {
                    if (item.isChecked == true)
                    {
                        var VehicleInfo = db.vwVehicleList.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                        dt.Rows.Add(
                            VehicleInfo.DealerName,
                            VehicleInfo.DealerBranchName,
                            VehicleInfo.VehicleModelName,
                            VehicleInfo.Variant,
                            VehicleInfo.VehicleColorName,
                            VehicleInfo.EngineNumber,
                            VehicleInfo.ChassisNumber
                            );
                    }
                }
            }

            string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            lr.EnableExternalImages = true;
            lr.EnableHyperlinks = true;

            ReportParameter[] prm = new ReportParameter[2];
            prm[0] = new ReportParameter("MAICompanyParameter", MAIInfo.MAIName);
            prm[1] = new ReportParameter("MAILogoParameter", imagePath);
            lr.SetParameters(prm);

            ReportDataSource rds = new ReportDataSource("PullOutList", dt);
            lr.DataSources.Clear();
            lr.DataSources.Add(rds);

            lr.Refresh();


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
            "  <PageWidth>11.69in</PageWidth>" +
            "  <PageHeight>8.27in</PageHeight>" +
            "  <MarginTop>0.25in</MarginTop>" +
            "  <MarginLeft>0.25in</MarginLeft>" +
            "  <MarginRight>0.25in</MarginRight>" +
            "  <MarginBottom>0.10in</MarginBottom>" +
            "</DeviceInfo>";

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
            return File(renderedBytes, mimeType);
        }

        public ActionResult PullOutReport2(int VehicleID)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "PullOutReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return RedirectToAction("VehicleInfo/" + VehicleID);
            }

            var Vehicle = GetVehicleInfo(VehicleID);
            MAI MAIInfo = new MAI();
            vwVehicleList VehicleInfo = new vwVehicleList();
            using (db = new VRSystemEntities())
            {
                MAIInfo = db.MAI.Where(o => o.MAIID == Vehicle.MAIID).FirstOrDefault();
                VehicleInfo = db.vwVehicleList.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
            }

            string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            lr.EnableExternalImages = true;
            lr.EnableHyperlinks = true;

            ReportParameter[] prm = new ReportParameter[9];
            prm[0] = new ReportParameter("MAICompanyParameter", Vehicle.MAIName);
            prm[1] = new ReportParameter("DealerCompanyParameter", Vehicle.DealerName);
            prm[2] = new ReportParameter("DealerBranchParameter", Vehicle.DealerBranchName);
            prm[3] = new ReportParameter("VehicleModelParameter", Vehicle.VehicleModelName);
            prm[4] = new ReportParameter("VehicleColorParameter", Vehicle.VehicleColorName);
            prm[5] = new ReportParameter("VehicleEngineNumberParameter", Vehicle.EngineNumber);
            prm[6] = new ReportParameter("VehicleChasisNumberParameter", Vehicle.ChassisNumber);
            prm[7] = new ReportParameter("MAILogoParameter", imagePath);
            prm[8] = new ReportParameter("VehicleVariantParameter", VehicleInfo.Variant);
            lr.SetParameters(prm);

            lr.Refresh();


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
            "  <PageWidth>11.69in</PageWidth>" +
            "  <PageHeight>8.27in</PageHeight>" +
            "  <MarginTop>0.25in</MarginTop>" +
            "  <MarginLeft>0.25in</MarginLeft>" +
            "  <MarginRight>0.25in</MarginRight>" +
            "  <MarginBottom>0.10in</MarginBottom>" +
            "</DeviceInfo>";

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
            return File(renderedBytes, mimeType);
        }

        public bool CoCReport(int VehicleID)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "CoCReport.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return false;
                }


                var Vehicle = GetVehicleInfo(VehicleID);
                DealerInvoice invoice = new DealerInvoice();
                Customer customer = new Customer();
                City City = new City();
                VehicleModel VehicleModel = new VehicleModel();
                VehicleClassification VC = new VehicleClassification();
                CTPL CTPL = new CTPL();
                CTPLTerm CTPLTerm = new CTPLTerm();
                Insurance Insurance = new Insurance();
                InsuranceCOCSeries CoCSeries = new InsuranceCOCSeries();
                var NameParameter = "";
                var AddressParameter = "";
                using (db = new VRSystemEntities())
                {
                    invoice = db.DealerInvoice.Where(o => o.Active == true && o.VehicleID == VehicleID).FirstOrDefault();
                    customer = db.Customer.Where(o => o.CustomerID == invoice.CustomerID).FirstOrDefault();
                    var TitleType = (from a in db.Title
                                     join b in db.TitleType on a.TitleTypeID equals b.TitleTypeID into temp
                                     from temptbl in temp.DefaultIfEmpty()
                                     select new
                                     {
                                         TitleID = a.TitleID,
                                         TitleTypeID = temptbl.TitleTypeID,
                                         TitleTypeName = temptbl.TitleTypeName
                                     })
                                 .Where(o => o.TitleID == customer.TitleID).FirstOrDefault();

                    City = db.City.Where(o => o.CityID == customer.CityID).FirstOrDefault();
                    VehicleModel = db.VehicleModel.Where(o => o.VehicleModelID == Vehicle.SelectedVehicleModelID).FirstOrDefault();
                    if (invoice.VehicleClassificationID != 0)
                    {
                        VC = db.VehicleClassification.Where(o => o.VehicleClassificationID == invoice.VehicleClassificationID).FirstOrDefault();
                    }
                    else
                    {
                        VC = db.VehicleClassification.Where(o => o.VehicleClassificationID == VehicleModel.VehicleClassificationID).FirstOrDefault();
                    }
                    CTPL = db.CTPL.Where(o => o.VehicleClassificationID == VC.VehicleClassificationID).FirstOrDefault();
                    CTPLTerm = db.CTPLTerm.Where(o => o.CPTLTermID == CTPL.CTPLTermID).FirstOrDefault();
                    Insurance = db.Insurance.Where(o => o.InsuranceID == invoice.InsuranceID).FirstOrDefault();
                    CoCSeries = db.InsuranceCOCSeries.Where(o => o.InsuranceID == Insurance.InsuranceID && o.Active == true).FirstOrDefault();

                    if (TitleType.TitleTypeID == 2)
                    {
                        NameParameter = customer.CorpName;
                        AddressParameter = customer.HouseBldgNumber + ", " + customer.StreetSubdivision + ", " + customer.Barangay + ", " + City.CityName + ", " + customer.ZipCode;
                    }
                    else
                    {
                        NameParameter = customer.FirstName + " " + customer.MiddleName + " " + customer.LastName;
                        AddressParameter = customer.HouseBldgNumber + ", " + customer.StreetSubdivision + ", " + customer.Barangay + ", " + City.CityName + ", " + customer.ZipCode;
                    }
                }

                var dateFrom = (DateTime)invoice.COCInceptionDate;
                var dateTo = (DateTime)invoice.COCExpirationDate;

                //string imagepath = Path.Combine(Server.MapPath("~/Content"), "DataBridge Asia Inc Logo-01.jpg");
                //var base64 = Convert.ToBase64String(dealerss.LogoByte);
                //var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
                //lr.EnableExternalImages = true;

                //convert byte array to base64string

                //string base64String = Convert.ToBase64String(Insurance.LogoByte);
                //var imgSrc = String.Format("data:image/png;base64,{0}", base64String);
                //var logoimage = "data:image/gif;base64," + base64String;

                //Image image;
                //using (MemoryStream ms = new MemoryStream(Insurance.LogoByte))
                //{
                //    image = Image.FromStream(ms);
                //}
                //var randomFileName = "Insurance-Logo" + Vehicle.VehicleID + ".png";
                //var fullPath = Path.Combine(Server.MapPath("~/scripts/Img/"), randomFileName);
                //image.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);


                string imagePath = new Uri(Server.MapPath("~/Logos/" + Insurance.Logo)).AbsoluteUri;
                lr.EnableExternalImages = true;
                lr.EnableHyperlinks = true;

                ReportParameter[] prm = new ReportParameter[24];
                prm[0] = new ReportParameter("NameParameter", NameParameter);
                prm[1] = new ReportParameter("AddressParameter", AddressParameter);
                prm[2] = new ReportParameter("AuthenticationParameter", invoice.COCAuthenticationCode);
                prm[3] = new ReportParameter("PolicyParameter", invoice.COCPolicyNumber);
                prm[4] = new ReportParameter("BusinessParameter", "");
                prm[5] = new ReportParameter("CoCParameter", invoice.COC);
                prm[6] = new ReportParameter("DateIssuedParameter", dateFrom.ToString("MMM dd, yyyy"));
                prm[7] = new ReportParameter("ORParameter", "");
                prm[8] = new ReportParameter("PeriodFromParameter", dateFrom.ToString("MMM dd, yyyy"));
                prm[9] = new ReportParameter("PeriodToParameter", dateTo.ToString("MMM dd, yyyy"));
                prm[10] = new ReportParameter("ModelParameter", Vehicle.VehicleModelName);
                prm[11] = new ReportParameter("MakeParameter", Vehicle.VehicleMakeName);
                prm[12] = new ReportParameter("BodyParameter", Vehicle.VehicleBodyTypeName);
                prm[13] = new ReportParameter("ColorParameter", Vehicle.VehicleColorName);
                prm[14] = new ReportParameter("MVFileNoParameter", "");
                prm[15] = new ReportParameter("PlateParameter", "");
                prm[16] = new ReportParameter("SerialOrChassisParameter", Vehicle.ChassisNumber);
                prm[17] = new ReportParameter("MotorParameter", Vehicle.EngineNumber);
                prm[18] = new ReportParameter("CapacityParameter", "");
                prm[19] = new ReportParameter("UnLadenWghtParameter", Vehicle.GrossVehicleWeight.ToString());
                prm[20] = new ReportParameter("LiabilityParameter", "100,000.00");
                prm[21] = new ReportParameter("PremiumParameter", CTPL.GrossPremium.ToString("0.00"));
                prm[22] = new ReportParameter("InsuranceAddress", Insurance.Address + "\nTelephone: " + Insurance.BusinessPhone);
                prm[23] = new ReportParameter("InsuranceLogoParameter", imagePath);
                lr.SetParameters(prm);

                //ReportDataSource rd = new ReportDataSource("MyDataSet", dealerlist);
                //lr.DataSources.Add(rd);

                lr.Refresh();

                string reportTypeImage = "PDF";
                string mimeTypeImage;
                string encodingImage;
                string fileNameExtensionImage;

                string deviceInforImage =

                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.27in</PageWidth>" +
                "  <PageHeight>11.69in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                "  <MarginLeft>0.25in</MarginLeft>" +
                "  <MarginRight>0.25in</MarginRight>" +
                "  <MarginBottom>0.10in</MarginBottom>" +
                "</DeviceInfo>";

                Warning[] warningsImage;
                string[] streamsImage;
                byte[] renderedBytesImage;

                renderedBytesImage = lr.Render(
                    reportTypeImage,
                    deviceInforImage,
                    out mimeTypeImage,
                    out encodingImage,
                    out fileNameExtensionImage,
                    out streamsImage,
                    out warningsImage);

                using (db = new VRSystemEntities())
                {
                    var Update = db.VehicleInfo.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();
                    var UpdateInvoice = db.DealerInvoice.Where(o => o.VehicleID == Vehicle.VehicleID).FirstOrDefault();

                    Update.CertificateOfConformity = renderedBytesImage;
                    Update.COCContentType = "application/pdf";
                    UpdateInvoice.COCByte = renderedBytesImage;


                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool LTOSubmit(List<int> VehicleIDList)
        {

            try
            {
                BatchHeaderModel BatchHeader = (BatchHeaderModel)TempData["BatchHeader"];
                //BatchList
                List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                //List<int> VehicleIDSuccessList = new List<int>();

                var BranchDealerTransaction = db.TransactionEntityBranch
                          .Where(o => o.EntityID == CurrentUser.Details.ReferenceID
                          && o.BranchID == CurrentUser.Details.SubReferenceID
                          && o.UserEntityID == (int)UserEntityEnum.Dealer
                          && o.Active == true
                          && DateTime.Now >= o.EffectivityDate
                          ).GroupBy(o => o.TransactionTypeID)
                          .Select(o => o.OrderByDescending(x => x.EffectivityDate).FirstOrDefault())
                          .ToList();

                var DealerTransaction = db.TransactionEntity
                           .Where(o => o.EntityID == CurrentUser.Details.ReferenceID
                           && o.UserEntityID == (int)UserEntityEnum.Dealer
                           && o.Active == true
                           && DateTime.Now >= o.EffectivityDate
                           )
                           .GroupBy(o => o.TransactionTypeID)
                           .Select(o => o.OrderByDescending(x => x.EffectivityDate).FirstOrDefault())
                           .ToList();

                var EntityTransaction = db.TransactionEntityType
                           .Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer
                           && o.Active == true
                           && DateTime.Now >= o.EffectivityDate
                           )
                           .GroupBy(o => o.TransactionTypeID)
                           .Select(o => o.OrderByDescending(x => x.EffectivityDate).FirstOrDefault())
                           .ToList();


                foreach (var VehicleID in VehicleIDList)
                {
                    var validateCheck = db.vwVehicleList.Where(o =>
                                o.VehicleID == VehicleID
                                &&
                                //o.BOCCertificateOfPayment != null
                                //&&
                                //o.StencilOfChasis != null
                                //&&
                                //o.StencilOfEngine != null
                                //&&
                                o.CertificateOfStockReport != null
                                &&
                                o.CertificateOfConformity != null
                                &&
                                (o.PNPClearance != null || o.AutoPNP == true)
                                &&
                                o.LTOSubmitted == false
                                &&
                                o.DealerID == (int)CurrentUser.Details.ReferenceID
                                &&
                                o.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                &&
                                o.Active == true).FirstOrDefault();


                    if (validateCheck != null)
                    {
                        var Update = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                        Update.LTOSubmitted = true;
                        Update.UpdatedBy = CurrentUser.Details.UserID;
                        Update.UpdatedDate = DateTime.Now;
                        //db.SaveChanges();

                        ////New Computation
                        //var TransactionEntityList = db.TransactionEntity
                        //   .Where(o => o.EntityID == CurrentUser.Details.ReferenceID
                        //   && o.UserEntityID == (int)UserEntityEnum.Dealer
                        //   && o.Active == true
                        //   && DateTime.Now >= o.EffectivityDate
                        //   ).ToList();

                        //var TransactionEntityTypeList = db.TransactionEntityType
                        //   .Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer
                        //   && o.Active == true
                        //   && DateTime.Now >= o.EffectivityDate
                        //   ).ToList();

                        //int TransactionID = 0;
                        //if (TransactionEntityList.Count() > 0)
                        //{
                        //    foreach (var Transaction in TransactionEntityList)
                        //    {
                        //        TransactionModel NewTransaction = new TransactionModel()
                        //        {
                        //            SelectedUserEntityID = (int)UserEntityEnum.Dealer,
                        //            SelectedEntityID = (int)CurrentUser.Details.ReferenceID,
                        //            TransactionTypeID = Transaction.TransactionTypeID,
                        //            Amount = Transaction.Amount,
                        //            CreatedBy = CurrentUser.Details.UserID,
                        //            CreatedDate = DateTime.Now,
                        //            Remarks = string.Concat("Vehicle ID: ", VehicleID),
                        //            VehicleID = VehicleID
                        //        };

                        //        TransactionID = Functions.InsertTransaction(NewTransaction);
                        //    }


                        //    if (TransactionID > 0)
                        //    {
                        //        db.SaveChanges();
                        //        VehicleIDSuccessList.Add(Update.VehicleID);
                        //        //return true;
                        //    }
                        //    else
                        //    {
                        //        return false;
                        //    }
                        //}
                        //else if (TransactionEntityTypeList.Count() > 0)
                        //{
                        //    foreach (var Transaction in TransactionEntityTypeList)
                        //    {
                        //        TransactionModel NewTransaction = new TransactionModel()
                        //        {
                        //            SelectedUserEntityID = (int)UserEntityEnum.Dealer,
                        //            SelectedEntityID = (int)CurrentUser.Details.ReferenceID,
                        //            TransactionTypeID = Transaction.TransactionTypeID,
                        //            Amount = Transaction.Amount,
                        //            CreatedBy = CurrentUser.Details.UserID,
                        //            CreatedDate = DateTime.Now,
                        //            Remarks = string.Concat("Vehicle ID: ", VehicleID),
                        //            VehicleID = VehicleID
                        //        };

                        //        TransactionID = Functions.InsertTransaction(NewTransaction);
                        //    }

                        //    if (TransactionID > 0)
                        //    {
                        //        db.SaveChanges();
                        //        VehicleIDSuccessList.Add(Update.VehicleID);
                        //        //return true;
                        //    }
                        //    else
                        //    {
                        //        return false;
                        //    }
                        //}
                        //else
                        //{
                        //    return false;
                        //}

                        int TransactionID = 0;
                        if (BranchDealerTransaction.Count > 0)
                        {
                            foreach (var Transaction in BranchDealerTransaction)
                            {
                                TransactionModel NewTransaction = new TransactionModel()
                                {
                                    SelectedUserEntityID = (int)UserEntityEnum.Dealer,
                                    SelectedEntityID = (int)CurrentUser.Details.ReferenceID,
                                    TransactionTypeID = Transaction.TransactionTypeID,
                                    Amount = Transaction.Amount,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                    VehicleID = VehicleID
                                };

                                TransactionID = Functions.InsertTransaction(NewTransaction);

                                if (TransactionID > 0)
                                {
                                    db.SaveChanges();
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else if (DealerTransaction.Count > 0)
                        {
                            foreach (var Transaction in DealerTransaction)
                            {
                                TransactionModel NewTransaction = new TransactionModel()
                                {
                                    SelectedUserEntityID = (int)UserEntityEnum.Dealer,
                                    SelectedEntityID = (int)CurrentUser.Details.ReferenceID,
                                    TransactionTypeID = Transaction.TransactionTypeID,
                                    Amount = Transaction.Amount,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                    VehicleID = VehicleID
                                };

                                TransactionID = Functions.InsertTransaction(NewTransaction);

                                if (TransactionID > 0)
                                {
                                    db.SaveChanges();
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else if (EntityTransaction.Count > 0)
                        {
                            foreach (var Transaction in EntityTransaction)
                            {
                                TransactionModel NewTransaction = new TransactionModel()
                                {
                                    SelectedUserEntityID = (int)UserEntityEnum.Dealer,
                                    SelectedEntityID = (int)CurrentUser.Details.ReferenceID,
                                    TransactionTypeID = Transaction.TransactionTypeID,
                                    Amount = Transaction.Amount,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                    VehicleID = VehicleID
                                };

                                TransactionID = Functions.InsertTransaction(NewTransaction);

                                if (TransactionID > 0)
                                {
                                    db.SaveChanges();
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }

                      

                        //Batch add to list
                        BatchDetailsList.Add(new BatchDetails
                        {
                            VehicleID = Update.VehicleID,
                            TransactionID = TransactionID
                        });
                    }
                    else
                    {
                        return false;
                    }
                }

                //Wallet Threshold
                int EntityID = CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI ? (int)CurrentUser.Details.SubReferenceID : (int)CurrentUser.Details.ReferenceID;
                Functions.IsThreshold(CurrentUser.Details.UserEntityID, EntityID);

                if (BatchDetailsList != null)
                {
                    BatchHeader.BatchTypeID = (int)BatchTypeList.LTO;
                    var rslt = false;
                    var batchID = BatchHeaderInsert(BatchHeader, BatchDetailsList.Count);
                    if (batchID != null)
                    {
                        //Batch Details Insert
                        BatchDetailsList.ForEach(o => o.BatchID = Convert.ToInt32(batchID));
                        rslt = BatchDetailsInsert(BatchDetailsList);
                    }
                    EmailPNP((int)batchID, BatchHeader.ReferenceNo);
                    Functions.LTOEmailStatus((int)batchID, LTOStatus.Submitted);
                    return rslt;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool AllocateVehicle(int VehicleID, int DealerID, int DealerBranchID)
        {

            try
            {
                var UpdateDealer = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                UpdateDealer.DealerID = DealerID;
                UpdateDealer.DealerBranchID = DealerBranchID;
                UpdateDealer.UpdatedBy = CurrentUser.Details.UserID;
                UpdateDealer.UpdatedDate = DateTime.Now;
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool AssigningDealers(int VehicleID, int DealerID, int DealerBranchID)
        {

            try
            {

                var validateCheck = db.vwVehicleList.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID
                                  && o.VehicleID == VehicleID
                                  && o.Assigned == false
                                  && o.BOCCertificateOfPayment != null
                                  && o.CertificateOfStockReport != null
                                  && o.StencilOfEngine != null
                                  && o.StencilOfChasis != null
                                  && o.Active == true).FirstOrDefault();

                if (validateCheck != null)
                {
                    var Update = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                    Update.DealerID = DealerID;
                    Update.DealerBranchID = DealerBranchID;
                    Update.Assigned = true;
                    Update.UpdatedBy = CurrentUser.Details.UserID;
                    Update.UpdatedDate = DateTime.Now;
                    db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool GetCOCAutoManual(COCModel model, int VehicleID, int DealerInsuranceID)
        {
            using (db = new VRSystemEntities())
            {
                try
                {
                    var validateCheck = (from a in db.DealerInvoice
                                         join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                         from temptbl in temp.DefaultIfEmpty()
                                         select new
                                         {
                                             temptbl,
                                         }).Where(o =>
                                         o.temptbl.VehicleID == VehicleID
                                         &&
                                         o.temptbl.CertificateOfStockReport != null
                                         &&
                                         o.temptbl.CertificateOfConformity == null
                                         &&
                                         o.temptbl.DealerID == (int)CurrentUser.Details.ReferenceID
                                         &&
                                         o.temptbl.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                         &&
                                         o.temptbl.Active == true).FirstOrDefault();

                    if (validateCheck != null)
                    {
                        //Get Insurance COC Auto Manual
                        var UpdateVehicle = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                        var Invoice = db.DealerInvoice.Where(o => o.VehicleID == VehicleID).FirstOrDefault();

                        if (Invoice.COC == null || Invoice.COC == string.Empty)
                        {
                            var cocbyte = model.COCFile.ToByte();

                            Invoice.COCAuthenticationCode = model.COCAuthenticationCode.ToString();
                            Invoice.COC = model.COC.ToString();
                            //Invoice.COCPolicyNumber = model.COCPolicyNumber;
                            Invoice.COCInceptionDate = model.InceptionDate;
                            Invoice.COCExpirationDate = model.ExpiryDate;
                            Invoice.InsuranceID = DealerInsuranceID;
                            Invoice.COCByte = cocbyte;
                            Invoice.COCContentType = model.COCFile.ContentType;
                            Invoice.AutoGenerateCOC = model.AutoGenerateCoC;

                            UpdateVehicle.CertificateOfConformity = cocbyte;
                            UpdateVehicle.COCContentType = model.COCFile.ContentType;

                            //InsuranceCOC.CurrentSeries = NextSeries;
                            //var UpdateVehicleCOC = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                            //UpdateVehicleCOC.COCNo = NextSeries.ToString();

                            db.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        private bool GetCOCManual(COCModel model, int VehicleID, int DealerInsuranceID)
        {
            using (db = new VRSystemEntities())
            {
                try
                {
                    var validateCheck = (from a in db.DealerInvoice
                                         join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                         from temptbl in temp.DefaultIfEmpty()
                                         select new
                                         {
                                             temptbl,
                                         }).Where(o =>
                                         o.temptbl.VehicleID == VehicleID
                                         &&
                                         o.temptbl.CertificateOfStockReport != null
                                         &&
                                         o.temptbl.CertificateOfConformity == null
                                         &&
                                         o.temptbl.DealerID == (int)CurrentUser.Details.ReferenceID
                                         &&
                                         o.temptbl.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                         &&
                                         o.temptbl.Active == true).FirstOrDefault();

                    if (validateCheck != null)
                    {
                        //Get Insurance COC
                        var UpdateVehicle = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                        //var InsuranceCOC = db.InsuranceCOCSeries.Where(o => o.InsuranceID == DealerInsuranceID && o.Active == true).FirstOrDefault();
                        var Invoice = db.DealerInvoice.Where(o => o.VehicleID == VehicleID).FirstOrDefault();

                        if (Invoice.COC == null || Invoice.COC == string.Empty)
                        {
                            var cocbyte = model.COCFile.ToByte();
                            //int NextSeries = (InsuranceCOC.CurrentSeries + 1);

                            Invoice.COCAuthenticationCode = model.COCAuthenticationCode.ToString();
                            Invoice.COC = model.COC.ToString();
                            //Invoice.COCPolicyNumber = model.COCPolicyNumber;
                            Invoice.COCInceptionDate = model.InceptionDate;
                            Invoice.COCExpirationDate = model.ExpiryDate;
                            Invoice.InsuranceID = DealerInsuranceID;
                            Invoice.COCByte = cocbyte;
                            Invoice.COCContentType = model.COCFile.ContentType;

                            UpdateVehicle.CertificateOfConformity = cocbyte;
                            UpdateVehicle.COCContentType = model.COCFile.ContentType;

                            //InsuranceCOC.CurrentSeries = NextSeries;
                            //var UpdateVehicleCOC = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                            //UpdateVehicleCOC.COCNo = NextSeries.ToString();

                            db.SaveChanges();
                            return true;
                        }
                        return false;
                        //New Computation
                        //var TransactionEntityList = db.TransactionEntity
                        //   .Where(o => o.EntityID == DealerInsuranceID
                        //   && o.UserEntityID == (int)UserEntityEnum.Insurance
                        //   && o.Active == true
                        //   && DateTime.Now >= o.EffectivityDate
                        //   ).ToList();
                        //var TransactionEntityTypeList = db.TransactionEntityType
                        //   .Where(o => o.UserEntityID == (int)UserEntityEnum.Insurance
                        //   && o.Active == true
                        //   && DateTime.Now >= o.EffectivityDate
                        //   ).ToList();

                        //if (TransactionEntityList.Count() > 0)
                        //{
                        //    foreach (var Transaction in TransactionEntityList)
                        //    {
                        //        TransactionModel NewTransaction = new TransactionModel()
                        //        {
                        //            SelectedUserEntityID = (int)UserEntityEnum.Insurance,
                        //            SelectedEntityID = DealerInsuranceID,
                        //            TransactionTypeID = Transaction.TransactionTypeID,
                        //            Amount = Transaction.Amount,
                        //            CreatedBy = CurrentUser.Details.UserID,
                        //            CreatedDate = DateTime.Now,
                        //            Remarks = string.Concat("Vehicle ID: ", VehicleID),
                        //            VehicleID = VehicleID
                        //        };

                        //        Functions.InsertTransaction(NewTransaction);
                        //    }

                        //    //return true;
                        //    var success = CoCReport(VehicleID);

                        //    if (success == true)
                        //    {
                        //        return true;
                        //    }
                        //    else
                        //    {
                        //        return false;
                        //        //TempData["ErrorMessage"] = "An error has occured.";
                        //    }
                        //    //TempData["SuccessMessage"] = "COC Registration successful!";
                        //    //return RedirectToAction("CoCReport", "VehicleInfo", new { VehicleID = Vehicle.VehicleID });
                        //}
                        //else if (TransactionEntityTypeList.Count() > 0)
                        //{
                        //    foreach (var Transaction in TransactionEntityTypeList)
                        //    {
                        //        TransactionModel NewTransaction = new TransactionModel()
                        //        {
                        //            SelectedUserEntityID = (int)UserEntityEnum.Insurance,
                        //            SelectedEntityID = DealerInsuranceID,
                        //            TransactionTypeID = Transaction.TransactionTypeID,
                        //            Amount = Transaction.Amount,
                        //            CreatedBy = CurrentUser.Details.UserID,
                        //            CreatedDate = DateTime.Now,
                        //            Remarks = string.Concat("Vehicle ID: ", VehicleID),
                        //            VehicleID = VehicleID
                        //        };

                        //        Functions.InsertTransaction(NewTransaction);
                        //    }

                        //    //return true;
                        //    var success = CoCReport(VehicleID);

                        //    if (success == true)
                        //    {
                        //        return true;
                        //    }
                        //    else
                        //    {
                        //        //TempData["ErrorMessage"] = "An error has occured.";
                        //        return false;
                        //    }
                        //    //return RedirectToAction("CoCReport", "VehicleInfo", new { VehicleID = Vehicle.VehicleID });

                        //}
                        //else
                        //{
                        //    return false;
                        //}
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        private bool GetCOC(COCModel model, int VehicleID, int DealerInsuranceID)
        {
            {
                try
                {
                    using (db = new VRSystemEntities())
                    {
                        var validateCheck = (from a in db.DealerInvoice
                                             join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                             from temptbl in temp.DefaultIfEmpty()
                                             select new
                                             {
                                                 temptbl,
                                             }).Where(o =>
                                             o.temptbl.VehicleID == VehicleID
                                             &&
                                             o.temptbl.CertificateOfStockReport != null
                                             &&
                                             o.temptbl.CertificateOfConformity == null
                                             &&
                                             o.temptbl.DealerID == (int)CurrentUser.Details.ReferenceID
                                             &&
                                             o.temptbl.DealerBranchID == (int)CurrentUser.Details.SubReferenceID
                                             &&
                                             o.temptbl.Active == true).FirstOrDefault();

                        if (
                            validateCheck != null && 
                            validateCheck.temptbl.GrossVehicleWeight != null && 
                            validateCheck.temptbl.VehicleColorID != null && 
                            validateCheck.temptbl.VehicleBodyTypeName != null && 
                            validateCheck.temptbl.VehicleModelID != null && 
                            validateCheck.temptbl.VehicleMakeID != null
                            )
                        {
                            //Get Insurance COC
                            var InsuranceCOC = db.InsuranceCOCSeries.Where(o => o.InsuranceID == DealerInsuranceID && o.Active == true).FirstOrDefault();
                            var Invoice = db.DealerInvoice.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                            var CTPL = db.CTPL.Where(o => o.VehicleClassificationID == model.VehicleClassificationID).FirstOrDefault();
                            var CTPLTerm = db.CTPLTerm.Where(o => o.CPTLTermID == CTPL.CTPLTermID).FirstOrDefault();
                            var InsurancePrefix = db.Insurance.Where(o => o.InsuranceID == DealerInsuranceID).Select(o => o.COCPrefix).FirstOrDefault();

                            if (Invoice.COC == null || Invoice.COC == string.Empty)
                            {
                                int NextSeries = (InsuranceCOC.CurrentSeries + 1);

                                //Invoice.COCAuthenticationCode = Functions.RandomString(20);
                                Invoice.COC = InsurancePrefix + NextSeries.ToString();
                                Invoice.InsuranceID = DealerInsuranceID;
                                Invoice.COCInceptionDate = DateTime.Now;
                                Invoice.COCExpirationDate = DateTime.Now.AddYears(CTPLTerm.CoverageYear);
                                Invoice.COCBasicPremium = CTPL.BasicPremium;
                                Invoice.COCVAT = CTPL.VAT;
                                Invoice.COCDST = CTPL.DST;
                                Invoice.COCLGT = CTPL.LGT;
                                Invoice.COCTaxes = CTPL.Taxes;
                                Invoice.COCAuthenticationFee = CTPL.AuthenticationFee;
                                Invoice.COCPremium = CTPL.GrossPremium;
                                Invoice.COCPolicyNumber = InsuranceCOC.MasterPolicy;
                                Invoice.AutoGenerateCOC = true;

                                Invoice.VehicleTypeID = model.VehicleTypeID;
                                Invoice.VehicleClassificationID = CTPL.VehicleClassificationID;
                                Invoice.TaxTypeID = model.TaxTypeID;

                                InsuranceCOC.CurrentSeries = NextSeries;
                                //var UpdateVehicleCOC = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                                //UpdateVehicleCOC.COCNo = NextSeries.ToString();

                                db.SaveChanges();

                                //New Computation
                                var TransactionEntityList = db.TransactionEntity
                              .Where(o => o.EntityID == DealerInsuranceID
                              && o.UserEntityID == (int)UserEntityEnum.Insurance
                              && o.Active == true
                              && DateTime.Now >= o.EffectivityDate
                              ).ToList();
                                var TransactionEntityTypeList = db.TransactionEntityType
                                   .Where(o => o.UserEntityID == (int)UserEntityEnum.Insurance
                                   && o.Active == true
                                   && DateTime.Now >= o.EffectivityDate
                                   ).ToList();

                                if (TransactionEntityList.Count() > 0)
                                {
                                    foreach (var Transaction in TransactionEntityList)
                                    {
                                        TransactionModel NewTransaction = new TransactionModel()
                                        {
                                            SelectedUserEntityID = (int)UserEntityEnum.Insurance,
                                            SelectedEntityID = DealerInsuranceID,
                                            TransactionTypeID = Transaction.TransactionTypeID,
                                            Amount = Transaction.Amount,
                                            CreatedBy = CurrentUser.Details.UserID,
                                            CreatedDate = DateTime.Now,
                                            Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                            VehicleID = VehicleID
                                        };

                                        Functions.InsertTransaction(NewTransaction);
                                    }



                                }
                                else if (TransactionEntityTypeList.Count() > 0)
                                {
                                    foreach (var Transaction in TransactionEntityTypeList)
                                    {
                                        TransactionModel NewTransaction = new TransactionModel()
                                        {
                                            SelectedUserEntityID = (int)UserEntityEnum.Insurance,
                                            SelectedEntityID = DealerInsuranceID,
                                            TransactionTypeID = Transaction.TransactionTypeID,
                                            Amount = Transaction.Amount,
                                            CreatedBy = CurrentUser.Details.UserID,
                                            CreatedDate = DateTime.Now,
                                            Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                            VehicleID = VehicleID
                                        };

                                        Functions.InsertTransaction(NewTransaction);
                                    }

                                    //bool successAuth = false;
                                    //if (Invoice.COCAuthenticationCode == null)
                                    //    successAuth = cocGetAuthentication(VehicleID);
                                    //else
                                    //    successAuth = true;

                                    //if (successAuth)
                                    //{
                                    //    var success = CoCReport(VehicleID);

                                    //    if (success == true)
                                    //    {
                                    //        return true;
                                    //    }
                                    //    else
                                    //    {
                                    //        return false;
                                    //    }
                                    //}
                                    //else
                                    //    return false;

                                }
                                else
                                {
                                    return false;
                                }
                            }

                            //Wallet Threshold
                            //int EntityID = CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI ? (int)CurrentUser.Details.SubReferenceID : (int)CurrentUser.Details.ReferenceID;
                            Functions.IsThreshold((int)UserEntityEnum.Insurance, DealerInsuranceID);

                            //Check if Authorize
                            bool successAuth = false;
                            if (Invoice.COCAuthenticationCode == null)
                                successAuth = cocGetAuthentication(VehicleID);
                            else
                                successAuth = true;


                            if (successAuth)
                            {
                                var success = CoCReport(VehicleID);

                                if (success == true)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;

                                }
                            }
                            else
                                return false;

                        }
                        else
                        {
                            var WarningMessage1 = "";
                            var WarningMessage2 = "";
                            var WarningMessage3 = "";
                            var WarningMessage4 = "";
                            var WarningMessage5 = "";
                            if (validateCheck.temptbl.VehicleMakeID == null)
                                WarningMessage1 = "Vehicle Make,";
                            if (validateCheck.temptbl.VehicleModelID == null)
                                WarningMessage2 = "Vehicle Model,";
                            if (validateCheck.temptbl.VehicleBodyTypeName == null)
                                WarningMessage3 = "Vehicle Body Type,";
                            if (validateCheck.temptbl.VehicleColorID == null)
                                WarningMessage4 = "Vehicle Color,";
                            if (validateCheck.temptbl.GrossVehicleWeight == null)
                                WarningMessage5 = "Vehicle Gross Weight,";


                            var AllMessage = "";
                            if (WarningMessage1 != "")
                                AllMessage = WarningMessage1 + "<br />";
                            if (WarningMessage2 != "")
                                AllMessage = AllMessage + WarningMessage2 + "<br />";
                            if (WarningMessage3 != "")
                                AllMessage = AllMessage + WarningMessage3 + "<br />";
                            if (WarningMessage4 != "")
                                AllMessage = AllMessage + WarningMessage4 + "<br />";
                            if (WarningMessage5 != "")
                                AllMessage = AllMessage + WarningMessage5 + "<br />";

                            TempData["WarningMessage"] = MvcHtmlString.Create(AllMessage + "Required for COC auto genration!");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                #region [ Old Computation ]

                //COC Fee
                //    decimal GetAmount = 0;
                //if (db.TransactionEntity
                //    .Where(o => o.EntityID == (int)Vehicle.DealerID && o.UserEntityID == (int)UserEntityEnum.Dealer
                //    && o.Active == true
                //    && DateTime.Now >= o.EffectivityDate
                //    && o.TransactionTypeID == 3)
                //    .Count() > 0)
                //{
                //    GetAmount = db.TransactionEntity
                //        .Where(o => o.EntityID == (int)Vehicle.DealerID
                //        && o.UserEntityID == (int)UserEntityEnum.Dealer
                //        && o.Active == true
                //        && DateTime.Now >= o.EffectivityDate
                //        && o.TransactionTypeID == 3)
                //        .FirstOrDefault()
                //        .Amount;
                //}
                //else if (db.TransactionEntityType
                //        .Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer
                //        && o.Active == true
                //        && DateTime.Now >= o.EffectivityDate
                //        && o.TransactionTypeID == 3)
                //        .Count() > 0)
                //{
                //    GetAmount = db.TransactionEntityType
                //        .Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer
                //        && o.Active == true
                //        && DateTime.Now >= o.EffectivityDate
                //        && o.TransactionTypeID == 3)
                //        .FirstOrDefault()
                //        .Amount;
                //}
                //else
                //{
                //    TempData["ErrorMessage"] = "No default amount set.";
                //    return View(Vehicle);
                //}


                //TransactionModel NewTransaction = new TransactionModel()
                //{
                //    SelectedUserEntityID = (int)UserEntityEnum.Dealer,
                //    SelectedEntityID = (int)Vehicle.DealerID,
                //    TransactionTypeID = 3,
                //    Amount = GetAmount,
                //    CreatedBy = CurrentUser.Details.UserID,
                //    CreatedDate = DateTime.Now,
                //    Remarks = string.Concat("Vehicle ID: ", Vehicle.VehicleID)
                //};

                //bool success = Functions.InsertTransaction(NewTransaction);

                ////Authentication Fee
                //if (db.TransactionEntity
                //    .Where(o => o.EntityID == (int)Vehicle.DealerID && o.UserEntityID == (int)UserEntityEnum.Dealer
                //    && o.Active == true
                //    && DateTime.Now >= o.EffectivityDate
                //    && o.TransactionTypeID == 4)
                //    .Count() > 0)
                //{
                //    GetAmount = db.TransactionEntity
                //        .Where(o => o.EntityID == (int)Vehicle.DealerID
                //        && o.UserEntityID == (int)UserEntityEnum.Dealer
                //        && o.Active == true
                //        && DateTime.Now >= o.EffectivityDate
                //        && o.TransactionTypeID == 4)
                //        .FirstOrDefault()
                //        .Amount;
                //}
                //else if (db.TransactionEntityType
                //        .Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer
                //        && o.Active == true
                //        && DateTime.Now >= o.EffectivityDate
                //        && o.TransactionTypeID == 4)
                //        .Count() > 0)
                //{
                //    GetAmount = db.TransactionEntityType
                //        .Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer
                //        && o.Active == true
                //        && DateTime.Now >= o.EffectivityDate
                //        && o.TransactionTypeID == 4)
                //        .FirstOrDefault()
                //        .Amount;
                //}
                //else
                //{
                //    TempData["ErrorMessage"] = "No default amount set.";
                //    return View(Vehicle);
                //}


                //TransactionModel NewTransaction2 = new TransactionModel()
                //{
                //    SelectedUserEntityID = (int)UserEntityEnum.Dealer,
                //    SelectedEntityID = (int)Vehicle.DealerID,
                //    TransactionTypeID = 4,
                //    Amount = GetAmount,
                //    CreatedBy = CurrentUser.Details.UserID,
                //    CreatedDate = DateTime.Now,
                //    Remarks = string.Concat("Vehicle ID: ", Vehicle.VehicleID)
                //};

                //bool success2 = Functions.InsertTransaction(NewTransaction2);

                //if (success2 && success)
                //{
                //    TempData["SuccessMessage"] = "COC Registration successful!";
                //    return RedirectToAction("CoCReport", "VehicleInfo", new { VehicleID = Vehicle.VehicleID });
                //}
                //else
                //    TempData["ErrorMessage"] = "An error has occured.";
                #endregion
            }
        }

        private bool CheckAvailableBalance(int NumberOfVehicles, UserEntityEnum EntityType, int EntityID)
        {
            decimal TotalAmount = 0;



            var TransactionList = db.TransactionEntity
                                   .Where(o => o.EntityID == EntityID
                                   && o.UserEntityID == (int)EntityType
                                   && o.Active == true
                                   && DateTime.Now >= o.EffectivityDate
                                   ).ToList();

            if (TransactionList.Count > 0)
            {
                foreach (var Transaction in TransactionList)
                {
                    TotalAmount += Transaction.Amount;
                }
            }
            else
            {
                var TransactionEntityTypeList = db.TransactionEntityType
                                                  .Where(o => o.UserEntityID == (int)EntityType
                                                  && o.Active == true
                                                  && DateTime.Now >= o.EffectivityDate
                                                  ).ToList();
                foreach (var Transaction in TransactionEntityTypeList)
                {
                    TotalAmount += Transaction.Amount;
                }
            }


            var EntityWallet = Functions.GetWalletDetails((int)EntityType, EntityID);

            if (EntityWallet.AvailableBalance - (TotalAmount * NumberOfVehicles) >= 0) return true;

            return false;
        }
        private bool CSRSumbit(List<int> VehicleIDList)
        {
            try
            {
                BatchHeaderModel BatchHeader = (BatchHeaderModel)TempData["BatchHeader"];
                //BatchList
                List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                foreach (var VehicleID in VehicleIDList)
                {
                    Random r = new Random();
                    decimal RandomTax = r.Next(800, 1000);

                    //Generate Random CSR
                    string path = Path.Combine(Server.MapPath("~/Scripts/Img"), "csr.jpg");
                    var UpdateCSR = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();

                    UpdateCSR.CertificateOfStockReport = System.IO.File.ReadAllBytes(path);
                    UpdateCSR.CSRContentType = "image/jpeg";

                    UpdateCSR.TransactionID = r.Next(10000000, 99999999).ToString();
                    UpdateCSR.CSRNumber = r.Next(10000000, 99999999).ToString();

                    UpdateCSR.ReportEntryID = r.Next(10000000, 99999999).ToString();
                    UpdateCSR.ReportDate = DateTime.Now;
                    UpdateCSR.ItemType = r.Next(1, 3).ToString();

                    UpdateCSR.BIRCCMV = Functions.RandomString(9);
                    UpdateCSR.DateIssued3 = DateTime.Now;

                    UpdateCSR.TaxType = r.Next(1, 3).ToString();
                    UpdateCSR.TaxAmount = r.Next(800, 1000);

                    UpdateCSR.UpdatedBy = CurrentUser.Details.UserID;
                    UpdateCSR.UpdatedDate = DateTime.Now;

                    db.SaveChanges();


                    //New Computation
                    var TransactionEntityList = db.TransactionEntity
                       .Where(o => o.EntityID == CurrentUser.Details.SubReferenceID
                       && o.UserEntityID == (int)UserEntityEnum.MAI
                       && o.Active == true
                       && DateTime.Now >= o.EffectivityDate
                       ).ToList();

                    var TransactionEntityTypeList = db.TransactionEntityType
                       .Where(o => o.UserEntityID == (int)UserEntityEnum.MAI
                       && o.Active == true
                       && DateTime.Now >= o.EffectivityDate
                       ).ToList();

                    int TransactionID = 0;
                    if (TransactionEntityList.Count() > 0)
                    {
                        foreach (var Transaction in TransactionEntityList)
                        {
                            TransactionModel NewTransaction = new TransactionModel()
                            {
                                SelectedUserEntityID = (int)UserEntityEnum.MAI,
                                SelectedEntityID = (int)CurrentUser.Details.SubReferenceID,
                                TransactionTypeID = Transaction.TransactionTypeID,
                                Amount = Transaction.Amount,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                VehicleID = VehicleID
                            };

                            TransactionID = Functions.InsertTransaction(NewTransaction);
                        }


                        if (TransactionID > 0)
                        {
                            //return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (TransactionEntityTypeList.Count() > 0)
                    {
                        foreach (var Transaction in TransactionEntityTypeList)
                        {
                            TransactionModel NewTransaction = new TransactionModel()
                            {
                                SelectedUserEntityID = (int)UserEntityEnum.MAI,
                                SelectedEntityID = (int)CurrentUser.Details.SubReferenceID,
                                TransactionTypeID = Transaction.TransactionTypeID,
                                Amount = Transaction.Amount,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                VehicleID = VehicleID
                            };

                            TransactionID = Functions.InsertTransaction(NewTransaction);
                        }

                        if (TransactionID > 0)
                        {
                            //return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    //Batch add to list
                    BatchDetailsList.Add(new BatchDetails
                    {
                        VehicleID = UpdateCSR.VehicleID,
                        TransactionID = TransactionID
                    });
                }

                if (BatchDetailsList != null)
                {
                    var rslt = true;
                    var batchID = BatchHeaderInsert(BatchHeader, BatchDetailsList.Count);
                    if (batchID != null)
                    {

                        //Batch Details Insert
                        BatchDetailsList.ForEach(o => o.BatchID = Convert.ToInt32(batchID));
                        rslt = BatchDetailsInsert(BatchDetailsList);
                    }
                    return rslt;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public int? BatchHeaderInsert(BatchHeaderModel batch, int vehicleCount)
        {
            try
            {
                ModelState.Clear();
                if (TryValidateModel(batch))
                {

                    DateTime? Datesubmit = null;
                    if (batch.BatchTypeID == 4 || batch.BatchTypeID == (int)BatchTypeList.LTOCSR)
                    {
                        Datesubmit = DateTime.Now;
                    }
                    //Header
                    int entityID = 0;

                    switch (CurrentUser.Details.UserEntityID)
                    {
                        case (int)UserEntityEnum.MAI:
                            entityID = (int)CurrentUser.Details.SubReferenceID;
                            break;
                        default:
                            entityID = (int)CurrentUser.Details.ReferenceID;
                            break;
                    }
                    var NewBatch = new BatchMaster
                    {
                        BatchTypeID = batch.BatchTypeID,
                        ReferenceNo = batch.ReferenceNo,
                        BatchDescription = batch.BatchDescription,
                        BatchCount = vehicleCount,
                        DateSubmitted = Datesubmit,
                        EntityTypeID = CurrentUser.Details.UserEntityID,
                        UserReference = (int)CurrentUser.Details.ReferenceID,
                        UserSubRef = (int)CurrentUser.Details.SubReferenceID,
                        CreatedDate = DateTime.Now,
                        CreatedBy = CurrentUser.Details.UserID,
                        Active = true
                    };
                    db.BatchMaster.Add(NewBatch);
                    db.SaveChanges();

                    return NewBatch.BatchID;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool BatchDetailsInsert(List<BatchDetails> BatchDetails)
        {
            try
            {
                //Deatails
                foreach (var item in BatchDetails)
                {
                    var NewBatchDetails = new BatchDetails
                    {
                        BatchID = item.BatchID,
                        VehicleID = item.VehicleID,
                        TransactionID = item.TransactionID,
                        CreatedDate = DateTime.Now,
                        CreatedBy = CurrentUser.Details.UserID
                    };
                    db.BatchDetails.Add(NewBatchDetails);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool BatchMaster(BatchHeaderModel batch, List<vwVehicleListModel> vehicle, int? transactionID)
        {
            try
            {
                ModelState.Clear();
                if (TryValidateModel(batch))
                {
                    //Header
                    var NewBatch = db.BatchMaster.Where(o => o.BatchTypeID == batch.BatchTypeID && o.ReferenceNo == batch.ReferenceNo).FirstOrDefault();
                    if (NewBatch == null)
                    {
                        NewBatch = new BatchMaster
                        {
                            BatchTypeID = batch.BatchTypeID,
                            ReferenceNo = batch.ReferenceNo,
                            BatchDescription = batch.BatchDescription,
                            BatchCount = vehicle.Count,
                            BatchAmount = null,
                            DateSubmitted = null,
                            CreatedDate = DateTime.Now,
                            CreatedBy = CurrentUser.Details.UserID,
                            Active = true
                        };
                        db.BatchMaster.Add(NewBatch);
                        db.SaveChanges();
                    }

                    //Deatails
                    foreach (var item in vehicle)
                    {
                        if (item.isChecked == true)
                        {
                            var NewBatchDetails = new BatchDetails
                            {
                                BatchID = NewBatch.BatchID,
                                VehicleID = item.VehicleID,
                                TransactionID = transactionID,
                                CreatedDate = DateTime.Now,
                                CreatedBy = CurrentUser.Details.UserID
                            };
                            db.BatchDetails.Add(NewBatchDetails);
                            db.SaveChanges();
                        }
                    }


                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CoCVerification(string COCNumber, int vehicleID)
        {
            using (var client = new HttpClient())
            {
                var postjob = client.GetAsync("https://plgicws.paramount.com.ph/COCAFService/COCAFService.svc/verify?username=WSUSER&password=W3b4pp1950&cocNo=" + COCNumber + "");
                postjob.Wait();

                var postresult = postjob.Result;

                var readjob = postresult.Content.ReadAsStringAsync();
                readjob.Wait();
                var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(readjob.Result);
                if (response["cocNo"] != null)
                {
                    var update = db.VehicleInfo.Where(o => o.VehicleID == vehicleID).FirstOrDefault();
                    update.CoCVerified = true;
                    update.UpdatedBy = CurrentUser.Details.UserID;
                    update.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Your COC number has been verified!";
                    return true;
                }
                else
                {
                    TempData["ErrorMessage"] = response["errorMessage"];
                    return false;
                }
            }
        }

        public bool cocGetAuthentication(int VehicleID)
        {
            using (db = new VRSystemEntities())
            {
                if (ConfigurationManager.AppSettings["ServerType"].ToString() == "Production")
                {
                    using (var client = new HttpClient())
                    {
                        //REAL AUTHORIZATION TO PARAMOUNT
                        var builder = new UriBuilder("https://plgicws.paramount.com.ph/COCAFService/COCAFService.svc/register");
                        builder.Port = -1;
                        var query = HttpUtility.ParseQueryString(builder.Query);
                        query["username"] = ConfigurationManager.AppSettings["ParamountCOCAFUserName"].ToString();
                        query["password"] = ConfigurationManager.AppSettings["ParamountCOCAFPassword"].ToString();

                        var COCInfo = db.DealerInvoice.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                        var VehicleInfo = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).Select(o =>
                                        new VehicleInfoModel()
                                        {
                                            VehicleID = o.VehicleID,
                                            EngineNumber = o.EngineNumber,
                                            ChassisNumber = o.ChassisNumber
                                        }).FirstOrDefault();

                        query["regType"] = "N";
                        query["cocNo"] = COCInfo.COC; //10 characters with 3 company code as prefix
                        query["plateNo"] = "";
                        query["mvFileNo"] = "";
                        query["engineNo"] = VehicleInfo.EngineNumber;
                        query["chassisNo"] = VehicleInfo.ChassisNumber;
                        query["inceptionDate"] = ((DateTime)COCInfo.COCInceptionDate).ToString("MM/dd/yyyy"); //format = mm/dd/yyyy
                        query["expiryDate"] = ((DateTime)COCInfo.COCExpirationDate).ToString("MM/dd/yyyy"); //format = mm/dd/yyyy
                        query["mvType"] = db.VehicleType.Where(o => o.VehicleTypeID == COCInfo.VehicleTypeID).FirstOrDefault().VehicleCode;
                        query["mvPremium"] = COCInfo.VehicleClassificationID.ToString();
                        query["taxType"] = COCInfo.TaxTypeID.ToString();

                        var CustomerInfo = db.Customer.Where(o => o.CustomerID == COCInfo.CustomerID).Select(o =>
                        new CustomerModel()
                        {
                            FirstName = o.FirstName,
                            MiddleName = o.MiddleName,
                            LastName = o.LastName,
                            OrganizationName = o.OrganizationName,
                            TIN = o.TIN,
                            OrganizationTIN = o.OrganizationTIN,
                            TitleID = o.TitleID
                        }).FirstOrDefault();
                        var TitleTypeID = db.Title.Where(o => o.TitleID == CustomerInfo.TitleID).FirstOrDefault().TitleTypeID;
                        query["assuredName"] = TitleTypeID == 2 ? CustomerInfo.OrganizationName : CustomerInfo.FirstName + " " + CustomerInfo.MiddleName +" "+CustomerInfo.LastName;
                        query["assuredTin"] = TitleTypeID == 2 ? CustomerInfo.OrganizationTIN : CustomerInfo.TIN; //12 digit format = XXX-XXXXXX-XXX

                        builder.Query = query.ToString();
                        string url = builder.ToString();

                        //Post to Paramount
                        var postjob = client.GetAsync(url);
                        postjob.Wait();

                        var postresult = postjob.Result;

                        var readjob = postresult.Content.ReadAsStringAsync();
                        readjob.Wait();
                        var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(readjob.Result);

                        if (response["cocNo"] != null)
                        {
                            var update = db.DealerInvoice.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                            update.COCAuthenticationCode = response["authNo"];
                            update.UpdatedBy = CurrentUser.Details.UserID;
                            update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Your COC has been authenticated!";
                            return true;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = response["errorMessage"];
                            return false;
                        }
                    }
                }
                else
                {
                    //FAKE AUTHORIZATION
                    var update = db.DealerInvoice.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                    update.COCAuthenticationCode = Functions.RandomString(10);
                    update.UpdatedBy = CurrentUser.Details.UserID;
                    update.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Your COC has been authenticated!";
                    return true;
                }
            }
        }


        //public void EmailPlateLTO(BatchMaster model)
        //{
        //    //Email
        //    if (ConfigurationManager.AppSettings["LTOEmailRecipient"].ToString() != "")
        //    {
        //        using (db = new VRSystemEntities())
        //        {
        //            //Save File to a Folder
        //            List<string> Recipients = new List<string>();
        //            Recipients.Add(ConfigurationManager.AppSettings["LTOEmailRecipient"].ToString());

        //            string EmailBody;
        //            using (StreamReader srBody = new StreamReader(Server.MapPath(string.Format("~/MailBody/LTOPlateMailBody.html"))))
        //            {
        //                EmailBody = srBody.ReadToEnd();
        //            }


        //            List<string> Attachments = new List<string>();


        //            Attachments.Add(PlateSectionReport(model));

        //            ZohoMailSender.SendEmail("Plate Section Report", EmailBody, Recipients, Attachments);

        //            foreach (var file in Attachments)
        //            {
        //                if (System.IO.File.Exists(file))
        //                    System.IO.File.Delete(file);
        //            }
        //        }
        //    }
        //}
        //public string PlateSectionReport(BatchMaster model)
        //{
        //    LocalReport lr = new LocalReport();
        //    string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "PlateSectionReport.rdlc");
        //    if (System.IO.File.Exists(path))
        //    {
        //        lr.ReportPath = path;
        //    }
        //    //else
        //    //{
        //    //    return RedirectToAction("Index");
        //    //}

        //    DataTable dt = new DataTable("Details");
        //    dt.Columns.Add(new DataColumn("PlateAssignment", typeof(string)));
        //    dt.Columns.Add(new DataColumn("DateSalesInvoice", typeof(DateTime)));
        //    dt.Columns.Add(new DataColumn("CustomerName", typeof(string)));
        //    dt.Columns.Add(new DataColumn("Engine", typeof(string)));
        //    dt.Columns.Add(new DataColumn("Chassis", typeof(string)));
        //    dt.Columns.Add(new DataColumn("Amount", typeof(float)));
        //    dt.Columns.Add(new DataColumn("LBPRefNumber", typeof(string)));
        //    dt.Columns.Add(new DataColumn("TransactisonNumber", typeof(string)));

        //    BatchMaster Header = new BatchMaster();
        //    var DealerInfo = new DealerModel();
        //    float TotalAmountCost = 0;
        //    var TotalVehicle = 0;
        //    using (db = new VRSystemEntities())
        //    {
        //        Header = model;
        //        DealerInfo = db.Dealer.Where(o => o.DealerID == Header.UserReference)
        //                    .Select(o => new DealerModel
        //                    {
        //                        DealerName = o.DealerName,
        //                        BusinessPhone = o.BusinessPhone
        //                    })
        //                    .FirstOrDefault();
        //        //var Details = db.BatchDetails.Where(o => o.BatchID == Header.BatchID).ToList();

        //        var Details = (from a in db.BatchDetails
        //                       where a.BatchID == Header.BatchID
        //                       join b in db.vwVehicleList on a.VehicleID equals b.VehicleID
        //                       join c in db.DealerInvoice on b.VehicleID equals c.VehicleID
        //                       join d in db.vwCustomerList on c.CustomerID equals d.CustomerID into temp
        //                       from temptbl in temp.DefaultIfEmpty()
        //                       select new
        //                       {
        //                           VehicleID = b.VehicleID,
        //                           PlateAssign = c.PreferredEndingPlateNumber,
        //                           DateSalesInvoice = c.InvoiceDate,
        //                           CustomerInfo = temptbl,
        //                           Engine = b.EngineNumber,
        //                           Chassis = b.ChassisNumber,
        //                           Amount = c.VehicleCost,
        //                           LBFRefNumber = "",
        //                           TransactionNumber = ""
        //                       }).ToList();

        //        foreach (var item in Details)
        //        {
        //            var CustomerName = "";
        //            if (item.CustomerInfo.TitleTypeID == 2)
        //            {
        //                CustomerName = item.CustomerInfo.CorpName;
        //            }
        //            else
        //            {
        //                CustomerName = item.CustomerInfo.FirstName + " " + item.CustomerInfo.MiddleName + ", " + item.CustomerInfo.LastName;
        //            }
        //            dt.Rows.Add(
        //                item.PlateAssign,
        //                item.DateSalesInvoice,
        //                CustomerName,
        //                item.Engine,
        //                item.Chassis,
        //                item.Amount,
        //                item.LBFRefNumber,
        //                item.TransactionNumber
        //                );
        //            TotalAmountCost = (float)TotalAmountCost + (float)item.Amount;
        //            TotalVehicle++;
        //        }
        //    }

        //    //string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
        //    //lr.EnableExternalImages = true;
        //    //lr.EnableHyperlinks = true;

        //    ReportParameter[] prm = new ReportParameter[5];
        //    prm[0] = new ReportParameter("DealerNameParameter", DealerInfo.DealerName);
        //    prm[1] = new ReportParameter("ContactNumberParameter", DealerInfo.BusinessPhone);
        //    prm[2] = new ReportParameter("NumberOfUnitsParameter", TotalVehicle.ToString());
        //    prm[3] = new ReportParameter("TransmittalNumberParameter", "Test");
        //    prm[4] = new ReportParameter("TotalParameter", TotalAmountCost.ToString());
        //    lr.SetParameters(prm);

        //    ReportDataSource rds = new ReportDataSource("PlateSectionDetails", dt);
        //    lr.DataSources.Clear();
        //    lr.DataSources.Add(rds);

        //    lr.Refresh();


        //    string reportType = "PDF";
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo = "";

        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderedBytes;

        //    renderedBytes = lr.Render(
        //        reportType,
        //        deviceInfo,
        //        out mimeType,
        //        out encoding,
        //        out fileNameExtension,
        //        out streams,
        //        out warnings);

        //    ////return File(renderedBytes, mimeType);
        //    //Response.Buffer = true;
        //    //Response.Clear();
        //    //Response.ContentType = mimeType;
        //    //Response.AddHeader("content-disposition", "attachment; filename=Plate Section Report." + fileNameExtension);
        //    //Response.BinaryWrite(renderedBytes); // create the file
        //    //Response.End();
        //    ////Response.Flush(); // send it to the client to download

        //    //return File(Response.OutputStream, reportType).ToString();

        //    var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + DealerInfo.DealerName.Trim() + " - " + Header.ReferenceNo.Trim() + ".pdf";

        //    using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
        //    {
        //        fs.Write(renderedBytes, 0, renderedBytes.Length);
        //    }

        //    return pdfPath;
        //}

        public void EmailPNP(int BatchID, string ReferenceNo)
        {
            //Email
            if (ConfigurationManager.AppSettings["PNPEmailRecipient"].ToString() != "")
            {
                using (db = new VRSystemEntities())
                {
                    //Save File to a Folder
                    var AutoPNPBatchDetails = (from bd in db.BatchDetails
                                               join vi in db.VehicleInfo on bd.VehicleID equals vi.VehicleID
                                               join di in db.DealerInvoice on vi.VehicleID equals di.VehicleID
                                               where bd.BatchID == BatchID &&
                                                     vi.AutoPNP == true
                                               select new
                                               {
                                                   bd.VehicleID,
                                                   vi.EngineNumber,
                                                   vi.CertificateOfStockReport,
                                                   vi.CSRContentType,
                                                   vi.PNPReceipt,
                                                   vi.PNPReceiptContentType,
                                                   di.InvoiceByte,
                                                   di.InvoiceContentType
                                               }).ToList();

                    var DealerID = db.BatchMaster.Where(o => o.BatchID == BatchID).FirstOrDefault().UserReference;
                    var DealerName = db.Dealer.Where(o => o.DealerID == DealerID).FirstOrDefault().DealerName;

                    if (AutoPNPBatchDetails.Count > 0)
                    {
                        List<string> Recipients = new List<string>();
                        //Recipients.Add(ConfigurationManager.AppSettings["PNPEmailRecipient"].ToString());
                        Recipients = ConfigurationManager.AppSettings["PNPEmailRecipient"].ToString().Split(',').ToList();

                        string EmailBody;
                        using (StreamReader srBody = new StreamReader(Server.MapPath(string.Format("~/MailBody/PNPMailBody.html"))))
                        {
                            EmailBody = srBody.ReadToEnd();
                        }


                        List<string> Attachments = new List<string>();

                        Attachments.Add(PNPEmailReport(BatchID));

                        foreach (var vehicle in AutoPNPBatchDetails)
                        {
                            var path = Server.MapPath(string.Format("~/Reports/VRTempFiles/"));

                            if (!System.IO.Directory.Exists(path))
                                System.IO.Directory.CreateDirectory(path);

                            //CSR
                            var CSRImage = path + @"CSR_" + vehicle.EngineNumber + vehicle.CSRContentType.convertContentTypeToExtention();
                            System.IO.File.WriteAllBytes(CSRImage, vehicle.CertificateOfStockReport);

                            //Invoice
                            var InvoiceImage = path + @"Invoice_" + vehicle.EngineNumber + vehicle.InvoiceContentType.convertContentTypeToExtention();
                            System.IO.File.WriteAllBytes(InvoiceImage, vehicle.InvoiceByte);

                            //SBR PNP
                            var SBRImage = path + @"SBR_" + vehicle.EngineNumber + vehicle.PNPReceiptContentType.convertContentTypeToExtention();
                            System.IO.File.WriteAllBytes(SBRImage, vehicle.PNPReceipt);

                            //Add Image to Email
                            Attachments.Add(CSRImage);
                            Attachments.Add(InvoiceImage);
                            Attachments.Add(SBRImage);
                        }


                        ZohoMailSender.SendEmail("For PNP = Dealer: " + DealerName + " - Batch: " + ReferenceNo, EmailBody, Recipients, Attachments);

                        foreach (var file in Attachments)
                        {
                            if (System.IO.File.Exists(file))
                                System.IO.File.Delete(file);
                        }
                    }
                }
            }
        }

        public string PNPEmailReport(int BatchID)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "PNPEmailReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            //else
            //{
            //    return RedirectToAction("Index");
            //}

            DataTable dt = new DataTable("PNPEmailReport");
            dt.Columns.Add(new DataColumn("Engine", typeof(string)));
            dt.Columns.Add(new DataColumn("Chassis", typeof(string)));
            dt.Columns.Add(new DataColumn("Make", typeof(string)));
            dt.Columns.Add(new DataColumn("Model", typeof(string)));
            dt.Columns.Add(new DataColumn("InvoiceDate", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Cost", typeof(int)));
            dt.Columns.Add(new DataColumn("PNPReceiptReferenceNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("CSR", typeof(string)));

            BatchMaster Header = new BatchMaster();
            var DealerName = "";
            using (db = new VRSystemEntities())
            {
                Header = db.BatchMaster.Where(o => o.BatchID == BatchID).FirstOrDefault();
                DealerName = db.Dealer.Where(o => o.DealerID == Header.UserReference).Select(o => o.DealerName).FirstOrDefault();
                //var Details = db.BatchDetails.Where(o => o.BatchID == Header.BatchID).ToList();
                var Details = (from a in db.BatchDetails
                               where a.BatchID == Header.BatchID
                               join b in db.vwVehicleList on a.VehicleID equals b.VehicleID
                               join c in db.DealerInvoice on b.VehicleID equals c.VehicleID
                               join d in db.vwCustomerList on c.CustomerID equals d.CustomerID into temp
                               where b.AutoPNP == true
                               from temptbl in temp.DefaultIfEmpty()
                               select new
                               {
                                   Engine = b.EngineNumber,
                                   Chassis = b.ChassisNumber,
                                   Make = b.VehicleMakeName,
                                   Model = b.VehicleModelName,
                                   InvoiceDate = c.InvoiceDate,
                                   CustomerInfo = temptbl,
                                   Cost = c.VehicleCost,
                                   PNPReceiptReferenceNumber = b.PNPReceiptReferenceNumber,
                                   CSR = b.CSRNumber
                               }).ToList();
                foreach (var item in Details)
                {
                    var CustomerName = "";
                    if (item.CustomerInfo.TitleTypeID == 2)
                    {
                        CustomerName = item.CustomerInfo.CorpName;
                    }
                    else
                    {
                        CustomerName = item.CustomerInfo.FirstName + " " + item.CustomerInfo.MiddleName + " " + item.CustomerInfo.LastName;
                    }
                    dt.Rows.Add(
                        item.Engine,
                        item.Chassis,
                        item.Make,
                        item.Model,
                        (DateTime)item.InvoiceDate,
                        CustomerName,
                        (int)item.Cost,
                        item.PNPReceiptReferenceNumber,
                        item.CSR
                        );
                }
            }

            //string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            //lr.EnableExternalImages = true;
            //lr.EnableHyperlinks = true;

            ReportParameter[] prm = new ReportParameter[2];
            prm[0] = new ReportParameter("BatchNumberParameter", Header.ReferenceNo.ToString());
            prm[1] = new ReportParameter("DealerNameParameter", DealerName);
            //prm[2] = new ReportParameter("ReferenceNumberParameter", Header.PaymentRef);
            lr.SetParameters(prm);

            ReportDataSource rds = new ReportDataSource("PNPEmailReport", dt);
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

            ////return File(renderedBytes, mimeType);
            //Response.Buffer = true;
            //Response.Clear();
            //Response.ContentType = mimeType;
            //Response.AddHeader("content-disposition", "attachment; filename=CompletedReport." + fileNameExtension);
            //Response.BinaryWrite(renderedBytes); // create the file
            //Response.Flush(); // send it to the client to download
            var pdfPath = Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + DealerName.Trim() + " - " + Header.ReferenceNo.Trim() + ".pdf";

            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }

            return pdfPath;
        }

        #region [ CSR ]

        
        public ActionResult ForLTO_CSR()
        {
            ViewBag.HasCheckBox = true;
            using (db = new VRSystemEntities())
            {
                var model = new VehicleListModel
                {
                    VehicleList = db.vwVehicleList.Where(o =>
                                                      o.MAIID == CurrentUser.Details.SubReferenceID
                                                      && o.CertificateOfStockReport == null
                                                      && o.Active == true
                                                      && o.StencilOfEngine != null
                                                      && o.StencilOfChasis != null
                                                      && o.BOCCertificateOfPayment != null
                                                      && o.CSRSubmitted != true
                 )
                     .Select(o => new vwVehicleListModel
                     {
                         VehicleID = o.VehicleID,
                         VehicleMakeName = o.VehicleMakeName,
                         VehicleModelName = o.VehicleModelName,
                         Variant = o.Variant,
                         Year = o.Year,
                         EngineNumber = o.EngineNumber,
                         ChassisNumber = o.ChassisNumber,
                         BodyIDNumber = o.BodyIDNumber,
                         isChecked = false
                     }
                     ).ToList()
                };
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult ForLTO_CSR(VehicleListModel model, string submit)
        {
            ViewBag.HasCheckBox = true;
            using (db = new VRSystemEntities())
            {
                //if (model.VehicleList.Where(o => o.isChecked == true).Count() > 25)
                //{
                //    TempData["WarningMessage"] = "25 Vehicles are allowed per batch only!";
                //}
                //else
                //{
                switch (submit.ToLower())
                {
                    case "lto":

                        if (ModelState.IsValid)
                        {
                            //MAI ONLY
                            if (!CheckAvailableBalance(model.VehicleList.Where(o => o.isChecked == true).Count(), (UserEntityEnum)CurrentUser.Details.UserEntityID, (int)CurrentUser.Details.SubReferenceID))
                            {
                                TempData["ErrorMessage"] = "Wallet balance is not enough";
                                return View(model);
                            }

                            List<int> VehicleIDList = new List<int>();
                            foreach (var item in model.VehicleList.Where(o => o.isChecked == true))
                            {
                                if (item.isChecked == true)
                                {

                                    VehicleIDList.Add(item.VehicleID);
                                    //var Update = db.VehicleInfo.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                                    //Update.LTOSubmitted = true;
                                    //Update.UpdatedBy = CurrentUser.Details.UserID;
                                    //Update.UpdatedDate = DateTime.Now;
                                    //db.SaveChanges();
                                    //TempData["SuccessMessage"] = "Vehicle Information has been submitted!";
                                }
                            }

                            if (VehicleIDList != null)
                            {
                                TempData["BatchHeader"] = model.BatchHeader;
                                if (LTOCSRSubmit(VehicleIDList))
                                {
                                    TempData["SuccessMessage"] = "CSR Application is successful!";
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "An error has occured!";
                                }
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "There's something error!";
                        }
                        break;
                }
            }
            model = new VehicleListModel();
            //}
            return View(model);
        }

        private bool LTOCSRSubmit(List<int> VehicleIDList)
        {

            try
            {
                BatchHeaderModel BatchHeader = (BatchHeaderModel)TempData["BatchHeader"];
                //BatchList
                List<BatchDetails> BatchDetailsList = new List<BatchDetails>();
                List<int> VehicleIDSuccessList = new List<int>();
                foreach (var VehicleID in VehicleIDList)
                {
                    var validateCheck = db.vwVehicleList.Where(o =>
                                o.VehicleID == VehicleID
                                &&
                                o.BOCCertificateOfPayment != null
                                &&
                                o.StencilOfChasis != null
                                &&
                                o.StencilOfEngine != null
                                &&
                                o.MAIID == CurrentUser.Details.SubReferenceID
                                &&
                                o.CSRSubmitted != true
                                &&
                                o.Active == true).FirstOrDefault();


                    if (validateCheck != null)
                    {
                        var Update = db.VehicleInfo.Where(o => o.VehicleID == VehicleID).FirstOrDefault();
                        Update.CSRSubmitted = true;
                        Update.UpdatedBy = CurrentUser.Details.UserID;
                        Update.UpdatedDate = DateTime.Now;
                        //db.SaveChanges();

                        //New Computation
                        var TransactionEntityList = db.TransactionEntity
                           .Where(o => o.EntityID == CurrentUser.Details.SubReferenceID
                           && o.UserEntityID == (int)UserEntityEnum.MAI
                           && o.Active == true
                           && DateTime.Now >= o.EffectivityDate
                           ).ToList();

                        var TransactionEntityTypeList = db.TransactionEntityType
                           .Where(o => o.UserEntityID == (int)UserEntityEnum.MAI
                           && o.Active == true
                           && DateTime.Now >= o.EffectivityDate
                           ).ToList();

                        int TransactionID = 0;
                        if (TransactionEntityList.Count() > 0)
                        {
                            foreach (var Transaction in TransactionEntityList)
                            {
                                TransactionModel NewTransaction = new TransactionModel()
                                {
                                    SelectedUserEntityID = (int)UserEntityEnum.MAI,
                                    SelectedEntityID = (int)CurrentUser.Details.SubReferenceID,
                                    TransactionTypeID = Transaction.TransactionTypeID,
                                    Amount = Transaction.Amount,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                    VehicleID = VehicleID
                                };

                                TransactionID = Functions.InsertTransaction(NewTransaction);
                            }


                            if (TransactionID > 0)
                            {
                                db.SaveChanges();
                                VehicleIDSuccessList.Add(Update.VehicleID);
                                //return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (TransactionEntityTypeList.Count() > 0)
                        {
                            foreach (var Transaction in TransactionEntityTypeList)
                            {
                                TransactionModel NewTransaction = new TransactionModel()
                                {
                                    SelectedUserEntityID = (int)UserEntityEnum.MAI,
                                    SelectedEntityID = (int)CurrentUser.Details.SubReferenceID,
                                    TransactionTypeID = Transaction.TransactionTypeID,
                                    Amount = Transaction.Amount,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Remarks = string.Concat("Vehicle ID: ", VehicleID),
                                    VehicleID = VehicleID
                                };

                                TransactionID = Functions.InsertTransaction(NewTransaction);
                            }

                            if (TransactionID > 0)
                            {
                                db.SaveChanges();
                                VehicleIDSuccessList.Add(Update.VehicleID);
                                //return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }

                        //Wallet Threshold
                        int EntityID = CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI ? (int)CurrentUser.Details.SubReferenceID : (int)CurrentUser.Details.ReferenceID;
                        Functions.IsThreshold(CurrentUser.Details.UserEntityID, EntityID);

                        //Batch add to list
                        BatchDetailsList.Add(new BatchDetails
                        {
                            VehicleID = Update.VehicleID,
                            TransactionID = TransactionID
                        });
                    }
                    else
                    {
                        return false;
                    }
                }

                if (BatchDetailsList != null)
                {
                    BatchHeader.BatchTypeID = (int)BatchTypeList.LTOCSR;
                    var rslt = false;
                    var batchID = BatchHeaderInsert(BatchHeader, BatchDetailsList.Count);
                    if (batchID != null)
                    {
                        //Batch Details Insert
                        BatchDetailsList.ForEach(o => o.BatchID = Convert.ToInt32(batchID));
                        rslt = BatchDetailsInsert(BatchDetailsList);
                    }
                    return rslt;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        public ActionResult ForCSR_Submitted()
        {
            using (db = new VRSystemEntities())
            {
                var model = new CSRLTOSubmittedBatch();

                var MAIList = new List<LTOMAIFilter>();

                MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new LTOMAIFilter
                {
                    MAIID = o.MAIID,
                    MAIName = o.MAIName
                }).OrderBy(o => o.MAIName).ToList());

                model.MAIList = MAIList;

                model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                   && o.UserSubRef == CurrentUser.Details.SubReferenceID
                                                   && o.Assessed == false
                                                   && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                   && o.Active == true)
                                              .Select(o => new LTOBatchHeader
                                              {
                                                  BatchID = o.BatchID,
                                                  EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                                                  ReferenceNo = o.ReferenceNo,
                                                  BatchDescription = o.BatchDescription,
                                                  DateSubmitted = o.DateSubmitted,
                                                  BatchCount = o.BatchCount
                                              })
                                              .OrderByDescending(o => o.DateSubmitted)
                                              .ToList();

                model.VehicleList = new List<LTOBatchDetailVehicle>();

                return View(model);
            }
        }
        #endregion
    }
}