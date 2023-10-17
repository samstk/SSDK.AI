using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// Represents the solve type that a parent can pass to a calling child.
    /// </summary>
    public enum KBSolveType
    {
        /// <summary>
        /// Represents that there is no current solution to the child.
        /// </summary>
        NoSolution,
        /// <summary>
        /// Represents that this solves differently (child does not need solving)
        /// as parent will definitely handle it.
        /// </summary>
        Other,
        /// <summary>
        /// Represents that the child can be asserted to be true
        /// </summary>
        SolveTrue,
        /// <summary>
        /// Represents that the child can be asserted to be false
        /// </summary>
        SolveFalse,
        /// <summary>
        /// Represents that the child can be solved to a certain arithmetic value.
        /// The child must call the parent's GetAltSolutionForChild.
        /// </summary>
        SolveArithmetic
    }
}
