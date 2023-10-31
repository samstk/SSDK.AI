using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts how the agent responds to new environments.
    /// </summary>
    public enum AgentLearningType
    {
        /// <summary>
        /// No information is provided about the learning type, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// Depicts that the agent cannot learn in new environments, and 
        /// is mostly incompatible with new environments.
        /// </summary>
        KnowledgeIsGiven,
        /// <summary>
        /// Depicts that the agent is capable of learning in new environments.
        /// </summary>
        KnowledgeIsLearned
    }
}
