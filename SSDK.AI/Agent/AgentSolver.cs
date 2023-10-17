using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// An agent solver depicts an algorithm for a tick of an agent's operation.
    /// It also provides methods on how to account for new problems. A new instance
    /// of a solver should be initiated for every agent to avoid preprocessing errors.
    /// </summary>
    public abstract class AgentSolver
    {
        /// <summary>
        /// The tolerance in which a state is said to be the same.
        /// If set to 0, then the distance from one state to another to be the same must be 0.
        /// </summary>
        public double MatchTolerance = 0.0;

        /// <summary>
        /// Updates the solver's memory to account for the new problem as per 
        /// agent.UpdateProblem.
        /// </summary>
        /// <param name="agent">the agent that needs solving</param>
        public abstract void UpdateProblem(Agent agent);

        /// <summary>
        /// Attempts to solve the agent's current problem.
        /// </summary>
        /// <param name="agent">the agent that needs solving</param>
        /// <returns>an operation on the agent, which should lead to the desired state</returns>
        public abstract AgentOperation Solve(Agent agent);

        /// <summary>
        /// Checks to make sure that the operation will lead to the agent's desired space.
        /// </summary>
        /// <param name="agent">the agent that needs solving</param>
        /// <param name="operation">the current operation of the agent</param>
        /// <returns>true if there are no problems with the operation</returns>
        public abstract bool Check(Agent agent, AgentOperation operation);
    }
}
