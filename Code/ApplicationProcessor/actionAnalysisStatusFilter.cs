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
    
    public partial class actionAnalysisStatusFilter
    {
        public System.Guid actionAnalysisStatusFilterId { get; set; }
        public System.Guid actionEventId { get; set; }
        public System.Guid creditAnalysisStatusId { get; set; }
    
        public virtual actionEvent actionEvent { get; set; }
        public virtual creditAnalysisStatu creditAnalysisStatu { get; set; }
    }
}
