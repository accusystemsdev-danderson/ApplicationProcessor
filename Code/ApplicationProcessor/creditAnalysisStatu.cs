//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using System;
    using System.Collections.Generic;
    
    public partial class creditAnalysisStatu
    {
        public creditAnalysisStatu()
        {
            this.actionAnalysisStatusFilters = new HashSet<actionAnalysisStatusFilter>();
        }
    
        public System.Guid creditAnalysisStatusId { get; set; }
        public string creditAnalysisStatusDescription { get; set; }
        public string creditAnalysisStatusCode { get; set; }
        public bool isDefault { get; set; }
        public bool isActiveAnalysisStatus { get; set; }
    
        public virtual ICollection<actionAnalysisStatusFilter> actionAnalysisStatusFilters { get; set; }
    }
}
