using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSDK.Core;
using SSDK.Core.Structures.Graphs;

namespace SSDK.AI.Agent.Solvers.Elimination
{
    /// <summary>
    /// Depicts a solver by elimination of subproblems for an AI agent.
    /// <br/>
    /// Elimination Solver has the following: <br/>
    /// * To be used for solving a problem that can be solved via elimination of actions and targets. <br/>
    /// * SubproblemGenerator must be implemented, which populates a decreasing list of targets that can be
    ///   eliminated via a use of an action (on initial state only) <br/>
    /// * Refactor must be implemented, which 
    /// * Assumes that actions are deterministic <br/>
    /// * Does not use prediction to check actions for a particular state, and instead assumes that
    ///   all actions are possible and will result in a state where the subproblem is removed after
    ///   choosing an action.
    /// * Does not attempt to move to the desired space, and instead eliminates subproblems with certainty
    ///   (possible action count on particular problem = 1) <br/>
    /// + Can be used for complex problems such as sudoku, which has no known solution from the start <br/>
    /// - Assumes that actions are targets only (no ranges) <br/>
    /// - Actions that lead to a undesired state are unaccounted for <br/>
    /// </summary>
    public class EliminationSolver : AgentSolver
    {
        /// <summary>
        /// Gets or sets the generator for an initial problem space.
        /// </summary>
        public Func<AgentProblemSpace, (List<EliminationSubproblem>, AgentObject)> SubproblemGenerator { get; set; }

        /// <summary>
        /// Gets or sets the refactoring for how any action affects given subproblem. 
        /// </summary>
        public Action<AgentAction, EliminationSubproblem, AgentObject> Refactor { get; set; }

        /// <summary>
        /// Gets or sets the refactoring for how any action affects the given world state (subproblem tag
        /// references may be updated)
        /// </summary>
        public Action<AgentAction, AgentObject> StateRefactor { get; set; }

        public override bool Check(AgentProblemSpace problemSpace, AgentProblemSpace desiredSpace, Agent agent, AgentOperation operation)
        {
            // Always believe that an operation resulting from elimination is accurate.
            return true;
        }

        /// <summary>
        /// Solves the agent by using elimination to generate an operation that computes
        /// any given sequence of operations that results into a state with no subproblems,
        /// or a terminated state (with uncertainty for each subproblem)
        /// </summary>
        /// <param name="agent">the agent to solve</param>
        /// <returns>
        /// an operation attempts to lead the agent to a state with no subproblems, or 
        /// a terminated state (with no certain actions possible).
        /// returns>
        public override AgentOperation Solve(AgentProblemSpace problemSpace, AgentProblemSpace desiredSpace, Agent agent)
        {
            AgentProblemSpace currentProblem = agent.CurrentProblemSpace;

            if (currentProblem == null)
                return new AgentOperation();

            if(SubproblemGenerator == null || StateRefactor == null || Refactor == null)
            {
                throw new NotImplementedException(
                    "Refactor, StateRefactor and SubproblemGenerator must be implemented in the corresponding solver"
                    );
            }

            (List<EliminationSubproblem> subproblems, AgentObject problemTag) = SubproblemGenerator(currentProblem);
            AgentOperation operation = new AgentOperation();

            bool canContinue = true;
            while (canContinue)
            {
                if (subproblems.Count == 0)
                    break; // No more subproblems to solve.

                canContinue = false;
                for(int i = subproblems.Count - 1; i > -1; i--)
                {
                    EliminationSubproblem problem = subproblems[i];

                    if(problem.Actions.Length == 1 || problem.RequiredAction != null)
                    {
                        // We can perform the action, so add to operation
                        AgentAction action = problem.RequiredAction != null ? problem.RequiredAction : problem.Actions[0];
                        operation.Steps.Add(new AgentOperationStep(operation.Steps.Count,
                            new AgentActionExecution(action, 0)));

                        subproblems.RemoveAt(i);

                        StateRefactor(action, problemTag);

                        for(int x = subproblems.Count - 1; x > -1; x--)
                        {
                            Refactor(action, subproblems[x], problemTag);
                        }

                        // At least one operation was performed, so continue.
                        canContinue = true;
                    }
                }
            }

            operation.SolvedTag = problemTag;
            return operation; // No operations can be done with certainty.
        }
       
        public override void UpdateProblem(AgentProblemSpace problemSpace, AgentProblemSpace desiredSpace, Agent agent)
        {
            // All done in solve
        }

        public override string ToString()
        {
            return "Elimination Search";
        }
    }
}
