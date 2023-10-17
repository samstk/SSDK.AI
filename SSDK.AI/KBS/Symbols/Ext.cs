using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Symbols
{
    public static class SymbolExt
    {
        /// <summary>
        /// Selects a single symbol given a string id from symbols.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static KBSymbol SelectOne(this IEnumerable<KBSymbol> symbols, string id)
        {
            foreach(KBSymbol symbol in symbols)
            {
                if(symbol.ID == id)
                {
                    return symbol;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds all symbols in the array to the KB.
        /// </summary>
        /// <param name="symbols">the list of symbols to add to the knowledge base</param>
        /// <param name="kb">the knowledge base to add to</param>
        /// <returns>the unmodified list of symbols</returns>
        public static KBSymbol[] In(this KBSymbol[] symbols, KB kb)
        {
            foreach(KBSymbol symbol in symbols)
            {
                symbol.AddToKB(kb);
            }
            return symbols;
        }
    }
}
