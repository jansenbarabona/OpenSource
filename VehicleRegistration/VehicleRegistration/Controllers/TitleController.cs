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
    [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.DataBridgeAsia })]
    public class TitleController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Title
        public ActionResult Index()
        {
            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                //var titlelist = db.Title.Where(o => o.Active == true).ToList();

                var query = from title in db.Title.Where(o => o.Active == true)
                                    join type in db.TitleType on title.TitleTypeID equals type.TitleTypeID
                                    into holder
                                    from hold in holder.DefaultIfEmpty()
                                    select new { title, hold.TitleTypeName };

                var titlelist = query.Select(
                    o => new TitleModel()
                    {
                        TitleID = o.title.TitleID,
                        TitleAbbreviation = o.title.TitleAbbreviation,
                        TitleName = o.title.TitleName,
                        TitleTypeID = o.title.TitleTypeID,
                        TitleTypeName = o.TitleTypeName,
                        CreatedBy = o.title.CreatedBy,
                        CreatedDate = o.title.CreatedDate,
                        Active = o.title.Active
                    }).ToList(); ;

                return View(titlelist);
            }
        }
        [HttpGet]
        public ActionResult Title(int? id)
        {

            TitleModel Title = new TitleModel();

            using (db = new VRSystemEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;


                Title.TitleTypeList = db.TitleType.ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var Load = db.Title.Where(o => o.Active == true && o.TitleID == id).ToList().FirstOrDefault();

                    Title.TitleID = Load.TitleID;
                    Title.TitleAbbreviation = Load.TitleAbbreviation;
                    Title.TitleName = Load.TitleName;
                    Title.TitleTypeID = Load.TitleTypeID;
                }
                return PartialView(Title);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Title(TitleModel Title, string submit)
        {
            if (ModelState.IsValid)
            {
                switch (submit)
                {
                    case "Create":
                        using (db = new VRSystemEntities())
                        {
                            var NewTitle = new Title
                            {
                                TitleID = Title.TitleID,
                                TitleAbbreviation = Title.TitleAbbreviation.Trim(),
                                TitleName = Title.TitleName.Trim(),
                                TitleTypeID = Title.TitleTypeID,
                                CreatedBy = CurrentUser.Details.UserID,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            db.Title.Add(NewTitle);
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Created Successfully!";
                        }
                        break;
                    case "Save":
                        using (db = new VRSystemEntities())
                        {
                            var Update = db.Title.Where(o => o.TitleID == Title.TitleID).FirstOrDefault();
                            Update.TitleID = Title.TitleID;
                            Update.TitleAbbreviation = Title.TitleAbbreviation.Trim();
                            Update.TitleName = Title.TitleName.Trim();
                            Update.TitleTypeID = Title.TitleTypeID;
                            Update.UpdatedBy = CurrentUser.Details.UserID;
                            Update.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Updated Sucessfully!";
                        }
                        break;
                    case "Delete":
                        using (db = new VRSystemEntities())
                        {
                            var Update_active = db.Title.Where(o => o.TitleID == Title.TitleID).FirstOrDefault();
                            Update_active.Active = false;
                            Update_active.UpdatedBy = CurrentUser.Details.UserID;
                            Update_active.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                            TempData["WarningMessage"] = "Removed Sucessfully!";
                        }
                        break;
                }
                return RedirectToAction("Index");
            }
            else
            {
                Title.TitleTypeList = db.TitleType.ToList();
                TempData["ErrorMessage"] = "An error has occured.";

                if (submit == "Create")
                    ViewBag.Edit = false;
                return View(Title);
            }

        }
    }
}