using SSDK.Core.Structures.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// An agent's operation, which may have multiple sequences
    /// </summary>
    public sealed class AgentOperation
    {
        /// <summary>
        /// The time in which the operation started execution.
        /// </summary>
        public DateTime TimeStarted { get; private set; }

        /// <summary>
        /// If at any point the operation has been paused, then
        /// this depicts the time at which it was paused.
        /// </summary>
        public DateTime TimePaused { get; private set; }

        /// <summary>
        /// The integer argument of the operation.
        /// If using Agent.UpdateProblemUsingPrediction whilst using
        /// an integer to specific a type of action, then this will be 
        /// set to that integer.
        /// </summary>
        public int ActionType { get; private set; } = -1;

        /// <summary>
        /// The steps of the execution which must run in sequence.
        /// </summary>
        public List<AgentOperationStep> Steps { get; private set; } = new List<AgentOperationStep>();

        /// <summary>
        /// Creates an empty agent operation
        /// </summary>
        public AgentOperation() {
            Steps = new List<AgentOperationStep>();
        }
        
        /// <summary>
        /// Creates an agent operation with the given steps.
        /// </summary>
        /// <param name="steps">the steps of the opreation</param>
        public AgentOperation(params AgentOperationStep[] steps)
        {
            Steps = steps.ToList();
        }

        /// <summary>
        /// The status of the operation
        /// </summary>
        public AgentOperationStatus Status { get; private set; } = AgentOperationStatus.Scheduled;

        /// <summary>
        /// The index of the current step of the operation.
        /// </summary>
        public int CurrentStep { get; private set; } = 0;

        /// <summary>
        /// Gets the single operation target, assuming that the number of steps is one.
        /// </summary>
        /// <returns>-1 if empty operation, or the target of the first execution first</returns>
        public int AsSingle()
        {
            if (ActionType != -1) return ActionType; // Already as single
            if (Steps.Count == 0) return -1;

            return Steps[0].AsSingle();
        }

        /// <summary>
        /// Merges the other operation into this operation, by appending the other operations steps
        /// to this.
        /// </summary>
        /// <param name="endWithOperation">the operation to end with</param>
        public void Merge(AgentOperation endWithOperation)
        {
            Steps.AddRange(endWithOperation.Steps);
        }

        /// <summary>
        /// Reverses the steps in the current operation.
        /// Some algorithms require this as they backtrack to find the path.
        /// </summary>
        public void Reverse()
        {
            Steps.Reverse();
        }

        /// <summary>
        /// Continues the execution of the operation as normal. 
        /// </summary>
        public bool Execute(Agent agent)
        {
            if (CurrentStep >= Steps.Count) return true; // Operation finished

            if (Status == AgentOperationStatus.Finished)
                return true;

            if (Status == AgentOperationStatus.Scheduled)
            {
                TimeStarted = DateTime.Now;
                Status = AgentOperationStatus.Running;
            }
            else if (Status == AgentOperationStatus.Paused)
            {
                int offset = (int)(TimePaused - TimeStarted).TotalMilliseconds;
                TimeStarted = DateTime.Now;
                foreach(AgentOperationStep step in Steps)
                {
                    step.Time -= offset;
                }
                Status = AgentOperationStatus.Running;
            }

            // Execute in regard to time.
            int time = (int)(DateTime.Now - TimeStarted).TotalMilliseconds;
            AgentOperationStep currentStep = Steps[CurrentStep];
            
            if(currentStep.Execute(agent))
            {
                CurrentStep++;
                if(CurrentStep >= Steps.Count)
                {
                    Status = AgentOperationStatus.Finished;
                }
            }

            if (CurrentStep >= Steps.Count) return true; // Operation finished
            return false;
        }

        /// <summary>
        /// Completes the execution of the operation immediately, ignoring time steps. 
        /// </summary>
        public bool ExecuteAll(Agent agent)
        {
            if (CurrentStep >= Steps.Count) return true; // Operation finished

            if (Status == AgentOperationStatus.Finished)
                return true;

            if (Status == AgentOperationStatus.Scheduled)
            {
                TimeStarted = DateTime.Now;
                Status = AgentOperationStatus.Running;
            }
            else if (Status == AgentOperationStatus.Paused)
            {
                int offset = (int)(TimePaused - TimeStarted).TotalMilliseconds;
                TimeStarted = DateTime.Now;
                foreach (AgentOperationStep step in Steps)
                {
                    step.Time -= offset;
                }
                Status = AgentOperationStatus.Running;
            }

            // Execute all steps without regard to time or completion.
            for(; CurrentStep<Steps.Count; CurrentStep++)
            {
                Steps[CurrentStep].Execute(agent);
            }
            if (CurrentStep >= Steps.Count) return true; // Operation finished
            return false;
        }

        /// <summary>
        /// True if the operation is ready to be executed.
        /// </summary>

        public bool IsReady
        {
            get
            {
                return Status == AgentOperationStatus.Scheduled || Status == AgentOperationStatus.Running || Status == AgentOperationStatus.Finished;
            }
        }
        
        /// <summary>
        /// Returns a new operation with the exact details as this one.
        /// </summary>
        /// <returns>a clone of this operation</returns>
        public AgentOperation AsNew()
        {
            AgentOperation operation = new AgentOperation()
            {
                ActionType = ActionType
            };

            foreach(AgentOperationStep step in Steps)
            {
                operation.Steps.Add(step.AsNew());
            }

            return operation;
        }
        /// <summary>
        /// Pauses the operation entirely.
        /// </summary>
        public void Pause()
        {
            // Cannot pause an already finished operations
            if (Status == AgentOperationStatus.Finished) return; 
            TimePaused = DateTime.Now;
            Status = AgentOperationStatus.Paused;
        }

        /// <summary>
        /// Creates an operation from a single vague action type.
        /// </summary>
        /// <param name="actionType">an action type which may be intepreted by the problem space.</param>
        /// <returns>the operation representing the action type</returns>
        public static AgentOperation Single(int actionType)
        {
            return new AgentOperation() { ActionType = actionType };
        }

        /// <summary>
        /// Creates an operation from a single execution.
        /// </summary>
        /// <param name="singleExecution">the execution that represents this operation</param>
        /// <returns>the operation representing the execution</returns>
        public static AgentOperation Single(AgentActionExecution singleExecution)
        {
            return new AgentOperation(new AgentOperationStep(0, singleExecution));
        }

        /// <summary>
        /// Calculates the total cost of the operation
        /// </summary>
        /// <returns>the total cost of the operation</returns>
        public UncontrolledNumber CalculateTotalCost(Agent agent)
        {
            UncontrolledNumber total = 0;
            foreach(AgentOperationStep step in Steps)
            {
                foreach(AgentActionExecution execution in step.Executions)
                {
                    if (execution.Action.Cost == null) total += 1;
                    else total += execution.Action.Cost(agent, execution.Target); 
                }
            }
            return total;
        }

        public override string ToString()
        {
            string operation = "";
            int i = 1;
            foreach(AgentOperationStep step in Steps)
            {
                operation += $"{(i++)}. {step} \n";
            }
            return operation;
        }
    }

    /// <summary>
    /// Depicts the status of an operation
    /// </summary>
    public enum AgentOperationStatus
    {
        /// <summary>
        /// Depicts that the operation has not yet been started, but it
        /// is either scheduled or for planning purposes.
        /// </summary>
        Scheduled = 1,
        /// <summary>
        /// Depicts that the operation is currently being executed, but 
        /// hasn't finished yet.
        /// </summary>
        Running = 2,
        /// <summary>
        /// Depicts that the operation is finished entirely, and should
        /// be disposed.
        /// </summary>
        Finished = 4,
        /// <summary>
        /// Depicts that the operation is currently paused.
        /// </summary>
        Paused = 8,
    }

}
