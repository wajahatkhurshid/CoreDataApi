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
    
    public partial class DEA_KDWS_GDKProduct
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEA_KDWS_GDKProduct()
        {
            this.DEA_KDWS_GDKattachments = new HashSet<DEA_KDWS_GDKattachments>();
            this.DEA_KDWS_GDKBundle_Products = new HashSet<DEA_KDWS_GDKBundle_Products>();
            this.DEA_KDWS_GDKProductReviews = new HashSet<DEA_KDWS_GDKProductReviews>();
            this.DEA_KDWS_GDKproductcontributors = new HashSet<DEA_KDWS_GDKproductcontributors>();
            this.DEA_KDWS_GDKproductauthors = new HashSet<DEA_KDWS_GDKproductauthors>();
            this.DEA_KDWS_GDKproductseries = new HashSet<DEA_KDWS_GDKproductseries>();
            this.DEA_KDWS_GDKProductThemacode = new HashSet<DEA_KDWS_GDKProductThemacode>();
        }
    
        public string vare_id { get; set; }
        public Nullable<System.DateTime> updated { get; set; }
        public string kortbeskrivelse { get; set; }
        public string langbeskrivelse { get; set; }
        public Nullable<int> sider { get; set; }
        public string forside { get; set; }
        public string ISBN13 { get; set; }
        public string ostSBN { get; set; }
        public string titel { get; set; }
        public string undertitel { get; set; }
        public Nullable<int> udgave { get; set; }
        public string medietype { get; set; }
        public Nullable<System.DateTime> udgivelsesdato { get; set; }
        public string Oversaetter { get; set; }
        public string illustrator { get; set; }
        public string omslagsgrafiker { get; set; }
        public Nullable<decimal> vejledende_pris { get; set; }
        public string status { get; set; }
        public string ITunesLink { get; set; }
        public string AndroidLink { get; set; }
        public Nullable<int> work_id { get; set; }
        public string materialetype { get; set; }
        public string SEO_Text { get; set; }
        public Nullable<int> lagertal { get; set; }
        public string Website { get; set; }
        public string Url { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public string redaktion { get; set; }
        public Nullable<int> serier_id { get; set; }
        public string serier_name { get; set; }
        public Nullable<int> medietypeRank { get; set; }
        public Nullable<int> materialetypeRank { get; set; }
        public string Indlaeser { get; set; }
        public Nullable<decimal> pris_med_moms { get; set; }
        public string længde { get; set; }
        public string ErrorCode { get; set; }
        public string ParentISBN { get; set; }
        public Nullable<decimal> gennemsynseksemplar { get; set; }
        public string ReadingSamples { get; set; }
        public bool IsNextPrintRunPlanned { get; set; }
        public string spilletid { get; set; }
        public string PrintingNum { get; set; }
        public Nullable<double> Gross_weight { get; set; }
        public Nullable<double> Net_weight { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Width { get; set; }
        public Nullable<int> Thickness_depth { get; set; }
        public Nullable<System.DateTime> FirstPrintPublishDate { get; set; }
        public string Medarbejder { get; set; }
        public string Imprint { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GDKattachments> DEA_KDWS_GDKattachments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GDKBundle_Products> DEA_KDWS_GDKBundle_Products { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GDKProductReviews> DEA_KDWS_GDKProductReviews { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GDKproductcontributors> DEA_KDWS_GDKproductcontributors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GDKproductauthors> DEA_KDWS_GDKproductauthors { get; set; }
        public virtual DEA_KDWS_GDKWork DEA_KDWS_GDKWork { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GDKproductseries> DEA_KDWS_GDKproductseries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEA_KDWS_GDKProductThemacode> DEA_KDWS_GDKProductThemacode { get; set; }
    }
}