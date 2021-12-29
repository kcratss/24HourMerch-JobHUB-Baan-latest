using System.Collections.Generic;
using KEN_DataAccess;
using KEN.Models;

namespace KEN.Interfaces.Iservices
{
    public interface IOrganisationService:IServiceBase<tblOrganisation>
    {
        List<OrganisationViewModel> GetOrganisationList(string Type);
        ResponseViewModel OrganisationCheck(tblOrganisation entity, string PageSource, int ContactID, int PurchaseId);      //1 Sep 2018(N)
        ResponseViewModel UpdateAddress(tblAddress entity, string PageSource, int PurchaseId, int OppId); /*tarun 18-sept*/
        IEnumerable<OrganisationViewModel> GetOrganisationByPrefix(string Prefix, string OrgType);      // baans change 27th Sept
        tblOrganisation GetOrganisationById(int OrgId);
        ResponseViewModel OrgBatchTransaction(tblOrganisation Entity, string PageSource, int ContactID, BatchOperation operation, int PurchaseId);      //1 Sep 2018(N)
        //tarun 22/08/2018
        tblOrganisation GetOrgByType();
        //end
        //tarun 08/09/2018
        List<OrganisationViewModel> GetCustomOrganisationList(string CustomText, string TableName);
        //end

        ResponseViewModel AddIntuitID(int orgid, string IntutitId);     //27 Dec 2018 (N)
        tblAddress GetOrganisationAddress(int OrgId);
    }
}
