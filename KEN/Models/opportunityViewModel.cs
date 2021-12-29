using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class opportunityViewModel
    {
        public int OpportunityId { get; set; }
        public string DisplayOpportunityId { get; set; }
        //public string DispalayOpportunityId { get; set; }
        //public string DispalayOpportunityId
        //{
        //    get
        //    {
        //        string newId = "000000" + OpportunityId;
        //        return newId.Substring(newId.Length - 6, 6);
        //    }
        //}
        public string OppName { get; set; }
        public Nullable<DateTime> OppDate { get; set; }
        public Nullable<DateTime> StageWiseDate { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<System.DateTime> ReqDate { get; set; }
        public string Source { get; set; }

        public string Notes { get; set; }
        public string OppNotes { get; set; }
        // baans change 24th October for StageWiseNotes
        public string StageWiseNotes { get; set; }
            // baans end 24th October
        public Nullable<int> EventId { get; set; }
        public string Compaign { get; set; }
        public Nullable<int> AcctMgr { get; set; }
        public string Stage { get; set; }
        public string QuoteNotes { get; set; }
        public string OrderNotes { get; set; }
        public string JobNotes { get; set; }
        public string PackingNotes { get; set; }
        public string InvoicingNotes { get; set; }
        public string ShippingNotes { get; set; }
        public string CompleteNotes { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<System.DateTime> DepositReqDate { get; set; }
        public string Shipping { get; set; }
        public string Price { get; set; }
        public string Service { get; set; }
        public string firstname { get; set; }
        public string Contactfullname { get; set; }
        //tarun 18th Sept
        public Nullable<int> AddressId { get; set; }
        // end
        public string email { get; set; }
        //{
        //    get
        //    {
        //        return firstname + " " + lastname;
        //    }
        //}
        public string lastname { get; set; }

        public string AccountManagerFirstName { get; set; }

        public string AccountManagerLastName { get; set; }

        public string AccountManagerFullName { get; set; }
        //{
        //    get
        //    {
        //        return AccountManagerFirstName + " " + AccountManagerLastName;
        //    }
        //}
        public string AccountManagerName { get; set; }
        //{
        //    get
        //    {
        //        string Name = (AccountManagerFirstName == null ? "" : AccountManagerFirstName.Substring(0, 1)) + (AccountManagerLastName == null ? "" : AccountManagerLastName.Substring(0, 1));
        //        return Name;
        //    }
        //}

        public string EventName { get; set; }
        public Nullable<System.DateTime> QuoteDate { get; set; }
        public Nullable<System.DateTime> Orderdate { get; set; }
        public Nullable<System.DateTime> JobDate { get; set; }
        public Nullable<System.DateTime> PackingDate { get; set; }
        public Nullable<System.DateTime> InvoicingDate { get; set; }
        public Nullable<System.DateTime> ShippedDate { get; set; }
        public Nullable<System.DateTime> CompleteDate { get; set; }
        public Nullable<System.DateTime> StockInDate { get; set; }
        public Nullable<System.DateTime> ConfirmedDate { get; set; }
        public Nullable<System.DateTime> ArtOrderedDate { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<System.DateTime> ArtReadyDate { get; set; }
        public Nullable<System.DateTime> StockOrderedDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> Checkeddate { get; set; }
        public Nullable<System.DateTime> DecoratedDate { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<System.DateTime> PlannedDate { get; set; }
        public string Declined { get; set; }
        public string Lost { get; set; }
        public string Cancelled { get; set; }
        public string ShippingTo { get; set; }
        public Nullable<int> RepeatJobId { get; set; }
        public string PageSource { get; set; }
        public string ProdLine { get; set; }
        public string OppThumbnail { get; set; }
        public string QuoteMail { get; set; }
        public Nullable<int> AcctManagerId { get; set; }
        public string job_department { get; set; }
        public Nullable<System.DateTime> OrderConfirmedDate { get; set; }
        public string PackedInSet1 { get; set; }
        public string PackedInSet2 { get; set; }
        public string ConsigNoteNo { get; set; }
        public string PacagingNotes { get; set; }
         //by prashant 14aug end
        public string OrgName { get; set; }

        public string OrgID { get; set; }

        public string DepositAccountNo { get; set; }
        //by prashant 14aug end

        //15 Nov 2018 (N)
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> GP { get; set; }
        //15 Nov 2018 (N)
        public Nullable<System.DateTime> ConfirmMailDate { get; set; }
        public Nullable<System.DateTime> InvoiceMailDate { get; set; }
        public string OutSourcerName { get; set; }
        public Nullable<System.DateTime> JobAcceptedDate { get; set; }
        public Nullable<System.DateTime> ProofCreatedDate { get; set; }
        public Nullable<System.DateTime> ProofSentdate { get; set; }
        public Nullable<decimal> TotalBalance { get; set; }
        public string DeliveryDate { get; set; }
    }
    public class OpportunityPackInViewModel
    {
        public int OpportunityId { get; set; }
        public string PackedInSet1 { get; set; }
        public string PackedInSet2 { get; set; }
        public string ConsigNoteNo { get; set; }
        public string PacagingNotes { get; set; }
    }

    public class opportunityDetailsViewModel
    {
        opportunityViewModel opportunity { get; set; }
        ContactViewModel contact { get; set; }
    }

    //20 Aug 2018 (N)
    public class OppoDropdownViewModel
    {
        public int OpportunityId { get; set; }
        public string OppName { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string ReqDate { get; set; }
        public string Source { get; set; }
        public string Compaign { get; set; }
        public Nullable<int> AcctMgr { get; set; }
        public string Stage { get; set; }
        public string job_department { get; set; }
        public string DepositReqDate { get; set; }
        public string Shipping { get; set; }
        public string Price { get; set; }
        public string Service { get; set; }
        public string OppNotes { get; set; }
        public string ShippingTo { get; set; }
        public string Declined { get; set; }
        public string Lost { get; set; }
        public string Cancelled { get; set; }
        public string AccountManagerFullName { get; set; }
        public Nullable<int> AcctManagerId { get; set; }
        public string Status { get; set; }
    }
    //20 Aug 2018 (N)
}