using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// A logical connective that inverts the sentence wrapped.
    /// </summary>
    public class KBNot : KBFactor
    {
        /// <summary>
        /// Gets the symbol or sentence that if true, the assertion is false, and vice-versa.
        /// </summary>
        public KBFactor Sentence { get; private set; }
        public KBNot(KBFactor sentence)
        {
            Sentence = sentence;
        }
        public override bool Holds()
        {
            return !Sentence.Holds();
        }

        public override KBFactor HasConflict()
        {
            return Solved && Assertion && Sentence.Holds() ? Sentence : null;
        }

        public override string ToString()
        {
            return $"~{Sentence}";
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            return Sentence.GetSymbols();
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { Sentence };
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            return Solved ? Assertion ? KBSolveType.SolveFalse : KBSolveType.SolveTrue : KBSolveType.NoSolution;
        }
        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = Sentence.SolveAssertion(kb, this);

            if (!Solved)
            {
                KBSolveType type = parent as object == null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);
                if (type == KBSolveType.NoSolution || type == KBSolveType.Other)
                {
                    // Solve from child
                    if (Sentence.Solved)
                        type = Sentence.Assertion ? KBSolveType.SolveFalse : KBSolveType.SolveTrue;
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

        public override void SolveAssertTrue(KB kb)
        {
            // We can solve child if unsolved
            if (!Sentence.Solved) Sentence.SolveAssertFalse(kb);

            base.SolveAssertTrue(kb);
        }
        public override void SolveAssertFalse(KB kb)
        {// We can solve child if unsolved
            if (!Sentence.Solved) Sentence.SolveAssertTrue(kb);

            base.SolveAssertFalse(kb);
        }

        public override KBFactor Simplify()
        {
            Sentence = Sentence.Simplify();
            return this;
        }
    }
}
