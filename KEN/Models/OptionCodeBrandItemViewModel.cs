using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class OptionCodeBrandItemViewModel
    {
        public int id { get; set; }
        public string Code { get; set; }
        public Nullable<int> itemId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string Link { get; set; }
        public Nullable<decimal> cost { get; set; }
        public string ItemName { get; set; }
        public string BrandName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Status { get; set; }
    }

}