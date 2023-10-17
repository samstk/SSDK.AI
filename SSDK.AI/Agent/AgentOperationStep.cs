using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// Depicts a step in the agent's operation, with reference to time.
    /// </summary>
    public sealed class AgentOperationStep
    {
        /// <summary>
        /// Gets or sets the time this step should be performed at.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// The actions that must be performed in sequence in as little
        /// time as possible.
        /// </summary>
        public AgentActionExecution[] Executions { get; set; }

        /// <summary>
        /// Create a new step in any operation with the given time to execute, and
        /// action executions.
        /// </summary>
        /// <param name="time">the time in ms to execute relative to the operation start</param>
        /// <param name="executions">the execution specifications of the actions</param>
        public AgentOperationStep(int time, params AgentActionExecution[] executions)
        {
            Time = time;
            Executions = executions;
        }

        /// <summary>
        /// Gets the single execution target, assuming that the number of steps is one.
        /// </summary>
        /// <returns>-1 if empty step, or the target of the first execution</returns>
        public int AsSingle()
        {
            if (Executions.Length == 0) return -1;

            return Executions[0].Target;
        }

        /// <summary>
        /// Returns a new operation step with the exact details as this one.
        /// </summary>
        /// <returns>a clone of this operation step</returns>
        public AgentOperationStep AsNew()
        {
            AgentOperationStep step = new AgentOperationStep(Time, new AgentActionExecution[Executions.Length]);

            for(int i = 0; i<Executions.Length; i++)
            {
                step.Executions[i] = Executions[i].Clone(true);
            }
             
            return step;
        }

        /// <summary>
        /// Executes all active executions
        /// </summary>
        /// <returns></returns>
        public bool Execute(Agent agent)
        {
            bool finished = true;
            foreach(AgentActionExecution exec in Executions)
            {
                if(exec.Active)
                {
                    exec.Execute(agent);
                }
                if(exec.Active) finished = false;
            }
            return finished;
        }

        public override string ToString()
        {
            string txt = "";
            foreach (AgentActionExecution exec in Executions)
            {
                if (txt.Length > 0) txt += ",";
                txt += $"({exec})";
            }
            return txt;
        }
    }
}
