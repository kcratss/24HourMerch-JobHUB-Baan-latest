using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN_DataAccess;
using KEN.Interfaces.Repository;
using AutoMapper;
using KEN.AppCode;
using KEN.Models;
using System.Text.RegularExpressions;

namespace KEN.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<tbloption> _tblOptionRepository;
        private readonly IRepository<tblcontact> _st_contactRepository;
        private readonly IRepository<Vw_tblContact> _VW_tblcontactRepository;
        private readonly IRepository<tblOpportunity> _tblOpportunityRepository;
        private readonly IRepository<tblOppContactMapping> _tblOppContactMappingRepository;
        private readonly IRepository<Vw_tblOpportunity> _Vw_tblOpportunityRepository;
        ResponseViewModel response = new ResponseViewModel();
        KENNEWEntities DbContext = new KENNEWEntities();
        public ContactService(IRepository<tblcontact> st_contactRepository, IRepository<tblOpportunity> tblOpportunityRepository, IRepository<tblOppContactMapping> tblOppContactMappingRepository, IRepository<tbloption> tblOptionRepository, IRepository<Vw_tblContact> VW_tblcontactRepository, IRepository<Vw_tblOpportunity> Vw_tblOpportunityRepository)
        {
            _st_contactRepository = st_contactRepository;
            _tblOpportunityRepository = tblOpportunityRepository;
            _tblOppContactMappingRepository = tblOppContactMappingRepository;
            _tblOptionRepository = tblOptionRepository;
            _VW_tblcontactRepository = VW_tblcontactRepository;
            _Vw_tblOpportunityRepository = Vw_tblOpportunityRepository;
        }
        // baans change 27th September for Autocomplete by Type
        public IEnumerable<DropDownViewModel> GetByFirstPrefix(string Prefix, string ContType)
        {
            //23 Aug 2018 (N)
            var data = Mapper.Map<IEnumerable<ContactViewModel>>(_st_contactRepository.Get(_ => _.first_name.Contains(Prefix)));
            // baans end 27th September
            var newdata = data.Select(item => new DropDownViewModel
            {
                id = item.id,
                first_name = item.first_name,
                last_name = item.last_name,
                email = item.email,
                Company = item.company,
                title = item.title,
                notes = item.notes,
                mobile = item.mobile,
                ContactType = item.ContactType,
                acct_manager_id = item.acct_manager_id,
                ContactRole = item.ContactRole,
                OrgId = item.OrgId,
                OrgName = item.OrgName
                //23 Aug 2018 (N)


            }
           ).OrderBy(_ => _.first_name).ThenBy(_ => _.last_name).ToList();
            return newdata;
        }
        // baans change 27th September for Autocomplete by Type
        public IEnumerable<DropDownViewModel> GetByLastPrefix(string Prefix, string ContType)
        {
            //23 Aug 2018 (N)
            // baans change 10th Sept
            var data = Mapper.Map<IEnumerable<ContactViewModel>>(_st_contactRepository.Get(_ => _.last_name.Contains(Prefix)));
            // baans end 27th September
            // baans end 
            var newdata = data.Select(item => new DropDownViewModel
            {
                id = item.id,
                first_name = item.first_name,
                last_name = item.last_name,
                email = item.email,
                Company = item.company,
                title = item.title,
                notes = item.notes,
                mobile = item.mobile,
                ContactType = item.ContactType,
                acct_manager_id = item.acct_manager_id,
                ContactRole = item.ContactRole,
                OrgId = item.OrgId,
                OrgName = item.OrgName
                //23 Aug 2018 (N)

            }
           ).OrderBy(_ => _.last_name).ThenBy(_ => _.first_name).ToList();
            return newdata;

        }
        public bool Add(tblcontact entity)
        {
            throw new NotImplementedException();
        }
        public ResponseViewModel BatchTransaction(tblcontact Entity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }
        
        public ResponseViewModel ContactBatchTransaction(tblcontact Entity, OppContactMappingViewModel MappingModel, string PageSource,BatchOperation operation)
        {
            string tblName = "";
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            tblName = TableNames.tblContact;
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            if(Entity.mobile!=null && Entity.mobile !="")
                            {
                                Entity.mobile = Regex.Replace(Entity.mobile, @"[^0-9]", "");
                            }
                            if (PageSource == "OrganisationDetail")
                            {
                                Entity.OrgId = MappingModel.MappingID;
                            }
                            _st_contactRepository.Insert(Entity);

                            _st_contactRepository.Save();
                            // tblOppContactMapping insert
                            if (PageSource != "ContactDetails")
                            {
                                if (Entity.id > 0)
                                {
                                    // var COntact = _st_contactRepository.Get(_ => _.id == Entity.id).FirstOrDefault();
                                    tblName = TableNames.tblOppContactMapping;
                                    tblOppContactMapping ContactMapping = new tblOppContactMapping();
                                    ContactMapping.IsPrimary = MappingModel.isPrimary;
                                    ContactMapping.OpportunityId = MappingModel.MappingID;
                                    ContactMapping.ContactId = Entity.id;
                                    ContactMapping.CreatedBy = DataBaseCon.ActiveUser();
                                    ContactMapping.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _tblOppContactMappingRepository.Insert(ContactMapping);
                                    _tblOppContactMappingRepository.Save();
                                }
                            }
                            response.ID = Entity.id;
                            response.Message = ResponseMessage.SuccessMessage;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblContact;
                            break;
                        }
                    case BatchOperation.Delete:
                        {

                            var entity = _st_contactRepository.Get(_ => _.email == Entity.email).FirstOrDefault();
                            if (entity != null)
                            {
                                _st_contactRepository.Delete(entity);
                                _st_contactRepository.Save();
                            }
                            response.ID = entity.id;
                            response.Message = ResponseMessage.SuccessMessage;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblContact;
                            break;
                        }
                    default:
                        {
                            if (PageSource == "ContactDetails")
                            {
                                tblName = TableNames.tblContact;
                                var entity = _st_contactRepository.Get(_ => _.id == Entity.id).FirstOrDefault();
                                if (entity != null)
                                {
                                    entity.first_name = Entity.first_name;
                                    entity.last_name = Entity.last_name;
                                    entity.email = Entity.email;
                                    entity.ContactType = Entity.ContactType;
                                    if (Entity.mobile != null && Entity.mobile != "")
                                    {
                                        entity.mobile = Regex.Replace(Entity.mobile, @"[^0-9]", "");
                                    }
                                    entity.title = Entity.title;
                                    entity.ContactRole = Entity.ContactRole;

                                    entity.notes = Entity.notes;
                                    entity.acct_manager_id = Entity.acct_manager_id;
                                    if (PageSource == "OrganisationDetail")
                                    {
                                       entity.OrgId = Entity.OrgId;
                                    }
                                        entity.UpdatedBy = DataBaseCon.ActiveUser();
                                    entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _st_contactRepository.Update(entity);
                                    _st_contactRepository.Save();
                                }
                                response.ID = entity.id;
                                response.Message = ResponseMessage.SuccessMessage;
                                response.Result = ResponseType.Success;
                                response.tblName = TableNames.tblContact;
                            }
                            if (PageSource != "ContactDetails" && PageSource != "OrganisationDetail")
                            {
                                if (MappingModel.ContactID > 0)
                                {
                                    var ContactmappingForValidate = _tblOppContactMappingRepository.Get(_ => _.ContactId == MappingModel.ContactID && _.OpportunityId == MappingModel.MappingID).FirstOrDefault();
                                    if (ContactmappingForValidate == null)
                                    {
                                        tblName = TableNames.tblOppContactMapping;
                                        var COntact = _st_contactRepository.Get(_ => _.id == MappingModel.ContactID).FirstOrDefault();

                                        tblOppContactMapping ContactMapping = new tblOppContactMapping();

                                        ContactMapping.IsPrimary = MappingModel.isPrimary;

                                        ContactMapping.OpportunityId = MappingModel.MappingID;
                                        ContactMapping.ContactId = MappingModel.ContactID;
                                        ContactMapping.CreatedBy = DataBaseCon.ActiveUser();
                                        ContactMapping.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                        _tblOppContactMappingRepository.Insert(ContactMapping);

                                    }
                                    else
                                    {
                                        if (MappingModel.IsLinked)
                                        {
                                           
                                            ContactmappingForValidate.IsPrimary = MappingModel.isPrimary;
                                            ContactmappingForValidate.UpdatedBy = DataBaseCon.ActiveUser();
                                            ContactmappingForValidate.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                            _tblOppContactMappingRepository.Update(ContactmappingForValidate);
                                        }
                                        else
                                        {
                                            _tblOppContactMappingRepository.Delete(ContactmappingForValidate);
                                        }
                                    }
                                    _tblOppContactMappingRepository.Save();
                                    response.ID = MappingModel.ContactID;
                                    response.Message = ResponseMessage.SuccessMessage;
                                    response.Result = ResponseType.Success;
                                    response.tblName = TableNames.tblContact;
                                }
                            }
                            if(PageSource == "OrganisationDetail")
                            {
                                if (MappingModel.ContactID > 0)
                                {
                                    var ContactmappingForOrganisation = _st_contactRepository.Get(_ => _.id == MappingModel.ContactID).FirstOrDefault();
                                    if (ContactmappingForOrganisation != null)
                                    {
                                        if (MappingModel.IsLinked)
                                        {
                                            ContactmappingForOrganisation.OrgId = MappingModel.MappingID;
                                            ContactmappingForOrganisation.UpdatedBy = DataBaseCon.ActiveUser();
                                            ContactmappingForOrganisation.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                            _st_contactRepository.Update(ContactmappingForOrganisation);
                                           
                                        }
                                        else
                                        {
                                            ContactmappingForOrganisation.OrgId = null;
                                            ContactmappingForOrganisation.UpdatedBy = DataBaseCon.ActiveUser();
                                            ContactmappingForOrganisation.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                            _st_contactRepository.Update(ContactmappingForOrganisation);
                                        }

                                    }

                                    _st_contactRepository.Save();
                                   
                                }
                                response.ID = MappingModel.ContactID;
                                response.Message = ResponseMessage.SuccessMessage;
                                response.Result = ResponseType.Success;
                                response.tblName = TableNames.tblContact;
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.tblName = tblName;
                response.Result = ResponseType.Error;
                response.ErrorCode = ex.HResult;
            }

            return response;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblcontact> GetAll()
        {
            throw new NotImplementedException();
        }

        public tblcontact GetById(int id)
        {
            return _st_contactRepository.Get(_ => _.id == id).FirstOrDefault();
        }
        public Vw_tblContact GetContactById(int id)
        {
            return _VW_tblcontactRepository.Get(_ => _.id == id).FirstOrDefault();
        }
        public ResponseViewModel Update(tblcontact entity)
        {
            if (entity != null)
            {
                var EntityforValidate = _st_contactRepository.Get(_ => _.email == entity.email).FirstOrDefault();
                if (EntityforValidate != null)
                {
                    response = BatchTransaction(entity, BatchOperation.Update);
                }
                else
                {
                    response = BatchTransaction(entity, BatchOperation.Insert);
                }
            }
            return response;
        }

        ResponseViewModel IContactService.ValidateContact(tblcontact entity, OppContactMappingViewModel MappingModel,string PageSource)
        {
            var ID = entity.id;
            if (entity != null)
            {
                var EntityforValidate = _st_contactRepository.Get(_ => _.email == entity.email).FirstOrDefault();

                if (entity.id > 0)
                {
                    if (EntityforValidate != null && EntityforValidate.id != entity.id && PageSource=="ContactDetails")
                    {
                        response.Message = "Contact with this email already exists. click on open to change the contact details";
                        response.Result = ResponseType.Warning;
                            
                    }
                    else
                    {
                        response = ContactBatchTransaction(entity,MappingModel, PageSource, BatchOperation.Update);
                    }
                }
                else
                {
                    if (EntityforValidate != null)
                    {
                        response.Message = "Contact with this email already exists";
                        response.Result = ResponseType.Warning;
                    }
                    else
                    {
                        response = ContactBatchTransaction(entity, MappingModel, PageSource, BatchOperation.Insert);
                    }
                }
            }

            return response;
        }
        public List<ContactViewModel> GetLeadsContactTypeList(String Leads)
        {
            List<ContactViewModel> ContactList = new List<ContactViewModel>();
            try
            {
                if (Leads == "All")
                {
                    ContactList = Mapper.Map<List<ContactViewModel>>(_VW_tblcontactRepository.Get()).ToList();
                }
                else
                {
                    ContactList = Mapper.Map<List<ContactViewModel>>(_VW_tblcontactRepository.Get(_ => _.ContactType == Leads)).ToList();
                }
            }
            catch(Exception Ex)
            {

            }
            return ContactList;


        }

        public List<ContactViewModel> GetContactByOppId(int OppId)
        {
            List<ContactViewModel> ContactList = new List<ContactViewModel>();
            var ContactMapping = _tblOppContactMappingRepository.Get(_ => _.OpportunityId == OppId && _.IsPrimary == false).ToList();
            var PriContact = _tblOppContactMappingRepository.Get(_ => _.OpportunityId == OppId && _.IsPrimary == true).FirstOrDefault();
            ContactViewModel PrimaryContact = new ContactViewModel();
            if (PriContact != null)
            {
                PrimaryContact = Mapper.Map<ContactViewModel>(PriContact.tblcontact);
            }
            if (PrimaryContact.id!=0)
            {
                PrimaryContact.IsPrimary = true;
                ContactList.Add(PrimaryContact);
            }
            foreach (var item in ContactMapping)
            {
                ContactList.Add(Mapper.Map<ContactViewModel>(item.tblcontact));
            }
            return ContactList;

        }

        public List<opportunityViewModel> GetOppoListByContactOrOrganisation(string Stage, int ID, string PageSource)
        {
            List<opportunityViewModel> griddata = new List<opportunityViewModel>();
            
            if (PageSource == "OrganisationDetail")
            {

                if (Stage == "All")
                {
                    var data = _Vw_tblOpportunityRepository.Get(_ => _.OrgId == ID);
                    griddata = new List<opportunityViewModel>();
                    griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();

                }
                else
                {
                    if (Stage == "Lost")
                    {
                        var data = _Vw_tblOpportunityRepository.Get(_ => _.OrgId == ID && _.Lost == "Yes");
                        griddata = new List<opportunityViewModel>();
                        griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                    }
                    else if (Stage == "Declined")
                    {
                        var data = _Vw_tblOpportunityRepository.Get(_ => _.OrgId == ID && _.Declined == "Yes");
                        griddata = new List<opportunityViewModel>();
                        griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                    }
                    else if (Stage == "Cancelled")
                    {
                        var data = _Vw_tblOpportunityRepository.Get(_ => _.OrgId == ID && _.Cancelled == "Yes");
                        griddata = new List<opportunityViewModel>();
                        griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                    }
                    else
                    {

                        var data = _Vw_tblOpportunityRepository.Get(_ => _.OrgId == ID && _.Stage == Stage);
                        griddata = new List<opportunityViewModel>();
                        griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                    }
                }
            }
            else if (PageSource == "ContactDetails")
            {
                if (Stage == "All")
                {
                    var data = _Vw_tblOpportunityRepository.Get(_ => _.ContactID == ID);
                    griddata = new List<opportunityViewModel>();
                    griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();

                }
                else
                {
                    if (Stage == "Lost")
                    {
                        var data = _Vw_tblOpportunityRepository.Get(_ => _.ContactID == ID && _.Lost == "Yes");
                        griddata = new List<opportunityViewModel>();
                        griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                    }
                    else if (Stage == "Declined")
                    {
                        var data = _Vw_tblOpportunityRepository.Get(_ => _.ContactID == ID && _.Declined == "Yes");
                        griddata = new List<opportunityViewModel>();
                        griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                    }
                    else if (Stage == "Cancelled")
                    {
                        var data = _Vw_tblOpportunityRepository.Get(_ => _.ContactID == ID && _.Cancelled == "Yes");
                        griddata = new List<opportunityViewModel>();
                        griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                    }
                    else
                    {
                        var data = _Vw_tblOpportunityRepository.Get(_ => _.ContactID == ID && _.Stage == Stage);
                        griddata = new List<opportunityViewModel>();
                        griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                    }
                }
            }


            return griddata;
        }

        public List<OptionViewModel> getContactGSTGrid(int id, string Status)
        {
            List<OptionViewModel> gridValue = new List<OptionViewModel>();
            var data = _tblOptionRepository.Get(_ => _.OpportunityId == id && _.OptionStage==Status);
            gridValue = Mapper.Map<List<OptionViewModel>>(data);
            return gridValue;
        }
        public List<tblcontact> GetContactByOrgId(int OrgId)
        {
            return _st_contactRepository.Get(_ => _.OrgId == OrgId).ToList();
        }
        // baans change 23rd August for Change in ConfirmationMessage
        public bool CheckOrgByOppoID(int OpportunityID)
        {
            bool resultOrg = false;
            var data = _tblOppContactMappingRepository.Get(_ => _.OpportunityId == OpportunityID && _.IsPrimary == true).FirstOrDefault();
            if (data != null)
            {
                if (data.tblcontact.OrgId != null && data.tblcontact.OrgId != 0)
                    resultOrg = true;
            }
            return resultOrg;
        }

        public bool CheckPrimaryContactByOppoID(int OpportunityID)
        {
            bool Result = true;
            var data = _tblOppContactMappingRepository.Get(_ => _.OpportunityId == OpportunityID && _.IsPrimary == true).FirstOrDefault();
            if (data == null)
            {
                Result = false;
            }
            return Result;
        }
        public bool CheckMandatoryFieldsInOppo(int OpportunityID)
        {
            bool Result = true;
            var data = _tblOpportunityRepository.Get(_ => _.OpportunityId == OpportunityID).FirstOrDefault();
            if (data.OppName == null || data.Quantity == null || data.DepartmentName == null || data.ReqDate == null || data.DepositReqDate == null || data.Shipping == null || data.ShippingTo == null || data.Price == null || data.Source == null || data.AcctManagerId == null)
            {
                Result = false;
            }
            return Result;
        }
        // baans end 23rd August
        public List<ContactViewModel> GetCustomContactList(string CustomText, string TableName)
        {
            //  var ddd = DbContext.Pro_Search(TableName, CustomText);
            var CustomContact = Mapper.Map<List<ContactViewModel>>(DbContext.Database.SqlQuery<Vw_tblContact>("exec Pro_Search '" + TableName + "','" + CustomText + "'").ToList()).ToList();
            //  DbContext.Pro_Search
            return CustomContact;
        }
        public bool ValidateMapping(OppContactMappingViewModel MappingModel)
        {
            var flag = true;
           
                if (MappingModel.isPrimary)
                {
                    var IsPrimaryExist = _tblOppContactMappingRepository.Get(_ => _.OpportunityId == MappingModel.MappingID && _.IsPrimary == true).FirstOrDefault();
                    if (IsPrimaryExist != null)
                    {
                        if (IsPrimaryExist.ContactId != MappingModel.ContactID)
                        {
                            flag = false;
                        }
                    }
                }
            return flag;
        }
        // baans change 27th September for Autocomplete by Type
        public IEnumerable<DropDownViewModel> GetByEmailPrefix(string Prefix, string ContType)
        {
            //23 Aug 2018 (N)
            var data = Mapper.Map<IEnumerable<ContactViewModel>>(_st_contactRepository.Get(_ => _.email.Contains(Prefix)));
            // baans end 27th September
            var newdata = data.Select(item => new DropDownViewModel
            {
                id = item.id,
                first_name = item.first_name,
                last_name = item.last_name,
                email = item.email,
                Company = item.company,
                title = item.title,
                notes = item.notes,
                mobile = item.mobile,
                ContactType = item.ContactType,
                acct_manager_id = item.acct_manager_id,
                ContactRole = item.ContactRole,
                OrgId = item.OrgId,
                OrgName = item.OrgName
                //23 Aug 2018 (N)



            }
           ).OrderBy(_ => _.last_name).ToList();
            return newdata;

        }
        public ResponseViewModel DeleteContact(int Id)
        {
            KEN_DataAccess.tblcontact Entity = new KEN_DataAccess.tblcontact();
            var IsContact = _st_contactRepository.Get(_ => _.id == Id).FirstOrDefault();
            if (IsContact != null)
            {
                _st_contactRepository.Delete(IsContact);
                _st_contactRepository.Save();
                response.Message = "Data Deleted Successfully";
                response.Result = ResponseType.Success;
            }
            return response;
        }

    }
}