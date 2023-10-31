using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts how the agent uses its available computation power to
    /// act within the world.
    /// </summary>
    public enum AgentComputationalLimits
    {
        /// <summary>
        /// No information is provided about the computational limits, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// Depicts that the agent has perfect rationality.
        /// That is, it will attempt to use any amount of computation without
        /// considerations for limits, in order to determine the best course of action.
        /// </summary>
        PerfectRationality,
        /// <summary>
        /// Depicts that the agent makes use of available computation and memory limitations
        /// in order to determine good decisions within these limits.
        /// </summary>
        BoundedRationality
    }
}
