using System;
using System.Net;
using System.Net.Mail;
namespace BusinessLayer.Helpers
{
    public static class EmailHelper
    {
        public static bool SendEmail(string toEmail,string subject, string body, bool isHtml)
        {
            try
            {
                var fromAddress = new MailAddress(ConfigHelper.GetAppSettings("FromEmail"));
                var toAddress = new MailAddress(toEmail);
                string fromPassword = ConfigHelper.GetAppSettings("SmtpPassword");

                var smtp = new SmtpClient
                {
                    Host = ConfigHelper.GetAppSettings("SmtpHost"),
                    Port = Convert.ToInt32(ConfigHelper.GetAppSettings("SmtpPort")),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}