using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// A logical connective that is asserted true when all inner sentences are also asserted true.
    /// </summary>
    public class KBAnd : KBFactor
    {
        /// <summary>
        /// Gets the symbol or sentence that must all be true.
        /// </summary>
        public KBFactor[] Sentences { get; private set; }
        public KBAnd(params KBFactor[] sentences)
        {
            Sentences = sentences;
            if (sentences.Length == 0)
                throw new Exception("At least one logical sentence must be included in an AND connective.");
        }
        public override KBFactor HasConflict()
        {
            if (Solved && Assertion)
            {
                foreach (KBFactor factor in Sentences)
                {
                    KBFactor conflict = factor.HasConflict();
                    if (conflict as object != null) return conflict;
                    if (factor.Solved && !factor.Holds())
                    {
                        return factor;
                    }
                }
            }
            return null;
        }
        public override bool Holds()
        {
            for(int i = 0; i <Sentences.Length; i++)
            {
                if (!Sentences[i].Holds())
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            if (Sentences.Length > 1)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("(");
                builder.Append(Sentences[0]);
                for(int i = 1; i < Sentences.Length; i++)
                {
                    builder.Append($" and {Sentences[i]}");
                }
                builder.Append(")");
                return builder.ToString();
            }
            else return Sentences[0].ToString();
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> symbols = new HashSet<KBSymbol>();
            foreach (KBFactor sentence in Sentences)
            {
                HashSet<KBSymbol> sentenceSymbols = sentence.GetSymbols();
                foreach (KBSymbol symbol in sentenceSymbols)
                {
                    symbols.Add(symbol);
                }
            }
            return symbols;
        }

        public override List<KBFactor> GetChildren()
        {
            return Sentences.ToList();
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            if (!Solved) return KBSolveType.NoSolution;
            if (Assertion) return KBSolveType.SolveTrue; // All children in AND (a1, a2, a3) are true.
            return KBSolveType.NoSolution; // Cannot determine other children.
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = 0;
            foreach (KBFactor sentence in Sentences)
            {
                changes += sentence.SolveAssertion(kb, this);
            }

            if (!Solved)
            {
                KBSolveType type = parent as object == null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);
                if (type == KBSolveType.NoSolution || type == KBSolveType.Other)
                {
                    // Solve from child
                    foreach (KBFactor sentence in Sentences)
                    {
                        if (!sentence.Solved || !sentence.Assertion)
                        {
                            return changes;
                        }
                    }
                    type = KBSolveType.SolveTrue;
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

        public override KBFactor Simplify()
        {
            for (int i = 0; i < Sentences.Length; i++)
            {
                Sentences[i] = Sentences[i].Simplify();
            }
            if (Sentences.Length > 1) return this;
            return Sentences[0];
        }
    }
}
