using SSDK.Core.Structures.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// An abstract class that provides the bare minimum on how an agent perceives the
    /// world.
    /// 
    /// Extended class must implement the Perceive and Predict function which returns a
    /// new problem space.
    /// </summary>
    public abstract class AgentProblemSpace
    {
        /// <summary>
        /// The match tolerance for equals on this problem space.
        /// </summary>
        public virtual double MatchTolerance { get; } = 0.0;

        /// <summary>
        /// Perceives the world that the agent lives in to the agent's perspective.
        /// If this problem space is used to represent the world in its entirely, then
        /// leave as empty function.
        /// </summary>
        public abstract void Perceive(Agent agent);

        /// <summary>
        /// Calculates the certainty distance between two problem states. May be used in some algorithms
        /// to determine desirable actions. 
        /// <br/>
        /// The DistanceTo function is not to be confused with the heuristic function, which
        /// is the estimated cost to achieve the space/state. The Distance function is more generally
        /// how many things must change for the space to be the exact same, and is used for
        /// state comparison (how certain this state is the exact same, 0 = same)
        /// </summary>
        /// <param name="space">the state to distantiate</param>
        /// <returns>the real number indicating a distance to the other state from this state</returns>
        public abstract double DistanceTo(AgentProblemSpace space);

        /// <summary>
        /// Estimates the distance between two problem states. May be used in some algorithms
        /// to determine desirable actions. 
        /// <br/>
        /// The Heuristic function is not to be confused with the DistanceTo function, which
        /// is the certainty distance between states. The heuristic function must provide
        /// a guide on the cost required to reach the desired space.
        /// </summary>
        /// <param name="space">the state to distantiate (the desired space)</param>
        /// <returns>
        /// the real number indicating an estimated distance to the other state
        /// from this state lower or equal to the actual distance
        /// </returns>
        public virtual UncontrolledNumber Heuristic(AgentProblemSpace space) { return 0.0; }

        /// <summary>
        /// Calculates the desirability of this problem space.
        /// </summary>
        /// <returns>
        /// a value between 0 - 100% (0.0-1.0) representing how desirable this state is.
        /// At 100%, the agent assumes that it achieved perfection on all goals.
        /// </returns>
        public virtual double Desirability(Agent agent)
        {
            if (agent.DesiredProblemSpace == null) return 1;
            return 1.0 / DistanceTo(agent.DesiredProblemSpace);
        }

        /// <summary>
        /// Predicts the effect of an operation on a specific agent, returning the new
        /// problem space.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public abstract AgentProblemSpace Predict(Agent agent, AgentOperation operation);

        public override int GetHashCode()
        {
            return 0; // To be overwritten
        }

        public override bool Equals(object? obj)
        {
            return obj is AgentProblemSpace && DistanceTo((AgentProblemSpace)obj) <= MatchTolerance;
        }
    }
}
