using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class PaymentViewModel
    {
        public int PmtId { get; set; }

        public Nullable<int> OpportunityId { get; set; }

        public Nullable<int> OrgId { get; set; }
        public Nullable<decimal> OppBalance { get; set; }
        public Nullable<System.DateTime> PmtDate { get; set; }
        public Nullable<decimal> AmtReceived { get; set; }
        public string PmtMethod { get; set; }
        public string MemoDesc { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string DepositAccountNo { get; set; }
        public string DepositAccountName { get; set; }
        public string OppName { get; set; }
     

        public string Stage { get; set; }
        //public string DisplayOpportunityId { get; set; }

        public string DisplayOpportunityId
        {
            get
            {
                string newId = "000000" + OpportunityId;
                return newId.Substring(newId.Length - 6, 6);
            }
        }

        public Nullable<DateTime> OppDate { get; set; }
    }
}