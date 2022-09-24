using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Reporting.WebForms;
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
    //[AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
    public class LTOController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: LTO
        public ActionResult Index()
        {
            //List<vwLTOList> mailist = new List<vwLTOList>();
            db.Configuration.LazyLoadingEnabled = false;

            var LTOList = db.LTO.Where(o => o.Active == true).ToList();

            return View(LTOList);
        }
        #region LTOForm Registration
        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult LTORegistration(int? id)
        {
            ViewBag.id = id;

            LTOListModel NewLTO = new LTOListModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                NewLTO.BarangayList = new List<Barangay>();
                NewLTO.CityList = new List<City>();
                NewLTO.ProvinceList = db.Province.Where(o => o.Active == true).ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;


                    NewLTO.WalletDetail = Functions.GetWalletDetails((int)UserEntityEnum.LTO, (int)id);
                    NewLTO.EntityTransaction = db.vwTransactionEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer && o.EntityID == (int)id && o.Active == true).ToList();

                    var Load = db.LTO.Where(o => o.Active == true && o.LTOID == id).ToList().FirstOrDefault();
                    var ProvinceID = db.City.Where(o => o.CityID == Load.CityID).FirstOrDefault().ProvinceID;
                    NewLTO.CityList = db.City.Where(o => o.ProvinceID == ProvinceID).ToList();
                    NewLTO.BarangayList = db.Barangay.Where(o => o.CityID == Load.CityID).ToList();

                    NewLTO.LTOID = Load.LTOID;
                    NewLTO.LTOName = Load.LTOName;
                    NewLTO.EmailAddress = Load.EmailAddress;
                    NewLTO.EmailAddress2 = Load.EmailAddress2;
                    NewLTO.BusinessPhone = Load.BusinessPhone;
                    NewLTO.MobilePhone = Load.MobilePhone;
                    NewLTO.FaxNumber = Load.FaxNumber;
                    NewLTO.TIN = Load.TIN;
                    NewLTO.Website = Load.Website;
                    NewLTO.Address = Load.Address;
                    NewLTO.CityID = Load.CityID;
                    NewLTO.BarangayID = Load.BarangayID;
                    NewLTO.ZipCode = Load.ZipCode;
                    NewLTO.Logo = Load.Logo;
                    NewLTO.LogoByte = Load.LogoByte;
                    NewLTO.Notes = Load.Notes;
                    NewLTO.CreatedBy = Load.CreatedBy;
                    NewLTO.CreatedDate = Load.CreatedDate;
                    NewLTO.Active = Load.Active;
                    NewLTO.SelectedProvinceID = ProvinceID;


                    NewLTO.vwDealerInsuranceModelList = (from a in db.DealerInsurance
                                                         join b in db.Insurance on a.InsuranceID equals b.InsuranceID into temp
                                                         from temptbl in temp.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             LTOID = a.DealerID,
                                                             InsuranceID = temptbl.InsuranceID,
                                                             InsuranceName = temptbl.InsuranceName,
                                                         }).Where(o => o.LTOID == Load.LTOID).Select(
                        o => new vwDealerInsuranceModel()
                        {
                            DealerID = o.LTOID,
                            InsuranceID = o.InsuranceID,
                            InsuranceName = o.InsuranceName,
                        }).ToList();
                }
                return PartialView(NewLTO);
            }
        }
        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        [ValidateAntiForgeryToken]
        public ActionResult LTORegistration(LTOListModel lto, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewLTO = new LTO
                            {
                                Address = lto.Address.Trim(),
                                LTOName = lto.LTOName.Trim(),
                                MobilePhone = lto.MobilePhone?.Trim(),
                                Website = lto.Website?.Trim(),
                                ZipCode = lto.ZipCode?.Trim(),
                                Notes = lto.Notes?.Trim(),
                                Logo = lto.LogoFile != null ? lto.LogoFile.FileName : null,
                                LogoByte = lto.LogoFile != null ? lto.LogoFile.ToByte() : null,
                                FaxNumber = lto.FaxNumber?.Trim(),
                                EmailAddress = lto.EmailAddress.Trim(),
                                EmailAddress2 = lto.EmailAddress2.Trim(),
                                TIN = lto.TIN.Trim(),
                                BusinessPhone = lto.BusinessPhone.Trim(),
                                ProvinceID = lto.ProvinceID,
                                CityID = lto.CityID,
                                BarangayID = lto.BarangayID,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };

                            db.LTO.Add(NewLTO);
                            db.SaveChanges();

                            Functions.Logo(submit, "", lto.LogoFile);
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.LTO.Where(o => o.LTOID == lto.LTOID).FirstOrDefault();
                            Update.Address = lto.Address.Trim();
                            Update.LTOName = lto.LTOName.Trim();
                            Update.MobilePhone = lto.MobilePhone?.Trim();
                            Update.Website = lto.Website.Trim();
                            Update.ZipCode = lto.ZipCode.Trim();
                            Update.Notes = lto.Notes;
                            if (lto.LogoFile != null)
                            {
                                Functions.Logo(submit, Update.Logo, lto.LogoFile);

                                Update.Logo = lto.LogoFile != null ? lto.LogoFile.FileName : null;
                                Update.LogoByte = lto.LogoFile.ToByte();
                            }
                            Update.FaxNumber = lto.FaxNumber?.Trim();
                            Update.EmailAddress = lto.EmailAddress.Trim();
                            Update.EmailAddress2 = lto.EmailAddress2.Trim();
                            Update.TIN = lto.TIN.Trim();
                            Update.BusinessPhone = lto.BusinessPhone.Trim();
                            Update.CityID = lto.CityID;
                            Update.BarangayID = lto.BarangayID;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            TempData["SuccessMessage"] = "Updated Successfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.LTO.Where(o => o.LTOID == lto.LTOID).FirstOrDefault();
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
                lto.CityList = db.City.Where(o => o.Active == true).ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(lto);
            }

        }
        #endregion

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult SubmittedBatch()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOSubmittedBatch();

                var DealerList = new List<LTODealerFilter>();
                //{
                //    new DealerFilter() { DealerID = 0, DealerName = "ALL"}
                //};
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
                        o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                        o.BatchTypeID == (int)BatchTypeList.LTO &&
                        o.Active == true &&
                        o.DateSubmitted != null &&
                        o.Assessed == false &&
                        o.Downloaded == false &&
                        (o.Rejected == false || o.Reprocessed == true)
                    )
                    .Select(o => new LTOBatchHeader
                    {
                        BatchID = o.BatchID,
                        EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                        ReferenceNo = o.ReferenceNo,
                        BatchDescription = o.BatchDescription,
                        DateSubmitted = o.DateSubmitted,
                        BatchCount = o.BatchCount,
                        Remarks = o.ReprocessedRemarks
                    })
                    .OrderByDescending(o => o.DateSubmitted)
                    .ToList();

                //model.BatchList = new List<vwBatchHeader>() { 
                //new vwBatchHeader() { BatchID = 13, ReferenceNo = "123"},
                //new vwBatchHeader() { BatchID = 14, ReferenceNo = "234"},
                //new vwBatchHeader() { BatchID = 3, ReferenceNo = "345"}
                //};

                model.VehicleList = new List<LTOBatchDetailVehicle>();




                return View(model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmittedBatch(LTOSubmittedBatch model, string submit)
        {
            switch (submit)
            {
                case "Download":
                    using (db = new VRSystemEntities())
                    {
                        //Update Header
                        var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                        UpdateHeader.Downloaded = true;
                        UpdateHeader.DownloadedBy = CurrentUser.Details.UserID;
                        UpdateHeader.DownloadedDate = DateTime.Now;
                        db.SaveChanges();


                        string path = Server.MapPath("~/Reports/Excel/LTO_DIY_TEMPLATE.xlsx");

                        using (XLWorkbook wb = new XLWorkbook(path))
                        {
                            var businessinfo = db.Dealer.Where(o => o.DealerID == UpdateHeader.UserReference).FirstOrDefault();
                            var AccreditationNumber = db.DealerBranch
                                .Where(o => o.DealerID == UpdateHeader.UserReference && o.DealerBranchID == UpdateHeader.UserSubRef)
                                .Select(o => o.AccreditationNumber).FirstOrDefault();

                            var BatchVehicleList = db.BatchDetails.Where(o => o.BatchID == UpdateHeader.BatchID).ToList();

                            var ws = wb.Worksheet(1);

                            var ttl_vehicle = 0;
                            var tbl_row = 30;

                            foreach (var item in BatchVehicleList)
                            {
                                var vehicleinfo = db.VehicleInfo.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                                var invoiceinfo = db.DealerInvoice.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                                var customerinfo = db.vwCustomerList.Where(o => o.CustomerID == invoiceinfo.CustomerID).FirstOrDefault();

                                var RegionDesc = customerinfo.RegionNumber + " - " + customerinfo.RegionName;
                                var ProvinceDesc = customerinfo.ProvinceNumber + " - " + customerinfo.ProvinceName;
                                var CityDesc = customerinfo.CityNumber + " - " + customerinfo.CityName;

                                ////TRANSFER DISPOSITION OF STOCKS
                                //ENGINE NUMBER   
                                ws.Cell(tbl_row, 1).Value = vehicleinfo.EngineNumber;
                                //CHASSIS NUMBER  
                                ws.Cell(tbl_row, 2).Value = vehicleinfo.ChassisNumber;
                                //DEALER'S/OPERATOR'S NAME (TRANSFER, TRANSFER TO, TRANSFER FROM) 
                                ws.Cell(tbl_row, 3).Value = "";
                                //ACCREDITATION NUMBER    
                                ws.Cell(tbl_row, 4).Value = "";
                                //INVOICE NUMBER  
                                ws.Cell(tbl_row, 5).Value = "";
                                //INVOICE DATE    
                                ws.Cell(tbl_row, 6).Value = "";
                                //ws.Cell(tbl_row, 6).Style.DateFormat.SetFormat("MM/dd/yyyy");
                                //INVOICE AMOUNT
                                ws.Cell(tbl_row, 7).Value = "";

                                ////SOLD TO DETAILS			
                                //INVOICE NUMBER  
                                ws.Cell(tbl_row, 8).SetValue<string>(invoiceinfo.InvoiceNumber);
                                //ws.Cell(tbl_row, 8).Value = invoiceinfo.InvoiceNumber;
                                //INVOICE DATE
                                ws.Cell(tbl_row, 9).SetValue<string>(invoiceinfo.InvoiceDate.ToString("MM/dd/yyyy"));
                                //ws.Cell(tbl_row, 9).Value = invoiceinfo.InvoiceDate.ToString("MM/dd/yyyy");
                                //ws.Cell(tbl_row, 9).Style.DateFormat.SetFormat("MM/dd/yyyy");
                                //INVOICE AMOUNT
                                ws.Cell(tbl_row, 10).SetValue<string>(invoiceinfo.VehicleCost.ToString("N"));
                                //ws.Cell(tbl_row, 10).Value = invoiceinfo.VehicleCost.ToString();

                                if (customerinfo.TitleTypeID == 2)
                                {
                                    ////ORGANIZATION INFORMATION						
                                    //ORGANIZATION NAME (required)
                                    ws.Cell(tbl_row, 27).Value = customerinfo.CorpName;
                                    //ORG MNEMONIC(required)	
                                    ws.Cell(tbl_row, 28).Value = customerinfo.Alias;
                                    //PRIMARY CONTACT (required)
                                    ws.Cell(tbl_row, 29).Value = customerinfo.ContactNumber;
                                    //CONTACT DETAILS (required)
                                    ws.Cell(tbl_row, 30).Value = customerinfo.ContactNumber;
                                    //EMAIL ADDRESS
                                    ws.Cell(tbl_row, 31).Value = customerinfo.EmailAddress;
                                    //PHONE NO.
                                    ws.Cell(tbl_row, 32).Value = "";
                                    //TAX ID NO.
                                    ws.Cell(tbl_row, 33).Value = customerinfo.OrganizationTIN;


                                    //BUYER TYPE	
                                    ws.Cell(27, 2).Value = "ORG";
                                }
                                else
                                {
                                    ////INDIVIDUAL INFORMATION	
                                    //LAST NAME (required)
                                    ws.Cell(tbl_row, 11).Value = customerinfo.LastName;
                                    //FIRST NAME(required)   
                                    ws.Cell(tbl_row, 12).Value = customerinfo.FirstName;
                                    //MIDDLE NAME(required)  
                                    ws.Cell(tbl_row, 13).Value = customerinfo.MiddleName;
                                    //BIRTHDATE(required)
                                    //ws.Cell(tbl_row, 14).Value = customerinfo.Birthdate.ToString("MM/dd/yyyy");
                                    ws.Cell(tbl_row, 14).SetValue<string>(customerinfo.Birthdate.ToString("MM/dd/yyyy"));
                                    //BIRTH PLACE(required)	
                                    ws.Cell(tbl_row, 15).Value = customerinfo.Birthplace;
                                    //CITIZENSHIP (required)	
                                    ws.Cell(tbl_row, 16).Value = customerinfo.Citizenship;
                                    //CIVIL STATUS(required)	
                                    ws.Cell(tbl_row, 17).Value = customerinfo.CivilStatusName;
                                    //FATHER'S NAME (required)
                                    ws.Cell(tbl_row, 18).Value = customerinfo.FathersName;
                                    //MOTHER'S NAME (required)	
                                    ws.Cell(tbl_row, 19).Value = customerinfo.MothersName;
                                    //SEX (M / F)(required)	
                                    ws.Cell(tbl_row, 20).Value = customerinfo.SexCode;
                                    //HEIGHT (cm) (required)
                                    ws.Cell(tbl_row, 21).Value = customerinfo.Height;
                                    //WEIGHT(kg)(required)
                                    ws.Cell(tbl_row, 22).Value = customerinfo.Weight;
                                    //CONTACT DETAILS (required)	
                                    ws.Cell(tbl_row, 23).Value = customerinfo.ContactNumber;
                                    //EMAIL ADDRESS	
                                    ws.Cell(tbl_row, 24).Value = customerinfo.EmailAddress;
                                    //PHONE NO.	
                                    ws.Cell(tbl_row, 25).Value = customerinfo.ContactNumber;
                                    //TAX ID NO.
                                    ws.Cell(tbl_row, 26).Value = customerinfo.TIN;

                                    //BUYER TYPE	
                                    ws.Cell(27, 2).Value = "IND";
                                }
                                
                                ////ADDRESS INFORMATION								
                                //REGION NO. (required)	
                                ws.Cell(tbl_row, 34).Value = RegionDesc;
                                //BARANGGAY DESCRIPTION(required)	
                                ws.Cell(tbl_row, 35).Value = customerinfo.BarangayName;
                                //MUNICIPALITY DESCRIPTION(required)	
                                ws.Cell(tbl_row, 36).Value = customerinfo.CityName;
                                //PROVINCE DESCRIPTION (required)	
                                ws.Cell(tbl_row, 37).Value = customerinfo.ProvinceName;
                                //HOUSE/BLDG.NO. (required)
                                ws.Cell(tbl_row, 38).Value = customerinfo.HouseBldgNumber;
                                //STREET (required)
                                ws.Cell(tbl_row, 39).Value = customerinfo.StreetSubdivision;
                                //ZIP CODE (required)
                                ws.Cell(tbl_row, 40).Value = customerinfo.ZipCode;
                                //ADDRESS	
                                ws.Cell(tbl_row, 41).Value = "";
                                //AREA (required)
                                ws.Cell(tbl_row, 42).Value = "-";

                                tbl_row++;
                                ttl_vehicle++;
                            }

                            //BUSINESS NAME
                            ws.Cell(2, 3).Value = businessinfo.DealerName;
                            //BUSINESS ADDRESS
                            ws.Cell(3, 3).Value = businessinfo.Address;
                            //ACCREDITATION NO.
                            ws.Cell(4, 3).Value = AccreditationNumber;
                            //REPORT TYPE 	
                            ws.Cell(12, 3).Value = "1";
                            //TOTAL ITEMS
                            ws.Cell(14, 3).Value = ttl_vehicle;
                            //SALES TYPE	
                            ws.Cell(21, 2).Value = "1";


                            //wb.Save();
                            using (MemoryStream stream = new MemoryStream())
                            {
                                wb.SaveAs(stream);
                                var FileName = businessinfo.DealerName + " - " + model.SelectedBatchReferenceNo + ".xlsx";
                                TempData["SuccessMessage"] = "Downloaded is successful!";
                                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);

                            }


                        }
                    }
                case "Reject":
                    using (db = new VRSystemEntities())
                    {
                        //Update Header
                        var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                        UpdateHeader.Rejected = true;
                        UpdateHeader.RejectedBy = CurrentUser.Details.UserID;
                        UpdateHeader.RejectedDate = DateTime.Now;
                        UpdateHeader.RejectedRemarks = model.RejectedRemarks.Trim();
                        db.SaveChanges();

                        model = new LTOSubmittedBatch();

                        var DealerList = new List<LTODealerFilter>();

                        DealerList.Add(new LTODealerFilter() { DealerID = 0, DealerName = "ALL" });
                        DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                        {
                            DealerID = o.DealerID,
                            DealerName = o.DealerName
                        }).OrderBy(o => o.DealerName).ToList());

                        model.DealerList = DealerList;

                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                           && o.BatchTypeID == (int)BatchTypeList.LTO
                                                           && o.Active == true
                                                           && o.DateSubmitted != null
                                                           && o.Assessed == false
                                                           && o.Downloaded == false
                                                           && (o.Rejected == false
                                                           || o.Reprocessed == true))
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.DateSubmitted,
                                                          BatchCount = o.BatchCount,
                                                          Remarks = o.ReprocessedRemarks
                                                      })
                                                      .OrderByDescending(o => o.DateSubmitted)
                                                      .ToList();

                        //model.BatchList = new List<vwBatchHeader>() { 
                        //new vwBatchHeader() { BatchID = 13, ReferenceNo = "123"},
                        //new vwBatchHeader() { BatchID = 14, ReferenceNo = "234"},
                        //new vwBatchHeader() { BatchID = 3, ReferenceNo = "345"}
                        //};

                        model.VehicleList = new List<LTOBatchDetailVehicle>();

                        return View(model);
                    }
                default:
                    return View(model);
            }
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult AssessBatch()
        {
            using (db = new VRSystemEntities())
            {
                var model = new LTOAssessBatch();

                var DealerList = new List<LTODealerFilter>();
                DealerList.Add(new LTODealerFilter() { DealerID = 0, DealerName = "ALL" });
                DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
                {
                    DealerID = o.DealerID,
                    DealerName = o.DealerName
                }).OrderBy(o => o.DealerName).ToList());

                model.DealerList = DealerList;

                model.BatchList = db.BatchMaster
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
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.DownloadedDate,
                            BatchCount = o.BatchCount
                        })
                        .OrderByDescending(o => o.DateSubmitted)
                        .ToList();

                model.VehicleList = new List<LTOAssessBatchDetailVehicle>();

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult AssessBatch(LTOAssessBatch model, string submit)
        {

            using (db = new VRSystemEntities())
            {
                switch (submit)
                {
                    case "assessed":
                        foreach (var vehicle in model.VehicleList)
                        {
                            var UpdateDetails = db.BatchDetails.Where(o => o.BatchID == model.SelectedBatchID && o.VehicleID == vehicle.VehicleID).FirstOrDefault();
                            UpdateDetails.Assessed = true;
                            //UpdateDetails.AssessedAmount = Convert.ToDecimal(vehicle.AssessedAmount);
                            UpdateDetails.AssessedBy = CurrentUser.Details.UserID;
                            UpdateDetails.AssessedDate = DateTime.Now;
                            db.SaveChanges();
                        }

                        if (model.VehicleList.Count > 0)
                        {
                            var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                            UpdateHeader.Assessed = true;
                            UpdateHeader.AssessedBy = CurrentUser.Details.UserID;
                            UpdateHeader.AssessedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Assessment is successful!";

                            Functions.LTOEmailStatus(model.SelectedBatchID, LTOStatus.Assessed);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "There's something error!";
                        }
                        break;
                    case "reject":
                        var Updatereject = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                        Updatereject.Rejected = true;
                        Updatereject.RejectedBy = CurrentUser.Details.UserID;
                        Updatereject.RejectedDate = DateTime.Now;
                        Updatereject.RejectedRemarks = model.RejectRemarks.Trim();

                        Updatereject.Downloaded = false;
                        Updatereject.DateSubmitted = null;
                        db.SaveChanges();
                        TempData["InfoMessage"] = "Reject is successful!";
                        break;
                }
            }

            //using (db = new VRSystemEntities())
            //{

            //    model = new LTOAssessBatch();

            //    var DealerList = new List<LTODealerFilter>();
            //    DealerList.Add(new LTODealerFilter() { DealerID = 0, DealerName = "ALL" });
            //    DealerList.AddRange(db.Dealer.Where(o => o.Active == true).Select(o => new LTODealerFilter
            //    {
            //        DealerID = o.DealerID,
            //        DealerName = o.DealerName
            //    }).OrderBy(o => o.DealerName).ToList());

            //    model.DealerList = DealerList;

            //    model.BatchList = db.BatchMaster
            //        .Where(o =>
            //            o.Active == true &&
            //            o.BatchTypeID == (int)BatchTypeList.LTO &&
            //            o.EntityTypeID == (int)UserEntityEnum.Dealer &&
            //            o.DateSubmitted != null &&
            //            o.Downloaded == true &&
            //            o.Assessed == false &&
            //            o.PaymentRef == null &&
            //            o.Completed == false
            //            )
            //            .Select(o => new LTOBatchHeader
            //            {
            //                BatchID = o.BatchID,
            //                EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
            //                ReferenceNo = o.ReferenceNo,
            //                BatchDescription = o.BatchDescription,
            //                DateSubmitted = o.DownloadedDate,
            //                BatchCount = o.BatchCount
            //            })
            //            .OrderByDescending(o => o.DateSubmitted)
            //            .ToList();

            //    model.VehicleList = new List<LTOAssessBatchDetailVehicle>();

               

            //}
            return RedirectToAction("AssessBatch");
        }

        [HttpGet]
        public ActionResult PaymentBatch()
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
                        o.Completed == false &&
                        o.LTOBranchID == CurrentUser.Details.SubReferenceID
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
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult PaymentBatch(LTOPayment model, string submit)
        {
            switch (submit)
            {
                case "Download":
                    LTOCompletedReport(model.SelectedBatchID);
                    break;
                default:
                    using (db = new VRSystemEntities())
                    {
                        var hasError = false;
                        foreach (var vehicle in model.VehicleList)
                        {
                            ////if (vehicle.isChecked)
                            ////{
                            //    var UpdateDetails = db.BatchDetails.Where(o => o.BatchID == model.SelectedBatchID && o.VehicleID == vehicle.VehicleID).FirstOrDefault();
                            //    UpdateDetails.Completed = true;
                            //    UpdateDetails.CompletedBy = CurrentUser.Details.UserID;
                            //    UpdateDetails.CompletedDate = DateTime.Now;
                            //    db.SaveChanges();
                            //    cnt_sccss++;
                            ////}
                            if (vehicle.Completed == false && vehicle.Rejected == false)
                            {
                                TempData["ErrorMessage"] = "Please complete vehicle status";
                                hasError = true;
                            }
                        }

                        if (hasError == false)
                        {
                            var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                            UpdateHeader.Completed = true;
                            UpdateHeader.CompletedBy = CurrentUser.Details.UserID;
                            UpdateHeader.CompletedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "For OR/CR Processing!";
                        }

                        //
                        model = new LTOPayment();

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
                                o.Completed == false &&
                                o.LTOBranchID == CurrentUser.Details.SubReferenceID
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

                    }
                    break;
            }

          
            return View(model);
        }

        [HttpGet]
        public ActionResult CompletedBatch()
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
                        o.Completed == true &&
                        o.ForPickUp == false &&
                        o.LTOBranchID == CurrentUser.Details.SubReferenceID
                        )
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.AssessedDate,
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
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult CompletedBatch(LTOPayment model, string submit)
        {
            switch (submit)
            {
                case "Report":
                    LTOCompletedReport(model.SelectedBatchID);
                    break;
                case "ForPickUp":
                    using (db = new VRSystemEntities())
                    {
                        //Update Header
                        var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                        UpdateHeader.ForPickUp = true;
                        UpdateHeader.ForPickUpBy = CurrentUser.Details.UserID;
                        UpdateHeader.ForPickUpDate = DateTime.Now;
                        db.SaveChanges();
                        Functions.LTOEmailStatus(model.SelectedBatchID, LTOStatus.ForPickUp);
                        TempData["SuccessMessage"] = "Batch " + UpdateHeader.ReferenceNo + " is now ready for Pick Up!";
                    }
                    break;
                default:
                    break;
            }


            return RedirectToAction("CompletedBatch");
        }

        [HttpGet]
        public ActionResult ForPickUpBatch()
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

                model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                 && o.BatchTypeID == (int)BatchTypeList.LTO
                                                   && o.Assessed == true
                                                   && o.PaymentRef != null
                                                   && o.Completed == true
                                                   && o.ForPickUp == true
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
        [ValidateAntiForgeryToken]
        public ActionResult GetBatchList(int DealerID)
        {
            using (db = new VRSystemEntities())
            {
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                if (DealerID != 0)
                {
                    BatchList = db.BatchMaster.Where(o =>
                                                  o.Active == true
                                                  && o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                  && o.UserReference == DealerID
                                                  && o.BatchTypeID == (int)BatchTypeList.LTO
                                                  && o.DateSubmitted != null
                                                  && o.Assessed == false
                                                  && o.Downloaded == false
                                                  && (o.Rejected == false
                                                  || o.Reprocessed == true))
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
                }
                else
                {
                    BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                 && o.BatchTypeID == (int)BatchTypeList.LTO
                                                 && o.DateSubmitted != null
                                                 && o.Assessed == false
                                                 && o.Downloaded == false
                                                 && o.Rejected == false
                                                 && o.Active == true
                                                 && (o.Rejected == false
                                                 || o.Reprocessed == true))
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
                }
                return PartialView("_BatchList", BatchList);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetVehicleList(int BatchID)
        {
            using (db = new VRSystemEntities())
            {
                var VehicleIDList = db.BatchDetails.Where(o => o.BatchID == BatchID).Select(o => o.VehicleID).ToList();

                //var VehicleList = db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID)).ToList();

                var VehicleList2 = db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID))
                                    .Select(o => new LTOBatchDetailVehicle()
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        EngineNumber = o.EngineNumber,
                                        ChassisNumber = o.ChassisNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        YearOfMake = o.Year
                                    })
                                    .ToList();

                return PartialView("_BatchDetails", VehicleList2);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRejectedVehicleList(int BatchID)
        {
            using (db = new VRSystemEntities())
            {
                var VehicleBatchList = db.BatchDetails.Where(o => o.BatchID == BatchID && (o.Rejected == true && o.Completed == false)).ToList();
                var VehicleBatchID = VehicleBatchList.Select(o => o.VehicleID).ToList();
                //var VehicleList = db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID)).ToList();

                var VehicleList2 = db.vwVehicleList.Where(o => VehicleBatchID.Contains(o.VehicleID))
                                    .Select(o => new LTOBatchDetailVehicle()
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        EngineNumber = o.EngineNumber,
                                        ChassisNumber = o.ChassisNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        YearOfMake = o.Year
                                        //Remarks = VehicleBatchList.Where(a => a.VehicleID == o.VehicleID).FirstOrDefault().RejectedRemarks
                                    })
                                    .ToList();
                foreach(var vehicle in VehicleList2)
                {
                    vehicle.Remarks = VehicleBatchList.Where(o => o.VehicleID == vehicle.VehicleID).FirstOrDefault().RejectedRemarks;
                }

                return PartialView("_BatchDetails", VehicleList2);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDIYRejectedVehicleList(int BatchID)
        {
            using (db = new VRSystemEntities())
            {
                var VehicleBatchList = db.BatchDetails.Where(o => o.BatchID == BatchID).ToList();
                var VehicleBatchID = VehicleBatchList.Select(o => o.VehicleID).ToList();
                //var VehicleList = db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID)).ToList();

                var VehicleList2 = db.vwVehicleList.Where(o => VehicleBatchID.Contains(o.VehicleID))
                                    .Select(o => new LTOBatchDetailVehicle()
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        EngineNumber = o.EngineNumber,
                                        ChassisNumber = o.ChassisNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        YearOfMake = o.Year
                                        //Remarks = VehicleBatchList.Where(a => a.VehicleID == o.VehicleID).FirstOrDefault().RejectedRemarks
                                    })
                                    .ToList();
                foreach (var vehicle in VehicleList2)
                {
                    vehicle.Remarks = VehicleBatchList.Where(o => o.VehicleID == vehicle.VehicleID).FirstOrDefault().RejectedRemarks;
                }

                return PartialView("_BatchDetails", VehicleList2);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetAssessVehicleList(int BatchID)
        {
            using (db = new VRSystemEntities())
            {
                var VehicleIDList = db.BatchDetails.Where(o => o.BatchID == BatchID).Select(o => o.VehicleID).ToList();

                //var VehicleList = db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID)).ToList();

                var VehicleList = (from a in db.BatchDetails
                                   where a.BatchID == BatchID
                                   join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                   from temptbl in temp.DefaultIfEmpty()
                                   select new
                                   {
                                       VehicleID = a.VehicleID,
                                       VehicleMakeName = temptbl.VehicleMakeName,
                                       VehicleModelName = temptbl.VehicleModelName,
                                       Variant = temptbl.Variant,
                                       EngineNumber = temptbl.EngineNumber,
                                       ChassisNumber = temptbl.ChassisNumber,
                                       BodyIDNumber = temptbl.BodyIDNumber,
                                       Year = temptbl.Year,
                                       Assessed = a.Assessed,
                                       //AssessedAmount = a.AssessedAmount
                                   })
                                    .Select(o => new LTOAssessBatchDetailVehicle()
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        EngineNumber = o.EngineNumber,
                                        ChassisNumber = o.ChassisNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        YearOfMake = o.Year,
                                        Assessed = o.Assessed,
                                        //AssessedAmount = o.AssessedAmount.ToString()
                                    }).ToList();

                //db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID))
                //                    .Select(o => new LTOBatchDetailVehicle()
                //                    {
                //                        VehicleID = o.VehicleID,
                //                        VehicleMakeName = o.VehicleMakeName,
                //                        VehicleModelName = o.VehicleModelName,
                //                        Variant = o.Variant,
                //                        EngineNumber = o.EngineNumber,
                //                        ChassisNumber = o.ChassisNumber,
                //                        BodyIDNumber = o.BodyIDNumber,
                //                        YearOfMake = o.YearOfMake
                //                    })
                //                    .ToList();

                return PartialView("_AssessBatchDetails", VehicleList);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetBatchListAssess(int DealerID)
        {
            using (db = new VRSystemEntities())
            {
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                if (DealerID != 0)
                {
                    BatchList = db.BatchMaster
                        .Where(o =>
                        o.Active == true &&
                        o.BatchTypeID == (int)BatchTypeList.LTO &&
                        o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                        o.UserReference == DealerID &&
                        o.DateSubmitted != null &&
                        o.Downloaded == true &&
                        o.Assessed == false &&
                        o.PaymentRef == null &&
                        o.Completed == false
                        )
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
                }
                else
                {
                    BatchList = db.BatchMaster
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
                }
                return PartialView("_BatchList", BatchList);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPaymentBatchList(int DealerID)
        {
            using (db = new VRSystemEntities())
            {
                var BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                   && o.UserReference == DealerID
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
                return PartialView("_PaymentBatchList", BatchList);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPaymentVehicleList(int BatchID)
        {
            using (db = new VRSystemEntities())
            {
                var VehicleIDList = db.BatchDetails.Where(o => o.BatchID == BatchID).Select(o => o.VehicleID).ToList();

                //var VehicleList = db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID)).ToList();

                var VehicleList = (from a in db.BatchDetails
                                   where a.BatchID == BatchID
                                   join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                   from temptbl in temp.DefaultIfEmpty()
                                   select new
                                   {
                                       VehicleID = a.VehicleID,
                                       VehicleMakeName = temptbl.VehicleMakeName,
                                       VehicleModelName = temptbl.VehicleModelName,
                                       Variant = temptbl.Variant,
                                       EngineNumber = temptbl.EngineNumber,
                                       ChassisNumber = temptbl.ChassisNumber,
                                       BodyIDNumber = temptbl.BodyIDNumber,
                                       Year = temptbl.Year,
                                       Assessed = a.Assessed,
                                       //AssessedAmount = a.AssessedAmount
                                   })
                                    .Select(o => new LTOAssessBatchDetailVehicle()
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        EngineNumber = o.EngineNumber,
                                        ChassisNumber = o.ChassisNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        YearOfMake = o.Year,
                                        Assessed = o.Assessed,
                                        //AssessedAmount = o.AssessedAmount.ToString()
                                    }).ToList();

                return PartialView("_PaymentBatchDetails", VehicleList);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetProcessBatchList(int DealerID)
        {
            using (db = new VRSystemEntities())
            {
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                if (DealerID != 0)
                {
                    BatchList = db
                        .BatchMaster
                        .Where(o =>
                            o.Active == true &&
                            o.BatchTypeID == (int)BatchTypeList.LTO &&
                            o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                            o.UserReference == DealerID &&
                            o.DateSubmitted != null &&
                            o.Downloaded == true &&
                            o.Assessed == true &&
                            o.PaymentRef != null &&
                            o.Completed == false &&
                            o.LTOBranchID == CurrentUser.Details.SubReferenceID
                            )
                            .Select(o => new LTOBatchHeader
                            {
                                BatchID = o.BatchID,
                                EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                ReferenceNo = o.ReferenceNo,
                                BatchDescription = o.BatchDescription,
                                DateSubmitted = o.AssessedDate,
                                BatchCount = o.BatchCount,
                                Assessed = o.Assessed,
                                //AssessedAmount = o.AssessedAmount,
                                PaymentRef = o.PaymentRef,
                                PaymentImageContentType = o.PaymentFileType,
                                EPatImageContentType = o.PaymentEPATFileType
                            })
                            .OrderByDescending(o => o.DateSubmitted)
                            .ToList();
                }
                else
                {
                    BatchList = db
                        .BatchMaster
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
                            .Select(o => new LTOBatchHeader
                            {
                                BatchID = o.BatchID,
                                EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                ReferenceNo = o.ReferenceNo,
                                BatchDescription = o.BatchDescription,
                                DateSubmitted = o.AssessedDate,
                                BatchCount = o.BatchCount,
                                Assessed = o.Assessed,
                                //AssessedAmount = o.AssessedAmount,
                                PaymentRef = o.PaymentRef,
                                PaymentImageContentType = o.PaymentFileType,
                                EPatImageContentType = o.PaymentEPATFileType
                            })
                            .OrderByDescending(o => o.DateSubmitted)
                            .ToList();
                }
                return PartialView("_PaymentBatchList", BatchList);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetProcessVehicleList(int BatchID)
        {
            using (db = new VRSystemEntities())
            {
                var VehicleIDList = db.BatchDetails.Where(o => o.BatchID == BatchID).Select(o => o.VehicleID).ToList();

                //var VehicleList = db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID)).ToList();

                var VehicleList = (from a in db.BatchDetails
                                   where a.BatchID == BatchID
                                   join b in db.vwVehicleList on a.VehicleID equals b.VehicleID into temp
                                   from temptbl in temp.DefaultIfEmpty()
                                   select new
                                   {
                                       VehicleID = a.VehicleID,
                                       VehicleMakeName = temptbl.VehicleMakeName,
                                       VehicleModelName = temptbl.VehicleModelName,
                                       Variant = temptbl.Variant,
                                       EngineNumber = temptbl.EngineNumber,
                                       ChassisNumber = temptbl.ChassisNumber,
                                       BodyIDNumber = temptbl.BodyIDNumber,
                                       Year = temptbl.Year,
                                       Assessed = a.Assessed,
                                       //AssessedAmount = a.AssessedAmount,
                                       Completed = a.Completed,
                                       Rejected = a.Rejected
                                   })
                                    .Select(o => new LTOAssessBatchDetailVehicle()
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        EngineNumber = o.EngineNumber,
                                        ChassisNumber = o.ChassisNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        YearOfMake = o.Year,
                                        Assessed = o.Assessed,
                                        //AssessedAmount = o.AssessedAmount.ToString(),
                                        Completed = o.Completed,
                                        Rejected = o.Rejected
                                    }).ToList();

                foreach(var Vehicle in VehicleList)
                {
                   
                    if (Vehicle.Rejected == true && Vehicle.Completed == false)
                        Vehicle.Status = "Rejected";
                    else if (Vehicle.Completed == true)
                        Vehicle.Status = "Completed";
                }
                return PartialView("_ProcessBatchDetails", VehicleList);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPaymentInfo(int BatchID)
        {
            using (db = new VRSystemEntities())
            {
                var payment = db.BatchMaster.Where(o => o.BatchID == BatchID)
                                    .Select(o => new LTOPaymentInfo()
                                    {
                                        PaymentRef = o.PaymentRef.ToString(),
                                        //PaymentAmount = o.AssessedAmount.ToString(),
                                        PaymentImageByte = o.PaymentFileByte,
                                        PaymentImageContentType = o.PaymentFileType
                                    })
                                    .FirstOrDefault();

                return PartialView("_PaymentInfo", payment);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCompletedBatchList(int DealerID)
        {
            using (db = new VRSystemEntities())
            {
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                if (DealerID != 0)
                {

                    BatchList = db
                        .BatchMaster
                        .Where(o =>
                            o.Active == true &&
                            o.BatchTypeID == (int)BatchTypeList.LTO &&
                            o.EntityTypeID == (int)UserEntityEnum.Dealer &&
                            o.UserReference == DealerID &&
                            o.DateSubmitted != null &&
                            o.Downloaded == true &&
                            o.Assessed == true &&
                            o.PaymentRef != null &&
                            o.Completed == true &&
                            o.ForPickUp == false &&
                            o.LTOBranchID == CurrentUser.Details.SubReferenceID
                        )
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.CompletedDate,
                            BatchCount = o.BatchCount,
                            Assessed = o.Assessed,
                            //AssessedAmount = o.AssessedAmount,
                            PaymentRef = o.PaymentRef,
                            PaymentImageContentType = o.PaymentFileType,
                            EPatImageContentType = o.PaymentEPATFileType
                        })
                        .OrderByDescending(o => o.DateSubmitted)
                        .ToList();
                }
                else
                {
                    
                    BatchList = db
                        .BatchMaster
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
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.CompletedDate,
                            BatchCount = o.BatchCount,
                            Assessed = o.Assessed,
                            //AssessedAmount = o.AssessedAmount,
                            PaymentRef = o.PaymentRef,
                            PaymentImageContentType = o.PaymentFileType,
                            EPatImageContentType = o.PaymentEPATFileType
                        })
                        .OrderByDescending(o => o.DateSubmitted)
                        .ToList();
                }
                return PartialView("_PaymentBatchList", BatchList);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCompletedVehicleList(int BatchID)
        {
            using (db = new VRSystemEntities())
            {
                var VehicleIDList = db.BatchDetails.Where(o => o.BatchID == BatchID).Select(o => o.VehicleID).ToList();

                //var VehicleList = db.vwVehicleList.Where(o => VehicleIDList.Contains(o.VehicleID)).ToList();          

                var VehicleList = (from a in db.vwVehicleList
                                   where VehicleIDList.Contains(a.VehicleID)
                                   join b in db.BatchDetails on a.VehicleID equals b.VehicleID
                                   into temp
                                   from temptbl in temp.DefaultIfEmpty()
                                   where temptbl.BatchID == BatchID
                                   select new
                                   {
                                       VehicleID = a.VehicleID,
                                       VehicleMakeName = a.VehicleMakeName,
                                       VehicleModelName = a.VehicleModelName,
                                       Variant = a.Variant,
                                       EngineNumber = a.EngineNumber,
                                       ChassisNumber = a.ChassisNumber,
                                       BodyIDNumber = a.BodyIDNumber,
                                       Year = a.Year,
                                       Assessed = temptbl.Assessed,
                                       //AssessedAmount = a.AssessedAmount,
                                       Completed = temptbl.Completed
                                   })
                                    .Select(o => new LTOAssessBatchDetailVehicle()
                                    {
                                        VehicleID = o.VehicleID,
                                        VehicleMakeName = o.VehicleMakeName,
                                        VehicleModelName = o.VehicleModelName,
                                        Variant = o.Variant,
                                        EngineNumber = o.EngineNumber,
                                        ChassisNumber = o.ChassisNumber,
                                        BodyIDNumber = o.BodyIDNumber,
                                        YearOfMake = o.Year,
                                        Assessed = o.Assessed,
                                        //AssessedAmount = o.AssessedAmount.ToString(),
                                        Completed = o.Completed
                                    }).ToList();

                return PartialView("_PaymentBatchDetails", VehicleList);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetForPickUpBatchList(int DealerID)
        {
            using (db = new VRSystemEntities())
            {
                var BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.Dealer
                                                   && o.UserReference == DealerID
                                                   && o.BatchTypeID == (int)BatchTypeList.LTO
                                                   && o.Assessed == true
                                                   && o.PaymentRef != null
                                                   && o.Completed == true
                                                   && o.ForPickUp == true
                                                   && o.Active == true)
                                              .Select(o => new LTOBatchHeader
                                              {
                                                  BatchID = o.BatchID,
                                                  EntityName = db.Dealer.Where(p => p.DealerID == o.UserReference).FirstOrDefault().DealerName,
                                                  ReferenceNo = o.ReferenceNo,
                                                  BatchDescription = o.BatchDescription,
                                                  DateSubmitted = o.CompletedDate,
                                                  BatchCount = o.BatchCount,
                                                  Assessed = o.Assessed,
                                                  //AssessedAmount = o.AssessedAmount,
                                                  PaymentRef = o.PaymentRef,
                                                  PaymentImageContentType = o.PaymentFileType,
                                                  EPatImageContentType = o.PaymentEPATFileType
                                              })
                                              .OrderByDescending(o => o.DateSubmitted)
                                              .ToList();
                return PartialView("_PaymentBatchList", BatchList);
            }
        }


        public void LTOCompletedReport(int BatchID)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports/RDLC"), "LTOCompletedReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            //else
            //{
            //    return RedirectToAction("Index");
            //}

            DataTable dt = new DataTable("VehicleDetails");
            dt.Columns.Add(new DataColumn("Engine", typeof(string)));
            dt.Columns.Add(new DataColumn("Chassis", typeof(string)));
            dt.Columns.Add(new DataColumn("Make", typeof(string)));
            dt.Columns.Add(new DataColumn("Model", typeof(string)));
            dt.Columns.Add(new DataColumn("InvoiceDate", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("CustomerName", typeof(string)));
            dt.Columns.Add(new DataColumn("Cost", typeof(int)));

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
                        CustomerName = item.CustomerInfo.FirstName + " " + item.CustomerInfo.MiddleName + ", " + item.CustomerInfo.LastName;
                    }
                    dt.Rows.Add(
                        item.Engine,
                        item.Chassis,
                        item.Make,
                        item.Model,
                        (DateTime)item.InvoiceDate,
                        CustomerName,
                        (int)item.Cost
                        );
                }
            }

            //string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            //lr.EnableExternalImages = true;
            //lr.EnableHyperlinks = true;

            ReportParameter[] prm = new ReportParameter[3];
            prm[0] = new ReportParameter("BatchNumberParameter", Header.ReferenceNo.ToString());
            prm[1] = new ReportParameter("DealerNameParameter", DealerName);
            prm[2] = new ReportParameter("ReferenceNumberParameter", Header.PaymentRef);
            lr.SetParameters(prm);

            ReportDataSource rds = new ReportDataSource("VehicleDetails", dt);
            lr.DataSources.Clear();
            lr.DataSources.Add(rds);

            lr.Refresh();


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo ="";

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
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=CompletedReport." + fileNameExtension);
            Response.BinaryWrite(renderedBytes); // create the file
            Response.Flush(); // send it to the client to download
        }


        public ActionResult GetReceiptFile(int BatchID)
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
                var searchresult = String.Format("data:" + seasrch.PaymentImageContentType + ";base64,{0}", Convert.ToBase64String(seasrch.PaymentImageByte));
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        #region [ CSR ]
        public ActionResult CSRSubmittedBatch()
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                var model = new CSRLTOSubmittedBatch();

                var MAIList = new List<LTOMAIFilter>();
                //{
                //    new DealerFilter() { DealerID = 0, DealerName = "ALL"}
                //};
                MAIList.Add(new LTOMAIFilter() { MAIID = 0, MAIName = "ALL" });
                MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new LTOMAIFilter
                {
                    MAIID = o.MAIID,
                    MAIName = o.MAIName
                }).OrderBy(o => o.MAIName).ToList());

                model.MAIList = MAIList;

                model.BatchList = db
                    .BatchMaster
                    .Where(o =>
                        o.EntityTypeID == (int)UserEntityEnum.MAI &&
                        o.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                        o.Active == true &&
                        o.DateSubmitted != null &&
                        o.Assessed == false &&
                        o.Downloaded == false &&
                        (o.Rejected == false || o.Reprocessed == true)
                    )
                    .Select(o => new LTOBatchHeader
                    {
                        BatchID = o.BatchID,
                        EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                        ReferenceNo = o.ReferenceNo,
                        BatchDescription = o.BatchDescription,
                        DateSubmitted = o.DateSubmitted,
                        BatchCount = o.BatchCount,
                        Remarks = o.ReprocessedRemarks
                    })
                    .OrderByDescending(o => o.DateSubmitted)
                    .ToList();

                //model.BatchList = new List<vwBatchHeader>() { 
                //new vwBatchHeader() { BatchID = 13, ReferenceNo = "123"},
                //new vwBatchHeader() { BatchID = 14, ReferenceNo = "234"},
                //new vwBatchHeader() { BatchID = 3, ReferenceNo = "345"}
                //};

                model.VehicleList = new List<LTOBatchDetailVehicle>();




                return View(model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CSRSubmittedBatch(CSRLTOSubmittedBatch model, string submit)
        {
            ViewBag.EntityType = "MAI";
            switch (submit)
            {
                case "Download":
                    using (db = new VRSystemEntities())
                    {
                        //Update Header
                        var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                        UpdateHeader.Downloaded = true;
                        UpdateHeader.DownloadedBy = CurrentUser.Details.UserID;
                        UpdateHeader.DownloadedDate = DateTime.Now;
                        db.SaveChanges();

                        //return RedirectToAction("CSRSubmittedBatch");

                        string path = Server.MapPath("~/Reports/Excel/LTO_DIY_MAI_TEMPLATE.xlsx");

                        using (XLWorkbook wb = new XLWorkbook(path))
                        {
                            var businessinfo = db.MAI.Where(o => o.MAIID == UpdateHeader.UserSubRef).FirstOrDefault();
                            var AccreditationNumber = db.MAI
                                .Where(o => o.MAIID == UpdateHeader.UserSubRef)
                                .Select(o => o.AccreditationNumber).FirstOrDefault();

                            var ws = wb.Worksheet(1);

                            var ttl_vehicle = 0;
                            var tbl_row = 24;

                            foreach (var item in model.VehicleList)
                            {
                                var vehicleinfo = db.VehicleInfo.Where(o => o.VehicleID == item.VehicleID).FirstOrDefault();
                                var vehicleinfomake = db.VehicleMake.Where(o => o.Active == true && o.VehicleMakeID == vehicleinfo.VehicleMakeID).FirstOrDefault();
                                var vehicleinfobody = db.VehicleBodyType.Where(o => o.Active == true && o.VehicleBodyTypeID == vehicleinfo.VehicleMakeID).FirstOrDefault();
                                var vehicleinfocolor = db.VehicleColor.Where(o => o.Active == true && o.VehicleColorID == vehicleinfo.VehicleColorID).FirstOrDefault();
                                var vehicleinfofuel = db.VehicleFuelType.Where(o => o.Active == true && o.VehicleFuelTypeID == vehicleinfo.VehicleFuelTypeID).FirstOrDefault();
                                var vehicleinfoaircon = db.AirconType.Where(o => o.Active == true && o.AirconTypeReference == vehicleinfo.AirconType).FirstOrDefault();

                                ////ENGINE
                                //CONDUCTION STICKER NUMBER   
                                ws.Cell(tbl_row, 1).Value = vehicleinfo.ConductionSticker;
                                //ENGINE NUMBER  
                                ws.Cell(tbl_row, 2).Value = vehicleinfo.EngineNumber;
                                //FUEL TYPE
                                ws.Cell(tbl_row, 3).Value = vehicleinfofuel.VehicleFuelTypeName;
                                //CYLINDERS    
                                ws.Cell(tbl_row, 4).Value = vehicleinfo.Cylinders;
                                //PISTONE DISPLACEMENT  
                                ws.Cell(tbl_row, 5).Value = vehicleinfo.PistonDisplacement;
                                //BOC CP NUMBER   
                                ws.Cell(tbl_row, 6).Value = vehicleinfo.CPNumber;
                                //CP DATE
                                ws.Cell(tbl_row, 7).SetValue<string>(vehicleinfo.DateIssued1?.ToString("MM/dd/yyyy"));
                                //INFORMAL ENTRY NUMBER
                                ws.Cell(tbl_row, 8).Value = vehicleinfo.InformalEntryNumberEngine;

                                ////CHASSIS
                                //CHASSIS NUMBER  
                                ws.Cell(tbl_row, 9).Value = vehicleinfo.ChassisNumber;
                                //BOC CP NUMBER   
                                ws.Cell(tbl_row, 10).Value = vehicleinfo.CPNumber2;
                                //CP DATE
                                ws.Cell(tbl_row, 11).SetValue<string>(vehicleinfo.DateIssued2?.ToString("MM/dd/yyyy"));
                                //INFORMAL ENTRY NUMBER
                                ws.Cell(tbl_row, 12).Value = vehicleinfo.InformalEntryNumberChassis;

                                ////VEHICLE BODY INFO
                                //BODY ID NUMBER
                                ws.Cell(tbl_row, 13).Value = vehicleinfo.BodyIDNumber;
                                //MAKE
                                ws.Cell(tbl_row, 14).Value = vehicleinfomake.VehicleMakeName;
                                //SERIES
                                ws.Cell(tbl_row, 15).Value = vehicleinfo.Series;
                                //BODY TYPE
                                ws.Cell(tbl_row, 16).Value = vehicleinfobody.VehicleBodyTypeName;
                                //YEAR
                                ws.Cell(tbl_row, 17).Value = vehicleinfo.Year;
                                //COLOR
                                ws.Cell(tbl_row, 18).Value = vehicleinfocolor.VehicleColorName;
                                //GVW
                                ws.Cell(tbl_row, 19).Value = vehicleinfo.GrossVehicleWeight;
                                //AIRCON_REF
                                ws.Cell(tbl_row, 20).SetValue<string>(vehicleinfoaircon.AirconTypeReference);
                                //TAX_TYPE
                                ws.Cell(tbl_row, 21).Value = vehicleinfo.TaxType;
                                //TAX_AMOUNT
                                ws.Cell(tbl_row, 22).Value = vehicleinfo.TaxAmount;
                                //TIRE_SIZE_FRONT
                                ws.Cell(tbl_row, 23).Value = vehicleinfo.FrontTiresNumber;
                                //TIRE_SIZE_REAR
                                ws.Cell(tbl_row, 24).Value = vehicleinfo.RearTiresNumber;
                                //COC_NO
                                ws.Cell(tbl_row, 25).Value = vehicleinfo.COCNo;

                                tbl_row++;
                                ttl_vehicle++;
                            }

                            //BUSINESS NAME
                            ws.Cell(2, 3).Value = businessinfo.MAIName;
                            //BUSINESS ADDRESS
                            ws.Cell(3, 3).Value = businessinfo.Address;
                            //ACCREDITATION NO.
                            ws.Cell(4, 3).Value = AccreditationNumber;
                            //REPORT TYPE 	
                            ws.Cell(12, 3).Value = "1";
                            //TOTAL ITEMS
                            ws.Cell(14, 3).Value = ttl_vehicle;
                            //CATEGORY	
                            ws.Cell(20, 2).Value = "";
                            //AGENCY	
                            ws.Cell(21, 2).Value = "BOC";


                            //wb.Save();
                            using (MemoryStream stream = new MemoryStream())
                            {
                                wb.SaveAs(stream);
                                var FileName = businessinfo.MAIName + " - " + model.SelectedBatchReferenceNo + ".xlsx";
                                TempData["SuccessMessage"] = "Downloaded is successful!";
                                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);

                            }
                        }
                    }
                case "Reject":
                    using (db = new VRSystemEntities())
                    {
                        //Update Header
                        var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                        UpdateHeader.Rejected = true;
                        UpdateHeader.RejectedBy = CurrentUser.Details.UserID;
                        UpdateHeader.RejectedDate = DateTime.Now;
                        UpdateHeader.RejectedRemarks = model.RejectedRemarks.Trim();
                        db.SaveChanges();

                        model = new CSRLTOSubmittedBatch();

                        var MAIList = new List<LTOMAIFilter>();

                        MAIList.Add(new LTOMAIFilter() { MAIID = 0, MAIName = "ALL" });
                        MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new LTOMAIFilter
                        {
                            MAIID = o.MAIID,
                            MAIName = o.MAIName
                        }).OrderBy(o => o.MAIName).ToList());

                        model.MAIList = MAIList;

                        model.BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                           && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                           && o.Active == true
                                                           && o.DateSubmitted != null
                                                           && o.Assessed == false
                                                           && o.Downloaded == false
                                                           && (o.Rejected == false
                                                           || o.Reprocessed == true))
                                                      .Select(o => new LTOBatchHeader
                                                      {
                                                          BatchID = o.BatchID,
                                                          EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                                                          ReferenceNo = o.ReferenceNo,
                                                          BatchDescription = o.BatchDescription,
                                                          DateSubmitted = o.DateSubmitted,
                                                          BatchCount = o.BatchCount,
                                                          Remarks = o.ReprocessedRemarks
                                                      })
                                                      .OrderByDescending(o => o.DateSubmitted)
                                                      .ToList();

                        //model.BatchList = new List<vwBatchHeader>() { 
                        //new vwBatchHeader() { BatchID = 13, ReferenceNo = "123"},
                        //new vwBatchHeader() { BatchID = 14, ReferenceNo = "234"},
                        //new vwBatchHeader() { BatchID = 3, ReferenceNo = "345"}
                        //};

                        model.VehicleList = new List<LTOBatchDetailVehicle>();

                        return View(model);
                    }
                default:
                    return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCSRBatchList(int MAIID)
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                if (MAIID != 0)
                {
                    BatchList = db.BatchMaster.Where(o =>
                                                  o.Active == true
                                                  && o.EntityTypeID == (int)UserEntityEnum.MAI
                                                  && o.UserReference == MAIID
                                                  && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                  && o.DateSubmitted != null
                                                  && o.Assessed == false
                                                  && o.Downloaded == false
                                                  && (o.Rejected == false
                                                  || o.Reprocessed == true))
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
                }
                else
                {
                    BatchList = db.BatchMaster.Where(o => o.EntityTypeID == (int)UserEntityEnum.MAI
                                                 && o.BatchTypeID == (int)BatchTypeList.LTOCSR
                                                 && o.DateSubmitted != null
                                                 && o.Assessed == false
                                                 && o.Downloaded == false
                                                 && o.Rejected == false
                                                 && o.Active == true
                                                 && (o.Rejected == false
                                                 || o.Reprocessed == true))
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
                }
                return PartialView("_BatchList", BatchList);
            }
        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult CSRAssessBatch()
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                var model = new LTOAssessBatch();

                var MAIList = new List<MAIModel>();
                MAIList.Add(new MAIModel() { MAIID = 0, MAIName = "ALL" });
                MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                {
                    MAIID = o.MAIID,
                    MAIName = o.MAIName
                }).OrderBy(o => o.MAIName).ToList());

                model.MAIList = MAIList;

                model.BatchList = db.BatchMaster
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
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.DownloadedDate,
                            BatchCount = o.BatchCount
                        })
                        .OrderByDescending(o => o.DateSubmitted)
                        .ToList();

                model.VehicleList = new List<LTOAssessBatchDetailVehicle>();

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult CSRAssessBatch(LTOAssessBatch model)
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                decimal ttl_assessed_amount = 0;
                foreach (var vehicle in model.VehicleList)
                {
                    var UpdateDetails = db.BatchDetails.Where(o => o.BatchID == model.SelectedBatchID && o.VehicleID == vehicle.VehicleID).FirstOrDefault();
                    UpdateDetails.Assessed = true;
                    UpdateDetails.AssessedBy = CurrentUser.Details.UserID;
                    UpdateDetails.AssessedDate = DateTime.Now;
                    db.SaveChanges();
                }

                if (model.VehicleList.Count > 0)
                {
                    var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                    UpdateHeader.Assessed = true;
                    UpdateHeader.AssessedBy = CurrentUser.Details.UserID;
                    UpdateHeader.AssessedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "CSR Assessment is successful!";

                    //Functions.LTOEmailStatus(model.SelectedBatchID, LTOStatus.Assessed);
                }
                else
                {
                    TempData["ErrorMessage"] = "There's something error!";
                }
            }
            using (db = new VRSystemEntities())
            {
                model = new LTOAssessBatch();

                var MAIList = new List<MAIModel>();
                MAIList.Add(new MAIModel() { MAIID = 0, MAIName = "ALL" });
                MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                {
                    MAIID = o.MAIID,
                    MAIName = o.MAIName
                }).OrderBy(o => o.MAIName).ToList());

                model.MAIList = MAIList;

                model.BatchList = db.BatchMaster
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
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.DownloadedDate,
                            BatchCount = o.BatchCount
                        })
                        .OrderByDescending(o => o.DateSubmitted)
                        .ToList();

                model.VehicleList = new List<LTOAssessBatchDetailVehicle>();

            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCSRBatchListAssess(int MAIID)
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                if (MAIID != 0)
                {
                    BatchList = db.BatchMaster
                        .Where(o =>
                        o.Active == true &&
                        o.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                        o.EntityTypeID == (int)UserEntityEnum.MAI &&
                        o.UserReference == MAIID &&
                        o.DateSubmitted != null &&
                        o.Downloaded == true &&
                        o.Assessed == false &&
                        o.PaymentRef == null &&
                        o.Completed == false
                        )
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
                }
                else
                {
                    BatchList = db.BatchMaster
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
                }
                return PartialView("_BatchList", BatchList);
            }
        }

        [HttpGet]
        public ActionResult CSRPaymentBatch()
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                var model = new LTOPayment();

                var MAIList = new List<MAIModel>();
                MAIList.Add(new MAIModel() { MAIID = 0, MAIName = "ALL" });
                MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                {
                    MAIID = o.MAIID,
                    MAIName = o.MAIName
                }).OrderBy(o => o.MAIName).ToList());

                model.MAIList = MAIList;

                model.BatchList = db
                    .BatchMaster
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
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.PaymentDate,
                            BatchCount = o.BatchCount,
                            Assessed = o.Assessed,
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
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult CSRPaymentBatch(LTOPayment model, string submit)
        {
            ViewBag.EntityType = "MAI";
            switch (submit)
            {
                case "Download":
                    LTOCompletedReport(model.SelectedBatchID);
                    break;
                default:
                    using (db = new VRSystemEntities())
                    {
                        var hasError = false;
                        foreach (var vehicle in model.VehicleList)
                        {
                            if (vehicle.Completed == false && vehicle.Rejected == false)
                            {
                                TempData["ErrorMessage"] = "Please complete vehicle status";
                                hasError = true;
                            }
                        }

                        if (hasError == false)
                        {
                            var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                            UpdateHeader.Completed = true;
                            UpdateHeader.CompletedBy = CurrentUser.Details.UserID;
                            UpdateHeader.CompletedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "For OR/CR Processing!";
                        }

                        //
                        model = new LTOPayment();

                        var MAIList = new List<MAIModel>();
                        MAIList.Add(new MAIModel() { MAIID = 0, MAIName = "ALL" });
                        MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                        {
                            MAIID = o.MAIID,
                            MAIName = o.MAIName
                        }).OrderBy(o => o.MAIName).ToList());

                        model.MAIList = MAIList;

                        model.BatchList = db
                            .BatchMaster
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
                                .Select(o => new LTOBatchHeader
                                {
                                    BatchID = o.BatchID,
                                    EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                                    ReferenceNo = o.ReferenceNo,
                                    BatchDescription = o.BatchDescription,
                                    DateSubmitted = o.PaymentDate,
                                    BatchCount = o.BatchCount,
                                    Assessed = o.Assessed,
                                    PaymentRef = o.PaymentRef,
                                    PaymentImageContentType = o.PaymentFileType,
                                    EPatImageContentType = o.PaymentEPATFileType
                                })
                                .OrderByDescending(o => o.DateSubmitted)
                                .ToList();

                        model.VehicleList = new List<LTOAssessBatchDetailVehicle>();

                    }
                    break;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCSRProcessBatchList(int MAIID)
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                if (MAIID != 0)
                {
                    BatchList = db
                        .BatchMaster
                        .Where(o =>
                            o.Active == true &&
                            o.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                            o.EntityTypeID == (int)UserEntityEnum.MAI &&
                            o.UserReference == MAIID &&
                            o.DateSubmitted != null &&
                            o.Downloaded == true &&
                            o.Assessed == true &&
                            o.PaymentRef != null &&
                            o.Completed == false
                            )
                            .Select(o => new LTOBatchHeader
                            {
                                BatchID = o.BatchID,
                                EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                                ReferenceNo = o.ReferenceNo,
                                BatchDescription = o.BatchDescription,
                                DateSubmitted = o.AssessedDate,
                                BatchCount = o.BatchCount,
                                Assessed = o.Assessed,
                                PaymentRef = o.PaymentRef,
                                PaymentImageContentType = o.PaymentFileType,
                                EPatImageContentType = o.PaymentEPATFileType
                            })
                            .OrderByDescending(o => o.DateSubmitted)
                            .ToList();
                }
                else
                {
                    BatchList = db
                        .BatchMaster
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
                            .Select(o => new LTOBatchHeader
                            {
                                BatchID = o.BatchID,
                                EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                                ReferenceNo = o.ReferenceNo,
                                BatchDescription = o.BatchDescription,
                                DateSubmitted = o.AssessedDate,
                                BatchCount = o.BatchCount,
                                Assessed = o.Assessed,
                                PaymentRef = o.PaymentRef,
                                PaymentImageContentType = o.PaymentFileType,
                                EPatImageContentType = o.PaymentEPATFileType
                            })
                            .OrderByDescending(o => o.DateSubmitted)
                            .ToList();
                }
                return PartialView("_PaymentBatchList", BatchList);
            }
        }

        [HttpGet]
        public ActionResult CSRCompletedBatch()
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                var model = new LTOPayment();

                var MAIList = new List<MAIModel>();
                MAIList.Add(new MAIModel() { MAIID = 0, MAIName = "ALL" });
                MAIList.AddRange(db.MAI.Where(o => o.Active == true).Select(o => new MAIModel
                {
                    MAIID = o.MAIID,
                    MAIName = o.MAIName
                }).OrderBy(o => o.MAIName).ToList());

                model.MAIList = MAIList;

                model.BatchList = db
                    .BatchMaster
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
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.AssessedDate,
                            BatchCount = o.BatchCount,
                            Assessed = o.Assessed,
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
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.LTO })]
        public ActionResult CSRCompletedBatch(LTOPayment model, string submit)
        {
            ViewBag.EntityType = "MAI";
            switch (submit)
            {
                case "Report":
                    LTOCompletedReport(model.SelectedBatchID);
                    break;
                case "ForPickUp":
                    using (db = new VRSystemEntities())
                    {
                        //Update Header
                        var UpdateHeader = db.BatchMaster.Where(o => o.BatchID == model.SelectedBatchID).FirstOrDefault();
                        UpdateHeader.ForPickUp = true;
                        UpdateHeader.ForPickUpBy = CurrentUser.Details.UserID;
                        UpdateHeader.ForPickUpDate = DateTime.Now;
                        db.SaveChanges();
                        //Functions.LTOEmailStatus(model.SelectedBatchID, LTOStatus.ForPickUp);
                        TempData["SuccessMessage"] = "Batch " + UpdateHeader.ReferenceNo + " is now ready for Pick Up!";
                    }
                    break;
                default:
                    break;
            }
            return RedirectToAction("CSRCompletedBatch");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCSRCompletedBatchList(int MAIID)
        {
            ViewBag.EntityType = "MAI";
            using (db = new VRSystemEntities())
            {
                List<LTOBatchHeader> BatchList = new List<LTOBatchHeader>();
                if (MAIID != 0)
                {

                    BatchList = db
                        .BatchMaster
                        .Where(o =>
                            o.Active == true &&
                            o.BatchTypeID == (int)BatchTypeList.LTOCSR &&
                            o.EntityTypeID == (int)UserEntityEnum.MAI &&
                            o.UserReference == MAIID &&
                            o.DateSubmitted != null &&
                            o.Downloaded == true &&
                            o.Assessed == true &&
                            o.PaymentRef != null &&
                            o.Completed == true &&
                            o.ForPickUp == false
                        )
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.CompletedDate,
                            BatchCount = o.BatchCount,
                            Assessed = o.Assessed,
                            PaymentRef = o.PaymentRef,
                            PaymentImageContentType = o.PaymentFileType,
                            EPatImageContentType = o.PaymentEPATFileType
                        })
                        .OrderByDescending(o => o.DateSubmitted)
                        .ToList();
                }
                else
                {
                    BatchList = db
                        .BatchMaster
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
                        .Select(o => new LTOBatchHeader
                        {
                            BatchID = o.BatchID,
                            EntityName = db.MAI.Where(p => p.MAIID == o.UserSubRef).FirstOrDefault().MAIName,
                            ReferenceNo = o.ReferenceNo,
                            BatchDescription = o.BatchDescription,
                            DateSubmitted = o.CompletedDate,
                            BatchCount = o.BatchCount,
                            Assessed = o.Assessed,
                            PaymentRef = o.PaymentRef,
                            PaymentImageContentType = o.PaymentFileType,
                            EPatImageContentType = o.PaymentEPATFileType
                        })
                        .OrderByDescending(o => o.DateSubmitted)
                        .ToList();
                }
                return PartialView("_PaymentBatchList", BatchList);
            }
        }
        #endregion
    }
}