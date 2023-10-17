using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// A Logical Connective which identifies a piece of information about a symbol
    /// </summary>
    public class KBSymbolRelation : KBFactor
    {
        public KBSymbol About;
        public KBSymbol Class { get; private set; }
        public KBSymbol To { get; private set; }

        public KBWrappedSymbol WrappedSymbol { get; private set; }

        public KBSymbolRelation(KBSymbol about, KBSymbol @class, KBSymbol to)
        {
            About = about;
            Class = @class;
            Class.IsRelationalSymbol = true;
            To = to;
            WrappedSymbol = Class.CreateRelation(to); // By default create relation
        }


        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>(); // No children
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            return new HashSet<KBSymbol>() { About, Class, To };
        }

        public override bool Holds()
        {
            // In this mode, we must ensure KBSymbol has the given class.
            return About.Relations.Contains(WrappedSymbol);
        }

        public override KBFactor HasConflict()
        {
            return Solved && Assertion && !About.Relations.Contains(WrappedSymbol) || About.Relations.Contains(WrappedSymbol.GetInverse()) ? About : null;
        }

        public override bool InverseHolds()
        {
            // In this mode, we must ensure KBSymbol never has the inverse of the class.
            return About.Relations.Contains(WrappedSymbol.GetInverse());
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            return KBSolveType.Other; // Classifications are handled differently than boolean assertions.
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = About.SolveAssertion(kb, this); // Class cannot be solved.

            if(!Solved)
            {
                // We can solve a symbol classification when either ~class or class appears in the about list.
                KBSolveType solveType = parent as object == null ? KBSolveType.SolveTrue 
                    : parent.CanSolveForChild(kb, this);

                if (solveType == KBSolveType.NoSolution)
                {
                    // Perhaps the relation already exists on the target.
                    if (About.Relations.Contains(WrappedSymbol))
                    {
                        SolveAssertTrue(kb); changes++;
                    }
                    else if (About.Relations.Contains(WrappedSymbol.GetInverse()))
                    {
                        SolveAssertFalse(kb); changes++;
                    }
                }
                else if (solveType == KBSolveType.SolveTrue)
                {
                    SolveAssertTrue(kb); changes++;
                }
                else if (solveType == KBSolveType.SolveFalse)
                {
                    SolveAssertFalse(kb); changes++;
                }
            }

            return changes;
        }

        public override void SolveAssertTrue(KB kb)
        {
            About.Solved = true;
            About.IsClass = true;
            About.Relations.Add(Class.CreateRelation(To));
            base.SolveAssertTrue(kb);
        }

        public override void SolveAssertFalse(KB kb)
        {
            About.Solved = true;
            About.IsClass = true;
            About.Relations.Add(Class.CreateRelation(To).GetInverse());
            base.SolveAssertFalse(kb);
        }

        public override string ToString()
        {
            return $"{About}::{Class}({To})";
        }
    }
}
