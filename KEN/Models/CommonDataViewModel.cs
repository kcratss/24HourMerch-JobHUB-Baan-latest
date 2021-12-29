using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class CommonDataViewModel
    {

        public int Id { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string FieldDescription { get; set; }
    }
}