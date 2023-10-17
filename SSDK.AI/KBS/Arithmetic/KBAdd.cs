using SSDK.AI.KBS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Arithmetic
{
    /// <summary>
    /// Represents a add operation on a constant (e.g. 1 + 2)
    /// </summary>
    public sealed class KBAdd : KBFactor
    {
        /// <summary>
        /// The left-side term which is compared
        /// </summary>
        public KBFactor LHS { get; private set; }

        /// <summary>
        /// The right-side term which is compared.
        /// </summary>
        public KBFactor RHS { get; private set; }

        public KBAdd(KBFactor lhs, KBFactor rhs)
        {
            LHS = lhs;
            RHS = rhs;
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            if (Solved && HasAltAssertion && (LHS.Solved || RHS.Solved))
            {
                return KBSolveType.SolveArithmetic;
            }
            return KBSolveType.NoSolution;
        }

        public override KBFactor GetAltSolutionForChild(KB kb, KBFactor child)
        {
            if (Solved && HasAltAssertion)
            {
                if(LHS.Solved && !RHS.Solved && child.Equals(RHS))
                {
                    return AltAssertion.Apply('-', LHS.Calculate()); // c = a + b, b = c - a
                }
                else if (RHS.Solved && !LHS.Solved && child.Equals(LHS))
                {
                    return AltAssertion.Apply('-', RHS.Calculate()); // c = a + b, a = c - b
                }
            }
            return null;
        }

        public override KBFactor Calculate()
        {
            if (!Solved) return KBFactor.Null;
            return LHS.Calculate().Apply('+', RHS.Calculate());
        }

        public override bool Holds()
        {
            return true; // An arithmetic operation always holds true.
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> symbols = new HashSet<KBSymbol>();
            foreach (KBSymbol symbol in LHS.GetSymbols())
                symbols.Add(symbol);
            foreach (KBSymbol symbol in RHS.GetSymbols())
                symbols.Add(symbol);
            return symbols;
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { LHS, RHS };
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = LHS.SolveAssertion(kb, this) + RHS.SolveAssertion(kb, this);
            if (!Solved)
            {
                KBSolveType type = parent as object != null ? parent.CanSolveForChild(kb, this) : KBSolveType.NoSolution;
                if (type == KBSolveType.NoSolution || type == KBSolveType.Other)
                {
                    // attempt to solve from children.
                    if (LHS.Solved && RHS.Solved)
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
            return $"({LHS} + {RHS})";
        }
    }
}
