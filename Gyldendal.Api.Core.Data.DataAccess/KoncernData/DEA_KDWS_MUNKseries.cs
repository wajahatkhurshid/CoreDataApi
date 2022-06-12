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
    
    public partial class DEA_KDWS_MUNKseries
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEA_KDWS_MUNKseries()
        {
            this.DEA_KDWS_MUNKproductseries = new HashSet<DEA_KDWS_MUNKproductseries>();
            this.DEA_KDWS_MUNKSerieCategories = new HashSet<DEA_KDWS_MUNKSerieCategories>();
            this.DEA_KDWS_MUNKseriesLevel = new HashSet<DEA_KDWS_MUNKseriesLevel>();
            this.DEA_KDWS_MUNKseries1 = new HashSet<DEA_KDWS_MUNKseries>();
            this.DEA_KDWS_MUNKserieAreas = new HashSet<DEA_KDWS_MUNKserieAreas>();
            this.DEA_KDWS_MUNKserieSubAreas = new HashSet<DEA_KDWS_MUNKserieSubAreas>();
            this.DEA_KDWS_MUNKserieSubjects = new HashSet<DEA_KDWS_MUNKserieSubjects>();
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
        public virtual ICollection<DEA_KDWS_MUNKproductseries> DEA_KDWS_MUNKproductseries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_MUNKSerieCategories> DEA_KDWS_MUNKSerieCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_MUNKseriesLevel> DEA_KDWS_MUNKseriesLevel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_MUNKseries> DEA_KDWS_MUNKseries1 { get; set; }
        public virtual DEA_KDWS_MUNKseries DEA_KDWS_MUNKseries2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_MUNKserieAreas> DEA_KDWS_MUNKserieAreas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_MUNKserieSubAreas> DEA_KDWS_MUNKserieSubAreas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_MUNKserieSubjects> DEA_KDWS_MUNKserieSubjects { get; set; }
    }
}