using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Models;

namespace KEN.Interfaces.Iservices
{
    public interface IContactService:IServiceBase<tblcontact>
    {
        // baans change 27th September for Autocomplete by Type
        IEnumerable<DropDownViewModel> GetByFirstPrefix(string Prefix, string ContType);
        IEnumerable<DropDownViewModel> GetByLastPrefix(string Prefix, string ContType);
        // baans end 27th September
       ResponseViewModel ValidateContact(tblcontact entity, OppContactMappingViewModel MappingModel, string PageSource);
        ResponseViewModel ContactBatchTransaction(tblcontact Entity, OppContactMappingViewModel MappingModel, string PageSource, BatchOperation operation);
        List<ContactViewModel> GetLeadsContactTypeList(String Leads);
        List<ContactViewModel> GetContactByOppId(int OppId);
        List<opportunityViewModel> GetOppoListByContactOrOrganisation(string Stage, int ContactID, string PageSource);

        List<OptionViewModel> getContactGSTGrid(int id, string Status);
        List<tblcontact> GetContactByOrgId(int OrgId);
        bool CheckOrgByOppoID(int OpportunityID);
        Vw_tblContact GetContactById(int id);
        List<ContactViewModel> GetCustomContactList(string CustomText, string TableName);
        bool ValidateMapping(OppContactMappingViewModel MappingModel);
        // baans change 27th September for Autocomplete by Type
        IEnumerable<DropDownViewModel> GetByEmailPrefix(string Prefix, string ContType);
        // baans end 27th September
        // Baans change 23rd August for Confirmation Message
        bool CheckPrimaryContactByOppoID(int OpportunityID);
        bool CheckMandatoryFieldsInOppo(int OpportunityID);
        // baans end 23rd August
        // baans change 4th October for delete the contact
        ResponseViewModel DeleteContact(int Id);
        // baans end 4th October
    }
}