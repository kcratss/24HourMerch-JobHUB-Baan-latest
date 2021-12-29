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

namespace KEN.Controllers
{
    [UserAuthenticationFilter]
    public class ContactController : Controller
    {
        ResponseViewModel response = new ResponseViewModel();
        // GET: Contact
        private readonly IContactService _baseService;
        KENNEWEntities dbContext = new KENNEWEntities();
        public ContactController(IContactService baseService)
        {
            _baseService = baseService;
        }
        public ActionResult ContactList()
        {
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November
            return View();
        }

        // baans change 27th September for Autocomplete by Type
        public ActionResult GetContactByFirstName(string prefix, string ContType)
        {
            var data = _baseService.GetByFirstPrefix(prefix, ContType).ToList();
            // baans end 27th September
            // baans change 08 September for Autocomplete
            var Jsonresult = Json(data, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = Int32.MaxValue;
            return Jsonresult;
            //return Json(data, JsonRequestBehavior.AllowGet);
            //baans end
        }
        // baans change 27th September for Autocomplete by Type
        public ActionResult GetContactByLastName(string prefix, string ContType)
        {
            var data = _baseService.GetByLastPrefix(prefix, ContType).ToList();
            // baans end 27th September
            // baans change 08 September for Autocomplete
            var Jsonresult = Json(data, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = Int32.MaxValue;
            return Jsonresult;
            //return Json(data, JsonRequestBehavior.AllowGet);
            //baans end
        }


        public ActionResult AddContact(ContactViewModel model,OppContactMappingViewModel MappingModel)
        {
            //return null;
           // bool Result = false;

            var response = new ResponseViewModel();
            if (model != null)
            {
                var mappingflag = true;
                if (model.PageSource != "ContactDetails")
                {
                    mappingflag= _baseService.ValidateMapping(MappingModel);
                }
                if (mappingflag)
                {
                var Entity = Mapper.Map<tblcontact>(model);
                response = _baseService.ValidateContact(Entity, MappingModel,model.PageSource);
            }
                else
                {
                    response.Message = "Primary contact already Exist for this opportunity";
                     response.Result = ResponseType.Warning;
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult updateContact(ContactViewModel model)
        {
            if (model != null)
            {
                var Entity = Mapper.Map<tblcontact>(model);
                response = _baseService.Update(Entity);
            }
           
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContactList(string Leads)
        {
            var LeadContactTypeList = _baseService.GetLeadsContactTypeList(Leads);
            var Jsonresult = Json(LeadContactTypeList, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }
        public ActionResult GetContactByOppId(int OppId)
        {
            var ContactList = _baseService.GetContactByOppId(OppId);
          
           return Json(ContactList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ContactDetails(int id)
        {
            var model = new ContactViewModel();
            if (id > 0)
            {
                model = Mapper.Map<ContactViewModel>(_baseService.GetContactById(id));
            }

            ViewBag.Id = id;
            //ViewBag.ContactRoles = GetContactRoles();
            ViewBag.ManagerList = GetAccountManagers();
            ViewBag.ContactTypes = GetContactType();
            ViewBag.ContactTitle = GetContactTitle();
            ViewBag.StateList = GetAllStateList();
            // baans change 15th November
            ViewBag.ProfileList = getProfileList();
            // baans end 15th November

            return View(model);
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

        public List<EnumViewModel> GetContactTitle()
        {
            var getData = (from ContactTitle e in Enum.GetValues(typeof(ContactTitle)) select new { Name = e.ToString() }).ToList();
            var newData = getData.Select(item => new EnumViewModel
            {
                Name = item.Name,
            }).ToList();
            return newData;
        }
        public List<AccountManagerDropdownViewModel> GetAccountManagers()
        {

            // string[] Roles = new string[] { "Administrator", "Account Manager","Production Director"};
            var getData = Mapper.Map<List<AccountManagerDropdownViewModel>>(dbContext.tblusers
                .Where(_ => _.UserRole == "Account Manager" && _.status == "Active").ToList().OrderBy(_ => _.title)).OrderBy(_ => _.AccountManagerFullName).ToList();
            return getData;
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
            objState.Add(new StateList { stateName = "ACT" }); //added by baans 08Aug2020 
            objState.Add(new StateList { stateName = "NSW" });
            objState.Add(new StateList { stateName = "NT" });
            objState.Add(new StateList { stateName = "QLD" });
            objState.Add(new StateList { stateName = "SA" });
            objState.Add(new StateList { stateName = "TAS" });
            objState.Add(new StateList { stateName = "VIC" });
            objState.Add(new StateList { stateName = "WA" });

            return objState;
        }

        public ActionResult GetContactById(int ContactId)
        {
            var data = Mapper.Map<ContactViewModel>(_baseService.GetContactById(ContactId));
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOppoListByContactOrOrganisation(string Stage, int ID,string PageSource)
        {
            var GridContactDetails = _baseService.GetOppoListByContactOrOrganisation(Stage, ID, PageSource);
            // baans change 18th October for OpportunityList for new data
            var Jsonresult = Json(GridContactDetails, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
            //return Json(GridContactDetails, JsonRequestBehavior.AllowGet);
            // baans end 18th October
        }

        public ActionResult GetContactDownList(int id,string Status)
        {
            var GridData = _baseService.getContactGSTGrid(id,Status);
            // baans change 22nd October for include 
            foreach(var item in GridData)
            {
                item.include = item.include_job == true ? "Yes" : "No";
            }
            // baans end 22nd October
            return Json(GridData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContactByOrgId(int OrgId)
        {
            var data = Mapper.Map<List< ContactViewModel>>(_baseService.GetContactByOrgId(OrgId));
            // Baans change 18th October for ContactList for new data
            var Jsonresult = Json(data, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
            // return Json(data, JsonRequestBehavior.AllowGet);
            // baans end 18th October
        }
        public ActionResult CheckOrgByOppoID(int OpportunityID, string Stage)
        {
            // baans change 23rd August for Change in ConfirmationMessage
            var resultOrg = _baseService.CheckOrgByOppoID(OpportunityID);
            var result = _baseService.CheckPrimaryContactByOppoID(OpportunityID);
            var RequiredData = _baseService.CheckMandatoryFieldsInOppo(OpportunityID);
            // baans change 30th November for correct OptionCheck based on the Stage
            var OptionCount = 0;
            if (Stage == "Opp")
            {
                 OptionCount = dbContext.tbloptions.Where(_ => _.OpportunityId == OpportunityID && _.include_job == true && _.OptionStage == "Opp").ToList().Count;
            }
            else
            {
                 OptionCount = dbContext.tbloptions.Where(_ => _.OpportunityId == OpportunityID && _.include_job == true && _.OptionStage == "Order").ToList().Count;
            }

            // baans end 30th November
            return Json(new { result = result, resultOrg = resultOrg, RequiredData = RequiredData, OptionCount = OptionCount }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCustomContactList(string CustomText, string TableName)
        {
            var LeadContactTypeList = _baseService.GetCustomContactList(CustomText, TableName);
            var Jsonresult = Json(LeadContactTypeList, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = int.MaxValue;
            return Jsonresult;
        }
        // baans change 27th September for Autocomplete by Type
        public ActionResult GetContactByEmail(string prefix, string ContType)
        {
            var data = _baseService.GetByEmailPrefix(prefix, ContType).ToList();
            // baans end 27th September
            // baans change 08 September for Autocomplete
            var Jsonresult = Json(data, JsonRequestBehavior.AllowGet);
            Jsonresult.MaxJsonLength = Int32.MaxValue;
            return Jsonresult;
            //return Json(data, JsonRequestBehavior.AllowGet);
            //baans end
        }
        // baans change 4th October for checking the contact Status
        public ActionResult GetContactLinkStatus(int Id)
        {
            var ContIsValid = true;
            var data = dbContext.tblOppContactMappings.Where(_ => _.ContactId == Id).FirstOrDefault();
            if(data == null)
            {
                ContIsValid = false;
            }
            return Json(ContIsValid, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteContact(int Id)
        {
            var ContactDeletion = true;
            response = _baseService.DeleteContact(Id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // baans end 4th oct
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