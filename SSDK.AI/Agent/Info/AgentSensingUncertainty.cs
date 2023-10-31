using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts whether an agent can determine its resulting environment 
    /// state from its inputs.
    /// </summary>
    public enum AgentSensingUncertainty
    {
        /// <summary>
        /// No information is provided about sensing uncertainty, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// Depicts that the agent can observe the state of the world.
        /// </summary>
        FullyObservable,
        /// <summary>
        /// Depicts that the agent knows there are a number of states
        /// from the inputs.
        /// </summary>
        PartiallyObservable
    }
}
