using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// The action space contains the set all possible actions the agent can perform.
    /// </summary>
    public sealed class AgentActionSpace
    {
        /// <summary>
        /// The set of all actions that an agent of a type can perform
        /// </summary>
        public AgentAction[] Actions { get; private set; }

        /// <summary>
        /// Gets a list of all single step operations that can be found
        /// from this action space. <br/>
        /// Assumes that an integer can stored the features as a binary representation
        /// </summary>
        public List<AgentOperation> AllSingleStepOperations
        {
            get
            {
                List<AgentOperation> operations = new List<AgentOperation>();

                int totalFeatures = 1;

                foreach(AgentAction action in Actions)
                {
                    int range = action.MaxRange - action.MinRange + 1;

                    totalFeatures *= range;
                }

                if (AllowWait)
                {
                    // New 'Wait' operation (limited)
                    operations.Add(new AgentOperation());
                }
                
                // Generate all operations
                totalFeatures += 1;
                for (int i = 1; i < totalFeatures; i++)
                {
                    int features = i - 1;
                    // Initialse operation and step to edit
                    AgentOperationStep step = new AgentOperationStep(0,
                           new AgentActionExecution[Actions.Length]
                           );

                    AgentOperation operation = new AgentOperation(step) { };

                    // Populate executions in single step.
                    int actionIndex = 0;
                    foreach (AgentAction action in Actions)
                    {
                        int range = action.MaxRange - action.MinRange + 1;
                        int target = features % range + action.MinRange;
                        step.Executions[actionIndex++] = new AgentActionExecution(action, target);
                        features /= range;
                    }
                    operations.Add(operation);
                }
                return operations;
            }
        }

        /// <summary>
        /// If true, then certain solvers not necessarily perform at least one action.
        /// </summary>
        public bool AllowWait { get; private set; } = false;

        /// <summary>
        /// Creates a new action space with a given set of predefined actions.
        /// If multiple actions exist, then it suggests that 
        /// </summary>
        /// <param name="actions">
        /// the actions that an agent can do. 
        /// If the agent must choose between a number of actions, 
        /// then use a single AgentAction with different target ranges to achieve this.
        /// </param>
        public AgentActionSpace(params AgentAction[] actions)
        {
            Actions = actions;
        }
    }
}
