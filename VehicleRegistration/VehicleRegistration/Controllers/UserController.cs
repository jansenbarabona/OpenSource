using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using VehicleRegistration.Models;
using VehicleRegistration.Tools;
using ZohoMail;

namespace VehicleRegistration.Controllers
{
    public class UserController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: User
        [SessionExpire]
        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        public ActionResult Index()
        {
            List<vwUserList> userlist = new List<vwUserList>();
            using (db = new VRSystemEntities())
            {
                switch (CurrentUser.Details.UserEntityID)
                {
                    case (int)UserEntityEnum.DataBridgeAsia:
                        userlist = db.vwUserList.Where(o => o.Active == true).ToList();
                        break;
                    case (int)UserEntityEnum.MAI:
                        userlist = db.vwUserList.Where(o => o.Active == true
                        && o.ReferenceID == CurrentUser.Details.ReferenceID
                        && o.SubReferenceID == CurrentUser.Details.SubReferenceID
                        && o.UserEntityID == CurrentUser.Details.UserEntityID
                        ).ToList();
                        break;
                    case (int)UserEntityEnum.Dealer:
                    case (int)UserEntityEnum.Insurance:
                        if (CurrentUser.Details.IsMain == true)
                            userlist = db.vwUserList.Where(o => o.Active == true
                            && o.ReferenceID == CurrentUser.Details.ReferenceID
                            && o.UserEntityID == CurrentUser.Details.UserEntityID
                            ).ToList();
                        else if (CurrentUser.Details.IsMain == false)
                            userlist = db.vwUserList.Where(o => o.Active == true
                            && o.ReferenceID == CurrentUser.Details.ReferenceID
                            && o.UserEntityID == CurrentUser.Details.UserEntityID
                            && o.UserID == CurrentUser.Details.UserRoleID
                            ).ToList();
                        else
                            userlist = db.vwUserList.Where(o => o.Active == true
                            && o.ReferenceID == CurrentUser.Details.ReferenceID
                            && o.SubReferenceID == CurrentUser.Details.SubReferenceID
                            && o.UserEntityID == CurrentUser.Details.UserEntityID
                            ).ToList();
                        break;
                }


                return View(userlist);
            }
        }

