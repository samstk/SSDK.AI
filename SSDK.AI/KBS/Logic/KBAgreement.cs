using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// A logical connective that is asserted true on P and Q match.
    /// </summary>
    public class KBAgreement : KBFactor
    {
        /// <summary>
        /// Gets the symbol or logical sentence of P (left-side).
        /// </summary>
        public KBFactor P { get; private set; }

        /// <summary>
        /// Gets the symbol or logical sentence of Q (right-side).
        /// </summary>
        public KBFactor Q { get; private set; }

        public KBAgreement(KBFactor p, KBFactor q)
        {
            P = p;
            Q = q;
        }
        public override bool Holds()
        {
            return P.Holds() == Q.Holds();
        }

        public override KBFactor HasConflict()
        {
            KBFactor r = null;
            if ((r = P.HasConflict()) as object != null) return r;
            if ((r = Q.HasConflict()) as object != null) return r;

            return Solved && Assertion && !Holds() ? this : null;
        }

        public override string ToString()
        {
            return $"({P} <-> {Q})";
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> condSymbols = P.GetSymbols();
            HashSet<KBSymbol> impSymbols = Q.GetSymbols();

            HashSet<KBSymbol> set = new HashSet<KBSymbol>();
            foreach (KBSymbol symbol in condSymbols) set.Add(symbol);
            foreach (KBSymbol symbol in impSymbols) set.Add(symbol);
            return set;
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { P, Q };
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            if (P.Solved && child.Equals(Q)) return P.Assertion ? KBSolveType.SolveTrue : KBSolveType.SolveFalse;
            if (Q.Solved && child.Equals(P)) return Q.Assertion ? KBSolveType.SolveTrue : KBSolveType.SolveFalse;
            return KBSolveType.NoSolution;
        }
        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = P.SolveAssertion(kb, this) + Q.SolveAssertion(kb, this);

            if (!Solved)
            {
                KBSolveType type = parent as object == null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);

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

        public override KBFactor Simplify()
        {
            P = P.Simplify();
            Q = Q.Simplify();
            return this;
        }
    }
}
