using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.CSP
{
    public delegate bool CSPConstraintChecker();
    public delegate (CSPVariable, object) CSPConstraintHint();
    /// <summary>
    /// Represents a constraint within a CSP (Constraint-Satisfaction-Problem)
    /// </summary>
    public sealed class CSPConstraint
    {
        #region Properties & Fields
        public string Name = "Unknown Constraint Type";
        /// <summary>
        /// Gets the constraint checking function which formed this constraint
        /// </summary>
        public CSPConstraintChecker ConstraintChecker { get; private set; }

        /// <summary>
        /// Gets the hint of the constraint, which allows assumptions 
        /// of a particular value of a variable to be made based on the dynamics of the constraint<br/>
        /// (e.g. a containment constraint - see AddContainmentConstraint - requires that the
        /// given list of variables' domains must contain a domain value at least once, and
        /// if no other variable has the value in its domain, then a particular variable with that
        /// domain value must be solved to that value)
        /// </summary>
        public CSPConstraintHint ConstraintHint { get; private set; }

        /// <summary>
        /// Gets the related variables of this constraint.
        /// </summary>
        public CSPVariable[] RelatedVariables { get; private set; }
        #endregion

        /// <summary>
        /// Creates a CSP constraint with referenced variables
        /// </summary>
        /// <param name="relatedVariables">the related variables of this constraint</param>
        /// <param name="constraintChecker">the function to check whether the constraint holds</param>
        /// <param name="hint">
        /// a hint of the constraint, which allows assumptions 
        /// of a particular value of a variable to be made based on the dynamics of the constraint<br/>
        /// (e.g. a containment constraint - see AddContainmentConstraint - requires that the
        /// given list of variables' domains must contain a domain value at least once, and
        /// if no other variable has the value in its domain, then a particular variable with that
        /// domain value must be solved to that value)</param>
        public CSPConstraint(CSPConstraintChecker constraintChecker, CSPVariable[] relatedVariables, CSPConstraintHint hint=null, string name=null)
        {
            ConstraintChecker = constraintChecker;
            ConstraintHint = hint;
            foreach(CSPVariable variable in relatedVariables)
            {
                variable.RelatedConstraints.Add(this);
            }
            RelatedVariables = relatedVariables;
            Name = name;
        }

        /// <summary>
        /// Gets whether the constraint holds
        /// </summary>
        public bool Holds => ConstraintChecker();

        /// <summary>
        /// Gets the next hint of this constraint.
        /// </summary>
        /// <returns>(null,null) if no hint exists, or the variable and solution if it does</returns>
        public (CSPVariable, object) GetHint() => ConstraintHint == null ? (null,null) : ConstraintHint();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool next = false;
            foreach(CSPVariable variable in RelatedVariables)
            {
                if (next)
                    sb.Append(", ");
                next = true;
                sb.Append(variable.Name);
            }
            return $"{Name} On {sb} {(Holds ? "<Holds>" : "<Does NOT hold>")}";
        }
    }
}
