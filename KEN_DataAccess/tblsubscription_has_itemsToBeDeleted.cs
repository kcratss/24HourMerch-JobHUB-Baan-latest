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
    
    public partial class tblsubscription_has_itemsToBeDeleted
    {
        public int id { get; set; }
        public int subscription_id { get; set; }
        public int item_id { get; set; }
        public string amount { get; set; }
        public string description { get; set; }
        public Nullable<double> value { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
}
