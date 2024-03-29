﻿using System;
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
using System.Configuration;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.LinqExtender;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using System.Collections;
using System.Net;



namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class OpportunityController : Controller
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
        public string oauth_callback_url = ConfigurationManager.AppSettings["oauth_callback_url"];
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();
        KENNEWEntities DbContext = new KENNEWEntities();
        private readonly IOpportunityService _baseService;
        private readonly IOrganisationService _OrgbaseService;
        ResponseViewModel response = new ResponseViewModel();
        public OpportunityController(IOpportunityService baseService, IOrganisationService OrgbaseService)
        {
            _baseService = baseService;
            _OrgbaseService = OrgbaseService;
        }
        KENNEWEntities dbContext = new KENNEWEntities();
        // GET: Dashboard
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
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            //baans 20Aug2020
            ViewBag.ActiveUserRole = GetActiveUserData();
            return View();
        }

        public ActionResult QuoteDetails(int Id)
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
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            //baans 20Aug2020
            return View();
        }
        public ActionResult OrderDetails(int Id)
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
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            //baans 20Aug2020
            return View();
        }
        public ActionResult JobDetails(int Id)
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
            ViewBag.StateList = GetAllStateList();
            ViewBag.PaymentMethodList = GetPaymentMethodList();
            ViewBag.ID = Id;
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            //baans 20Aug2020
            ViewBag.ActiveUserRole = GetActiveUserData();
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

        public ActionResult GetOppById(int OppId)
        {
            var model = new opportunityViewModel();
            if (OppId > 0)
            {
                model = Mapper.Map<opportunityViewModel>(_baseService.GetOppById(OppId));

            }



            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OpportunityList()
        {
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            ViewBag.ActiveUserRole = GetActiveUserData();
            return View();
        }
        // Baans change 19th Sept
        public ActionResult GetBrandId(string BrandName)
        {
            var AddId = DbContext.tblbands.Where(_ => _.name == BrandName).FirstOrDefault();
            return Json(AddId, JsonRequestBehavior.AllowGet);
        }
        // baans end 19th Sept
        public ActionResult GetOpportunityDetail(string oppt, string startDate, string EndDate, int UserProfile)
        {
            var opptdata = _baseService.GetOpportunityData(oppt, startDate, EndDate, UserProfile);
            var Jsonresult = Json(opptdata, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = Int32.MaxValue;
            return Jsonresult;
        }
        public IEnumerable<tbldepartment> GetDepartments()
        {
            // baans change 19th Sept for active values
            // var getData = dbContext.tbldepartments.ToList().OrderBy(_=>_.department);
            var getData = dbContext.tbldepartments.Where(_ => _.Status == "Active").ToList().OrderBy(_ => _.department);
            return getData;
            // baans end 19th Sept
        }
        public IEnumerable<tblitem> GetItemList()
        {
            // baans change 19th Sept for active values
            //var getData = dbContext.tblitems.ToList().OrderBy(_=>_.name);
            var getData = dbContext.tblitems.Where(_ => _.Status == "Active").ToList().OrderBy(_ => _.name);
            return getData;
            // baans end 19th Sept
        }
        public IEnumerable<tblband> GetBrandList()
        {
            // baans change 15th September for Status on Master page
            //var getData = dbContext.tblbands.ToList().OrderBy(_=>_.name);
            var getData = dbContext.tblbands.Where(_ => _.Status == "Active").ToList().OrderBy(_ => _.name);
            // baans end 15th September
            return getData;
        }
        public IEnumerable<tblCampaign> GetCampaign()
        {
            var getData = dbContext.tblCampaigns.ToList().OrderBy(_ => _.Campaign);
            return getData;
        }

        //    public List<EnumViewModel> GetContactRoles()
        //{
        //    var getData = (from ContactRole e in Enum.GetValues(typeof(ContactRole))
        //                   select new { Name = e.ToString() }).ToList();
        //    var newdata = getData.Select(item => new EnumViewModel
        //    {
        //        Name = item.Name,
        //    }
        //    ).OrderBy(_=>_.Name).ToList();
        //    return newdata;
        //}
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
        // baans change 15th November
        public List<AccountManagerDropdownViewModel> getProfileList()
        {
            //var getData = dbContext.tblusers.Where(_ => _.UserRole == "Account Manager").ToList();
            //var newData = getData.Select(item => new EnumViewModel
            //{
            //    Name = item.firstname + " " + item.lastname,
            //}).OrderBy(_ => _.Name).ToList();
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.firstname)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;


        }
        // baans end 15th November

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
        public List<EnumViewModel> GetShipping()
        {
            var getData = (from Shipping e in Enum.GetValues(typeof(Shipping))
                           select new { Name = e.ToString() }).ToList();
            var newdata = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }
            ).OrderBy(_ => _.Name).ToList();
            return newdata;
        }
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
        public List<AccountManagerDropdownViewModel> GetAccountManagers()
        {

            // string[] Roles = new string[] { "Administrator", "Account Manager","Production Director"};
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.title)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        public ActionResult GetDeletedManager()
        {
            // string[] Titles = new string[] { "Administrator", "Account Manager", "Production Director" };
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "deleted")).ToList();
            return Json(getData, JsonRequestBehavior.AllowGet);
        }

        // baans change 13th September
        public ActionResult SaveNewBrand(string OptionBrand)
        {
            tblband Entity = new tblband();

            response = _baseService.SaveNewBrand(Entity, OptionBrand);
            var getData = dbContext.tblbands.ToList().OrderBy(_ => _.name);
            //return getData;
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // baans end 13th September
        public ActionResult updateOpportunity(opportunityViewModel model)
        {
            if (model != null)
            {

                if (model.Stage != null && model.Stage != "")
                {
                    switch (model.Stage)
                    {
                        case "Opportunity":
                            {
                                model.OppNotes = model.Notes;
                                break;
                            }
                        case "Quote":
                            {
                                model.QuoteNotes = model.Notes;
                                break;
                            }
                        case "Order":
                            {
                                model.OrderNotes = model.Notes;
                                break;
                            }
                        case "Job":
                            {
                                model.JobNotes = model.Notes;
                                break;
                            }
                        case "Packing":
                            {
                                model.PackingNotes = model.Notes;
                                break;
                            }
                        case "Invoicing":
                            {
                                model.InvoicingNotes = model.Notes;
                                break;

                            }
                        case "Shipping":
                            {
                                model.ShippingNotes = model.Notes;
                                break;
                            }
                        case "Complete":
                            {
                                model.CompleteNotes = model.Notes;
                                break;
                            }
                    }
                }
                var Entity = Mapper.Map<tblOpportunity>(model);
                if (model.OpportunityId > 0)
                {
                    response = _baseService.OppBatchTransaction(Entity, model.PageSource, BatchOperation.Update);
                }
                else
                {
                    response = _baseService.OppBatchTransaction(Entity, model.PageSource, BatchOperation.Insert);
                }

            }
            //return Json(new {response = response, IsConfValid= IsConfValid }, JsonRequestBehavior.AllowGet);
            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetOptionGrid(int OpportunityID, string Status)
        {
            var OptionData = _baseService.GetOptionGrid(OpportunityID, Status);
            var OppData = _baseService.GetOppById(OpportunityID);
            double Total = 0;
            double PaymentTotal = 0;
            double TotalGarmentCost = 0;
            double TotalDecorationCost = 0;
            double TotalGp = 0;
            double TotalWithShipping = 0;
            // baans change 18th August for GstValue
            decimal GSTValue = 0;
            double DecorationCost = 0;      /*10 Sep 2018 (N)*/
            double TotalOtherCost = 0;      //20 Nov 2018 (N)
            // baans end 18th August
            foreach (var item in OptionData)
            {
                /*10 Sep 2018 (N)*/
                item.Front_decCost = item.Front_decCost == null ? 0 : item.Front_decCost;
                item.Back_decCost = item.Back_decCost == null ? 0 : item.Back_decCost;
                item.Right_decCost = item.Right_decCost == null ? 0 : item.Right_decCost;
                item.Left_decCost = item.Left_decCost == null ? 0 : item.Left_decCost;
                item.Extra_decCost = item.Extra_decCost == null ? 0 : item.Extra_decCost;
                /*10 Sep 2018 (N)*/

                item.OtherCost = item.OtherCost == null ? 0 : item.OtherCost;       //20 Nov 2018 (N)

                item.include = item.include_job == true ? "Yes" : "No";
                if (item.ExtExGST != null && item.include_job == true)
                {
                    PaymentTotal += Convert.ToDouble(item.ExtInclGST);
                    Total += Convert.ToDouble(item.ExtExGST);
                    TotalGarmentCost += Convert.ToDouble(item.quantity) * Convert.ToDouble(item.Cost);
                    /*11 Sep 2018 (N)*/
                    /*                    DecorationCost += Convert.ToDouble(item.Front_decCost + item.Back_decCost + item.Left_decCost + item.Right_decCost + item.Extra_decCost);*/
                    DecorationCost = Convert.ToDouble(item.Front_decCost + item.Back_decCost + item.Left_decCost + item.Right_decCost + item.Extra_decCost);
                    /*10 Sep 2018 (N)*/
                    TotalDecorationCost += Convert.ToDouble(item.quantity) * DecorationCost;        /*10 Sep 2018 (N)*/

                    TotalOtherCost += Convert.ToDouble(item.quantity) * Convert.ToDouble(item.OtherCost);       //20 Nov 2018 (N)
                }


            }

            if (OppData != null)
            {

                if (OppData.Price != null && OppData.Price != "")
                {
                    TotalWithShipping = (Total + Convert.ToDouble(OppData.Price)) * Convert.ToDouble(OppData.GSTValue);
                }
                else
                {
                    TotalWithShipping = PaymentTotal;
                }
                GSTValue = Convert.ToDecimal(OppData.GSTValue);
            }
            double CalcValue = ((Total - TotalGarmentCost - TotalDecorationCost) * 100);
            if (CalcValue > 0)
                TotalGp = ((Total - TotalGarmentCost - TotalDecorationCost - TotalOtherCost) * 100) / Total;        //20 Nov 2018 (N)(Subtarct OtherCost)
            else
                TotalGp = 0;

            double TotalPaid = _baseService.TotalPaidBalance(OpportunityID);
            return Json(new { data = OptionData, Total = Math.Round(Total, 2), PaymentTotal = Math.Round(PaymentTotal, 2), TotalWithShipping = Math.Round(TotalWithShipping, 2), GSTValue = Math.Round(GSTValue, 2), TotalGp = Math.Round(TotalGp, 2), TotalPaid = Math.Round(TotalPaid, 2), TotalDue = Math.Round(TotalWithShipping,2) - Math.Round(TotalPaid, 2) }, JsonRequestBehavior.AllowGet);
        }

        // baans change 20th Sept

        public ActionResult GetItemStatus(int ItemId)
        {
            var ItemStatus = dbContext.tblitems.Where(_ => _.id == ItemId).Select(_ => _.Status).FirstOrDefault();

            return Json(ItemStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartmentStatus(int DepId)
        {
            var DepStatus = dbContext.tbldepartments.Where(_ => _.id == DepId).Select(_ => _.Status).FirstOrDefault();
            return Json(DepStatus, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getInactiveDept(int DepId)
        {
            var Item = dbContext.tbldepartments.Where(_ => _.id == DepId).FirstOrDefault();
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        // baans end 20th Sept
        //Action result for ajax call
        [HttpPost]
        public ActionResult GetCityByStaeId(string SizeType, int OppId)
        {
            // baans change 31st October for order wise tbc
            //List<Size> objcity = new List<Size>();
            // baans change 7th July for shirt sorting

            //if (SizeType == "Mens")
            //{

            //    objcity = GetAllSizes().Where(m => m.sizeType == SizeType).ToList();

            //}
            //else
            //{
            //    objcity = GetAllSizes().Where(m => m.sizeType == SizeType).OrderBy(_ => _.size).ToList();
            //}
            List<tblOptionSize> objSize = new List<tblOptionSize>();

            var data = DbContext.tblOptionSizes.Where(_ => _.sizeType == SizeType).OrderBy(_ => _.SortOrder).ToList();
            var TbcValid = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
            if (TbcValid != null)
            {
                // the following lines have been commented so that the code runs correctly 

                //if (TbcValid.Stage == "Opportunity" || TbcValid.Stage == "Quote" || TbcValid.Stage == "Order")
                //{
                data.Add(new tblOptionSize { SortOrder = 0, size = "TBC" });
                //}
            }
            // baans end 31st October
            SelectList obgcity = new SelectList(data, "size", "size", 0);
            return Json(obgcity);
        }


        // Collection for state
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


        //collection for all Sizes

        public List<DecorationViewModel> GetDecorationList()
        {

            var DecorationList = _baseService.GetDecorationList();
            return DecorationList;

        }
        public List<DecorationCostViewModel> GetDecorationCost(string DecorationDesc)
        {
            var DecorationList = _baseService.GetDecorationCost(DecorationDesc);
            return DecorationList;
        }
        public ActionResult GetDecorationCostByQty(string prefix, string Decoration)
        {
            var data = _baseService.GetDecorationCostByQty(prefix, Decoration).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDecorationByDesc(string prefix)
        {
            var data = _baseService.GetDecorationByDesc(prefix).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateOption(OptionViewModel model, OptionCodeBrandItemViewModel OptionCodeModel)
        {

            //P 10 Jan OptionCode
            if (OptionCodeModel != null && OptionCodeModel.cost != null && OptionCodeModel.Link != null)
            {
                var data = dbContext.tblOptionCodes.Where(x => x.Code == OptionCodeModel.Code).FirstOrDefault();
                if (OptionCodeModel.id == 0 && data == null)
                {
                    var OptionCodeData = Mapper.Map<tblOptionCode>(OptionCodeModel);
                    OptionCodeData.CreatedBy = DataBaseCon.ActiveUser();
                    OptionCodeData.CreatedOn = DateTime.Now;
                    dbContext.tblOptionCodes.Add(OptionCodeData);
                    dbContext.SaveChanges();
                }
            }
            //P 10 Jan OptionCode

            if (model != null)
            {
                model.include_job = model.include == "Yes" ? true : false;
                var Entity = Mapper.Map<tbloption>(model);
                if (Entity.id > 0)
                {
                    response = _baseService.OptionBatchTransaction(Entity, BatchOperation.Update);
                }
                else
                {
                    response = _baseService.OptionBatchTransaction(Entity, BatchOperation.Insert);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDecorationCostById(string Desc)
        {
            var getData = dbContext.tblDecorationCosts.Where(_ => _.Dec_Desc == Desc && _.Status == "Active").Select(item => new DecorationCostViewModel { Quantity = item.Quantity, Cost = item.Cost }).ToList();

            return Json(getData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDecorationImagesList(string keyword)     //13 July 2019 (N)
        {
            var DecImageList = _baseService.GetDecorationImagesList(keyword).Select(item => new ApplicationViewModel
            {
                ApplicationId = item.ApplicationId,
                DecorationDate = item.DecorationDate,
                AppName = item.AppName,
                AppType = item.AppType,
                //AcctMgrId = item.AcctMgr,
                AppImage = item.AppImage
            }).ToList();

            return Json(DecImageList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeStageByOppoID(int OppId, string Stage)
        {
            var data = _baseService.ChangeStageByOppoID(OppId, Stage);
            //29 April Stage Change List
            var JobStage = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).Select(_ => _.Stage).FirstOrDefault();
            //29 April Stage Change List
            //return Json(data, JsonRequestBehavior.AllowGet);
            return Json(new { data = data, Stage = JobStage }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadOppImage(string imageData, string filename, int OppId)
        {
            string path = "~/Content/uploads/Opportunity/";
            path = Server.MapPath(path);
            var OldFile = filename;
            var OldPath = path + OldFile;
            var FileName = filename.Split('.');
            string field = filename.Split('_')[0];
            string NewFileName = field + "_" + OppId + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).Ticks + "." + FileName[1];
            //string newFileName = FileName[0] + "_" + OppId + "." + FileName[1];DateTime.Now.Ticks

            string newPath = path + NewFileName;


            byte[] bytes = Convert.FromBase64String(imageData.Split(',')[1]);

            if (System.IO.File.Exists(OldPath))
            {
                System.IO.File.Delete(OldPath);
            }

            using (var imageFile = new FileStream(newPath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

            var data = _baseService.UploadOppImage(NewFileName, OppId, field, path);

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetInquiryData(int Oppid)
        {
            var data = _baseService.GetInquiryData(Oppid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateOppInquiry(InquiryViewModel model)
        {
            //  bool Result = false;

            if (model != null)
            {
                var Entity = Mapper.Map<tblInquiry>(model);
                response = _baseService.UpdateOppInquiry(Entity);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpportunityContactMapping(OppContactMappingViewModel model)
        {
            response = _baseService.OpportunityContactMapping(model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteOppImage(InquiryImageDeleteViewModel model)
        {
            //bool Result = false;
            string path = "~/Content/uploads/Opportunity/";
            path = Server.MapPath(path);
            if (model != null)
            {
                response = _baseService.DeleteOppImage(model, path);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadOppThumbnail(string imageData, string filename, int OppId)
        {

            ImageResponseViewModel res = new ImageResponseViewModel();
            try
            {
                string path = "~/Content/uploads/Opportunity/";
                path = Server.MapPath(path);
                var OldFile = filename;
                var OldPath = path + OldFile;
                var FileName = filename.Split('.');
                string NewFileName = FileName[0] + Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())).Ticks + "." + FileName[1];
                string newPath = path + NewFileName;
                byte[] bytes = Convert.FromBase64String(imageData.Split(',')[1]);

                if (System.IO.File.Exists(OldPath))
                {
                    System.IO.File.Delete(OldPath);
                }

                using (var imageFile = new FileStream(newPath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                res = _baseService.UploadOppThumbnail(NewFileName, OppId, path);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Result = KEN.Models.ResponseType.Error;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomOppList(string CustomText, string TableName)
        {
            var OppListData = _baseService.GetCustomOppList(CustomText, TableName);
            var Jsonresult = Json(OppListData, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }

        //  baans change 25th Sept for Moving to Invoice
        public ActionResult GetOptionPackStatus(int OppId)
        {
            var IsValid = true;
            // Commnet the check for checking the size packed for each Option 24th November
            //var CountData = dbContext.tbloptions.Where(_ => _.OpportunityId == OppId && _.include_job == true && _.OptionStage == "Order").ToList();
            //for(var h=0; h<CountData.Count; h++)
            //{
            //    if(CountData[h].SizesPacked == null)
            //    {
            //        IsValid = false;
            //    }
            //}
            // baans end 24th November
            return Json(IsValid, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOptionCompleteStatus(int OppId)
        {
            var IsValid = true;
            var Data = dbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).Select(_ => _.ConsigNoteNo).FirstOrDefault();
            if (Data == null)
            {
                IsValid = false;
            }
            return Json(IsValid, JsonRequestBehavior.AllowGet);
        }
        // baans end 25th Sept

        //9 Aug 2018 (N)
        public ActionResult GetPaymentHistory(int id)
        {
            var data = _baseService.GetPaymentHistory(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //9 Aug 2018 (N)
        public ActionResult PackingDetails(int Id)
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
            ViewBag.StateList = GetAllStateList();
            ViewBag.PaymentMethodList = GetPaymentMethodList();
            ViewBag.ID = Id;
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }
        public ActionResult InvoicingDetails(int Id)
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
            ViewBag.StateList = GetAllStateList();
            ViewBag.PaymentMethodList = GetPaymentMethodList();
            ViewBag.ID = Id;
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }
        public ActionResult ShippingDetails(int Id)
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
            ViewBag.StateList = GetAllStateList();
            ViewBag.PaymentMethodList = GetPaymentMethodList();
            ViewBag.ID = Id;
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }
        public ActionResult CompleteDetails(int Id)
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
            ViewBag.StateList = GetAllStateList();
            ViewBag.PaymentMethodList = GetPaymentMethodList();
            ViewBag.ID = Id;
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }
        public ActionResult UpdateOppPackin(OpportunityPackInViewModel model)
        {
            response = _baseService.UpdateOppPackin(model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPackinDetails(int OppId)
        {
            var result = false;
            result = _baseService.GetPackinDetails(OppId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetShipDetails(int OppId)
        {
            var result = false;
            result = _baseService.GetPackinDetails(OppId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //baans change by prashant 14 aug start
        public ActionResult getBalance(int OppId, string Optionstage)
        {
            var Balance = dbContext.Pro_GetOppBalance(OppId, Optionstage).FirstOrDefault();
            if (Balance == null)
                Balance = 0;
            //return View(Balance);
            return Json(Balance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPaymentList(int OrgId)
        {

            var Paymentdata = _baseService.GetPaymentList(OrgId);
            return Json(Paymentdata, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCommandDataForPaymentDescription()
        {
            var data = Mapper.Map<CommonDataViewModel>(_baseService.GetCommandDataForPaymentDescription());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Dheeraj change 31Aug start

        public ActionResult PaymentDetails(PaymentViewModel model)
        {
            var Entity = Mapper.Map<tblPayment>(model);
            if (model.PmtId > 0)
            {
                response = _baseService.PaymentBatchTransaction(Entity, model, BatchOperation.Update);

            }
            else
            {
                response = _baseService.PaymentBatchTransaction(Entity, model, BatchOperation.Insert);
                if (response.Result == "Success")
                {
                    savePaymentinQuickbooks(model);
                }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public ActionResult savePaymentinQuickbooks(PaymentViewModel model)
        {
            bool result = false;
            try
            {
                //User Authentication
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

                //Generating New Cuystomer If Not Exists
                Customer Quickbookscustomer = new Customer();
                if (model.OrgId != null && model.OrgId != 0)
                {
                    var orgData = _OrgbaseService.GetOrganisationById(Convert.ToInt32(model.OrgId));
                    QueryService<Customer> customerQueryService = new QueryService<Customer>(serviceContext);
                    var intid = "0";
                    if (orgData.IntuitID != null)
                    {
                        intid = Convert.ToString(orgData.IntuitID);
                    }

                    Quickbookscustomer = customerQueryService.ExecuteIdsQuery("Select * From Customer where Id='" + intid + "'").FirstOrDefault();
                    if (Quickbookscustomer == null)
                    {
                        var Oppdata = dbContext.tblOpportunities.Where(_ => _.OpportunityId == model.OpportunityId).Select(_ => _.AddressId).FirstOrDefault();

                        List<tblAddress> orgAddress = new List<tblAddress>();
                        if (Oppdata == null)
                        {
                            orgAddress = dbContext.tblAddresses.Where(_ => _.OrgId == model.OrgId && _.DeliveryAddress == "Delivery1").ToList();
                        }
                        else
                        {
                            orgAddress = dbContext.tblAddresses.Where(_ => _.AddressId == Oppdata).ToList();
                        }

                        Customer cust = new Customer();

                        PhysicalAddress DiliveryAddress1 = new PhysicalAddress();
                        //   WebSiteAddress WebAddress = new WebSiteAddress();
                        TelephoneNumber contactdetail = new TelephoneNumber();
                        contactdetail.FreeFormNumber = orgData.MainPhone;
                        // WebAddress.URI = "http://www.google.com";
                        if (orgAddress != null && orgAddress.Count != 0)
                        {
                            DiliveryAddress1.Country = orgAddress[0].Country;
                            DiliveryAddress1.PostalCode = orgAddress[0].Postcode;
                            DiliveryAddress1.Line1 = orgAddress[0].Address1;
                            DiliveryAddress1.Line2 = orgAddress[0].Address2;
                            cust.BillAddr = DiliveryAddress1;
                        }
                        cust.GivenName = orgData.OrgName;
                        cust.DisplayName = orgData.OrgName;
                        cust.PrimaryPhone = contactdetail;
                        // cust.WebAddr = WebAddress;
                        Quickbookscustomer = commonServiceQBO.Add(cust);
                        //Quickbookscustomer = commonServiceQBO.Update(cust);

                        var orgid = Convert.ToInt32(model.OrgId);
                        _OrgbaseService.AddIntuitID(orgid, Quickbookscustomer.Id);
                    }
                    else
                    {
                        var Oppdata = dbContext.tblOpportunities.Where(_ => _.OpportunityId == model.OpportunityId).Select(_ => _.AddressId).FirstOrDefault();

                        List<tblAddress> orgAddress = new List<tblAddress>();
                        if (Oppdata == null)
                        {
                            orgAddress = dbContext.tblAddresses.Where(_ => _.OrgId == model.OrgId && _.DeliveryAddress == "Delivery1").ToList();
                        }
                        else
                        {
                            orgAddress = dbContext.tblAddresses.Where(_ => _.AddressId == Oppdata).ToList();
                        }

                        PhysicalAddress DiliveryAddress1 = new PhysicalAddress();
                        //   WebSiteAddress WebAddress = new WebSiteAddress();
                        TelephoneNumber contactdetail = new TelephoneNumber();
                        contactdetail.FreeFormNumber = orgData.MainPhone;
                        // WebAddress.URI = "http://www.google.com";
                        if (orgAddress != null && orgAddress.Count != 0)
                        {
                            DiliveryAddress1.Country = orgAddress[0].Country;
                            DiliveryAddress1.PostalCode = orgAddress[0].Postcode;
                            DiliveryAddress1.Line1 = orgAddress[0].Address1;
                            DiliveryAddress1.Line2 = orgAddress[0].Address2;
                            Quickbookscustomer.BillAddr = DiliveryAddress1;
                        }
                        Quickbookscustomer.GivenName = orgData.OrgName;
                        Quickbookscustomer.DisplayName = orgData.OrgName;
                        Quickbookscustomer.PrimaryPhone = contactdetail;
                        Quickbookscustomer = commonServiceQBO.Update(Quickbookscustomer);
                    }


                    //Generating Invoice For The Customer
                    QueryService<Invoice> InvoiceQueryService = new QueryService<Invoice>(serviceContext);
                    var QuickbooksInvoice = InvoiceQueryService.ExecuteIdsQuery("Select * From invoice where  DocNumber = '" + model.OpportunityId + "'").ToList();
                    var OptionsForInvoice = _baseService.getOptionsForInvoice(Convert.ToInt32(model.OpportunityId), "Order");
                    var OppShipping = DbContext.tblOpportunities.Where(_ => _.OpportunityId == model.OpportunityId).Select(_ => _.Price).FirstOrDefault();

                    QueryService<Account> accountQueryService = new QueryService<Account>(serviceContext);
                    //Account account = accountQueryService.ExecuteIdsQuery("Select * From Account Where AccountType='Accounts Receivable' StartPosition 1 MaxResults 1").FirstOrDefault();

                    Account account;

                    if (model.PmtMethod == "PAYPAL")
                    {
                        account = accountQueryService.ExecuteIdsQuery("Select * From Account where Name='PayPal' StartPosition 1 MaxResults 1").FirstOrDefault();
                    }
                    else
                    {
                        account = accountQueryService.ExecuteIdsQuery("Select * From Account where Name='ANZ Transaction Account' StartPosition 1 MaxResults 1").FirstOrDefault();
                    }

                    Invoice invoiceAdded = new Invoice();

                    if (QuickbooksInvoice.Count == 0 && OptionsForInvoice.Count > 0)
                    {
                        QueryService<TaxCode> TaxCodeQueryService = new QueryService<TaxCode>(serviceContext);
                        var TaxCode = TaxCodeQueryService.ExecuteIdsQuery("Select * From TaxCode where Name='GST'").FirstOrDefault();

                        Invoice invoice = new Invoice();

                        TxnTaxDetail TaxDetail = new TxnTaxDetail();
                        Line[] InvoiceLines = new Line[OptionsForInvoice.Count + 1];
                        int Counter = 0;
                        foreach (var item in OptionsForInvoice)
                        {
                            Line invoiceLine = new Line();

                            invoiceLine.Amount = Convert.ToDecimal(item.ExtExGST);

                            invoiceLine.AmountSpecified = true;
                            invoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                            invoiceLine.DetailTypeSpecified = true;
                            SalesItemLineDetail salesline = new SalesItemLineDetail();
                            salesline.TaxCodeRef = new ReferenceType()
                            {
                                Value = TaxCode.Id,
                            };

                            salesline.Qty = item.quantity;
                            salesline.QtySpecified = true;

                            invoiceLine.Description = item.ItemName + "-" + (item.colour == null ? "" : item.colour) + "-" + item.code + "-" + item.SizeGrid + "-" + item.InitialSizes;
                            invoiceLine.AnyIntuitObject = salesline;

                            InvoiceLines[Counter] = invoiceLine;
                            Counter++;
                        }


                        //start bhav

                        // New InvoiceLine for adding shipping price to the Invoice 
                        Line shipinvoiceLine = new Line();

                        //Add shipping price to the main amount
                        shipinvoiceLine.Amount = Convert.ToDecimal(OppShipping);
                        shipinvoiceLine.AmountSpecified = true;
                        shipinvoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        shipinvoiceLine.DetailTypeSpecified = true;

                        //Providing reference to the shipping item
                        SalesItemLineDetail ShippingSalesLine = new SalesItemLineDetail();
                        ShippingSalesLine.ItemRef = new ReferenceType()
                        {
                            Value = "SHIPPING_ITEM_ID",
                        };

                        ShippingSalesLine.TaxCodeRef = new ReferenceType()
                        {
                            Value = TaxCode.Id,
                        };

                        shipinvoiceLine.AnyIntuitObject = ShippingSalesLine;

                        InvoiceLines[Counter] = shipinvoiceLine;

                        //end bhav


                        invoice.Line = InvoiceLines;
                        invoice.DocNumber = Convert.ToString(model.OpportunityId);
                        invoice.CustomerRef = new ReferenceType()
                        {

                            Value = Quickbookscustomer.Id
                        };

                        /*Invoice */
                        invoiceAdded = commonServiceQBO.Add(invoice);

                    }
                    Payment pay = new Payment();

                    //To link payment with it's respective invoices we need to give reference to the invoice whose payment is made to keep record and manage balance amount.  
                    LinkedTxn[] LinkedInvoice = new LinkedTxn[1];
                    LinkedTxn linklist = new LinkedTxn();
                    if (QuickbooksInvoice.Count != 0)
                    {
                        linklist.TxnId = QuickbooksInvoice[0].Id;
                    }
                    else
                    {
                        linklist.TxnId = invoiceAdded.Id;
                    }
                    linklist.TxnType = "Invoice";

                    //Payment line to add Amount received as per the invoice number.
                    Line[] Payment = new Line[1];
                    Line paymentline = new Line();

                    paymentline.Amount = Convert.ToDecimal(model.AmtReceived);
                    paymentline.AmountSpecified = true;

                    LinkedInvoice[0] = linklist;
                    paymentline.LinkedTxn = LinkedInvoice;
                    Payment[0] = paymentline;

                    pay.PaymentRefNum = Convert.ToString(model.OpportunityId);
                    pay.CustomerRef = new ReferenceType() { Value = Quickbookscustomer.Id };
                    pay.TotalAmt = Convert.ToDecimal(model.AmtReceived);
                    pay.TotalAmtSpecified = true;

                    pay.Line = Payment;

                    pay.PaymentType = PaymentTypeEnum.Check;
                    pay.PaymentTypeSpecified = true;

                    pay.DepositToAccountRef = new ReferenceType()
                    {
                        name = account.Name,
                        Value = account.Id
                    };
                    Payment InsertedPayment = commonServiceQBO.Add(pay);
                }

            }
            catch (Exception ex)
            {

            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }
        protected IOAuthSession CreateSession()
        {
            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1
            };
            return new OAuthSession(consumerContext,
                                    REQUEST_TOKEN_URL,
                                    OAUTH_URL,
                                    ACCESS_TOKEN_URL);
        }
        public ActionResult Callback(string state, string code, string realmId)
        {
            if (Request.QueryString.Count > 0)
            {

                IOAuthSession clientSession = CreateSession();
                IToken requestToken = clientSession.GetRequestToken();
                requestToken.Token = strrequestToken;
                requestToken.TokenSecret = tokenSecret;
                IToken accessToken = clientSession.ExchangeRequestTokenForAccessToken(requestToken, Request.QueryString["oauth_verifier"].ToString());
                UpdateTOken(accessToken);
                _baseService.UpdateTOken(realmId, "QuickBooks_RealmID");
                OAuthRequestValidator oauthValidator = new OAuthRequestValidator(accessToken.Token, accessToken.TokenSecret, consumerKey, consumerSecret);
                ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            }
            return RedirectToAction("OrderDetails", new
            {
                Id = Session["OppId"]
            });
        }
        public ActionResult UpdateTOken(IToken tokenResp)
        {


            _baseService.UpdateTOken(tokenResp.Token, "QuickBooks_Access_Token");
            _baseService.UpdateTOken(tokenResp.TokenSecret, "QuickBooks_Access_TokenSecret");
            //   return RedirectToAction("OrderDetails", "QuickBooks");
            return RedirectToAction("OrderDetails", new
            {
                Id = Session["OppId"]
            });

        }
        public ActionResult QuickBooksAuthentication(QuickBooksAuthViewModel model)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            bool result = false;
            bool Acctresult = true;
            string URI = "";
            try
            {
                Session["PageSource"] = model.PageSource;
                Session["OppId"] = model.id;
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
                Item items = new Item();
                List<Item> results = commonServiceQBO.FindAll<Item>(items, 1, 1).ToList<Item>();

                Account PayPalaccount, AnzAccount;
                QueryService<Account> accountQueryService = new QueryService<Account>(serviceContext);
                PayPalaccount = accountQueryService.ExecuteIdsQuery("Select * From Account where Name='PayPal' StartPosition 1 MaxResults 1").FirstOrDefault();

                AnzAccount = accountQueryService.ExecuteIdsQuery("Select * From Account where Name='ANZ Transaction Account' StartPosition 1 MaxResults    1").FirstOrDefault();

                if (PayPalaccount == null || AnzAccount == null)
                {
                    Acctresult = false;
                }

                result = true;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unauthorized-401")
                {

                    IOAuthSession session = CreateSession();
                    IToken requestToken = session.GetRequestToken();
                    strrequestToken = requestToken.Token;
                    tokenSecret = requestToken.TokenSecret;
                    tokenSecret = requestToken.TokenSecret;
                    var authUrl = string.Format("{0}?oauth_token={1}&oauth_callback={2}", AUTHORIZE_URL, requestToken.Token, UriUtility.UrlEncode(oauth_callback_url));
                    OAUTH_URL = authUrl;
                    URI = authUrl;
                }
            }

            return Json(new
            {
                result = result,
                Acctresult = Acctresult,
                URI = URI
            }, JsonRequestBehavior.AllowGet);
        }

        //Dheeraj change 31Aug end

        //baans change by prashant 14 aug end
        public ActionResult GetAccountManagerById(int Id)
        {
            //var Data = dbContext.tblusers.Where(_ => _.id == Id).FirstOrDefault();
            var Data = _baseService.GetAccountManagerById(Id);
            var First = Data.firstname;
            var Last = Data.lastname;
            return Json(new { FirstName = First, LastName = Last }, JsonRequestBehavior.AllowGet);

        }

        //18 Aug 2018 (N)
        public ActionResult GetOpportunityByOppName(string prefix)
        {
            var data = _baseService.GetOpportunityByOppName(prefix).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //18 Aug 2018 (N)



        //25 Sep 2018 (N)
        public ActionResult GetOptionStatus(int Oppid)
        {
            var result = false;
            result = _baseService.GetOptionStatus(Oppid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //25 Sep 2018 (N)   

        // baans change 26th September for ProductionDate
        public ActionResult GetOppProductionDate(int OppId)
        {
            var DateCount = Mapper.Map<List<KanBanViewModel>>(dbContext.tblkanbans.Where(_ => _.OppId == OppId && _.ProductionDate != null).OrderByDescending(_ => _.KanbanId)).ToList();



            return Json(DateCount, JsonRequestBehavior.AllowGet);
            //return Json(new { data = DateCount, Dept = Dept, Dept1 = Dept1 }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetOppProductionDepartment(int DepId)
        {
            var Data = Mapper.Map<List<DepartmentViewModel>>(DbContext.tbldepartments.Where(_ => _.id == DepId)).ToList();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        // baans end 26th September

        // baans change 28th September for Active USER
        public ActionResult GetActiveUser()
        {
            var ActiveUser = DataBaseCon.ActiveUser();
            var User = dbContext.tblusers.Where(_ => _.email == ActiveUser).FirstOrDefault();
            var CurrentUser = User.firstname;
            //var userImage = User.userpic;
            //return Json(new { CurrentUser = CurrentUser, userImage = userImage }, JsonRequestBehavior.AllowGet);
            return Json(CurrentUser, JsonRequestBehavior.AllowGet);
        }
        // Baans end 28th September
        // baans change 23rd October for delete the Option

        public ActionResult DeleteOption(int Id)
        {
            response = _baseService.DeleteOption(Id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // baans end 23rd October
        // baans change 24th October for Checking the partial Payment
        public ActionResult CheckPaymentForOpportunity(int OppId)
        {
            var IsValid = false; ;
            var data = dbContext.tblPayments.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
            if (data != null)
            {
                IsValid = true;
            }
            return Json(IsValid, JsonRequestBehavior.AllowGet);
        }
        // baans end 24th October
        // baans change 30th october for checking tbc
        public ActionResult CheckTbcSize(int OppId)
        {
            var IsValid = true;

            var data = DbContext.tbloptions.Where(_ => _.OpportunityId == OppId && _.OptionStage == "Order" && _.include_job == true).ToList();
            if (data.Count != 0)
            {
                for (var h = 0; h < data.Count; h++)
                {
                    if (data[h].InitialSizes.Contains("TBC"))
                    {
                        IsValid = false;
                    }
                }
            }
            return Json(IsValid, JsonRequestBehavior.AllowGet);
        }
        // baans end 30th october
        // baans change 2nd November for checking the Delivery Address
        public ActionResult CheckDeliveryAddressByOppId(int OppId)
        {
            var IsValid = false;
            var data = DbContext.tblOppContactMappings.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
            if (data != null)
            {
                var OrgId = DbContext.Vw_tblContact.Where(_ => _.id == data.ContactId).FirstOrDefault();
                if (OrgId != null)
                {
                    var OrgAddId = DbContext.tblAddresses.Where(_ => _.OrgId == OrgId.OrgId).FirstOrDefault();
                    if (OrgAddId != null)
                    {
                        IsValid = true;
                    }
                }
            }
            return Json(IsValid, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckOldConfirmationDateByOppId(int OppId, string ConfirmedDate)
        {
            var IsConfValid = false;
            var Confdate = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).FirstOrDefault();
            if (Confdate != null)
            {
                if (Confdate.ConfirmedDate != null)
                {
                    if (ConfirmedDate != "")
                    {
                        DateTime NewConfirmDate = Convert.ToDateTime(ConfirmedDate);
                        if (NewConfirmDate != Confdate.ConfirmedDate)
                        {
                            IsConfValid = true;
                        }
                    }
                }
            }
            return Json(IsConfValid, JsonRequestBehavior.AllowGet);
        }
        // baans end 2nd November
        // baans change 03rd November for checking the OppStageFor Confirmation
        public ActionResult CheckStageByOppoIdForReconfirmation(int OppId)
        {
            var IsValid = false;
            var Data = DbContext.tblOpportunities.Where(_ => _.OpportunityId == OppId).Select(_ => _.Stage).FirstOrDefault();
            if (Data != null)
            {
                if (Data == "Job")
                {
                    IsValid = true;
                }
            }
            return Json(IsValid, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeConfirmedDateOppoID(int OppId, string ConfirmedDate)
        {
            var data = _baseService.ChangeConfirmedDateOppoID(OppId, ConfirmedDate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //baans end 03rd November

        //13 Nov 2018 (N)
        public ActionResult OrgData(int OrgId)
        {
            var data = _baseService.OrgData(OrgId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //13 Nov 2018 (N)
        // Baans change 15th November for current title
        public ActionResult GetUserByTitle()
        {
            var CurrentUser = 0;
            //var ActiveUser = DataBaseCon.ActiveUser();
            //var User = dbContext.tblusers.Where(_ => _.email == ActiveUser && (_.title == "Account Manager" || _.title == "Administrator")).FirstOrDefault();
            //if (User != null)
            //{
            //     CurrentUser = User.firstname + " " + User.lastname;
            //}
            var IsContains = false;
            var UserAdminRight = false;
            var ListData = DbContext.tblusers.Where(_ => _.UserRole == "Account Manager").Select(_ => _.id).ToList();
            var ActiveUser = DataBaseCon.ActiveUser();
            var UserId = DbContext.tblusers.Where(_ => _.email == ActiveUser).Select(_ => _.id).FirstOrDefault();
            if (ListData.Contains(UserId))
            {
                IsContains = true;
            }
            var SessionMyUser = (int)Session["MyUser"];
            if ((int)Session["MyUser"] == 0)
            {
                IsContains = true;
            }

            if (IsContains)
            {
                CurrentUser = (int)Session["MyUser"];
            }
            else
            {
                if (this.Session["EventType"] == "LoadEvent")
                {
                    var DefaultData = DbContext.tblusers.Where(_ => _.UserRole == "Account Manager").Select(_ => _.id).OrderBy(_ => _).ToList();
                    for (var h = 0; h < 1; h++)
                    {
                        CurrentUser = DefaultData[h];
                    }
                }
                else
                {
                    CurrentUser = (int)Session["MyUser"];
                }


            }

            var IsUserAdmin = dbContext.tblusers.Where(_ => _.email == ActiveUser && _.admin == true).FirstOrDefault();
            if (IsUserAdmin != null)
            {
                UserAdminRight = true;
            }
            //return Json(CurrentUser, JsonRequestBehavior.AllowGet);
            return Json(new { CurrentUser = CurrentUser, AdminRight = UserAdminRight }, JsonRequestBehavior.AllowGet);
        }
        // baans end 15th November
        // baans change 16th November for change the user by dropdown
        public ActionResult SetNewUser(int UserProfile)
        {
            var OldSession = this.Session["MyUser"];
            this.Session["EventType"] = "ChangeEvent";
            this.Session["MyUser"] = UserProfile;
            var NewUser = this.Session["MyUser"];
            return Json(NewUser, JsonRequestBehavior.AllowGet);
        }
        // baans end 16th November

        // Baans change 26th November to getOptionDataByOptId
        public ActionResult getOptionDataByOptId(int OptId)
        {


            var Data = DbContext.tbloptions.Where(_ => _.id == OptId).FirstOrDefault();
            var OptionData = Mapper.Map<OptionViewModel>(Data);
            return Json(OptionData, JsonRequestBehavior.AllowGet);
        }
        // baans end 26th November

        // baans change 10th January for checkOppStatusForRepeatOrder
        public ActionResult StatusChkByOppIdForMakeRepeat(int OppId)
        {
            var Result = false;
            Result = _baseService.StatusChkByOppIdForMakeRepeat(OppId);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MakeRepeatOrder(int OppId)
        {
            if (OppId != null)
            {
                response = _baseService.MakeRepeatOrder(OppId);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // baans end 10th January

        //P 10 Jan OptionCode
        public ActionResult OptionCodeByPrefix(string Prefix)
        {
            var data = _baseService.GetOptionByPrefix(Prefix);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //P 10 Jan OptionCode

        // 29 April NotesEditing List
        public ActionResult UpdateStatusNotes(int id, string stage, string notes)
        {
            var Oppdata = dbContext.Vw_tblOpportunity.Where(_ => _.OpportunityId == id).FirstOrDefault();
            if (Oppdata.Stage == stage && Oppdata.StageWiseNotes != notes)
            {
                response = _baseService.UpdateStatusNotes(id, stage, notes);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // 29 April NotesEditing List

        //29 April Stage Change List
        public ActionResult GetJobStatusByOppId(int OppId, string lblDate)
        {
            var result = false;
            result = _baseService.GetJobStatusByOppId(OppId, lblDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //29 April Stage Change List
        public List<UserViewModels> GetOutSourceUser() //20Aug2020
        {
            var UserData = Mapper.Map<List<UserViewModels>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Outsourcer" && _.status == "Active").ToList());
            return UserData;
        }
        public string GetActiveUserData()
        {
            var ActiveUser = DataBaseCon.ActiveUser();
            var ActiveUserRole = dbContext.tblusers.Where(_ => _.email == ActiveUser).Select(_ => _.UserRole).FirstOrDefault();
            return ActiveUserRole;
        }
        public ActionResult UpdateDecoration(int id, string location)
        {
            var OptionData = dbContext.tbloptions.Where(_ => _.id == id).FirstOrDefault();
            if (location == "Front")
            {
                OptionData.front_decoration = null;
            }
            else if (location == "Back")
            {
                OptionData.back_decoration = null;
            }
            else if (location == "Left")
            {
                OptionData.left_decoration = null;
            }
            else if (location == "Right")
            {
                OptionData.right_decoration = null;
            }
            else if (location == "Other")
            {
                OptionData.extra_decoration = null;
            }
            dbContext.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}