using SSDK.AI.Agent.Info;
using SSDK.AI.KBS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// The base for every AI agent, with the aim to produce 'intelligence' using a generic model,
    /// that be modified to use different algorithms for solve a problem.
    /// 
    /// A generic agent is made up of the following components:
    /// (A) Action Space which is the set of all possible actions the agent can perform.
    /// (B) Problem Space, which contains the current state of the world the agent can perceive,
    ///                    the desired state of the world the agent wants to be in.
    /// (C) Agent solver,  which takes the problem space and turns it into a sequence of operations
    ///                    of the action space.
    /// </summary>
    public sealed class Agent
    {
        /// <summary>
        /// Gets the action space, which contains the set of all possible operations the
        /// agent can perform.
        /// </summary>
        public AgentActionSpace ActionSpace { get; private set; }

        /// <summary>
        /// The solver which attempts to solve the problem of this agent with one or more algorithms.
        /// </summary>
        public AgentSolver Solver { get; private set; }

        /// <summary>
        /// The current operation that the agent is undertaking to achieve said
        /// desired space.
        /// </summary>
        public AgentOperation CurrentOperation { get; private set; }

        /// <summary>
        /// Gets the current problem space, which contains what the agent can perceive, and
        /// methods to predict for some operations.
        /// </summary>
        public AgentProblemSpace CurrentProblemSpace { get; private set; }

        /// <summary>
        /// Gets or sets additional information about the agent.
        /// </summary>
        public AgentInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the desired problem space, which contains what the agent wants to perceive.
        /// </summary>
        public AgentProblemSpace DesiredProblemSpace { get; set; }


        /// <summary>
        /// Gets the queue of desired problem spaces.
        /// When using the guide function, it populates this queue so that
        /// our desired spaces are reached sequentially.
        /// </summary>
        public Queue<AgentProblemSpace> ProblemDivisionQueue { get; private set; } = new Queue<AgentProblemSpace>();


        private bool HasUpdatedProblemForSolver = false;

        /// <summary>
        /// Constructs an agent for the sole purpose of deploying actions in the action space,
        /// to solve the problem defined in the problem space.
        /// </summary>
        /// <param name="actionSpace">the space containing the set of all possible actions the agent can perform</param>
        /// <param name="problem">the problem which the agent has initially</param>
        /// <param name="solver">a solver for th a</param>
        /// <param name="info">additional agent information (does not affect the agent)</param>
        public Agent(AgentActionSpace actionSpace, AgentProblemSpace problem, AgentSolver solver, AgentInfo info=null)
        {
            ActionSpace = actionSpace;
            CurrentProblemSpace = problem;
            Solver = solver;
            Info = info;
        }

        /// <summary>
        /// Guides the agent into desiring a certain state first before finally reaching the desired space.
        /// </summary>
        public void Guide(params AgentProblemSpace[] sequentialSpaces)
        {
            foreach (AgentProblemSpace space in sequentialSpaces)
            {
                ProblemDivisionQueue.Enqueue(space);
            }

            // Store desired space to end of guided steps.
            if (DesiredProblemSpace != null)
                ProblemDivisionQueue.Enqueue(DesiredProblemSpace);

            // Updates the current desired space
            DesiredProblemSpace = ProblemDivisionQueue.Dequeue();
        }

        /// <summary>
        /// Update the agent along-side the perception of the world, and attempt
        /// to solve the problem the agent currently faces. If no desired problem space is defined,
        /// then it will do nothing.
        /// </summary>
        public void Solve()
        {
            CurrentProblemSpace?.Perceive(this);

            if (DesiredProblemSpace != null)
            {
                // Update next desired space if current problem space matches desired space.
                if (CurrentProblemSpace.DistanceTo(DesiredProblemSpace) <= DesiredProblemSpace.MatchTolerance)
                {
                    if (ProblemDivisionQueue.Count > 0)
                    {
                        DesiredProblemSpace = ProblemDivisionQueue.Dequeue();
                    }
                }
            }

            // Attempt to solve the problem
            if (Solver != null)
            {
                if (!HasUpdatedProblemForSolver)
                {
                    Solver.UpdateProblem(this);
                    HasUpdatedProblemForSolver = true;
                }

                if (CurrentOperation != null)
                {
                    if (Solver.Check(this, CurrentOperation))
                    {
                        CurrentOperation = null; // Disregard current operation
                    }
                }

                if (CurrentOperation == null)
                {
                    CurrentOperation = Solver.Solve(this);
                    if (CurrentOperation != null) CurrentOperation = CurrentOperation.AsNew();
                }
            }
        }

        /// <summary>
        /// Attempts to execute or continue the current operation if it exists.
        /// </summary>
        public void Execute()
        {
            if (CurrentOperation != null)
            {
                if (CurrentOperation.IsReady && CurrentOperation.Execute(this))
                {
                    CurrentOperation = null;
                }
            }
        }

        /// <summary>
        /// Attempts to completely execute (finish) the current operation if it exists.
        /// </summary>
        public void ExecuteAll()
        {
            if (CurrentOperation != null)
            {
                if (CurrentOperation.IsReady && CurrentOperation.ExecuteAll(this))
                {
                    CurrentOperation = null;
                }
            }
        }

        /// <summary>
        /// Updates the current problem space.
        /// </summary>
        public void UpdateProblem(AgentProblemSpace newProblem)
        {
            CurrentProblemSpace = newProblem;
            Solver?.UpdateProblem(this);
            HasUpdatedProblemForSolver = true;
        }

        /// <summary>
        /// Updates the current problem space, by branching from the current space
        /// by using prediction of what a given operation will do to the space.
        /// </summary>
        /// <param name="operation">the operation to predict from</param>
        public void UpdateProblemUsingPrediction(AgentOperation operation)
        {
            UpdateProblem(CurrentProblemSpace.Predict(this, operation));
        }

        /// <summary>
        /// Updates the current problem space, by branching from the current space
        /// by using prediction of what a given operation will do to the space.
        /// </summary>
        /// <param name="type">
        /// the operation action type which may be picked up in the problem space to indicate
        /// a specific action to take.
        /// </param>
        public void UpdateProblemUsingPrediction(int type)
        {
            UpdateProblemUsingPrediction(AgentOperation.Single(type));
        }
    }
}
