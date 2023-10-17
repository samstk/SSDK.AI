using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS
{

    /// <summary>
    /// Represents a sentence/symbol that may contain various properties or relations.
    /// Must be extended (currently KBSymbol and KBWrappedSymbol).
    /// </summary>
    public abstract class KBVariable : KBFactor
    {
        
        /// <summary>
        /// A property that exists within this symbol (if stored in a relation, then exists as a default backup property).
        /// </summary>
        public Dictionary<KBSymbol, KBFactor> Properties = new Dictionary<KBSymbol, KBFactor>();

        /// <summary>
        /// A set of classifications this symbol relates to.
        /// </summary>
        public HashSet<KBWrappedSymbol> Relations { get; private set; } = new HashSet<KBWrappedSymbol>();

        /// <summary>
        /// Creates or sets a property onto this symbol.
        /// </summary>
        /// <param name="prop">property symbol</param>
        /// <param name="value">property value symbol</param>
        public void SetProperty(KBSymbol prop, KBFactor value)
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
            return false;
        }

        public List<(KBSymbol, KBFactor)> GetProperties()
        {
            List<(KBSymbol, KBFactor)> props = new List<(KBSymbol, KBFactor)>();
            HashSet<KBSymbol> addedProps = new HashSet<KBSymbol>();
            foreach (KBSymbol key in Properties.Keys)
            {
                props.Add((key, Properties[key]));
                addedProps.Add(key);
            }
            foreach (KBWrappedSymbol rel in Relations)
            {
                foreach (KBSymbol key in rel.Properties.Keys)
                {
                    if (!addedProps.Contains(key))
                    {
                        addedProps.Add(key);
                        props.Add((key, rel.Properties[key]));
                    }
                }
            }
            return props;
        }
    }
}
