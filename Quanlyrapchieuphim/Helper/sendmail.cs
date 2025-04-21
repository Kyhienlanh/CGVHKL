using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace Quanlyrapchieuphim.Helper
{
    public class sendmail
    {
        public class SendMail
        {
            public static bool SendMail123(string to, string subject, string body, string attachFile)
            {
                try
                {
                    // Your email sending logic goes here
                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Credentials = new NetworkCredential("kyhienlanh1", "lvpc jojh rkyw ttra"),
                        EnableSsl = true,
                        Port = 587 // Use the appropriate port for your SMTP server
                    };

                    var mailMessage = new MailMessage("kyhienlanh1@gmail.com", to, subject, body);

                    // Attach file if provided
                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        var attachment = new Attachment(attachFile);
                        mailMessage.Attachments.Add(attachment);
                    }

                    smtpClient.Send(mailMessage);

                    // Return true if email sent successfully
                    return true;
                }
                catch
                {
                    // Return false if an exception occurs during email sending
                    return false;
                }
            }
        }
    }
}