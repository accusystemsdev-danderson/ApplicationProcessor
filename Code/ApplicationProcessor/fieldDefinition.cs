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
    
    public partial class fieldDefinition
    {
        public System.Guid fieldDefId { get; set; }
        public string fieldDefName { get; set; }
        public string fieldDefLabel { get; set; }
        public bool fieldDefIsCollateral { get; set; }
        public bool fieldDefIsSearchable { get; set; }
        public bool fieldDefIsDisplayable { get; set; }
        public int fieldDefDisplayOrder { get; set; }
        public string fieldDefDataType { get; set; }
        public int fieldDefDataSize { get; set; }
        public int fieldDefDataPrecision { get; set; }
        public string fieldDefChoiceList { get; set; }
        public string fieldDefChoiceDefaultValue { get; set; }
        public System.Guid fieldDefGroupId { get; set; }
        public Nullable<System.Guid> accountClassId { get; set; }
    
        public virtual accountClass accountClass { get; set; }
        public virtual fieldDefGroup fieldDefGroup { get; set; }
    }
}
