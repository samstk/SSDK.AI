using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts when the agent must reason.
    /// </summary>
    public enum AgentInteractionTime
    {
        /// <summary>
        /// No information is provided about the agent's interaction time, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// Depicts that the agent must reason before acting within the environment.
        /// </summary>
        Offline,
        /// <summary>
        /// Depicts that the agent must reason while acting within the environment.
        /// </summary>
        Online
    }
}
