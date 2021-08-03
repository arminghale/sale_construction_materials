using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;

namespace DataBaseBackend
{
    public static class SendMail
    {
        public static void Send(string To, string Subject, string Body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("armin.m.ghaleh@gmail.com", "سامانه فروش مصالح");
            //mail.From = new MailAddress("info@caspianglobaltraders.com", "Caspian Global Traders");
            mail.To.Add(To);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;


            SmtpServer.Port = 587;
            //SmtpServer.Port = 143;
            //SmtpServer.Port = 110;
            //SmtpServer.Port = 3698;
            //SmtpServer.Port = 443;
            SmtpServer.UseDefaultCredentials = false;

            SmtpServer.Credentials = new System.Net.NetworkCredential("armin.m.ghaleh@gmail.com", "benyamin");
            
            //SmtpServer.Credentials = new System.Net.NetworkCredential("info@caspianglobaltraders.com", "!Jid57k2");
            //SmtpServer.EnableSsl = false;
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
            SmtpServer.Dispose();
            mail.Dispose();

        }
    }
}
