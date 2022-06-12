//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gyldendal.Api.CoreData.DataAccess.KoncernData
{
    using System;
    using System.Collections.Generic;
    
    public partial class DEA_KDWS_GDKThemacode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEA_KDWS_GDKThemacode()
        {
            this.DEA_KDWS_GDKProductThemacode = new HashSet<DEA_KDWS_GDKProductThemacode>();
        }
    
        public int ThemaCodeId { get; set; }
        public string ThemaCode { get; set; }
        public string DanishDescription { get; set; }
        public string EnglishDescription { get; set; }
        public string ThemaType { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string WarningSuggestionMessage { get; set; }
        public string ToolTipMessage { get; set; }
        public Nullable<bool> HasChilds { get; set; }
        public Nullable<bool> kd_slettet { get; set; }
        public System.DateTime kd_oprettet { get; set; }
        public Nullable<System.DateTime> lastupdated { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GDKProductThemacode> DEA_KDWS_GDKProductThemacode { get; set; }
    }
}