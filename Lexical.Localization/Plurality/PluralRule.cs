﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Basic plural rule info.
    /// </summary>
    public class PluralRule : IPluralRule, IPluralRuleEvaluatable
    {
        /// <summary>
        /// Plural rule info
        /// </summary>
        public PluralRuleInfo Info { get; protected set; }

        /// <summary>
        /// Create rule.
        /// </summary>
        /// <param name="info">info</param>
        public PluralRule(PluralRuleInfo info)
        {
            this.Info = info;
        }

        /// <summary>
        /// Evaluate number to the rule.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public virtual bool Evaluate(IPluralNumber number)
            => false;

        /// <summary>
        /// Print debug info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}[{Info}]";

        /// <summary>
        /// Zero case that matches when number is 0.
        /// </summary>
        public class Zero : PluralRule
        {
            /// <summary>
            /// Create rule that compares to zero value.
            /// </summary>
            /// <param name="info"></param>
            public Zero(PluralRuleInfo info) : base(info)
            {
            }

            /// <summary>
            /// Compare to zero.
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
                => number != null && number.Sign == 0;

            /// <summary>
            /// Print string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => "[" + Info + "] n=0"; 
        }

        /// <summary>
        /// Zero case that matches when number is 1.
        /// </summary>
        public class One : PluralRule
        {
            readonly static IPluralNumber _one = new DecimalNumber.Long(1L);

            /// <summary>
            /// Create rule that compares to zero value.
            /// </summary>
            /// <param name="info"></param>
            public One(PluralRuleInfo info) : base(info)
            {
            }

            /// <summary>
            /// Compare to zero.
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
                => PluralNumberComparer.Default.Equals(number, _one);

            /// <summary>
            /// Print string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => "[" + Info + "] n=1";
        }

        /// <summary>
        /// Null case that matches when number is null or empty.
        /// </summary>
        public class Empty : PluralRule
        {
            /// <summary>
            /// Create rule
            /// </summary>
            /// <param name="info"></param>
            public Empty(PluralRuleInfo info) : base(info)
            {
            }

            /// <summary>
            /// Compare to null.
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
                => number == null || (number.I_Digits == 0 && number.F_Digits == 0 && number.E_Digits == 0 && number.Sign == 0);

            /// <summary>
            /// Print string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => "[" + Info + "] n=null";
        }

        /// <summary>
        /// Case that always evaluates to true value.
        /// Used for fallback case "other".
        /// </summary>
        public class True : PluralRule
        {
            /// <summary>
            /// Create rule that always evaluates to true.
            /// </summary>
            /// <param name="info"></param>
            public True(PluralRuleInfo info) : base(info)
            {
            }

            /// <summary>
            /// Always true
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
                => true;

            /// <summary>
            /// Print string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => "[" + Info + "] true";
        }

        /// <summary>
        /// Case that always evaluates to true value.
        /// Used for fallback case "other".
        /// </summary>
        public class TrueWithExpression : PluralRule, IPluralRuleExpression
        {
            /// <summary/>
            public IPluralRuleInfosExpression Infos { get; internal set; }
            /// <summary/>
            public IExpression Rule { get; internal set; }
            /// <summary/>
            public ISamplesExpression[] Samples { get; internal set; }
            /// <summary> </summary>
            public int ComponentCount => 1 + (Samples==null?0:Samples.Length);
            /// <summary> </summary>
            public IExpression GetComponent(int ix)
                => ix == 0 ? Infos : (IExpression)Samples[ix - 1];

            /// <summary>
            /// Create rule that always evaluates to true.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="infosExp"></param>
            /// <param name="ruleExp"></param>
            /// <param name="samplesExps"></param>
            public TrueWithExpression(PluralRuleInfo info, IPluralRuleInfosExpression infosExp, IExpression ruleExp, params ISamplesExpression[] samplesExps) : base(info)
            {
                this.Infos = infosExp;
                this.Rule = ruleExp;
                this.Samples = samplesExps;
            }

            /// <summary>
            /// Always true
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
                => true;

            /// <summary>
            /// Print rule expression
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                var printer = new PluralRuleExpressionStringPrinter();
                if (Infos == null) printer.Append('[').Append(Info.ToString()).Append("] true"); else printer.Append(Infos);
                if (Samples != null) printer.Append(' ').Append(Samples, " ");
                return printer.ToString();
            }

        }

        /// <summary>
        /// Rule that evaluates against an expression
        /// </summary>
        public class Expression : PluralRule, IPluralRuleEvaluatable, IPluralRuleExpression
        {
            /// <summary>
            /// Info expressions
            /// </summary>
            public IPluralRuleInfosExpression Infos { get; protected set; }

            /// <summary>
            /// Rule expression that can evaluate a number
            /// </summary>
            public IExpression Rule { get; protected set; }

            /// <summary>
            /// Samples
            /// </summary>
            public ISamplesExpression[] Samples { get; protected set; }

            /// <summary>
            /// No samples
            /// </summary>
            public static ISamplesExpression[] NO_SAMPLES = new ISamplesExpression[0];

            /// <summary> </summary>
            public int ComponentCount => 1 + (Samples == null ? 0 : Samples.Length);
            /// <summary> </summary>
            public IExpression GetComponent(int ix)
                => ix == 0 ? Infos : (IExpression)Samples[ix - 1];

            /// <summary>
            /// Convert <see cref="IPluralRuleInfosExpression"/> to <see cref="PluralRuleInfo"/>.
            /// </summary>
            /// <param name="infoExps"></param>
            /// <returns></returns>
            public static PluralRuleInfo Convert(IPluralRuleInfosExpression infoExps)
            {
                string ruleset = "", category = "", culture = "", @case = ""; int optional = 0;
                if (infoExps != null && infoExps.Infos!=null)
                {
                    foreach(var infoExp in infoExps.Infos)
                    {
                        if (infoExp.Name == "RuleSet") ruleset = infoExp.Value ?? "";
                        else if (infoExp.Name == "Category") category = infoExp.Value ?? "";
                        else if (infoExp.Name == "Culture") culture = infoExp.Value ?? "";
                        else if (infoExp.Name == "Case") @case = infoExp.Value ?? "";
                        else if (infoExp.Name == "Optional" && !string.IsNullOrEmpty(infoExp.Value)) optional = Int32.Parse(infoExp.Value);
                    }
                }
                return new PluralRuleInfo(ruleset, category, culture, @case, optional);
            }

            /// <summary>
            /// Create rule that evaluates with <paramref name="ruleExpression"/>.
            /// 
            /// Extracts <see cref="PluralRuleInfo"/> from <paramref name="infoExpression"/> expression.
            /// </summary>
            /// <param name="infoExpression">info expression</param>
            /// <param name="ruleExpression"></param>
            /// <param name="samplesExpression"></param>
            public Expression(IPluralRuleInfosExpression infoExpression, IExpression ruleExpression, params ISamplesExpression[] samplesExpression) : base(Convert(infoExpression))
            {
                this.Infos = infoExpression;
                this.Rule = ruleExpression;
                this.Samples = samplesExpression ?? NO_SAMPLES;
            }

            /// <summary>
            /// Create rule that evaluates with <paramref name="ruleExpression"/>.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="infoExpression">info expression</param>
            /// <param name="ruleExpression"></param>
            /// <param name="samplesExpression"></param>
            public Expression(PluralRuleInfo info, IPluralRuleInfosExpression infoExpression, IExpression ruleExpression, params ISamplesExpression[] samplesExpression) : base(info)
            {
                this.Infos = infoExpression;
                this.Rule = ruleExpression;
                this.Samples = samplesExpression ?? NO_SAMPLES;
            }

            /// <summary>
            /// Evaluate <paramref name="number"/> against <see cref="Rule"/>.
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
                => Rule == null ? true : new PluralRuleExpressionEvaluator(number).EvaluateBoolean(Rule);

            /// <summary>
            /// Print rule expression
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                var printer = new PluralRuleExpressionStringPrinter();
                if (Infos == null) printer.Append('[').Append(Info.ToString()).Append("] ");
                printer.Append(this);
                return printer.ToString();
            }
                
                
        }

    }

}
