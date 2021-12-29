using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class PantoneMasterViewModel
    {
        public int Id { get; set; }
        public string Pantone { get; set; }
        public string BucketId { get; set; }
        public string Hexvalue { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}