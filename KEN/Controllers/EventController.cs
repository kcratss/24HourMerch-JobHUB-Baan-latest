using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KEN.Models;
using AutoMapper;
using KEN_DataAccess;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.Filters;

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class EventController : Controller
    {
        private readonly IEventService _baseService;
        ResponseViewModel response = new ResponseViewModel();
        public EventController(IEventService baseService)
        {
            _baseService = baseService;
        }
        KENNEWEntities dbContext = new KENNEWEntities();   // Add
        public ActionResult EventList()
        {
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }
        public ActionResult EventDetails(int id)
        {
            //16 Aug (N)
            ViewBag.Id = id;
            ViewBag.Cycles = GetCycles();
            ViewBag.ShippingList = GetShipping();
            ViewBag.SourceList = GetSource();
            ViewBag.StageList = GetStage();
            ViewBag.CampaignList = GetCampaign();
            ViewBag.ManagerList = GetAccountManagers();
            ViewBag.ContactTypes = GetContactType();
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
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
        public List<AccountManagerDropdownViewModel> GetAccountManagers()
        {

            // string[] Roles = new string[] { "Administrator", "Account Manager","Production Director"};
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.title)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
        }
        public IEnumerable<tblCampaign> GetCampaign()
        {
            var getData = dbContext.tblCampaigns.ToList().OrderBy(_ => _.Campaign);
            return getData;
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
        public ActionResult GetEventList()
        {
            var LeadContactTypeList = _baseService.GetLeadsEventTypeList();
            return Json(LeadContactTypeList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult updateEvent(EventViewModel model)
        {

            if (model != null)
            {
                var Entity = Mapper.Map<tblEvent>(model);
                if (model.EventId > 0)
                {
                    response = _baseService.BatchTransaction(Entity, BatchOperation.Update);
                }
                else {
                    response = _baseService.BatchTransaction(Entity,BatchOperation.Insert);
                }
            }

           
            return Json(response,JsonRequestBehavior.AllowGet);
        }
        // GET: Event
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetEventByName(string prefix)
        {
            var data = _baseService.GetEventByName(prefix).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddEvent(EventViewModel model)
        {
            if (model.EventId>0)
            {

                var Entity = Mapper.Map<tblEvent>(model);
                response = _baseService.EventBatchTransaction(Entity,model.OpportunityId, BatchOperation.Update, model.PageSource);

            }
            else
            {
                var Entity = Mapper.Map<tblEvent>(model);
                response = _baseService.EventBatchTransaction(Entity, model.OpportunityId, BatchOperation.Insert, model.PageSource);
            }
            
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEventById(int EventId)
        {

           var EventData=Mapper.Map<EventViewModel>(  _baseService.GetEventById(EventId));
            return Json(EventData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomEventList(string CustomText, string TableName)
        {
            var EventListData = _baseService.GetCustomEventList(CustomText, TableName);
            var Jsonresult = Json(EventListData, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }

        //17 Aug 2018 (N)
        public ActionResult GetOpportunityByEventId(string Stage, int EventId)
        {
            var data = _baseService.GetOpportunityByEventId(Stage, EventId);
            //var FirstData = data.First();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //17 Aug 2018 (N)
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