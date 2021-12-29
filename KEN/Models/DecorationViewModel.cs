using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KEN.Models
{
    public class DecorationViewModel
    {
        public string Dec_Desc { get; set; }
        
    }
    public class DecorationCostViewModel
    {
        public string Quantity { get; set; }
        public Nullable<decimal> Cost { get; set; }
    }

    //public class st_DecorationViewModel
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public int sort { get; set; }
    //    public string CreatedBy { get; set; }
    //    public Nullable<System.DateTime> CreatedOn { get; set; }
    //    public string UpdatedBy { get; set; }
    //    public Nullable<System.DateTime> UpdatedOn { get; set; }
    //    public Nullable<System.DateTime> DecDate { get; set; }
    //    public string Type { get; set; }
    //    public string Garment { get; set; }
    //    public string Position { get; set; }
    //    public string Measurement { get; set; }
    //    public Nullable<int> Widthsize { get; set; }
    //    public Nullable<int> Lengthsize { get; set; }
    //    public string Designer { get; set; }
    //    public string AcctMgr { get; set; }
    //    public string DecStatus { get; set; }
    //    public string Invoiced { get; set; }
    //    public string DecNotes { get; set; }
    //    public string Image { get; set; }
    //}
}