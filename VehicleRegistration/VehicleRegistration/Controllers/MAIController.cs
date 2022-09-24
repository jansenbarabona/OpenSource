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


namespace VehicleRegistration.Controllers
{
    [SessionExpire]
    public class MAIController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: MAI
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                List<vwMAIList> mailist = new List<vwMAIList>();
                db.Configuration.LazyLoadingEnabled = false;
                try
                {
                    mailist = db.vwMAIList.Where(o => o.Active == true).ToList();

                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error has occured";
                }

                return View(mailist);
            }
        }
        #region MaiInfo
     
        public ActionResult MaiInfo(int id)
        {
            ViewBag.id = id;

            MAIModel MaiInfoView = new MAIModel();
           
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = true;
                ViewBag.Edit = true;
           
                MaiInfoView.WalletDetail = Functions.GetWalletDetails((int)UserEntityEnum.MAI, (int)id);
                MaiInfoView.EntityTransaction = db.vwTransactionEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer && o.EntityID == (int)id && o.Active == true).ToList();
                //MaiInfoView.MAITypeList = db.MAIType.Where(o => o.Active == true).ToList();
                //MaiInfoView.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                //MaiInfoView.CityList = db.City.Where(o => o.Active == true).ToList();

                var MAILoad = db.vwMAIList.Where(o => o.Active == true && o.MAIID == id).ToList().FirstOrDefault();
                //var vwMAILoad = db.vwMAIList.Where(o => o.Active == true && o.MAIID == id).ToList().FirstOrDefault();

                MaiInfoView.MAIID = MAILoad.MAIID;
                //MaiInfoView.MAITypeID = MAILoad.MAITypeID;
                MaiInfoView.MAITypeName = MAILoad.MAITypeName;
                MaiInfoView.MAIName = MAILoad.MAIName;
                MaiInfoView.EmailAddress = MAILoad.EmailAddress;
                MaiInfoView.AccreditationNumber = MAILoad.AccreditationNumber;
                MaiInfoView.BusinessPhone = MAILoad.BusinessPhone;
                MaiInfoView.MobilePhone = MAILoad.MobilePhone;
                MaiInfoView.FaxNumber = MAILoad.FaxNumber;
                MaiInfoView.Logo = MAILoad.Logo;
                MaiInfoView.Website = MAILoad.Website;
                MaiInfoView.Address = MAILoad.Address;
                MaiInfoView.CityName = MAILoad.CityName;
                MaiInfoView.BarangayName = MAILoad.BarangayName;

                MaiInfoView.ProvinceName = MAILoad.ProvinceName;
                MaiInfoView.ZipCode = MAILoad.ZipCode;
                MaiInfoView.Notes = MAILoad.Notes;

                MaiInfoView.SelectedMAITypeID = MaiInfoView.MAITypeID;

            }
            return PartialView(MaiInfoView);
        }   

        #endregion


        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult MAI_Register(int? id)
        {
            ViewBag.id = id;

            MAIModel NewMAI = new MAIModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;


                NewMAI.MAITypeList = db.MAIType.Where(o => o.Active == true).ToList();
                NewMAI.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                NewMAI.CityList = db.City.Where(o => o.Active == true).ToList();
                NewMAI.BarangayList = new List<Barangay>();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    NewMAI.WalletDetail = Functions.GetWalletDetails((int)UserEntityEnum.MAI, (int)id);
                    NewMAI.EntityTransaction = db.vwTransactionEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer && o.EntityID == (int)id && o.Active == true).ToList();

                    var MAILoad = db.MAI.Where(o => o.Active == true && o.MAIID == id).ToList().FirstOrDefault();
                    NewMAI.CityList = db.City.Where(o => o.ProvinceID == MAILoad.ProvinceID && o.Active == true).ToList();
                    NewMAI.BarangayList = db.Barangay.Where(o => o.BarangayID == MAILoad.BarangayID && o.Active == true).ToList();

                    NewMAI.MAIID = MAILoad.MAIID;
                    NewMAI.MAITypeID = MAILoad.MAITypeID;
                    NewMAI.MAIName = MAILoad.MAIName;
                    NewMAI.EmailAddress = MAILoad.EmailAddress;
                    NewMAI.EmailAddress2 = MAILoad.EmailAddress2;
                    NewMAI.BusinessPhone = MAILoad.BusinessPhone;
                    NewMAI.MobilePhone = MAILoad.MobilePhone;
                    NewMAI.FaxNumber = MAILoad.FaxNumber;
                    NewMAI.TIN = MAILoad.TIN;
                    NewMAI.Website = MAILoad.Website;
                    NewMAI.Address = MAILoad.Address;
                    NewMAI.CityID = MAILoad.CityID;
                    NewMAI.BarangayID = MAILoad.BarangayID;
                    NewMAI.SelectedCityID = MAILoad.CityID;
                    NewMAI.ProvinceID = MAILoad.ProvinceID;
                    NewMAI.SelectedProvinceID = MAILoad.ProvinceID;
                    NewMAI.ZipCode = MAILoad.ZipCode;
                    NewMAI.Logo = MAILoad.Logo;
                    NewMAI.Notes = MAILoad.Notes;
                    NewMAI.AccreditationNumber = MAILoad.AccreditationNumber;
                    NewMAI.CreatedBy = MAILoad.CreatedBy;
                    NewMAI.CreatedDate = MAILoad.CreatedDate;
                    NewMAI.Active = MAILoad.Active;

                    NewMAI.SelectedMAITypeID = NewMAI.MAITypeID;
                }

                return PartialView(NewMAI);
            }

            //test
        }
        [HttpPost]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        [ValidateAntiForgeryToken]
        public ActionResult MAI_Register(MAIModel mai/*,HttpPostedFileBase logo*/, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        if (InsertMAI(mai))
                        {
                            Functions.Logo(submit, "", mai.LogoFile);
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "An error has occured!";
                            return View(mai);
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var UpdateMAI = db.MAI.Where(o => o.MAIID == mai.MAIID).FirstOrDefault();
                            UpdateMAI.Address = mai.Address.Trim();
                            UpdateMAI.MAITypeID = mai.SelectedMAITypeID;
                            UpdateMAI.MAIName = mai.MAIName.Trim();
                            UpdateMAI.MobilePhone = mai.MobilePhone?.Trim();
                            UpdateMAI.Website = mai.Website?.Trim();
                            UpdateMAI.ZipCode = mai.ZipCode.Trim();
                            UpdateMAI.Notes = mai.Notes?.Trim();
                            if (mai.LogoFile != null)
                            {
                                Functions.Logo(submit, UpdateMAI.Logo, mai.LogoFile);
                                UpdateMAI.Logo = mai.LogoFile != null ? mai.LogoFile.FileName : null;
                                UpdateMAI.LogoByte = mai.LogoFile.ToByte();
                            }
                            UpdateMAI.FaxNumber = mai.FaxNumber?.Trim();
                            UpdateMAI.EmailAddress = mai.EmailAddress.Trim();
                            UpdateMAI.EmailAddress2 = mai.EmailAddress2.Trim();
                            UpdateMAI.BusinessPhone = mai.BusinessPhone.Trim();
                            UpdateMAI.TIN = mai.TIN.Trim();
                            UpdateMAI.CityID = mai.SelectedCityID;
                            UpdateMAI.BarangayID = mai.BarangayID;
                            UpdateMAI.ProvinceID = mai.SelectedProvinceID;
                            UpdateMAI.AccreditationNumber = mai.AccreditationNumber;
                            UpdateMAI.UpdatedBy = CurrentUser.Details.UserID;
                            UpdateMAI.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var UpdateMAI = db.MAI.Where(o => o.MAIID == mai.MAIID).FirstOrDefault();
                            UpdateMAI.Active = false;
                            UpdateMAI.UpdatedBy = CurrentUser.Details.UserID;
                            UpdateMAI.UpdatedDate = DateTime.Now;
                            db.SaveChanges();

                            Functions.Logo(submit, UpdateMAI.Logo, null);
                            TempData["WarningMessage"] = "MAI Deleted";
                        }
                        break;
                }
                return RedirectToAction("Index");
            }
            else
            {
                mai.MAITypeList = db.MAIType.Where(o => o.Active == true).ToList();
                mai.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                mai.CityList = db.City.Where(o => o.Active == true).ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(mai);
            }

        }

        public ActionResult UploadMAI()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadMAI(HttpPostedFileBase upload, DataTable AnotherDataTable)
        {

            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


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
                        return View();
                    }

                    DataTable NewDataTable;
                    try
                    {

                        NewDataTable = reader.AsDataSet().Tables[0].FormatDataTable();

                    }
                    catch (Exception)
                    {
                        TempData["ErrorMessage"] = "Unable to upload file!";
                        return View();
                    }

                    List<MAIModel> UploadMAIList = new List<MAIModel>();
                    foreach (DataRow item in NewDataTable.Rows)
                    {
                        try
                        {
                            UploadMAIList.Add(new MAIModel()
                            {
                                MAITypeID = Int32.Parse(item.ItemArray[1].ToString().Trim()),
                                MAIName = item.ItemArray[2].ToString().Trim(),
                                EmailAddress = item.ItemArray[3].ToString().Trim(),
                                BusinessPhone = item.ItemArray[4].ToString().Trim(),
                                MobilePhone = item.ItemArray[5].ToString()?.Trim(),
                                FaxNumber = item.ItemArray[6].ToString()?.Trim(),
                                Website = item.ItemArray[7].ToString()?.Trim(),
                                Address = item.ItemArray[8].ToString().Trim(),
                                ProvinceID = Int32.Parse(item.ItemArray[10].ToString().Trim()),
                                CityID = Int32.Parse(item.ItemArray[12].ToString().Trim()),
                                ZipCode = item.ItemArray[13].ToString().Trim(),
                                Notes = item.ItemArray[14].ToString()?.Trim(),
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            });
                        }
                        catch (Exception ex)
                        {

                            TempData["ErrorMessage"] = "There's something error on uploading file!";
                            return View();
                        }

                    }

                    if (InsertMAI(UploadMAIList.ToArray()))
                        TempData["SuccessMessage"] = "Upload Successfully!";
                    else
                        TempData["ErrorMessage"] = "Upload Error!";

                    //using (db = new VRSystemEntities())
                    //{
                    //    int sccss = 0;
                    //    foreach (DataRow item in NewDataTable.Rows)
                    //    {
                    //        //validating
                    //        var email = item.ItemArray[3].ToString();
                    //        if (db.User.Where(o => o.EmailAddress == email).Count() > 0)
                    //        {
                    //            TempData["ErrorMessage"] = "Email already existing";
                    //            return View();
                    //        }

                    //        try
                    //        {
                    //            var NewMAI = new MAI
                    //            {
                    //                MAITypeID = Int32.Parse(item.ItemArray[1].ToString().Trim()),
                    //                MAIName = item.ItemArray[2].ToString().Trim(),
                    //                EmailAddress = item.ItemArray[3].ToString().Trim(),
                    //                BusinessPhone = item.ItemArray[4].ToString().Trim(),
                    //                MobilePhone = item.ItemArray[5].ToString().Trim(),
                    //                FaxNumber = item.ItemArray[6].ToString().Trim(),
                    //                Website = item.ItemArray[7].ToString().Trim(),
                    //                Address = item.ItemArray[8].ToString().Trim(),
                    //                ProvinceID = Int32.Parse(item.ItemArray[10].ToString().Trim()),
                    //                CityID = Int32.Parse(item.ItemArray[12].ToString().Trim()),
                    //                ZipCode = item.ItemArray[13].ToString().Trim(),
                    //                Notes = item.ItemArray[14].ToString().Trim(),
                    //                CreatedBy = CurrentUser.Details.UserID,
                    //                CreatedDate = DateTime.Now,
                    //                Active = true
                    //            };
                    //            db.MAI.Add(NewMAI);
                    //        }
                    //        catch
                    //        {
                    //            TempData["ErrorMessage"] = "There's something error on uploading file!";
                    //            return View();
                    //        }
                    //        sccss++;
                    //    }
                    //    if (NewDataTable.Rows.Count == sccss)
                    //    {
                    //        db.SaveChanges();
                    //        TempData["SuccessMessage"] = "Upload Successfully!";
                    //    }
                    //    else
                    //    {
                    //        TempData["ErrorMessage"] = "There's something in query!";
                    //        return View();
                    //    }
                    //}

                    reader.Close();
                    reader.Dispose();

                    return View(NewDataTable);

                }
                else
                {
                    TempData["ErrorMessage"] = "Please upload your file!";
                }
            }
            return View();
        }

        public ActionResult UploadMAITest()
        {
            var UploadMAI = new MAIUploadModel();
            return View(UploadMAI);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadMAITest(MAIUploadModel MAIUpload, FormCollection form)
        {
            if (ModelState.IsValid)
            {

                if (MAIUpload.UploadFile != null && MAIUpload.UploadFile.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = MAIUpload.UploadFile.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (MAIUpload.UploadFile.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (MAIUpload.UploadFile.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "This file is not supported";
                    }

                    if (reader != null)
                    {
                        try
                        {
                            MAIUpload.UploadedDataTable = reader.AsDataSet().Tables[0].FormatDataTable();
                        }
                        catch (Exception)
                        {
                            TempData["ErrorMessage"] = "Unable to upload file!";
                        }
                        reader.Close();
                        reader.Dispose();
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please upload your file!";
                }
            }
            return View(MAIUpload);
        }

        public ActionResult GetProvinceCity(decimal ProviceCode)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.City.Where(o => o.ProvinceID == ProviceCode).OrderBy(o => o.CityName).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }


        //MAI Vehicle Make
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        [HttpGet]
        public ActionResult MAIVehicleMake()
        {
            var mai = new MAIVehicleMakeModel();
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                mai.vwMAIVehicleMakeModelList = db.vwMAIVehicleMake.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID && o.Active == true).ToList();

                mai.VehicleMakeModelList = db.VehicleMake.Where(s => !db.MAIVehicleMake.Where(es => es.VehicleMakeID == s.VehicleMakeID && es.MAIID == CurrentUser.Details.SubReferenceID && es.Active == true).Any()).Select(
                    o => new VehicleMakeModel()
                    {
                        VehicleMakeID = o.VehicleMakeID,
                        VehicleMakeName = o.VehicleMakeName,
                        CreatedBy = o.CreatedBy,
                        CreatedDate = o.CreatedDate,
                        Active = o.Active
                    }).ToList();

                //var query = from a in db.VehicleMake
                //            where a.Active == true
                //            || db.MAIVehicleMake.Any(o => o.VehicleMakeID != a.VehicleMakeID)
                //            select new
                //            {
                //                VehicleMakeID = a.VehicleMakeID,
                //                VehicleMakeName = a.VehicleMakeName,
                //                CreatedBy = a.CreatedBy,
                //                CreatedDate = a.CreatedDate,
                //                Active = a.Active
                //            };
                //foreach (var q in query)
                //{
                //    mai.VehicleMakeModelList.Add(new VehicleMakeModel
                //    {
                //        VehicleMakeID = q.VehicleMakeID,
                //        VehicleMakeName = q.VehicleMakeName,
                //        CreatedBy = q.CreatedBy,
                //        CreatedDate = q.CreatedDate,
                //        Active = q.Active
                //    });
                //}
                return View(mai);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MAIVehicleMake(MAIVehicleMakeModel model)
        {
            using (db = new VRSystemEntities())
            {
                foreach (var item in model.VehicleMakeModelList)
                {
                    if (item.isChecked == true)
                    {
                        var insert = new MAIVehicleMake
                        {
                            MAIID = (int)CurrentUser.Details.SubReferenceID,
                            VehicleMakeID = item.VehicleMakeID,
                            CreatedBy = CurrentUser.Details.UserID,
                            CreatedDate = DateTime.Now,
                            Active = true
                        };
                        db.MAIVehicleMake.Add(insert);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Vehicle Make successfully added!";
                    }
                }


            }

            return RedirectToAction("MAIVehicleMake");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MAIVehicleMake_delete(int maivehiclemakeid)
        {
            using (db = new VRSystemEntities())
            {
                var Update = db.MAIVehicleMake.Where(o => o.MAIVehicleMakeID == maivehiclemakeid).FirstOrDefault();
                Update.Active = false;
                Update.UpdatedBy = CurrentUser.Details.UserID;
                Update.UpdatedDate = DateTime.Now;
                db.SaveChanges();
                TempData["ErrorMessage"] = "Vehicle Make has been removed!";
            }
            return Json("Successful");
        }

        //MAI DEALER LIST - START
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.MAI })]
        [HttpGet]
        public ActionResult MAI_Dealer()
        {
            var mai_id = CurrentUser.Details.SubReferenceID;
            var mai_deal = new MAI_DealerModel();
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

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
                mai_deal.MAI_DealerList = query.Where(o => o.MAIID == mai_id && o.Active == true).Select(
                    o => new MAI_DealerModel()
                    {
                        MAIDealerID = o.MAIDealerID,
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();

                //mai_deal.MAI_DealerList = db.Dealer.Where(s => db.MAIDealer.Where(es => es.DealerID == s.DealerID && es.MAIID == mai_id).Any()).Select(
                //    o => new MAI_DealerModel()
                //    {
                //        DealerID = o.DealerID,
                //        DealerName = o.DealerName
                //    }).ToList();

                mai_deal.DealerList = db.Dealer.Where(s => !db.MAIDealer.Where(es => es.DealerID == s.DealerID && es.MAIID == mai_id && es.Active == true).Any()).Select(
                    o => new MAI_DealerModel()
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    }).ToList();
                return View(mai_deal);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MAI_Dealer(MAI_DealerModel model, string submit)
        {
            switch (submit)
            {
                case "Save":
                    using (db = new VRSystemEntities())
                    {
                        foreach (var item in model.DealerList)
                        {
                            if (item.isChecked == true)
                            {
                                var insert = new MAIDealer
                                {
                                    MAIID = (int)CurrentUser.Details.SubReferenceID,
                                    DealerID = item.DealerID,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true
                                };
                                db.MAIDealer.Add(insert);
                                db.SaveChanges();
                                TempData["SuccessMessage"] = "Dealer has been successfully added!";
                            }
                        }
                    }
                    break;
                case "Delete":
                    using (db = new VRSystemEntities())
                    {
                        var Update = db.MAIDealer.Where(o => o.MAIDealerID == model.MAIDealerID).FirstOrDefault();
                        Update.Active = false;
                        Update.UpdatedBy = CurrentUser.Details.UserID;
                        Update.UpdatedDate = DateTime.Now;
                        db.SaveChanges();
                        TempData["ErrorMessage"] = "Dealer has been removed!";
                    }
                    break;
            }
            return RedirectToAction("MAI_Dealer");
        }
        //MAI DEALER LIST - END

        #region [ Private Functions ]
        private bool InsertMAI(params MAIModel[] NewMAIList)
        {
            bool success = false;
            int MAINumber = 0;
            using (db = new VRSystemEntities())
            {
                foreach (var NewMAI in NewMAIList)
                {
                    try
                    {
                        if (TryValidateModel(NewMAI))
                        {
                            var InsertMAI = new MAI
                            {
                                Address = NewMAI.Address.Trim(),
                                MAITypeID = NewMAI.SelectedMAITypeID,
                                MAIName = NewMAI.MAIName.Trim(),
                                MobilePhone = NewMAI.MobilePhone?.Trim(),
                                Website = NewMAI.Website?.Trim(),
                                ZipCode = NewMAI.ZipCode.Trim(),
                                Notes = NewMAI.Notes?.Trim(),
                                Logo = NewMAI.LogoFile != null ? NewMAI.LogoFile.FileName : null,
                                LogoByte = NewMAI.LogoFile != null ? NewMAI.LogoFile.ToByte() : null,
                                FaxNumber = NewMAI.FaxNumber?.Trim(),
                                EmailAddress = NewMAI.EmailAddress.Trim(),
                                EmailAddress2 = NewMAI.EmailAddress2.Trim(),
                                TIN = NewMAI.TIN.Trim(),
                                BusinessPhone = NewMAI.BusinessPhone.Trim(),
                                CityID = NewMAI.SelectedCityID,
                                BarangayID = NewMAI.BarangayID,
                                ProvinceID = NewMAI.SelectedProvinceID,
                                AccreditationNumber = NewMAI.AccreditationNumber,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };

                            db.MAI.Add(InsertMAI);
                            MAINumber++;
                        }
                        else
                            break;
                    }
                    catch (Exception)
                    {
                        success = false;
                    }
                }

                if (MAINumber == NewMAIList.Count())
                {
                    db.SaveChanges();
                    success = true;
                }
            }

            return success;
        }
        #endregion
    }
}