using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.CSP
{
    /// <summary>
    /// Represents a variable within a CSP (Constraint-Satisfaction-Problem)
    /// </summary>
    public sealed class CSPVariable
    {
        #region Properties & Fields
        /// <summary>
        /// Gets or sets the name of the variable for reference
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the tag of the variable
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets whether the variable has been solved within a CSP.
        /// </summary>
        public bool Solved { get; internal set; } = false;

        /// <summary>
        /// Gets the singular solution of this variable.
        /// Accurate if Solved=true
        /// </summary>
        public object Solution { get; internal set; }

        /// <summary>
        /// Gets the 
        /// </summary>
        internal int BacktrackingIndex = 0;

        /// <summary>
        /// Gets or sets the domain of the variable.
        /// </summary>
        public CSPDomain Domain { get; set; }

        /// <summary>
        /// Gets the constraints that relates to this variable.
        /// </summary>
        public List<CSPConstraint> RelatedConstraints { get; internal set; } = new List<CSPConstraint>();
        #endregion
        public static implicit operator CSPVariable(string name) => new CSPVariable(name);

        public CSPVariable(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Attempts to reduce related domains of other variables
        /// related to this variable.
        /// </summary>
        /// <remarks>
        /// Only effective if this variable is solved.
        /// If used when variable is unsolved and Solution is inaccurate,
        /// then may eliminate incorrect values from domain.
        /// </remarks>
        public void ReduceRelatedDomains()
        {
            RelatedConstraints.ForEach((constraint) =>
            {
                foreach(CSPVariable variable in constraint.RelatedVariables)
                {
                    if (variable != this)
                    {
                        if (Solved)
                        {
                            variable.ReduceDomain();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Trims the domains of this variable
        /// (i.e. if any constraints fails for a particular value of an existing domain,
        /// then it is removed)
        /// </summary>
        /// <remarks>
        /// Cannot reduce the domain if the variable is solved.
        /// </remarks>
        public void ReduceDomain()
        {
            if (Solved)
                return;

            List<object> toRemove = new List<object>();
            foreach (object obj in Domain)
            {
                Solution = obj;
                foreach (CSPConstraint constraint in RelatedConstraints)
                {
                    if (!constraint.Holds)
                    {
                        toRemove.Add(obj);
                        break;
                    }
                }
            }

            foreach (object obj in toRemove)
                Domain.Remove(obj);

            Solution = null;
            Solved = false;
        }

        /// <summary>
        /// Sets the solution of this variable.
        /// </summary>
        /// <param name="solution">the solution of this variable</param>
        public void SetSolution(object solution)
        {
            Solved = true;
            Solution = solution;
            Domain.Set(solution);
        }

        public override string ToString() => $"{Name}{(Solved ? $" = {Solution}" : Solution != null ? $" ?= {Solution}" : "")}{(!Solved ? $" {Domain}" : "")}";
    }
}
