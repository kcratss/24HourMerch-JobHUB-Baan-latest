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
    
    public partial class tblEvent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblEvent()
        {
            this.tblOpportunities = new HashSet<tblOpportunity>();
        }
    
        public int EventId { get; set; }
        public string EventName { get; set; }
        public Nullable<System.DateTime> EventDate { get; set; }
        public string EventCycle { get; set; }
        public Nullable<System.DateTime> NextDate { get; set; }
        public string EventLocation { get; set; }
        public string EventWebsite { get; set; }
        public string EventNotes { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOpportunity> tblOpportunities { get; set; }
    }
}
