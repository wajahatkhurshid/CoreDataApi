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
    
    public partial class DEA_KDWS_MUNKproductcontributors
    {
        public string vare_id { get; set; }
        public string contributor_id { get; set; }
        public int role_id { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public int firmpers_stamdata_sortorder { get; set; }
    
        public virtual DEA_KDWS_MUNKContributors DEA_KDWS_MUNKContributors { get; set; }
        public virtual DEA_KDWS_MUNKroles DEA_KDWS_MUNKroles { get; set; }
        public virtual DEA_KDWS_MUNKproduct DEA_KDWS_MUNKproduct { get; set; }
    }
}
