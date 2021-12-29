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
    
    public partial class tblOrganisation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblOrganisation()
        {
            this.tblcontacts = new HashSet<tblcontact>();
            this.tblcontacts1 = new HashSet<tblcontact>();
            this.tblPayments = new HashSet<tblPayment>();
            this.tblAddresses = new HashSet<tblAddress>();
        }
    
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string TradingName { get; set; }
        public string ABN { get; set; }
        public string MainPhone { get; set; }
        public string WebAddress { get; set; }
        public string Industry { get; set; }
        public string OrgNotes { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Brand { get; set; }
        public Nullable<int> AcctMgrId { get; set; }
        public string OrganisationType { get; set; }
        public Nullable<int> IntuitID { get; set; }
        public string EmailAddress { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblcontact> tblcontacts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblcontact> tblcontacts1 { get; set; }
        public virtual tbluser tbluser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPayment> tblPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAddress> tblAddresses { get; set; }
    }
}
