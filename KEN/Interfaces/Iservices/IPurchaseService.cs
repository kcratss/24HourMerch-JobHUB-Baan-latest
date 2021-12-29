using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KEN_DataAccess;
using KEN.Models;
using System.Net.Mail;
using iTextSharp.text;

namespace KEN.Interfaces.Iservices
{
    public interface IPurchaseService:IServiceBase<tblPurchase>
    {
        List<PurchaseDetailViewModel> GetPurchaseDetailOptionGrid(int PurchaseId);
        tblPurchase GetPurchById(int Id);
        Vw_tblOpportunity GetPurchEmailById(int OppId);
        ResponseViewModel OptionBatchTransaction(tblPurchaseDetail Entity, BatchOperation operation);
        ResponseViewModel PurchaseBatchTransaction(tblPurchase Entity, BatchOperation operation);

        //List<PurchaseDetailViewModel> GetOptionToPurchaseDetail(int id, int OppId);
        //void GetOptionToPurchaseDetail(int PurchaseId, int OppId);


        //tblPurchase GetSupplierByPurchaseId(int PurchaseId);    //tarun 07/09/2018

        //8 Sep 2018(N)
        List<PurchaseViewModel> GetPurchaseList(string PurchaseTabs, string DateFrom, string DateTo, string Supplier);

        IEnumerable<OrganisationViewModel> GetSupplierName();
        
        List<PurchaseViewModel> GetCustomPurchaseList(string CustomText, string TableName);
        //8 Sep 2018(N)
        EmailContentViewModel GetPurchaseEmailContent();
        tblPurchase GetPurchaseByOpportunityId(int OpportunityId);
        ResponseViewModel SetPurchaseStatus(int PurchaseId);
    }
}
