using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent
{
    /// <summary>
    /// Depicts an object attached an agent, operation or action.
    /// </summary>
    public class AgentObject
    {
        /// <summary>
        /// Modifies the object according to an agent operation
        /// if applicable.
        /// </summary>
        /// <param name="operation">the operation to modify with</param>
        public virtual void Modify(AgentOperation operation)
        {

        }
        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns>a new object which is an exact copy of this one</returns>
        public virtual AgentObject Clone()
        {
            return new AgentObject();
        }
    }
}
