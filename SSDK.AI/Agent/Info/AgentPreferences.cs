using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts what the agent is attempting to achieve
    /// </summary>
    [Flags]
    public enum AgentPreferences
    {
        /// <summary>
        /// No information is provided about the preferences, or unknown.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Depicts that the agent is trying to achieve a single state (noted as
        /// the goal)
        /// </summary>
        AchievementGoal = 1,
        /// <summary>
        /// Depicts that the agent has complex preferences, which may involve
        /// two or more goals.
        /// </summary>
        ComplexPreferences = 2,
        /// <summary>
        /// Depicts that the complex preferences of this agent is ordered.
        /// (i.e. what must be achieved first)
        /// </summary>
        ComplexOrdinalPreferences = 4,
        /// <summary>
        /// Depicts that absolute values relating to complex preferences are
        /// important to this agent.
        /// </summary>
        ComplexCardinalPreferences = 8,
    }
}
