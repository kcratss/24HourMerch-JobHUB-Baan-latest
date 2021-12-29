using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class InquiryViewModel
    {
        public int InquiryId { get; set; }
        public Nullable<int> OpportunityId { get; set; }
        public string ItemColours { get; set; }
        public string PrefBrandsAndStyle { get; set; }
        public string GeneralNotes { get; set; }
        public string FrontPrintArt { get; set; }
        public string FrontPrintNotes { get; set; }
        public string BackPrintArt { get; set; }
        public string BackPrintNotes { get; set; }
        public string LeftPrintArt { get; set; }
        public string LEftPrintNotes { get; set; }
        public string RighPrintArt { get; set; }
        public string RightPrintNotes { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}