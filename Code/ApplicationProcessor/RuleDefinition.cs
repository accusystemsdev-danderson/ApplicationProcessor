//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    /// <summary>
    /// Provides the definition for a data rule
    /// </summary>
    public class RuleDefinition
    {
        /// <summary>
        /// Gets or sets the rule's processing field
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the rule's comparison operator
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the rule's comparison value
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// Gets or sets the rule's action to take
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the rule's Parameter1 value
        /// </summary>
        public string Parameter1 { get; set; }

        /// <summary>
        /// Gets or sets the rule's Parameter2 value
        /// </summary>
        public string Parameter2 { get; set; }

        /// <summary>
        /// Gets or sets the rule's Parameter3 value
        /// </summary>
        public string Parameter3 { get; set; }
    }
}
