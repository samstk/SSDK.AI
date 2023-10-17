using SSDK.AI.KBS.Logic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Channels;

namespace SSDK.AI.KBS
{
    /// <summary>
    /// A KB Symbol which identifies a piece of information.
    /// An inverse symbol should not appear on an logical sentence, instead
    /// the KBNot should be used.
    /// </summary>
    public class KBSymbol : KBVariable
    {
        /// <summary>
        /// An identifier for the symbol
        /// </summary>
        public string ID;

        /// <summary>
        /// A unique identifier for the symbol.
        /// </summary>
        public int UniqueID = -1;

        /// <summary>
        /// If true, then the symbol has been added to a KB.
        /// </summary>
        private bool _AddedToKB = false;

        /// <summary>
        /// A dictionary of relation types that might occur from this class.
        /// </summary>
        public Dictionary<KBSymbol, KBWrappedSymbol> RelationTypes = new Dictionary<KBSymbol, KBWrappedSymbol>();

        /// <summary>
        /// Creates a relation from this class to a symbol.
        /// For example if this class is 'if', then 
        /// this function creates a relation if(to)
        /// </summary>
        /// <returns>the wrapped symbol for the relation</returns>
        public KBWrappedSymbol CreateRelation(KBSymbol to)
        {
            if (RelationTypes.ContainsKey(to))
                return RelationTypes[to];

            KBWrappedSymbol symbol = new KBWrappedSymbol(this, to);
            RelationTypes.Add(to, symbol);
            return symbol;
        }

        /// <summary>
        /// Creates or sets a property onto this symbol.
        /// </summary>
        /// <param name="prop">property symbol</param>
        /// <param name="value">property value symbol</param>
        public void SetProperty(KBSymbol prop, KBSymbol value)
        {
            if (Properties.ContainsKey(prop))
                Properties[prop] = value;
            else Properties.Add(prop, value);
        }

        /// <summary>
        /// Returns true if the property with the given value exists
        /// on this symbol.
        /// </summary>
        /// <param name="prop">property symbol</param>
        /// <param name="value">property value symbol</param>
        public bool HasProperty(KBSymbol prop, KBSymbol value)
        {
            if (Properties.ContainsKey(prop))
                return Properties[prop].Equals(value);
            return Relations.Any((rel) => rel.HasProperty(prop, value));
        }

        /// <summary>
        /// Gets the inverse symbol if created (else use GetInverse)
        /// </summary>
        public KBSymbol Inverse { get; private set; }

        /// <summary>
        /// If true, then this symbol is an inverse symbol.
        /// </summary>
        public bool IsInverse { get; internal set; }

        /// <summary>
        /// Creates an symbol that is not linked to any KB.
        /// </summary>
        /// <param name="id"></param>
        public KBSymbol(string id)
        {
            ID = id;
        }

        public KBSymbol(KB kb, string id)
        {
            ID = id;
            AddToKB(kb);
        }

        /// <summary>
        /// Adds the symbol to the given knowledge base
        /// </summary>
        /// <param name="kb">knowledge base</param>
        internal void AddToKB(KB kb)
        {
            if (_AddedToKB) throw new Exception("Added to second KB");

            UniqueID = kb.GetNextSymbolID();
            kb.ExistingSymbols.Add(ID, this);
            _AddedToKB = true;
        }

        internal KBSymbol() { }

        /// <summary>
        /// If true, then the symbol is used as a class symbol.
        /// </summary>
        public bool IsRelationalSymbol { get; internal set; } = false;


        public override bool Holds()
        {
            return Solved && Assertion || HasAltAssertion;
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is KBSymbol && ((KBSymbol)obj).UniqueID == UniqueID)
            {
                KBSymbol other = (KBSymbol)obj;
                return other.UniqueID == UniqueID && UniqueID != -1;
            }

            if (obj is KBFactor && Solved)
            {
                KBFactor factor = (KBFactor)obj;
                return factor.Solved && factor.Calculate().Equals(this.Calculate());
            }

            return false;
        }

        public override string ToString()
        {
            return ID;
        }
        public string ToStringWithProperties(bool bracketsOnAssertValue = true)
        {
            string propStr = "";
            List<(KBSymbol, KBFactor)> props = GetProperties();
            if (props.Count == 0) return ToString(true, bracketsOnAssertValue);
            foreach ((KBSymbol key, KBFactor val) in props) {
                if (propStr.Length > 0)
                {
                    propStr += ", ";
                }
                propStr += $"{key}={val}";
            }
            return propStr.Length > 0 ? $"{ID} <{propStr}>" : ID;
        }
        public string ToClassString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ID);
            if (Relations.Count > 0)
            {
                builder.Append("[");
                bool added = false;
                foreach (KBWrappedSymbol cl in Relations)
                {
                    if (added)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(cl);
                    added = true;
                }
                builder.Append("]");
            }
            return builder.ToString();
        }



        public override HashSet<KBSymbol> GetSymbols()
        {
            return new HashSet<KBSymbol>() { this };
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>();
        }
        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            return KBSolveType.Other; // There is no child
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            if (Solved) return 0;

            if (!Solved)
            {
                KBSolveType type = parent as object == null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);
                if (type == KBSolveType.SolveTrue)
                {
                    SolveAssertTrue(kb); return 1;
                }
                else if (type == KBSolveType.SolveFalse)
                {
                    SolveAssertFalse(kb); return 1;
                }
                else if (type == KBSolveType.SolveArithmetic)
                {
                    SolveAssertOther(kb, parent.GetAltSolutionForChild(kb, this)); return 1;
                }
            }

            return 0;
        }
        public override void ResetSolution()
        {
            Relations.Clear();
            base.ResetSolution();
        }

        /// <summary>
        /// Creates the inverse symbol (not symbol)
        /// Used in classifications (definitely not symbol).
        /// </summary>
        /// <param name="kb">knowledge base</param>
        /// <returns>the inverse symbol</returns>
        public KBSymbol GetInverse(KB kb)
        {
            if (Inverse.Equals(null))
            {
                Inverse = new KBSymbol(kb, "~" + ID);
                Inverse.Inverse = this;
                Inverse.IsInverse = true;
            }
            return Inverse;
        }

        public static implicit operator KBSymbol(string id) => new KBSymbol(id);
        public static implicit operator KBSymbol((KB, string) args) {
            return new KBSymbol(args.Item1, args.Item2);
        }
        
        /// <summary>
        /// Gets a symbol relations of a class to another symbol.
        /// </summary>
        public KBSymbolRelation this[KBSymbol @class, KBSymbol to]
        {
            get
            {
                return new KBSymbolRelation(this, @class, to);
            }
        } 
    }
}