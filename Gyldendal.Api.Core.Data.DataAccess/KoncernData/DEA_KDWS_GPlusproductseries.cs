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
    
    public partial class DEA_KDWS_GPlusproductseries
    {
        public string vare { get; set; }
        public int serie { get; set; }
        public Nullable<int> sekvens { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public decimal kd_slettet { get; set; }
        public System.DateTime kd_oprettet { get; set; }
    
        public virtual DEA_KDWS_GPlusseries DEA_KDWS_GPlusseries { get; set; }
        public virtual DEA_KDWS_GPlusproduct DEA_KDWS_GPlusproduct { get; set; }
    }
}
