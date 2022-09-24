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
    public class MyAccountController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: MyAccount
        [HttpGet]
        public ActionResult Index()
        {
            int? id = CurrentUser.Details.UserID;
            ViewBag.id = id;

            UserModel NewUser = new UserModel();
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;


                //NewUser.UserEntityList = db.UserEntity.Where(o => o.Active == true).ToList();
                //NewUser.UserRoleList = db.UserRole.Where(o => o.Active == true).ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var UserLoad = db.User.Where(o => o.Active == true && o.UserID == id).FirstOrDefault();
                    switch (UserLoad.UserEntityID)
                    {
                        case 2:
                            NewUser.ReferenceName = db.MAIType.Where(o => o.Active == true && o.MAITypeID == UserLoad.ReferenceID)
                                .Select(o => o.MAITypeName).FirstOrDefault();
                            NewUser.SubReferenceName = db.MAI.Where(o => o.Active == true && o.MAIID == UserLoad.SubReferenceID)
                                .Select(o => o.MAIName).FirstOrDefault();

                            //NewUser.MAITypeList = db.MAIType.Where(o => o.Active == true && o.MAITypeID == UserLoad.ReferenceID).ToList();
                            //NewUser.MAIList = db.MAI.Where(o => o.Active == true && o.MAIID == UserLoad.SubReferenceID).ToList();
                            break;
                        case 3:
                            NewUser.ReferenceName = db.Dealer.Where(o => o.Active == true && o.DealerID == UserLoad.ReferenceID)
                                .Select(o => o.DealerName).FirstOrDefault();
                            NewUser.SubReferenceName = db.DealerBranch.Where(o => o.Active == true && o.DealerBranchID == UserLoad.SubReferenceID)
                                .Select(o => o.DealerBranchName).FirstOrDefault();

                            //NewUser.DealerList = db.Dealer.Where(o => o.Active == true && o.DealerID == UserLoad.ReferenceID).ToList();
                            //NewUser.DealerBranchList = db.DealerBranch.Where(o => o.Active == true && o.DealerBranchID == UserLoad.SubReferenceID).ToList();
                            break;
                        case 4:
                            NewUser.ReferenceName = db.Insurance.Where(o => o.Active == true && o.InsuranceID == UserLoad.ReferenceID)
                                .Select(o => o.InsuranceName).FirstOrDefault();
                            NewUser.SubReferenceName = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceBranchID == UserLoad.SubReferenceID)
                                .Select(o => o.InsuranceBranchName).FirstOrDefault();

                            //NewUser.InsuranceList = db.Insurance.Where(o => o.Active == true && o.InsuranceID == UserLoad.ReferenceID).ToList();
                            //NewUser.InsuranceBranchist = db.InsuranceBranch.Where(o => o.Active == true && o.InsuranceBranchID == UserLoad.SubReferenceID).ToList();
                            break;
                    }

                    NewUser.UserID = UserLoad.UserID;
                    NewUser.SelectedUserEntityID = UserLoad.UserEntityID;
                    NewUser.SelectedUserRoleID = UserLoad.UserRoleID;
                    NewUser.EmailAddress = UserLoad.EmailAddress;
                    NewUser.LastName = UserLoad.LastName;
                    NewUser.FirstName = UserLoad.FirstName;
                    //NewUser.ReferenceID =  UserLoad.ReferenceID;
                    //NewUser.SubReferenceID = UserLoad.SubReferenceID;
                }

                return View(NewUser);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserModel User, string submit)
        {
            //User.UserEntityList = db.UserEntity.Where(o => o.Active == true).ToList();
            //User.UserRoleList = db.UserRole.Where(o => o.Active == true).ToList();
            //var UserLoad = db.User.Where(o => o.Active == true && o.UserID == CurrentUser.Details.UserID).FirstOrDefault();
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
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var UpdateUser = db.User.Where(o => o.UserID == CurrentUser.Details.UserID).FirstOrDefault();
                            //UpdateUser.UserEntityID = User.SelectedUserEntityID;
                            //UpdateUser.UserRoleID = User.SelectedUserRoleID;
                            if (User.NewPassword != null)
                            {
                                UpdateUser.Password = User.NewPassword.Trim().Encrypt(User.EmailAddress);
                            }
                            UpdateUser.LastName = User.LastName.Trim();
                            UpdateUser.FirstName = User.FirstName.Trim();
                            //UpdateUser.ReferenceID = User.ReferenceID;
                            //UpdateUser.SubReferenceID = User.SubReferenceID;
                            db.SaveChanges();
                        }
                        break;
                }
                TempData["SuccessMessage"] = "Updated Sucessfully!";
            }else
            {
                TempData["ErrorMessage"] = "An error has occured.";
            }

            return View(User);
        }
    }
}