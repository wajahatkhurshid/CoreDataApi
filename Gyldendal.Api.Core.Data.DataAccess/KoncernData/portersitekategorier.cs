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
    
    public partial class portersitekategorier
    {
        public int id { get; set; }
        public string navn { get; set; }
        public string sti { get; set; }
        public string area { get; set; }
        public string subject { get; set; }
        public string subarea { get; set; }
        public Nullable<System.DateTime> opdateret { get; set; }
        public string website { get; set; }
        public string Varer_id { get; set; }
    
        public virtual varer varer { get; set; }
    }
}