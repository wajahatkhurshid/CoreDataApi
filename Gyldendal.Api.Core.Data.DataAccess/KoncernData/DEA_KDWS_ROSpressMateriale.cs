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
    
    public partial class DEA_KDWS_ROSpressMateriale
    {
        public int materiale_id { get; set; }
        public string forfatter_id { get; set; }
        public string materiale_fotograf { get; set; }
        public string materiale_foto_link { get; set; }
        public string materiale_foto_thumbnail { get; set; }
    
        public virtual DEA_KDWS_ROSpressForfatter DEA_KDWS_ROSpressForfatter { get; set; }
    }
}
