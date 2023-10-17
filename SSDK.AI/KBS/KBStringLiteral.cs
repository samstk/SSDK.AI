using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS
{
    public class KBStringLiteral : KBSymbol
    {
        public string Text = "";
        public KBStringLiteral(string text)
        {
            Text = text;
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            if(!Solved)
            {
                Solved = true;
                AltAssertion = this;
            }
            return 0;
        }
        public override bool Holds()
        {
            return true;
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is KBStringLiteral)) return false;
            return Text == (obj as KBStringLiteral).Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override string ToString()
        {
            return Text.ToString();
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>();
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            return new HashSet<KBSymbol>();
        }
    }
}
