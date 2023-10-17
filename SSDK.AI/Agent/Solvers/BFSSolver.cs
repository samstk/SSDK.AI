using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSDK.AI.Agent;
using SSDK.Core;
using SSDK.Core.Structures.Graphs;

namespace SSDK.AI.Agent.Solvers
{
    /// <summary>
    /// Depicts a BFS solver for an AI agent.
    /// <br/>
    /// BFS has the following: <br/>
    /// * To be used for solving a problem all at once (resulting in a set of subsequently achievable states) <br/>
    /// * Assumes that actions are deterministic <br/>
    /// * Requires Hash and Equals to be implemented in problem space <br/>
    /// + Shortest path detection with no action cost <br/>
    /// + Perfect rationality but heavy memory consumption for every state <br/>
    /// - Heavy memory consumption for exponential states, however guiding the agent into sub-problems may alleviate this problem.  <br/>
    /// - Depends on exact state computation <br/>
    /// - A blind search algorithm
    /// </summary>
    public class BFSSolver : AgentSolver
    {

        public override bool Check(Agent agent, AgentOperation operation)
        {
            // Always believe that an operation resulting from BFS is accurate.
            return true;
        }

        /// <summary>
        /// Solves the agent by using BFS to generate an operation that computes
        /// the closest path without accounting for action costs.
        /// </summary>
        /// <param name="agent">the agent to solve</param>
        /// <returns>an operation attempts to lead the agent to the desired space</returns>
        public override AgentOperation Solve(Agent agent)
        {
            // Solve BFS by constructing expanding graph.
            Graph<AgentProblemSpace> graph = new Graph<AgentProblemSpace>();
            GraphVertex<AgentProblemSpace> startingNode = graph.Add(agent.CurrentProblemSpace);

            // Generate queue for problem spaces
            Queue<GraphVertex<AgentProblemSpace>> frontierQueue = new Queue<GraphVertex<AgentProblemSpace>>();
            
            // Initialise the explore hash set
            HashSet<AgentProblemSpace> exploredSet = new HashSet<AgentProblemSpace>();
            HashSet<AgentProblemSpace> frontierSet = new HashSet<AgentProblemSpace>();
            frontierSet.Add(startingNode.Value);
            frontierQueue.Enqueue(startingNode);

            // Explore BFS until distance from desired node to starting node is completed.
            // Form edges and graph
            while(frontierQueue.Count > 0)
            {
                GraphVertex<AgentProblemSpace> explorationVertex = frontierQueue.Dequeue();
                AgentProblemSpace explorationSpace = explorationVertex.Value;
                frontierSet.Remove(explorationSpace);
                exploredSet.Add(explorationSpace);

                // Check if desired state is found.
                double dist = explorationSpace.DistanceTo(agent.DesiredProblemSpace);
                if (dist <= MatchTolerance)
                {
                    // Generate operation based on found path (leading edge allows for backtracing)
                    AgentOperation newOperation = new AgentOperation();
                    while (explorationVertex != startingNode)
                    {
                        newOperation.Merge(explorationVertex.LeadingEdge.Tag as AgentOperation);
                        explorationVertex = explorationVertex.LeadingEdge.VertexFrom;
                    }
                    newOperation.Reverse();
                    return newOperation;
                }

                // Compute every state branch from current state using list of current available operations.
                foreach(AgentOperation operation in AllOperations)
                {
                    AgentProblemSpace newSpace = explorationSpace.Predict(agent, operation);
                    dist = explorationSpace.DistanceTo(newSpace);

                    if (dist <= MatchTolerance) continue;// Only add new spaces for queue
                    
                    if (!exploredSet.Contains(newSpace) && !frontierSet.Contains(newSpace))
                    {
                        // Create node in graph and create path from start to new
                        GraphVertex<AgentProblemSpace> newVertex = graph.Add(newSpace);
                        newVertex.LeadingEdge = graph.CreatePath(explorationVertex, newVertex, 0);
                        newVertex.LeadingEdge.Tag = operation;
                        frontierQueue.Enqueue(newVertex);
                    }
                }
            }

            // Could not find a way to the desired state, so just use empty operation.
            return new AgentOperation();
        }

        private List<AgentOperation> AllOperations = null;
        public override void UpdateProblem(Agent agent)
        {
            // Simply populate list of actions, assuming that agent actions can't change.
            AllOperations = agent.ActionSpace.AllSingleStepOperations;
        }

        public override string ToString()
        {
            return "Breadth-First Search";
        }
    }
}
