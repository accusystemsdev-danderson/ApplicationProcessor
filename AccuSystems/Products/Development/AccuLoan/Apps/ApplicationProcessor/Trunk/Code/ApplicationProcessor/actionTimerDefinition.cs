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
    
    public partial class actionTimerDefinition
    {
        public actionTimerDefinition()
        {
            this.actionEventTimers = new HashSet<actionEventTimer>();
            this.actionTimers = new HashSet<actionTimer>();
        }
    
        public System.Guid actionTimerDefId { get; set; }
        public string actionTimerDefName { get; set; }
        public Nullable<int> notificationPeriod { get; set; }
        public bool isDefaultApplicationTimer { get; set; }
        public string notificationEmail { get; set; }
    
        public virtual ICollection<actionEventTimer> actionEventTimers { get; set; }
        public virtual ICollection<actionTimer> actionTimers { get; set; }
    }
}
