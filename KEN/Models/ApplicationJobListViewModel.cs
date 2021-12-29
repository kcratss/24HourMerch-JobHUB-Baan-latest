using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class ApplicationJobListViewModel
    {
        public int OpportunityId { get; set; }
        public Nullable<System.DateTime> ConfirmedDate { get; set; }
        public string OppName { get; set; }
        public string Stage { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string JobManagerInitial { get; set; }
    }
}