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
    
    public partial class DEA_KDWS_ROShierarki
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEA_KDWS_ROShierarki()
        {
            this.DEA_KDWS_ROSwork = new HashSet<DEA_KDWS_ROSwork>();
        }
    
        public string hierarki_id { get; set; }
        public string hierarki_navn { get; set; }
        public Nullable<int> hierarki_niveau { get; set; }
        public string hierarki_parent_hierarki_id { get; set; }
        public Nullable<int> hierarki_sort_order { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_ROSwork> DEA_KDWS_ROSwork { get; set; }
    }
}
