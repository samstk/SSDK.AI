using SSDK.AI.KBS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Arithmetic
{
    /// <summary>
    /// Represents a equals operation on a constant (e.g. 1 >= 2)
    /// Not to be confused with the logical agreement, although this
    /// can be considered a logical connective to be used for arithmetic purposes
    /// </summary>
    public sealed class KBGreaterThanOrEquals : KBFactor
    {
        /// <summary>
        /// The left-side term which is compared
        /// </summary>
        public KBFactor LHS { get; private set; }

        /// <summary>
        /// The right-side term which is compared.
        /// </summary>
        public KBFactor RHS { get; private set; }

        public KBGreaterThanOrEquals(KBFactor lhs, KBFactor rhs)
        {
            LHS = lhs;
            RHS = rhs;
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            // Cannot solve exactly.
            return KBSolveType.NoSolution;
        }

        public override KBFactor GetAltSolutionForChild(KB kb, KBFactor child)
        {
            return null;
        }

        public override KBFactor Calculate()
        {
            return LHS.Calculate().Apply('G', RHS.Calculate());
        }

        public override KBFactor HasConflict()
        {
            KBFactor left = LHS.Calculate();
            KBFactor right = RHS.Calculate();
            return Solved && Assertion && left as object != null && right as object != null && left.Apply('G', right).IsBooleanFalse ? this : null;
        }

        public override bool Holds()
        {
            return Solved && Assertion; // Exchangable with logical operations.
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
                KBSolveType type = parent as object == null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);
                if (type == KBSolveType.NoSolution || type == KBSolveType.Other)
                {
                    if (LHS.Solved && RHS.Solved)
                    {
                        KBFactor left = LHS.Calculate();
                        KBFactor right = RHS.Calculate();
                        type = left.Apply('G', right).IsBooleanTrue ? KBSolveType.SolveTrue : KBSolveType.SolveFalse;
                    }
                }
                if (type == KBSolveType.SolveTrue)
                {
                    SolveAssertTrue(kb); changes++;
                }
                else if (type == KBSolveType.SolveFalse)
                {
                    SolveAssertFalse(kb); changes++;
                }
            }
            return changes;
        }

        public override string ToString()
        {
            return $"({LHS} >= {RHS})";
        }
    }
}
