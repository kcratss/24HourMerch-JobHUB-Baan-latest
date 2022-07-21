using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Net.Mail;
using KEN_DataAccess;
using KEN.Controllers;
using KEN.Interfaces.Iservices;
using System.Web.Mvc;
using KEN.Filters;
using System.Web;
using Intuit.Ipp.Core.Configuration;

namespace KEN.AppCode
{

    public class DataBaseCon
    {

        public static string FromEmailID = ConfigurationManager.AppSettings["FromEmailID"].ToString();
        public static string FromEmailName = ConfigurationManager.AppSettings["FromEmailName"].ToString();
        //public static string SenderID = ConfigurationManager.AppSettings["SenderID"].ToString();
        //public static string SenderPassword = ConfigurationManager.AppSettings["SenderPassword"].ToString();
        //public static string BCCMailID = ConfigurationManager.AppSettings["BCCMailID"].ToString();
        public static string CCMailID = ConfigurationManager.AppSettings["CCMailID"].ToString();
        public static string PurchaseFromMail = ConfigurationManager.AppSettings["PurchaseMail"].ToString();

        // baans change 21st November
        //static SqlConnection con = null;
        // baans end 21st November



        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "JAY2VEER4PBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                       //private readonly ILoginService _baseService;
     
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            try
            {
                string EncryptionKey = "JAY2VEER4PBNI99212";
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {

                return "0";
            }
        }
        //public static DateTime ToTimeZoneTime(DateTime time, TimeZoneInfo tzi)
        //{
        //    return TimeZoneInfo.ConvertTimeFromUtc(time, tzi);
        //}
        //public static DateTime ToTimeZoneTime(DateTime time, string timeZoneId = "Central Standard Time")
        //{
        //    KENNEWEntities dbcontext = new KENNEWEntities();
        //    //var hoursdata = dbcontext.tbl_DaylightShift.FirstOrDefault();
        //    decimal hours = Convert.ToInt32(-5);
        //    return time.AddHours(Convert.ToInt32(hours));

        //}
        public static DateTime ToTimeZoneTime(DateTime time, string timeZoneId = "AUS Eastern Standard Time")
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(time, tzi);
            //decimal hours = getCell("select [Hours] from     dbo.tblDaylightShift");
            //return time.AddHours(Convert.ToInt32(hours));

        }
        // Baand change 21st November for tblDayLightShift
        //public static decimal getCell(string Query)
        //{

        //    decimal de = 0;
        //    try
        //    {
        //        con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }
        //        //con.Open();
        //        SqlCommand com = new SqlCommand(Query, con);
        //        //com.CommandTimeout = 0;
        //        de = Convert.ToDecimal((object)com.ExecuteScalar());
        //    }
        //    catch (Exception ex) { }
        //    finally { con.Close(); }
        //    return de;
        //}
        // baans end 21st November

        //Email For Password Reset(Starts)
        public static bool SendEmailWithName(string To, string Sub, string Msg)
        {
            bool Status = false;
            string From = "azure_0df29fac9bf524c9c35a58c87ad05d0b@azure.com";
            string Password = "tBmetVs7N2DpnEa";
            NetworkCredential Credential = new NetworkCredential(From, Password);
            SmtpClient Client = new SmtpClient();
            Client.DeliveryMethod = SmtpDeliveryMethod.Network;
            Client.EnableSsl = true;
            Client.Host = "smtp.sendgrid.net";
            Client.Port = 587;
            Client.UseDefaultCredentials = false;
            Client.Credentials = Credential;
            MailMessage MailMsg = new MailMessage();
            MailMsg.From = new MailAddress(FromEmailID, FromEmailName);
            string tomail = To.Replace(":", ",");
            string[] multi = tomail.Split(',');
            foreach (string multiemail in multi)
            {
                MailMsg.To.Add(new MailAddress(multiemail));
            }
            MailMsg.Subject = Sub;
            MailMsg.Body = Msg;
            MailMsg.IsBodyHtml = true;
            try
            {
               
                Client.Send(MailMsg);
                Status = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Status = false;
            }
            return Status;
        }
        //Email For Password Reset(Ends)
        public static string ActiveUser()
        {
            // baans change 29th October for Session timeout to 2 hour
            HttpContext.Current.Session.Timeout = 120;
            // baans end 29th October
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["UserEmail"] as string))
            {
                return HttpContext.Current.Session["UserEmail"].ToString();
            }
            else
            {
                return "";
            }
           
        }      
        public static string ActiveClient()
        {
            HttpContext.Current.Session.Timeout = 120;
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["ClientEmail"] as string))
            {
                return HttpContext.Current.Session["ClientEmail"].ToString();
            }
            else
            {
                return "";
            }
        }
        public static int ActiveClientId()
        {
            HttpContext.Current.Session.Timeout = 240;
            var a = (HttpContext.Current.Session["MyClientId"]);
            if (a != null)
            {
                return ((int)HttpContext.Current.Session["MyClientId"]);
            }
            else
            {
               HttpContext.Current.Response.Redirect("/Client/Login", true);
            }
            return 0;
        }


        //27 May 2019 (N) (Function Moved to MasterPdfController)
        //public static bool SendEmail(string To, string Sub, AlternateView Av, string path, string PathPdf, string AcctEmail)
        //{
        //    bool Status = false;
        //    NetworkCredential Credential = new NetworkCredential(SenderID, SenderPassword);
        //    SmtpClient Client = new SmtpClient();
        //    Client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    Client.EnableSsl = true;
        //    Client.Host = "smtp.sendgrid.net";
        //    Client.Port = 25;
        //    Client.UseDefaultCredentials = false;
        //    Client.Credentials = Credential;
        //    MailMessage MailMsg = new MailMessage();
        //    // baans change 24th November for sending by AccountManagersEmail
        //    MailMsg.From = new MailAddress(AcctEmail, FromEmailName);
        //    //MailMsg.From = new MailAddress(FromEmailID, FromEmailName);
        //    //string tomail = To.Replace(":", ",");
        //    string tomail = To;
        //    string[] multi = tomail.Split(',');
        //    foreach (string multiemail in multi)
        //    {
        //        MailMsg.To.Add(new MailAddress(multiemail));
        //    }
        //    if(BCCMailID!="")
        //    MailMsg.Bcc.Add(new MailAddress(BCCMailID));

        //    //if (CCMailID != "")
        //    //    MailMsg.CC.Add(new MailAddress(CCMailID));
        //    if (AcctEmail != "")
        //        MailMsg.CC.Add(new MailAddress(AcctEmail));
        //    // baans end 24th November

        //    MailMsg.AlternateViews.Add(Av);
        //    MailMsg.Subject = Sub;
        //    MailMsg.BodyEncoding = Encoding.Default;
        //    MailMsg.IsBodyHtml = true;
        //    MailMsg.Attachments.Add(new Attachment(PathPdf));


        //    try
        //    {


        //            Client.Send(MailMsg);
        //            Status = true;
        //        foreach(var item in MailMsg.Attachments)
        //        {
        //            item.Dispose();
        //        }
        //        if (File.Exists(PathPdf))
        //        {
        //           File.Delete(PathPdf);
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        Status = false;
        //    }
        //    return Status;
        //}
        //27 May 2019 (N)

    }
}