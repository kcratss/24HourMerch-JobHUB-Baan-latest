using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Models;
using System.Net.Mail;
using iTextSharp.text;

namespace KEN.Interfaces.Iservices
{
    public interface IOpportunityService:IServiceBase<tblOpportunity>
    {
        ResponseViewModel OppBatchTransaction(tblOpportunity Entity,string PageSource, BatchOperation operation);
        List<OptionViewModel> GetOptionGrid( int OpportunityID,string Status);
        List<DecorationViewModel> GetDecorationList();
        List<DecorationCostViewModel> GetDecorationCost(string DecorationDesc);
        IEnumerable<DecorationCostViewModel> GetDecorationCostByQty(string Prefix, string Decoration);
        IEnumerable<DecorationViewModel> GetDecorationByDesc(string Prefix);
        //List<opportunityViewModel> GetOpportunityData(string oppt);
        List<opportunityViewModel> GetOpportunityData(string oppt, string startDate, string EndDate, int UserProfile);
        ResponseViewModel OptionBatchTransaction(tbloption Entity, BatchOperation operation);
        //13 July 2019 (N)
        //List<st_DecorationViewModel> GetDecorationImagesList(string keyword);
        List<ApplicationViewModel> GetDecorationImagesList(string keyword);
        //13 July 2019 (N)
        StageChangeResponseViewModel ChangeStageByOppoID(int OppId, string Stage);
        StageChangeResponseViewModel ResetStageByOppoID(int OppId, string Stage);
        ResponseViewModel UploadOppImage(string filename, int OppId, string field, string path);
        ResponseViewModel UpdateOppInquiry(tblInquiry Entity);
        InquiryViewModel GetInquiryData(int OppId);

        //27 May 2019 (N)
        //ResponseViewModel SendEmail(EmailViewModel model, string path, int OpportunityId, string PathPdf);
        ResponseViewModel OpportunityContactMapping(OppContactMappingViewModel model);
        ResponseViewModel DeleteOppImage(InquiryImageDeleteViewModel model,string path);
        ImageResponseViewModel UploadOppThumbnail(string filename, int OppId, string path);
        EmailContentViewModel GetMailMessage(string OptionStatus);
        Vw_tblOpportunity GetOppById(int id);
        List<opportunityViewModel> GetCustomOppList(string CustomText, string TableName);
        //27 Aug 2018 (N)
        //List<PaymentViewModel> GetPaymentHistory(int id);
        List<Pro_HistoryOfPayments_Result> GetPaymentHistory(int id);
        //27 Aug 2018 (N)
        // baans change 13th Sept for Add New Brand
        ResponseViewModel SaveNewBrand(tblband Entity, string OptionBrand);
        // baans end 13th Sept

        ResponseViewModel SaveNewItem(string optionItem);
        ResponseViewModel UpdateOppPackin(OpportunityPackInViewModel model);
        bool GetPackinDetails(int OppId);
        bool GetShipDetails(int OppId);
        tblCommonData GetCommandDataForPaymentDescription();
        double TotalPaidBalance(int OpportunityID);
        List<Pro_PayHistory_Result> GetPaymentList(int OrgId);
        ResponseViewModel PaymentBatchTransaction(tblPayment Entity, PaymentViewModel model, BatchOperation operation);
        // baans change 22nd August for Getting AccountManagerName
        tbluser GetAccountManagerById(int Id);
        // baans end 22nd August
        //20 Aug 2018 (N)
        IEnumerable<OppoDropdownViewModel> GetOpportunityByOppName(string prefix);
        //20 Aug 2018 (N)

        //28 Aug 2018 (N)
        tblPurchase GetPurchaseData(int Id);
        List<PurchaseDetailViewModel> OptionDescForPurchase(int Id);
        //28 Aug 2018 (N)


        //Dheeraj Baans change 31 Aug start
        List<tblCommonData> AuthdataListByDesc();

        List<OptionViewModel> getOptionsForInvoice(int OpportunityID, string Status);
        bool UpdateTOken(string Token, string Type);
        //Dheeraj Baans change 31 Aug end

        //25 Sep 2018 (N)
        bool GetOptionStatus(int Oppid);
        //25 Sep 2018 (N)
        // baans change 23rd October for Deleting the option
        ResponseViewModel DeleteOption(int Id);
        // baans end 23rd October
        // baans change 03rd November for ChangeConfirmedDateOppoID
        ResponseViewModel ChangeConfirmedDateOppoID(int OppId, string ConfirmedDate);
        // baans end 03rd November
        bool OrgData(int OrgId);    //13 Nov 2018 (N)

        // Baans change 10th January for checking the status of Opportunity By OppId for make repeat Order
        bool StatusChkByOppIdForMakeRepeat(int OppId);
        ResponseViewModel MakeRepeatOrder(int OppId);
        // baans end 10th January
        // baans change 16th Jan for OptionCode
        List<OptionCodeBrandItemViewModel> GetOptionByPrefix(string Prefix);
        // baans end 16th Jan

        // 29 April NotesEditing List
        ResponseViewModel UpdateStatusNotes(int id, string stage, string notes);
        // 29 April NotesEditing List

        //29 April Stage Change List
        bool GetJobStatusByOppId(int OppId, string lblDate);
        //29 April Stage Change List
    }

}