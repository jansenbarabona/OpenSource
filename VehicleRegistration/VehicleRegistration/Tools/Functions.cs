using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using VehicleRegistration.Models;
using ZohoMail;

namespace VehicleRegistration.Tools
{
    public class Functions
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool IsMainBranch(UserEntityEnum EntityType, int BranchID)
        {
            bool IsMain = false;
            using (var db = new VRSystemEntities())
            {
                switch (EntityType)
                {
                    case UserEntityEnum.Dealer:
                        IsMain = db.DealerBranch.Where(o => o.DealerBranchID == BranchID).FirstOrDefault().IsMain;
                        break;
                    case UserEntityEnum.Insurance:
                        IsMain = db.InsuranceBranch.Where(o => o.InsuranceBranchID == BranchID).FirstOrDefault().IsMain;
                        break;
                }
            }
            return IsMain;
        }

        public static IsExistEngineORChassis IsExistedEngineORChassis(string engine, string chassis)
        {
            using (var db = new VRSystemEntities())
            {
                IsExistEngineORChassis IsExistDetails = new IsExistEngineORChassis();
                if (db.VehicleInfo.Where(o => o.EngineNumber == engine).Count() > 0)
                {
                    IsExistDetails.EngineNumber = engine;
                }
                if (db.VehicleInfo.Where(o => o.ChassisNumber == chassis).Count() > 0)
                {
                    IsExistDetails.ChassisNumber = chassis;
                }

                return IsExistDetails;
            }
        }

        public static WalletModel GetWalletDetails(int UserEntityID, int EntityID)
        {
            using (var db = new VRSystemEntities())
            {
                WalletModel WalletDetail = new WalletModel();

                WalletDetail.TransactionList = db.spEntityWallet(UserEntityID, EntityID).OrderByDescending(o => o.CreatedDate).ToList();

                foreach (var item in WalletDetail.TransactionList)
                {
                    WalletDetail.AvailableBalance += (item.Amount * item.TransactionEntryMultiplier);
                }

                WalletDetail.Threshold = Math.Round(db.Wallet.Where(o => o.UserEntityID == UserEntityID && o.EntityID == EntityID).FirstOrDefault().Threshold, 2, MidpointRounding.AwayFromZero);
                WalletDetail.ThresholdInput = WalletDetail.Threshold;

                return WalletDetail;
            }
        }

