using SSDK.AI.KBS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SSDK.AI.KBS
{
    /// <summary>
    /// A KB Symbol which identifies a piece of information.
    /// An inverse symbol should not appear on an logical sentence, instead
    /// the KBNot should be used.
    /// 
    /// A wrapped symbol is a symbol that joins two symbols together.
    /// E.g. is(animal), where is the outer symbol, and animal is the inner symbol.
    /// </summary>
    public class KBWrappedSymbol : KBVariable
    {
        /// <summary>
        /// The outer symbol (relation type)
        /// </summary>
        public KBSymbol Outer { get; private set; }
        /// <summary>
        /// The inner symbol (to)
        /// </summary>
        public KBSymbol Inner { get; private set; }

        /// <summary>
        /// If true, then this symbol is an inverse symbol.
        /// </summary>
        public bool IsInverse { get; internal set; }

        /// <summary>
        /// Gets the inverse symbol if created (else use GetInverse)
        /// </summary>
        public KBWrappedSymbol Inverse { get; private set; }
        

        public KBWrappedSymbol(KBSymbol outer, KBSymbol inner)
        {
            Outer = outer;
            Inner = inner;
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            // Never solved.
            return KBSolveType.NoSolution;
        }
        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            // Never solved
            return 0;
        }
        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { Outer, Inner };
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            return new HashSet<KBSymbol>() { Outer, Inner };
        }

        public override bool Holds()
        {
            // Never called
            return true;
        }

        public override bool InverseHolds()
        {
            // Never called
            return true;
        }
        public override bool Equals(object? obj)
        {
            if (!(obj is KBWrappedSymbol)) return false;
            KBWrappedSymbol other = (KBWrappedSymbol)obj;
            return other.Outer.Equals(Outer) && other.Inner.Equals(Inner) && other.IsInverse == IsInverse;
        }
        public override int GetHashCode()
        {
            return (Outer.GetHashCode() + Inner.GetHashCode()) * (IsInverse ? -1 : 1);
        }
        public override string ToString()
        {
            return (IsInverse ? "~" : "") + $"{Outer}({Inner})";
        }
        /// <summary>
         /// Creates the inverse symbol (not wrapped symbol)
         /// Used in classifications (definitely not symbol).
         /// </summary>
         /// <param name="kb">knowledge base</param>
         /// <returns>the inverse symbol</returns>
        public KBWrappedSymbol GetInverse()
        {
            if (Inverse as object == null)
            {
                Inverse = new KBWrappedSymbol(Outer, Inner);
                Inverse.Inverse = this;
                Inverse.IsInverse = true;
            }
            return Inverse;
        }
    }
}
