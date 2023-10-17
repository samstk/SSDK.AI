using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// A Logical Connective which identifies a piece of information about a specific relation 
    /// e.g. a symbol that is(player) has sym health that is symbol (numeral 50)
    /// </summary>
    public class KBPropertyDeclaration : KBFactor
    {
        /// <summary>
        /// The relation or symbol that this property is declared to.
        /// </summary>
        public KBVariable Relation { get; private set; }

        /// <summary>
        /// The property symbol that is defined.
        /// </summary>
        public KBSymbol Property { get; private set; }

        /// <summary>
        /// The property value that is defined.
        /// </summary>
        public KBFactor Value { get; private set; }

        public KBPropertyDeclaration(KBWrappedSymbol relation, KBSymbol prop, KBFactor val)
        {
            Relation = relation;
            Property = prop;
            Value = val;
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> symbols = new HashSet<KBSymbol>() { Property };
            return symbols;
        }
        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { Relation, Property, Value };
        }

        public override bool Holds()
        {
            return Relation.Properties[Property].Equals(Value);            
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            // No child solving.
            return KBSolveType.NoSolution;
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = 0;

            if (!Solved)
            {
                // We can solve a symbol classification when either ~class or class appears in the about list.
                KBSolveType solveType = parent as object == null ? KBSolveType.SolveTrue
                    : parent.CanSolveForChild(kb, this);

                if (solveType == KBSolveType.NoSolution)
                {
                    // Perhaps the property already exists on the target.
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
            // The property must exist on the target.
            Relation.SetProperty(Property, Value);
            base.SolveAssertTrue(kb);
        }

        public override void SolveAssertFalse(KB kb)
        {
            // The property does not exist on the target, so just ignore.
            base.SolveAssertFalse(kb);
        }
        public override string ToString()
        { 
            if(Relation is KBWrappedSymbol)
            {
                return $"For any symbol::{Relation}, it has {Property} : {Value}";
            }
            else
            {
                return $"{Relation} has {Property} : {Value}";
            }
        }

    }
}
