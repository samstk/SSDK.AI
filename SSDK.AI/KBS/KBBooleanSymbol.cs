using SSDK.Core.Structures.Primitive;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS
{
    /// <summary>
    /// A boolean value, generally defined as a result of 
    /// an arithmetic operation (==, or !=)
    /// </summary>
    public class KBBooleanSymbol : KBSymbol
    {
        public bool Bit = false;
        public KBBooleanSymbol(bool bit)
        {
            Bit = bit;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is KBBooleanSymbol)) return false;
            return Bit == (obj as KBBooleanSymbol).Bit;
        }

        public override bool Holds()
        {
            return true;
        }

        public override KBFactor Calculate()
        {
            return this;
        }


        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            if(!Solved)
            {
                Solved = true;
                Assertion = Bit;
            }
            return base.SolveAssertion(kb, parent);
        }

        public override int GetHashCode()
        {
            return Bit.GetHashCode();
        }

        public override string ToString()
        {
            return Bit.ToString();
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>();
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            return new HashSet<KBSymbol>();
        }

        public static implicit operator KBBooleanSymbol(bool boolean) => new KBBooleanSymbol(boolean);
    }
}
