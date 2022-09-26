using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace VehicleRegistration.Tools
{
    public class ZohoEmail
    {
        public static bool SendEmail(string Subject, string Body, List<string> Recipient)
        {
            try
            {
                SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["ZohoEmailClient"].ToString());
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ZohoEmailUserName"].ToString(), ConfigurationManager.AppSettings["ZohoEmailPassword"].ToString());
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["ZohoEmailPort"].ToString());
                client.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("no-reply@databridgeasia.com");
                foreach(var emailaddress in Recipient)
                {
                    mailMessage.To.Add(emailaddress);
                }
                mailMessage.Subject = Subject;
                mailMessage.Body = Body;
                mailMessage.IsBodyHtml = true;

                client.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}