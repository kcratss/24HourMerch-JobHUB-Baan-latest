using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string DispalayId
        {
            get
            {
                string newId = "0000" + EventId;
                return newId.Substring(newId.Length - 4, 4);
            }
        }
        public string EventName { get; set; }
        public Nullable<System.DateTime> EventDate { get; set; }
        public string EventDate1 { get; set; }
        public string EventCycle { get; set; }
        public Nullable<System.DateTime> NextDate { get; set; }
        public string NextDate1 { get; set; }
        public string EventLocation { get; set; }
        public string EventWebsite { get; set; }
        public string EventNotes { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> JobId { get; set; }
        public int OpportunityId { get; set; }

        //23 Aug 2018 (N)
        public string PageSource { get; set; }
        //23 Aug 2018 (N)
    }
}