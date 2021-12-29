using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class PurchaseViewModel
    {
        public int PurchaseId { get; set; }
        public Nullable<int> OpportunityId { get; set; }
        public Nullable<System.DateTime> Purchasedate { get; set; }
        public Nullable<System.DateTime> DispatchDate { get; set; }
        public string PurchStatus { get; set; }
        public Nullable<System.DateTime> PlannedDate { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> OrgId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string PurchaseNotes { get; set; }
        public string Depts { get; set; }
        public Nullable<int> QuantityRequired { get; set; }
        public Nullable<System.DateTime> RequiredByDate { get; set; }
        public string ShippingIn { get; set; }
        public Nullable<decimal> ShippingCharge { get; set; }
        public string BillNo { get; set; }
        public Nullable<System.DateTime> BillDate { get; set; }
        public Nullable<int> DeliveryToId { get; set; }
        public string DisplayPurchaseId { get; set; }
        public string OppName { get; set; }
        public string OrgName { get; set; }
        public Nullable<decimal> AmountTotal { get; set; }
        public string WebOrderNo { get; set; }
        public string AccountManagerFullName { get; set; }
        public Nullable<System.DateTime> PurchaseMailDate { get; set; }
    }
}