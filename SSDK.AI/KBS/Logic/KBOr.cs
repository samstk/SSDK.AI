using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// A logical connective that is asserted true when any inner sentences are also asserted true.
    /// </summary>
    public class KBOr : KBFactor
    {
        /// <summary>
        /// Gets the symbol or sentence that at least one must be true for this to be true.
        /// </summary>
        public KBFactor[] Sentences { get; private set; }
        public KBOr(params KBFactor[] sentences)
        {
            Sentences = sentences;
            if (sentences.Length == 0)
                throw new Exception("At least one logical sentence must be included in an OR connective.");
        }

        public override KBFactor HasConflict()
        {
            if(Solved && !Assertion)
            {
                foreach(KBFactor factor in Sentences)
                {
                    KBFactor conflict = factor.HasConflict();
                    if (conflict as object != null) return conflict;
                    if(factor.Solved && factor.Holds())
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
                if (Sentences[i].Holds())
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            if (Sentences.Length > 1)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("(");
                builder.Append(Sentences[0]);
                for (int i = 1; i < Sentences.Length; i++)
                {
                    builder.Append($" or {Sentences[i]}");
                }
                builder.Append(")");
                return builder.ToString();
            }
            else return Sentences[0].ToString();
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> symbols = new HashSet<KBSymbol>();
            foreach(KBFactor sentence in Sentences)
            {
                HashSet<KBSymbol> sentenceSymbols = sentence.GetSymbols();
                foreach(KBSymbol symbol in sentenceSymbols)
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
            if (!Assertion) return KBSolveType.SolveFalse; // All children in AND (a1, a2, ...) are false.

            return KBSolveType.NoSolution; // No need to determine other children, as next iterations may reveal them.
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = 0;
            foreach(KBFactor sentence in Sentences)
            {
                changes += sentence.SolveAssertion(kb, this);
            }


            if (!Solved)
            {
                KBSolveType type = parent as object == null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);
                if (type == KBSolveType.NoSolution || type == KBSolveType.Other)
                {
                    // Solve from child
                    foreach(KBFactor sentence in Sentences)
                    {
                        if(sentence.Solved && sentence.Assertion)
                        {
                            type = KBSolveType.SolveTrue;
                            break;
                        }
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
        public override KBFactor Simplify()
        {
            for(int i = 0; i<Sentences.Length; i++)
            {
                Sentences[i] = Sentences[i].Simplify();
            }
            if (Sentences.Length > 1) return this;
            return Sentences[0];
        }
    }
}
