﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KEN_DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class KENNEWEntities : DbContext
    {
        public KENNEWEntities()
            : base("name=KENNEWEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblEvent> tblEvents { get; set; }
        public virtual DbSet<tblOppContactMapping> tblOppContactMappings { get; set; }
        public virtual DbSet<tblDecorationColour> tblDecorationColours { get; set; }
        public virtual DbSet<tblInquiry> tblInquiries { get; set; }
        public virtual DbSet<tblband> tblbands { get; set; }
        public virtual DbSet<tblCampaign> tblCampaigns { get; set; }
        public virtual DbSet<tblcontact> tblcontacts { get; set; }
        public virtual DbSet<tbldecoration> tbldecorations { get; set; }
        public virtual DbSet<tbljob> tbljobs { get; set; }
        public virtual DbSet<tblkanban> tblkanbans { get; set; }
        public virtual DbSet<tbluser> tblusers { get; set; }
        public virtual DbSet<tblEmailContent> tblEmailContents { get; set; }
        public virtual DbSet<Vw_tblContact> Vw_tblContact { get; set; }
        public virtual DbSet<tblPayment> tblPayments { get; set; }
        public virtual DbSet<tblCommonData> tblCommonDatas { get; set; }
        public virtual DbSet<vw_tblKanban> vw_tblKanban { get; set; }
        public virtual DbSet<tbldepartment> tbldepartments { get; set; }
        public virtual DbSet<tblitem> tblitems { get; set; }
        public virtual DbSet<tblDecorationCost> tblDecorationCosts { get; set; }
        public virtual DbSet<tblOptionSize> tblOptionSizes { get; set; }
        public virtual DbSet<VW_GetHolidays> VW_GetHolidays { get; set; }
        public virtual DbSet<tblHoliday_dates> tblHoliday_dates { get; set; }
        public virtual DbSet<tblDecorationDesc> tblDecorationDescs { get; set; }
        public virtual DbSet<tblPurchase> tblPurchases { get; set; }
        public virtual DbSet<tblPurchaseDetail> tblPurchaseDetails { get; set; }
        public virtual DbSet<Vw_tblPurchase> Vw_tblPurchase { get; set; }
        public virtual DbSet<tblOrganisation> tblOrganisations { get; set; }
        public virtual DbSet<Vw_tblOrganisation> Vw_tblOrganisation { get; set; }
        public virtual DbSet<tblApplicationArt> tblApplicationArts { get; set; }
        public virtual DbSet<tblApplicationArtSuppplier> tblApplicationArtSupppliers { get; set; }
        public virtual DbSet<TblApplicationColour> TblApplicationColours { get; set; }
        public virtual DbSet<TblApplicationColoursMapping> TblApplicationColoursMappings { get; set; }
        public virtual DbSet<TblApplicationCustomInfo> TblApplicationCustomInfoes { get; set; }
        public virtual DbSet<tblApplicationCustomInfoMapping> tblApplicationCustomInfoMappings { get; set; }
        public virtual DbSet<tblApplicationDesigner> tblApplicationDesigners { get; set; }
        public virtual DbSet<tblApplicationStatu> tblApplicationStatus { get; set; }
        public virtual DbSet<tblApplicationType> tblApplicationTypes { get; set; }
        public virtual DbSet<tblPantoneMaster> tblPantoneMasters { get; set; }
        public virtual DbSet<TblApplication> TblApplications { get; set; }
        public virtual DbSet<tblAddress> tblAddresses { get; set; }
        public virtual DbSet<tbloption> tbloptions { get; set; }
        public virtual DbSet<tblOptionCode> tblOptionCodes { get; set; }
        public virtual DbSet<tblOpportunity> tblOpportunities { get; set; }
        public virtual DbSet<vw_tblopp1> vw_tblopp1 { get; set; }
        public virtual DbSet<Vw_tblOpportunity> Vw_tblOpportunity { get; set; }
    
        public virtual ObjectResult<Pro_DecorationExport_Result> Pro_DecorationExport()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_DecorationExport_Result>("Pro_DecorationExport");
        }
    
        public virtual ObjectResult<Nullable<decimal>> Pro_GetOppBalance(Nullable<int> oppid, string stageType)
        {
            var oppidParameter = oppid.HasValue ?
                new ObjectParameter("oppid", oppid) :
                new ObjectParameter("oppid", typeof(int));
    
            var stageTypeParameter = stageType != null ?
                new ObjectParameter("StageType", stageType) :
                new ObjectParameter("StageType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("Pro_GetOppBalance", oppidParameter, stageTypeParameter);
        }
    
        public virtual ObjectResult<Pro_HistoryOfPayments_Result> Pro_HistoryOfPayments(Nullable<int> oppid)
        {
            var oppidParameter = oppid.HasValue ?
                new ObjectParameter("oppid", oppid) :
                new ObjectParameter("oppid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_HistoryOfPayments_Result>("Pro_HistoryOfPayments", oppidParameter);
        }
    
        public virtual ObjectResult<Pro_PayHistory_Result> Pro_PayHistory(Nullable<int> orgId)
        {
            var orgIdParameter = orgId.HasValue ?
                new ObjectParameter("OrgId", orgId) :
                new ObjectParameter("OrgId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_PayHistory_Result>("Pro_PayHistory", orgIdParameter);
        }
    
        public virtual ObjectResult<Pro_QuoteOptionsDetail_Result> Pro_QuoteOptionsDetail(Nullable<int> oppd, string optionStage)
        {
            var oppdParameter = oppd.HasValue ?
                new ObjectParameter("oppd", oppd) :
                new ObjectParameter("oppd", typeof(int));
    
            var optionStageParameter = optionStage != null ?
                new ObjectParameter("OptionStage", optionStage) :
                new ObjectParameter("OptionStage", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_QuoteOptionsDetail_Result>("Pro_QuoteOptionsDetail", oppdParameter, optionStageParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> Pro_MakeRepeatOrder(Nullable<int> opportunityID)
        {
            var opportunityIDParameter = opportunityID.HasValue ?
                new ObjectParameter("OpportunityID", opportunityID) :
                new ObjectParameter("OpportunityID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("Pro_MakeRepeatOrder", opportunityIDParameter);
        }
    
        public virtual ObjectResult<Pro_ManagerStageWiseReport_Result> Pro_ManagerStageWiseReport(string fromDate, string todate, string source)
        {
            var fromDateParameter = fromDate != null ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(string));
    
            var todateParameter = todate != null ?
                new ObjectParameter("Todate", todate) :
                new ObjectParameter("Todate", typeof(string));
    
            var sourceParameter = source != null ?
                new ObjectParameter("Source", source) :
                new ObjectParameter("Source", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_ManagerStageWiseReport_Result>("Pro_ManagerStageWiseReport", fromDateParameter, todateParameter, sourceParameter);
        }
    
        public virtual ObjectResult<Pro_OppValueConversionReport_Result> Pro_OppValueConversionReport(string fromDate, string todate, string source)
        {
            var fromDateParameter = fromDate != null ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(string));
    
            var todateParameter = todate != null ?
                new ObjectParameter("Todate", todate) :
                new ObjectParameter("Todate", typeof(string));
    
            var sourceParameter = source != null ?
                new ObjectParameter("Source", source) :
                new ObjectParameter("Source", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_OppValueConversionReport_Result>("Pro_OppValueConversionReport", fromDateParameter, todateParameter, sourceParameter);
        }
    
        public virtual ObjectResult<Pro_ValueConversionReport_Result> Pro_ValueConversionReport(string fromDate, string todate, string source)
        {
            var fromDateParameter = fromDate != null ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(string));
    
            var todateParameter = todate != null ?
                new ObjectParameter("Todate", todate) :
                new ObjectParameter("Todate", typeof(string));
    
            var sourceParameter = source != null ?
                new ObjectParameter("Source", source) :
                new ObjectParameter("Source", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_ValueConversionReport_Result>("Pro_ValueConversionReport", fromDateParameter, todateParameter, sourceParameter);
        }
    
        public virtual int pro_optiontopurchasedetail(Nullable<int> oppid, Nullable<int> purchid)
        {
            var oppidParameter = oppid.HasValue ?
                new ObjectParameter("oppid", oppid) :
                new ObjectParameter("oppid", typeof(int));
    
            var purchidParameter = purchid.HasValue ?
                new ObjectParameter("purchid", purchid) :
                new ObjectParameter("purchid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("pro_optiontopurchasedetail", oppidParameter, purchidParameter);
        }
    
        public virtual ObjectResult<Pro_PurchaseData_Result> Pro_PurchaseData(Nullable<int> purchaseId)
        {
            var purchaseIdParameter = purchaseId.HasValue ?
                new ObjectParameter("PurchaseId", purchaseId) :
                new ObjectParameter("PurchaseId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_PurchaseData_Result>("Pro_PurchaseData", purchaseIdParameter);
        }
    
        public virtual ObjectResult<Pro_PurchaseOptionData_Result> Pro_PurchaseOptionData(Nullable<int> purchaseId)
        {
            var purchaseIdParameter = purchaseId.HasValue ?
                new ObjectParameter("PurchaseId", purchaseId) :
                new ObjectParameter("PurchaseId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_PurchaseOptionData_Result>("Pro_PurchaseOptionData", purchaseIdParameter);
        }
    
        public virtual ObjectResult<Pro_AppplicationJobsDetail_Result> Pro_AppplicationJobsDetail(Nullable<int> applicationId)
        {
            var applicationIdParameter = applicationId.HasValue ?
                new ObjectParameter("ApplicationId", applicationId) :
                new ObjectParameter("ApplicationId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_AppplicationJobsDetail_Result>("Pro_AppplicationJobsDetail", applicationIdParameter);
        }
    
        public virtual ObjectResult<Pro_ProofHeaderInfo_Result> Pro_ProofHeaderInfo(Nullable<int> oppId, Nullable<int> optionId, string optionStage)
        {
            var oppIdParameter = oppId.HasValue ?
                new ObjectParameter("OppId", oppId) :
                new ObjectParameter("OppId", typeof(int));
    
            var optionIdParameter = optionId.HasValue ?
                new ObjectParameter("OptionId", optionId) :
                new ObjectParameter("OptionId", typeof(int));
    
            var optionStageParameter = optionStage != null ?
                new ObjectParameter("OptionStage", optionStage) :
                new ObjectParameter("OptionStage", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_ProofHeaderInfo_Result>("Pro_ProofHeaderInfo", oppIdParameter, optionIdParameter, optionStageParameter);
        }
    
        public virtual ObjectResult<Pro_SalesReport_Result> Pro_SalesReport(string fromdate, string todate)
        {
            var fromdateParameter = fromdate != null ?
                new ObjectParameter("fromdate", fromdate) :
                new ObjectParameter("fromdate", typeof(string));
    
            var todateParameter = todate != null ?
                new ObjectParameter("todate", todate) :
                new ObjectParameter("todate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_SalesReport_Result>("Pro_SalesReport", fromdateParameter, todateParameter);
        }
    
        public virtual ObjectResult<Pro_PaymentReport_Result> Pro_PaymentReport(string reqdate)
        {
            var reqdateParameter = reqdate != null ?
                new ObjectParameter("reqdate", reqdate) :
                new ObjectParameter("reqdate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_PaymentReport_Result>("Pro_PaymentReport", reqdateParameter);
        }
    
        public virtual ObjectResult<Pro_PaymentReportNew_Result> Pro_PaymentReportNew(string reqdate)
        {
            var reqdateParameter = reqdate != null ?
                new ObjectParameter("reqdate", reqdate) :
                new ObjectParameter("reqdate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_PaymentReportNew_Result>("Pro_PaymentReportNew", reqdateParameter);
        }
    
        public virtual ObjectResult<Pro_InvoicedReport_Result> Pro_InvoicedReport(string fromdate, string todate)
        {
            var fromdateParameter = fromdate != null ?
                new ObjectParameter("fromdate", fromdate) :
                new ObjectParameter("fromdate", typeof(string));
    
            var todateParameter = todate != null ?
                new ObjectParameter("todate", todate) :
                new ObjectParameter("todate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_InvoicedReport_Result>("Pro_InvoicedReport", fromdateParameter, todateParameter);
        }
    
        public virtual ObjectResult<Pro_QuoteCustomerData_Result> Pro_QuoteCustomerData(Nullable<int> oppid)
        {
            var oppidParameter = oppid.HasValue ?
                new ObjectParameter("oppid", oppid) :
                new ObjectParameter("oppid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Pro_QuoteCustomerData_Result>("Pro_QuoteCustomerData", oppidParameter);
        }
    }
}