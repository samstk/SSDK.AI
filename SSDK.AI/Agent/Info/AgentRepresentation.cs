using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts information about how an agent can reason
    /// </summary>
    public enum AgentRepresentation
    {
        /// <summary>
        /// No information is provided about the representation, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// Depicts that the agent reasons about explicit (unique) states, which
        /// is a certain arrangement of the environment.
        /// </summary>
        ExplicitStates,
        /// <summary>
        /// Depicts that the agent reasons about features or propositions (true/false)
        /// </summary>
        Features,
        /// <summary>
        /// Depicts that the agent reasons based on relations on individuals.
        /// </summary>
        IndividualsAndRelations
    }
}
