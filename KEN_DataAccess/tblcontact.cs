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
    
    public partial class tblcontact
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblcontact()
        {
            this.tblOppContactMappings = new HashSet<tblOppContactMapping>();
        }
    
        public int id { get; set; }
        public Nullable<int> acct_manager_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public Nullable<short> send_email { get; set; }
        public string notes { get; set; }
        public string title { get; set; }
        public string company { get; set; }
        public string ContactType { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string ContactRole { get; set; }
        public Nullable<int> OrgId { get; set; }
        public Nullable<int> oldacctMgrId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOppContactMapping> tblOppContactMappings { get; set; }
        public virtual tbluser tbluser { get; set; }
        public virtual tblOrganisation tblOrganisation { get; set; }
        public virtual tblOrganisation tblOrganisation1 { get; set; }
    }
}