using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class DecorationCostMasterViewModel
    {
        public int DecCostId { get; set; }
        public string Dec_Desc { get; set; }
        public string Quantity { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Status { get; set; }
    }
}