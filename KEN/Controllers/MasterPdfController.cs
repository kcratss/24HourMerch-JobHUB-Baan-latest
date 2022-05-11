using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using KEN_DataAccess;
using AutoMapper;
using KEN.AppCode;
using KEN.Interfaces.Iservices;
using KEN.Interfaces;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using iTextSharp.text;
using iTextSharp.text.pdf;
using KEN.Filters;
using PostmarkDotNet;
using PostmarkDotNet.Model;
using System.Text.RegularExpressions;
using System.Configuration;
using iTextSharp.text.html;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class MasterPdfController : Controller
    {
        public static string PostmarkToken = ConfigurationManager.AppSettings["PostmarkToken"].ToString();

        private readonly IOpportunityService _baseService;
        ResponseViewModel response = new ResponseViewModel();
        public MasterPdfController(IOpportunityService baseService)
        {
            _baseService = baseService;

        }
        KENNEWEntities dbContext = new KENNEWEntities();
        // GET: CombineAllPdf
        public ActionResult Index()
        {

            return View();
        }
        public async System.Threading.Tasks.Task<ActionResult> SendEmail(EmailViewModel model, int OpportunityId)
        {
            string path = "";
            string PathPdf = "";
            string JpegPath = "";
            if (model.OptionStatus == "Opp")
            {
                path = Server.MapPath("~/Images/MailFooter.jpg"); // 04 Oct 2018(N)
                PathPdf = "~/Content/uploads/Quotes/Quote_" + OpportunityId + "- " + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd-MM-yyyy") + ".pdf";
                PathPdf = Server.MapPath(PathPdf);
                QuotesPdf(OpportunityId, PathPdf, model.OptionStatus, model.Type);
                // Added by baans 16Sep2020 to add jpeg image in attachment
                JpegPath = "~/Content/uploads/Quotes/";
                JpegPath = Server.MapPath(JpegPath);
                LoadImage(PathPdf, JpegPath);
            }
            else if (model.OptionStatus == "Order")
            {
                path = Server.MapPath("~/Images/MailFooter.jpg");        //20 Sep 2018 (N)
                PathPdf = "~/Content/uploads/Order/Order_" + OpportunityId + "- " + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd-MM-yyyy") + ".pdf";
                PathPdf = Server.MapPath(PathPdf);
                OrderPdf(OpportunityId, PathPdf, model.OptionStatus, model.Type);
                // Added by baans 23Sep2020 to add jpeg image in attachment
                JpegPath = "~/Content/uploads/Order/";
                JpegPath = Server.MapPath(JpegPath);
                LoadImage(PathPdf, JpegPath);
            }
            else if (model.OptionStatus == "Invoice")
            {
                path = Server.MapPath("~/Images/MailFooter.jpg");
                PathPdf = "~/Content/uploads/Invoice/Invoice_" + OpportunityId + "- " + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd-MM-yyyy") + ".pdf";
                PathPdf = Server.MapPath(PathPdf);
                InvoicePdf(OpportunityId, PathPdf, model.OptionStatus, model.Type);    /*tarun 12/10/2018*/
                // Added by baans 23Sep2020 to add jpeg image in attachment
                JpegPath = "~/Content/uploads/Invoice/";
                JpegPath = Server.MapPath(JpegPath);
                LoadImage(PathPdf, JpegPath);
            }
            else if (model.OptionStatus == "Confirm")
            {
                path = Server.MapPath("~/Images/MailFooter.jpg");
                // baans change 14th November for changing the confirmation
                //PathPdf = "~/Content/uploads/Confirm/Confirmation_" + OpportunityId + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).Ticks + ".pdf";
                PathPdf = "~/Content/uploads/Confirm/OrderConfirmation_" + OpportunityId + "- " + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd-MM-yyyy") + ".pdf";
                // baans end 14th November
                PathPdf = Server.MapPath(PathPdf);
                ConfirmationPdf(OpportunityId, PathPdf, "Order", model.Type);    /*tarun 12/10/2018*/
                // Added by baans 23Sep2020 to add jpeg image in attachment
                JpegPath = "~/Content/uploads/Confirm/";
                JpegPath = Server.MapPath(JpegPath);
                LoadImage(PathPdf, JpegPath);
            }
            else
            {
                path = Server.MapPath("~/Images/MailFooter.jpg");
                PathPdf = "~/Content/uploads/Order/Order_" + OpportunityId + "- " + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd-MM-yyyy") + ".pdf";
                PathPdf = Server.MapPath(PathPdf);
                OrderPdf(OpportunityId, PathPdf, "Order", model.Type);
                // Added by baans 23Sep2020 to add jpeg image in attachment
                JpegPath = "~/Content/uploads/Order/";
                JpegPath = Server.MapPath(JpegPath);
                LoadImage(PathPdf, JpegPath);
            }



            //################################################################
            //Email Content and mailing
            try
            {
                var EmailContent = dbContext.tblEmailContents.Where(_ => _.Purpose == model.OptionStatus).FirstOrDefault();
                var contactDetails = dbContext.tblOppContactMappings.Where(_ => _.OpportunityId == OpportunityId && _.IsPrimary == true).FirstOrDefault().tblcontact;
                string ContactFullname = "", ContactEmail = "", Organisation = "", AccountManageName = "", ContactName = "", AcctDesignation = "", AcctEmail = "";

                // baans change 26th October for Dynamic Designation
                var CurrentAccountManagerDetail = dbContext.tblOpportunities.Where(_ => _.OpportunityId == OpportunityId).Select(_ => _.AcctManagerId).FirstOrDefault();
                if (CurrentAccountManagerDetail != null)
                {

                    AccountManageName = dbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.firstname + " " + _.lastname).FirstOrDefault();
                    AcctDesignation = dbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.title).FirstOrDefault();
                    AcctEmail = dbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.email).FirstOrDefault();
                }
                // baans end 26th October

                if (contactDetails != null)
                {
                    ContactFullname = contactDetails.first_name + " " + contactDetails.last_name;
                    ContactEmail = contactDetails.email;
                    ContactName = contactDetails.first_name;
                    if (contactDetails.tblOrganisation != null)
                        Organisation = contactDetails.tblOrganisation.OrgName;

                }
                if (model.MailMessage2 != null)
                {
                    model.MailMessage2 = Regex.Replace(model.MailMessage2, "\n", "<br>");
                }
                else
                {
                    model.MailMessage2 = "";
                }

                string[] MailSignature = { };
                string Msg4 = "";

                if (EmailContent != null)
                {
                    var accManager = dbContext.tblcontacts.FirstOrDefault(_ => _.acct_manager_id == CurrentAccountManagerDetail);
                    string mobileNumber = accManager.mobile;
                    string changeNumber = EmailContent.Body3.Replace("02 9559 2400", mobileNumber);
                    MailSignature = changeNumber.Split('^');
                    /*MailSignature = EmailContent.Body3.Split('^');*/
                }

                //                string Msg1 = "", Msg2 = "", Msg3 = "", Msg4 = "";

                //if (EmailContent != null)
                //{
                //    // Baans change 15th November for msg from body1 pop up and 2
                //    Msg1 = EmailContent.Body1;
                //    Msg2 = EmailContent.Body2;
                //    // Baans end 15th November
                //    Msg3 = EmailContent.Body3;
                //}
                // baans change 20th November for changing the content
                if (AccountManageName.Contains("Kenneth"))
                {
                    Msg4 = "http://au.linkedin.com/in/kennethswan";
                }
                // baans end 20th November
                LinkedResource Img = null;
                string str = "";
                if (model.OptionStatus == "Opp")
                {
                    str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + MailSignature[0] + "<br><br>" + AcctDesignation + "<br>" + MailSignature[1] + "<br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = \"cid:MailFooter.jpg\" id = 'img' alt = '' style='width:86%;height:auto;' /></a></td></tr></table>";
                    // baans end 17th October
                }
                // baans change 07 august for email through job
                else if (model.OptionStatus == "Confirm")
                {
                    //Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
                    //Img.ContentId = "MyImage";

                    str = @"<br>Hi " + ContactName + ",<br><br>Your order is confirmed to be dispatched by " + model.Shipping + " by " + model.ConfirmedDate + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + MailSignature[0] + "<br><br>" + AcctDesignation + "<br>" + MailSignature[1] + "<br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = \"cid:MailFooter.jpg\" id = 'img' alt = '' style='width:86%;height:auto;' /></a></td></tr></table>";
                    // baans end 22nd Octber
                }

                else if (model.OptionStatus == "Invoice")
                {
                    //Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
                    //Img.ContentId = "MyImage";

                    str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + MailSignature[0] + "<br><br>" + AcctDesignation + "<br>" + MailSignature[1] + "<br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = \"cid:MailFooter.jpg\" id = 'img' alt = '' style='width:86%;height:auto;' /></a></td></tr></table>";
                }
                // baans end 07 august
                else
                {
                    //Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
                    //Img.ContentId = "MyImage";
                    // baans change 17th October for Change in the Signature

                    str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + MailSignature[0] + "<br><br>" + AcctDesignation + "<br>" + MailSignature[1] + "<br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = \"cid:MailFooter.jpg\" id = 'img' alt = '' style='width:86%;height:auto;' /></a></td></tr></table>";

                    //baans end 17th October
                }

                string To = model.Email;
                string subject = model.Subject;

                //######################################################
                //Sending Mail Using PostMark

                var message = new PostmarkMessage()
                {
                    To = To,
                    //From = DataBaseCon.FromEmailName+" <" + AcctEmail +">",
                    From = DataBaseCon.FromEmailID,
                    // Bcc = DataBaseCon.BCCMailID == "" ? null : DataBaseCon.BCCMailID,
                    Cc = AcctEmail == "" ? null : AcctEmail,
                    TrackOpens = true,
                    Subject = subject,
                    //TextBody = "Plain Text Body",
                    HtmlBody = str,
                    Tag = "business-message",
                    Headers = new HeaderCollection{
                    { "Name", "24 Hour Merchandise"},
                }
                };

                var PdfFileName = PathPdf.Split('\\');
                var PdfName = PdfFileName.Length - 1;

                var pdfContent = System.IO.File.ReadAllBytes(PathPdf);
                var imageContent = System.IO.File.ReadAllBytes(path);

                message.AddAttachment(imageContent, "MailFooter.jpg", "image/jpg", "MailFooter.jpg");

                message.AddAttachment(pdfContent, PdfFileName[PdfName], "application/pdf", "cid:MyPdf");

                //Added by baans 16Sep2020 to add pdf jpeg image in attachment start
                string[] PdfImages = Directory.GetFiles(JpegPath, "*.Jpeg", SearchOption.AllDirectories);

                foreach (string PdfImg in PdfImages)
                {
                    var PdfImageContent = System.IO.File.ReadAllBytes(PdfImg);
                    string outImageName = Path.GetFileName(PdfImg);
                    message.AddAttachment(PdfImageContent, outImageName, "image/jpeg");
                }
                //Added by baans 16Sep2020 to add pdf jpeg image in attachment end

                var client = new PostmarkClient(PostmarkToken);
                var sendResult = await client.SendMessageAsync(message);

                if (System.IO.File.Exists(PathPdf))
                {
                    System.IO.File.Delete(PathPdf);
                }
                foreach (string PdfImg in PdfImages)
                {
                    if (System.IO.File.Exists(PdfImg))
                    {
                        System.IO.File.Delete(PdfImg);
                    }
                }

                //PostMark Mail ends here
                //##############################################

                var Oppodata = dbContext.tblOpportunities.Where(_ => _.OpportunityId == OpportunityId).FirstOrDefault();

                Oppodata.QuoteMail = model.MailMessage2;
                if (model.OptionStatus == "Opp")
                    Oppodata.QuoteMailDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                if (model.OptionStatus == "Order")
                    Oppodata.OrderMailDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                if (model.OptionStatus == "Invoice")
                    Oppodata.InvoiceMailDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                if (model.OptionStatus == "Confirm")
                    Oppodata.ConfirmMailDate = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));

                Oppodata.UpdatedBy = DataBaseCon.ActiveUser();
                Oppodata.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));


                dbContext.SaveChanges();
                response.Result = ResponseType.Success;
                response.Message = "Mail Sent successfully";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public async System.Threading.Tasks.Task<ActionResult> SendProofEmail(EmailViewModel model, string OpportunityId, string PageName)
        {
            string path = "";
            string PathPdf = "";
            string MultipleFiles = "";
            string JpegPath = "";

            var OppoId = Convert.ToInt32(OpportunityId);

            var OpportunityData = dbContext.tblOpportunities.Where(_ => _.OpportunityId == OppoId).FirstOrDefault();

            List<OptionViewModel> ProofOptions = new List<OptionViewModel>();
            if (PageName == "QuoteDetails")
            {
                ProofOptions = Mapper.Map<List<OptionViewModel>>(dbContext.tbloptions.Where(_ => _.OpportunityId == OppoId && _.OptionStage == "Opp" && _.include_job == true && _.ProofSent == true));
            }
            else
            {
                ProofOptions = Mapper.Map<List<OptionViewModel>>(dbContext.tbloptions.Where(_ => _.OpportunityId == OppoId && _.OptionStage == "Order" && _.include_job == true && _.ProofSent == true));
            }

            foreach (var item in ProofOptions)
            {
                PathPdf = "~/Content/uploads/Proof/24 Hour Merchandise Proof - " + OpportunityData.OppName + " - " + item.OpportunityId + " - " + item.id + ".pdf";
                PathPdf = Server.MapPath(PathPdf);
                Proof_24HourMerchandise(item.id, PathPdf, "Order", "Proof");
                MultipleFiles = MultipleFiles + "|" + PathPdf;
                // Added by baans 16Sep2020 to add jpeg image in attachment
                JpegPath = "~/Content/uploads/Proof/";
                JpegPath = Server.MapPath(JpegPath);
                LoadImage(PathPdf, JpegPath);

            }

            path = Server.MapPath("~/Images/MailFooter.jpg");

            //################################################################
            //Email Content and mailing
            try
            {
                var EmailContent = dbContext.tblEmailContents.Where(_ => _.Purpose == model.OptionStatus).FirstOrDefault();
                var contactDetails = dbContext.tblOppContactMappings.Where(_ => _.OpportunityId == OppoId && _.IsPrimary == true).FirstOrDefault().tblcontact;

                string ContactFullname = "", ContactEmail = "", Organisation = "", AccountManageName = "", ContactName = "", AcctDesignation = "", AcctEmail = "";

                var CurrentAccountManagerDetail = dbContext.tblOpportunities.Where(_ => _.OpportunityId == OppoId).Select(_ => _.AcctManagerId).FirstOrDefault();
                if (CurrentAccountManagerDetail != null)
                {
                    AccountManageName = dbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.firstname + " " + _.lastname).FirstOrDefault();
                    AcctDesignation = dbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.title).FirstOrDefault();
                    AcctEmail = dbContext.tblusers.Where(_ => _.id == CurrentAccountManagerDetail).Select(_ => _.email).FirstOrDefault();
                }
                // baans end 26th October

                if (contactDetails != null)
                {
                    ContactFullname = contactDetails.first_name + " " + contactDetails.last_name;
                    ContactEmail = contactDetails.email;
                    ContactName = contactDetails.first_name;
                    if (contactDetails.tblOrganisation != null)
                        Organisation = contactDetails.tblOrganisation.OrgName;

                }
                if (model.MailMessage2 != null)
                {
                    model.MailMessage2 = Regex.Replace(model.MailMessage2, "\n", "<br>");
                }
                else
                {
                    model.MailMessage2 = "";
                }


                string[] MailSignature = { };
                string Msg4 = "";

                if (EmailContent != null)
                {
                    MailSignature = EmailContent.Body3.Split('^');
                }

                if (AccountManageName.Contains("Kenneth"))
                {
                    Msg4 = "http://au.linkedin.com/in/kennethswan";
                }

                string str = "";
                if (model.OptionStatus == "Proof")
                {
                    str = @"<br>Hi " + ContactName + ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br>" + MailSignature[0] + "<br><br>" + AcctDesignation + "<br>" + MailSignature[1] + "<br>" + Msg4 + "<br><br><table style ='width:100%'><tr><td colspan='3'><a href ='https://24hourt-shirts.com.au/'><img src = \"cid:MailFooter.jpg\" id = 'img' alt = '' style='width:86%;height:auto;' /></a></td></tr></table>";
                }

                string To = model.Email;
                string subject = model.Subject;

                //######################################################
                //Sending Mail Using PostMark

                var message = new PostmarkMessage()
                {
                    To = To,
                    From = DataBaseCon.FromEmailName + " <" + AcctEmail + ">",
                    // Bcc = DataBaseCon.BCCMailID == "" ? null : DataBaseCon.BCCMailID,
                    Cc = AcctEmail == "" ? null : AcctEmail,
                    TrackOpens = true,
                    Subject = subject,
                    HtmlBody = str,
                    Tag = "business-message",
                    Headers = new HeaderCollection{
                    { "Name", "24 Hour Merchandise"},
                }
                };

                var Count = 1;
                var Attachments = MultipleFiles.Split('|');
                foreach (var file in Attachments)
                {
                    if (file != null && file != "")
                    {
                        var PdfFileName = file.Split('\\');
                        var PdfName = PdfFileName.Length - 1;
                        var pdfContent = System.IO.File.ReadAllBytes(file);
                        message.AddAttachment(pdfContent, PdfFileName[PdfName], "application/pdf", "cid:MyPdf" + Count);
                        Count++;
                    }
                }

                var imageContent = System.IO.File.ReadAllBytes(path);

                message.AddAttachment(imageContent, "MailFooter.jpg", "image/jpg", "MailFooter.jpg");

                //Added by baans 23Sep2020 to add pdf jpeg image in attachment start
                string[] PdfImages = Directory.GetFiles(JpegPath, "*.Jpeg", SearchOption.AllDirectories);

                foreach (string PdfImg in PdfImages)
                {
                    var PdfImageContent = System.IO.File.ReadAllBytes(PdfImg);
                    string outImageName = Path.GetFileName(PdfImg);
                    message.AddAttachment(PdfImageContent, outImageName, "image/jpeg");
                }
                //Added by baans 23Sep2020 to add pdf jpeg image in attachment end

                var client = new PostmarkClient(PostmarkToken);
                var sendResult = await client.SendMessageAsync(message);

                foreach (var file in Attachments)
                {
                    if (file != null && file != "")
                    {
                        if (System.IO.File.Exists(file))
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                }
                foreach (string PdfImg in PdfImages)
                {
                    if (System.IO.File.Exists(PdfImg))
                    {
                        System.IO.File.Delete(PdfImg);
                    }
                }

                //PostMark Mail ends here
                //##############################################
                var optionstage = "";

                if (PageName == "QuoteDetails")
                {
                    optionstage = "Opp";
                }
                else
                {
                    optionstage = "Order";
                }

                var Optiondata = dbContext.tbloptions.Where(_ => _.OpportunityId == OppoId && _.OptionStage == optionstage && _.include_job == true && _.ProofSent == true).ToList();

                if (Optiondata.Count > 0)
                {
                    for (var i = 0; i < Optiondata.Count; i++)
                    {
                        Optiondata[i].ProofSent = false;
                        Optiondata[i].ProofMailSent = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                        var Version = Optiondata[i].ProofVerion;
                        if (Version != null)
                        {
                            Optiondata[i].ProofVerion = Version + 1;
                        }
                        else
                        {
                            Optiondata[i].ProofVerion = 1;
                        }
                    }
                    dbContext.SaveChanges();
                }


                dbContext.SaveChanges();
                response.Result = ResponseType.Success;
                response.Message = "Mail Sent successfully";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public void Print(EmailViewModel model, int OpportunityId)
        {
            QuotesPdf(OpportunityId, "", model.OptionStatus, model.Type);
            // return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMailMessage(int OpportunityId, string OptionStatus)
        {
            var data = new EmailContentViewModel();
            data = _baseService.GetMailMessage(OptionStatus);
            var Oppodata = dbContext.Vw_tblOpportunity.Where(_ => _.OpportunityId == OpportunityId).FirstOrDefault();
            if (Oppodata.ContactID != 0 && Oppodata.ContactID != null)
            {
                var contact = dbContext.tblcontacts.Where(_ => _.id == Oppodata.ContactID).FirstOrDefault();
                data.ClientEmailID = contact.email;
            }
            // baans change 17th Jan for MailSubject with OpportunityId and OpportunityName
            if (data.Subject != null && data.Subject != "")
            {
                var CurrentOpportunityName = dbContext.tblOpportunities.Where(_ => _.OpportunityId == OpportunityId).Select(_ => _.OppName).FirstOrDefault();
                data.Subject = data.Subject + " - " + OpportunityId + " - " + CurrentOpportunityName;
            }
            // baans end 17th Jan
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Quote Pdf 
        public ActionResult QuotesPdf(int id, string path, string OptionStatus, string QuoteType)
        {

            var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();
            var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();
            var AddressinOpp = CustomerDetail.AddressId;

            var dAddress = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.AddressId == AddressinOpp).FirstOrDefault());

            Document doc = new Document(PageSize.A4, -80f, -80f, 20f, 20f);   //Page Size set top ,left,margin
            doc.SetPageSize(PageSize.A4.Rotate());
            PdfWriter write;
            HeaderFooterStatement footer;
            if (QuoteType == "Print")
            {
                write = PdfWriter.GetInstance(doc, Response.OutputStream);
                footer = new HeaderFooterStatement();
                //write.PageEvent = footer;
                var page = new HeaderFooterStatement();
                page.id = id;
                write.PageEvent = page;
                Response.ContentType = ("application/pdf");
            }
            else
            {

                write = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                footer = new HeaderFooterStatement();
                //write.PageEvent = footer;
                var page = new HeaderFooterStatement();
                page.id = id;
                write.PageEvent = page;
            }



            doc.Open();

            int i, j, k = 5;
            var rcount = 0;
            var FColor = new BaseColor(38, 171, 227);  //40, 171, 255        //font color declaration for highlighted data
            var RColor = new BaseColor(230, 230, 230);            //font color declaration for table rows  
            var FColor2 = new BaseColor(51, 51, 51);                /*tarun 20/09/2018*/
            var FColor3 = new BaseColor(255, 255, 255);

            var Heading1 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, FColor);
            var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 28, iTextSharp.text.Font.NORMAL, FColor);
            var Heading3 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
            var Heading4 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1250, true, 9, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
            var contentfontCheck = FontFactory.GetFont((Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
            var Heading5 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
            var Heading6 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor3);  /*tarun 20/09/2018*/



            Phrase phrase;
            PdfPCell cell;

            PdfPTable headertable1 = new PdfPTable(15);
            headertable1.SetWidths(new int[] { 5, 5, 5, 5, 7, 1, 6, 7, 7, 7, 7, 3, 5, 5, 5 });
            //PdfPTable headertable2 = new PdfPTable(18);
            //headertable2.SetWidths(new int[] { 7, 4, 6, 6, 9, 6, 11, 9, 9, 9, 9, 9, 6, 9, 8, 8, 9, 9 });
            PdfPTable headertable2 = new PdfPTable(19);
            //headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 14, 8, 8, 8, 8, 8, 6, 9, 8, 8, 9, 9 });
            headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 19, 6, 6, 6, 6, 6, 6, 9, 8, 8, 9, 9 });
            headertable2.SpacingAfter = 2f;
            PdfPTable headertable3 = new PdfPTable(18);
            headertable3.SetWidths(new int[] { 7, 6, 6, 6, 7, 6, 8, 10, 11, 10, 9, 9, 6, 8, 8, 9, 9, 8 });

            // TABLE DATA ############################################################################################################## 

            if (OptionData.Count > 0)
            {
                var CurrentColor = BaseColor.WHITE;

                for (i = 0; i < OptionData.Count; i++)
                {
                    // cell.BackgroundColor = (i % 2) == 0
                    // ? RColor : Color.WHITE;

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (i % 2 == 0)                               // Adding alternate colors to cells
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].OptionNo.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);



                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].Quantity.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].brand.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].code.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].Item.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].colour.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    if (OptionData[i].Link != "N/A")
                    {
                        phrase = new Phrase();
                        //string linkurl = OptionData[i].Link.ToString();
                        //string linkurl2 = "";
                        //if (linkurl.Length > 12)
                        //{
                        //    linkurl2 = linkurl.Substring(0, 12);
                        //}
                        //else
                        //{
                        //    linkurl2 = linkurl;
                        //}
                        //Chunk chunk = new Chunk(linkurl2, FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.UNDERLINE, BaseColor.BLUE));
                        iTextSharp.text.Image ImagePath = iTextSharp.text.Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/maximize.png"));
                        System.Drawing.PointF location = new System.Drawing.PointF(1, 1);
                        System.Drawing.RectangleF linkBounds = new System.Drawing.RectangleF(location, new System.Drawing.SizeF(ImagePath.Width, ImagePath.Height));
                        Chunk chunk = new Chunk(ImagePath, 0, 0, true);
                        chunk.SetAnchor(OptionData[i].Link.ToString());
                        phrase.Add(chunk);
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 3f;
                        //cell.NoWrap = true;
                        cell.PaddingBottom = 5f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);
                    }
                    else
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk("N/A", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);
                    }

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].size.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    //cell.NoWrap = true;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    // baans change 28th November
                    if (OptionData[i].Front_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc1.ToString(), Heading3));
                    }
                    //phrase.Add(new Chunk("Hi" , Heading3));
                    //phrase.Add(Chunk.NEWLINE);
                    //phrase.Add(new Paragraph(" Digital Print", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    //phrase.Add(new Chunk(OptionData[i].back.ToString(), Heading3));
                    if (OptionData[i].Back_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc1.ToString(), Heading3));
                    }
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    //phrase.Add(new Chunk(OptionData[i].leftdec.ToString(), Heading3));
                    if (OptionData[i].Left_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc1.ToString(), Heading3));
                    }
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    //phrase.Add(new Chunk(OptionData[i].rightdec.ToString(), Heading3));
                    if (OptionData[i].Right_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc1.ToString(), Heading3));
                    }
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    //phrase.Add(new Chunk(OptionData[i].other.ToString(), Heading3));
                    if (OptionData[i].Other_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc1.ToString(), Heading3));
                    }
                    // baans end 28th November
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    string decimalvalue = string.Format("{0:C}", OptionData[i].OtherCost.ToString());
                    //converting value to decimal format with Currency sign($).

                    phrase.Add(new Chunk(decimalvalue, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].Service.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);


                    phrase = new Phrase();
                    string decimalvalue1 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].UnitExGST.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture)), 2));
                    //converting value to decimal format with Currency sign($).

                    phrase.Add(new Chunk(decimalvalue1, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);


                    phrase = new Phrase();
                    string decimalvalue2 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].UnitInclGST.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture)), 2));
                    phrase.Add(new Chunk(decimalvalue2, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    string decimalvalue3 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].ExtExGST.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture)), 2));
                    phrase.Add(new Chunk(decimalvalue3, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);


                    phrase = new Phrase();
                    string decimalvalue4 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].ExtInclGST.ToString("#,##0.00", System.Globalization.NumberFormatInfo.InvariantInfo)), 2));
                    phrase.Add(new Chunk(decimalvalue4, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = 0;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -5f;
                    cell.Border = 0;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                        CurrentColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                        CurrentColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    rcount++;
                    if (OptionData.Count > 6 && i != OptionData.Count - 1)
                    {
                        if (i > 0 && rcount % 6 == 0)
                        {
                            doc.Add(headertable2);
                            headertable2.DeleteBodyRows();
                            doc.NewPage();
                            rcount = 0;
                        }
                    }
                }


                for (j = 0; j <= (k - rcount); j++)
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.FixedHeight = 25f;
                    cell.PaddingBottom = 6f;
                    cell.Border = 0;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;

                    }
                    else
                    {
                        cell.BackgroundColor = RColor;

                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                        CurrentColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                        CurrentColor = RColor;
                    }
                    headertable2.AddCell(cell);
                }
            }

            //phrase = new Phrase();
            //phrase.Add(new Chunk(" ", Heading3));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 18;
            //cell.PaddingBottom = -2f;
            //cell.Border = 0;
            //headertable3.AddCell(cell);

            //Bottom Address ##############################################################################

            phrase = new Phrase();
            //Commented and change by baans 11Dec2020 start
            //phrase.Add(new Chunk("Please Note: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, FColor)));
            //phrase.Add(new Chunk("This is a Quote only. To Proceed please request an Invoice", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
            phrase.Add(new Chunk("Please Note: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 24, iTextSharp.text.Font.NORMAL, FColor)));
            phrase.Add(new Chunk("This is a Quote only. To Proceed please request an Invoice", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
            //Commented and change by baans 11Dec2020 end
            cell = new PdfPCell(phrase);
            cell.Colspan = 15;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.PaddingBottom = 5f;
            cell.Rowspan = 2;
            cell.PaddingLeft = -1;
            cell.PaddingTop = 4f;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Shipping:", Heading4));
            phrase.Add(new Chunk(" " + CustomerDetail.ShippingBy.ToString(), Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingLeft = 15f;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            string decivalue = string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.ShippingPrice.ToString()), 2));
            phrase.Add(new Chunk(decivalue, Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);


            //commented by baans 30Sep2020 start
            //phrase = new Phrase();
            //phrase.Add(new Chunk("", Heading3));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 7;
            //cell.PaddingBottom = -2;
            //cell.PaddingTop = 2;
            //cell.Border = 0;
            //headertable3.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("Next Step: If you want to go ahead, please provide size breakdown, delivery requirements and deadline", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 11;
            //cell.PaddingBottom = -2f;
            //cell.PaddingTop = 2;
            //cell.Border = 0;
            //headertable3.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("PRODUCTION DATE: ", FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, FColor)));
            //phrase.Add(new Chunk("Not Confirmed", FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 7;
            //cell.PaddingBottom = 6f;
            //cell.Border = 0;
            //headertable3.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("For your order to be ready for pickup/dispatch by:  ", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            //string reqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.ReqDate);
            //phrase.Add(new Chunk(reqdate, FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            //cell = new PdfPCell(phrase);
            //cell.HorizontalAlignment = Element.ALIGN_LEFT | Element.ALIGN_BOTTOM;
            //cell.Colspan = 5;
            //cell.PaddingTop = 10f;
            //cell.PaddingBottom = -5f;
            //cell.Border = 0;
            //headertable3.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("Your 50% Production Deposit is required by:  ", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            //string depreqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.DepositReqDate);
            //phrase.Add(new Chunk(depreqdate, FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 6;
            //cell.PaddingTop = 10f;
            //cell.HorizontalAlignment = Element.ALIGN_RIGHT | Element.ALIGN_BOTTOM;
            //cell.PaddingBottom = -5f;
            //cell.Border = 0;
            //headertable3.AddCell(cell);
            //// next Line

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", Heading5));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 6;
            //cell.Border = 0;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //cell.PaddingBottom = 0f;
            //headertable3.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("Need it sooner? Contact us now!" , FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 6;
            //cell.Border = 0;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.PaddingTop = 0f;
            //headertable3.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", Heading5));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 6;
            //cell.Border = 0;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //cell.PaddingBottom = 0f;
            //headertable3.AddCell(cell);
            //commented by baans 30Sep2020 end        

            //Added new values by baans 01Oct2020 start
            phrase = new Phrase();
            phrase.Add(new Chunk("Quote Total:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 8f;
            cell.Border = 0;
            cell.PaddingLeft = 15f;
            headertable3.AddCell(cell);

            if (CustomerDetail.QuoteTotal != null)
            {
                phrase = new Phrase();
                string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.QuoteTotal).ToString()), 2));
                phrase.Add(new Chunk(Ordertotal, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                headertable3.AddCell(cell);

            }
            else
            {
                phrase = new Phrase();
                phrase.Add(new Chunk("$0.00", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                headertable3.AddCell(cell);

            }

            //Next Row
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 15;
            cell.PaddingBottom = -2f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("GST:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 7f;
            cell.Border = 0;
            cell.PaddingLeft = 15f;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            if (CustomerDetail.QuoteGST != null)
            {
                phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.QuoteTotal + CustomerDetail.ShippingPrice) / 11), 2)), Heading4));
            }
            else
            {
                phrase.Add(new Chunk("$0.00", Heading4));
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 7f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            //Next Row
            phrase = new Phrase();
            phrase.Add(new Chunk("Delivery Address:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            cell.PaddingLeft = -1;
            headertable3.AddCell(cell);

            var Address1 = " ";
            if (AddressinOpp != null)
            {
                Address1 = dAddress.Address1 == null ? "" : dAddress.Address1.ToString();
            }
            else
            {
                Address1 = CustomerDetail.Address1 == "N/A" ? "" : CustomerDetail.Address1.ToString();
            }
            phrase = new Phrase();
            phrase.Add(new Chunk(Address1, Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("\x6C", contentfontCheck));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("All quotes are inclusive of set-up charges. Nothing more to pay \n", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 9;
            cell.PaddingBottom = 8f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Final Total:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingLeft = 15f;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);


            phrase = new Phrase();
            if (CustomerDetail.QuoteFinalTotal != null)
            {
                phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.QuoteTotal + CustomerDetail.ShippingPrice), 2)), Heading4));
            }
            else
            {
                phrase.Add(new Chunk("$0.00", Heading4));
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            //Next Row
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            var Address2 = " ";
            if (AddressinOpp != null)
            {
                Address2 = dAddress.Address2 == null ? "" : dAddress.Address2.ToString();
            }
            else
            {
                Address2 = CustomerDetail.Address2 == "N/A" ? "" : CustomerDetail.Address2.ToString();
            }
            phrase = new Phrase();
            phrase.Add(new Chunk(Address2, Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("\x6C", contentfontCheck));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.PaddingBottom = 3f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("For every confirmed order, prior to production a full colour proof will be sent to you to confirm your artwork \n", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 9;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Deposit Paid:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingLeft = 15f;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);


            phrase = new Phrase();
            if (CustomerDetail.AmtReceived != null)
            {
                phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.AmtReceived.ToString()), 2)), Heading4));
            }
            else
            {
                phrase.Add(new Chunk("$0.00", Heading4));
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            //Next Row
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            if (AddressinOpp != null)
            {
                phrase.Add(new Chunk(dAddress.State == null ? "" : dAddress.State.ToString() + "  " + dAddress.Postcode.ToString(), Heading4));
            }
            else
            {
                if (CustomerDetail.Postcode != null)
                {
                    phrase.Add(new Chunk(CustomerDetail.State == "N/A" ? "" : CustomerDetail.State.ToString() + "  " + CustomerDetail.Postcode.ToString(), Heading4));
                }
                else
                {
                    phrase.Add(new Chunk(" ", Heading4));
                }
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("\x6C", contentfontCheck));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.PaddingBottom = 3f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Stock levels are changing daily. The earlier an order is confirmed the less the chance of an item being unavailable \n", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 9;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Balance Due:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingLeft = 15f;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingTop = -1;
            cell.Border = 0;
            headertable3.AddCell(cell);

            decimal? Balance = 0;
            if (CustomerDetail.AmtReceived != null)
            {

                Balance = Math.Round(Convert.ToDecimal(CustomerDetail.QuoteFinalTotal + CustomerDetail.ShippingPrice), 2) - Math.Round(Convert.ToDecimal(CustomerDetail.AmtReceived.ToString()), 2);
            }
            else
            {
                Balance = Math.Round(Convert.ToDecimal(CustomerDetail.QuoteFinalTotal + CustomerDetail.ShippingPrice), 2);
            }
            phrase = new Phrase();
            phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(Balance.ToString()), 2)), Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);
            //Added new values by baans 01Oct2020 end

            doc.Add(headertable1);
            doc.Add(headertable2);
            doc.Add(headertable3);

            doc.Close();
            doc.Dispose();
            if (QuoteType == "Print")
            {

                return View();
            }
            else
            {
                return null;
            }
        }

        // Header/Footer For Quote Pdf
        public class HeaderFooterStatement : PdfPageEventHelper
        {
            int counter = 0;
            public int id { get; set; }
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                KENNEWEntities dbContext = new KENNEWEntities();

                //int id = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["id"]);
                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();

                Rectangle pageSize = doc.PageSize;

                var FColor = new BaseColor(38, 171, 227);  //40, 171, 255        //font color declaration for highlighted data
                var RColor = new BaseColor(230, 230, 230);            //font color declaration for table rows  
                var FColor2 = new BaseColor(51, 51, 51);                /*tarun 20/09/2018*/
                var FColor3 = new BaseColor(38, 38, 38);
                var FColor4 = new BaseColor(255, 255, 255);

                //var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, FColor);
                var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 32, iTextSharp.text.Font.NORMAL, FColor3);
                var Heading2 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 32, iTextSharp.text.Font.NORMAL, FColor);
                var Heading3 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var Heading4 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1250, true, 9, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var contentfontCheck = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
                var Heading5 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var Heading6 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor3);
                var Heading7 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor);
                var Heading8 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 12, iTextSharp.text.Font.NORMAL, FColor4);
                var Heading9 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 13, iTextSharp.text.Font.BOLD, FColor4);
                var Heading10 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 7, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/



                Phrase phrase;
                PdfPCell cell;

                PdfPTable headertable1 = new PdfPTable(15);
                headertable1.SetWidths(new int[] { 6, 4, 5, 5, 7, 1, 6, 7, 7, 7, 7, 3, 5, 5, 5 });
                //PdfPTable headertable2 = new PdfPTable(18);
                //headertable2.SetWidths(new int[] { 7, 4, 6, 6, 9, 6, 11, 9, 9, 9, 9, 9, 6, 9, 8, 8, 9, 9 });
                //headertable2.SpacingAfter = 1f;
                PdfPTable headertable2 = new PdfPTable(19);
                headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 19, 6, 6, 6, 6, 6, 6, 9, 8, 8, 9, 9 });
                headertable2.SpacingAfter = 2f;

                PdfPTable headertable3 = new PdfPTable(14);
                string ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Header.jpg");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(ImagePath), true);
                cell.Border = 0;
                cell.PaddingTop = -18;
                cell.Colspan = 15;
                cell.FixedHeight = 58;
                cell.PaddingRight = -700;
                cell.PaddingLeft = -28;
                headertable1.AddCell(cell);

                /*tarun 20/09/2018*/
                //phrase = new Phrase(); //Commented by baans 30Sep2020
                //phrase.Add(new Chunk(" ", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 15;
                //cell.PaddingBottom = 1f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Job Name: ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);
               
                phrase = new Phrase();
                /*phrase.Add(new Chunk("Custom Merchandise Quote", Heading2));*/
                phrase.Add(new Chunk("CUSTOM MERCHANDISE ", Heading1));
                phrase.Add(new Chunk("QUOTE", Heading2));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 9;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Quote Date:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                //cell.PaddingBottom = 5f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingLeft = 15f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                string datetime2 = string.Format("{0:dd/MM/yyyy}", CustomerDetail.QDate);      //setting date format to only show date in desired format
                phrase.Add(new Chunk(datetime2, Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                // Next Line

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Job Name:", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.PaddingTop = -4f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.PaddingTop = -4f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contact: ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 7f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 7f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("\x6C", contentfontCheck));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 5f;
                //cell.PaddingTop = 5f;
                //cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;                    /*tarun 20/09/2018*/
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("All quotes are inclusive of set-up charges. Nothing more to pay \n", Heading5));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 4;
                //cell.PaddingBottom = 5f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 9;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 4;
                //cell.PaddingBottom = 5f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Quote Date:", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingLeft = 15f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //string datetime2 = string.Format("{0:dd/MM/yyyy}", CustomerDetail.QDate);      //setting date format to only show date in desired format
                //phrase.Add(new Chunk(datetime2, Heading4));
                //cell = new PdfPCell(phrase);
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.Colspan = 1;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                // baans change 2nd November for the change to the jobhub number
                phrase.Add(new Chunk("Quote/Invoice No:", Heading4));
                //phrase.Add(new Chunk("JobHUB Number:", Heading4));
                // baans end 2nd November
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingBottom = 5f;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.QuoteNo.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                //cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                // Next Line

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Organisation:", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Contact:", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                ////cell.PaddingTop = -4;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                ////cell.PaddingTop = -4;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Organisation:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                //cell.PaddingBottom = 3f;
                cell.PaddingBottom = 5f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                cell.PaddingBottom = 5f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();            
                //phrase.Add(new Chunk("", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("\x6C", contentfontCheck));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 0f;
                //cell.PaddingTop = 6f;
                //cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;           /*tarun 20/09/2018*/
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("For every confirmed order, prior to production a full colour proof will be sent to you for your approval. \n", Heading5));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 4;
                //cell.PaddingBottom = 0f;
                ////cell.PaddingTop = -8;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                /*phrase.Add(new Chunk("Custom Merchandise Quote", Heading2));*/
                phrase.Add(new Chunk("Quote Status:", Heading7));
                phrase.Add(new Chunk(" Pending", Heading6));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.Colspan = 9;
                cell.PaddingBottom = 10f;
                cell.PaddingTop = -15;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Quote Date:", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingLeft = 15f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //string datetime2 = string.Format("{0:dd/MM/yyyy}", CustomerDetail.QDate);      //setting date format to only show date in desired format
                //phrase.Add(new Chunk(datetime2, Heading4));
                //cell = new PdfPCell(phrase);
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.Colspan = 1;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //// baans change 2nd November for the change to the jobhub number
                //phrase.Add(new Chunk("Quote/Invoice No:", Heading4));
                ////phrase.Add(new Chunk("JobHUB Number:", Heading4));
                //// baans end 2nd November
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingBottom = 3f;
                ////cell.PaddingTop = -4;
                //cell.PaddingLeft = 15f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.QuoteNo.ToString(), Heading4));
                //cell = new PdfPCell(phrase);
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.Colspan = 1;
                ////cell.PaddingTop = -4;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Terms:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingBottom = 3f;
                cell.PaddingBottom = 3f;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" 50% Production Deposit \n Balance prior to shipping", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 2;
                //cell.PaddingTop = 10f;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                // Next Line

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Contact:", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                //cell.PaddingTop = -4;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                //Commented and changed by baans 11Dec2020 start
                //phrase.Add(new Chunk("COMPLETION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("To be Confirmed", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                phrase.Add(new Chunk("COMPLETION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("To be Confirmed", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //Commented and changed by baans 11Dec2020 start
                cell = new PdfPCell(phrase);
                cell.Colspan = 7;
                cell.PaddingBottom = 6f;
                cell.PaddingTop = 3f;
                cell.PaddingLeft = -1f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("To proceed please request an Invoice", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = 3f;
                cell.PaddingTop = 3f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                cell.PaddingLeft = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("To proceed please request an Invoice", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 5;
                cell.PaddingBottom = 1f;
                cell.PaddingTop = 3f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                //cell.PaddingTop = -4;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Organisation:", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                ////phrase.Add(new Chunk("\x6C", contentfontCheck));
                //phrase.Add(new Chunk("", Heading5));
                //cell = new PdfPCell(phrase);
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.Colspan = 7;
                //cell.PaddingBottom = 2f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 8;
                //cell.PaddingBottom = 2f;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                ////cell.PaddingLeft = 20f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //// baans change 2nd November for the change to the jobhub number
                //phrase.Add(new Chunk("Quote/Invoice No:", Heading4));
                ////phrase.Add(new Chunk("JobHUB Number:", Heading4));
                //// baans end 2nd November
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingBottom = 3f;
                ////cell.PaddingTop = -4;
                //cell.PaddingLeft = 15f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.QuoteNo.ToString(), Heading4));
                //cell = new PdfPCell(phrase);
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.Colspan = 1;
                ////cell.PaddingTop = -4;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Terms:", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingBottom = 3f;
                ////cell.PaddingTop = -4;
                //cell.PaddingLeft = 15f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" 50% Deposit Upfront \n Balance prior to shipping", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.Colspan = 2;
                ////cell.PaddingTop = -4;
                //cell.PaddingBottom = 3f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                // Next Line

                BaseColor BackgroundColor = WebColors.GetRGBColor("#AAE0F6");
                //string TTabImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Content/images/JobStageIndicators-Quote-V2.png");
                //cell = new PdfPCell(iTextSharp.text.Image.GetInstance(TTabImagePath), true);
                //cell.Border = 0;
                
                //headertable1.AddCell(cell);


               
                //phrase = new Phrase();
              
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                
                //cell.Border = 0;
               

                BaseColor TopBarColor = WebColors.GetRGBColor("#28ABE1");
                phrase = new Phrase();
                phrase.Add(new Chunk("[ QUOTE ]", Heading9));
                phrase.Add(new Chunk("        >>>        ", Heading8));
                phrase.Add(new Chunk("INVOICE        >>>        ", Heading8));                
                phrase.Add(new Chunk("DEPOSIT        >>>        ", Heading8));
                phrase.Add(new Chunk("PROOF        >>>        ", Heading8));
                phrase.Add(new Chunk("PRODUCTION        >>>        ", Heading8));
                phrase.Add(new Chunk("COMPLETE        >>>        ", Heading8));
                phrase.Add(new Chunk("PAID        >>>        ", Heading8));
                phrase.Add(new Chunk("SHIPPED", Heading8));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 14;
                cell.PaddingTop = 3f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = TopBarColor;
                headertable3.AddCell(cell);
                //Next Line
                phrase = new Phrase();
                phrase.Add(new Chunk("For your order to be completed by: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));

                string reqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.ReqDate);
                phrase.Add(new Chunk(reqdate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));

                phrase.Add(new Chunk(" a 50% Production Deposit is required by: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));

                string depreqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.DepositReqDate);
                phrase.Add(new Chunk(depreqdate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 15;
                cell.PaddingTop = 3f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = BackgroundColor;
                headertable3.AddCell(cell);




                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -6f;
                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Option No ", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingBottom = 2f;
                cell.PaddingTop = 0f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Qty", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Brand", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 1;
                cell.PaddingLeft = 4f;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Code", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 1;
                cell.PaddingLeft = 4f;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Item", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingLeft = 4f;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Colour", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Link", Heading5));
                phrase.Add(new Chunk("View", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 0f;
                cell.NoWrap = true;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Sizes", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.NoWrap = true;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Front Dec", Heading5)); //Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Front App", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Back Dec", Heading5));//Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Back App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Lft Slv Dec", Heading5));//Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Lft Slv App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Rht Slv Dec", Heading5));//Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Rht Slv App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Other Dec", Heading5));//Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Other App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Other", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Service", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit Ex GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit + GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext Ex GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext + GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -5f;
                cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                doc.Add(headertable1);
                doc.Add(headertable3);
                doc.Add(headertable2);
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfPCell cell;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 800;
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                KENNEWEntities dbContext = new KENNEWEntities();

                var OptionStatus = "Opp";
                var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();

                var FColor2 = new BaseColor(89, 89, 89);   /*tarun 20/09/2018*/

                var FColor3 = new BaseColor(26, 26, 26); /*tarun 20/09/2018*/

                BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/Myriad Pro Regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                iTextSharp.text.Font footerfont1 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont2 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont3 = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont4 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor3);

                Paragraph timelimit = new Paragraph("Quotes valid for 30 days", footerfont1);  /*tarun 20/09/2018*/
                /*Paragraph footer = new Paragraph("TeeCorp Pty Ltd t/a 24 Hour Merchandise", footerfont2);*/                 /*tarun 20/09/2018*/

                /*Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia.    PO Box 7295 Alexandria NSW 2015    ABN 60 130 686 234", footerfont3);*/    /*tarun 20/09/2018*/
                //commented and changed by baans 30Sep2020
                Paragraph footer = new Paragraph("Tuff Tees Pty Ltd t/a 24 Hour Merchandise", footerfont2);
                Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia. PO Box 7295 Alexandria NSW 2015  ABN 81 003 060 633  Ph: 02 9559 2400  Account Manager: " + CustomerDetail.AccountManager, footerfont3);

                Paragraph numbering = new Paragraph("Page 1 of 1", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering2 = new Paragraph("Page 1 of 2", footerfont4);
                Paragraph numbering3 = new Paragraph("Page 2 of 2", footerfont4);
                Paragraph numbering4 = new Paragraph("Page 1 of 3", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering5 = new Paragraph("Page 2 of 3", footerfont4);
                Paragraph numbering6 = new Paragraph("Page 3 of 3", footerfont4);

                //cell = new PdfPCell(timelimit);
                //cell.Border = 0;
                //cell.PaddingLeft = 0;
                //cell.PaddingTop = 50;
                //cell.PaddingBottom = -20;
                //footerTbl.AddCell(cell);
                //footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                //cell = new PdfPCell(footer);
                //cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingTop = 2;        /*tarun 20/09/2018*/
                //footerTbl.AddCell(cell);
                //footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                //cell = new PdfPCell(address);
                //cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingTop = 1;         /*tarun 20/09/2018*/
                //footerTbl.AddCell(cell);
                //footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);


                string StartrackImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Startrack.png");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(StartrackImagePath), true);
                cell.Border = 0;
                cell.PaddingBottom = -38f;
                cell.PaddingTop = 32f;
                cell.FixedHeight = 19;
                footerTbl.AddCell(cell);

                string PayPalImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Credit-Card_PayPal.jpg");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(PayPalImagePath), true);
                cell.Border = 0;
                cell.PaddingBottom = -20f;
                cell.PaddingTop = 15f;
                cell.FixedHeight = 22;
                cell.PaddingLeft = 670;
                footerTbl.AddCell(cell);

                cell = new PdfPCell(footer);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -5;        /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                cell = new PdfPCell(address);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 1;         /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);


                counter++;
                if (OptionData.Count <= 8 && counter == 1)
                {
                    cell = new PdfPCell(numbering);// 1/1
                    cell.Border = 0;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.PaddingTop = 1;
                    footerTbl.AddCell(cell);
                    footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                    counter = 0;
                }
                else if (OptionData.Count > 16)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering4);// 1/1
                        cell.Border = 0;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 2)
                    {
                        cell = new PdfPCell(numbering5);// 1/1
                        cell.Border = 0;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 3)
                    {
                        cell = new PdfPCell(numbering6);// 1/1
                        cell.Border = 0;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }
                else if (OptionData.Count <= 16 && OptionData.Count > 8)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering2);// 1/2
                        cell.Border = 0;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else
                    {
                        cell = new PdfPCell(numbering3);// 2/2
                        cell.Border = 0;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }

            }
        }

        // Order Pdf
        public ActionResult OrderPdf(int id, string PathPdf, string OptionStatus, string QuoteType)
        {
            var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();
            var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

            var AddressinOpp = CustomerDetail.AddressId;

            var dAddress = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.AddressId == AddressinOpp).FirstOrDefault());

            Document doc = new Document(PageSize.A4, -80f, -80f, 20f, 20f);
            doc.SetPageSize(PageSize.A4.Rotate());
            PdfWriter write;
            HeaderFooterStatementforOrder footer;

            if (QuoteType == "Print")
            {
                write = PdfWriter.GetInstance(doc, Response.OutputStream);
                footer = new HeaderFooterStatementforOrder();
                //write.PageEvent = footer;
                var page = new HeaderFooterStatementforOrder();
                page.id = id;
                write.PageEvent = page;
                // write.PageEvent = iTextSharp.text.pdf.IPdfPageEvent.OptionStatus;
                Response.ContentType = ("application/pdf");
            }
            else
            {

                write = PdfWriter.GetInstance(doc, new FileStream(PathPdf, FileMode.Create));
                footer = new HeaderFooterStatementforOrder();
                //write.PageEvent = footer;
                var page = new HeaderFooterStatementforOrder();
                page.id = id;
                write.PageEvent = page;
            }

            doc.Open();

            int i, j, k = 5;
            var rcount = 0;
            var FColor = new BaseColor(38, 171, 227);
            var RColor = new BaseColor(230, 230, 230);
            var FColor2 = new BaseColor(51, 51, 51);
            var FColor3 = new BaseColor(255, 255, 255);

            var Heading1 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, FColor);
            var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 28, iTextSharp.text.Font.NORMAL, FColor);
            var Heading3 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
            var Heading4 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1250, true, 9, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
            var contentfontCheck = FontFactory.GetFont((Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
            var Heading5 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
            var Heading6 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor3);  /*tarun 20/09/2018*/Phrase phrase;
            PdfPCell cell;

            PdfPTable headertable1 = new PdfPTable(15);
            headertable1.SetWidths(new int[] { 5, 5, 5, 5, 7, 1, 6, 7, 7, 7, 7, 3, 5, 5, 5 });
            PdfPTable headertable2 = new PdfPTable(19);
            headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 19, 6, 6, 6, 6, 6, 6, 9, 8, 8, 9, 9 });
            headertable2.SpacingAfter = 2f;
            PdfPTable headertable3 = new PdfPTable(14);
            headertable3.SetWidths(new int[] { 3, 4, 6, 7, 7, 7, 7, 7, 7, 7, 4, 4, 5, 5 });
            //headertable3.SpacingBefore = 50f;


            // Table Data ############################################################################################################

            if (OptionData.Count > 0)
            {
                var CurrentColor = BaseColor.WHITE;

                for (i = 0; i < OptionData.Count; i++)
                {
                    // cell.BackgroundColor = (i % 2) == 0
                    // ? RColor : Color.WHITE;

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (i % 2 == 0)                               // Adding alternate colors to cells
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].OptionNo.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.FixedHeight = 19f;
                    cell.Colspan = 1;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);



                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].Quantity.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.FixedHeight = 19f;
                    cell.Colspan = 1;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].brand.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.FixedHeight = 19f;
                    cell.Colspan = 1;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].code.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].Item.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].colour.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    if (OptionData[i].Link != "N/A")
                    {
                        phrase = new Phrase();
                        //string linkurl = OptionData[i].Link.ToString();
                        //string linkurl2 = "";
                        //if (linkurl.Length > 8)
                        //{
                        //    linkurl2 = linkurl.Substring(0, 8);
                        //}
                        //else
                        //{
                        //    linkurl2 = linkurl;
                        //}
                        //Chunk chunk = new Chunk(linkurl2, FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.UNDERLINE, BaseColor.BLUE));
                        iTextSharp.text.Image ImagePath = iTextSharp.text.Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/maximize.png"));
                        Chunk chunk = new Chunk(ImagePath, 0, 0, true);
                        chunk.SetAnchor(OptionData[i].Link.ToString());
                        phrase.Add(chunk);
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 15f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 3f;
                        //cell.NoWrap = true;
                        cell.PaddingBottom = 5f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);
                    }
                    else
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk("N/A", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        //cell.NoWrap = true;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                    }

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].size.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    //cell.NoWrap = true;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    // baans change 28th November
                    //phrase.Add(new Chunk(OptionData[i].front.ToString(), Heading5));
                    if (OptionData[i].Front_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc1.ToString(), Heading3));
                    }
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 0f;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    //phrase.Add(new Chunk(OptionData[i].back.ToString(), Heading5));
                    if (OptionData[i].Back_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc1.ToString(), Heading3));
                    }
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    //phrase.Add(new Chunk(OptionData[i].leftdec.ToString(), Heading5));
                    if (OptionData[i].Left_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc1.ToString(), Heading3));
                    }
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    //phrase.Add(new Chunk(OptionData[i].rightdec.ToString(), Heading5));
                    if (OptionData[i].Right_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc1.ToString(), Heading3));
                    }
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    //phrase.Add(new Chunk(OptionData[i].other.ToString(), Heading5));
                    if (OptionData[i].Other_Detail_Desc1 != "N/A")
                    {
                        phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc1.ToString(), Heading3));
                        phrase.Add(Chunk.NEWLINE);
                        phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc2.ToString(), Heading3));
                    }
                    else
                    {
                        phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc1.ToString(), Heading3));
                    }
                    // baans end 28th November
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    string decimalvalue = string.Format("{0:C}", OptionData[i].OtherCost.ToString());
                    //converting value to decimal format with Currency sign($).

                    phrase.Add(new Chunk(decimalvalue, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].Service.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);


                    phrase = new Phrase();
                    string decimalvalue1 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].UnitExGST.ToString()), 2));
                    //converting value to decimal format with Currency sign($).

                    phrase.Add(new Chunk(decimalvalue1, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);


                    phrase = new Phrase();
                    string decimalvalue2 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].UnitInclGST.ToString()), 2));
                    phrase.Add(new Chunk(decimalvalue2, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    string decimalvalue3 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].ExtExGST.ToString()), 2));
                    phrase.Add(new Chunk(decimalvalue3, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);


                    phrase = new Phrase();
                    string decimalvalue4 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].ExtInclGST.ToString()), 2));
                    phrase.Add(new Chunk(decimalvalue4, Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 19f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = 0;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -5f;
                    cell.Border = 0;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                        CurrentColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                        CurrentColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    rcount++;
                    if (OptionData.Count > 6 && i != OptionData.Count - 1)
                    {
                        if (i > 0 && rcount % 6 == 0)
                        {
                            doc.Add(headertable2);
                            headertable2.DeleteBodyRows();
                            doc.NewPage();
                            rcount = 0;
                        }
                    }
                }

                for (j = 0; j <= (k - rcount); j++)
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.FixedHeight = 25f;
                    cell.PaddingBottom = 6f;
                    cell.Border = 0;
                    cell.BackgroundColor = RColor;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;

                    }
                    else
                    {
                        cell.BackgroundColor = RColor;

                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                        CurrentColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                        CurrentColor = RColor;
                    }
                    headertable2.AddCell(cell);
                }

            }

            //Bottom Address ##############################################################################

            // baans change 13th December for EFT in the Confirmation pdf

            phrase = new Phrase();
            //phrase.Add(new Chunk("Paying by EFT please enter this number as the payment ID:", Heading2));
            //Commented and changed by baans 11Dec2020 start
            //phrase.Add(new Chunk("Please Note: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, FColor)));
            phrase.Add(new Chunk("Please Note: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 24, iTextSharp.text.Font.NORMAL, FColor)));
            //Commented and changed by baans 11Dec2020 end
            /*phrase.Add(new Chunk("Paying by EFT please enter this number as the Description: Inv"+CustomerDetail.QuoteNo.ToString(), FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 14, iTextSharp.text.Font.NORMAL,BaseColor.RED)));*/
            phrase.Add(new Chunk("Paying by EFT, as the payment description please enter this number: Inv" + CustomerDetail.QuoteNo.ToString(), FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
            cell = new PdfPCell(phrase);
            cell.Colspan = 11;
            cell.Rowspan = 2;
            cell.Border = 0;
            cell.PaddingLeft = -1;
            cell.PaddingTop = 4f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Shipping:", Heading4));
            phrase.Add(new Chunk(" " + CustomerDetail.ShippingBy.ToString(), Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingLeft = 15f;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            string decivalue = string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.ShippingPrice.ToString()), 2));
            phrase.Add(new Chunk(decivalue, Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 5f;
            cell.PaddingRight = -1;
            cell.Border = 0;
            headertable3.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(" ", Heading3));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 11;
            //cell.Border = 0;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.PaddingBottom = -2f;
            //headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Order/Invoice Total:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 9f;
            cell.Border = 0;
            cell.PaddingLeft = 15f;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            // (CustomerDetail.OrderTotal + Convert.ToDecimal(CustomerDetail.ShippingPrice))
            // string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.OrderTotal.ToString()), 2));
            string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.OrderTotal).ToString()), 2));
            phrase.Add(new Chunk(Ordertotal, Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 1f;
            cell.Border = 0;
            cell.PaddingRight = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 11;
            cell.Border = 0;
            cell.PaddingBottom = -2f;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("GST:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingLeft = 15f;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            if (CustomerDetail.GST != null)
            {
                phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.QuoteTotal + CustomerDetail.ShippingPrice) / 11), 2)), Heading4));
            }
            else
            {     
                phrase.Add(new Chunk("$0.00", Heading4));
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.PaddingBottom = 8f;
            cell.PaddingTop = -1;
            cell.Border = 0;
            cell.PaddingRight = -1;
            headertable3.AddCell(cell);

            //Next Row
            phrase = new Phrase();
            phrase.Add(new Chunk("Delivery Address:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            cell.PaddingLeft = -1;
            headertable3.AddCell(cell);

            var Address1 = " ";
            if (AddressinOpp != null)
            {
                Address1 = dAddress.Address1 == null ? "" : dAddress.Address1.ToString();
            }
            else
            {
                Address1 = CustomerDetail.Address1 == "N/A" ? "" : CustomerDetail.Address1.ToString();
            }
            phrase = new Phrase();
            phrase.Add(new Chunk(Address1, Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            cell.PaddingBottom = 8f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("To proceed with this order please make payment to:", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 3;
            cell.PaddingBottom = 8f;
            cell.Border = 0;
            cell.PaddingLeft = -50f;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Bank: ANZ", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 3;
            cell.PaddingBottom = 8f;
            cell.PaddingLeft = -50f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("Order/Invoice Total:", Heading3));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 2;
            //cell.PaddingBottom = 5f;
            //cell.Border = 0;
            //cell.PaddingLeft = 15f;
            //headertable3.AddCell(cell);

            //phrase = new Phrase();
            //// (CustomerDetail.OrderTotal + Convert.ToDecimal(CustomerDetail.ShippingPrice))
            //// string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.OrderTotal.ToString()), 2));
            //string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.OrderTotal + CustomerDetail.ShippingPrice).ToString()), 2));
            //phrase.Add(new Chunk(Ordertotal, Heading3));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 1;
            //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell.PaddingBottom = 5f;
            //cell.Border = 0;
            //headertable3.AddCell(cell);

            //commented and added new cell 29sep2020

            phrase = new Phrase();
            phrase.Add(new Chunk("Final Total:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingLeft = 15f;
            cell.PaddingBottom = 8f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);


            phrase = new Phrase();          
            if (CustomerDetail.FinalTotal != null)
            {
                decimal? FinalTotal = Math.Round(Convert.ToDecimal(CustomerDetail.QuoteTotal + CustomerDetail.ShippingPrice), 2);
                phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(FinalTotal.ToString()), 2)), Heading4));
            }
            else
            {
                phrase.Add(new Chunk("$0.00", Heading4));
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 8f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            cell.PaddingRight = -1;
            headertable3.AddCell(cell);

            //Next Row
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            var Address2 = " ";
            if (AddressinOpp != null)
            {
                Address2 = dAddress.Address2 == null ? "" : dAddress.Address2.ToString();
            }
            else
            {
                Address2 = CustomerDetail.Address2 == "N/A" ? "" : CustomerDetail.Address2.ToString();
            }
            phrase = new Phrase();
            phrase.Add(new Chunk(Address2, Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            cell.PaddingBottom = 8f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Or pay by Visa, Mastercard or PayPal", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 3;
            cell.PaddingLeft = -50f;
            cell.PaddingTop = -1;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Acct Name: Tuff Tees", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 3;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = -50f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Deposit Paid:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingLeft = 15f;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);


            phrase = new Phrase();
            if (CustomerDetail.AmtReceived != null)
            {
                phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.AmtReceived.ToString()), 2)), Heading4));
            }
            else
            {
                phrase.Add(new Chunk("$0.00", Heading4));
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            cell.PaddingRight = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            if (AddressinOpp != null)
            {
                phrase.Add(new Chunk(dAddress.State == null ? "" : dAddress.State.ToString() + "  " + dAddress.Postcode.ToString(), Heading4));
            }
            else
            {
                if (CustomerDetail.Postcode != null)
                {
                    phrase.Add(new Chunk(CustomerDetail.State == "N/A" ? "" : CustomerDetail.State.ToString() + "  " + CustomerDetail.Postcode.ToString(), Heading4));
                }
                else
                {
                    phrase.Add(new Chunk(" ", Heading4));
                }
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Phone 02 9559 2400 for payment", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //cell.PaddingTop = -4f;
            cell.Colspan = 3;
            cell.PaddingLeft = -50f;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            //phrase.Add(new Chunk("BSB: 012351      Acct No.: 401 303 731", Heading3));  //Commented and changed by baans 01Oct2020
            phrase.Add(new Chunk("BSB: 012301         Acct No.:  323 562 825", Heading4));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 3;
            cell.PaddingLeft = -50f;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Balance Due:", Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingLeft = 15f;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            headertable3.AddCell(cell);

            decimal? Balance = 0;
            if (CustomerDetail.AmtReceived != null)
            {

                Balance = Math.Round(Convert.ToDecimal(CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice), 2) - Math.Round(Convert.ToDecimal(CustomerDetail.AmtReceived.ToString()), 2);
            }
            else
            {      
                Balance = Math.Round(Convert.ToDecimal(CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice), 2);
            }
            phrase = new Phrase();
            phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(Balance.ToString()), 2)), Heading4));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.PaddingTop = -1;
            cell.PaddingRight = -1;
            headertable3.AddCell(cell);

            doc.Add(headertable2);
            doc.Add(headertable3);

            doc.Close();
            doc.Dispose();
            if (QuoteType == "Print")
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        // Header/Footer For Order Pdf
        public class HeaderFooterStatementforOrder : PdfPageEventHelper
        {
            int counter = 0;
            public int id { get; set; }
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                KENNEWEntities dbContext = new KENNEWEntities();

                //int id = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["id"]);
                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();

                Rectangle pageSize = doc.PageSize;

                var FColor = new BaseColor(38, 171, 227);  //40, 171, 255        //font color declaration for highlighted data
                var RColor = new BaseColor(230, 230, 230);            //font color declaration for table rows  
                var FColor2 = new BaseColor(51, 51, 51);                /*tarun 20/09/2018*/
                var FColor3 = new BaseColor(38, 38, 38);
                var FColor4 = new BaseColor(255, 255, 255);

                //var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, FColor);
                //Commented and changed 10Dec2020 by baasns start
                //var Heading2 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor);
                //var Heading4 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 32, iTextSharp.text.Font.NORMAL, FColor3);
                var Heading2 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 32, iTextSharp.text.Font.NORMAL, FColor);
                var Heading3 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var Heading4 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1250, true, 9, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var contentfontCheck = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
                var Heading5 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var Heading6 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor3);
                var Heading7 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor);
                var Heading8 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 12, iTextSharp.text.Font.NORMAL, FColor4);
                var Heading9 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 13, iTextSharp.text.Font.BOLD, FColor4);
                var Heading10 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 7, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/


                Phrase phrase;
                PdfPCell cell;

                PdfPTable headertable1 = new PdfPTable(15);
                headertable1.SetWidths(new int[] { 6, 4, 5, 5, 7, 1, 6, 7, 7, 7, 7, 3, 5, 5, 5 });
                PdfPTable headertable2 = new PdfPTable(19);
                headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 19, 6, 6, 6, 6, 6, 6, 9, 8, 8, 9, 9 });
                headertable2.SpacingAfter = 2f;
                PdfPTable headertable3 = new PdfPTable(15);

                string ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Header.jpg");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(ImagePath), true);
                cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -18;
                cell.Colspan = 15;
                cell.FixedHeight = 58;
                cell.PaddingRight = -700;
                cell.PaddingLeft = -28;
                headertable1.AddCell(cell);

                //Commented by baans 29Sep2020 start
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 14;
                //cell.PaddingBottom = 15f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);
                //Commented by baans 29Sep2020 end

                //phrase = new Phrase(); //Commented by baans 28Sep2020 start
                //phrase.Add(new Chunk("Organisation:", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.Border = 0;
                //headertable1.AddCell(cell); //Commented by baans 28Sep2020 end

                phrase = new Phrase();
                phrase.Add(new Chunk("Job Name: ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("CUSTOM MERCHANDISE ORDER", Heading4)); //Commented and changed by baans 28Sep2020
                phrase.Add(new Chunk("CUSTOM MERCHANDISE ", Heading1));
                phrase.Add(new Chunk("TAX INVOICE", Heading2));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 9;
                cell.PaddingBottom = 5f;
                //cell.PaddingTop = -8;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 2f;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingLeft = 20f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Order Date:", Heading3)); //Commented and changed by baans 28Sep2020
                phrase.Add(new Chunk("Order/Invoice Date:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                //cell.PaddingBottom = 1f;
                cell.PaddingTop = 24f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                string datetime2 = string.Format("{0:dd/MM/yyyy}", CustomerDetail.Orderdate);      //setting date format to only show date in desired format
                phrase.Add(new Chunk(datetime2, Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contact: ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 7f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 7f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 9;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable1.AddCell(cell);
              
                phrase = new Phrase();
                // baans change 2nd November for the JobHub Number
                phrase.Add(new Chunk("Order/Invoice No:", Heading4));
                //phrase.Add(new Chunk("JobHUB Number:", Heading3));
                // baans end 2nd November 
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.QuoteNo.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                //Commented by baans 28Sep2020 start
                //phrase = new Phrase();
                //phrase.Add(new Chunk("Job Name:", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingTop = 9f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingTop = 9f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);
                //Commented by baans 28Sep2020 end

                phrase = new Phrase();
                phrase.Add(new Chunk("Organisation:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingTop = 13f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Order Status: ", Heading7));
                phrase.Add(new Chunk("Pending Deposit", Heading6));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.Colspan = 9;
                cell.PaddingBottom = 10f;
                cell.PaddingTop = -15;
                cell.Border = 0;
                headertable1.AddCell(cell);
              
                phrase = new Phrase();
                phrase.Add(new Chunk("Terms:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" 50% Production Deposit \n Balance prior to shipping", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 2;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                //Commented by baans 28Sep2020 start
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading2));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 14;
                //cell.Border = 0;
                //cell.PaddingBottom = -4f;
                //headertable1.AddCell(cell);
                //Commented by baans 28Sep2020 end

                phrase = new Phrase();
                //Commented and changed by baans 28Sep2020 
                //phrase.Add(new Chunk("PRODUCTION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("Not Confirmed", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //Commented and changed by baans 11Dec2020 Start
                //phrase.Add(new Chunk("COMPLETION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("To be Confirmed", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                phrase.Add(new Chunk("COMPLETION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("To be Confirmed", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //Commented and changed by baans 11Dec2020 end
                cell = new PdfPCell(phrase);
                cell.Colspan = 7;
                cell.PaddingBottom = 6f;
                cell.PaddingTop = 3f;
                cell.Border = 0;
                cell.PaddingLeft = -1f;
                headertable1.AddCell(cell);
                //Commented by baans 28Sep2020 start
                //phrase = new Phrase();
                //phrase.Add(new Chunk("For your order to be ready for pickup/dispatch by: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));

                //string reqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.ReqDate);
                //phrase.Add(new Chunk(reqdate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                //cell = new PdfPCell(phrase);
                //cell.HorizontalAlignment = Element.ALIGN_LEFT | Element.ALIGN_BOTTOM;
                //cell.Colspan = 4;
                //cell.PaddingTop = 10f;
                //cell.PaddingBottom = 0f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Your 50% Production Deposit is required by: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));

                //string depreqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.DepositReqDate);
                //phrase.Add(new Chunk(depreqdate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 5;
                //cell.PaddingTop = 10f;
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT | Element.ALIGN_BOTTOM;
                //cell.PaddingBottom = 0f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);
                //Commented by baans 28Sep2020 end

                phrase = new Phrase();
                //commented and changed by baans 10Dec2020 start
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("To proceed please pay the 50% production deposit", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("To proceed please pay the 50% production deposit", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                //commented and changed by baans 10Dec2020 end
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 1f;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);


                phrase = new Phrase();
                //commented and changed by baans 10Dec2020 start
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("To proceed please pay the 50% production deposit", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("To proceed please pay the 50% production deposit", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                //commented and changed by baans 10Dec2020 end
                cell = new PdfPCell(phrase);
                cell.Colspan = 6;
                cell.PaddingBottom = 1f;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                BaseColor BackgroundColor = WebColors.GetRGBColor("#AAE0F6");
                BaseColor TopBarColor = WebColors.GetRGBColor("#28ABE1");
                

                phrase = new Phrase();
                phrase.Add(new Chunk("QUOTE        >>>        ", Heading8));
                phrase.Add(new Chunk("[ INVOICE ]", Heading9));
                phrase.Add(new Chunk("        >>>        ", Heading8));
                phrase.Add(new Chunk("DEPOSIT        >>>        ", Heading8));
                phrase.Add(new Chunk("PROOF        >>>        ", Heading8));
                phrase.Add(new Chunk("PRODUCTION        >>>        ", Heading8));
                phrase.Add(new Chunk("COMPLETE        >>>        ", Heading8));
                phrase.Add(new Chunk("PAID        >>>        ", Heading8));
                phrase.Add(new Chunk("SHIPPED", Heading8));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 15;
                cell.PaddingTop = 3f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = TopBarColor;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("For your order to be completed by: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));

                string reqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.ReqDate);
                phrase.Add(new Chunk(reqdate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));

                phrase.Add(new Chunk(" a 50% Production Deposit is required by: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));

                string depreqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.DepositReqDate);
                phrase.Add(new Chunk(depreqdate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 15;
                cell.PaddingTop = 3f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = BackgroundColor;
                headertable3.AddCell(cell);

                //Commented by baans 28Sep2020 start
                //phrase = new Phrase();
                //phrase.Add(new Chunk("Your 50% Production Deposit is required by: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));

                //string depreqdate = string.Format("{0:dd/MM/yyyy}", CustomerDetail.DepositReqDate);
                //phrase.Add(new Chunk(depreqdate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 5;
                //cell.PaddingTop = 10f;
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT | Element.ALIGN_BOTTOM;
                //cell.PaddingBottom = 0f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);
                //Commented by baans 28Sep2020 end

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading5));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 19;
                //cell.PaddingBottom = -4f;
                //cell.Border = 0;
                //headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -6f;
                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Option No ", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingBottom = 2f;
                cell.PaddingTop = 0f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Qty", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Brand", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Code", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Item", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Colour", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("View", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 0f;
                cell.NoWrap = true;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Sizes", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.NoWrap = true;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Front Dec", Heading5)); //Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Front App", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Back Dec", Heading5));//Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Back App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Lft Slv Dec", Heading5));//Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Lft Slv App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Rht Slv Dec", Heading5));//Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Rht Slv App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Other Dec", Heading5));//Commented and changed by baans 28Sep2020 
                phrase.Add(new Chunk("Other App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Other", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Service", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.PaddingLeft = 4f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit Ex GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit + GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext Ex GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext + GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -5f;
                cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                doc.Add(headertable1);
                doc.Add(headertable3);
                doc.Add(headertable2);
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfPCell cell;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 800;
                //footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                KENNEWEntities dbContext = new KENNEWEntities();

                var OptionStatus = "Order";
                var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();

                //var counter = 0;

                var FColor2 = new BaseColor(89, 89, 89);   /*tarun 20/09/2018*/

                var FColor3 = new BaseColor(26, 26, 26); /*tarun 20/09/2018*/
                //Commented and changed by baans 11Dec2020 start
                //BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/Myriad Pro Regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                //Commented and changed by baans 11Dec2020 end
                //iTextSharp.text.Font footerfont1 = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont2 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont3 = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont4 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor3);

                Paragraph footer = new Paragraph("Tuff Tees Pty Ltd t/a 24 Hour Merchandise", footerfont2);/*tarun 20/09/2018*/
                /*Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia.    PO Box 7295 Alexandria NSW 2015    ABN 60 130 686 234", footerfont3);*/    /*tarun 20/09/2018*/
                Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia. PO Box 7295 Alexandria NSW 2015  ABN 81 003 060 633  Ph: 02 9559 2400  Account Manager: " + CustomerDetail.AccountManager, footerfont3);
                Paragraph numbering = new Paragraph("Page 1 of 1", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering2 = new Paragraph("Page 1 of 2", footerfont4);
                Paragraph numbering3 = new Paragraph("Page 2 of 2", footerfont4);
                Paragraph numbering4 = new Paragraph("Page 1 of 3", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering5 = new Paragraph("Page 2 of 3", footerfont4);
                Paragraph numbering6 = new Paragraph("Page 3 of 3", footerfont4);

                string StartrackImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Startrack.png");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(StartrackImagePath), true);
                cell.Border = 0;
                cell.PaddingBottom = -38f;
                cell.PaddingTop = 32f;
                cell.FixedHeight = 19;
                footerTbl.AddCell(cell);

                string PayPalImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Credit-Card_PayPal.jpg");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(PayPalImagePath), true);
                cell.Border = 0;
                cell.PaddingBottom = -20f;
                cell.PaddingTop = 15f;
                cell.FixedHeight = 22;
                cell.PaddingLeft = 675;
                cell.PaddingRight = -4;
                footerTbl.AddCell(cell);

                cell = new PdfPCell(footer);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -5;
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                cell = new PdfPCell(address);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 1;         /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                counter++;
                if (OptionData.Count <= 5 && counter == 1)
                {
                    cell = new PdfPCell(numbering);// 1/1
                    cell.Border = 0;
                    //cell.PaddingLeft = 50;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.PaddingTop = 1;
                    footerTbl.AddCell(cell);
                    footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                    counter = 0;
                }
                else if (OptionData.Count > 10)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering4);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 2)
                    {
                        cell = new PdfPCell(numbering5);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 3)
                    {
                        cell = new PdfPCell(numbering6);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }
                else if (OptionData.Count <= 10 && OptionData.Count > 5)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering2);// 1/2
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else
                    {
                        cell = new PdfPCell(numbering3);// 2/2
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }


            }
        }

        // Invoice Pdf
        public ActionResult InvoicePdf(int id, string PathPdf, string OptionStatus, string QuoteType)
        {
            try
            {
                OptionStatus = "Order";
                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();
                var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

                var AddressinOpp = CustomerDetail.AddressId;

                var dAddress = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.AddressId == AddressinOpp).FirstOrDefault());

                Document doc = new Document(PageSize.A4, -80f, -80f, 20f, 20f);
                doc.SetPageSize(PageSize.A4.Rotate());
                PdfWriter write;
                HeaderFooterStatementForInvoice footer;
                if (QuoteType == "Print")
                {
                    write = PdfWriter.GetInstance(doc, Response.OutputStream);
                    footer = new HeaderFooterStatementForInvoice();
                    //write.PageEvent = footer;
                    var page = new HeaderFooterStatementForInvoice();
                    page.id = id;
                    write.PageEvent = page;
                    Response.ContentType = ("application/pdf");
                }
                else
                {

                    write = PdfWriter.GetInstance(doc, new FileStream(PathPdf, FileMode.Create));
                    footer = new HeaderFooterStatementForInvoice();
                    //write.PageEvent = footer;
                    var page = new HeaderFooterStatementForInvoice();
                    page.id = id;
                    write.PageEvent = page;
                }

                doc.Open();

                int i, j, k = 5;
                var rcount = 0;
                var FColor = new BaseColor(38, 171, 227);
                var RColor = new BaseColor(230, 230, 230);
                var FColor2 = new BaseColor(51, 51, 51);
                var FColor3 = new BaseColor(255, 255, 255);
                //var FColor3 = new BaseColor(26, 26, 26);

                var Heading1 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, FColor);
                var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 28, iTextSharp.text.Font.NORMAL, FColor);
                var Heading3 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading4 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1250, true, 9, iTextSharp.text.Font.NORMAL, FColor2);
                var contentfontCheck = FontFactory.GetFont((Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
                var Heading5 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading6 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor3);

                Phrase phrase;
                PdfPCell cell;

                PdfPTable headertable1 = new PdfPTable(15);
                headertable1.SetWidths(new int[] { 5, 5, 5, 5, 7, 1, 6, 7, 7, 7, 7, 3, 5, 5, 5 });
                PdfPTable headertable2 = new PdfPTable(19);
                headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 19, 6, 6, 6, 6, 6, 6, 9, 8, 8, 9, 9 });
                headertable2.SpacingAfter = 2f;
                PdfPTable headertable3 = new PdfPTable(14);
                headertable3.SetWidths(new int[] { 3, 4, 6, 7, 7, 7, 7, 7, 7, 7, 3, 5, 5, 5 });

                // Table Data ############################################################################################################

                if (OptionData.Count > 0)
                {
                    var CurrentColor = BaseColor.WHITE;

                    for (i = 0; i < OptionData.Count; i++)
                    {
                        // cell.BackgroundColor = (i % 2) == 0
                        // ? RColor : Color.WHITE;

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.PaddingBottom = -6f;
                        cell.Border = 0;
                        if (i % 2 == 0)                               // Adding alternate colors to cells
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].OptionNo.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 16f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);



                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].Quantity.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].brand.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].code.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].Item.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].colour.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        if (OptionData[i].Link != "N/A")
                        {
                            phrase = new Phrase();
                            string linkurl = OptionData[i].Link.ToString();
                            //string linkurl2 = "";
                            //if (linkurl.Length > 8)
                            //{
                            //    linkurl2 = linkurl.Substring(0, 8);
                            //}
                            //else
                            //{
                            //    linkurl2 = linkurl;
                            //}
                            //Chunk chunk = new Chunk(linkurl2, FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.UNDERLINE, BaseColor.BLUE));
                            //chunk.SetAnchor(OptionData[i].Link.ToString());
                            ////phrase.Add(new Chunk(OptionData[i].Link.ToString(), FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 7, iTextSharp.text.Font.UNDERLINE, BaseColor.BLUE)));
                            Image ImagePath = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/maximize.png"));
                            Chunk chunk = new Chunk(ImagePath, 0, 0, true);
                            phrase.Add(chunk);
                            cell = new PdfPCell(phrase);
                            cell.Colspan = 1;
                            cell.FixedHeight = 19f;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.PaddingTop = 3f;
                            //cell.NoWrap = true;
                            cell.PaddingBottom = 6f;
                            cell.Border = PdfPCell.RIGHT_BORDER;
                            if (i % 2 == 0)
                            {
                                cell.BackgroundColor = BaseColor.WHITE;
                            }
                            else
                            {
                                cell.BackgroundColor = RColor;
                            }
                            headertable2.AddCell(cell);
                        }
                        else
                        {
                            phrase = new Phrase();
                            phrase.Add(new Chunk("N/A", Heading3));
                            cell = new PdfPCell(phrase);
                            cell.Colspan = 1;
                            cell.FixedHeight = 19f;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.PaddingTop = 0f;
                            //cell.NoWrap = true;
                            cell.PaddingBottom = 0f;
                            cell.Border = PdfPCell.RIGHT_BORDER;
                            if (i % 2 == 0)
                            {
                                cell.BackgroundColor = BaseColor.WHITE;
                            }
                            else
                            {
                                cell.BackgroundColor = RColor;
                            }
                            headertable2.AddCell(cell);
                        }

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].size.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        //cell.NoWrap = true;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        // baans change 28th November
                        //phrase.Add(new Chunk(OptionData[i].front.ToString(), Heading5));
                        if (OptionData[i].Front_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc1.ToString(), Heading3));
                        }
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        //phrase.Add(new Chunk(OptionData[i].back.ToString(), Heading5));
                        if (OptionData[i].Back_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc1.ToString(), Heading3));
                        }
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        //phrase.Add(new Chunk(OptionData[i].leftdec.ToString(), Heading5));
                        if (OptionData[i].Left_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc1.ToString(), Heading3));
                        }
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        //phrase.Add(new Chunk(OptionData[i].rightdec.ToString(), Heading5));
                        if (OptionData[i].Right_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc1.ToString(), Heading3));
                        }
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        //phrase.Add(new Chunk(OptionData[i].other.ToString(), Heading5));
                        if (OptionData[i].Other_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc1.ToString(), Heading3));
                        }
                        // baans end 28th November
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        string decimalvalue = string.Format("{0:C}", OptionData[i].OtherCost.ToString());
                        //converting value to decimal format with Currency sign($).

                        phrase.Add(new Chunk(decimalvalue, Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].Service.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);


                        phrase = new Phrase();
                        string decimalvalue1 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].UnitExGST.ToString()), 2));
                        //converting value to decimal format with Currency sign($).

                        phrase.Add(new Chunk(decimalvalue1, Heading3));
                        //phrase.Add(new Chunk('$' + dt_data.Rows[i]["UnitExGST"].ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);


                        phrase = new Phrase();
                        string decimalvalue2 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].UnitInclGST.ToString()), 2));
                        phrase.Add(new Chunk(decimalvalue2, Heading3));
                        //phrase.Add(new Chunk('$' + dt_data.Rows[i]["UnitInclGST"].ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        string decimalvalue3 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].ExtExGST.ToString()), 2));
                        phrase.Add(new Chunk(decimalvalue3, Heading3));
                        //phrase.Add(new Chunk('$' + dt_data.Rows[i]["ExtExGST"].ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);


                        phrase = new Phrase();
                        string decimalvalue4 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].ExtInclGST.ToString()), 2));
                        phrase.Add(new Chunk(decimalvalue4, Heading3));
                        //phrase.Add(new Chunk('$'+dt_data.Rows[i]["ExtInclGST"].ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = 0;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading5));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.PaddingBottom = -5f;
                        cell.Border = 0;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                            CurrentColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                            CurrentColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        rcount++;
                        if (OptionData.Count > 6 && i != OptionData.Count - 1)
                        {
                            if (i > 0 && rcount % 6 == 0)
                            {
                                doc.Add(headertable2);
                                headertable2.DeleteBodyRows();
                                doc.NewPage();
                                rcount = 0;
                            }
                        }
                    }

                    for (j = 0; j <= (k - rcount); j++)
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading5));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.PaddingBottom = -6f;
                        cell.Border = 0;
                        if (CurrentColor == RColor)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.FixedHeight = 22f;
                        cell.PaddingBottom = 6f;
                        cell.Border = 0;
                        cell.BackgroundColor = RColor;
                        if (CurrentColor == RColor)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;

                        }
                        else
                        {
                            cell.BackgroundColor = RColor;

                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.PaddingBottom = -6f;
                        cell.Border = 0;
                        if (CurrentColor == RColor)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                            CurrentColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                            CurrentColor = RColor;
                        }
                        headertable2.AddCell(cell);
                    }
                }

                //Address ##############################################################################

                // baans change 13th December for EFT in the Confirmation pdf
                //Row
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 14;
                //cell.Border = 0;
                //cell.PaddingBottom = -5f;
                //headertable3.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                //commented and change by baans 11Dec2020 start
                //phrase.Add(new Chunk("Please Note: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("Please Note: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 24, iTextSharp.text.Font.NORMAL, FColor)));
                //commented and change by baans 11Dec2020 end
                //phrase.Add(new Chunk("Paying by EFT please enter this number as the payment ID:", Heading2));
                /*phrase.Add(new Chunk("Paying by EFT please enter this number as the Description: Inv" + CustomerDetail.QuoteNo.ToString(), FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 14, iTextSharp.text.Font.NORMAL, BaseColor.RED)));*/
                //commentd and changed by baans 01Oct2020
                phrase.Add(new Chunk("Your order is complete! Thank you for choosing 24 Hour Merchandise", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 11;
                cell.Rowspan = 2;
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.PaddingTop = 4f;
                cell.PaddingLeft = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Shipping:", Heading4));
                phrase.Add(new Chunk(" " + CustomerDetail.ShippingBy.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingLeft = 25f;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                string decivalue = string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.ShippingPrice.ToString()), 2));
                phrase.Add(new Chunk(decivalue, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 5f;
                cell.PaddingRight = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Order/Invoice Total:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 9f;
                cell.Border = 0;
                cell.PaddingLeft = 25f;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                // (CustomerDetail.OrderTotal + Convert.ToDecimal(CustomerDetail.ShippingPrice))
                // string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.OrderTotal.ToString()), 2));
                string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.OrderTotal).ToString()), 2));
                phrase.Add(new Chunk(Ordertotal, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 1f;
                cell.PaddingRight = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 11;
                cell.PaddingBottom = -2f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("GST:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingLeft = 25f;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                if (CustomerDetail.GST != null)
                {
                    phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.QuoteTotal + CustomerDetail.ShippingPrice) / 11), 2)), Heading4));
                }
                else
                {
                    phrase.Add(new Chunk("$0.00", Heading4));
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.PaddingBottom = 8f;
                cell.PaddingRight = -1;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 14;
                //cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingBottom = -2f;
                //headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Delivery Address:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                cell.PaddingLeft = -1;
                headertable3.AddCell(cell);

                var Address1 = " ";
                if (AddressinOpp != null)
                {
                    Address1 = dAddress.Address1 == null ? "" : dAddress.Address1.ToString();
                }
                else
                {
                    Address1 = CustomerDetail.Address1 == "N/A" ? "" : CustomerDetail.Address1.ToString();
                }
                phrase = new Phrase();
                phrase.Add(new Chunk(Address1, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("To finalise this Invoice please make payment to:", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingLeft = -50f;
                cell.Colspan = 3;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Bank: ANZ", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.PaddingLeft = -50f;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Final Total:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                cell.PaddingLeft = 25f;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                if (CustomerDetail.FinalTotal != null)
                {
                    phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice).ToString()), 2)), Heading4));
                }
                else
                {
                    phrase.Add(new Chunk("$0.00", Heading4));
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                cell.PaddingRight = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                var Address2 = " ";
                if (AddressinOpp != null)
                {
                    Address2 = dAddress.Address2 == null ? "" : dAddress.Address2.ToString();
                }
                else
                {
                    Address2 = CustomerDetail.Address2 == "N/A" ? "" : CustomerDetail.Address2.ToString();
                }
                phrase = new Phrase();
                phrase.Add(new Chunk(Address2, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Or pay by Visa, Mastercard or PayPal", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.PaddingTop = -1;
                cell.PaddingLeft = -50f;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Acct Name: Tuff Tees", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = -50f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Deposit Paid:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingLeft = 25f;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);


                phrase = new Phrase();
                if (CustomerDetail.AmtReceived != null)
                {
                    phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.AmtReceived.ToString()), 2)), Heading4));
                }
                else
                {
                    phrase.Add(new Chunk("$0.00", Heading4));
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                if (AddressinOpp != null)
                {
                    phrase.Add(new Chunk(dAddress.State == null ? "" : dAddress.State.ToString() + "  " + dAddress.Postcode.ToString(), Heading4));
                }
                else
                {
                    if (CustomerDetail.Postcode != null)
                    {
                        phrase.Add(new Chunk(CustomerDetail.State == "N/A" ? "" : CustomerDetail.State.ToString() + "  " + CustomerDetail.Postcode.ToString(), Heading4));
                    }
                    else
                    {
                        phrase.Add(new Chunk(" ", Heading4));
                    }
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Phone 02 9559 2400 for payment", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingTop = -4f;
                cell.Colspan = 3;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = -50f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("BSB: 012351      Acct No.: 401 303 731", Heading3)); 
                //commented and changed by baans 01Oct2020
                phrase.Add(new Chunk("BSB: 012301      Acct No.: 323 562 825", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = -50f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Balance Due:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingLeft = 25f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                decimal? Balance = 0;
                if (CustomerDetail.AmtReceived != null)
                {

                    Balance = Math.Round(Convert.ToDecimal(CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice), 2) - Math.Round(Convert.ToDecimal(CustomerDetail.AmtReceived.ToString()), 2);
                }
                else
                {
                    Balance = Math.Round(Convert.ToDecimal(CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice), 2);
                }
                phrase = new Phrase();
                phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(Balance.ToString()), 2)), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 5f;
                cell.PaddingRight = -1;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                //doc.Add(headertable1);
                doc.Add(headertable2);
                doc.Add(headertable3);

                doc.Close();
                doc.Dispose();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = ResponseType.Error;
            }
            if (QuoteType == "Print")
            {

                return View();
            }
            else
            {
                return null;
            }
        }

        // Header/Footer For Invoice Pdf 
        public class HeaderFooterStatementForInvoice : PdfPageEventHelper
        {
            int counter = 0;
            public int id { get; set; }
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                KENNEWEntities dbContext = new KENNEWEntities();

                //int id = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["id"]);
                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();
                var ProductionDate = dbContext.tblOpportunities.Where(_ => _.OpportunityId == id).Select(_ => _.InvoicingDate).FirstOrDefault();

                Rectangle pageSize = doc.PageSize;

                var FColor = new BaseColor(38, 171, 227); 
                var RColor = new BaseColor(230, 230, 230);
                var FColor2 = new BaseColor(51, 51, 51);
                var FColor3 = new BaseColor(38, 38, 38);
                var FColor4 = new BaseColor(255, 255, 255);

                var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 32, iTextSharp.text.Font.NORMAL, FColor3);
                var Heading2 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 32, iTextSharp.text.Font.NORMAL, FColor);
                var Heading3 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var Heading4 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1250, true, 9, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var contentfontCheck = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
                var Heading5 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
                var Heading6 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor3);
                var Heading7 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor);
                var Heading8 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 12, iTextSharp.text.Font.NORMAL, FColor4);
                var Heading9 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 13, iTextSharp.text.Font.BOLD, FColor4);
                var Heading10 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 7, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/

                Phrase phrase;
                PdfPCell cell;

                PdfPTable headertable1 = new PdfPTable(15);
                headertable1.SetWidths(new int[] { 6, 4, 5, 5, 7, 1, 6, 7, 7, 7, 7, 3, 5, 5, 5 });
                PdfPTable headertable2 = new PdfPTable(19);
                headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 19, 6, 6, 6, 6, 6, 6, 9, 8, 8, 9, 9 });
                headertable2.SpacingAfter = 2f;
                PdfPTable headertable3 = new PdfPTable(16);

                string ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Header.jpg");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(ImagePath), true);
                cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -18;
                cell.Colspan = 15;
                cell.FixedHeight = 58;
                cell.PaddingRight = -700;
                cell.PaddingLeft = -28;
                headertable1.AddCell(cell);

                //next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Job Name: ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading3));//Commented and chaged by baans 01Oct2020
                //phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.Border = 0;
                //cell.PaddingTop = 12f;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("CUSTOM MERCHANDISE", Heading1));
                phrase.Add(new Chunk(" TAX INVOICE", Heading2));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 9;
                cell.PaddingBottom = 5f;                
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 2f;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingLeft = 20f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Order/Invoice Date:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                cell.PaddingTop = 24f;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                string datetime2 = string.Format("{0:dd/MM/yyyy}", ProductionDate);      //setting date format to only show date in desired format
                phrase.Add(new Chunk(datetime2, Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingTop = 24f;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contact: ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 7f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                // cell.PaddingTop = 9f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 7f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                // cell.PaddingTop = 9f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.Border = 0;
                //cell.PaddingTop = 12f;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 9;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 2f;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingLeft = 20f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Order/Invoice No:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 15f;
                cell.Border = 0;

                /*cell.PaddingTop = 12f;*/
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.QuoteNo.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingRight = -1;
                cell.Border = 0;
                /*cell.PaddingTop = -1f;*/
                headertable1.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Organisation:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                //cell.PaddingTop = 5;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                //cell.PaddingTop = 5;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                ////phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading3)); //Commented and changed by baans 01Oct2020
                //phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingTop = -4f;
                //cell.Border = 0;
                //cell.PaddingTop = 12f;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Order Status: ", Heading7));
                phrase.Add(new Chunk("Complete!", Heading6));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.Colspan = 9;
                cell.PaddingTop = -15f;
                cell.PaddingBottom = 10f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 2f;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                ////cell.PaddingLeft = 20f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Terms:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                //cell.PaddingTop = -4f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                //cell.PaddingTop = 12f;
                cell.PaddingLeft = 15f;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("50% Production Deposit \nBalance prior to shipping", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                //cell.PaddingTop = -4f;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                cell.PaddingLeft = 5f;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                //Commented by baans 01Oct2020 Start
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading2));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 14;
                //cell.Border = 0;
                ////cell.PaddingBottom = 6f;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading2));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 4;
                //cell.Border = 0;
                //cell.PaddingBottom = -8f;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("To avoid shipping delays please pay your balance", Heading1));
                //cell = new PdfPCell(phrase);
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                ////cell.PaddingLeft = 0f;
                //cell.PaddingTop = -17f;
                //cell.Colspan = 6;
                //cell.Border = 0;
                //cell.PaddingBottom = -2f;
                //headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading2));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 4;
                //cell.Border = 0;
                //cell.PaddingBottom = -8f;
                //headertable1.AddCell(cell);
                //Commented by baans 01Oct2020 end

                phrase = new Phrase();
                //Commented and changed by  baans 11Dec2020 Start
                //phrase.Add(new Chunk("COMPLETION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("Complete", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                phrase.Add(new Chunk("COMPLETION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("Complete", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //Commented and changed by  baans 11Dec2020 end
                cell = new PdfPCell(phrase);
                cell.Colspan = 7;
                cell.PaddingBottom = 6f;
                cell.Border = 0;
                cell.PaddingLeft = -1f;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("To proceed please pay the 50% production deposit", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 1f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("For quick shipping, please pay any invoice balance due", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 14, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 6;
                cell.PaddingBottom = 1f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                headertable1.AddCell(cell);

                BaseColor BackgroundColor = WebColors.GetRGBColor("#AAE0F6");
                //string TTabImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Content/images/JobStageIndicators-Invoiced-V2.png");
                //cell = new PdfPCell(iTextSharp.text.Image.GetInstance(TTabImagePath), true);
                //cell.Border = 0;
                //cell.PaddingTop = -1f;
                //cell.Colspan = 14;
                //cell.FixedHeight = 30;
                //cell.BackgroundColor = BackgroundColor;
                //cell.PaddingRight = -1;
                //cell.PaddingLeft = -1;
                //headertable1.AddCell(cell);

                BaseColor TopBarColor = WebColors.GetRGBColor("#28ABE1");
                

                phrase = new Phrase();
                phrase.Add(new Chunk("QUOTE        >>>        ", Heading8));
                phrase.Add(new Chunk("INVOICE        >>>        ", Heading8));
                phrase.Add(new Chunk("DEPOSIT        >>>        ", Heading8));
                phrase.Add(new Chunk("PROOF        >>>        ", Heading8));
                phrase.Add(new Chunk("PRODUCTION        >>>        ", Heading8));
                phrase.Add(new Chunk("[ COMPLETE ]", Heading9));
                phrase.Add(new Chunk("        >>>        ", Heading8));
                phrase.Add(new Chunk("PAID        >>>        ", Heading8));
                phrase.Add(new Chunk("SHIPPED", Heading8));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 16;
                cell.PaddingTop = 3f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = TopBarColor;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Your order is Complete. Thank you for choosing 24 Hour Merchandise", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 7;
                cell.PaddingTop = 3f;
                cell.PaddingLeft = 2f;
                cell.PaddingRight = -2;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = BackgroundColor;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("If your Invoice is fully paid for your order will be automatically released for Pick Up or Shipping", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 9;
                cell.PaddingTop = 3f;
                cell.PaddingRight = 2f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = BackgroundColor;
                headertable3.AddCell(cell);

                // Order Detail Table ###############################################################################################

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading5));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 19;
                //cell.PaddingBottom = -16f;
                //cell.PaddingTop = -16f;
                //cell.Border = 0;
                //headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -6f;
                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Option No ", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingBottom = 2f;
                cell.PaddingTop = 0f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Qty", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Brand", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Code", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.PaddingLeft = 4f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Item", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.PaddingLeft = 4f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Colour", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("View", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 0f;
                cell.NoWrap = true;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Sizes", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.NoWrap = true;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Front Dec", Heading5)); //commented and changed by baans 01Oct2020
                phrase.Add(new Chunk("Front App", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Back Dec", Heading5)); //commented and changed by baans 01Oct2020
                phrase.Add(new Chunk("Back App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Lft Slv Dec", Heading5));//Commentd and changed by baans 01Oct2020
                phrase.Add(new Chunk("Lft Slv App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Rht Slv Dec", Heading5));//commented and changed by baans 01Oct2020
                phrase.Add(new Chunk("Rht Slv App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Other Dec", Heading5)); //commented and changed by baans 01Oct2020
                phrase.Add(new Chunk("Other App", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Other", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Service", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.PaddingLeft = 4f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit Ex GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit + GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext Ex GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext + GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingTop = -4f;
                cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                doc.Add(headertable1);
                doc.Add(headertable3);
                doc.Add(headertable2);
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfPCell cell;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 800;
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                KENNEWEntities dbContext = new KENNEWEntities();

                var OptionStatus = "Order";
                var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();

                var FColor2 = new BaseColor(89, 89, 89);   /*tarun 20/09/2018*/

                var FColor3 = new BaseColor(26, 26, 26); /*tarun 20/09/2018*/
                //Commented and changed by baans 11Dec2020 start
                //BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/Myriad Pro Regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                //Commented and changed by baans 11Dec2020 end
                //iTextSharp.text.Font footerfont1 = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont2 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont3 = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont4 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor3);

                //Paragraph footer = new Paragraph("TeeCorp Pty Ltd t/a 24 Hour Merchandise", footerfont2);/*tarun 20/09/2018*/
                //Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia.    PO Box 7295 Alexandria NSW 2015    ABN 60 130 686 234", footerfont3);    /*tarun 20/09/2018*/
                Paragraph footer = new Paragraph("Tuff Tees Pty Ltd t/a 24 Hour Merchandise", footerfont2);
                Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia. PO Box 7295 Alexandria NSW 2015 ABN 81 003 060 633 Ph: 02 9559 2400 Account Manager: " + CustomerDetail.AccountManager, footerfont3);
                Paragraph numbering = new Paragraph("Page 1 of 1", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering2 = new Paragraph("Page 1 of 2", footerfont4);
                Paragraph numbering3 = new Paragraph("Page 2 of 2", footerfont4);
                Paragraph numbering4 = new Paragraph("Page 1 of 3", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering5 = new Paragraph("Page 2 of 3", footerfont4);
                Paragraph numbering6 = new Paragraph("Page 3 of 3", footerfont4);


                string StartrackImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Startrack.png");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(StartrackImagePath), true);
                cell.Border = 0;
                cell.PaddingBottom = -38f;
                cell.PaddingTop = 32f;
                cell.FixedHeight = 19;
                footerTbl.AddCell(cell);

                string PayPalImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Credit-Card_PayPal.jpg");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(PayPalImagePath), true);
                cell.Border = 0;
                cell.PaddingBottom = -20f;
                cell.PaddingTop = 15f;
                cell.FixedHeight = 22;
                cell.PaddingLeft = 675;
                cell.PaddingRight = -1;
                footerTbl.AddCell(cell);

                cell = new PdfPCell(footer);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -5;        /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                cell = new PdfPCell(address);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 1;         /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                counter++;
                if (OptionData.Count <= 5 && counter == 1)
                {
                    cell = new PdfPCell(numbering);// 1/1
                    cell.Border = 0;
                    //cell.PaddingLeft = 50;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.PaddingTop = 1;
                    footerTbl.AddCell(cell);
                    footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                    counter = 0;
                }
                else if (OptionData.Count > 10)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering4);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 2)
                    {
                        cell = new PdfPCell(numbering5);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 3)
                    {
                        cell = new PdfPCell(numbering6);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }
                else if (OptionData.Count <= 10 && OptionData.Count > 5)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering2);// 1/2
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else
                    {
                        cell = new PdfPCell(numbering3);// 2/2
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }

            }
        }

        // Packing pdf
        public ActionResult PackagingPdf(int id, string PathPdf, string OptionStatus, string QuoteType)
        {
            OptionStatus = "Order";
            var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();
            var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

            var AddressinOpp = CustomerDetail.AddressId;

            var dAddress = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.AddressId == AddressinOpp).FirstOrDefault());

            Document doc = new Document(PageSize.A4, -80f, -80f, 20f, 20f);
            doc.SetPageSize(PageSize.A4.Rotate());
            PdfWriter write;
            HeaderFooterStatementForPacking footer;
            if (QuoteType == "Print")
            {
                write = PdfWriter.GetInstance(doc, Response.OutputStream);
                footer = new HeaderFooterStatementForPacking();
                //write.PageEvent = footer;
                var page = new HeaderFooterStatementForPacking();
                page.id = id;
                write.PageEvent = page;
                Response.ContentType = ("application/pdf");
            }
            else
            {

                write = PdfWriter.GetInstance(doc, new FileStream(PathPdf, FileMode.Create));
                footer = new HeaderFooterStatementForPacking();
                //write.PageEvent = footer;
                var page = new HeaderFooterStatementForPacking();
                page.id = id;
                write.PageEvent = page;
            }

            doc.Open();

            int i, j, k = 6;
            var rcount = 0;
            var FColor = new BaseColor(26, 26, 26);             //font color declaration for highlighted data
            var RColor = new BaseColor(230, 230, 230);            //font color declaration for table rows background 
            var FColor2 = new BaseColor(51, 51, 51);

            var Heading1 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 42, iTextSharp.text.Font.NORMAL, FColor);
            //var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 27, iTextSharp.text.Font.NORMAL, FColor);
            var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 27, iTextSharp.text.Font.NORMAL, FColor);
            //var Heading4 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 27, iTextSharp.text.Font.NORMAL, FColor2);
            var Heading3 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor2);
            var Heading5 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, FColor2);

            Phrase phrase;
            PdfPCell cell;

            PdfPTable headertable1 = new PdfPTable(14);
            headertable1.SetWidths(new int[] { 4, 4, 6, 6, 7, 7, 7, 7, 7, 7, 6, 4, 4, 6 });
            PdfPTable headertable2 = new PdfPTable(19);
            headertable2.SetWidths(new int[] { 10, 7, 10, 10, 14, 12, 10, 7, 7, 7, 7, 7, 7, 6, 7, 6, 6, 6, 10 });
            PdfPTable headertable3 = new PdfPTable(14);
            headertable3.SetWidths(new int[] { 4, 4, 6, 6, 7, 7, 7, 7, 7, 7, 4, 5, 5, 6 });


            // Table Data ############################################################################################################

            if (OptionData.Count > 0)
            {
                var CurrentColor = BaseColor.WHITE;

                for (i = 0; i < OptionData.Count; i++)
                {
                    // cell.BackgroundColor = (i % 2) == 0
                    // ? RColor : Color.WHITE;

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (i % 2 == 0)                               // Adding alternate colors to cells
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].OptionNo.ToString(), Heading5));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 26f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);



                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].Quantity.ToString(), Heading5));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 26f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].brand.ToString(), Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 26f;
                    cell.PaddingTop = 0f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].code.ToString(), Heading5));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 26f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].Item.ToString(), Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 26f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].colour.ToString(), Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 26f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    if (OptionData[i].Link != "N/A")
                    {
                        phrase = new Phrase();
                        string linkurl = OptionData[i].Link.ToString();
                        string linkurl2 = "";
                        if (linkurl.Length > 9)
                        {
                            linkurl2 = linkurl.Substring(0, 9);
                        }
                        else
                        {
                            linkurl2 = linkurl;
                        }
                        Chunk chunk = new Chunk(linkurl2, FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 7, iTextSharp.text.Font.UNDERLINE, BaseColor.BLUE));
                        chunk.SetAnchor(OptionData[i].Link.ToString());
                        //phrase.Add(new Chunk(OptionData[i].Link.ToString(), FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 7, iTextSharp.text.Font.UNDERLINE, BaseColor.BLUE)));
                        phrase.Add(chunk);
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 26f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        //cell.NoWrap = true;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);
                    }
                    else
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk("N/A", Heading5));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 26f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        //cell.NoWrap = true;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);
                    }

                    phrase = new Phrase();
                    phrase.Add(new Chunk(OptionData[i].sizepacked.ToString(), Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 11;
                    cell.FixedHeight = 26f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingTop = 0f;
                    //cell.NoWrap = true;
                    cell.PaddingBottom = 0f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.FixedHeight = 26f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = 0;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (i % 2 == 0)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                        CurrentColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                        CurrentColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    rcount++;
                    if (OptionData.Count > 7 && i != OptionData.Count - 1)
                    {
                        if (i > 0 && rcount % 7 == 0)
                        {
                            doc.Add(headertable2);
                            headertable2.DeleteBodyRows();
                            doc.NewPage();
                            rcount = 0;
                        }
                    }
                }

                for (j = 0; j <= (k - rcount); j++)
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.FixedHeight = 26f;
                    cell.PaddingBottom = 6f;
                    cell.Border = 0;
                    cell.BackgroundColor = RColor;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;

                    }
                    else
                    {
                        cell.BackgroundColor = RColor;

                    }
                    headertable2.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 19;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    if (CurrentColor == RColor)
                    {
                        cell.BackgroundColor = BaseColor.WHITE;
                        CurrentColor = BaseColor.WHITE;
                    }
                    else
                    {
                        cell.BackgroundColor = RColor;
                        CurrentColor = RColor;
                    }
                    headertable2.AddCell(cell);
                }
            }

            ///Bottom Address Part ###########################################################################################

            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 14;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = -2f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Delivery Address:", Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.PaddingBottom = 3f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            var Address1 = " ";
            if (AddressinOpp != null)
            {
                Address1 = dAddress.Address1 == null ? "" : dAddress.Address1.ToString();
            }
            else
            {
                Address1 = CustomerDetail.Address1 == "N/A" ? "" : CustomerDetail.Address1.ToString();
            }
            phrase = new Phrase();
            phrase.Add(new Chunk(Address1, Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 4;
            cell.PaddingBottom = 3f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 12, iTextSharp.text.Font.NORMAL)));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 8;
            cell.PaddingBottom = 3f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            //Next Line
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            var Address2 = " ";
            if (AddressinOpp != null)
            {
                Address2 = dAddress.Address2 == null ? "" : dAddress.Address2.ToString();
            }
            else
            {
                Address2 = CustomerDetail.Address2 == "N/A" ? "" : CustomerDetail.Address2.ToString();
            }
            phrase = new Phrase();
            phrase.Add(new Chunk(Address2, Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 4;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL)));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 8;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            //Next Line
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading3));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            if (AddressinOpp != null)
            {
                phrase.Add(new Chunk(dAddress.State == null ? "" : dAddress.State.ToString() + "  " + dAddress.Postcode.ToString(), Heading3));
            }
            else
            {
                if (CustomerDetail.Postcode != null)
                {
                    phrase.Add(new Chunk(CustomerDetail.State == "N/A" ? "" : CustomerDetail.State.ToString() + "  " + CustomerDetail.Postcode.ToString(), Heading3));
                }
                else
                {
                    phrase.Add(new Chunk(" ", Heading3));
                }
            }
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("", Heading3));
            cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 10;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable3.AddCell(cell);

            doc.Add(headertable1);
            doc.Add(headertable2);
            doc.Add(headertable3);

            doc.Close();
            doc.Dispose();
            if (QuoteType == "Print")
            {

                return View();
            }
            else
            {
                return null;
            }
        }

        // Header/Footer For Packing Pdf
        public class HeaderFooterStatementForPacking : PdfPageEventHelper
        {
            int counter = 0;
            public int id { get; set; }
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                KENNEWEntities dbContext = new KENNEWEntities();

                int id = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["id"]);
                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();

                Rectangle pageSize = doc.PageSize;

                var FColor = new BaseColor(26, 26, 26);
                var RColor = new BaseColor(230, 230, 230);            //font color declaration for table rows background 
                var FColor2 = new BaseColor(51, 51, 51);

                var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 42, iTextSharp.text.Font.NORMAL, FColor);
                //var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 27, iTextSharp.text.Font.NORMAL, FColor);
                var Heading2 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 27, iTextSharp.text.Font.NORMAL, FColor);
                //var Heading4 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 27, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading3 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 9, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading5 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor2);

                Phrase phrase;
                PdfPCell cell;

                PdfPTable headertable1 = new PdfPTable(14);
                headertable1.SetWidths(new int[] { 4, 4, 6, 6, 7, 7, 7, 7, 7, 7, 6, 4, 4, 6 });
                PdfPTable headertable2 = new PdfPTable(19);
                headertable2.SetWidths(new int[] { 10, 7, 10, 10, 14, 12, 10, 7, 7, 7, 7, 7, 7, 6, 7, 6, 6, 6, 10 });
                PdfPTable headertable3 = new PdfPTable(14);
                headertable3.SetWidths(new int[] { 5, 5, 5, 5, 7, 7, 7, 7, 7, 7, 4, 5, 5, 6 });


                string ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Packing Heading.PNG");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(ImagePath), true);
                cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -20;
                cell.Colspan = 14;
                cell.FixedHeight = 58;
                cell.PaddingRight = -800;
                cell.PaddingLeft = -28;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 14;
                cell.PaddingBottom = 15f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Organisation:", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("CUSTOM MERCHANDISE ORDER", Heading2));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 6;
                cell.PaddingBottom = 0f;
                cell.PaddingTop = -8;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 2f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingLeft = 20f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Order Date:", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 2f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingLeft = 20f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                string datetime2 = string.Format("{0:dd/MM/yyyy}", CustomerDetail.Orderdate);      //setting date format to only show date in desired format
                phrase.Add(new Chunk(datetime2, Heading3));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contact:", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = -4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = -4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 6;
                cell.PaddingBottom = -4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = -4f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingLeft = 20f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Invoice No:", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = -4f;
                //cell.PaddingLeft = 20f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.QuoteNo.ToString(), Heading3));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = -4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Job Name:", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingTop = -4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingTop = -4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Packing List", Heading1));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 6;
                cell.PaddingTop = -9f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 4;
                cell.PaddingTop = -4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 14;
                cell.Border = 0;
                cell.PaddingBottom = -8f;
                headertable1.AddCell(cell);

                // Packing Detail Table ####################################################################################################

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -4f;
                cell.Border = 0;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -7f;
                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Option No ", Heading5));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingBottom = 0f;
                cell.PaddingTop = -2f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Qty", Heading5));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = -2f;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Brand", Heading5));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = -2f;

                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Code", Heading5));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = -2f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Item", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -2f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Colour", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -2f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Link", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -2f;
                cell.NoWrap = true;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Sizes", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 11;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -2f;
                cell.NoWrap = true;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Total", Heading5));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = -2f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -6f;
                cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                doc.Add(headertable1);
                doc.Add(headertable2);
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfPCell cell;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 800;
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                KENNEWEntities dbContext = new KENNEWEntities();

                var OptionStatus = "Order";
                var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

                var FColor2 = new BaseColor(89, 89, 89);   /*tarun 20/09/2018*/

                var FColor3 = new BaseColor(26, 26, 26); /*tarun 20/09/2018*/

                BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                //iTextSharp.text.Font footerfont1 = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont2 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont3 = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont4 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor3);

                Paragraph footer = new Paragraph("TeeCorp Pty Ltd t/a 24 Hour Merchandise", footerfont2);/*tarun 20/09/2018*/
                Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia.   PO Box 7295 Alexandria NSW 2015   ABN 81 003 060 633", footerfont3);    /*tarun 20/09/2018*/
                Paragraph numbering = new Paragraph("Page 1 of 1", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering2 = new Paragraph("Page 1 of 2", footerfont4);
                Paragraph numbering3 = new Paragraph("Page 2 of 2", footerfont4);
                Paragraph numbering4 = new Paragraph("Page 1 of 3", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering5 = new Paragraph("Page 2 of 3", footerfont4);
                Paragraph numbering6 = new Paragraph("Page 3 of 3", footerfont4);


                cell = new PdfPCell(footer);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 44;        /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                cell = new PdfPCell(address);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 1;         /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                counter++;
                if (OptionData.Count <= 7 && counter == 1)
                {
                    cell = new PdfPCell(numbering);// 1/1
                    cell.Border = 0;
                    //cell.PaddingLeft = 50;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.PaddingTop = -10;
                    footerTbl.AddCell(cell);
                    footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                    counter = 0;
                }
                else if (OptionData.Count > 14)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering4);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = -10;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 2)
                    {
                        cell = new PdfPCell(numbering5);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = -10;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 3)
                    {
                        cell = new PdfPCell(numbering6);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = -10;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }
                else if (OptionData.Count <= 14 && OptionData.Count > 7)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering2);// 1/2
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = -10;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else
                    {
                        cell = new PdfPCell(numbering3);// 2/2
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = -10;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }

            }
        }

        // Confirmation Pdf
        public ActionResult ConfirmationPdf(int id, string PathPdf, string OptionStatus, string QuoteType)
        {
            try
            {
                OptionStatus = "Order";
                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();
                var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

                var AddressinOpp = CustomerDetail.AddressId;

                var dAddress = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.AddressId == AddressinOpp).FirstOrDefault());

                Document doc = new Document(PageSize.A4, -80f, -80f, 20f, 20f);
                doc.SetPageSize(PageSize.A4.Rotate());
                PdfWriter write;
                //HeaderFooterStatementforOrder footer;

                if (QuoteType == "Print")
                {
                    write = PdfWriter.GetInstance(doc, Response.OutputStream);
                    //footer = new HeaderFooterStatementforConfirmation();
                    //write.PageEvent = footer;
                    var page = new HeaderFooterStatementforConfirmation();
                    page.id = id;
                    write.PageEvent = page;
                    // write.PageEvent = iTextSharp.text.pdf.IPdfPageEvent.OptionStatus;
                    Response.ContentType = ("application/pdf");
                }
                else
                {

                    write = PdfWriter.GetInstance(doc, new FileStream(PathPdf, FileMode.Create));
                    //footer = new HeaderFooterStatementforOrder();
                    //write.PageEvent = footer;
                    var page = new HeaderFooterStatementforConfirmation();
                    page.id = id;
                    write.PageEvent = page;
                }

                doc.Open();

                int i, j, k = 5;
                var rcount = 0;
                var FColor = new BaseColor(38, 171, 227);
                var RColor = new BaseColor(230, 230, 230);
                var FColor2 = new BaseColor(51, 51, 51);
                var FColor3 = new BaseColor(255, 255, 255);
                //var FColor3 = new BaseColor(26, 26, 26);

                var Heading1 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, FColor);
                var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 28, iTextSharp.text.Font.NORMAL, FColor);
                var Heading3 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);  
                var Heading4 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1250, true, 9, iTextSharp.text.Font.NORMAL, FColor2);  
                var contentfontCheck = FontFactory.GetFont((Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
                var Heading5 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor2);  
                var Heading6 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor3);  
              
                Phrase phrase;
                PdfPCell cell;

                PdfPTable headertable1 = new PdfPTable(15);
                headertable1.SetWidths(new int[] { 5, 5, 5, 5, 7, 1, 6, 7, 7, 7, 7, 3, 5, 5, 5 });
                PdfPTable headertable2 = new PdfPTable(19);
                headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 19, 6, 6, 6, 6, 6, 6, 9, 8, 8, 9, 9 });
                headertable2.SpacingAfter = 2f;
                PdfPTable headertable3 = new PdfPTable(14);
                headertable3.SetWidths(new int[] { 3, 4, 6, 7, 7, 7, 7, 7, 7, 7, 3, 5, 5, 5 });
                //headertable3.SpacingBefore = 50f;


                // Table Data ############################################################################################################

                if (OptionData.Count > 0)
                {
                    var CurrentColor = BaseColor.WHITE;

                    for (i = 0; i < OptionData.Count; i++)
                    {
                        // cell.BackgroundColor = (i % 2) == 0
                        // ? RColor : Color.WHITE;

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.PaddingBottom = -6f;
                        cell.Border = 0;
                        if (i % 2 == 0)                               // Adding alternate colors to cells
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].OptionNo.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.FixedHeight = 19f;
                        cell.Colspan = 1;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);



                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].Quantity.ToString(), Heading5));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.FixedHeight = 19f;
                        cell.Colspan = 1;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].brand.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.FixedHeight = 19f;
                        cell.Colspan = 1;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].code.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].Item.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].colour.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        if (OptionData[i].Link != "N/A")
                        {
                            phrase = new Phrase();
                            string linkurl = OptionData[i].Link.ToString();
                            //string linkurl2 = "";
                            //if (linkurl.Length > 8)
                            //{
                            //    linkurl2 = linkurl.Substring(0, 8);
                            //}
                            //else
                            //{
                            //    linkurl2 = linkurl;
                            //}
                            //Chunk chunk = new Chunk(linkurl2, FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.UNDERLINE, BaseColor.BLUE));
                            Image ImagePath = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/maximize.png"));
                            Chunk chunk = new Chunk(ImagePath, 0, 0, true);
                            chunk.SetAnchor(OptionData[i].Link.ToString());
                            phrase.Add(chunk);
                            cell = new PdfPCell(phrase);
                            cell.Colspan = 1;
                            cell.FixedHeight = 19f;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.PaddingTop = 3f;
                            //cell.NoWrap = true;
                            cell.PaddingBottom = 6f;
                            cell.Border = PdfPCell.RIGHT_BORDER;
                            if (i % 2 == 0)
                            {
                                cell.BackgroundColor = BaseColor.WHITE;
                            }
                            else
                            {
                                cell.BackgroundColor = RColor;
                            }
                            headertable2.AddCell(cell);
                        }
                        else
                        {
                            phrase = new Phrase();
                            phrase.Add(new Chunk("N/A", Heading3));
                            cell = new PdfPCell(phrase);
                            cell.Colspan = 1;
                            cell.FixedHeight = 19f;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.PaddingTop = 0f;
                            //cell.NoWrap = true;
                            cell.PaddingBottom = 0f;
                            cell.Border = PdfPCell.RIGHT_BORDER;
                            if (i % 2 == 0)
                            {
                                cell.BackgroundColor = BaseColor.WHITE;
                            }
                            else
                            {
                                cell.BackgroundColor = RColor;
                            }
                            headertable2.AddCell(cell);

                        }

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].size.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        //cell.NoWrap = true;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        // baans change 28th November
                        //phrase.Add(new Chunk(OptionData[i].front.ToString(), Heading5));
                        if (OptionData[i].Front_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Front_Detail_Desc1.ToString(), Heading3));
                        }
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.PaddingTop = 0f;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        //phrase.Add(new Chunk(OptionData[i].back.ToString(), Heading5));
                        if (OptionData[i].Back_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Back_Detail_Desc1.ToString(), Heading3));
                        }
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        //phrase.Add(new Chunk(OptionData[i].leftdec.ToString(), Heading5));
                        if (OptionData[i].Left_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Left_Detail_Desc1.ToString(), Heading3));
                        }
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        //phrase.Add(new Chunk(OptionData[i].rightdec.ToString(), Heading5));
                        if (OptionData[i].Right_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Right_Detail_Desc1.ToString(), Heading3));
                        }
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        //phrase.Add(new Chunk(OptionData[i].other.ToString(), Heading5));
                        if (OptionData[i].Other_Detail_Desc1 != "N/A")
                        {
                            phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc1.ToString(), Heading3));
                            phrase.Add(Chunk.NEWLINE);
                            phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc2.ToString(), Heading3));
                        }
                        else
                        {
                            phrase.Add(new Chunk(OptionData[i].Other_Detail_Desc1.ToString(), Heading3));
                        }
                        // baans end 28th November
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        string decimalvalue = string.Format("{0:C}", OptionData[i].OtherCost.ToString());
                        //converting value to decimal format with Currency sign($).

                        phrase.Add(new Chunk(decimalvalue, Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(OptionData[i].Service.ToString(), Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);


                        phrase = new Phrase();
                        string decimalvalue1 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].UnitExGST.ToString()), 2));
                        //converting value to decimal format with Currency sign($).

                        phrase.Add(new Chunk(decimalvalue1, Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);


                        phrase = new Phrase();
                        string decimalvalue2 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].UnitInclGST.ToString()), 2));
                        phrase.Add(new Chunk(decimalvalue2, Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        string decimalvalue3 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].ExtExGST.ToString()), 2));
                        phrase.Add(new Chunk(decimalvalue3, Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = PdfPCell.RIGHT_BORDER;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);


                        phrase = new Phrase();
                        string decimalvalue4 = string.Format("{0:C}", Math.Round(Convert.ToDecimal(OptionData[i].ExtInclGST.ToString()), 2));
                        phrase.Add(new Chunk(decimalvalue4, Heading3));
                        cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Colspan = 1;
                        cell.FixedHeight = 19f;
                        cell.PaddingTop = 0f;
                        cell.PaddingBottom = 0f;
                        cell.Border = 0;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.PaddingBottom = -5f;
                        cell.Border = 0;
                        if (i % 2 == 0)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                            CurrentColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                            CurrentColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        rcount++;
                        if (OptionData.Count > 6 && i != OptionData.Count - 1)
                        {
                            if (i > 0 && rcount % 6 == 0)
                            {
                                doc.Add(headertable2);
                                headertable2.DeleteBodyRows();
                                doc.NewPage();
                                rcount = 0;
                            }
                        }
                    }

                    for (j = 0; j <= (k - rcount); j++)
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.PaddingBottom = -6f;
                        cell.Border = 0;
                        if (CurrentColor == RColor)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.FixedHeight = 25f;
                        cell.PaddingBottom = 6f;
                        cell.Border = 0;
                        cell.BackgroundColor = RColor;
                        if (CurrentColor == RColor)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;

                        }
                        else
                        {
                            cell.BackgroundColor = RColor;

                        }
                        headertable2.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading3));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 19;
                        cell.PaddingBottom = -6f;
                        cell.Border = 0;
                        if (CurrentColor == RColor)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                            CurrentColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = RColor;
                            CurrentColor = RColor;
                        }
                        headertable2.AddCell(cell);
                    }

                }

                //Bottom Address ##############################################################################

                // baans change 13th December for EFT in the Confirmation pdf


                //Next Row
                phrase = new Phrase();
                //phrase.Add(new Chunk("Paying by EFT please enter this number as the payment ID:", Heading2));
                //Commented and changed by baans 11Dec2020 start
                //phrase.Add(new Chunk("Please Note: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("Please Note: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 24, iTextSharp.text.Font.NORMAL, FColor)));
                //Commented and changed by baans 11Dec2020 end
                phrase.Add(new Chunk("Orders will only proceed after your Artwork Proof is confirmed", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 11;
                cell.Rowspan = 2;
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                /*cell.PaddingBottom = 5f;*/
                cell.PaddingTop = 4f;
                cell.PaddingLeft = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Shipping:", Heading4));
                phrase.Add(new Chunk(" " + CustomerDetail.ShippingBy.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingLeft = 25f;
                cell.PaddingBottom = 5f;            
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                string decivalue = string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.ShippingPrice.ToString()), 2));
                phrase.Add(new Chunk(decivalue, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 5f;
                cell.PaddingRight = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                //Next Row
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 14;
                //cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.PaddingBottom = -2f;
                //headertable3.AddCell(cell);//commented by baans 01Oct2020

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Order/Invoice Total:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 9f;
                cell.Border = 0;
                cell.PaddingLeft = 25f;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                // (CustomerDetail.OrderTotal + Convert.ToDecimal(CustomerDetail.ShippingPrice))
                // string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.OrderTotal.ToString()), 2));
                string Ordertotal = string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.OrderTotal).ToString()), 2));
                phrase.Add(new Chunk(Ordertotal, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 1f;
                cell.PaddingRight = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 11;
                cell.Border = 0;
                cell.PaddingBottom = -2f;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("GST:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                cell.PaddingLeft = 25f;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                if (CustomerDetail.GST != null)
                {
                    phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice) / 11), 2)), Heading4));
                }
                else
                {
                    phrase.Add(new Chunk("$0.00", Heading4));
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 8f;
                cell.PaddingRight = -1;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Delivery Address:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                cell.PaddingLeft = -1;
                headertable3.AddCell(cell);

                var Address1 = " ";
                if (AddressinOpp != null)
                {
                    Address1 = dAddress.Address1 == null ? "" : dAddress.Address1.ToString();
                }
                else
                {
                    Address1 = CustomerDetail.Address1 == "N/A" ? "" : CustomerDetail.Address1.ToString();
                }
                phrase = new Phrase();
                phrase.Add(new Chunk(Address1, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = 8f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("To finalise this Invoice please make payment to:", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingLeft = -50f;
                cell.Colspan = 3;
                cell.PaddingBottom = 8f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Bank: ANZ", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.PaddingLeft = -50f;
                cell.PaddingBottom = 8f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Final Total:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 8f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                cell.PaddingLeft = 25f;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                if (CustomerDetail.FinalTotal != null)
                {
                    phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal((CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice).ToString()), 2)), Heading4));
                }
                else
                {
                    phrase.Add(new Chunk("$0.00", Heading4));
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 8f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                cell.PaddingRight = -1;
                headertable3.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.PaddingBottom = 5f;
                headertable3.AddCell(cell);

                var Address2 = " ";
                if (AddressinOpp != null)
                {
                    Address2 = dAddress.Address2 == null ? "" : dAddress.Address2.ToString();
                }
                else
                {
                    Address2 = CustomerDetail.Address2 == "N/A" ? "" : CustomerDetail.Address2.ToString();
                }
                phrase = new Phrase();
                phrase.Add(new Chunk(Address2, Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = 8f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Or pay by Visa, Mastercard or PayPal", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.PaddingLeft = -50f;
                cell.PaddingTop = -1;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Acct Name: Tuff Tees", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.PaddingLeft = -50f;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Deposit Paid:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingLeft = 25f;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);


                phrase = new Phrase();
                if (CustomerDetail.AmtReceived != null)
                {
                    phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(CustomerDetail.AmtReceived.ToString()), 2)), Heading4));
                }
                else
                {
                    phrase.Add(new Chunk("$0.00", Heading4));
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = -1;
                cell.PaddingRight = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                if (AddressinOpp != null)
                {
                    phrase.Add(new Chunk(dAddress.State == null ? "" : dAddress.State.ToString() + "  " + dAddress.Postcode.ToString(), Heading4));
                }
                else
                {
                    if (CustomerDetail.Postcode != null)
                    {
                        phrase.Add(new Chunk(CustomerDetail.State == "N/A" ? "" : CustomerDetail.State.ToString() + "  " + CustomerDetail.Postcode.ToString(), Heading4));
                    }
                    else
                    {
                        phrase.Add(new Chunk(" ", Heading4));
                    }
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Phone 02 9559 2400 for payment", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingTop = -4f;
                cell.Colspan = 3;
                cell.PaddingLeft = -50f;
                cell.PaddingTop = -1;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("BSB: 012301          Acct No.: 323 562 825", Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.PaddingLeft = -50f;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Balance Due:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingLeft = 25f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = -1;
                cell.Border = 0;
                headertable3.AddCell(cell);

                decimal? Balance = 0;
                if (CustomerDetail.AmtReceived != null)
                {

                    Balance = Math.Round(Convert.ToDecimal(CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice), 2) - Math.Round(Convert.ToDecimal(CustomerDetail.AmtReceived.ToString()), 2);
                }
                else
                {
                    Balance = Math.Round(Convert.ToDecimal(CustomerDetail.FinalTotal + CustomerDetail.ShippingPrice), 2);
                }
                phrase = new Phrase();
                phrase.Add(new Chunk(string.Format("{0:C}", Math.Round(Convert.ToDecimal(Balance.ToString()), 2)), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 5f;
                cell.PaddingRight = -1;
                cell.Border = 0;
                cell.PaddingTop = -1;
                headertable3.AddCell(cell);

                //doc.Add(headertable1);
                doc.Add(headertable2);
                doc.Add(headertable3);

                doc.Close();
                doc.Dispose();
            }
            catch (Exception ex)
            {

            }
            if (QuoteType == "Print")
            {

                return View();
            }
            else
            {
                return null;
            }
        }

        // Header/Footer For Confirmation Pdf
        public class HeaderFooterStatementforConfirmation : PdfPageEventHelper
        {
            int counter = 0;
            public int id { get; set; }
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                KENNEWEntities dbContext = new KENNEWEntities();

                //int id = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["id"]);
                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();
                // baans change 14th November for ProductionDate 
                //var ProductionDate = Mapper.Map<KanBanViewModel>(dbContext.tblkanbans.Where(_ => _.OppId == id).FirstOrDefault());
                var ProductionDate = dbContext.tblOpportunities.Where(_ => _.OpportunityId == id).Select(_ => _.ConfirmedDate).FirstOrDefault();
                // baans end 14th November
                Rectangle pageSize = doc.PageSize;

                var FColor = new BaseColor(38, 171, 227);
                var RColor = new BaseColor(230, 230, 230);
                var FColor2 = new BaseColor(51, 51, 51);
                var FColor3 = new BaseColor(38, 38, 38);
                var FColor4 = new BaseColor(255, 255, 255);

                //var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, FColor);
                //Commented and changed by baans 11Dec2020 start
                //var Heading2 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor);
                //var Heading4 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 32, iTextSharp.text.Font.NORMAL, FColor3);
                var Heading2 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 32, iTextSharp.text.Font.NORMAL, FColor);
                var Heading3 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading4 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1250, true, 9, iTextSharp.text.Font.NORMAL, FColor2);
                var contentfontCheck = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
                var Heading5 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading6 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor3);
                var Heading7 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, FColor);
                var Heading8 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 12, iTextSharp.text.Font.NORMAL, FColor4);
                var Heading9 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 13, iTextSharp.text.Font.BOLD, FColor4);
                var Heading10 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 7, iTextSharp.text.Font.NORMAL, FColor2);

                Phrase phrase;
                PdfPCell cell;

                 PdfPTable headertable1 = new PdfPTable(15);
                 headertable1.SetWidths(new int[] { 6, 4, 5, 5, 7, 1, 6, 7, 7, 7, 7, 3, 5, 5, 5 });
                 PdfPTable headertable2 = new PdfPTable(19);
                 headertable2.SetWidths(new int[] { 7, 4, 6, 6, 8, 6, 7, 19, 6, 6, 6, 6, 6, 6, 9, 8, 8, 9, 9 });
                headertable2.SpacingAfter = 2f;
                PdfPTable headertable3 = new PdfPTable(16);
                //Row 
                string ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Header.jpg");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(ImagePath), true);
                cell.Border = 0;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -18;
                cell.Colspan = 15;
                cell.FixedHeight = 58;
                cell.PaddingRight = -700;
                cell.PaddingLeft = -28;
                headertable1.AddCell(cell);

                //Commented  by baans 03Oct2020
                ////Next Row
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 14;
                //cell.PaddingBottom = 15f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Job Name: ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 24f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.OppName.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingTop = 23f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("CUSTOM MERCHANDISE ORDER", Heading4));
                phrase.Add(new Chunk("CUSTOM MERCHANDISE ", Heading1));
                phrase.Add(new Chunk("TAX INVOICE", Heading2));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 9;
                cell.PaddingBottom = 5f;
                //cell.PaddingTop = -8;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 2f;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                ////cell.PaddingLeft = 20f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Order Date:", Heading3)); //commented and changed by baans 03Oct2020
                phrase.Add(new Chunk("Order/Invoice Date:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingTop = 24f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                string datetime2 = string.Format("{0:dd/MM/yyyy}", CustomerDetail.Orderdate);      //setting date format to only show date in desired format
                phrase.Add(new Chunk(datetime2, Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingTop = 24f;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                cell.PaddingRight = -1f;
                headertable1.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Contact: ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 7f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                // cell.PaddingTop = 9f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 7f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                // cell.PaddingTop = 9f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.ContactName.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.PaddingTop = 10f;
                //cell.Border = 0;
                //cell.PaddingLeft = -1;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 9;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 1;
                //cell.PaddingBottom = 2f;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                ////cell.PaddingLeft = 20f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                // baans change 03OCT2020 for the JobHub Number
                phrase.Add(new Chunk("Order/Invoice No:", Heading4));
                //phrase.Add(new Chunk("Invoice No:", Heading3));
                // baans end 2nd November 
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                cell.PaddingLeft = 15f;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.QuoteNo.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;                
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Organisation:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 5f;
                //cell.PaddingTop = 5;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 5f;
                //cell.PaddingTop = 5;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(CustomerDetail.Organisation.ToString(), Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 2;
                //cell.PaddingBottom = 2f;
                //cell.PaddingTop = 12f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Order Status: ", Heading7));
                phrase.Add(new Chunk("Confirmed!", Heading6));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.Colspan = 9;
                cell.PaddingBottom = 10f;
                cell.PaddingTop = -15;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Terms:", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                //cell.PaddingTop = 12f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.PaddingLeft = 15f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("50% Production Deposit \nBalance prior to shipping", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                //cell.PaddingTop = 12f;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                //Next Row
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading2));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 14;
                //cell.Border = 0;
                //cell.PaddingBottom = -4f;
                //headertable1.AddCell(cell);

                //Next Row
                //phrase = new Phrase();
                //phrase.Add(new Chunk("Dispatch/Pick Up Date: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, FColor)));
                //if (ProductionDate != null)
                //{
                //    string Prodate = string.Format("{0:dd/MM/yyyy}", ProductionDate);
                //    phrase.Add(new Chunk(Prodate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //}
                //else
                //{
                //    phrase.Add(new Chunk("", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //}
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 6;
                //cell.PaddingBottom = 6f;
                //cell.Border = 0;
                //headertable1.AddCell(cell);
                //Commented and changed by baans 03Oct2020
                phrase = new Phrase();
                //Commented and change by baans 11Dec2020 start
                //phrase.Add(new Chunk("COMPLETION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //if (ProductionDate != null)
                //{
                //    string Prodate = string.Format("{0:dd/MM/yyyy}", ProductionDate);
                //    phrase.Add(new Chunk(Prodate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //}
                //else
                //{
                //    phrase.Add(new Chunk("", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //}
                phrase.Add(new Chunk("COMPLETION DATE: ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                if (ProductionDate != null)
                {
                    string Prodate = string.Format("{0:dd/MM/yyyy}", ProductionDate);
                    phrase.Add(new Chunk(Prodate, FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                }
                else
                {
                    phrase.Add(new Chunk("", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                }
                //Commented and change by baans 11Dec2020 end
                cell = new PdfPCell(phrase);
                cell.Colspan = 7;
                cell.PaddingBottom = 6f;
                cell.Border = 0;
                cell.PaddingLeft = -1f;
                headertable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Please keep an eye on your email for our production proof.", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 8;
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                //cell.PaddingBottom = 5f;
                //cell.Border = 0;
                //headertable1.AddCell(cell); //Commented and changed by baans 03Oct2020

                phrase = new Phrase();
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("An Artwork Proof will be sent to you for confirmation", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("An Artwork Proof will be sent to you for confirmation", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 16, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingBottom = 1f;
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                //phrase.Add(new Chunk("An Artwork Proof will be sent to you for confirmation", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                //phrase.Add(new Chunk("NEXT STEP >>> ", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/COMPACTASB-BOLD.OTF")), BaseFont.CP1252, true, 26, iTextSharp.text.Font.NORMAL, FColor)));
                phrase.Add(new Chunk("An Artwork Proof will be sent to you for confirmation", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 15, iTextSharp.text.Font.NORMAL, BaseColor.RED)));
                cell = new PdfPCell(phrase);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 6;
                cell.PaddingBottom = 1f;
                cell.Border = 0;               
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingRight = -1;
                headertable1.AddCell(cell);

                BaseColor BackgroundColor = WebColors.GetRGBColor("#AAE0F6");
                //string TTabImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Content/images/JobStageIndicators-Confirmed-V2.png");
                //cell = new PdfPCell(iTextSharp.text.Image.GetInstance(TTabImagePath), true);
                //cell.Border = 0;
                //cell.PaddingTop = -1f;
                //cell.Colspan = 14;
                //cell.FixedHeight = 30;
                //cell.PaddingLeft = -1;
                //cell.PaddingRight = -1;
                //cell.BackgroundColor = BackgroundColor;
                //headertable1.AddCell(cell);

                BaseColor TopBarColor = WebColors.GetRGBColor("#28ABE1");
                phrase = new Phrase();
                phrase.Add(new Chunk("QUOTE        >>>        ", Heading8));
                phrase.Add(new Chunk("INVOICE        >>>        ", Heading8));
                phrase.Add(new Chunk("[ DEPOSIT ]", Heading9));
                phrase.Add(new Chunk("        >>>        ", Heading8));
                phrase.Add(new Chunk("PROOF        >>>        ", Heading8));
                phrase.Add(new Chunk("PRODUCTION        >>>        ", Heading8));
                phrase.Add(new Chunk("COMPLETE        >>>        ", Heading8));
                phrase.Add(new Chunk("PAID        >>>        ", Heading8));
                phrase.Add(new Chunk("SHIPPED", Heading8));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 16;
                cell.PaddingTop = 3f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = TopBarColor;
                headertable3.AddCell(cell);


                phrase = new Phrase();
                phrase.Add(new Chunk("Your order is scheduled to be completed on or before the Completion Date", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 8;
                cell.PaddingTop = 3f;
                cell.PaddingLeft = 2f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = BackgroundColor;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Please keep an eye on your email for a confirmation proof of your artwork", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 8;
                cell.PaddingRight = 2f;
                cell.PaddingTop = 3f;
                cell.PaddingBottom = 7f;
                cell.Border = 0;
                cell.BackgroundColor = BackgroundColor;
                headertable3.AddCell(cell);

                //Connfirm table start form here
                //#############################################################################################################

                //Next Row
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading5));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 19;
                //cell.PaddingBottom = -4f;
                //cell.Border = 0;
                //headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -6f;
                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Option No ", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingBottom = 2f;
                cell.PaddingTop = 0f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Qty", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Brand", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.PaddingLeft = 4f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Code", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Item", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.PaddingLeft = 4f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Colour", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Link", Heading5)); //commented and changed by baans 03Oct2020
                phrase.Add(new Chunk("View", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 0f;
                cell.NoWrap = true;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Sizes", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.NoWrap = true;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                //phrase.Add(new Chunk("Front Dec", Heading5)); //Commentd and changed by baans 03Oct2020
                phrase.Add(new Chunk("Front App", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Back App", Heading10));
                //phrase.Add(new Chunk("Back Dec", Heading5));//Commented and changed by baans 03Oct2020
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Lft Slv App", Heading10));
                //phrase.Add(new Chunk("Lft Slv Dec", Heading5)); //Commented and changed by baans 03Oct2020
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Rht Slv App", Heading10));
                //phrase.Add(new Chunk("Rht Slv Dec", Heading5)); //Commented and changed by baans 03Oct2020
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Other App", Heading10));
                //phrase.Add(new Chunk("Other Dec", Heading5)); //Commented and Changed By baans 03oct2020
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Other", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Service", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.PaddingLeft = 4f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit Ex GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit + GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext Ex GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext + GST", Heading10));
                cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingBottom = 2f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading10));
                cell = new PdfPCell(phrase);
                cell.Colspan = 19;
                cell.PaddingBottom = -5f;
                cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headertable2.AddCell(cell);

                doc.Add(headertable1);
                doc.Add(headertable3);
                doc.Add(headertable2);
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfPCell cell;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 800;
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                KENNEWEntities dbContext = new KENNEWEntities();

                var OptionStatus = "Order";
                var OptionData = dbContext.Pro_QuoteOptionsDetail(id, OptionStatus).ToList();

                var CustomerDetail = dbContext.Pro_QuoteCustomerData(id).FirstOrDefault();

                //var counter = 0;

                var FColor2 = new BaseColor(89, 89, 89);   /*tarun 20/09/2018*/

                var FColor3 = new BaseColor(26, 26, 26); /*tarun 20/09/2018*/
                //Commented and changed by baans 11Dec2020 start
                //BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/Myriad Pro Regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                //Commented and changed by baans 11Dec2020 end
                //iTextSharp.text.Font footerfont1 = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont2 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont3 = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont4 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor3);

                /*Paragraph footer = new Paragraph("TeeCorp Pty Ltd t/a 24 Hour Merchandise", footerfont2);*//*tarun 20/09/2018*/
                /*Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia.    PO Box 7295 Alexandria NSW 2015    ABN 60 130 686 234", footerfont3);*/    /*tarun 20/09/2018*/
                //Commented and change by baans 01Oct2020
                Paragraph footer = new Paragraph("Tuff Tees Pty Ltd t/a 24 Hour Merchandise", footerfont2);
                Paragraph address = new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia. PO Box 7295 Alexandria NSW 2015  ABN 81 003 060 633  Ph: 02 9559 2400  Account Manager: " + CustomerDetail.AccountManager, footerfont3);
                Paragraph numbering = new Paragraph("Page 1 of 1", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering2 = new Paragraph("Page 1 of 2", footerfont4);
                Paragraph numbering3 = new Paragraph("Page 2 of 2", footerfont4);
                Paragraph numbering4 = new Paragraph("Page 1 of 3", footerfont4);  /*tarun 20/09/2018*/
                Paragraph numbering5 = new Paragraph("Page 2 of 3", footerfont4);
                Paragraph numbering6 = new Paragraph("Page 3 of 3", footerfont4);

                string StartrackImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Startrack.png");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(StartrackImagePath), true);
                cell.Border = 0;
                cell.PaddingBottom = -38f;
                cell.PaddingTop = 32f;
                cell.FixedHeight = 19;
                footerTbl.AddCell(cell);

                string PayPalImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Credit-Card_PayPal.jpg");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(PayPalImagePath), true);
                cell.Border = 0;
                cell.PaddingBottom = -20f;
                cell.PaddingTop = 15f;
                cell.FixedHeight = 22;
                cell.PaddingLeft = 675;
                cell.PaddingRight = -4;
                footerTbl.AddCell(cell);

                cell = new PdfPCell(footer);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -5;        /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                cell = new PdfPCell(address);
                cell.Border = 0;
                //cell.PaddingLeft = 50;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = 1;         /*tarun 20/09/2018*/
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                counter++;
                if (OptionData.Count <= 5 && counter == 1)
                {
                    cell = new PdfPCell(numbering);// 1/1
                    cell.Border = 0;
                    //cell.PaddingLeft = 50;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.PaddingTop = 1;
                    footerTbl.AddCell(cell);
                    footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);

                    counter = 0;
                }
                else if (OptionData.Count > 10)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering4);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 2)
                    {
                        cell = new PdfPCell(numbering5);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else if (counter == 3)
                    {
                        cell = new PdfPCell(numbering6);// 1/1
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }
                else if (OptionData.Count <= 10 && OptionData.Count > 5)
                {
                    if (counter == 1)
                    {
                        cell = new PdfPCell(numbering2);// 1/2
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                    }
                    else
                    {
                        cell = new PdfPCell(numbering3);// 2/2
                        cell.Border = 0;
                        //cell.PaddingLeft = 50;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.PaddingTop = 1;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 20, 70, writer.DirectContent);
                        counter = 0;
                    }
                }


            }
        }
        // baans change 15th November
        public List<AccountManagerDropdownViewModel> getProfileList()
        {
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.firstname)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        // baans end 15th November

        //Proof Pdf
        public ActionResult Proof_24HourMerchandise(int OptionId, string PathPdf, string QuoteType, string PdfType)
        {
            //PdfType = "PackingList";
            //int id = 42862;
            OptionViewModel ProofOptions = new OptionViewModel();
            if (QuoteType == "Print")
            {
                ProofOptions = Mapper.Map<OptionViewModel>(dbContext.tbloptions.Where(_ => _.id == OptionId && _.include_job == true).FirstOrDefault());
            }
            else
            {
                ProofOptions = Mapper.Map<OptionViewModel>(dbContext.tbloptions.Where(_ => _.id == OptionId && _.include_job == true && _.ProofSent == true).FirstOrDefault());
            }

            List<ProofApplication> ProofApplications = new List<ProofApplication>();

            if (ProofOptions.front_decoration != null)
            {
                var Front = dbContext.TblApplications.Where(_ => _.ApplicationId == ProofOptions.front_decoration).FirstOrDefault();

                ProofApplication FrontDecoration = new ProofApplication();
                FrontDecoration.ApplicationId = Front.ApplicationId;
                FrontDecoration.ApplicationLocation = "FRONT";
                FrontDecoration.AppType = Front.AppType;
                FrontDecoration.AppWidth = Front.AppWidth;
                FrontDecoration.AppImage = Front.AppImage;
                FrontDecoration.MockUpImage = Front.MockUpImage;

                var ColoursId = dbContext.TblApplicationColoursMappings.Where(_ => _.ApplicationId == ProofOptions.front_decoration).ToList();
                List<ProofClours> ProofDesignColours = new List<ProofClours>();
                for (var CId = 0; CId < ColoursId.Count; CId++)
                {
                    var ColorId = ColoursId[CId].ApplicationColourId;
                    var cData = Mapper.Map<ApplicationColourViewModel>(dbContext.TblApplicationColours.Where(_ => _.ApplicationColourId == ColorId).FirstOrDefault());

                    ProofClours DesignColour = new ProofClours();
                    DesignColour.ApplicationColourId = cData.ApplicationColourId;
                    DesignColour.InkColour = cData.InkColour;
                    DesignColour.ThreadColour = cData.ThreadColour;
                    DesignColour.TransferColour = cData.TransferColour;
                    DesignColour.PantoneName = cData.PantoneName;
                    DesignColour.HexvalueColour = cData.HexvalueColour;

                    ProofDesignColours.Add(DesignColour);
                }
                FrontDecoration.Colours = ProofDesignColours;
                ProofApplications.Add(FrontDecoration);
            }

            if (ProofOptions.back_decoration != null)
            {
                var Back = dbContext.TblApplications.Where(_ => _.ApplicationId == ProofOptions.back_decoration).FirstOrDefault();

                ProofApplication BackDecoration = new ProofApplication();
                BackDecoration.ApplicationId = Back.ApplicationId;
                BackDecoration.ApplicationLocation = "BACK";
                BackDecoration.AppType = Back.AppType;
                BackDecoration.AppWidth = Back.AppWidth;
                BackDecoration.AppImage = Back.AppImage;
                BackDecoration.MockUpImage = Back.MockUpImage;

                var ColoursId = dbContext.TblApplicationColoursMappings.Where(_ => _.ApplicationId == ProofOptions.back_decoration).ToList();
                List<ProofClours> ProofDesignColours = new List<ProofClours>();
                for (var CId = 0; CId < ColoursId.Count; CId++)
                {
                    var ColorId = ColoursId[CId].ApplicationColourId;
                    var cData = Mapper.Map<ApplicationColourViewModel>(dbContext.TblApplicationColours.Where(_ => _.ApplicationColourId == ColorId).FirstOrDefault());

                    ProofClours DesignColour = new ProofClours();
                    DesignColour.ApplicationColourId = cData.ApplicationColourId;
                    DesignColour.InkColour = cData.InkColour;
                    DesignColour.ThreadColour = cData.ThreadColour;
                    DesignColour.TransferColour = cData.TransferColour;
                    DesignColour.PantoneName = cData.PantoneName;
                    DesignColour.HexvalueColour = cData.HexvalueColour;

                    ProofDesignColours.Add(DesignColour);
                }
                BackDecoration.Colours = ProofDesignColours;
                ProofApplications.Add(BackDecoration);
            }
            if (ProofOptions.left_decoration != null)
            {
                var Left = dbContext.TblApplications.Where(_ => _.ApplicationId == ProofOptions.left_decoration).FirstOrDefault();

                ProofApplication leftDecoration = new ProofApplication();
                leftDecoration.ApplicationId = Left.ApplicationId;
                leftDecoration.ApplicationLocation = "LEFT";
                leftDecoration.AppType = Left.AppType;
                leftDecoration.AppWidth = Left.AppWidth;
                leftDecoration.AppImage = Left.AppImage;
                leftDecoration.MockUpImage = Left.MockUpImage;

                var ColoursId = dbContext.TblApplicationColoursMappings.Where(_ => _.ApplicationId == ProofOptions.left_decoration).ToList();
                List<ProofClours> ProofDesignColours = new List<ProofClours>();
                for (var CId = 0; CId < ColoursId.Count; CId++)
                {
                    var ColorId = ColoursId[CId].ApplicationColourId;
                    var cData = Mapper.Map<ApplicationColourViewModel>(dbContext.TblApplicationColours.Where(_ => _.ApplicationColourId == ColorId).FirstOrDefault());

                    ProofClours DesignColour = new ProofClours();
                    DesignColour.ApplicationColourId = cData.ApplicationColourId;
                    DesignColour.InkColour = cData.InkColour;
                    DesignColour.ThreadColour = cData.ThreadColour;
                    DesignColour.TransferColour = cData.TransferColour;
                    DesignColour.PantoneName = cData.PantoneName;
                    DesignColour.HexvalueColour = cData.HexvalueColour;

                    ProofDesignColours.Add(DesignColour);
                }
                leftDecoration.Colours = ProofDesignColours;
                ProofApplications.Add(leftDecoration);
            }

            if (ProofOptions.right_decoration != null)
            {
                var Right = dbContext.TblApplications.Where(_ => _.ApplicationId == ProofOptions.right_decoration).FirstOrDefault();

                ProofApplication rightDecoration = new ProofApplication();
                rightDecoration.ApplicationId = Right.ApplicationId;
                rightDecoration.ApplicationLocation = "RIGHT";
                rightDecoration.AppType = Right.AppType;
                rightDecoration.AppWidth = Right.AppWidth;
                rightDecoration.AppImage = Right.AppImage;
                rightDecoration.MockUpImage = Right.MockUpImage;

                var ColoursId = dbContext.TblApplicationColoursMappings.Where(_ => _.ApplicationId == ProofOptions.right_decoration).ToList();
                List<ProofClours> ProofDesignColours = new List<ProofClours>();
                for (var CId = 0; CId < ColoursId.Count; CId++)
                {
                    var ColorId = ColoursId[CId].ApplicationColourId;
                    var cData = Mapper.Map<ApplicationColourViewModel>(dbContext.TblApplicationColours.Where(_ => _.ApplicationColourId == ColorId).FirstOrDefault());

                    ProofClours DesignColour = new ProofClours();
                    DesignColour.ApplicationColourId = cData.ApplicationColourId;
                    DesignColour.InkColour = cData.InkColour;
                    DesignColour.ThreadColour = cData.ThreadColour;
                    DesignColour.TransferColour = cData.TransferColour;
                    DesignColour.PantoneName = cData.PantoneName;
                    DesignColour.HexvalueColour = cData.HexvalueColour;

                    ProofDesignColours.Add(DesignColour);
                }
                rightDecoration.Colours = ProofDesignColours;
                ProofApplications.Add(rightDecoration);
            }

            if (ProofOptions.extra_decoration != null)
            {
                var Extra = dbContext.TblApplications.Where(_ => _.ApplicationId == ProofOptions.extra_decoration).FirstOrDefault();

                ProofApplication extraDecoration = new ProofApplication();
                extraDecoration.ApplicationId = Extra.ApplicationId;
                extraDecoration.ApplicationLocation = "EXTRA";
                extraDecoration.AppType = Extra.AppType;
                extraDecoration.AppWidth = Extra.AppWidth;
                extraDecoration.AppImage = Extra.AppImage;
                extraDecoration.MockUpImage = Extra.MockUpImage;

                var ColoursId = dbContext.TblApplicationColoursMappings.Where(_ => _.ApplicationId == ProofOptions.extra_decoration).ToList();
                List<ProofClours> ProofDesignColours = new List<ProofClours>();
                for (var CId = 0; CId < ColoursId.Count; CId++)
                {
                    var ColorId = ColoursId[CId].ApplicationColourId;
                    var cData = Mapper.Map<ApplicationColourViewModel>(dbContext.TblApplicationColours.Where(_ => _.ApplicationColourId == ColorId).FirstOrDefault());

                    ProofClours DesignColour = new ProofClours();
                    DesignColour.ApplicationColourId = cData.ApplicationColourId;
                    DesignColour.InkColour = cData.InkColour;
                    DesignColour.ThreadColour = cData.ThreadColour;
                    DesignColour.TransferColour = cData.TransferColour;
                    DesignColour.PantoneName = cData.PantoneName;
                    DesignColour.HexvalueColour = cData.HexvalueColour;

                    ProofDesignColours.Add(DesignColour);
                }
                extraDecoration.Colours = ProofDesignColours;
                ProofApplications.Add(extraDecoration);
            }

            Document doc = new Document(PageSize.A4, -50f, -50f, 20f, 5f);
            doc.SetPageSize(PageSize.A4);
            PdfWriter write;

            if (QuoteType == "Print")
            {
                write = PdfWriter.GetInstance(doc, Response.OutputStream);
                var page = new HeaderFooterStatementforProof();
                page.OpportunityId = Convert.ToInt32(ProofOptions.OpportunityId);
                page.OptionId = ProofOptions.id;
                page.OptionStage = ProofOptions.OptionStage;
                page.PdfType = PdfType;
                write.PageEvent = page;
                Response.ContentType = ("application/pdf");
            }
            else
            {
                write = PdfWriter.GetInstance(doc, new FileStream(PathPdf, FileMode.Create));
                var page = new HeaderFooterStatementforProof();
                page.OpportunityId = Convert.ToInt32(ProofOptions.OpportunityId);
                page.OptionId = ProofOptions.id;
                page.OptionStage = ProofOptions.OptionStage;
                page.PdfType = PdfType;
                write.PageEvent = page;
            }

            doc.Open();

            int i, j, k = 4;
            var rcount = 0;
            var FColor = new BaseColor(38, 38, 38);
            var FColorLight = new BaseColor(153, 153, 153);
            var FColorRed = new BaseColor(185, 35, 39);
            var ColourChipBorder = new BaseColor(115, 115, 115);

            var Heading1 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 13, iTextSharp.text.Font.BOLD, FColorRed);
            var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 9, iTextSharp.text.Font.NORMAL, FColorLight);
            var Heading3 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.BOLD, FColor);
            var Heading4 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColor);
            var Heading5 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 9, iTextSharp.text.Font.NORMAL, FColor);

            Phrase phrase;
            PdfPCell cell;

            PdfPTable headertable2 = new PdfPTable(14);
            headertable2.SetWidths(new float[] { 6.5f, 7, 7, 7, 7, 7, 7.5f, 4, 7, 7, 7, 7, 7, 10 });
            PdfPTable headertable3 = new PdfPTable(10);
            headertable3.SetWidths(new float[] { 11, 10, 2.5f, 18.5f, 2.5f, 18.5f, 2.5f, 18.5f, 2.5f, 18.5f });
            PdfPTable headertable1 = new PdfPTable(3);
            headertable1.SetWidths(new int[] { 47, 3, 50 });
            PdfPTable headertable4 = new PdfPTable(21);
            headertable4.SetWidths(new int[] { 4, 1, 10, 1, 10, 10, 1, 6, 1, 6, 1, 4, 1, 10, 1, 10, 10, 1, 6, 1, 6 });
            PdfPTable headertable5 = new PdfPTable(16);
            headertable5.SetWidths(new int[] { 5, 15, 10, 10, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 });

            foreach (var data in ProofApplications)
            {

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 14;
                cell.PaddingBottom = -5f;
                cell.Border = 0;
                headertable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(data.ApplicationLocation, Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable2.AddCell(cell);

                if (data.MockUpImage != null && data.MockUpImage != "")
                {
                    string MockImage = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + data.MockUpImage);
                    cell = new PdfPCell(iTextSharp.text.Image.GetInstance(MockImage), true);
                    cell.Colspan = 6;
                    cell.FixedHeight = 230;
                    cell.PaddingBottom = 0f;
                    cell.Border = 0;
                    headertable2.AddCell(cell);
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 6;
                    cell.FixedHeight = 230;
                    cell.PaddingBottom = 0f;
                    cell.Border = 0;
                    headertable2.AddCell(cell);
                }

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable2.AddCell(cell);

                if (data.AppImage != null && data.AppImage != "")
                {
                    string ApplicationImage = Path.Combine(Server.MapPath("~/Content/uploads/Application/") + data.AppImage);
                    cell = new PdfPCell(iTextSharp.text.Image.GetInstance(ApplicationImage), true);
                    cell.Colspan = 6;
                    cell.FixedHeight = 230;
                    cell.PaddingBottom = 0f;
                    cell.Border = 0;
                    headertable2.AddCell(cell);
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 6;
                    cell.FixedHeight = 230;
                    cell.PaddingBottom = 0f;
                    cell.Border = 0;
                    headertable2.AddCell(cell);
                }
                //Next Row


                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 10;
                //cell.PaddingTop = 5f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Application No: " + Convert.ToString(data.ApplicationId), Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingTop = -3f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                for (i = 0; i < 4; i++)
                {
                    if (data.Colours.Count > i)
                    {

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading4));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        if (data.Colours[i].HexvalueColour != null && data.Colours[i].HexvalueColour != "")
                        {
                            var ColourHexCode = data.Colours[i].HexvalueColour;
                            if (data.Colours[i].HexvalueColour == "NA" || data.Colours[i].HexvalueColour == "TBC" || data.Colours[i].HexvalueColour.Contains("White"))
                            {
                                ColourHexCode = "ffffff";
                            }
                            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#" + Convert.ToString(ColourHexCode)));
                        }
                        cell.BorderColor = ColourChipBorder;
                        headertable3.AddCell(cell);

                        var AppColor = "";
                        if (data.AppType == "Digital Print" || data.AppType == "Screen Print")
                        {
                            AppColor = data.Colours[i].InkColour;
                        }
                        else if (data.AppType == "Embroidery")
                        {
                            AppColor = data.Colours[i].ThreadColour;
                        }
                        else
                        {
                            AppColor = data.Colours[i].TransferColour;
                        }

                        phrase = new Phrase();
                        phrase.Add(new Chunk(" " + Convert.ToString(AppColor) + " " + Convert.ToString(data.Colours[i].PantoneName), Heading5));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.Border = 0;
                        headertable3.AddCell(cell);
                    }
                    else
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading4));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 2;
                        cell.Border = 0;
                        headertable3.AddCell(cell);
                    }
                }

                phrase = new Phrase();
                phrase.Add(new Chunk("Type: " + Convert.ToString(data.AppType), Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.PaddingTop = -3;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 8;
                cell.PaddingBottom = -5f;
                cell.Border = 0;
                headertable3.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Image Width: " + Convert.ToString(data.AppWidth), Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.Border = 0;
                headertable3.AddCell(cell);


                for (i = 4; i < 8; i++)
                {
                    if (data.Colours.Count > i)
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading4));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        if (data.Colours[i].HexvalueColour != null && data.Colours[i].HexvalueColour != "")
                        {
                            var ColourHexCode = data.Colours[i].HexvalueColour;
                            if (data.Colours[i].HexvalueColour == "NA" || data.Colours[i].HexvalueColour == "TBC" || data.Colours[i].HexvalueColour.Contains("White"))
                            {
                                ColourHexCode = "ffffff";
                            }
                            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#" + Convert.ToString(ColourHexCode)));
                        }
                        cell.BorderColor = ColourChipBorder;
                        headertable3.AddCell(cell);

                        var AppColor = "";
                        if (data.AppType == "Digital Print" || data.AppType == "Screen Print")
                        {
                            AppColor = data.Colours[i].InkColour;
                        }
                        else if (data.AppType == "Embroidery")
                        {
                            AppColor = data.Colours[i].ThreadColour;
                        }
                        else
                        {
                            AppColor = data.Colours[i].TransferColour;
                        }

                        phrase = new Phrase();
                        phrase.Add(new Chunk("  " + Convert.ToString(AppColor) + " " + Convert.ToString(data.Colours[i].PantoneName), Heading5));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 1;
                        cell.Border = 0;
                        headertable3.AddCell(cell);
                    }
                    else
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk(" ", Heading4));
                        cell = new PdfPCell(phrase);
                        cell.Colspan = 2;
                        cell.Border = 0;
                        headertable3.AddCell(cell);
                    }

                }
                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading4));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 12;
                //cell.PaddingBottom = -2f;
                //cell.Border = 0;
                //headertable3.AddCell(cell);

                doc.Add(headertable2);
                headertable2.DeleteBodyRows();

                doc.Add(headertable3);
                headertable3.DeleteBodyRows();

                rcount++;
            }

            if (rcount % 2 != 0)
            {
                string Placeholder = Path.Combine(Server.MapPath("~/Images/Place-Holder.jpg"));
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(Placeholder), true);
                cell.Colspan = 14;
                cell.PaddingTop = 15;
                cell.PaddingLeft = -15;
                cell.PaddingRight = -15;
                cell.FixedHeight = 275;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                headertable2.AddCell(cell);
            }
            //Footer at the end of documentation.
            string jobtype = PdfType;
            if (jobtype == "Proof")
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = -3f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("PRODUCTION SIGN OFF:", Heading1));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading1));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("The Fine Print", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Postitional Mock Up only - not exactly to scale. Please check all details, spelling, print size & colours. * Colours are a digital representation only and can vary on different monitors, please check colours against a Pantone® Colour Guide. \n If an edit is required, please reply to this email with a description of what needs to change.", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 0f;
                cell.SetLeading(1.1f, 1.1f);
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading1));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("• Order turnaround times quoted commence from the moment we receive your approval to proceed.Any delays in getting the approval back to us will result in a delay to your order. \n• We’re proud of our work & occasionally like to share(show off) images of the jobs we produce through our social media channels. \n• If you would prefer we do not promote your product, please add “DO NOT SHARE” to your approval email.", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 8, iTextSharp.text.Font.NORMAL, FColorLight)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                cell.SetLeading(1.1f, 1.1f);
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("OK TO PROCEED? ", Heading1));
                phrase.Add(new Chunk(" Simply reply to this email with the word ‘Approved’", FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 12, iTextSharp.text.Font.NORMAL, FColor)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                cell.SetLeading(1.2f, 1.2f);
                headertable1.AddCell(cell);
            }

            //2nd footer

            else if (jobtype == "JobSheet")
            {

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 21;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("FILM", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("INKCOLOR", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("PANTONE", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("INKID", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("MESH", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("ORDER", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("FILM", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("INKCOLOUR", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("PANTONE", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("INKID", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("MESH", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("ORDER", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                headertable4.AddCell(cell);

                //2 row
                phrase = new Phrase();
                phrase.Add(new Chunk("A", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("A", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                //3 row

                phrase = new Phrase();
                phrase.Add(new Chunk("B", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("B", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                //4 row

                phrase = new Phrase();
                phrase.Add(new Chunk("C", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("C", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                //5 row

                phrase = new Phrase();
                phrase.Add(new Chunk("D", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("D", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                //6 row

                phrase = new Phrase();
                phrase.Add(new Chunk("E", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("E", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                // 7 row

                phrase = new Phrase();
                phrase.Add(new Chunk("F", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("F", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.Border = 0;
                cell.PaddingTop = 5;
                headertable4.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 5;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = FColorLight;
                headertable4.AddCell(cell);
            }
            else
            {
                //3rd footer

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 16;
                //cell.PaddingTop = 1f;
                cell.Border = 0;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ctn No", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Garment", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ladies/Men", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Colour", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("6", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("8", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("10", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("12", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("14", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("16", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("18", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("20", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("22", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("24", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                headertable5.AddCell(cell);

                // next row

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("XXS", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("XS", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("S", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("M", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("L", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("XL", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("2XL", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("3XL", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("4XL", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("5XL", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 1f;
                cell.BorderColor = FColorLight;
                cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                headertable5.AddCell(cell);

                // next row
                for (i = 0; i <= 3; i++)
                {

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.PaddingBottom = 12f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("", Heading4));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.PaddingTop = 1f;
                    cell.BorderColor = FColorLight;
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                    headertable5.AddCell(cell);
                }
            }

            doc.Add(headertable2);
            doc.Add(headertable3);
            if (jobtype == "Proof")
            {
                doc.Add(headertable1);
            }
            else if (jobtype == "JobSheet")
            {
                doc.Add(headertable4);
            }
            else
            {
                doc.Add(headertable5);
            }
            doc.Close();
            doc.Dispose();
            return View();

        }

        // Header & Footer For Confirmation Pdf
        public class HeaderFooterStatementforProof : PdfPageEventHelper
        {
            public int OpportunityId { get; set; }
            public int OptionId { get; set; }
            public string OptionStage { get; set; }
            public string PdfType { get; set; }

            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                KENNEWEntities dbContext = new KENNEWEntities();

                var HeaderInfo = dbContext.Pro_ProofHeaderInfo(OpportunityId, OptionId, OptionStage).FirstOrDefault();

                Rectangle pageSize = doc.PageSize;

                var FColor = new BaseColor(38, 38, 38);

                var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 24, iTextSharp.text.Font.NORMAL, FColor);
                var Heading2 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 9, iTextSharp.text.Font.BOLD, FColor);
                var Heading3 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 9, iTextSharp.text.Font.NORMAL, FColor);

                Phrase phrase;
                PdfPCell cell;

                PdfPTable headertable1 = new PdfPTable(7);
                headertable1.SetWidths(new int[] { 8, 19, 5, 20, 21, 10, 17 });

                string ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Proof-Header.png");
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(ImagePath), true);
                cell.Border = 0;
                cell.PaddingTop = -18;
                cell.Colspan = 7;
                cell.FixedHeight = 58;
                cell.PaddingRight = -19;
                cell.PaddingLeft = -19;
                headertable1.AddCell(cell);

                //next row

                phrase = new Phrase();
                phrase.Add(new Chunk("Job No :", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.PaddingTop = -10f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(Convert.ToString(HeaderInfo.OpportunityId), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingTop = -10f;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                String jobtype = PdfType;

                phrase = new Phrase();
                if (jobtype == "PackingList")
                {
                    phrase.Add(new Chunk("CUSTOM MERCHANDISE", Heading1));
                }
                else
                {
                    phrase.Add(new Chunk("CUSTOM MERCHANDISE PROOF", Heading1));
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.Rowspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingTop = -10f;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.PaddingTop = -10f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Job Name :", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(Convert.ToString(HeaderInfo.oppname), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Date :", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //var CurrentTimestamp = string.Format("{0:dd/MM/yyyy hh:mm}", DateTime.Now);
                var version = 1;
                if (HeaderInfo.ProofVerion != null)
                {
                    version = Convert.ToInt32(HeaderInfo.ProofVerion);
                }
                phrase = new Phrase();
                phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy hh:mm") + " - V" + version.ToString(), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                // next row

                phrase = new Phrase();
                phrase.Add(new Chunk("Quantity :", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(Convert.ToString(HeaderInfo.quantity), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                if (jobtype == "PackingList")
                {
                    phrase.Add(new Chunk("        PACKING LIST", Heading1));
                }
                else
                {
                    phrase.Add(new Chunk(" ", Heading1));
                }
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.Rowspan = 2;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contact :", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(Convert.ToString(HeaderInfo.ContactName), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                // next row

                phrase = new Phrase();
                phrase.Add(new Chunk("Sizes: ", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(Convert.ToString(HeaderInfo.InitialSizes), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Organisation :", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(Convert.ToString(HeaderInfo.OrgName), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                //Next Row
                phrase = new Phrase();
                phrase.Add(new Chunk("Item :", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(Convert.ToString(HeaderInfo.Item), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 4;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Job Manager :", Heading2));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(Convert.ToString(HeaderInfo.JobManager), Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;


                headertable1.AddCell(cell);
                doc.Add(headertable1);
            }
        }
        public void LoadImage(string InputPDFFile, string JpegPath)
        {
            string outImageName = Path.GetFileNameWithoutExtension(InputPDFFile);
            try
            {
                Spire.Pdf.PdfDocument pdfdocument = new Spire.Pdf.PdfDocument(InputPDFFile);
                for (int i = 0; i < pdfdocument.Pages.Count; i++)
                {
                    System.Drawing.Image image = pdfdocument.SaveAsImage(i, 96, 96);
                    image.Save(string.Format(JpegPath + outImageName + i + ".Jpeg", i), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}