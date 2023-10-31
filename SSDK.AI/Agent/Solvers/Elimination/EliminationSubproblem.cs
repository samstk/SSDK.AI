using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Solvers.Elimination
{
    /// <summary>
    /// Depicts a subproblem for an elimination solver, which
    /// contains a number of actions that could be applied to a
    /// particular target.
    /// </summary>
    public sealed class EliminationSubproblem
    {
        /// <summary>
        /// Gets or sets the target of this subproblem for reference.
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// Gets or sets the tag of the problem which may be used
        /// to store information outside of the subproblem, but is relevant
        /// to the subproblem.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets the particular actions that could occur and effect
        /// this particular target.
        /// </summary>
        public AgentAction[] Actions { get; set; }

        /// <summary>
        /// Gets or sets the required action that will eliminate this subproblem
        /// on the next iteration it receives. <br/>
        /// Can be used to simplify logic when actions may conflict.
        /// </summary>
        public AgentAction RequiredAction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="actions"></param>
        public EliminationSubproblem(object target, AgentAction[] actions)
        {
            Target = target;
            Actions = actions;
        }
    }
}
