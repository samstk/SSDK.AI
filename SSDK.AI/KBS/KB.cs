using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS
{
    /// <summary>
    /// Represents the knowledge base
    /// </summary>
    public sealed class KB
    {

        /// <summary>
        /// The next symbol id that will be returned on GetNextSymbolID
        /// </summary>
        private int _NextSymbolID = 0;

        /// <summary>
        /// Gets the next symbol ID, and increments 1 each time this function is called.
        /// </summary>
        /// <returns>the next symbol id</returns>
        public int GetNextSymbolID()
        {
            return _NextSymbolID++;
        }

        public KBSymbol Is { get; private set; }

        public KB()
        {
            Is = new KBSymbol(this, "is");
        }

        /// <summary>
        /// A list of assertions that should definitely hold true regardless of the world state.
        /// </summary>
        public List<KBFactor> Assertions { get; private set; } = new List<KBFactor>();

        /// <summary>
        /// A list of assertions passed by a query that should definitely hold true regardless of the world state. This list
        /// is immediately reset after solving.
        /// </summary>
        public List<KBFactor> QueryAssertions { get; private set; } = new List<KBFactor>();

        /// <summary>
        /// A lits of queries that is required to be solved or was solved.
        /// </summary>
        public List<KBFactor> Queries { get; private set; } = new  List<KBFactor> { };

        /// <summary>
        /// If true, the KB has been solved at least once.
        /// </summary>
        public bool Solved { get; private set; } = false;

        /// <summary>
        /// Returns the first conflict within the assertions found.
        /// Only applies to solved assertions.
        /// </summary>
        /// <returns>a tuple of the first and second factors in a conflict, with their exception</returns>
        public (KBFactor, KBFactor, Exception) HasConflict()
        {
            if(!Solved)
            {
                Solve();
            }
            for (int i = 0; i < Assertions.Count; i++)
            {
                HashSet<KBSymbol> syms = Assertions[i].GetSymbols();
                bool allSolved = syms.All((sym) => sym.Solved);
                List<KBFactor> children = Assertions[i].GetChildren();
                KBFactor conflict = null;
                foreach (KBFactor factor in children)
                {
                    conflict = (factor.Solved ? factor.HasConflict() : null);
                    if (factor.Solved && conflict as object != null || allSolved && !factor.Solved)
                    {
                        string symbols = "";
                        foreach(KBSymbol sym in Assertions[i].GetSymbols())
                        {
                            if (symbols.Length > 0)
                                symbols += ", ";
                            if (!sym.Solved)
                                symbols += sym.ToString(false, false) + " is unsolvable";
                            else
                                symbols += sym.ToStringWithProperties(false);
                        }
                        return (Assertions[i], conflict, new Exception($"{conflict} in assertion {Assertions[i]} does not hold true where {symbols}, indicating a conflict with the assertions."));
                    }
                }

                if ((conflict = Assertions[i].HasConflict()) as object != null)
                {
                    string symbols = "";
                    foreach (KBSymbol sym in Assertions[i].GetSymbols())
                    {
                        if (symbols.Length > 0)
                            symbols += ", ";
                        if (!sym.Solved)
                            symbols += sym.ToString(false, false) + " is unsolvable";
                        else
                        symbols += sym.ToStringWithProperties(false);
                    }
                    return (Assertions[i], Assertions[i], new Exception($"Assertion {Assertions[i]} does not hold true where {symbols}, indicating a conflict with the assertions."));
                }
            }
            return (null, null, null);
        }

        /// <summary>
        /// The dictionary of existing symbols, that contains all symbols ever created.
        /// </summary>
        public Dictionary<string, KBSymbol> ExistingSymbols { get; internal set; } = new Dictionary<string, KBSymbol>();

        /// <summary>
        /// Gets the symbol from the existing symbols dictionary using the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public KBSymbol GetSymbol(string id)
        {
            if (ExistingSymbols.ContainsKey(id))
                return ExistingSymbols[id];
            return null;
        }
        /// <summary>
        /// Solves the current knowledge base's assertions. Clear queries (and query assertions) before-hand.
        /// You will need to assert the necessary information for it to solve. <br/>
        /// e.g. an undefined symbol (symbol that appears but is not asserted to be either true or false),
        ///      cannot be used to solve any other symbol.
        /// <br/>
        /// In order to solve for probabilities, logic must be in a simplified form (e.g. p ^ p -> p, p ^ !p -> false)
        /// </summary>
        public void Solve()
        {
            Solved = false;
            // Unset assertions except direct assertions
            Assertions.ForEach((sentence) =>
            {
                sentence.ResetSolution();
            });
            QueryAssertions.ForEach((sentence) =>
            {
                sentence.ResetSolution();
            });

            foreach(KBSymbol symbol in ExistingSymbols.Values)
            {
                symbol.ResetSolution();
            }

            // Simplify all assertions (only removes invalid and/ors)
            for (int i = 0; i<Assertions.Count; i++)
            {
                Assertions[i] = Assertions[i].Simplify();
            }

            for (int i = 0; i < QueryAssertions.Count; i++)
            {
                QueryAssertions[i] = QueryAssertions[i].Simplify();
            }


            int changes = -1;
            while (changes != 0)
            {
                changes = 0;
                Assertions.ForEach((sentence) =>
                {
                    changes += sentence.SolveAssertion(this, null);
                });
                QueryAssertions.ForEach((sentence) =>
                {
                    changes += sentence.SolveAssertion(this, null);
                });
            }

            // Simplify all assertions (removes unnecessary relations)
            for (int i = 0; i < Assertions.Count; i++)
            {
                Assertions[i] = Assertions[i].Simplify();
            }
            for (int i = 0; i < QueryAssertions.Count; i++)
            {
                QueryAssertions[i] = QueryAssertions[i].Simplify();
            }

            QueryAssertions.Clear();
            Solved = true;
        }

        /// <summary>
        /// Beginning of a query on a knowledge base
        /// </summary>
        /// <param name="queryAssertions">a new set of assertions relevant to the query</param>
        /// <returns>this knowledge base</returns>
        public KB If(params KBFactor[] queryAssertions)
        {
            QueryAssertions = queryAssertions.ToList();
            return this;
        }

        /// <summary>
        /// Resolves the knowledge base, but additional solves the query based on given assertions.
        /// </summary>
        /// <param name="sentencesToQuery">the sentences to query (returned as result)</param>
        /// <returns>sentencesToQuery</returns>
        public KBFactor[] Query(params KBFactor[] sentencesToQuery)
        {
            Queries = sentencesToQuery.ToList();
            Solve();
            for (int i = 0; i<sentencesToQuery.Length; i++)
            {
                KBFactor sentence = sentencesToQuery[i];
                KBFactor solved = Queries[i];

                // Carry direct result
                sentence.Solved = solved.Solved;
                sentence.Assertion = solved.Assertion;
                sentence.AltAssertion = solved.AltAssertion;
            }
            return sentencesToQuery;
        }

        /// <summary>
        /// Gets a list of symbols mentioned in the assertions.
        /// </summary>
        /// <returns></returns>
        public HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> symbols = new HashSet<KBSymbol>();
            foreach(HashSet<KBSymbol> symbolList in Assertions.Select((sentence) => sentence.GetSymbols()))
            {
                foreach(KBSymbol symbol in symbolList)
                {
                    symbols.Add(symbol);
                }
            }
            return symbols;
        }

        public override string ToString()
        {
            return ToString(true, false);
        }

        public string ToString(bool includeSolution=true, bool showSolvedOnly=false)
        {
            StringBuilder builder = new StringBuilder();
            HashSet<KBSymbol> symbols = GetSymbols();

            // Create symbol list
            builder.Append("[SYMBOLS: ");
            if (symbols.Count > 0)
            {
                bool firstSymbol = true;
                foreach (KBSymbol symbol in symbols)
                {
                    if (symbol.IsRelationalSymbol || showSolvedOnly && !symbol.Solved) continue;

                    if (firstSymbol)
                    {
                        builder.Append(!includeSolution ? symbol.ToString() : symbol.ToStringWithProperties());
                        firstSymbol = false;
                    }
                    else
                    {
                        builder.Append(", " + (!includeSolution ? symbol.ToString() : symbol.ToStringWithProperties()));
                    }
                    if (includeSolution && symbol.Relations.Count > 0)
                    {
                        builder.Append(" [");
                        bool added = false;
                        foreach (KBWrappedSymbol cl in symbol.Relations)
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
                }

            }
            builder.Append("]\n");
            builder.Append("[ASSERTIONS]\n");
            Assertions.ForEach((sentence) =>
            {
                builder.AppendLine(sentence.ToString());
            });
            return builder.ToString();
        }
    }
}