        [SessionExpire]
        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        [HttpGet]
        public ActionResult User_Register(int? id)
        {
            ViewBag.id = id;

            UserModel NewUser = new UserModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;


                //NewUser.UserEntityList = db.UserEntity.Where(o => o.Active == true).ToList();
                //NewUser.UserRoleList = db.UserRole.Where(o => o.Active == true).ToList();

                NewUser = GetListByEntityAndRole(NewUser);
                NewUser.LTOUserTypeList = db.LTOUserType.Where(o => o.Active == true).ToList();
                if (id == null)
                {
                    ViewBag.Edit = false;
                    if (CurrentUser.Details.UserEntityID != (int)UserEntityEnum.DataBridgeAsia)
                    {
                        NewUser.SelectedUserEntityID = CurrentUser.Details.UserEntityID;
                        NewUser.ReferenceID = CurrentUser.Details.ReferenceID;
                        if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                        {
                            if (CurrentUser.Details.IsMain == true)
                                NewUser.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerID == CurrentUser.Details.ReferenceID).ToList();
                            else
                                NewUser.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerBranchID == CurrentUser.Details.SubReferenceID).ToList();
                        }

                        if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                            NewUser.SubReferenceID = CurrentUser.Details.SubReferenceID;
                    }
                }
                else
                {
                    ViewBag.Edit = true;

                    var UserLoad = db.User.Where(o => o.Active == true && o.UserID == id).ToList().FirstOrDefault();

                    switch (UserLoad.UserEntityID)
                    {
                        case (int)UserEntityEnum.MAI:
                            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.MAI)
                            {
                                NewUser.MAITypeList = db.MAIType.Where(o => o.Active == true && o.MAITypeID == UserLoad.ReferenceID).ToList();
                                NewUser.MAIList = db.MAI.Where(o => o.Active == true && o.MAIID == UserLoad.SubReferenceID).ToList();
                            }
                            else
                            {
                                NewUser.MAITypeList = db.MAIType.Where(o => o.Active == true).ToList();
                                NewUser.MAIList = db.MAI.Where(o => o.Active == true && o.MAITypeID == UserLoad.ReferenceID).ToList();
                            }
                            break;
                        case (int)UserEntityEnum.Dealer:
                            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                            {
                                NewUser.DealerList = db.Dealer.Where(o => o.Active == true && o.DealerID == UserLoad.ReferenceID).ToList();
                                if (CurrentUser.Details.IsMain == true)
                                    NewUser.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerID == UserLoad.ReferenceID).ToList();
                                else
                                    NewUser.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerBranchID == UserLoad.SubReferenceID).ToList();
                            }
                            else
                            {
                                NewUser.DealerList = db.Dealer.Where(o => o.Active == true).ToList();
                                NewUser.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerID == UserLoad.ReferenceID).ToList();
                            }
                            //NewUser.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerBranchID == UserLoad.SubReferenceID).ToList();
                            break;
                        case (int)UserEntityEnum.Insurance:
                            if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Insurance)
                            {
                                NewUser.InsuranceList = db.Insurance.Where(o => o.Active == true && o.InsuranceID == UserLoad.ReferenceID).ToList();
                                NewUser.InsuranceBranchist = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceBranchID == UserLoad.SubReferenceID).ToList();
                            }
                            else
                            {
                                NewUser.InsuranceList = db.Insurance.Where(o => o.Active == true).ToList();
                                NewUser.InsuranceBranchist = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceID == UserLoad.ReferenceID).ToList();
                            }
                            break;
                    }

                    NewUser.UserID = UserLoad.UserID;
                    NewUser.SelectedUserEntityID = UserLoad.UserEntityID;
                    NewUser.SelectedUserRoleID = UserLoad.UserRoleID;
                    NewUser.EmailAddress = UserLoad.EmailAddress;
                    NewUser.Password = UserLoad.Password.Encrypt(UserLoad.EmailAddress);
                    NewUser.LastName = UserLoad.LastName;
                    NewUser.FirstName = UserLoad.FirstName;
                    NewUser.ReferenceID = UserLoad.ReferenceID;
                    NewUser.SubReferenceID = UserLoad.SubReferenceID;
                }

                return PartialView(NewUser);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserRole = new[] { UserRoleEnum.Administrator })]
        public ActionResult User_Register(UserModel User, string submit, int? id)
        {
            try
            {


                User = GetListByEntityAndRole(User);

                if (CurrentUser.Details.UserEntityID != (int)UserEntityEnum.DataBridgeAsia)
                {
                    User.SelectedUserEntityID = CurrentUser.Details.UserEntityID;
                    User.ReferenceID = CurrentUser.Details.UserEntityID;
                }
                //User.UserEntityList = db.UserEntity.Where(o => o.Active == true).ToList();
                //User.UserRoleList = db.UserRole.Where(o => o.Active == true).ToList();
                //var UserLoad = db.User.Where(o => o.Active == true && o.UserID == id).FirstOrDefault();
                //switch (UserLoad.UserEntityID)
                //{
                //    case 2:
                //        User.MAITypeList = db.MAIType.Where(o => o.Active == true && o.MAITypeID == UserLoad.ReferenceID).ToList();
                //        User.MAIList = db.MAI.Where(o => o.Active == true && o.MAIID == UserLoad.SubReferenceID).ToList();
                //        break;
                //    case 3:
                //        User.DealerList = db.Dealer.Where(o => o.Active == true && o.DealerID == UserLoad.ReferenceID).ToList();
                //        User.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerBranchID == UserLoad.SubReferenceID).ToList();
                //        break;
                //    case 4:
                //        User.InsuranceList = db.Insurance.Where(o => o.Active == true && o.InsuranceID == UserLoad.ReferenceID).ToList();
                //        User.InsuranceBranchist = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceBranchID == UserLoad.SubReferenceID).ToList();
                //        break;
                //}
                if (ModelState.IsValid)
                {
                    switch (submit)
                    {
                        case "Create":
                            var CheckEmail = db.User.Where(o => o.Active == true && o.EmailAddress == User.EmailAddress).FirstOrDefault()?.EmailAddress;
                            if (CheckEmail != null)
                            {
                                TempData["ErrorMessage"] = "The email '" + CheckEmail + "' is already registered.";
                                return RedirectToAction("Index");
                            }

                            if (User.SelectedUserEntityID != (int)UserEntityEnum.DataBridgeAsia)
                            {
                                switch (User.SelectedUserEntityID)
                                {
                                    case (int)UserEntityEnum.MAI:

                                        if (User.SubReferenceID == null)
                                        {
                                            ViewBag.Edit = false;
                                            TempData["ErrorMessage"] = "MAI Branch is required.";
                                            return RedirectToAction("Index");

                                        }
                                        break;
                                    case (int)UserEntityEnum.Dealer:


                                        if (User.SubReferenceID == null)
                                        {
                                            ViewBag.Edit = false;
                                            TempData["ErrorMessage"] = "Dealer Branch is required.";
                                            return RedirectToAction("Index");

                                        }
                                        break;
                                    case (int)UserEntityEnum.Insurance:
                                        if (User.SubReferenceID == null)
                                        {
                                            ViewBag.Edit = false;
                                            TempData["ErrorMessage"] = "Insurance is required.";
                                            return RedirectToAction("Index");

                                        }
                                        break;
                                    case (int)UserEntityEnum.LTO:

                                        if (User.SubReferenceID == null)
                                        {
                                            ViewBag.Edit = false;
                                            TempData["ErrorMessage"] = "LTO Region is required.";
                                            return RedirectToAction("Index");

                                        }
                                        break;

                                }
                            }
                            using (db = new VRSystemEntities())
                            {

                                var NewUser = new User
                                {
                                    UserEntityID = User.SelectedUserEntityID,
                                    UserRoleID = User.SelectedUserRoleID,
                                    EmailAddress = User.EmailAddress.Trim(),
                                    Password = User.Password.Encrypt(User.EmailAddress),
                                    LastName = User.LastName.Trim(),
                                    FirstName = User.FirstName.Trim(),
                                    CreatedBy = @CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                    Active = true,
                                    ReferenceID = CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia ? User.ReferenceID : CurrentUser.Details.ReferenceID,
                                    SubReferenceID = User.SubReferenceID,
                                    LTOUserTypeID = User.SelectedLTOUserTypeID
                                };
                                db.User.Add(NewUser);
                                db.SaveChanges();

                                TempData["SuccessMessage"] = "Sucessfully Registered!";

                            }
                            break;
                        case "Save":
                            using (db = new VRSystemEntities())
                            {
                                var UpdateUser = db.User.Where(o => o.UserID == id).FirstOrDefault();
                                UpdateUser.UserEntityID = User.SelectedUserEntityID;
                                UpdateUser.UserRoleID = User.SelectedUserRoleID;
                                UpdateUser.EmailAddress = User.EmailAddress.Trim();
                                //UpdateUser.Password = User.Password.Encrypt(User.EmailAddress);
                                UpdateUser.LastName = User.LastName.Trim();
                                UpdateUser.FirstName = User.FirstName.Trim();
                                UpdateUser.ReferenceID = CurrentUser.Details.UserEntityID == (int)UserEntityEnum.DataBridgeAsia ? User.ReferenceID : CurrentUser.Details.ReferenceID;
                                UpdateUser.SubReferenceID = User.SubReferenceID;
                                UpdateUser.LTOUserTypeID = User.SelectedLTOUserTypeID;
                                db.SaveChanges();
                                TempData["SuccessMessage"] = "Updated Sucessfully!";
                            }
                            break;
                        case "Delete":
                            using (db = new VRSystemEntities())
                            {
                                var UpdateUser = db.User.Where(o => o.UserID == id).FirstOrDefault();
                                UpdateUser.Active = false;
                                db.SaveChanges();
                                TempData["WarningMessage"] = "Removed Successfully!";
                            }
                            break;
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    if (submit == "Create")
                        ViewBag.Edit = false;
                    return View(User);
                }
            }
            catch (Exception)
            {

                TempData["ErrorMessage"] = "An error has occured.";
                return View(User);
            }
        }
        public ActionResult Login()
        {
            if (CurrentUser.Details != null)
                return RedirectToAction("Dashboard", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserModel User)
        {
            try
            {
                if (User.Password.Trim() != string.Empty)
                {
                    using (db = new VRSystemEntities())
                    {
                        var EncryptedPassword = User.Password.Encrypt(User.EmailAddress);
                        var LoggedUser = db.vwUserList.Where(o => o.EmailAddress == User.EmailAddress && o.Password == EncryptedPassword && o.Active == true).FirstOrDefault();
                        var Entity_Logos = "";


                        if (LoggedUser != null)
                        {
                            if (LoggedUser.UserEntityID == (int)UserEntityEnum.MAI)
                            {
                                var MAI_Entity = db.MAI.Where(o => o.MAIID == LoggedUser.SubReferenceID).FirstOrDefault();
                                Entity_Logos = MAI_Entity.Logo;
                            }
                            else if (LoggedUser.UserEntityID == (int)UserEntityEnum.Dealer)
                            {
                                var Dealer_Entity = db.Dealer.Where(o => o.DealerID == LoggedUser.ReferenceID).FirstOrDefault();
                                Entity_Logos = Dealer_Entity.Logo;

                                if (LoggedUser.SubReferenceID == null)
                                {
                                    TempData["ErrorMessage"] = "Something is wrong with your user account, please contact databridge.";
                                    return RedirectToAction("Dashboard", "Home");
                                }

                            }
                            else if (LoggedUser.UserEntityID == (int)UserEntityEnum.Insurance)
                            {
                                var Insurane_Entity = db.Insurance.Where(o => o.InsuranceID == LoggedUser.ReferenceID).FirstOrDefault();
                                Entity_Logos = Insurane_Entity.Logo;

                                if (LoggedUser.SubReferenceID == null)
                                {
                                    TempData["ErrorMessage"] = "Something is wrong with your user account, please contact databridge.";
                                    return RedirectToAction("Dashboard", "Home");
                                }
                            }
                            //else if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.LTO)
                            //{
                            //    var LTO_Entity = db.lto.Where(o => o.id == LoggedUser.UserEntityID).FirstOrDefault();
                            //    CurrentUser.Details.EntityLogo =
                            //}

                            CurrentUser.Details = LoggedUser.ToModel<vwUserList, vwUserListModel>();
                            CurrentUser.Details.UploadVersion = db.DealerBranch.Where(o => o.Active == true && o.DealerID == LoggedUser.ReferenceID && o.DealerBranchID == LoggedUser.SubReferenceID).FirstOrDefault()?.UploadVersion;
                            CurrentUser.Details.Password = string.Empty;
                            CurrentUser.Details.EntityLogo = Entity_Logos;

                            db.AuditLogin.Add(new AuditLogin
                            {
                                UserID = CurrentUser.Details.UserID,
                                LoginDate = DateTime.Now
                            });
                            db.SaveChanges();

                            if (Session["RedirectFromLogin"] != null)
                            {
                                string RedirectURL = Session["RedirectFromLogin"].ToString();
                                Session.Remove("RedirectFromLogin");
                                return new RedirectResult(RedirectURL);

                            }

                            else
                                return RedirectToAction("Dashboard", "Home");


                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Username and Password does not match.";
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error has occured.";
            }

            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel User)
        {

            var validate_email = db.User.Where(o => o.EmailAddress == User.EmailAddress_Password).Select(o => o.EmailAddress).FirstOrDefault();
            if (validate_email != null)
            {
                string newPassword = Functions.RandomString(6);

                List<string> Recipients = new List<string>();
                Recipients.Add(User.EmailAddress_Password.Trim());

                string EmailBody;
                using (StreamReader srBody = new StreamReader(Server.MapPath(string.Format("~/MailBody/ForgotPassword.html"))))
                {
                    EmailBody = srBody.ReadToEnd();
                }
                EmailBody = EmailBody.Replace("@NewPassword", newPassword);

                if (ZohoMailSender.SendEmail("DORS Reset Password Request", EmailBody, Recipients))
                {
                    User.Password = newPassword;
                    var UpdatePassword = db.User.Where(o => o.EmailAddress == User.EmailAddress_Password).FirstOrDefault();
                    UpdatePassword.Password = User.Password.Encrypt(User.EmailAddress_Password);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Email sent.";
                }
                else
                    TempData["ErrorMessage"] = "An error has occured.";
            }
            else
            {
                TempData["ErrorMessage"] = "This email address doesn't exist!";
            }
            return RedirectToAction("Login", "User");
        }

        #region [ References ]
        // GET Reference
        public ActionResult GetMAI_ReferenceID(int entity)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.MAIType.Where(o => o.Active == true).ToList().Select(
                    o => new
                    {
                        o.MAITypeID,
                        o.MAITypeName
                    });
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDealer_ReferenceID(int entity)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.Dealer.Where(o => o.Active == true).Select(
                    o => new
                    {
                        o.DealerID,
                        o.DealerName
                    }).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetInsurance_ReferenceID(int entity)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.Insurance.Where(o => o.Active == true).Select(
                    o => new
                    {
                        o.InsuranceID,
                        o.InsuranceName
                    }).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLTO_ReferenceID(int entity)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.LTO.Where(o => o.Active == true).Select(
                    o => new
                    {
                        o.LTOID,
                        o.LTOName
                    }).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        // GET Subreference
        public ActionResult GetMAI_SubReferenceID(int reference_id)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.MAI.Where(o => o.Active == true && o.MAITypeID == reference_id).Select(
                    o => new
                    {
                        o.MAIID,
                        o.MAIName
                    }).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDealer_SubReferenceID(int reference_id)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.DealerBranch.Where(o => o.Active == true && o.DealerID == reference_id).Select(
                    o => new
                    {
                        o.DealerBranchID,
                        o.DealerBranchName
                    }).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetInsurance_SubReferenceID(int reference_id)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceID == reference_id).Select(
                    o => new
                    {
                        o.InsuranceBranchID,
                        o.InsuranceBranchName
                    }).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetLTO_SubReferenceID(int reference_id)
        {
            using (var db = new VRSystemEntities())
            {
                var searchresult = db.LTOBranch.Where(o => o.Active == true && o.LTOID == reference_id).Select(
                    o => new
                    {
                        o.LTOBranchID,
                        o.LTOBranchName
                    }).ToList();
                return Json(searchresult, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [ Functions ]

        private UserModel GetListByEntityAndRole(UserModel model)
        {
            using (var db = new VRSystemEntities())
            {
                if (CurrentUser.Details.IsMain == false)
                    model.UserRoleList = db.UserRole.Where(o => o.Active == true && o.UserRoleID == 2).ToList();
                else
                    model.UserRoleList = db.UserRole.Where(o => o.Active == true).ToList();

                switch (CurrentUser.Details.UserEntityID)
                {
                    case (int)UserEntityEnum.DataBridgeAsia:
                        model.UserEntityList = db.UserEntity.Where(o => o.Active == true).ToList();

                        break;
                    case (int)UserEntityEnum.MAI:
                        model.UserEntityList = model.UserEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.MAI).ToList();

                        model.MAITypeList = db.MAIType.Where(o => o.MAITypeID == CurrentUser.Details.ReferenceID).ToList();
                        model.MAIList = db.MAI.Where(o => o.MAIID == CurrentUser.Details.SubReferenceID).ToList();
                        break;

                    case (int)UserEntityEnum.Dealer:
                        model.UserEntityList = model.UserEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer).ToList();
                        model.DealerList = db.Dealer.Where(o => o.DealerID == CurrentUser.Details.ReferenceID).ToList();
                        if (Functions.IsMainBranch((UserEntityEnum)CurrentUser.Details.UserEntityID, Convert.ToInt32(CurrentUser.Details.SubReferenceID)))
                            model.DealerBranchList = db.DealerBranch.Where(o => o.DealerID == CurrentUser.Details.ReferenceID).ToList();
                        else
                            model.DealerBranchList = db.DealerBranch.Where(o => o.DealerBranchID == CurrentUser.Details.SubReferenceID).ToList();
                        break;
                    case (int)UserEntityEnum.Insurance:
                        model.UserEntityList = model.UserEntityList.Where(o => o.UserEntityID == (int)UserEntityEnum.Dealer).ToList();
                        model.InsuranceList = db.Insurance.Where(o => o.InsuranceID == CurrentUser.Details.ReferenceID).ToList();
                        if (Functions.IsMainBranch((UserEntityEnum)CurrentUser.Details.UserEntityID, Convert.ToInt32(CurrentUser.Details.SubReferenceID)))
                            model.InsuranceBranchist = db.InsuranceBranch.Where(o => o.InsuranceID == CurrentUser.Details.ReferenceID).ToList();
                        else
                            model.InsuranceBranchist = db.InsuranceBranch.Where(o => o.InsuranceBranchID == CurrentUser.Details.SubReferenceID).ToList();
                        break;
                }
            }


            return model;
        }

        #endregion
    }
}