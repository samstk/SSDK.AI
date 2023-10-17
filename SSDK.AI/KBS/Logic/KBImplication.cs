using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// A logical connective that is asserted true on P->Q when P is false, or Q.
    /// When using probabilities
    /// </summary>
    public class KBImplication : KBFactor
    {
        /// <summary>
        /// Gets the symbol or logical sentence that implies a certain assertion.
        /// </summary>
        public KBFactor Condition { get; private set; }

        /// <summary>
        /// Gets the symbol or logical sentence that is implied by the condition.
        /// </summary>
        public KBFactor Implication { get; private set; }

        public KBImplication(KBFactor condition,KBFactor implication)
        {
            Condition = condition;
            Implication = implication;
        }
        public override bool Holds()
        {
            return !Condition.Holds() || Implication.Holds();
        }


        public override KBFactor HasConflict()
        {
            KBFactor r = null;
            if ((r = Condition.HasConflict()) as object != null) return r;
            if ((r = Implication.HasConflict()) as object != null) return r;

            return Solved && Assertion && !Holds() ? this : null;
        }

        public override string ToString()
        {
            return $"({Condition} -> {Implication})";
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> condSymbols = Condition.GetSymbols();
            HashSet<KBSymbol> impSymbols = Implication.GetSymbols();

            HashSet<KBSymbol> set = new HashSet<KBSymbol>();
            foreach (KBSymbol symbol in condSymbols) set.Add(symbol);
            foreach (KBSymbol symbol in impSymbols) set.Add(symbol);
            return set;
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { Condition, Implication };
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            if (child.Equals(Condition)) return KBSolveType.NoSolution; // Cannot solve for condition, p -> q only affects q on different values of p
            return Solved && Condition.Solved && Condition.Holds() ? KBSolveType.SolveTrue : KBSolveType.NoSolution;
        }
        // Can only solve children if condition has been solved and it is true (i.e. implication is true)

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = Condition.SolveAssertion(kb, this) + Implication.SolveAssertion(kb, this);

            if (!Solved)
            {
                KBSolveType type = parent as object== null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);
          
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
            Condition = Condition.Simplify();
            Implication = Implication.Simplify();
            return this;
        }
    }
}
