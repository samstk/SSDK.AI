using SSDK.Core.Structures.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// The action handler which handles the given target of the action.
    /// </summary>
    /// <param name="agent">the agent to perform the action on</param>
    /// <param name="target">the target of the action (within min-max)</param>
    public delegate void AgentActionHandler(Agent agent, int target);

    /// <summary>
    /// The cost handler which calculates the cost of the given action.
    /// </summary>
    /// <param name="agent">the agent to perform the action on</param>
    /// <param name="target">the target of the action (within min-max)</param>
    /// <returns>the 'cost' of the action</returns>
    public delegate UncontrolledNumber AgentCostFunction(Agent agent, int target);
    /// <summary>
    /// An action which can be activated to a certain range.
    /// </summary>
    public sealed class AgentAction
    {
        /// <summary>
        /// Name for the action for references purposes.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets the min action target for this operation (default is zero)
        /// </summary>
        public int MinRange { get; private set; } = 0;

        /// <summary>
        /// Gets the max action target for this operation (default is one).
        /// Inclusive.
        /// </summary>
        public int MaxRange { get; private set; } = 1;

        /// <summary>
        /// A tag that may be required for prediction.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets the action handler that performs the action on the agent's
        /// world, using an integer to specify an argument.
        /// </summary>
        public AgentActionHandler Action { get; private set; }

        /// <summary>
        /// Gets the cost calculation which may be used on certain solvers
        /// to determine best paths.
        /// </summary>
        public AgentCostFunction Cost { get; private set; }

        /// <summary>
        /// Defines a new agent operation which takes an action handler, and allows
        /// a certain range of values from (min-max) inclusive, to be passed to the 
        /// handler every solution.
        /// </summary>
        /// <param name="actionHandler">
        /// the action handler that performs the action on the agent's
        /// world, using an integer to specify an argument.
        /// </param>
        /// <param name="costHandler">
        /// the cost calculation function that takes the agent, a target within min-max,
        /// and returns the estimated cost to perform the action.
        /// </param>
        /// <param name="minRange">the min target integer</param>
        /// <param name="maxRange">the max target integer inclusive</param>
        /// <param name="tag">tag for reference purposes</param>
        public AgentAction(AgentActionHandler actionHandler, AgentCostFunction costHandler = null, int minRange = 0, int maxRange = 1, object tag=null)
        {
            Action = actionHandler;
            Cost = costHandler;
            MinRange = minRange;
            MaxRange = maxRange;
            Tag = tag;
        }

        /// <summary>
        /// Defines a new agent operation which takes an action handler, and allows
        /// a certain range of values from (min-max) inclusive, to be passed to the 
        /// handler every solution.
        /// </summary>
        /// <param name="name">name of the action for reference purposes</param>
        /// <param name="actionHandler">
        /// the action handler that performs the action on the agent's
        /// world, using an integer to specify an argument.
        /// </param>
        /// <param name="costHandler">
        /// the cost calculation function that takes the agent, a target within min-max,
        /// and returns the estimated cost to perform the action.
        /// </param>
        /// <param name="minRange">the min target integer</param>
        /// <param name="maxRange">the max target integer inclusive</param>
        /// <param name="tag">tag for reference purposes</param>
        public AgentAction(string name, AgentActionHandler actionHandler, 
            AgentCostFunction costHandler = null, int minRange = 0, int maxRange = 1, object tag=null)
        {
            Name = name;
            Action = actionHandler;
            Cost = costHandler;
            MinRange = minRange;
            MaxRange = maxRange;
            Tag = tag;
        }
    }
}
