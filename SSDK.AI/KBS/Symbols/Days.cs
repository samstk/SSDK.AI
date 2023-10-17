using SSDK.AI.KBS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Symbols
{
    public static class KBSymbolExt_Days
    {
        /// <summary>
        /// Creates symbols for the days (0-Sunday, 6-Monday).
        /// max must be more than min.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static KBSymbol[] CreateDaySymbols(this KB kb, int min, int max)
        {
            if (min < 0 || max < min || max >= 7) throw new Exception("Symbol Creation: Day min/max out of bounds.");
            KBSymbol[] symbols = new KBSymbol[max - min + 1];
            string[] names = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
            for(int i = 0; i<symbols.Length; i++)
            {
                symbols[i] = new KBSymbol(kb, names[i]);
            }
            return symbols;
        }

        /// <summary>
        /// Creates logical sentences for day classifications (e.g. yesterday of monday is sunday).
        /// </summary>
        /// <param name="daySymbols"></param>
        /// <returns></returns>
        public static List<KBFactor> CreateDayClassifications(this KB kb, KBSymbol[] daySymbols, KBSymbol today, KBSymbol yesterday, KBSymbol tomorrow)
        {
            List<KBFactor> sentences = new List<KBFactor>();
            for(int i = 0; i<daySymbols.Length; i++)
            {
                int yesterdayIndex = (i - 1);
                if (yesterdayIndex == -1) yesterdayIndex = 6;
                if(yesterdayIndex < daySymbols.Length && yesterday as object != null)
                {
                    sentences.Add(new KBAgreement(new KBSymbolRelation(today, kb.Is, daySymbols[i]), new KBSymbolRelation(yesterday, kb.Is, daySymbols[yesterdayIndex])));   
                }
                int tomorrowIndex = (i + 1);
                if (tomorrowIndex == 8) tomorrowIndex = 0;
                if(tomorrowIndex < daySymbols.Length && tomorrow as object  != null)
                {
                    sentences.Add(new KBAgreement(new KBSymbolRelation(today, kb.Is, daySymbols[i]), new KBSymbolRelation(tomorrow, kb.Is, daySymbols[tomorrowIndex])));
                }
            }
            return sentences;
        }
    }
}
