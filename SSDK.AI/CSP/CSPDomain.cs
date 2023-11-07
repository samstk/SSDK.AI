using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.CSP
{
    /// <summary>
    /// Represents a domain within a CSP problem.
    /// </summary>
    public sealed class CSPDomain : IEnumerable
    {
        #region Properties & Fields
        /// <summary>
        /// Gets or sets the possible values within the domain
        /// </summary>
        public HashSet<object> Values { get; private set; }

        public int Count
        {
            get
            {
                return Values.Count;
            }
        }

        #endregion

        /// <summary>
        /// Creates a domain with the given values.
        /// </summary>
        /// <param name="values">the set of values that this domain contains</param>
        public CSPDomain(HashSet<object> values)
        {
            Values = values;
        }

        /// <summary>
        /// Sets the domain to a single value
        /// </summary>
        /// <param name="obj">the singular possible value in this domain</param>
        public void Set(object obj)
        {
            Values.Clear();
            Values.Add(obj);
        }

        /// <summary>
        /// Adds a value to the domain
        /// </summary>
        /// <param name="obj">the value to add</param>
        public void Add(object obj)
        {
            Values.Add(obj);
        }

        /// <summary>
        /// Removes the value to the domain
        /// </summary>
        /// <param name="obj">the value to remove</param>
        public void Remove(object obj)
        {
            Values.Remove(obj);
        }

        /// <summary>
        /// Gets the value at the given index of the set
        /// </summary>
        /// <param name="index">the index to retrieve the value from</param>
        /// <returns></returns>
        public object ValueAt(int index)
        {
            return Values.ElementAt(index);
        }

        /// <summary>
        /// Tests whether the object is within the domain.
        /// </summary>
        /// <param name="obj">the value to test</param>
        /// <returns>true if the domain contains the object</returns>
        public bool Contains(object obj)
        {
            return Values.Contains(obj);
        }


        /// <summary>
        /// Gets the first value in the set enumerator.
        /// (accurate if count = 1)
        /// </summary>
        /// <returns>the first value in the set enumerator</returns>
        public object FirstValue()
        {
            return Values.First();
        }

        public IEnumerator GetEnumerator()
        {
            return Values.GetEnumerator();
        }
       
        public static implicit operator CSPDomain(HashSet<object> values) => new CSPDomain(values);
        public static implicit operator CSPDomain(object[] values)
        {
            HashSet<object> set = new HashSet<object>();
            foreach(object obj in values)
            {
                set.Add(obj);
            }
            return set;
        }

        /// <summary>
        /// Clones the domain into a new instance
        /// </summary>
        /// <returns>a new domain, which is an exact copy of this</returns>
        public CSPDomain Clone()
        {
            HashSet<object> values = new HashSet<object>();
            foreach (object obj in Values)
                values.Add(obj);
            return values;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            bool next = false;
            foreach(object obj in Values)
            {
                if (next)
                    sb.Append(", ");
                sb.Append($"{obj}");
                next = true;
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
