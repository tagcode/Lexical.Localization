﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Simple immutable collection of plural rules.
    /// 
    /// Implements queryable, but does not cache indices.
    /// </summary>
    public class PluralRulesArray : IPluralRules, IPluralRulesEnumerable, IPluralRulesQueryable
    {
        /// <summary>
        /// Array of rules
        /// </summary>
        public readonly IPluralRule[] Rules;

        /// <summary>
        /// Create rules
        /// </summary>
        /// <param name="rules"></param>
        public PluralRulesArray(params IPluralRule[] rules)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        /// <summary>
        /// Create rules.
        /// </summary>
        /// <param name="rules"></param>
        public PluralRulesArray(IEnumerable<IPluralRule> rules)
        {
            Rules = (rules ?? throw new ArgumentNullException(nameof(rules))).ToArray();
        }

        /// <summary>
        /// Enumerate rules
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IPluralRule> GetEnumerator()
            => ((IEnumerable<IPluralRule>)Rules).GetEnumerator();

        /// <summary>
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <returns></returns>
        public virtual IPluralRulesEnumerable Query(PluralRuleInfo filterCriteria)
        {
            if (filterCriteria.Equals(PluralRuleInfo.Empty)) return this;

            int c = 0;
            foreach (var rule in this)
                if (filterCriteria.FilterMatch(rule.Info)) c++;
            IPluralRule[] result = new IPluralRule[c];

            int i = 0;
            foreach (var rule in this)
                if (filterCriteria.FilterMatch(rule.Info))
                    result[i++] = rule;

            return new PluralRulesArray(result);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)Rules).GetEnumerator();
    }

    /// <summary>
    /// Simple mutable collection of plural rules.
    /// 
    /// Implements queryable, but does not cache indices.
    /// </summary>
    public class PluralRulesList : List<IPluralRule>, IPluralRules, IPluralRulesEnumerable, IPluralRulesQueryable
    {
        /// <summary>
        /// Create rules
        /// </summary>
        public PluralRulesList() : base()
        {
        }

        /// <summary>
        /// Create rules
        /// </summary>
        public PluralRulesList(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// Create rules
        /// </summary>
        /// <param name="rules"></param>
        public PluralRulesList(params IPluralRule[] rules) : base(rules)
        {
        }

        /// <summary>
        /// Create rules.
        /// </summary>
        /// <param name="rules"></param>
        public PluralRulesList(IEnumerable<IPluralRule> rules) : base(rules)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <returns></returns>
        public virtual IPluralRulesEnumerable Query(PluralRuleInfo filterCriteria)
        {
            if (filterCriteria.Equals(PluralRuleInfo.Empty)) return this;

            int c = 0;
            foreach (var rule in this)
                if (filterCriteria.FilterMatch(rule.Info)) c++;
            IPluralRule[] result = new IPluralRule[c];

            int i = 0;
            foreach (var rule in this)
                if (filterCriteria.FilterMatch(rule.Info))
                    result[i++] = rule;

            return new PluralRulesArray(result);
        }

    }

    /// <summary>
    /// Collection of <see cref="IPluralRule"/> that caches queries.
    /// 
    /// IF query is not found i cache, it is read from the <see cref="ruleSource"/>, and then result cached.
    /// 
    /// If query involes cases of one set of ruleset,cagetory,culture, and it doesn't implement <see cref="IPluralRulesEvaluatable"/>,
    /// then the result is wrapped into <see cref="PluralRulesCasesEvaluatable"/>.
    /// </summary>
    public class PluralRulesEvaluatable : IPluralRules, IPluralRulesEvaluatable, IPluralRulesQueryable, IPluralRulesEnumerable
    {
        /// <summary>
        /// Cached queries
        /// </summary>
        protected Dictionary<PluralRuleInfo, IPluralRulesEnumerable> queries = new Dictionary<PluralRuleInfo, IPluralRulesEnumerable>();

        /// <summary>
        /// Cache lock.
        /// </summary>
        protected object m_lock = new object();

        /// <summary>
        /// Source of rules.
        /// </summary>
        protected object ruleSource;

        /// <summary>
        /// 
        /// </summary>
        protected PluralRulesEvaluatable()
        {
        }

        /// <summary>
        /// Create rules
        /// </summary>
        /// <param name="ruleReader">source of rules</param>
        public PluralRulesEvaluatable(IEnumerable<IPluralRule> ruleReader)
        {
            this.ruleSource = ruleReader ?? throw new ArgumentNullException(nameof(ruleReader));
        }

        /// <summary>
        /// Create rules
        /// </summary>
        /// <param name="rules">source of rules that implements <see cref="IPluralRulesQueryable"/> or <see cref="IPluralRulesEnumerable"/></param>
        public PluralRulesEvaluatable(IPluralRules rules)
        {
            this.ruleSource = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        /// <summary>
        /// Filter rules by <paramref name="filterCriteria"/>.
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <returns><see cref="PluralRulesCasesEvaluatable"/> or <see cref="PluralRulesArray"/>, or null if content is empty</returns>
        public IPluralRulesEnumerable Query(PluralRuleInfo filterCriteria)
        {
            // Try get
            IPluralRulesEnumerable result = null;
            lock (m_lock) if (queries.TryGetValue(filterCriteria, out result)) return result;

            // Query
            if (ruleSource is IPluralRulesQueryable queryable)
            {
                result = queryable.Query(filterCriteria);
                // Wrap into PluralRulesEvaluatable
                if (result != null && filterCriteria.Category != null && filterCriteria.Culture != null && filterCriteria.Case == null && filterCriteria.Optional == -1) result = new PluralRulesCasesEvaluatable(result);
            }
            else if (ruleSource is IEnumerable<IPluralRule> enumr)
            {
                // Filter rules
                StructList8<IPluralRule> list = new StructList8<IPluralRule>();
                StructList8<string> rulesets = new StructList8<string>();
                foreach (IPluralRule rule in enumr)
                {
                    // Filter by criteria
                    if (!filterCriteria.FilterMatch(rule.Info)) continue;
                    // Gather a list of rulesets
                    if (rule.Info.RuleSet != null && !rulesets.Contains(rule.Info.RuleSet)) rulesets.Add(rule.Info.RuleSet);
                    // Add to list
                    list.Add(rule);
                }
                // No result
                if (list.Count == 0) result = null;
                // Instantiate PluralRulesEvaluatable
                else if (rulesets.Count <= 1 && filterCriteria.Category != null && filterCriteria.Culture != null && filterCriteria.Case == null && filterCriteria.Optional == -1) result = new PluralRulesCasesEvaluatable(list.ToArray());
                // Instantiate PluralRules.
                else result = new PluralRulesArray(list.ToArray());
            }
            // Could not read source
            else
            {
                result = null;
            }
            // Write to cache, if is still new
            lock (m_lock) if (!queries.ContainsKey(filterCriteria)) queries[filterCriteria] = result;

            // Return result
            return result;
        }

        /// <summary>
        /// Evaluate cases
        /// </summary>
        /// <param name="subset">filter</param>
        /// <param name="number"></param>
        /// <returns>matching cases. First ones are optional, last one is non-optional. Or null if none matched.</returns>
        public IPluralRule[] Evaluate(PluralRuleInfo subset, IPluralNumber number)
            => Query(subset) is IPluralRulesEvaluatable eval ? eval.Evaluate(subset, number) : null;

        /// <summary>
        /// (re-)read rules.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IPluralRule> GetEnumerator()
            => ruleSource is IEnumerable<IPluralRule> enumr ? enumr.GetEnumerator() : no_rules.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ruleSource is IEnumerable enumr ? enumr.GetEnumerator() : no_rules.GetEnumerator();

        static IEnumerable<IPluralRule> no_rules = new IPluralRule[0];
    }

    /// <summary>
    /// Indexed rules and evaluator for cases of one combination of { RuleSet,Culture,Category }.
    /// 
    /// This class evaluates an array of <see cref="IPluralRuleEvaluatable"/> as a whole, and returns
    /// all the cases - optional and required - that match the requested <see cref="IPluralNumber" />.
    /// </summary>
    public class PluralRulesCasesEvaluatable : PluralRulesArray, IPluralRulesEvaluatable
    {
        /// <summary>
        /// List of evaluatable cases in order of: 1. optional, 2. required.
        /// </summary>
        public readonly IPluralRuleEvaluatable[] EvaluatableCases;

        /// <summary>
        /// Number of cases that are optional.
        /// </summary>
        public readonly int OptionalCaseCount;

        /// <summary>
        /// Number of permutations of optional cases: 2 ^ OptionalCaseCount - 1
        /// </summary>
        public readonly int OptionalCasePerumutationCount;

        /// <summary>
        /// List of cases organized so that each required case forms a <see cref="Line"/>.
        /// 
        /// And each line has a preconfigured result array for each permutation of optional cases.
        /// </summary>
        Line[] lines;

        /// <summary>
        /// Reorders so that optional cases are first then non-optional.
        /// Also filters out non-<see cref="IPluralRulesEvaluatable"/> rules.
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If one of the cases doesn't implement <see cref="IPluralRuleEvaluatable"></see></exception>
        static IEnumerable<IPluralRule> ReorderAndFilter(IEnumerable<IPluralRule> rules)
        {
            // Add optional cases
            foreach (IPluralRule rule in rules)
            {
                if (rule.Info.Optional == 1 && rule is IPluralRuleEvaluatable)
                    yield return rule;
            }

            // Add required cases
            foreach (IPluralRule rule in rules)
            {
                if (rule.Info.Optional == 0 && rule is IPluralRuleEvaluatable) yield return rule;
            }
        }

        /// <summary>
        /// Create evaluatable rules from a list of cases.
        /// 
        /// Last case can be non-evaluatable (e.g. "other"). 
        /// It will be used as fallback result, if no evaluatable cases match.
        /// </summary>
        /// <param name="evaluatableRule">cases that implement <see cref="IPluralRuleEvaluatable"></see></param>
        public PluralRulesCasesEvaluatable(params IPluralRule[] evaluatableRule) : this((IEnumerable<IPluralRule>)evaluatableRule) { }

        /// <summary>
        /// Create evaluatable rules from a list of cases.
        /// 
        /// Last case can be non-evaluatable (e.g. "other"). 
        /// It will be used as fallback result, if no evaluatable cases match.
        /// </summary>
        /// <param name="evaluatableCases">cases that implement <see cref="IPluralRuleEvaluatable"></see></param>
        public PluralRulesCasesEvaluatable(IEnumerable<IPluralRule> evaluatableCases) : base(ReorderAndFilter(evaluatableCases))
        {
            StructList12<IPluralRuleEvaluatable> evaluatables = new StructList12<IPluralRuleEvaluatable>();
            int firstNonOptionalCase = -1;
            for (int i = 0; i < Rules.Length; i++)
            {
                IPluralRule rule = Rules[i];
                if (rule is IPluralRuleEvaluatable ce) evaluatables.Add(ce);
                bool isOptional = rule.Info.Optional == 1;
                if (!isOptional && firstNonOptionalCase < 0) firstNonOptionalCase = i;
            }
            this.EvaluatableCases = evaluatables.ToArray();
            this.OptionalCaseCount = firstNonOptionalCase;
            if (OptionalCaseCount > 10) throw new ArgumentException($"Maximum number of optional cases is 10, got {OptionalCaseCount}");
            OptionalCasePerumutationCount = (1 << OptionalCaseCount);

            // Add non-optional
            StructList12<IPluralRule> list = new StructList12<IPluralRule>();
            StructList12<Line> lines = new StructList12<Line>();
            for (int l = firstNonOptionalCase; l < EvaluatableCases.Length; l++)
            {
                IPluralRule c = Rules[l];
                IPluralRuleEvaluatable ce = EvaluatableCases[l];
                Line line = new Line { Evaluatable = ce };
                line.OptionalRulePermutations = (IPluralRule[][])Array.CreateInstance(typeof(IPluralRule[]), OptionalCasePerumutationCount);
                for (int i = 0; i < OptionalCasePerumutationCount; i++)
                {
                    list.Clear();
                    for (int j = 0; j < OptionalCaseCount; j++)
                        if ((i & (1 << j)) != 0) list.Add(Rules[j]);
                    list.Add(c);
                    line.OptionalRulePermutations[i] = list.ToArray();
                }
                lines.Add(line);
            }
            this.lines = lines.ToArray();
        }

        /// <summary>
        /// Evaluate cases
        /// </summary>
        /// <param name="subset">filter</param>
        /// <param name="number"></param>
        /// <returns>matching cases. First ones are optional, last one is non-optional. Or null if none matched.</returns>
        public IPluralRule[] Evaluate(PluralRuleInfo subset, IPluralNumber number)
        {
            // Evaluate each optional cases
            int optionalCaseBits = 0;
            for (int i = 0; i < OptionalCaseCount; i++)
                if (EvaluatableCases[i].Evaluate(number)) optionalCaseBits |= 1 << i;

            // Evaluate required cases
            for (int i = 0; i < lines.Length; i++)
                // Evaluate required case
                if (lines[i].Evaluatable.Evaluate(number))
                    // Return precalculated array
                    return lines[i].OptionalRulePermutations[optionalCaseBits];

            // None matched
            return null;
        }

        /// <summary>
        /// List of cases organized so that each non-optional case forms a <see cref="Line"/>.
        /// 
        /// And each line has a preconfigured result array for each permutation of optional cases.
        /// </summary>
        class Line
        {
            /// <summary>
            /// Evaluatable, non-optional, rule.
            /// </summary>
            public IPluralRuleEvaluatable Evaluatable;

            /// <summary>
            /// List of case-result arrays for the result of <see cref="IPluralRulesEvaluatable.Evaluate(PluralRuleInfo, IPluralNumber)"/>.
            /// One result array for every permutation of optional cases.
            /// 
            /// The last element of the array is the required case.
            /// </summary>
            public IPluralRule[][] OptionalRulePermutations;
        }
    }

}
