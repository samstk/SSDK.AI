using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts whether an agent can determine its resulting environment 
    /// state from its initial state and next action
    /// </summary>
    public enum AgentEffectUncertainty
    {
        /// <summary>
        /// No information is provided about effect uncertainty, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// Depicts that the agent can predict the exact state from any state
        /// and action.
        /// </summary>
        Deterministic,
        /// <summary>
        /// Depicts that the agent cannot predict the exact state from any state
        /// and action. 
        /// </summary>
        Stochastic
    }
}
