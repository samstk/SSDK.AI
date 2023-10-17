using SSDK.AI.KBS.Arithmetic;
using SSDK.Core.Structures.Primitive;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS
{
    public class KBNumericSymbol : KBSymbol
    {
        public UncontrolledNumber Number;
        public KBNumericSymbol(double number)
        {
            Number = new UncontrolledNumber(number);
        }

        public KBNumericSymbol(UncontrolledNumber number)
        {
           Number = number;
        }


        public override KBFactor Calculate()
        {
            return this;
        }

        public override bool Holds()
        {
            return true;
        }
        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            if (!Solved)
            {
                Solved = true;
                AltAssertion = this; // Always solved
            }
            return 0;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is KBNumericSymbol)) return false;
            return Number == (obj as KBNumericSymbol).Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override string ToString()
        {
            return Number.ToString();
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>();
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            return new HashSet<KBSymbol>();
        }

        public override KBFactor Apply(char op, params KBFactor[] terms)
        {
            switch(op)
            {
                case '-':
                    if(terms.Length == 0)
                    {
                        return new KBNumericSymbol(-Number);
                    }
                    else if(terms.Length == 1 && terms[0] is KBNumericSymbol)
                    {
                        return new KBNumericSymbol(Number - ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;
                case '+':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBNumericSymbol(Number + ((KBNumericSymbol)terms[0]).Number);
                    }
                    else if (terms[0] is KBStringLiteral)
                    {
                        return new KBStringLiteral(Number + ((KBStringLiteral)terms[0]).Text);
                    }
                    return Null;

                case '*':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBNumericSymbol(Number * ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;

                case '/':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBNumericSymbol(Number / ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;

                case '%':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBNumericSymbol(Number % ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;

                case '=':
                    // Assume terms is 1 (other).
                    return new KBBooleanSymbol(this.Equals(terms[0]));

                case '!':
                    return new KBBooleanSymbol(this.Equals(terms[0]));

                case '^':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBNumericSymbol(Number ^ ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;

                case '|':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBNumericSymbol(Number | ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;


                case '&':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBNumericSymbol(Number & ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;

                case '<':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBBooleanSymbol(Number < ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;

                case 'L':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBBooleanSymbol(Number <= ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;

                case '>':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBBooleanSymbol(Number > ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;

                case 'G':
                    // Assume terms is 1 (other).
                    if (terms[0] is KBNumericSymbol)
                    {
                        return new KBBooleanSymbol(Number >= ((KBNumericSymbol)terms[0]).Number);
                    }
                    return Null;
            }
            return base.Apply(op, terms);
        }
        public static implicit operator KBNumericSymbol(double number) => new KBNumericSymbol(number);
        public static implicit operator KBNumericSymbol(UncontrolledNumber number) => new KBNumericSymbol(number);
    }
}
