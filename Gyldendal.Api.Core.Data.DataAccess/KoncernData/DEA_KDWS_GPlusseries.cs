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
    
    public partial class DEA_KDWS_GPlusseries
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEA_KDWS_GPlusseries()
        {
            this.DEA_KDWS_GPlusproductseries = new HashSet<DEA_KDWS_GPlusproductseries>();
            this.DEA_KDWS_GPlusserieAreas = new HashSet<DEA_KDWS_GPlusserieAreas>();
            this.DEA_KDWS_GPlusSerieCategories = new HashSet<DEA_KDWS_GPlusSerieCategories>();
            this.DEA_KDWS_GPlusseriesLevel = new HashSet<DEA_KDWS_GPlusseriesLevel>();
            this.DEA_KDWS_GPlusserieSubAreas = new HashSet<DEA_KDWS_GPlusserieSubAreas>();
            this.DEA_KDWS_GPlusserieSubjects = new HashSet<DEA_KDWS_GPlusserieSubjects>();
            this.DEA_KDWS_GPlusseries1 = new HashSet<DEA_KDWS_GPlusseries>();
        }
    
        public int id { get; set; }
        public string navn { get; set; }
        public short niveau { get; set; }
        public string sti { get; set; }
        public Nullable<bool> internet { get; set; }
        public string webadresse { get; set; }
        public string webkoncept { get; set; }
        public string beskrivelse { get; set; }
        public Nullable<int> parent_id { get; set; }
        public string parent_sti { get; set; }
        public string forlag { get; set; }
        public string afdeling { get; set; }
        public bool Is_Image_Uploaded { get; set; }
        public string redaktion { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public decimal kd_slettet { get; set; }
        public System.DateTime kd_oprettet { get; set; }
        public decimal Type { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GPlusproductseries> DEA_KDWS_GPlusproductseries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GPlusserieAreas> DEA_KDWS_GPlusserieAreas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GPlusSerieCategories> DEA_KDWS_GPlusSerieCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GPlusseriesLevel> DEA_KDWS_GPlusseriesLevel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GPlusserieSubAreas> DEA_KDWS_GPlusserieSubAreas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GPlusserieSubjects> DEA_KDWS_GPlusserieSubjects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GPlusseries> DEA_KDWS_GPlusseries1 { get; set; }
        public virtual DEA_KDWS_GPlusseries DEA_KDWS_GPlusseries2 { get; set; }
    }
}
