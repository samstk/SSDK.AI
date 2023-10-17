using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// A function which determines whether the agent can perform an action yet.
    /// </summary>
    /// <param name="agent">the agent to test</param>
    /// <param name="action">the action to check</param>
    /// <returns>true if the action can be performed</returns>
    public delegate bool AgentPromptFunction(Agent agent, AgentAction action);
    /// <summary>
    /// Depicts an execution of a particular action
    /// </summary>
    public sealed class AgentActionExecution
    {
        /// <summary>
        /// The action that will be performed on the target.
        /// </summary>
        public AgentAction Action { get; private set; }

        /// <summary>
        /// The prompt that determines
        /// </summary>
        public AgentPromptFunction Prompt { get; private set; }

        /// <summary>
        /// The target in which the action is to be performed to.
        /// </summary>
        public int Target { get; private set; }

        /// <summary>
        /// Gets or sets whether this execution should be checked
        /// or performed.
        /// </summary>
        public bool Active { get; set; } = true;
        
        /// <summary>
        /// Creates information for an execution of the action
        /// </summary>
        /// <param name="agent">the agent that this action is performed on</param>
        /// <param name="action">the action that is going to be performed</param>
        /// <param name="target">the target argument of the action</param>
        /// <param name="prompt">the prompt function which determines if this can be executed</param>
        public AgentActionExecution(AgentAction action, int target, AgentPromptFunction prompt=null)
        {
            Action = action;
            Target = target;
            Prompt = prompt;
        }

        /// <summary>
        /// Executes on the current agent using the target on the set action.
        /// </summary>
        /// <returns>true if the action could be executed</returns>
        public bool Execute(Agent agent)
        {
            bool canExecute = Prompt == null || Prompt(agent, Action);
            
            if(canExecute)
            {
                // Perform the action
                Action.Action(agent, Target);
                Active = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a perfect clone of this execution
        /// </summary>
        /// <param name="asActive">if true, the clone is set to active</param>
        /// <returns>the clone of the object</returns>
        public AgentActionExecution Clone(bool asActive=true)
        {
            return new AgentActionExecution(Action, Target, Prompt)
            {
                Active = asActive
            };
        }

        public override string ToString()
        {
            return Target.ToString();
        }
    }
}
