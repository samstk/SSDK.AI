using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts information about the modularity of the agent, which is the
    /// extent in which the system can be decomposed into interacting modules.
    /// </summary>
    public enum AgentModularity
    {
        /// <summary>
        /// No information is provided about the modularity, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// The model has only one level of abstraction
        /// </summary>
        Flat,
        /// <summary>
        /// The model has multiple modules that is understood seperately.
        /// </summary>
        Modular,
        /// <summary>
        /// The model is recursively decomposed into seperate modules.
        /// </summary>
        Hierarchical
    }
}
