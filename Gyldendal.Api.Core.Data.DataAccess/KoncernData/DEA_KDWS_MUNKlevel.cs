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
    
    public partial class DEA_KDWS_MUNKlevel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEA_KDWS_MUNKlevel()
        {
            this.DEA_KDWS_MUNKseriesLevel = new HashSet<DEA_KDWS_MUNKseriesLevel>();
            this.DEA_KDWS_MUNKProductLevels = new HashSet<DEA_KDWS_MUNKProductLevels>();
        }
    
        public int id { get; set; }
        public string navn { get; set; }
        public Nullable<int> niveau { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public Nullable<int> kategori_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_MUNKseriesLevel> DEA_KDWS_MUNKseriesLevel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_MUNKProductLevels> DEA_KDWS_MUNKProductLevels { get; set; }
        public virtual DEA_KDWS_MUNKcategory DEA_KDWS_MUNKcategory { get; set; }
    }
}
