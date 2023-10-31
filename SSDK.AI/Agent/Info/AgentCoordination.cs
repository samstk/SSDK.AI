using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts how the agent co-ordinates with other agents.
    /// </summary>
    public enum AgentCoordination
    {
        /// <summary>
        /// No information is provided about the co-ordination, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// Depicts that the agent does not have any knowledge of other agents, and
        /// does not need to collaborate with them.
        /// </summary>
        SingleAgent,
        /// <summary>
        /// Depicts that the agent needs to reason strategically around other agents.
        /// </summary>
        MultipleAgents,
    }
}
