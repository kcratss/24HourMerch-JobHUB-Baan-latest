using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using KEN_DataAccess;
using AutoMapper;
using KEN.Interfaces.Iservices;
using KEN.Interfaces;
using KEN.Filters;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class OrganisationController : Controller
    {
     
        ResponseViewModel response = new ResponseViewModel();

        private readonly IOrganisationService _baseService;
        private readonly IOpportunityService _OppbaseService;

        KENNEWEntities dbContext = new KENNEWEntities();

        public OrganisationController(IOrganisationService baseService, IOpportunityService OppbaseService)
        {
            _baseService = baseService;
            _OppbaseService = OppbaseService;
        }
        public ActionResult OrganisationList()
        {
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();

        }
        public ActionResult GetOrganisationList(string Type)
        {
            var OrganisationList = _baseService.GetOrganisationList(Type);
            var Jsonresult = Json(OrganisationList, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;

        }

        public ActionResult AddOrganisation(OrganisationViewModel model)
        {
            if (model != null)
            {
                var Entity = Mapper.Map<tblOrganisation>(model);
                response = _baseService.OrganisationCheck(Entity,model.PageSource, model.ContactID,model.PurchaseId);           //1 Sep 2018(N)
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddOrganisationAddress(AddressViewModel model)
        {
            if (model != null)
            {
                var Entity = Mapper.Map<tblAddress>(model);
                response = _baseService.UpdateAddress(Entity,model.PageSource,model.PurchaseId, model.OppId);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        // baans change 27th September for Autocomplete  by type
        public ActionResult GetOrganisationByFirstName(string Prefix, string OrgType)        
        {
            // baans end 27th Sept
            var data = _baseService.GetOrganisationByPrefix(Prefix, OrgType);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult CheckDeliveryAddress(int OrgId)
        {
            List<AddressViewModel> addressList = new List<AddressViewModel>();
            List<tblAddress> addressEntity =dbContext.tblAddresses.Where(_ => _.OrgId == OrgId).ToList();
            foreach (var item in addressEntity)
            {
                addressList.Add(Mapper.Map<AddressViewModel>(item));
            }
           
            return Json(addressList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrganisationAddress(int OrgId, string AddType)
        {
            AddressViewModel data = new AddressViewModel();
            //var data = _baseService.GetOrganisationAddress(OrgId,AddType);
            //tarun 07/09/2018
            if (AddType != "" && AddType == "Purchase")
            {
                data = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.AddressId == OrgId).FirstOrDefault());
            }
            else
            {
                if (AddType != "")
                {

                    data = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.OrgId == OrgId && _.DeliveryAddress == AddType).FirstOrDefault());
                }
                else
                {
                    data = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.OrgId == OrgId).FirstOrDefault());
                }
            }
            //end
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // baans change 21st Sept
        public ActionResult GetAddressStatus(int OppId)
        {
            var Status = true;
            var data = dbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).Select(_ => _.AddressId).FirstOrDefault();
            if(data == null || data == 0)
            {
                Status = false;
            }
            return Json(Status, JsonRequestBehavior.AllowGet);
        }

            // baans end 21st Sept

        public ActionResult GetOrganisationById(int OrgId)
        {
            var data = Mapper.Map<OrganisationViewModel>(_baseService.GetOrganisationById(OrgId));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrganisationDetails(int id)
        {
            ViewBag.ManagerList = GetAccountManagers();
            ViewBag.ContactTypes = GetContactType();
            ViewBag.ContactTitle = GetContactTitle();
            //ViewBag.ContactRoles = GetContactRoles();
            ViewBag.StateList = GetAllStateList();
            ViewBag.Id = id;
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }

        public List<StateList> GetAllStateList()
        {
            List<StateList> objState = new List<StateList>();
            //objState.Add(new StateList { stateName = "New South Wales" });
            //objState.Add(new StateList { stateName = "Queensland south" });
            //objState.Add(new StateList { stateName = "South Australia" });
            //objState.Add(new StateList { stateName = "Tasmania" });
            //objState.Add(new StateList { stateName = "Victoria" });
            //objState.Add(new StateList { stateName = "Western Australia" });
            //objState.Add(new StateList { stateName = "South west" });
            //objState.Add(new StateList { stateName = "South west india" });
            objState.Add(new StateList { stateName = "ACT" }); //added by baans 17Aug2020
            objState.Add(new StateList { stateName = "NSW" });
            objState.Add(new StateList { stateName = "NT" });
            objState.Add(new StateList { stateName = "QLD" });
            objState.Add(new StateList { stateName = "SA" });
            objState.Add(new StateList { stateName = "TAS" });
            objState.Add(new StateList { stateName = "VIC" });
            objState.Add(new StateList { stateName = "WA" });

            return objState;
        }

        public List<AccountManagerDropdownViewModel> GetAccountManagers()
        {

            // string[] Roles = new string[] { "Administrator", "Account Manager","Production Director"};
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.title)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }


        public List<EnumViewModel> GetContactType()
        {
            var getData = (from ContactType e in Enum.GetValues(typeof(ContactType)) select new { Name = e.ToString() }).ToList();
            var newData = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).ToList();
            return newData;
        }

        //public List<EnumViewModel> GetContactRoles()
        //{
        //    var getData = (from ContactRole e in Enum.GetValues(typeof(ContactRole)) select new { Name = e.ToString() }).ToList();
        //    var newData = getData.Select(item => new EnumViewModel
        //    {
        //        Name = item.Name,
        //    }
        //    ).ToList();
        //    return newData;
        //}

        public List<EnumViewModel> GetContactTitle()
        {
            var getData = (from ContactTitle e in Enum.GetValues(typeof(ContactTitle)) select new { Name = e.ToString() }).ToList();
            var newData = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }).ToList();
            return newData;
        }

        //tarun 22/08/2018
        public ActionResult GetOrgByType()
        {

            OrganisationViewModel data = new OrganisationViewModel();
            data = Mapper.Map<OrganisationViewModel>(_baseService.GetOrgByType());

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //end
        //tarun 08/09/2018
        public ActionResult GetCustomOrganisationList(string CustomText,string TableName)
        {
            var OrgTypeList = _baseService.GetCustomOrganisationList(CustomText, TableName);
            var Jsonresult = Json(OrgTypeList, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }
        //end

        //Address Pdf ###############################################################################################################

        //9 Aug 2018 (N)
        public ActionResult PrintAddressPdf(int Oppid)
        {
            var opp = _OppbaseService.GetOppById(Oppid);

            var address = opp.AddressId;

            var dAddress = Mapper.Map<AddressViewModel>(dbContext.tblAddresses.Where(_ => _.AddressId == address).FirstOrDefault());

            var FColor = new BaseColor(51, 51, 51);

            Document doc = new Document();
            Rectangle one = new Rectangle(422.36208f, 289.134f);
            doc.SetPageSize(one);
            doc.SetMargins(-40f, -45f, 0f, 4f);
            PdfWriter write;
            HeaderFooterStatement page;
           
            write = PdfWriter.GetInstance(doc, Response.OutputStream);
            page = new HeaderFooterStatement();
            page.Oppid = Oppid;
            write.PageEvent = page;
            Response.ContentType = ("application/pdf");
            
            doc.Open();

            var Heading1 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 24, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            //var Heading2 = FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 27, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Phrase phrase;
            PdfPCell cell;

            PdfPTable headertable1 = new PdfPTable(4);
            headertable1.SetWidths(new int[] { 12, 13, 11, 11 });

            string ImagePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Adddress.PNG");
            cell = new PdfPCell(iTextSharp.text.Image.GetInstance(ImagePath), true);
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.PaddingTop = 10;
            cell.Colspan = 4;
            cell.FixedHeight = 76;
            cell.PaddingRight = -10;
            cell.PaddingLeft = -10;
            headertable1.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("Job Name:", FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 25, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            //cell.PaddingLeft = -10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            headertable1.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk(opp.OppName.ToString(), FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            //cell.PaddingLeft = -10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 3f;
            cell.PaddingTop = 8f;
            cell.Border = 0;
            headertable1.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("SHIP TO:", FontFactory.GetFont((Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 30, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            //cell.PaddingLeft = -10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 4f;
            cell.Border = 0;
            headertable1.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk(dAddress.TradingName, FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 4f;
            cell.Border = 0;
            headertable1.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("", Heading1));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            //cell.PaddingLeft = -10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 3f;
            cell.Border = 0;
            headertable1.AddCell(cell);

            if (dAddress.Attention != null)
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(dAddress.Attention, FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.PaddingTop = -10f;
                cell.Border = 0;
                headertable1.AddCell(cell);
            }
            else
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.PaddingTop = -10f;
                cell.Border = 0;
                headertable1.AddCell(cell);
            }

            phrase = new Phrase();
            phrase.Add(new Chunk("", Heading1));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            //cell.PaddingLeft = -10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 3f;
            cell.Border = 0;
            headertable1.AddCell(cell);

            if (dAddress.Address1 != null)
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(dAddress.Address1, FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                headertable1.AddCell(cell);
            }
            else
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                headertable1.AddCell(cell);
            }

            phrase = new Phrase();
            phrase.Add(new Chunk("", Heading1));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            //cell.PaddingLeft = -10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 3f;
            cell.Border = 0;
            headertable1.AddCell(cell);

            if (dAddress.Address2 != null)
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(dAddress.Address2, FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                headertable1.AddCell(cell);
            }
            else
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 3f;
                cell.Border = 0;
                headertable1.AddCell(cell);
            }
            phrase = new Phrase();
            phrase.Add(new Chunk("", Heading1));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            //cell.PaddingLeft = -10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 4f;
            cell.Border = 0;
            headertable1.AddCell(cell);

            if (dAddress.State != null)
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(dAddress.State + "  ", FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                phrase.Add(new Chunk(dAddress.Postcode, FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 4f;
                cell.Border = 0;
                headertable1.AddCell(cell);
            }
            else
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(" " + "  ", FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                phrase.Add(new Chunk(dAddress.Postcode, FontFactory.GetFont((Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 19, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 6f;
                cell.Border = 0;
                headertable1.AddCell(cell);
            }
           
            //phrase = new Phrase();
            //phrase.Add(new Chunk(" ", Heading1));
            //cell = new PdfPCell(phrase);
            //cell.Colspan = 4;
            ////cell.PaddingLeft = -10f;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //cell.PaddingBottom = -12f;
            //cell.Border = 0;
            //headertable1.AddCell(cell);
            
            doc.Add(headertable1);
            doc.Close();
            return View();
        }
        //9 Aug 2018 (N)

        public class HeaderFooterStatement : PdfPageEventHelper
        {
            public int Oppid { get; set; }
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                
                KENNEWEntities dbContext = new KENNEWEntities();

                var opp = dbContext.tblOpportunities.Where(_ => _.OpportunityId == Oppid).FirstOrDefault();
                
                Rectangle pageSize = doc.PageSize;

                var FColor = new BaseColor(51, 51, 51);
                var Heading1 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 24, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                Phrase phrase;
                PdfPCell cell;

                PdfPTable headertable1 = new PdfPTable(4);
                headertable1.SetWidths(new int[] { 12, 13, 11, 11 });


                phrase = new Phrase();
                phrase.Add(new Chunk("SHIP Via:", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 25, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 2f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(opp.Shipping.ToString(), FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto.regular.ttf")), BaseFont.CP1252, true, 18, iTextSharp.text.Font.NORMAL, FColor)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 8f;
                cell.PaddingLeft = 26f;
                cell.Border = 0;
                headertable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ctn:______of______", FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 22, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cell = new PdfPCell(phrase);
                cell.Colspan = 2;
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingBottom = 5f;
                cell.Border = 0;
                headertable1.AddCell(cell);


                headertable1.TotalWidth = pageSize.Width - doc.LeftMargin - doc.RightMargin;
                headertable1.WriteSelectedRows(0, -1, doc.LeftMargin, headertable1.TotalHeight + doc.BottomMargin, writer.DirectContent);
                //doc.Add(headertable1);
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
    }
}