//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gyldendal.Api.CoreData.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class CampaignItem
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string VareId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public Nullable<int> sequence { get; set; }
    
        public virtual Campaign Campaign { get; set; }
        public virtual varer varer { get; set; }
    }
}