using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.IO;
using System.Configuration;
using System.Net;
using log4net;

namespace JobsPortal.Helper
{
    // The MailHelper class is defined as a static class, meaning it can't be instantiated,
    // and its methods can be called without creating an instance of the class.
    public static class MailHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MailHelper));

        // SendJobApplicationEmail is a public static method that takes several parameters
        // The method composes an email message with HTML body content, including the applicant's name,
        // and calls the SendEmailWithAttachment method to send this email.
        public static bool SendJobApplicationEmail(string toEmail, string employeeName, string resumePath, string details)
        {
            bool IsBodyHtml = true;
            string subject = $"Application from {employeeName}";

            string body = $@"
                <h2>Job Application</h2>
                <p>The Candidate <strong>{employeeName}</strong> wishes to apply for your job.</p>
                <p>These are his details:</p>
                {details}
            ";

            bool emailStatus = SendEmailWithAttachment(toEmail, subject, body, IsBodyHtml, resumePath);
            return emailStatus;
        }

        private static bool SendEmailWithAttachment(string toEmail, string subject, string body, bool IsBodyHtml, string attachmentPath)
        {
            bool status = false;
            try
            {
                // Configuration settings from the Web.config file
                string HostAddress = ConfigurationManager.AppSettings["Host"].ToString();
                string FormEmailId = ConfigurationManager.AppSettings["MailFrom"].ToString();
                string Password = ConfigurationManager.AppSettings["Password"].ToString();
                string Port = ConfigurationManager.AppSettings["Port"].ToString();

                // A MailMessage object is created to represent the email message.
                // It includes sender information, subject, body, and recipient.
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(FormEmailId);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = IsBodyHtml;
                mailMessage.To.Add(new MailAddress(toEmail));

                // If attachmentPath is not empty and a file exists at that path, an email attachment is added.
                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    mailMessage.Attachments.Add(new Attachment(attachmentPath));
                }

                //SMTP server configuration settings are read from the application's configuration file
                //using ConfigurationManager.AppSettings
                SmtpClient smtp = new SmtpClient();
                smtp.Host = HostAddress;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Set network credentials
                // NetworkCredential is used to set the network credentials for the SMTP server,
                // including the sender's email address and password.
                NetworkCredential networkCredential = new NetworkCredential();
                networkCredential.UserName = mailMessage.From.Address;
                networkCredential.Password = Password;

                // It configures the SMTP server settings, including host, credentials, port, and SSL.
                smtp.Credentials = networkCredential;
                smtp.Port = Convert.ToInt32(Port);
                smtp.EnableSsl = true;

                // Send the email
                // The Send method is called on the SmtpClient object to send the email.
                smtp.Send(mailMessage);
                status = true;
            }
            catch (Exception e)
            {
                log.Error($"Failed to send email to {toEmail}. Exception: {e}");
                return status;
            }
            return status;
        }
    }
}
