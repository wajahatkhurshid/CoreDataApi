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
    
    public partial class DEA_KDWS_MUNKSerieCategories
    {
        public int serie { get; set; }
        public int kategori { get; set; }
        public int kd_id { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public System.DateTime kd_oprettet { get; set; }
    
        public virtual DEA_KDWS_MUNKcategory DEA_KDWS_MUNKcategory { get; set; }
        public virtual DEA_KDWS_MUNKseries DEA_KDWS_MUNKseries { get; set; }
    }
}
