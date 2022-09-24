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
    public class WalletController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Wallet
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
        public ActionResult NewTransaction()
        {
            TransactionModel NewTrans = new TransactionModel();
            //var UserLoad = db.Wallet.Where(o => o.Active == true && o.TransactionID == id).ToList().FirstOrDefault();

            using (db = new VRSystemEntities())
            {
                //NewTrans.Walletlist = db.Wallet.ToList();
                NewTrans.TransactionTypelist = db.TransactionType.Where(o => o.Active == true).ToList();
                NewTrans.WalletDetail = new WalletModel();
                NewTrans.UserEntityTypeList = db.UserEntity.Where(o => o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).ToList();

            }
            return View(NewTrans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewTransaction(TransactionModel NewTransaction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (db = new VRSystemEntities())
                    {
                        var InsertTransaction = new Transaction
                        {
                            WalletID = db.Wallet.Where(o => o.UserEntityID == NewTransaction.SelectedUserEntityID && o.EntityID == NewTransaction.SelectedEntityID).FirstOrDefault().WalletID,
                            TransactionTypeID = NewTransaction.TransactionTypeID,
                            Amount = NewTransaction.Amount,
                            Remarks = NewTransaction.Remarks?.Trim(),
                            VehicleID = 0,
                            CreatedBy = CurrentUser.Details.UserID,
                            CreatedDate = DateTime.Now
                        };

                        db.Transaction.Add(InsertTransaction);
                        db.SaveChanges();

                        //eReceipt

                        //var entity = db.UserEntity.Where(o => o.UserEntityID == NewTransaction.SelectedUserEntityID).FirstOrDefault();
                        //var transactiontype = db.TransactionType.Where(o => o.TransactionTypeID == NewTransaction.TransactionTypeID).FirstOrDefault();
                        //Receipt rcpt = new Receipt();
                        //switch (entity.UserEntityName)
                        //{
                        //    case "MAI":
                        //        var MAIEntityinfo = db.MAI.Where(o => o.MAIID == NewTransaction.SelectedEntityID).FirstOrDefault();
                        //        rcpt = new Receipt
                        //        {
                        //            trans_id = NewTransaction.TransactionID.ToString(),
                        //            account_no = "",
                        //            payor_name = MAIEntityinfo.MAIName,
                        //            address = MAIEntityinfo.Address,
                        //            TIN = MAIEntityinfo.TIN,
                        //            transaction_type = NewTransaction.TransactionTypeID.ToString(),
                        //            payment_option = transactiontype.ToString(),
                        //            email = MAIEntityinfo.EmailAddress2,
                        //            amount = NewTransaction.Amount.ToString(),
                        //            special_discount = "",
                        //            discount_id = "",
                        //            particulars = ""
                        //        };
                        //        break;
                        //    case "Dealer":
                        //        var DealerEntityinfo = db.Dealer.Where(o => o.DealerID == NewTransaction.SelectedEntityID).FirstOrDefault();
                        //        rcpt = new Receipt
                        //        {
                        //            trans_id = NewTransaction.TransactionID.ToString(),
                        //            account_no = "",
                        //            payor_name = DealerEntityinfo.DealerName,
                        //            address = DealerEntityinfo.Address,
                        //            TIN = DealerEntityinfo.TIN,
                        //            transaction_type = NewTransaction.TransactionTypeID.ToString(),
                        //            payment_option = transactiontype.ToString(),
                        //            email = DealerEntityinfo.EmailAddress2,
                        //            amount = NewTransaction.Amount.ToString(),
                        //            special_discount = "",
                        //            discount_id = "",
                        //            particulars = ""
                        //        };
                        //        break;
                        //    case "Insurance":
                        //        var InsuranceEntityinfo = db.Insurance.Where(o => o.InsuranceID == NewTransaction.SelectedEntityID).FirstOrDefault(); rcpt = new Receipt
                        //        {
                        //            trans_id = NewTransaction.TransactionID.ToString(),
                        //            account_no = "",
                        //            payor_name = InsuranceEntityinfo.InsuranceName,
                        //            address = InsuranceEntityinfo.Address,
                        //            TIN = InsuranceEntityinfo.TIN,
                        //            transaction_type = NewTransaction.TransactionTypeID.ToString(),
                        //            payment_option = transactiontype.ToString(),
                        //            email = InsuranceEntityinfo.EmailAddress2,
                        //            amount = NewTransaction.Amount.ToString(),
                        //            special_discount = "",
                        //            discount_id = "",
                        //            particulars = ""
                        //        };
                        //        break;
                        //}


                        //var result = Functions.eReceiptConnect(rcpt);
                        
                        TempData["SuccessMessage"] = "New Transaction!";
                        NewTransaction.WalletDetail = Functions.GetWalletDetails(NewTransaction.SelectedUserEntityID, NewTransaction.SelectedEntityID);
                        NewTransaction.UserEntityTypeList = db.UserEntity.Where(o => o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).ToList();
                        NewTransaction.TransactionTypelist = db.TransactionType.Where(o => o.Active == true).ToList();
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error has occured.";
                    throw;
                }

            }
            return View(NewTransaction);
        }


        public ActionResult TransactionTypeList()
        {
            using (db = new VRSystemEntities())
            {
                var TransactionTypeList = db.vwTransactionTypeList.Where(o => o.Active).ToList();
                return View(TransactionTypeList);
            }

        }
        [HttpGet]

        public ActionResult TransactionType(int? id)
        {
            using (db = new VRSystemEntities())
            {
                TransactionTypeModel TransactionType = new TransactionTypeModel();
                TransactionType.EntryTypeList = db.TransactionEntryType.ToList();
                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var Load = db.TransactionType.Where(o => o.TransactionTypeID == id).FirstOrDefault();

                    TransactionType.TransactionTypeID = Load.TransactionTypeID;
                    TransactionType.TransactionName = Load.TransactionName;
                    TransactionType.TransactionEntryTypeID = Load.TransactionEntryTypeID;
                }
                return View(TransactionType);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult TransactionType(TransactionTypeModel TransactionType, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewTransactionType = new TransactionType
                            {
                                TransactionName = TransactionType.TransactionName.Trim(),
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true,
                                TransactionEntryTypeID = TransactionType.TransactionEntryTypeID
                            };
                            db.TransactionType.Add(NewTransactionType);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.TransactionType.Where(o => o.TransactionTypeID == TransactionType.TransactionTypeID).FirstOrDefault();
                            Update.TransactionName = TransactionType.TransactionName.Trim();
                            Update.TransactionEntryTypeID = TransactionType.TransactionEntryTypeID;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update_active = db.TransactionType.Where(o => o.TransactionTypeID == TransactionType.TransactionTypeID).FirstOrDefault();
                            Update_active.Active = false;
                            Update_active.UpdatedBy = CurrentUser.Details.UserID;
                            Update_active.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Sucessfully!";
                        }
                        break;
                }
                return RedirectToAction("TransactionTypeList");
            }
            else
            {
                //Title.TitleTypeList = db.TitleType.ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(TransactionType);
            }

        }

        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        public ActionResult MyWallet()
        {
            WalletModel WalletDetail = Functions.GetWalletDetails(CurrentUser.Details.UserEntityID, 
                CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI ?
                (int)CurrentUser.Details.SubReferenceID :
                (int)CurrentUser.Details.ReferenceID);
            return View(WalletDetail);
        }

        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        [HttpPost]
        public ActionResult MyWallet(WalletModel model)
        {
            using (db = new VRSystemEntities())
            {
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia)
                {
                    int EntityID = model.UserEntityID == (int)UserEntityEnum.MAI ? model.SubReferenceID : model.ReferenceID;
                    var UpdateWallet = db.Wallet.Where(o => o.UserEntityID == model.UserEntityID && o.EntityID == EntityID).FirstOrDefault();
                    UpdateWallet.Threshold = Convert.ToDecimal(model.ThresholdInput);
                    db.SaveChanges();
                    model = Functions.GetWalletDetails(model.UserEntityID, EntityID);

                    Response.Redirect(Request.UrlReferrer.ToString(), false);
                }
                else
                {
                    int EntityID = CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI ? (int)CurrentUser.Details.SubReferenceID : (int)CurrentUser.Details.ReferenceID;
                    var UpdateWallet = db.Wallet.Where(o => o.UserEntityID == CurrentUser.Details.UserEntityID && o.EntityID == EntityID).FirstOrDefault();
                    UpdateWallet.Threshold = Convert.ToDecimal(model.ThresholdInput);
                    db.SaveChanges();
                    model = Functions.GetWalletDetails(CurrentUser.Details.UserEntityID, EntityID);
                }


                return View(model);
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetEntityList(int EntityTypeID)
        {
            using (var db = new VRSystemEntities())
            {
                switch (EntityTypeID)
                {
                    case (int)UserEntityEnum.MAI:
                        var MAIResult = db.MAI.Where(o => o.Active == true)
                            .Select(o => new
                            {
                                o.MAIID,
                                o.MAIName
                            }).ToList();
                        return Json(MAIResult, JsonRequestBehavior.AllowGet);
                    case (int)UserEntityEnum.Dealer:
                        var DealerResult = db.Dealer.Where(o => o.Active == true)
                            .Select(o => new
                            {
                                o.DealerID,
                                o.DealerName
                            }).ToList();
                        return Json(DealerResult, JsonRequestBehavior.AllowGet);
                    case (int)UserEntityEnum.Insurance:
                        var InsuranceResult = db.Insurance.Where(o => o.Active == true)
                            .Select(o => new
                            {
                                o.InsuranceID,
                                o.InsuranceName
                            }).ToList();
                        return Json(InsuranceResult, JsonRequestBehavior.AllowGet);
                    default:
                        return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult GetTransactionList(int EntityTypeID, int EntityID)
        {
            WalletModel WalletDetail = Functions.GetWalletDetails(EntityTypeID, EntityID);
            WalletDetail.UserEntityID = EntityTypeID;
            if (EntityTypeID == (int)UserEntityEnum.MAI)
            {
                WalletDetail.SubReferenceID = EntityID;
            }
            else
            {
                WalletDetail.ReferenceID = EntityID;
            }
            return PartialView("_TransactionHistory", WalletDetail);
        }

        #region [ Entity Type Charge  ]
      
        [SessionExpire]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
       
        [HttpGet]
        public ActionResult EntityTypeChargeList()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var transactionentitytypelist = db.vwTransactionEntityType.Where(o => o.Active == true).ToList();

                return View(transactionentitytypelist);
            }
        }
        /// <summary>
        /// Register
        /// </summary>


        [HttpGet]
        public ActionResult EntityTypeCharge(int? id)
        {
            TransactionEntityTypeModel TETM = new TransactionEntityTypeModel();

            using (db = new VRSystemEntities())
            {
                TETM.TransactionTypelist = db.TransactionType.Where(o => o.Active == true).ToList();
                TETM.UserEntityList = db.UserEntity.Where(o => o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).ToList();
                
                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    var Load = db.TransactionEntityType.Where(o => o.TransactionEntityTypeID == id).FirstOrDefault();
                    TETM.TransactionEntityTypeID = Load.TransactionEntityTypeID;
                    TETM.UserEntityID = Load.UserEntityID;
                    TETM.TransactionTypeID = Load.TransactionTypeID;
                    TETM.Amount = Load.Amount;
                    TETM.EffectivityDate = Load.EffectivityDate;

                    ViewBag.Edit = true;
                }
            }
            return View(TETM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EntityTypeCharge(TransactionEntityTypeModel EntityTypeCharge, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var Insert = new TransactionEntityType
                            {
                                UserEntityID = EntityTypeCharge.UserEntityID,
                                TransactionTypeID = EntityTypeCharge.TransactionTypeID,
                                Amount = EntityTypeCharge.Amount,
                                EffectivityDate = EntityTypeCharge.EffectivityDate,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            db.TransactionEntityType.Add(Insert);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.TransactionEntityType.Where(o => o.TransactionEntityTypeID == EntityTypeCharge.TransactionEntityTypeID).FirstOrDefault();
                            Update.UserEntityID = EntityTypeCharge.UserEntityID;
                            Update.TransactionTypeID = EntityTypeCharge.TransactionTypeID;
                            Update.Amount = EntityTypeCharge.Amount;
                            Update.EffectivityDate = EntityTypeCharge.EffectivityDate;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Delete = db.TransactionEntityType.Where(o => o.TransactionEntityTypeID == EntityTypeCharge.TransactionEntityTypeID).FirstOrDefault();
                            Delete.Active = false;
                            Delete.UpdatedBy = CurrentUser.Details.UserID;
                            Delete.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Sucessfully!";
                        }
                        break;
                }
                return RedirectToAction("EntityTypeChargeList");
            }
            else
            {
                //Title.TitleTypeList = db.TitleType.ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(EntityTypeCharge);
            }
        }
        #endregion

        #region [ Entity Charge ]
        [HttpGet]
        public ActionResult EntityChargeList()
        {
            using (db = new VRSystemEntities())
            {
                var ModelList = db.vwTransactionEntityList.Where(o => o.Active == true).ToList();
                return View(ModelList);
            }
           
        }

        public PartialViewResult GetEntityChargeList(int EntityTypeID, int EntityID)
        {
            var ModelList = db.vwTransactionEntityList.Where(o => o.UserEntityID == EntityTypeID && o.EntityID == EntityID &&  o.Active == true).ToList();
            return PartialView("_EntityChargeList", ModelList);
        }

        [HttpGet]
        public ActionResult EntityCharge(int? id)
        {
            TransactionEntityModel TEM = new TransactionEntityModel();
            using (db = new VRSystemEntities())
            {
                TEM.TransactionTypelist = db.TransactionType.Where(o => o.Active == true).ToList();
                TEM.UserEntityList = db.UserEntity.Where(o => o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).ToList();
                TEM.MAIList = db.MAI.Select(o => new { o.MAIID, o.MAIName, o.Active }).Where(o => o.Active == true)
                    .Select(o => new MAIModel
                    {
                        MAIID = o.MAIID,
                        MAIName = o.MAIName
                    }).ToList();
                TEM.DealerList = db.Dealer.Select(o => new { o.DealerID, o.DealerName, o.Active }).Where(o => o.Active == true)
                    .Select(o => new DealerModel
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    })
                    .ToList();
                TEM.InsuranceList = db.Insurance.Select(o => new { o.InsuranceID, o.InsuranceName, o.Active }).Where(o => o.Active == true)
                    .Select(o => new InsuranceModel
                    {
                        InsuranceID = o.InsuranceID,
                        InsuranceName = o.InsuranceName
                    })
                    .ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    var Load = db.TransactionEntity.Where(o => o.TransactionEntityTypeID == id).FirstOrDefault();
                    TEM.TransactionEntityTypeID = Load.TransactionEntityTypeID;
                    TEM.UserEntityID = Load.UserEntityID;
                    TEM.EntityID = Load.EntityID;
                    TEM.TransactionTypeID = Load.TransactionTypeID;
                    TEM.Amount = Load.Amount;
                    TEM.EffectivityDate = Load.EffectivityDate;

                    ViewBag.Edit = true;
                }

                return View(TEM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EntityCharge(TransactionEntityModel EntityCharge, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var Insert = new TransactionEntity
                            {
                                UserEntityID = EntityCharge.UserEntityID,
                                EntityID = EntityCharge.EntityID,
                                TransactionTypeID = EntityCharge.TransactionTypeID,
                                Amount = EntityCharge.Amount,
                                EffectivityDate = EntityCharge.EffectivityDate,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            db.TransactionEntity.Add(Insert);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.TransactionEntity.Where(o => o.TransactionEntityTypeID == EntityCharge.TransactionEntityTypeID).FirstOrDefault();
                            Update.EntityID = EntityCharge.EntityID;
                            Update.TransactionTypeID = EntityCharge.TransactionTypeID;
                            Update.Amount = EntityCharge.Amount;
                            Update.EffectivityDate = EntityCharge.EffectivityDate;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Delete = db.TransactionEntity.Where(o => o.TransactionEntityTypeID == EntityCharge.TransactionEntityTypeID).FirstOrDefault();
                            Delete.Active = false;
                            Delete.UpdatedBy = CurrentUser.Details.UserID;
                            Delete.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Sucessfully!";
                        }
                        break;
                }
                return RedirectToAction("EntityChargeList");
            }
            else
            {
                //Title.TitleTypeList = db.TitleType.ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(EntityCharge);
            }
        }
        #endregion

        #region [ Entity Branch Charge ]

        public ActionResult EntityBranchChargeList()
        {
            using (db = new VRSystemEntities())
            {
                var ModelList = db.vwTransactionEntityBranchList.Where(o => o.Active == true).ToList();
                return View(ModelList);
            }

        }

        [HttpGet]
        public ActionResult EntityBranchCharge(int? id)
        {
            TransactionEntityBranchModel TEBM = new TransactionEntityBranchModel();
            using (db = new VRSystemEntities())
            {
                TEBM.TransactionTypelist = db.TransactionType.Where(o => o.Active == true).ToList();
                TEBM.UserEntityList = db.UserEntity.Where(o => o.UserEntityID != (int)UserEntityEnum.DataBridgeAsia).ToList();
                TEBM.MAIList = db.MAI.Select(o => new { o.MAIID, o.MAIName, o.Active }).Where(o => o.Active == true)
                    .Select(o => new MAIModel
                    {
                        MAIID = o.MAIID,
                        MAIName = o.MAIName
                    }).ToList();
                TEBM.DealerList = db.Dealer.Select(o => new { o.DealerID, o.DealerName, o.Active }).Where(o => o.Active == true)
                    .Select(o => new DealerModel
                    {
                        DealerID = o.DealerID,
                        DealerName = o.DealerName
                    })
                    .ToList();
                TEBM.DealerBranchList = new List<DealerBranchModel>();
                TEBM.InsuranceList = db.Insurance.Select(o => new { o.InsuranceID, o.InsuranceName, o.Active }).Where(o => o.Active == true)
                    .Select(o => new InsuranceModel
                    {
                        InsuranceID = o.InsuranceID,
                        InsuranceName = o.InsuranceName
                    })
                    .ToList();
                TEBM.InsuranceBranchList = new List<InsuranceBranchModel>();

                if (id == null)
                {
                    ViewBag.Edit = false;
                    TEBM.EffectivityDate = DateTime.Now;
                }
                else
                {
                    var Load = db.TransactionEntityBranch.Where(o => o.TransactionEntityBranchID == id).FirstOrDefault();
                    TEBM.TransactionEntityBranchID = Load.TransactionEntityBranchID;
                    TEBM.UserEntityID = Load.UserEntityID;
                    TEBM.EntityID = Load.EntityID;
                    TEBM.BranchID = Load.BranchID;
                    TEBM.TransactionTypeID = Load.TransactionTypeID;
                    TEBM.Amount = Load.Amount;
                    TEBM.EffectivityDate = Load.EffectivityDate;

                    ViewBag.Edit = true;

                    switch(TEBM.UserEntityID)
                    {
                        case (int)UserEntityEnum.Dealer:
                            TEBM.DealerBranchList = db.DealerBranch.Where(o => o.DealerID == TEBM.EntityID && o.Active == true)
                                .Select(x => new DealerBranchModel
                                {
                                    DealerBranchID = x.DealerBranchID,
                                    DealerBranchName = x.DealerBranchName
                                })
                                .ToList();
                            break;
                        case (int)UserEntityEnum.Insurance:
                            TEBM.InsuranceBranchList = db.InsuranceBranch.Where(o => o.InsuranceID == TEBM.EntityID && o.Active == true)
                                .Select(x => new InsuranceBranchModel
                                {
                                    InsuranceBranchID = x.InsuranceBranchID,
                                    InsuranceBranchName = x.InsuranceBranchName
                                })
                                .ToList();
                            break;
                    }
                }

                return View(TEBM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EntityBranchCharge(TransactionEntityBranchModel EntityBranchCharge, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var Insert = new TransactionEntityBranch
                            {
                                UserEntityID = EntityBranchCharge.UserEntityID,
                                EntityID = EntityBranchCharge.EntityID,
                                BranchID = EntityBranchCharge.BranchID,
                                TransactionTypeID = EntityBranchCharge.TransactionTypeID,
                                Amount = EntityBranchCharge.Amount,
                                EffectivityDate = EntityBranchCharge.EffectivityDate,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            db.TransactionEntityBranch.Add(Insert);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.TransactionEntityBranch.Where(o => o.TransactionEntityBranchID == EntityBranchCharge.TransactionEntityBranchID).FirstOrDefault();
                            Update.EntityID = EntityBranchCharge.EntityID;
                            Update.BranchID = EntityBranchCharge.BranchID;
                            Update.TransactionTypeID = EntityBranchCharge.TransactionTypeID;
                            Update.Amount = EntityBranchCharge.Amount;
                            Update.EffectivityDate = EntityBranchCharge.EffectivityDate;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Delete = db.TransactionEntityBranch.Where(o => o.TransactionEntityBranchID == EntityBranchCharge.TransactionEntityBranchID).FirstOrDefault();
                            Delete.Active = false;
                            Delete.UpdatedBy = CurrentUser.Details.UserID;
                            Delete.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Sucessfully!";
                        }
                        break;
                }
                return RedirectToAction("EntityBranchChargeList");
            }
            else
            {
                //Title.TitleTypeList = db.TitleType.ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(EntityBranchCharge);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetEntityBranchList(int UserEntityID, int EntityID)
        {
            using (var db = new VRSystemEntities())
            {
                switch (UserEntityID)
                {
                    //case (int)UserEntityEnum.MAI:
                    //    var MAIResult = db.MAI.Where(o => o.Active == true)
                    //        .Select(o => new
                    //        {
                    //            o.MAIID,
                    //            o.MAIName
                    //        }).ToList();
                    //    return Json(MAIResult, JsonRequestBehavior.AllowGet);
                    case (int)UserEntityEnum.Dealer:
                        var DealerBranchResult = db.DealerBranch.Where(o => o.Active == true && o.DealerID == EntityID)
                            .Select(o => new
                            {
                                o.DealerBranchID,
                                o.DealerBranchName
                            }).ToList();
                        return Json(DealerBranchResult, JsonRequestBehavior.AllowGet);
                    case (int)UserEntityEnum.Insurance:
                        var InsuranceBranchResult = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceID == EntityID)
                            .Select(o => new
                            {
                                o.InsuranceBranchID,
                                o.InsuranceBranchName
                            }).ToList();
                        return Json(InsuranceBranchResult, JsonRequestBehavior.AllowGet);
                    default:
                        return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion
    }
}