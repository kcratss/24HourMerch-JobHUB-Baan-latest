//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KEN_DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProductionCopy
    {
        public int id { get; set; }
        public string myob_ord { get; set; }
        public Nullable<System.DateTime> deposit_paid { get; set; }
        public int digital_art { get; set; }
        public int brief_to_artroom { get; set; }
        public Nullable<System.DateTime> proof_to_client { get; set; }
        public Nullable<System.DateTime> proof_approved { get; set; }
        public Nullable<System.DateTime> stock_ordered { get; set; }
        public int stock_in { get; set; }
        public int in_production { get; set; }
        public int packed { get; set; }
        public Nullable<System.DateTime> invoiced { get; set; }
        public int paid { get; set; }
        public string production_note { get; set; }
        public int is_active { get; set; }
        public int quote_id { get; set; }
        public Nullable<System.DateTime> conv_date_bk { get; set; }
        public Nullable<System.DateTime> conv_date { get; set; }
    }
}
