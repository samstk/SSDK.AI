using SSDK.AI.KBS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Arithmetic
{
    /// <summary>
    /// Represents a negation operation on a constant (e.g. -1)
    /// Not to be confused with the logical not.
    /// 
    /// A negation can only be applied on an arithmetic variable (i.e. KBNumericSymbol).
    /// </summary>
    public sealed class KBNegation : KBFactor
    {
        /// <summary>
        /// The term which is negated.
        /// </summary>
        public KBFactor Factor { get; private set; }

        public KBNegation(KBFactor term)
        {
            Factor = term;
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            if(Solved && HasAltAssertion)
            {
                return KBSolveType.SolveArithmetic;
            }
            return KBSolveType.NoSolution;
        }

        public override KBFactor GetAltSolutionForChild(KB kb, KBFactor child)
        {
            if (Solved && HasAltAssertion)
            {
                return AltAssertion.Apply('-');
            }
            return null;
        }

        public override KBFactor Calculate()
        {
            if (!Solved) return KBFactor.Null;
            return Factor.Solved ? Factor.Calculate().Apply('-') : null;
        }

        public override bool Holds()
        {
            return true; // An arithmetic operation always holds true.
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            return Factor.GetSymbols();
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { Factor };
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = Factor.SolveAssertion(kb, this);
            if (!Solved)
            {
                KBSolveType type = parent as object != null ? parent.CanSolveForChild(kb, this) : KBSolveType.NoSolution;
                if(type == KBSolveType.NoSolution || type == KBSolveType.Other)
                {
                    // attempt to solve from child.
                    if(Factor.Solved)
                    {
                        SolveAssertOther(kb, Calculate());
                        changes++;
                    }
                }
                else if (type == KBSolveType.SolveArithmetic)
                {
                    SolveAssertOther(kb, parent.GetAltSolutionForChild(kb, this)); changes++;
                }
            }
            return changes;
        }

        public override string ToString()
        {
            return $"-{Factor}";
        }
    }
}