        public static bool IsThreshold(int UserEntityID, int EntityID)
        {
            using (var db = new VRSystemEntities())
            {
                WalletModel WalletDetail = new WalletModel();

                WalletDetail.TransactionList = db.spEntityWallet(UserEntityID, EntityID).OrderByDescending(o => o.CreatedDate).ToList();

                foreach (var item in WalletDetail.TransactionList)
                {
                    WalletDetail.AvailableBalance += (item.Amount * item.TransactionEntryMultiplier);
                }

                WalletDetail.Threshold = Math.Round(db.Wallet.Where(o => o.UserEntityID == UserEntityID && o.EntityID == EntityID).FirstOrDefault().Threshold, 2, MidpointRounding.AwayFromZero);

                if (WalletDetail.AvailableBalance <= WalletDetail.Threshold)
                {
                 
                    try
                    {
                        List<string> Recipients = new List<string>();
                        Recipients = db.User.Where(o => o.UserEntityID == UserEntityID && o.ReferenceID == EntityID && o.Active == true && o.UserRoleID == (int)UserRoleEnum.Administrator).Select(o => o.EmailAddress).ToList();

                        string EmailBody;
                        using (StreamReader srBody = new StreamReader(HttpContext.Current.Server.MapPath(string.Format("~/MailBody/WalletThreshold.html"))))
                        {
                            EmailBody = srBody.ReadToEnd();
                        }

                        EmailBody = EmailBody.Replace("@AvailableBalance", WalletDetail.AvailableBalance.ToString());

                        ZohoMailSender.SendEmail("Plate Section Report", EmailBody, Recipients);

                    }
                    catch (Exception ex)
                    {
                        string filePath = HttpContext.Current.Server.MapPath(string.Format("~/Reports/VRTempFiles/Error.txt"));

                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine("-----------------------------------------------------------------------------");
                            writer.WriteLine("Date : " + DateTime.Now.ToString());
                            writer.WriteLine();

                            while (ex != null)
                            {
                                writer.WriteLine(ex.GetType().FullName);
                                writer.WriteLine("Message : " + ex.Message);
                                writer.WriteLine("StackTrace : " + ex.StackTrace);

                                ex = ex.InnerException;
                            }
                        }
                    }

                    return true;
                }
                else
                    return false;
               
            }
        }

        public static int InsertTransaction(TransactionModel Transaction)
        {
            try
            {
                using (var db = new VRSystemEntities())
                {
                    var InsertTransaction = new Transaction
                    {
                        WalletID = db.Wallet.Where(o => o.UserEntityID == Transaction.SelectedUserEntityID && o.EntityID == Transaction.SelectedEntityID).FirstOrDefault().WalletID,
                        TransactionTypeID = Transaction.TransactionTypeID,
                        Amount = Transaction.Amount,
                        Remarks = Transaction.Remarks?.Trim(),
                        VehicleID = Transaction.VehicleID,
                        CreatedBy = CurrentUser.Details.UserID,
                        CreatedDate = DateTime.Now
                    };

                    db.Transaction.Add(InsertTransaction);
                    db.SaveChanges();
                    return InsertTransaction.TransactionID;
                }


            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static bool Logo(string submit, string OldLogo, HttpPostedFileBase NewLogoFile)
        {
            try
            {

                var fullPath_Delete = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Logos"), OldLogo);
                var fullPath_Save = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Logos"), NewLogoFile.FileName);
                switch (submit)
                {
                    case "Create":
                        if (NewLogoFile != null)
                        {
                            NewLogoFile.SaveAs(fullPath_Save);
                        }
                        break;
                    case "Save":
                        if (NewLogoFile != null)
                        {
                            System.IO.File.Delete(fullPath_Delete);
                            NewLogoFile.SaveAs(fullPath_Save);
                        }
                        break;
                    case "Delete":
                        System.IO.File.Delete(fullPath_Delete);
                        break;
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }


        public static ReceiptResult eReceiptConnect(Receipt rcpt)
        {
            ReceiptResult RcptRslt = new ReceiptResult();
            using (var client = new HttpClient())
            {
                var formDictionary = new Dictionary<string, string>{
                    {"merchant_code", rcpt.merchant_code },
                    {"trans_id", rcpt.trans_id },
                    {"account_no", rcpt.account_no },
                    {"payor_name", rcpt.payor_name },
                    {"address", rcpt.address },
                    {"TIN", rcpt.TIN },
                    {"transaction_type", rcpt.transaction_type },
                    {"payment_option", rcpt.payment_option },
                    {"email", rcpt.email },
                    {"amount", rcpt.amount },
                    {"special_discount", rcpt.special_discount },
                    {"discount_id", rcpt.discount_id },
                    {"particulars", rcpt.particulars }
                };

                //client.BaseAddress = new Uri("http://3.12.130.204/eorapi-databridgeasia/app/Controller/");
                var content = new FormUrlEncodedContent(formDictionary);
                var postjob = client.PostAsync("http://3.12.130.204/eorapi-databridgeasia/app/Controller/receive.php", content);
                postjob.Wait();

                var postresult = postjob.Result;
                if (postresult.IsSuccessStatusCode)
                {
                    var readjob = postresult.Content.ReadAsStringAsync();
                    readjob.Wait();
                    RcptRslt = JsonConvert.DeserializeObject<ReceiptResult>(readjob.Result);

                    return RcptRslt;
                }
                else
                {
                    var readjob = postresult.Content.ReadAsStringAsync();
                    readjob.Wait();
                    RcptRslt = JsonConvert.DeserializeObject<ReceiptResult>(readjob.Result);

                    return RcptRslt;
                }
            }
        }

        public static void EmailPlateLTO(BatchMaster model)
        {
            try
            {
                //Email
                if (ConfigurationManager.AppSettings["LTOEmailRecipient"].ToString() != "")
                {
                    using (var db = new VRSystemEntities())
                    {
                        //Save File to a Folder
                        List<string> Recipients = new List<string>();
                        Recipients.Add(ConfigurationManager.AppSettings["LTOEmailRecipient"].ToString());

                        string EmailBody;
                        using (StreamReader srBody = new StreamReader(HttpContext.Current.Server.MapPath(string.Format("~/MailBody/LTOPlateMailBody.html"))))
                        {
                            EmailBody = srBody.ReadToEnd();
                        }


                        List<string> Attachments = new List<string>();


                        Attachments.Add(PlateSectionReport(model));

                        ZohoMailSender.SendEmail("Plate Section Report", EmailBody, Recipients, Attachments);

                        foreach (var file in Attachments)
                        {
                            if (System.IO.File.Exists(file))
                                System.IO.File.Delete(file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string filePath = HttpContext.Current.Server.MapPath(string.Format("~/Reports/VRTempFiles/Error.txt"));

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }
            }
        }
        public static string PlateSectionReport(BatchMaster model)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Reports/RDLC"), "PlateSectionReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            //else
            //{
            //    return RedirectToAction("Index");
            //}

            DataTable dt = new DataTable("Details");
            dt.Columns.Add(new DataColumn("PlateAssignment", typeof(string)));
            dt.Columns.Add(new DataColumn("DateSalesInvoice", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("CustomerName", typeof(string)));
            dt.Columns.Add(new DataColumn("Engine", typeof(string)));
            dt.Columns.Add(new DataColumn("Chassis", typeof(string)));
            dt.Columns.Add(new DataColumn("Amount", typeof(float)));
            dt.Columns.Add(new DataColumn("LBPRefNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("TransactisonNumber", typeof(string)));

            BatchMaster Header = new BatchMaster();
            var DealerInfo = new DealerModel();
            float TotalAmountCost = 0;
            var TotalVehicle = 0;
            using (var db = new VRSystemEntities())
            {
                Header = model;
                DealerInfo = db.Dealer.Where(o => o.DealerID == Header.UserReference)
                            .Select(o => new DealerModel
                            {
                                DealerName = o.DealerName,
                                BusinessPhone = o.BusinessPhone
                            })
                            .FirstOrDefault();
                //var Details = db.BatchDetails.Where(o => o.BatchID == Header.BatchID).ToList();

                var Details = (from a in db.BatchDetails
                               where a.BatchID == Header.BatchID
                               join b in db.vwVehicleList on a.VehicleID equals b.VehicleID
                               join c in db.DealerInvoice on b.VehicleID equals c.VehicleID
                               join d in db.vwCustomerList on c.CustomerID equals d.CustomerID into temp
                               from temptbl in temp.DefaultIfEmpty()
                               select new
                               {
                                   VehicleID = b.VehicleID,
                                   PlateAssign = c.PreferredEndingPlateNumber,
                                   DateSalesInvoice = c.InvoiceDate,
                                   CustomerInfo = temptbl,
                                   Engine = b.EngineNumber,
                                   Chassis = b.ChassisNumber,
                                   Amount = c.VehicleCost,
                                   LBFRefNumber = "",
                                   TransactionNumber = ""
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
                        item.PlateAssign,
                        item.DateSalesInvoice,
                        CustomerName,
                        item.Engine,
                        item.Chassis,
                        item.Amount,
                        item.LBFRefNumber,
                        item.TransactionNumber
                        );
                    TotalAmountCost = (float)TotalAmountCost + (float)item.Amount;
                    TotalVehicle++;
                }
            }

            //string imagePath = new Uri(Server.MapPath("~/Logos/" + MAIInfo.Logo)).AbsoluteUri;
            //lr.EnableExternalImages = true;
            //lr.EnableHyperlinks = true;

            ReportParameter[] prm = new ReportParameter[5];
            prm[0] = new ReportParameter("DealerNameParameter", DealerInfo.DealerName);
            prm[1] = new ReportParameter("ContactNumberParameter", DealerInfo.BusinessPhone);
            prm[2] = new ReportParameter("NumberOfUnitsParameter", TotalVehicle.ToString());
            prm[3] = new ReportParameter("TransmittalNumberParameter", "Test");
            prm[4] = new ReportParameter("TotalParameter", TotalAmountCost.ToString());
            lr.SetParameters(prm);

            ReportDataSource rds = new ReportDataSource("PlateSectionDetails", dt);
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

            var pdfPath = HttpContext.Current.Server.MapPath(string.Format("~/Reports/VRTempFiles/")) + DealerInfo.DealerName.Trim() + " - " + Header.ReferenceNo.Trim() + ".pdf";

            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                fs.Write(renderedBytes, 0, renderedBytes.Length);
            }

            return pdfPath;
        }
        public static void LTOEmailStatus(int BatchID, LTOStatus LTOStatus)
        {
            try
            {
                using (var db = new VRSystemEntities())
                {
                    //Get Batch Info
                    var batchInfo = db.BatchMaster.Where(o => o.BatchID == BatchID).FirstOrDefault();

                    List<string> Recipients = new List<string>();
                    string Header = "Batch " + batchInfo.ReferenceNo;
                    string EmailBody = "";
                    string EmailStatusWording = "";

                    var request = HttpContext.Current.Request;
                    var appUrl = HttpRuntime.AppDomainAppVirtualPath;

                    if (appUrl != "/")
                        appUrl += "/";

                    //string ServerAddress = @"https://www.databridgeasia.com/";
                    string ServerAddress = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

                    switch (LTOStatus)
                    {
                        case LTOStatus.Submitted:
                            {
                                //set header
                                EmailStatusWording = " has been submitted for registration.";
                                Header += EmailStatusWording;

                                //get batch owner email
                                Recipients = System.Configuration.ConfigurationManager.AppSettings["LTOStatusEmailRecipient"].ToString().Split(',').ToList();


                                using (StreamReader srBody = new StreamReader(HttpContext.Current.Server.MapPath(string.Format("~/MailBody/LTOStatus.html"))))
                                {
                                    EmailBody = srBody.ReadToEnd().Replace("@link", ServerAddress + "LTO/SubmittedBatch")
                                                                  .Replace("@batchReference", batchInfo.ReferenceNo)
                                                                  .Replace("@status", EmailStatusWording);
                                }
                                break;
                            }

                        case LTOStatus.Assessed:
                            {
                                //set header
                                EmailStatusWording = " is now ready for payment.";
                                Header += EmailStatusWording;

                                //get batch owner email
                                Recipients.Add(db.User.Where(o => o.UserID == batchInfo.CreatedBy).FirstOrDefault().EmailAddress);


                                using (StreamReader srBody = new StreamReader(HttpContext.Current.Server.MapPath(string.Format("~/MailBody/LTOStatus.html"))))
                                {
                                    EmailBody = srBody.ReadToEnd().Replace("@link", ServerAddress + "VehicleInfo/ForLTO_Payment")
                                                                  .Replace("@batchReference", batchInfo.ReferenceNo)
                                                                  .Replace("@status", EmailStatusWording);
                                }

                                break;
                            }
                        case LTOStatus.Paid:
                            {
                                //set header
                                EmailStatusWording = " has been paid!";
                                Header += EmailStatusWording;

                                //get batch owner email
                                Recipients = System.Configuration.ConfigurationManager.AppSettings["LTOStatusEmailRecipient"].ToString().Split(',').ToList();


                                using (StreamReader srBody = new StreamReader(HttpContext.Current.Server.MapPath(string.Format("~/MailBody/LTOStatus.html"))))
                                {
                                    EmailBody = srBody.ReadToEnd().Replace("@link", ServerAddress + "LTO/PaymentBatch")
                                                                  .Replace("@batchReference", batchInfo.ReferenceNo)
                                                                  .Replace("@status", EmailStatusWording);
                                }
                                break;
                            }
                        case LTOStatus.ForPickUp:
                            {
                                //set header
                                EmailStatusWording = " is now ready for Pick Up";
                                Header += EmailStatusWording;

                                //get batch owner email
                                Recipients.Add(db.User.Where(o => o.UserID == batchInfo.CreatedBy).FirstOrDefault().EmailAddress);

                                using (StreamReader srBody = new StreamReader(HttpContext.Current.Server.MapPath(string.Format("~/MailBody/LTOStatus.html"))))
                                {
                                    EmailBody = srBody.ReadToEnd().Replace("@link", ServerAddress + "VehicleInfo/ForLTO_ForPickUp")
                                                                  .Replace("@batchReference", batchInfo.ReferenceNo)
                                                                  .Replace("@status", EmailStatusWording); ;
                                }

                                break;
                            }
                    }

                    if (Recipients.Count > 0)
                        ZohoMailSender.SendEmail(Header, EmailBody, Recipients);
                }
            }
            catch (Exception ex)
            {
                string filePath = HttpContext.Current.Server.MapPath(string.Format("~/Reports/VRTempFiles/Error.txt"));
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }
            }

        }
    }
}