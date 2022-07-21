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
    
    public partial class tblUserItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUserItem()
        {
            this.tblDraftQuoteItems = new HashSet<tblDraftQuoteItem>();
            this.tblOrderDetails = new HashSet<tblOrderDetail>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ImageId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> BackLogoId { get; set; }
        public Nullable<double> FrontLogoWidth { get; set; }
        public Nullable<double> FrontLogoheight { get; set; }
        public Nullable<double> FrontLogoPositionTop { get; set; }
        public Nullable<double> FrontLogoPositionLeft { get; set; }
        public Nullable<double> BackLogoWidth { get; set; }
        public Nullable<double> BackLogoheight { get; set; }
        public Nullable<double> BackLogoPositionTop { get; set; }
        public Nullable<double> BackLogoPositionLeft { get; set; }
        public string FrontImageSource { get; set; }
        public Nullable<int> FrontLogoId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Status { get; set; }
        public string BackImageSource { get; set; }
        public Nullable<int> UserLogoProcess_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDraftQuoteItem> tblDraftQuoteItems { get; set; }
        public virtual tblOptionProperty tblOptionProperty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOrderDetail> tblOrderDetails { get; set; }
        public virtual tblUserLogo tblUserLogo { get; set; }
        public virtual tblUserLogo tblUserLogo1 { get; set; }
        public virtual tbluser tbluser { get; set; }
        public virtual tblUserLogoProcess tblUserLogoProcess { get; set; }
    }
}
