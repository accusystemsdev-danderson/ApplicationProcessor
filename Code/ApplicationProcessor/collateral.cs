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
    
    public partial class collateral
    {
        public System.Guid parentLoanId { get; set; }
        public System.Guid collateralLoanId { get; set; }
        public System.Guid rowguid { get; set; }
        public string statusOverrideYN { get; set; }
        public string ignoreExceptionsOverrideYN { get; set; }
        public Nullable<int> collateralSequence { get; set; }
    
        public virtual loan loan { get; set; }
        public virtual loan loan1 { get; set; }
    }
}
