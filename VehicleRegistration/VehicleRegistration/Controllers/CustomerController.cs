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
    public class CustomerController : Controller
    {
        VRSystemEntities db = new VRSystemEntities();
        // GET: Customer
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult Index()
        {
            var CustomerList = new List<vwCustomerList>();
            try
            {
               
                if (CurrentUser.Details.UserEntityID == (int)UserEntityEnum.Dealer)
                    CustomerList = db.vwCustomerList.Where(o => o.DealerID == CurrentUser.Details.ReferenceID && o.Active == true).ToList();
               
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error has occured.";
            }
            return View(CustomerList);

        }

        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult Customer(int? id)
        {
            ViewBag.id = id;

            CustomerModel Customer = new CustomerModel();

            using (db = new VRSystemEntities())
            {
               
                Customer.SexList = db.Sex.ToList();
                Customer.CivilStatusList = db.CivilStatus.ToList();
                Customer.CityList = new List<City>();
                Customer.BarangayList = new List<Barangay>();
                Customer.ProvinceList = db.Province.OrderBy(o => o.ProvinceName).ToList();
                //Customer.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                Customer.TitleList = db.Title.ToList();

                if (id == null)
                {
                    ViewBag.Edit = false;
                }
                else
                {
                    ViewBag.Edit = true;

                    var CustomerLoad = db.Customer.Where(o => o.CustomerID == id).ToList().FirstOrDefault();
                    var ProvinceID = db.City.Where(o => o.CityID == CustomerLoad.CityID).FirstOrDefault().ProvinceID;
                    Customer.CityList = db.City.Where(o => o.ProvinceID == ProvinceID).ToList();
                    Customer.BarangayList = db.Barangay.Where(o => o.CityID == CustomerLoad.CityID).ToList();

                    Customer.ClientReferenceNumber = CustomerLoad.ClientReferenceNumber;
                    Customer.CustomerID = CustomerLoad.CustomerID;
                    Customer.SelectedTitleID = CustomerLoad.TitleID;
                    Customer.DealerID = CustomerLoad.DealerID;
                    Customer.LastName = CustomerLoad.LastName;
                    Customer.FirstName = CustomerLoad.FirstName;
                    Customer.MiddleName = CustomerLoad.MiddleName;
                    Customer.CorpName = CustomerLoad.CorpName;
                    Customer.FathersName = CustomerLoad.FathersName;
                    Customer.MothersName = CustomerLoad.MothersName;
                    Customer.SelectedSexCode = CustomerLoad.SexCode;
                    Customer.SelectedCivilStatusCode = CustomerLoad.CivilStatusCode;
                    Customer.Citizenship = CustomerLoad.Citizenship;
                    Customer.HouseBldgNumber = CustomerLoad.HouseBldgNumber;
                    Customer.StreetSubdivision = CustomerLoad.StreetSubdivision;
                    Customer.Barangay = CustomerLoad.Barangay;
                    Customer.SelectedCityID = CustomerLoad.CityID;
                    Customer.SelectedProvinceID = ProvinceID;
                    Customer.ZipCode = CustomerLoad.ZipCode;
                    Customer.Height = CustomerLoad.Height;
                    Customer.Weight = CustomerLoad.Weight;
                    Customer.Birthdate = CustomerLoad.Birthdate;
                    Customer.Birthplace = CustomerLoad.Birthplace;
                    Customer.ContactNumber = CustomerLoad.ContactNumber;
                    Customer.EmailAddress = CustomerLoad.EmailAddress;
                    Customer.TIN = CustomerLoad.TIN;
                    Customer.AdditionalAddress = CustomerLoad.AdditionalAddress;
                    Customer.OrganizationName = CustomerLoad.OrganizationName;
                    Customer.OrganizationTIN = CustomerLoad.OrganizationTIN;
                    Customer.OrganizationAddress = CustomerLoad.OrganizationAddress;
                    Customer.ContactPerson = CustomerLoad.ContactPerson;
                    Customer.ContactPersonNumber = CustomerLoad.ContactPersonNumber;
                    Customer.Alias = CustomerLoad.Alias;

                    ViewBag.TitleTypeID = db.Title.Where(o => o.TitleID == Customer.SelectedTitleID).FirstOrDefault().TitleTypeID.ToString();
                }

                return View(Customer);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedUserEntity = new[] { UserEntityEnum.Dealer })]
        public ActionResult Customer(CustomerModel Customer, string submit)
        {
            var tittletype = db.Title.Where(o => o.TitleID == Customer.SelectedTitleID).FirstOrDefault();
            if (tittletype.TitleTypeID == (int)TitleTypeEnum.Individual)
            {
                ModelState.Remove("CorpName");
                ModelState.Remove("Alias");
                ModelState.Remove("OrganizationName");
                ModelState.Remove("OrganizationAddress");
                ModelState.Remove("OrganizationTIN");
                ModelState.Remove("ContactPerson");
                ModelState.Remove("ContactPersonNumber");
            }
            else if (tittletype.TitleTypeID == (int)TitleTypeEnum.Corporation)
            {
                ModelState.Remove("LastName");
                ModelState.Remove("FirstName");
                ModelState.Remove("MiddleName");
                ModelState.Remove("Birthdate");
                ModelState.Remove("Birthplace");
                ModelState.Remove("FathersName");
                ModelState.Remove("MothersName");
                ModelState.Remove("SelectedSexCode");
                ModelState.Remove("SelectedCivilStatusCode"); 
                ModelState.Remove("Citizenship");
                ModelState.Remove("Height");
                ModelState.Remove("Weight");

                ModelState.Remove("OrganizationName");
                ModelState.Remove("OrganizationAddress");
                ModelState.Remove("OrganizationTIN");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    switch (submit)
                    {
                        case "Create":
                            using (db = new VRSystemEntities())
                            {

                                var NewCustomer = new Customer
                                {
                                    ClientReferenceNumber = Customer.ClientReferenceNumber?.Trim(),
                                    DealerID = Convert.ToInt32(CurrentUser.Details.ReferenceID),
                                    TitleID = Customer.SelectedTitleID,
                                    //
                                    LastName = Customer.LastName?.Trim(),
                                    FirstName = Customer.FirstName?.Trim(),
                                    MiddleName = Customer.MiddleName?.Trim(),

                                    Birthdate = Customer.Birthdate ?? DateTime.Now,
                                    Birthplace = Customer.Birthplace?.Trim(),
                                    FathersName = Customer.FathersName?.Trim(),
                                    MothersName = Customer.MothersName?.Trim(),
                                    SexCode = Customer.SelectedSexCode,
                                    CivilStatusCode = Customer.SelectedCivilStatusCode,
                                    Citizenship = Customer.Citizenship?.Trim(),
                                    Height = Customer.Height?.Trim(),
                                    Weight = Customer.Weight?.Trim(),
                                    //
                                    CorpName = Customer.CorpName?.Trim(),
                                    Alias = Customer.Alias?.Trim(),

                                    ContactPerson = Customer.ContactPerson?.Trim(),
                                    ContactPersonNumber = Customer.ContactPersonNumber?.Trim(),

                                    OrganizationName = Customer.CorpName?.Trim(),
                                    OrganizationAddress = Customer.AdditionalAddress?.Trim(),
                                    OrganizationTIN = Customer.TIN?.Trim(),
                                    //
                                    ContactNumber = Customer.ContactNumber?.Trim(),
                                    EmailAddress = Customer.EmailAddress?.Trim(),
                                    TIN = Customer.TIN?.Trim(),
                                    HouseBldgNumber = Customer.HouseBldgNumber?.Trim(),
                                    StreetSubdivision = Customer.StreetSubdivision?.Trim(),
                                    Barangay = Customer.Barangay?.Trim(),
                                    CityID = Customer.SelectedCityID,
                                    ZipCode = Customer.ZipCode?.Trim(),
                                    AdditionalAddress = Customer.AdditionalAddress?.Trim(),
                                    //
                                    Active = true,
                                    CreatedBy = CurrentUser.Details.UserID,
                                    CreatedDate = DateTime.Now,
                                };
                                db.Customer.Add(NewCustomer);
                                db.SaveChanges();

                            }
                            TempData["SuccessMessage"] = "Created Successfully!";
                            break;

                        case "Save":
                            using (db = new VRSystemEntities())
                            {
                                var UpdateCustomer = db.Customer.Where(o => o.CustomerID == Customer.CustomerID).FirstOrDefault();
                                UpdateCustomer.ClientReferenceNumber = Customer.ClientReferenceNumber?.Trim();
                                UpdateCustomer.TitleID = Customer.SelectedTitleID;
                                UpdateCustomer.LastName = Customer.LastName?.Trim();
                                UpdateCustomer.FirstName = Customer.FirstName?.Trim();
                                UpdateCustomer.MiddleName = Customer.MiddleName?.Trim();
                                UpdateCustomer.CorpName = Customer.CorpName?.Trim();
                                UpdateCustomer.FathersName = Customer.FathersName?.Trim();
                                UpdateCustomer.MothersName = Customer.MothersName?.Trim();
                                UpdateCustomer.SexCode = Customer.SelectedSexCode;
                                UpdateCustomer.CivilStatusCode = Customer.SelectedCivilStatusCode;
                                UpdateCustomer.Citizenship = Customer.Citizenship?.Trim();
                                UpdateCustomer.HouseBldgNumber = Customer.HouseBldgNumber?.Trim();
                                UpdateCustomer.StreetSubdivision = Customer.StreetSubdivision?.Trim();
                                UpdateCustomer.Barangay = Customer.Barangay?.Trim();
                                UpdateCustomer.CityID = Customer.SelectedCityID;
                                UpdateCustomer.ZipCode = Customer.ZipCode?.Trim();
                                UpdateCustomer.Height = Customer.Height?.Trim();
                                UpdateCustomer.Weight = Customer.Weight?.Trim();
                                UpdateCustomer.Birthdate = Customer.Birthdate??DateTime.Now;
                                UpdateCustomer.Birthplace = Customer.Birthplace?.Trim();
                                UpdateCustomer.ContactNumber = Customer.ContactNumber?.Trim();
                                UpdateCustomer.EmailAddress = Customer.EmailAddress?.Trim();
                                UpdateCustomer.TIN = Customer.TIN?.Trim();
                                UpdateCustomer.AdditionalAddress = Customer.AdditionalAddress?.Trim();
                                UpdateCustomer.OrganizationName = Customer.OrganizationName?.Trim();
                                UpdateCustomer.OrganizationAddress = Customer.OrganizationAddress?.Trim();
                                UpdateCustomer.OrganizationTIN = Customer.TIN?.Trim();
                                UpdateCustomer.ContactPerson = Customer.ContactPerson?.Trim();
                                UpdateCustomer.ContactPersonNumber = Customer.ContactPersonNumber?.Trim();
                                UpdateCustomer.UpdatedBy = CurrentUser.Details.UserID;
                                UpdateCustomer.UpdatedDate = DateTime.Now;
                                UpdateCustomer.Alias = Customer.Alias;
                                db.SaveChanges();
                            }
                            TempData["SuccessMessage"] = "Updated Successfully!";
                            break;
                        case "Delete":
                            using (db = new VRSystemEntities())
                            {
                                var DeleteCustomer = db.Customer.Where(o => o.CustomerID == Customer.CustomerID).FirstOrDefault();
                                DeleteCustomer.Active = false;
                                DeleteCustomer.UpdatedBy = CurrentUser.Details.UserID;
                                DeleteCustomer.UpdatedDate = DateTime.Now;
                                db.SaveChanges();
                            }
                            TempData["WarningMessage"] = "Removed Successfully!";
                            break;
                    }
                    return RedirectToAction("Index");
                }
                else if (ModelState["Birthdate"].Errors.Count == 1)
                {
                    TempData["ErrorMessage"] = "The Birthdate field must be in date range.";
                }
                else
                {
                    if (submit == "Create")
                        ViewBag.Edit = false;
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error has occured.";
            }

            using (db = new VRSystemEntities())
            {
                Customer.SexList = db.Sex.ToList();
                Customer.ProvinceList = db.Province.Where(o => o.Active == true).ToList();
                Customer.CityList = db.City.Where(o => o.Active == true && o.ProvinceID == Customer.SelectedProvinceID).ToList();
                Customer.BarangayList = db.Barangay.Where(o => o.Active == true && o.CityID == Customer.SelectedCityID).ToList();
                Customer.TitleList = db.Title.ToList();
                Customer.CivilStatusList = db.CivilStatus.ToList();
            }
            ViewBag.TitleTypeID = tittletype.TitleTypeID;
            return View(Customer);
        }
    }
}