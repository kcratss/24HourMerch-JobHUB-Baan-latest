using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class OrderViewModal
    {
        public Nullable<int> OpportunityId { get; set; }
        public string DisplayOpportunityId { get; set; }
        //public string DispalayOpportunityId
        //{
        //    get
        //    {
        //        string newId = "000000" + OpportunityId;
        //        return newId.Substring(newId.Length - 6, 6);
        //    }
        //}
        public Nullable<DateTime> OppDate { get; set; }

        public string OppName { get; set; }

        public Nullable<int> Quantity { get; set; }

        

        public string DepartmentName { get; set; }

        public Nullable<DateTime> ReqDate { get; set; }

        public Nullable<DateTime> DepositReqDate { get; set; }

        public string mobile { get; set; }

        public string OrderNotes { get; set; }

        public Nullable<int> Value {
            get; set;
            //{
            //    if(Value==0)
            //    {
            //        return null;
            //    }
            //}

       }

        public string Decoration { get; set; }

        public Nullable<DateTime> DepositDate { get; set; }

        public Nullable<DateTime> DispDate { get; set; }

        public Nullable<DateTime> PlannedDate { get; set; }

        public string JobNotes { get; set; }
        public Nullable<int> Margin { get; set; }
        public string AccountManagerFirstName { get; set; }

        public string AccountManagerLastName { get; set; }


        public string AcctManager
        {
            get
            {
                return AccountManagerFirstName + " " + AccountManagerLastName;
            }
        }
        public string Stage { get; set; }
        // Nikhil change 17th November
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> GP { get; set; }
        public Nullable<DateTime> ConfirmedDate { get; set; }
        public Nullable<DateTime> ArtOrderedDate { get; set; }
        public Nullable<DateTime> ApprovedDate { get; set; }
        public Nullable<DateTime> ArtReadyDate { get; set; }
        public Nullable<DateTime> StockOrderedDate { get; set; }
        public Nullable<DateTime> ReceivedDate { get; set; }
        public Nullable<DateTime> Checkeddate { get; set; }

        // Nikhil end

        // Nikhil change 22nd November
        public string Source { get; set; }
        public string OppNotes { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> EventId { get; set; }
        public string Compaign { get; set; }
        public string PackingNotes { get; set; }
        public string InvoicingNotes { get; set; }
        public string ShippingNotes { get; set; }
        public string CompleteNotes { get; set; }
        public string RepeatJob { get; set; }
        public Nullable<int> RepeatJobId { get; set; }
        public string Shipping { get; set; }
        public string Price { get; set; }
        public string Service { get; set; }
        public Nullable<System.DateTime> ShippingDate { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        public string ProdLine { get; set; }
        public Nullable<System.DateTime> QuoteDate { get; set; }
        public Nullable<System.DateTime> Orderdate { get; set; }
        public Nullable<System.DateTime> JobDate { get; set; }
        public Nullable<System.DateTime> PackingDate { get; set; }
        public Nullable<System.DateTime> InvoicingDate { get; set; }
        public Nullable<System.DateTime> ShippedDate { get; set; }
        public Nullable<System.DateTime> CompleteDate { get; set; }
        public Nullable<System.DateTime> StockInDate { get; set; }
        public Nullable<System.DateTime> DecoratedDate { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public string Declined { get; set; }
        public string Lost { get; set; }
        public string Cancelled { get; set; }
        public string ShippingTo { get; set; }
        public string job_department { get; set; }
        public Nullable<int> OldJob_id { get; set; }
        public Nullable<double> OldJob_value { get; set; }
        public Nullable<bool> oldsend_quote { get; set; }
        public string oldQty_Range { get; set; }
        public string Oldmyob_ord { get; set; }
        public Nullable<bool> Olddigital_art { get; set; }
        public Nullable<bool> Oldbrief_to_artroom { get; set; }
        public Nullable<System.DateTime> Oldproof_to_client { get; set; }
        public Nullable<bool> oldstock_in { get; set; }
        public Nullable<bool> oldIn_production { get; set; }
        public Nullable<bool> old_packed { get; set; }
        public Nullable<bool> old_paid { get; set; }
        public string OldProd_notes { get; set; }
        public Nullable<bool> OldIs_active { get; set; }
        public Nullable<System.DateTime> Oldconv_date { get; set; }
        public Nullable<System.DateTime> Oldconv_date_bk { get; set; }
        public string OppThumbnail { get; set; }
        public string QuoteMail { get; set; }
        public Nullable<int> AcctManagerId { get; set; }
        public Nullable<decimal> GSTValue { get; set; }
        public Nullable<System.DateTime> QuoteMailDate { get; set; }
        public Nullable<System.DateTime> OrderMailDate { get; set; }
        public string Complete { get; set; }
        public Nullable<System.DateTime> OrderConfirmedDate { get; set; }
        public string PackedInSet1 { get; set; }
        public string PackedInSet2 { get; set; }
        public string ConsigNoteNo { get; set; }
        public string PacagingNotes { get; set; }
        public Nullable<int> AddressId { get; set; }
        public string StageWiseNotes { get; set; }
        public Nullable<System.DateTime> StageWiseDate { get; set; }
        public Nullable<int> ContactID { get; set; }
        public string email { get; set; }
        public Nullable<int> OrgId { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string Contactfullname { get; set; }
        public string EventName { get; set; }
        public string AccountManagerFullName { get; set; }
        public string AccountManagerName { get; set; }
        //Nikhil End
        public Nullable<decimal> TotalBalance { get; set; }
        public int Totalpayment { get; set; }

        public Nullable<System.DateTime> JobAcceptedDate { get; set; }
        public Nullable<System.DateTime> ProofCreatedDate { get; set; }
        public Nullable<System.DateTime> ProofSentdate { get; set; }
    }
}