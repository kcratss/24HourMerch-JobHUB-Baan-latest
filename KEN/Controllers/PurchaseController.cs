using System;
using System.Collections.Generic;
using System.Linq;
using KEN_DataAccess;
using KEN.Models;
using KEN.Interfaces;
using KEN.Services;
using KEN.Interfaces.Iservices;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using KEN.Filters;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PostmarkDotNet;
using KEN.AppCode;
using System.Configuration;
using Intuit.Ipp.Security;
using Intuit.Ipp.Core;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Data;
using Intuit.Ipp.LinqExtender;
using Intuit.Ipp.QueryFilter;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using System.IO;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class PurchaseController : Controller
    {
        public static String REQUEST_TOKEN_URL = ConfigurationManager.AppSettings["GET_REQUEST_TOKEN"];
        public static String ACCESS_TOKEN_URL = ConfigurationManager.AppSettings["GET_ACCESS_TOKEN"];
        public static String AUTHORIZE_URL = ConfigurationManager.AppSettings["AuthorizeUrl"];
        public static String OAUTH_URL = ConfigurationManager.AppSettings["OauthLink"];
        public String consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        public String consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        //public static String DomaimURL = ConfigurationManager.AppSettings["DomaimURL"];

        public static string ServiceContextUrl = ConfigurationManager.AppSettings["ServiceContext.BaseUrl.Qbo"];

        public static string strrequestToken = string.Empty;
        public static string tokenSecret = string.Empty;


        private readonly IOpportunityService _baseService;
        private readonly IPurchaseService _purchasebaseService;
        private readonly IOrganisationService _OrgbaseService;      //4 Sept 2018 (N)

        KENNEWEntities DbContext = new KENNEWEntities();

        ResponseViewModel response = new ResponseViewModel();

        public static string PostmarkToken = ConfigurationManager.AppSettings["PostmarkToken"].ToString();

        public PurchaseController(IOpportunityService baseService, IPurchaseService baseService1, IOrganisationService OrgbaseService){     //4 Sept 2018 (N)
            _baseService = baseService;
            _purchasebaseService = baseService1;
            _OrgbaseService = OrgbaseService;       //4 Sept 2018 (N)
        }
        // GET: Purchase
        public ActionResult PurchaseDetails(int Id, int OppId)
        {

            ViewBag.DepartmentList = GetDepartments();
            ViewBag.ManagerList = GetAccountManagers();
            ViewBag.CampaignList = GetCampaign();
            ViewBag.ContactTypes = GetContactType();
           // ViewBag.ContactRoles = GetContactRoles();
            ViewBag.StageList = GetStage();
            ViewBag.SourceList = GetSource();
            ViewBag.Cycles = GetCycles();
            ViewBag.SizeType = GetAllState();
            ViewBag.DecorationList = GetDecorationList();
            ViewBag.BrandList = GetBrandList();
            ViewBag.ItemList = GetItemList();
            ViewBag.ShippingList = GetShipping();
            ViewBag.PaymentMethodList = GetPaymentMethodList();
            ViewBag.ID = Id;
            ViewBag.OppId = OppId;
            ViewBag.StateList = GetAllStateList();
            ViewBag.PurchaseStatus = GetPurchaseStatus();
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November

            return View();
        }
        public ActionResult GetPurchById(int Id,int OppId)
        {
            
            var model = new opportunityViewModel();
            var data = Mapper.Map<tblPurchase>(_purchasebaseService.GetPurchById(Id));
            if (OppId > 0)
            {
                //var ema = _purchasebaseService.GetPurchEmailById(id);
                model = Mapper.Map<opportunityViewModel>(_purchasebaseService.GetPurchEmailById(OppId));
                
                //return Json(data, JsonRequestBehavior.AllowGet);
            }
            //return Json(data, model, JsonRequestBehavior.AllowGet);
            return Json(new { data = data, model = model }, JsonRequestBehavior.AllowGet);

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
        //public ActionResult Purchase()
        //{
        //    var data = _baseService.GetCustomOppList();

        //}
        // Baans change 19th Sept
        public ActionResult GetBrandId(string BrandName)
        {
            var AddId = DbContext.tblbands.Where(_ => _.name == BrandName).FirstOrDefault();
            return Json(AddId, JsonRequestBehavior.AllowGet);
        }
        // baans end 19th Sept

        public ActionResult OpportunityDetails(int Id)
        {
            ViewBag.DepartmentList = GetDepartments();
            ViewBag.ManagerList = GetAccountManagers();
            ViewBag.CampaignList = GetCampaign();
            ViewBag.ContactTypes = GetContactType();
            //ViewBag.ContactRoles = GetContactRoles();
            ViewBag.StageList = GetStage();
            ViewBag.SourceList = GetSource();
            ViewBag.Cycles = GetCycles();
            ViewBag.SizeType = GetAllState();
            ViewBag.DecorationList = GetDecorationList();
            ViewBag.BrandList = GetBrandList();
            ViewBag.ItemList = GetItemList();
            ViewBag.ShippingList = GetShipping();
            ViewBag.PaymentMethodList = GetPaymentMethodList();
            ViewBag.ID = Id;
            return View();
        }
        public IEnumerable<tbldepartment> GetDepartments()
        {
            var getData = DbContext.tbldepartments.ToList().OrderBy(_ => _.department);
            return getData;
        }
        public List<AccountManagerDropdownViewModel> GetAccountManagers()
        {

            // string[] Roles = new string[] { "Administrator", "Account Manager","Production Director"};
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(DbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.title)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        public IEnumerable<tblCampaign> GetCampaign()
        {
            var getData = DbContext.tblCampaigns.ToList().OrderBy(_ => _.Campaign);
            return getData;
        }

        public List<EnumViewModel> GetContactType()
        {
            var getData = (from ContactType e in Enum.GetValues(typeof(ContactType))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }
        //public List<EnumViewModel> GetContactRoles()
        //{
        //    var getData = (from ContactRole e in Enum.GetValues(typeof(ContactRole))
        //                   select new { Name = e.ToString() }).ToList();
        //    var newdata = getData.Select(item => new EnumViewModel
        //    {
        //        Name = item.Name,
        //    }
        //    ).OrderBy(_ => _.Name).ToList();
        //    return newdata;
        //}
        public IEnumerable<EnumViewModel> GetStage()
        {
            var getData = (from Stages e in Enum.GetValues(typeof(Stages))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).ToList();
            return newdata;
        }
        public List<EnumViewModel> GetSource()
        {

            var getData = (from SourceEnum e in Enum.GetValues(typeof(SourceEnum))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }
        public List<EnumViewModel> GetCycles()
        {
            var getData = (from Cycles e in Enum.GetValues(typeof(Cycles))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }
        public List<sizeType> GetAllState()
        {
            List<sizeType> objstate = new List<sizeType>();
            objstate.Add(new sizeType { value = "", typeName = "--Select--" });
            objstate.Add(new sizeType { value = "Mens", typeName = "Mens" });
            objstate.Add(new sizeType { value = "Shirts", typeName = "Shirts" });
            objstate.Add(new sizeType { value = "Shorts/Pants", typeName = "Shorts/Pants" });
            objstate.Add(new sizeType { value = "Toddlers/Youth", typeName = "Toddlers/Youth" });
            objstate.Add(new sizeType { value = "Womens", typeName = "Womens" });
            // baans change 13th November for old Women
            objstate.Add(new sizeType { value = "Womens_Old", typeName = "Womens_Old" });
            // baans end 13th November
            objstate.Add(new sizeType { value = "Youths", typeName = "Youths" });
            objstate.Add(new sizeType { value = "Custom", typeName = "Custom" });
            return objstate.OrderBy(_ => _.value).ToList();

        }
        public List<DecorationViewModel> GetDecorationList()
        {

            var DecorationList = _baseService.GetDecorationList();
            return DecorationList;

        }
        public IEnumerable<tblband> GetBrandList()
        {
            var getData = DbContext.tblbands.ToList().OrderBy(_ => _.name);
            return getData;
        }
        public IEnumerable<tblitem> GetItemList()
        {
            var getData = DbContext.tblitems.ToList().OrderBy(_ => _.name);
            return getData;
        }
        public List<EnumViewModel> GetShipping()
        {
            var getData = (from Shipping e in Enum.GetValues(typeof(Cycles))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }
        public IEnumerable<EnumViewModel> GetPaymentMethodList()
        {
            var getData = (from KEN.Models.PaymentMethod e in Enum.GetValues(typeof(KEN.Models.PaymentMethod))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).ToList();
            return newdata;
        }
        public ActionResult GetPurchaseDetailOptionGrid(int PurchaseId)
        {
            var OptionData = _purchasebaseService.GetPurchaseDetailOptionGrid(PurchaseId);
            var ShippingPrice = DbContext.tblPurchases.Where(_ => _.PurchaseId == PurchaseId).Select(_ => _.ShippingCharge).FirstOrDefault();
            
            double Total = 0;
            double SubTotal = 0;
            double Tax = 0;
            double ShippingTax = 0;
            double Shipping = 0;

            foreach(var item in OptionData)
            {
                item.ExtExGST = item.ExtExGST == null ? 0 : item.ExtExGST;
                //item.ExtInclGST = item.ExtInclGST == null ? 0 : item.ExtInclGST;

                SubTotal += Convert.ToDouble(item.ExtExGST);
                //Total += Convert.ToDouble(item.ExtInclGST);
            }

            Tax = SubTotal / 10;

            if(ShippingPrice != null)
            {
                Shipping = Convert.ToDouble(ShippingPrice);
                ShippingTax = Shipping / 10;
                Tax = Tax + ShippingTax;
            }

            Total = SubTotal + Shipping + Tax;

            var result = new { data = OptionData, Total = Math.Round(Total, 2), Shipping = Math.Round(Shipping, 2), SubTotal = Math.Round(SubTotal, 2), Tax = Math.Round(Tax, 2)};

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdatePurchaseOption(PurchaseDetailViewModel model)
        {

            if (model != null)
            {
                //model.include_job = model.include == "Yes" ? true : false;
                var Entity = Mapper.Map<tblPurchaseDetail>(model);
                if (Entity.PurchaseDetailId > 0)
                {
                    response = _purchasebaseService.OptionBatchTransaction(Entity, BatchOperation.Update);
                }
                else
                {
                    response = _purchasebaseService.OptionBatchTransaction(Entity, BatchOperation.Insert);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PurchaseDelete(PurchaseDetailViewModel model)
        {
            if (model != null)
            {
                //model.include_job = model.include == "Yes" ? true : false;
                var Entity = Mapper.Map<tblPurchaseDetail>(model);
                if (Entity.PurchaseDetailId > 0)
                {
                    response = _purchasebaseService.OptionBatchTransaction(Entity, BatchOperation.Delete);
                }
                
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SavePurDetail(PurchaseViewModel model)
        {
            if (model != null)
            {
                var Entity = Mapper.Map<tblPurchase>(model);
                if (model.PurchaseId > 0)
                {
                    response = _purchasebaseService.PurchaseBatchTransaction(Entity, BatchOperation.Update);
                }
                else
                {
                    response = _purchasebaseService.PurchaseBatchTransaction(Entity, BatchOperation.Insert);
                    
                }

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //public List<PurchaseDetailViewModel> GetOptionToPurchaseDetail(int PurchaseId, int OppId)
        //{
        //    //var optiontopurchase = _purchasebaseService.GetOptionToPurchaseDetail(PurchaseId, OppId);
        //    var optiontopurchase = Mapper.Map<List<PurchaseDetailViewModel>>(dbContext.Database.SqlQuery<PurchaseDetailViewModel>("exec pro_optiontopurchasedetail '" + OppId + "','" + PurchaseId + "'").ToList()).ToList();
        //    //var optiontopurchase = dbContext.pro_optiontopurchasedetail(PurchaseId, OppId);
        //    return optiontopurchase;
        //}
        //tarun 01/09/2018
        public List<EnumViewModel> GetPurchaseStatus()
        {
            var getData = (from PurchaseStatus e in Enum.GetValues(typeof(PurchaseStatus))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).ToList();
            return newdata;
        }
        //tarun 07/09/2018
        //public tblPurchase GetSupplierByPurchaseId(int PurchaseId)
        //{
        //    var purchasedata = _purchasebaseService.GetSupplierByPurchaseId(PurchaseId);
        //    return purchasedata;
        //}
        //end


        //Prashant
        public ActionResult PurchaseList()
        {
            ViewBag.OrgName = GetSupplierName();
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }

        public ActionResult GetPurchaseList(string PurchaseTabs, string DateFrom, string DateTo, string Supplier)
        {
            var data = _purchasebaseService.GetPurchaseList(PurchaseTabs, DateFrom, DateTo, Supplier);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<OrganisationViewModel> GetSupplierName()
        {

            var data = _purchasebaseService.GetSupplierName();
            return data;
        }
        //Prashant End
        //8 Sep 2018(N)
        public ActionResult GetCustomPurchaseList(string CustomText, string TableName)
        {
            var PurchaseListData = _purchasebaseService.GetCustomPurchaseList(CustomText, TableName);
            var Jsonresult = Json(PurchaseListData, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }
        //8 Sep 2018(N)


        //#####################################################################################################################
        public ActionResult PerformaInvoicePdf(int Id, string PdfType, string path)
        {
            var model = new PurchaseViewModel();
            if (Id > 0)
            {
                model = Mapper.Map<PurchaseViewModel>(_purchasebaseService.GetPurchById(Id));

            }

            var PurchaseData = DbContext.Pro_PurchaseData(Id).FirstOrDefault();

            var PurOptionData = DbContext.Pro_PurchaseOptionData(Id).ToList();

            Document doc = new Document(PageSize.A4, -35f, -35f, 20f, 20f);   //Page Size set top ,left,margin
            doc.SetPageSize(PageSize.A4);
            PdfWriter write;

            var page = new HeaderFooterStatementforOrder();
            page.id = Id;

            if (PdfType == "Print")
            {
                write = PdfWriter.GetInstance(doc, Response.OutputStream);
                Response.ContentType = ("application/pdf");
                write.PageEvent = page;
            }
            else
            {
                write = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                write.PageEvent = page;
            }
            
            doc.Open();

            int i, j, k = 8;

            var rcount = 0;
            var FColor = new BaseColor(38, 171, 227);  //40, 171, 255        //font color declaration for highlighted data
            var RColor = new BaseColor(230, 230, 230);            //font color declaration for table rows  
            var FColor2 = new BaseColor(51, 51, 51);


            var Heading3 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 9, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/
            //var contentfontCheck = FontFactory.GetFont((Server.MapPath("~/fonts/WINGDING.TTF")), BaseFont.CP1252, true, 4);
            var Heading5 = FontFactory.GetFont((Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor2);  /*tarun 20/09/2018*/

            Phrase phrase;
            PdfPCell cell;


            PdfPTable table1 = new PdfPTable(8);
            PdfPTable table2 = new PdfPTable(5);

            table1.SetWidths(new int[] { 5, 4, 5, 5, 6, 10, 5, 5 });
            table2.SetWidths(new int[] { 5, 5, 15, 7, 5 });

            if (PurOptionData.Count > 0)
            {
                var CurrentColor = BaseColor.WHITE;

                for (i = 0; i < PurOptionData.Count; i++)
                {

                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 8;
                    cell.PaddingBottom = -6;
                    cell.Border = 0;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);


                    phrase = new Phrase();
                    phrase.Add(new Chunk(PurOptionData[i].PurchaseDetailId.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.PaddingLeft = 4f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);


                    phrase = new Phrase();
                    phrase.Add(new Chunk(PurOptionData[i].Quantity.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.PaddingRight = 4f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(PurOptionData[i].brand.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.PaddingLeft = 4f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(PurOptionData[i].code.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.PaddingLeft = 4f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(PurOptionData[i].Item.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.PaddingLeft = 4f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk(PurOptionData[i].colour.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 1;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.PaddingLeft = 4f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("$" + PurOptionData[i].UnitExGST.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.PaddingRight = 4f;
                    cell.Border = PdfPCell.RIGHT_BORDER;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("$" + PurOptionData[i].ExtExGST.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.PaddingRight = 4f;
                    cell.Border = 0;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);

                    // row space
                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 8;
                    cell.FixedHeight = 10f;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    cell.BackgroundColor = RColor;
                    table1.AddCell(cell);

                    // row space
                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 8;
                    cell.FixedHeight = 10f;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table1.AddCell(cell);


                    phrase = new Phrase();
                    phrase.Add(new Chunk(PurOptionData[i].size.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 5;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.Border = 0;
                    cell.PaddingBottom = 0f;
                    cell.PaddingLeft = 4f;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table1.AddCell(cell);


                    phrase = new Phrase();
                    phrase.Add(new Chunk(PurOptionData[i].Notes.ToString(), Heading3));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 3;
                    cell.FixedHeight = 10f;
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 0f;
                    cell.Border = 0;
                    cell.PaddingLeft = 4f;
                    cell.Border = PdfPCell.LEFT_BORDER;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table1.AddCell(cell);


                    // row space
                    phrase = new Phrase();
                    phrase.Add(new Chunk(" ", Heading5));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 8;
                    cell.FixedHeight = 10f;
                    cell.PaddingBottom = -6f;
                    cell.Border = 0;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table1.AddCell(cell);


                    rcount++;
                    if (rcount > 6 && i != PurOptionData.Count - 1)
                    {
                        doc.Add(table1);
                        table1.DeleteBodyRows();
                        doc.NewPage();
                        rcount = 0;
                    }
                }
            }

            for (j = 1; j < (k - rcount); j++)
            {
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 8;
                cell.FixedHeight = 30f;
                cell.PaddingBottom = -12f;
                cell.Border = 0;
                cell.BackgroundColor = RColor;
                table1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 8;
                cell.FixedHeight = 30f;
                cell.PaddingBottom = -12f;
                cell.Border = 0;
                cell.BackgroundColor = BaseColor.WHITE;
                table1.AddCell(cell);
            }
            doc.Add(table1);

            ////TABLE 2
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 5;
            cell.Rowspan = 2;
            cell.FixedHeight = 20f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);



            //nextline
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 5;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("DeliveryAddress:", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);


            phrase = new Phrase();
            phrase.Add(new Chunk("24 Hour Merchandise", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);



            phrase = new Phrase();
            phrase.Add(new Chunk("Purchase Order Total:", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("$" + PurchaseData.ExtExGST.ToString(), Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);



            //nextline
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 5;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);


            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);



            phrase = new Phrase();
            phrase.Add(new Chunk("145 Renwick St", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);


            var GSTCal = (PurchaseData.ExtExGST) * 10 / 100;

            phrase = new Phrase();
            phrase.Add(new Chunk("GST:", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("$" + GSTCal.ToString(), Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);



            //nextline
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 5;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);


            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);


            phrase = new Phrase();
            phrase.Add(new Chunk("Marrickville", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 2;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);


            phrase = new Phrase();
            phrase.Add(new Chunk("Bill Total:", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);

            phrase = new Phrase();
            phrase.Add(new Chunk("$" + PurchaseData.ExtInclGST.ToString(), Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);



            //nextline
            phrase = new Phrase();
            phrase.Add(new Chunk(" ", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 5;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);


            phrase = new Phrase();
            phrase.Add(new Chunk("", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);



            phrase = new Phrase();
            phrase.Add(new Chunk("NSW 2204", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 1;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);



            phrase = new Phrase();
            phrase.Add(new Chunk("", Heading5));
            cell = new PdfPCell(phrase);
            cell.Colspan = 3;
            cell.FixedHeight = 10f;
            cell.PaddingBottom = -6f;
            cell.Border = 0;
            table2.AddCell(cell);


            doc.Add(table2);
            doc.Close();
            doc.Dispose();
            if (PdfType == "Print")
            {

                return View();
            }
            else
            {
                return null;
            }
        }

        public class HeaderFooterStatementforOrder : PdfPageEventHelper
        {
            public int id { get; set; }

            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                KENNEWEntities dbContext = new KENNEWEntities();

                var PurchaseData = dbContext.Pro_PurchaseData(id).FirstOrDefault();

                var FColor = new BaseColor(35, 168, 225);             //font color declaration for highlighted data
                var RColor = new BaseColor(230, 230, 230);            //font color declaration for table rows background 
                var FColor2 = new BaseColor(38, 38, 38);

                var Heading4 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/compactaboldbt.ttf")), BaseFont.CP1252, true, 27, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading3 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 9, iTextSharp.text.Font.NORMAL, FColor2);
                var Heading5 = FontFactory.GetFont((System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf")), BaseFont.CP1252, true, 10, iTextSharp.text.Font.NORMAL, FColor2);


                Phrase phrase;
                PdfPCell cell;


                PdfPTable headerTable1 = new PdfPTable(5);

                PdfPTable headerTable2 = new PdfPTable(8);

                headerTable1.SetWidths(new int[] { 7, 14, 8, 7, 8 });
                headerTable1.SpacingAfter = 2f;

                headerTable2.SetWidths(new int[] { 5, 4, 5, 5, 6, 10, 5, 5 });
                headerTable1.SpacingAfter = 2f;

                //Image for heading
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Images/Header.jpg"));
                cell = new PdfPCell(iTextSharp.text.Image.GetInstance(image), true);
                cell.FixedHeight = 70f;
                cell.Colspan = 5;
                cell.Border = 0;
                cell.PaddingTop = -15;
                cell.PaddingRight = -32;
                cell.PaddingLeft = -32;
                headerTable1.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk(" ", Heading3));
                //cell = new PdfPCell(phrase);
                //cell.Colspan = 5;
                //cell.PaddingBottom = 4f;
                //cell.Border = 0;
                //headerTable1.AddCell(cell);


                //Purchase Order heading
                phrase = new Phrase();
                phrase.Add(new Chunk("PURCHASE ORDER", Heading4));
                cell = new PdfPCell(phrase);
                cell.Colspan = 5;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerTable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 5;
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headerTable1.AddCell(cell);

                //first row of first table
                phrase = new Phrase();
                phrase.Add(new Chunk("Organisation:", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                headerTable1.AddCell(cell);


                phrase = new Phrase();
                phrase.Add(new Chunk(PurchaseData.Organisation.ToString(), Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                headerTable1.AddCell(cell);


                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headerTable1.AddCell(cell);



                phrase = new Phrase();
                phrase.Add(new Chunk("Order Date:", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                headerTable1.AddCell(cell);

                var dateonly = string.Format("{0:dd/MM/yyyy}", PurchaseData.PurchaseDate);

                phrase = new Phrase();
                phrase.Add(new Chunk(dateonly, Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                headerTable1.AddCell(cell);

                //nextline
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 5;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                headerTable1.AddCell(cell);


                //2nd row of 1st table
                phrase = new Phrase();
                phrase.Add(new Chunk("Job Name:", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                headerTable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(PurchaseData.OppName.ToString(), Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                headerTable1.AddCell(cell);


                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.PaddingBottom = 2f;
                cell.Border = 0;
                headerTable1.AddCell(cell);



                phrase = new Phrase();
                phrase.Add(new Chunk("Purchase Ord No.:", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                headerTable1.AddCell(cell);

                var PurOrderNo = PurchaseData.PurchaseId;
                var PurOrderDigit = PurOrderNo.ToString("000000");

                phrase = new Phrase();
                phrase.Add(new Chunk(PurOrderDigit, Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                headerTable1.AddCell(cell);


                //nextline
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 5;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                headerTable1.AddCell(cell);


                //3rd row of 1st table
                phrase = new Phrase();
                phrase.Add(new Chunk("JobHUB No:", Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                headerTable1.AddCell(cell);


                phrase = new Phrase();
                phrase.Add(new Chunk(PurchaseData.OpportunityId.ToString(), Heading5));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Border = 0;
                headerTable1.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.FixedHeight = 10f;
                cell.PaddingBottom = -6f;
                cell.Colspan = 3;
                cell.Border = 0;
                headerTable1.AddCell(cell);


                //nextline
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading5));
                cell = new PdfPCell(phrase);
                cell.PaddingBottom = 4f;
                cell.Colspan = 5;
                cell.Border = 0;
                headerTable1.AddCell(cell);


                // table heading
                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 8;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = -6f;
                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headerTable2.AddCell(cell);



                phrase = new Phrase();
                phrase.Add(new Chunk("Option No ", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingBottom = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingTop = 0f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;

                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Qty", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 0f;
                cell.Border = 0;
                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Brand", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Code", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Item", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Colour", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Unit Ex GST", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER;
                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Ext Ex GST", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 1;
                cell.PaddingTop = 0f;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = 0f;
                cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 8;
                cell.PaddingLeft = 4f;
                cell.PaddingBottom = -6f;
                cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                headerTable2.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(" ", Heading3));
                cell = new PdfPCell(phrase);
                cell.Colspan = 8;
                cell.PaddingLeft = 4f;
                cell.FixedHeight = 1f;
                cell.Border = 0;
                headerTable2.AddCell(cell);

                doc.Add(headerTable1);
                doc.Add(headerTable2);

            }

            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                //int counter = 1;

                int TotalPageCount = 0;
                PdfPCell cell;
                PdfPTable Footertable = new PdfPTable(1);
                Footertable.TotalWidth = 560;
                Footertable.HorizontalAlignment = Element.ALIGN_CENTER;

                KENNEWEntities dbContext = new KENNEWEntities();

                var PurOptionData = dbContext.Pro_PurchaseOptionData(id).ToList();


                var FColor2 = new BaseColor(89, 89, 89);   /*tarun 20/09/2018*/

                var FColor3 = new BaseColor(26, 26, 26); /*tarun 20/09/2018*/

                BaseFont bf = BaseFont.CreateFont(System.Web.HttpContext.Current.Server.MapPath("~/fonts/roboto-condensed.regular.ttf"), BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                iTextSharp.text.Font footerfont2 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont3 = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL, FColor2);
                iTextSharp.text.Font footerfont4 = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, FColor3);

                if (PurOptionData.Count <= 7)
                {
                    TotalPageCount = 1;
                }
                else if (PurOptionData.Count >= 7 && PurOptionData.Count <= 14)
                {
                    TotalPageCount = 2;
                }
                else if (PurOptionData.Count >= 14 && PurOptionData.Count <= 21)
                {
                    TotalPageCount = 3;
                }

                else if (PurOptionData.Count >= 21 && PurOptionData.Count <= 28)
                {
                    TotalPageCount = 3;
                }


                int pageno = writer.PageNumber;
                string text = "Page " + pageno.ToString() + " of " + TotalPageCount.ToString();

                cell = new PdfPCell(new Paragraph("TeeCorp Pty Ltd t/a 24 Hour Merchandise", footerfont2));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                Footertable.AddCell(cell);
                Footertable.WriteSelectedRows(0, -1, 0, 40, writer.DirectContent);


                cell = new PdfPCell(new Paragraph("145 Renwick St, Marrickville NSW 2204 Australia.    PO Box 7295 Alexandria NSW 2015    ABN 60 130 686 234", footerfont3));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                Footertable.AddCell(cell);
                Footertable.WriteSelectedRows(0, -1, 0, 40, writer.DirectContent);


                cell = new PdfPCell(new Paragraph(text, footerfont3));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.PaddingTop = -11f;
                cell.PaddingRight = 0f;
                Footertable.AddCell(cell);
                Footertable.WriteSelectedRows(0, -1, 0, 40, writer.DirectContent);

            }
        }
        public List<AccountManagerDropdownViewModel> getProfileList()
        {
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(DbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.firstname)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        // baans end 15th November
        public ActionResult GetPurchaseEmailContent(int OpportunityId, int PurchaseId)
        {
            EmailContentViewModel data = new EmailContentViewModel();
            data = _purchasebaseService.GetPurchaseEmailContent();

            var OrgId = DbContext.tblPurchases.Where(_ => _.PurchaseId == PurchaseId).Select(_ => _.OrgId).FirstOrDefault();
            var ToEmail = DbContext.tblOrganisations.Where(_ => _.OrgId == OrgId).Select(_ => _.EmailAddress).FirstOrDefault();
            data.ClientEmailID = ToEmail;

            if (data.Subject != null && data.Subject != "")
            {
                var CurrentOpportunityName = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OpportunityId).Select(_ => _.OppName).FirstOrDefault();
                data.Subject = data.Subject + " - " + CurrentOpportunityName + " - " + OpportunityId;
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async System.Threading.Tasks.Task<ActionResult> SendPurchaseEmail(EmailViewModel model, int OpportunityId,int PurchaseId)
        {
            //generate pdf in upload folder.
            string PathPdf = "~/Content/uploads/PurchaseOrder/PurchaseOrder_" + OpportunityId + "-" + PurchaseId + "-" + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).ToString("dd-MM-yyyy") + ".pdf";
            PathPdf = Server.MapPath(PathPdf);
            PerformaInvoicePdf(PurchaseId, model.Type, PathPdf);

            //send email
            string AcctEmail = "", AccountManageName = "";

            var EmailContent = DbContext.tblEmailContents.Where(_ => _.Purpose == "PurchaseOrder").FirstOrDefault();

            var OrgId = DbContext.tblPurchases.Where(_ => _.PurchaseId == PurchaseId).Select(_ => _.OrgId).FirstOrDefault();
            var OrgName = DbContext.tblOrganisations.Where(_ => _.OrgId == OrgId).Select(_ => _.OrgName).FirstOrDefault();

            var Senderperson = DbContext.tblusers.Where(_ => _.title == "Production Director" && _.firstname == "Jon").FirstOrDefault();
            AcctEmail = DataBaseCon.PurchaseFromMail;
            AccountManageName = Senderperson.firstname + " " + Senderperson.lastname;

            string str = "";
            str = @"<br>Hi " + OrgName +  ",<br><br>" + model.MailMessage2 + "<br><br>Regards,<br><br>" + AccountManageName + "<br><br><b> Operations Director </b><br> 24 Hour Merchandise <br>"+ EmailContent.Body3;

            string To = model.Email;
            string Subject = model.Subject;

            var cc = AcctEmail;

            var message = new PostmarkMessage()
            {
                To = To,
                From = DataBaseCon.FromEmailName + " <" + AcctEmail + ">",
                //Bcc = DataBaseCon.BCCMailID == "" ? null : DataBaseCon.BCCMailID,
                Cc = cc,//AcctEmail == "" ? null : AcctEmail,
                TrackOpens = true,
                Subject = Subject,
                HtmlBody = str,
                Tag = "business-message",
            };

            var PdfFileName = PathPdf.Split('\\');
            var PdfName = PdfFileName.Length - 1;

            var pdfContent = System.IO.File.ReadAllBytes(PathPdf);

            message.AddAttachment(pdfContent, PdfFileName[PdfName], "application/pdf", "cid:MyPdf");

            var client = new PostmarkClient(PostmarkToken);
            try
            {
                var sendResult = await client.SendMessageAsync(message);
            }
            catch (Exception Ex)
            {

            }

            if (System.IO.File.Exists(PathPdf))
            {
                System.IO.File.Delete(PathPdf);
            }

            response.Result = KEN.Models.ResponseType.Success;
            response.Message = "Mail Sent successfully";

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SubmitPurchaseBillToQuickBooks(int OpportunityId, int PurchaseId)
        {
            //var PurchaseJob = Mapper.Map<tblPurchase>(_purchasebaseService.GetPurchaseByOpportunityId(OpportunityId));

            var PurchaseJob = Mapper.Map<tblPurchase>(_purchasebaseService.GetPurchById(PurchaseId));

            var OpportunityName = DbContext.tblOpportunities.Where(_ => _.OpportunityId == PurchaseJob.OpportunityId).Select(_ => _.OppName).FirstOrDefault();

            var PurchaseItems = _purchasebaseService.GetPurchaseDetailOptionGrid(PurchaseJob.PurchaseId);

            var OrgId = Convert.ToInt32(PurchaseJob.OrgId);
            var SupplierOrg = _OrgbaseService.GetOrganisationById(OrgId);

            try
            {
                string QuickBooks_accessToken = "", QuickBooks_accessTokenSecret = "", QuickBooks_realmId = "";
                var AuthData = _baseService.AuthdataListByDesc();
                foreach (var item in AuthData)
                {
                    if (item.FieldName == "QuickBooks_Access_Token")
                    {
                        QuickBooks_accessToken = item.FieldValue;
                    }
                    if (item.FieldName == "QuickBooks_Access_TokenSecret")
                    {
                        QuickBooks_accessTokenSecret = item.FieldValue;
                    }
                    if (item.FieldName == "QuickBooks_RealmID")
                    {
                        QuickBooks_realmId = item.FieldValue;
                    }

                }

                OAuthRequestValidator oauthValidator = new OAuthRequestValidator(QuickBooks_accessToken, QuickBooks_accessTokenSecret, consumerKey, consumerSecret);
                ServiceContext serviceContext = new ServiceContext(QuickBooks_realmId, IntuitServicesType.QBO, oauthValidator);
                serviceContext.IppConfiguration.BaseUrl.Qbo = ServiceContextUrl;
                serviceContext.IppConfiguration.Message.Request.SerializationFormat = Intuit.Ipp.Core.Configuration.SerializationFormat.Xml;
                serviceContext.IppConfiguration.Message.Response.SerializationFormat = Intuit.Ipp.Core.Configuration.SerializationFormat.Xml;
                serviceContext.IppConfiguration.MinorVersion.Qbo = "11";
                DataService commonServiceQBO = new DataService(serviceContext);

                //Generate New Supplier Supplier
                Vendor QuickBooksSupplier = new Vendor();
                if (SupplierOrg != null)
                {
                    var QbSupplierId = "0";
                    if (SupplierOrg.IntuitID != null)
                    {
                        QbSupplierId = Convert.ToString(SupplierOrg.IntuitID);
                    }
                    QueryService<Vendor> vendorQueryService = new QueryService<Vendor>(serviceContext);
                    QuickBooksSupplier = vendorQueryService.ExecuteIdsQuery("Select * From Vendor where Id='" + QbSupplierId + "'").FirstOrDefault();
                    if (QuickBooksSupplier == null)
                    {
                        Vendor vendor = new Vendor();

                        EmailAddress SupplierEmailAddress = new EmailAddress();
                        SupplierEmailAddress.Address = SupplierOrg.EmailAddress;
                        vendor.PrimaryEmailAddr = SupplierEmailAddress;

                        vendor.GivenName = SupplierOrg.OrgName;
                        vendor.DisplayName = SupplierOrg.OrgName;

                        WebSiteAddress webaddress = new WebSiteAddress();
                        webaddress.URI = SupplierOrg.WebAddress;
                        vendor.WebAddr = webaddress;

                        TelephoneNumber Suppliercontact = new TelephoneNumber();
                        Suppliercontact.FreeFormNumber = SupplierOrg.MainPhone;
                        vendor.PrimaryPhone = Suppliercontact;

                        var SupplierAddress = _OrgbaseService.GetOrganisationAddress(SupplierOrg.OrgId);
                        PhysicalAddress SupplierAdd = new PhysicalAddress();
                        if (SupplierAddress != null)
                        {
                            SupplierAdd.Line1 = SupplierAddress.Address1;
                            SupplierAdd.Line2 = SupplierAddress.Address2;
                            SupplierAdd.PostalCode = SupplierAddress.Postcode;
                            SupplierAdd.CountrySubDivisionCode = SupplierAddress.State;

                            vendor.BillAddr = SupplierAdd;
                        }

                        QuickBooksSupplier = commonServiceQBO.Add(vendor);


                        _OrgbaseService.AddIntuitID(OrgId, QuickBooksSupplier.Id);
                    }
                }
                //Supplier Ends Here

                //QueryService<Item> itemQueryService = new QueryService<Item>(serviceContext);
                //var QuickBooksItems = itemQueryService.ExecuteIdsQuery("Select * from Item").ToList();

                QueryService<Account> accountQueryService = new QueryService<Account>(serviceContext);
                Account account;

                //Generate Bill for Purchase
                var CustomOpportunityName = OpportunityName;
                if (CustomOpportunityName.Length > 10)
                {
                    CustomOpportunityName = CustomOpportunityName.Substring(0, 10);
                }
                var CustomBillNumber = Convert.ToString(PurchaseJob.BillNo + "-" + CustomOpportunityName);

                QueryService<Bill> billQueryService = new QueryService<Bill>(serviceContext);
                var QuickBooksBill = billQueryService.ExecuteIdsQuery("Select * from Bill where DocNumber = '" + CustomBillNumber + "'").ToList();

                Bill FinalBill = new Bill();
                if (QuickBooksBill.Count == 0 && PurchaseItems.Count > 0)
                {
                    QueryService<TaxCode> TaxCodeQueryService = new QueryService<TaxCode>(serviceContext);
                    var TaxCode = TaxCodeQueryService.ExecuteIdsQuery("Select * From TaxCode where Name='GST on purchases'").FirstOrDefault();
                    //var DifferentTax = TaxCodeQueryService.ExecuteIdsQuery("Select * from TaxCode").ToList();

                    Bill Bill = new Bill();
                    TxnTaxDetail Tax = new TxnTaxDetail();

                    Line[] BillLine = new Line[PurchaseItems.Count + 1];
                    int Counter = 0;

                    //Adding PurchaseItems
                    foreach (var item in PurchaseItems)
                    {
                        Line billitems = new Line();

                        billitems.Amount = Convert.ToDecimal(item.ExtExGST);
                        billitems.AmountSpecified = true;

                        billitems.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                        billitems.DetailTypeSpecified = true;
                        AccountBasedExpenseLineDetail AcctExpense = new AccountBasedExpenseLineDetail();
                        AcctExpense.TaxCodeRef = new ReferenceType()
                        {
                            Value = TaxCode.Id
                        };

                        account = accountQueryService.ExecuteIdsQuery("Select * From Account where Name='" + item.ItemName + "' StartPosition 1 MaxResults 1").FirstOrDefault();
                        if (account == null)
                        {
                            account = accountQueryService.ExecuteIdsQuery("Select * From Account where Name='Unmatched' StartPosition 1 MaxResults 1").FirstOrDefault();
                        }
                        AcctExpense.AccountRef = new ReferenceType()
                        {
                            name = account.Name,
                            Value = account.Id
                        };

                        billitems.Description = OpportunityName + ", " + item.quantity + ", " + item.BrandName + ", " + item.code + ", " + item.colour + ", " + item.InitialSizes;
                        billitems.AnyIntuitObject = AcctExpense;

                        BillLine[Counter] = billitems;
                        Counter++;
                    }

                    //Adding ShippingInfo
                    if (PurchaseJob.ShippingCharge != null && PurchaseJob.ShippingCharge > 0)
                    {
                        Line ShippingCharges = new Line();

                        ShippingCharges.Amount = Convert.ToDecimal(PurchaseJob.ShippingCharge);
                        ShippingCharges.AmountSpecified = true;

                        ShippingCharges.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                        ShippingCharges.DetailTypeSpecified = true;
                        AccountBasedExpenseLineDetail ShipAcct = new AccountBasedExpenseLineDetail();
                        ShipAcct.TaxCodeRef = new ReferenceType()
                        {
                            Value = TaxCode.Id
                        };

                        account = accountQueryService.ExecuteIdsQuery("Select * From Account where Name='Shipping on Purchases' StartPosition 1 MaxResults 1").FirstOrDefault();
                        ShipAcct.AccountRef = new ReferenceType()
                        {
                            name = account.Name,
                            Value = account.Id
                        };

                        ShippingCharges.AnyIntuitObject = ShipAcct;

                        BillLine[Counter] = ShippingCharges;
                    }


                    Bill.Line = BillLine;
                    Bill.VendorRef = new ReferenceType()
                    {
                        Value = QuickBooksSupplier.Id
                    };

                    Bill.DocNumber = CustomBillNumber;
                    //Bill.Memo = OpportunityName;

                    var CustomPurchaseID = PurchaseJob.OpportunityId + "_" + PurchaseJob.PurchaseId;
                    Bill.PrivateNote = Convert.ToString(CustomPurchaseID);

                    EmailAddress email = new EmailAddress();
                    email.Address = SupplierOrg.EmailAddress;
                    Bill.BillEmail = email;

                    FinalBill = commonServiceQBO.Add(Bill);

                    var QBBillId = FinalBill.Id;
                }

                response.Result = KEN.Models.ResponseType.Success;
                response.tblName = "Data";   
            }
            catch(Exception Ex)
            {
                response.Message = Ex.Message;
                response.Result = KEN.Models.ResponseType.Error;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckOrgAddress(string OrganisationId)
        {
            var result = true;

            var OrgId = Convert.ToInt32(OrganisationId);
            var data = DbContext.tblAddresses.Where(_ => _.OrgId == OrgId).FirstOrDefault();
            if(data == null)
            {
                result = false;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetPurchaseStatus(int PurchaseId)
        {
            response = _purchasebaseService.SetPurchaseStatus(PurchaseId);

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}