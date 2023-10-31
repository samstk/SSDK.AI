using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Depicts how far the agent looks into the future when deciding what to do.
    /// (may vary with solver)
    /// </summary>
    public enum AgentPlanningHorizon
    {
        /// <summary>
        /// No information is provided about the planning horizon, or unknown.
        /// </summary>
        Undefined,
        /// <summary>
        /// Depicts a horizon where the world does not change.
        /// </summary>
        Static,
        /// <summary>
        /// Depicts a horizon where the agent reasons from a fixed number of time steps.
        /// </summary>
        FiniteStage,
        /// <summary>
        /// Depicts a horizon where the agent reasons about a finite, 
        /// but not predetermined, number of time steps.
        /// </summary>
        IndefiniteStage,
        /// <summary>
        /// Depicts a horizon where the agent plans forever.
        /// </summary>
        InfiniteStage
    }
}
