﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Rule expression evaluator.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Plural_rules_syntax"/>
    /// </summary>
    public struct PluralRuleExpressionEvaluator
    {
        /// <summary>
        /// Number to evaluate
        /// </summary>
        public IPluralNumber Number;

        /// <summary>
        /// Create plural number evaluator
        /// </summary>
        /// <param name="number"></param>
        /// <exception cref="ArgumentNullException">null</exception>
        public PluralRuleExpressionEvaluator(IPluralNumber number)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
        }

        /// <summary>
        /// Evaluate rule to number.
        /// </summary>
        /// <param name="rule">Rule to evaluate against.</param>
        /// <returns>true if rule matches</returns>
        /// <exception cref="NotSupportedException">Problem with expression</exception>
        public bool EvaluateBoolean(IExpression rule)
            => rule switch
            {
                IConstantExpression cnst => (bool)cnst.Value,
                IParenthesisExpression pexp => EvaluateBoolean(pexp.Element),
                IUnaryOpExpression uop =>
                    uop.Op switch
                    {
                        UnaryOp.Not => !EvaluateBoolean(uop.Element),
                        UnaryOp.OnesComplement => !EvaluateBoolean(uop.Element),
                        _ => throw new NotSupportedException($"Cannote valuate {nameof(IUnaryOpExpression)} with Op={uop.Op} to boolean result")
                    },
                IBinaryOpExpression bop => bop.Op switch
                {
                    BinaryOp.LogicalAnd => EvaluateBoolean(bop.Left) && EvaluateBoolean(bop.Right),
                    BinaryOp.LogicalOr => EvaluateBoolean(bop.Left) || EvaluateBoolean(bop.Right),
                    BinaryOp.Xor => EvaluateBoolean(bop.Left) ^ EvaluateBoolean(bop.Right),
                    BinaryOp.Equal => EvaluateEquals(bop.Left, bop.Right),
                    BinaryOp.NotEqual => EvaluateNotEqual(bop.Left, bop.Right),
                    _ => EvaluateBinaryOp(bop.Op, bop.Left, bop.Right),
                },
                _ => false
            };

        /// <summary>
        /// Evaluate rule to number.
        /// </summary>
        /// <param name="exp">Rule to evaluate against.</param>
        /// <returns>number range (min, max(inclusive))</returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public IPluralNumber EvaluateAsNumber(IExpression exp)
            => exp switch
            {
                //IUnaryOpExpression uop => uop.Op switch { UnaryOp. },
                IParenthesisExpression pexp => EvaluateAsNumber(pexp.Element),
                IBinaryOpExpression bop => bop.Op switch { BinaryOp.Modulo => Modulo(EvaluateAsNumber(bop.Left), EvaluateAsNumber(bop.Right)), _ => throw new NotSupportedException($"BinaryOpExpression '{bop.Op}' is not supported") },
                IArgumentNameExpression arg => arg.Name switch { "n" => Number.N, "i" => Number.I, "e" => Number.E, "f" => Number.F, "t" => Number.T, "v" => new DecimalNumber.Long(Number.F_Digits), "w" => new DecimalNumber.Long(Number.T_Digits), _ => throw new NotSupportedException($"Argument '{arg.Name}' is not supported") },
                IConstantExpression c => (IPluralNumber)c.Value,
                _ => throw new NotSupportedException($"{exp.GetType()} not supported.")
            };

        /// <summary>
        /// Evaluate modulo operation
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IPluralNumber Modulo(IPluralNumber left, IPluralNumber right)
        {
            long rightValue;
            if (!right.TryGet(out rightValue)) throw new InvalidOperationException("Failed to evaluate expression to numeric value.");
            if (rightValue < int.MinValue|| rightValue > int.MaxValue) throw new InvalidOperationException($"Modulo divider out of integer range (value={right})");
            return left.Modulo((int)rightValue);
        }

        bool EvaluateBinaryOp(BinaryOp bop, IExpression leftExp, IExpression rightExp)
        {
            // Group
            if (leftExp is IGroupExpression lGroup)
            {
                foreach (IExpression lGroupItem in lGroup.Values)
                {
                    if (EvaluateBinaryOp(bop, lGroupItem, rightExp)) return true;
                }
                return false;
            }
            if (rightExp is IGroupExpression rGroup)
            {
                foreach (IExpression rGroupItem in rGroup.Values)
                {
                    if (EvaluateBinaryOp(bop, leftExp, rGroupItem)) return true;
                }
                return false;
            }

            // Evaluate into number ranges
            PluralNumberComparer comparer = PluralNumberComparer.Default;
            if (leftExp is IRangeExpression lr)
            {
                IPluralNumber min = EvaluateAsNumber(lr.MinValue), max = EvaluateAsNumber(lr.MaxValue), number = EvaluateAsNumber(rightExp);

                // As per contract in https://www.unicode.org/reports/tr35/tr35-numbers.html#Relations
                // "The positive relations are of the format x = y and x = y mod z. 
                //  The y value can be a comma-separated list, such as n = 3, 5, 7..15, and is treated as if each relation were expanded into an OR statement. 
                //  The range value a..b is equivalent to listing all the integers between a and b, inclusive. When != is used, it means the entire relation is negated."
                if (number.T_Digits > 0) return false;

                return bop switch
                {
                    BinaryOp.Equal => comparer.Compare(min, number) <= 0 && comparer.Compare(number, max) <= 0,
                    BinaryOp.NotEqual => comparer.Compare(min, number) > 0 && comparer.Compare(number, max) > 0,
                    BinaryOp.LessThan => comparer.Compare(number, min) < 0 && comparer.Compare(number, max) < 0,
                    BinaryOp.LessThanOrEqual => comparer.Compare(number, min) <= 0 && comparer.Compare(number, max) <= 0,
                    BinaryOp.GreaterThan => comparer.Compare(number, min) > 0 && comparer.Compare(number, max) > 0,
                    BinaryOp.GreaterThanOrEqual => comparer.Compare(number, min) >= 0 && comparer.Compare(number, max) >= 0,
                    _ => throw new InvalidOperationException($"BinaryOperand {bop} is not supported for boolean result")
                };
            } else
            if (rightExp is IRangeExpression rr)
            {
                // Range comparison
                IPluralNumber min = EvaluateAsNumber(rr.MinValue), max = EvaluateAsNumber(rr.MaxValue), number = EvaluateAsNumber(leftExp);

                // As per contract in https://www.unicode.org/reports/tr35/tr35-numbers.html#Relations
                // "The positive relations are of the format x = y and x = y mod z. 
                //  The y value can be a comma-separated list, such as n = 3, 5, 7..15, and is treated as if each relation were expanded into an OR statement. 
                //  The range value a..b is equivalent to listing all the integers between a and b, inclusive. When != is used, it means the entire relation is negated."
                if (number.T_Digits > 0) return false;

                return bop switch
                {
                    //BinaryOp.Equal => comparer.Compare(min, number) <= 0 && comparer.Compare(number, max) <= 0,
                    //BinaryOp.NotEqual => comparer.Compare(min, number) > 0 && comparer.Compare(number, max) > 0,
                    BinaryOp.LessThan => comparer.Compare(number, min) < 0 && comparer.Compare(number, max) < 0,
                    BinaryOp.LessThanOrEqual => comparer.Compare(number, min) <= 0 && comparer.Compare(number, max) <= 0,
                    BinaryOp.GreaterThan => comparer.Compare(number, min) > 0 && comparer.Compare(number, max) > 0,
                    BinaryOp.GreaterThanOrEqual => comparer.Compare(number, min) >= 0 && comparer.Compare(number, max) >= 0,
                    _ => throw new InvalidOperationException($"BinaryOperand {bop} is not supported for boolean result")
                };
            }
            else
            {
                IPluralNumber l = EvaluateAsNumber(leftExp), r = EvaluateAsNumber(rightExp);
                return bop switch
                {
                    //BinaryOp.Equal => comparer.Compare(l, r) == 0,
                    //BinaryOp.NotEqual => comparer.Compare(l, r) != 0,
                    BinaryOp.LessThan => comparer.Compare(l, r) < 0,
                    BinaryOp.LessThanOrEqual => comparer.Compare(l, r) <= 0,
                    BinaryOp.GreaterThan => comparer.Compare(l, r) > 0,
                    BinaryOp.GreaterThanOrEqual => comparer.Compare(l, r) >= 0,
                    _ => throw new InvalidOperationException($"BinaryOperand {bop} is not supported for boolean result")
                };
            }
        }

        /// <summary>
        /// Evaluate equals <see cref="BinaryOpExpression"/>. 
        /// 
        /// As a special rule, CLDR Equals handles <see cref="IGroupExpression"/> and <see cref="IRangeExpression"/>s.
        /// Group and range expressions returns true, if one of the values matches. 
        /// For example "i % 100 = 0,20,40,60,80" is true for i==20.
        /// </summary>
        /// <param name="leftExp"></param>
        /// <param name="rightExp"></param>
        /// <returns></returns>
        bool EvaluateEquals(IExpression leftExp, IExpression rightExp)
        {
            // Group, recurse into each element
            if (leftExp is IGroupExpression lGroup)
            {
                foreach (IExpression lGroupItem in lGroup.Values)
                    if (EvaluateEquals(lGroupItem, rightExp)) return true;
                return false;
            }
            if (rightExp is IGroupExpression rGroup)
            {
                foreach (IExpression rGroupItem in rGroup.Values)
                    if (EvaluateEquals(leftExp, rGroupItem)) return true;
                return false;
            }

            // Evaluate into number ranges
            PluralNumberComparer comparer = PluralNumberComparer.Default;
            if (leftExp is IRangeExpression lr)
            {
                // Evaluate number.
                IPluralNumber number = EvaluateAsNumber(rightExp);
                // As per contract in https://www.unicode.org/reports/tr35/tr35-numbers.html#Relations
                // "The positive relations are of the format x = y and x = y mod z. 
                //  The y value can be a comma-separated list, such as n = 3, 5, 7..15, and is treated as if each relation were expanded into an OR statement. 
                //  The range value a..b is equivalent to listing all the integers between a and b, inclusive. When != is used, it means the entire relation is negated."
                if (number.T_Digits > 0) return false;

                IPluralNumber min = EvaluateAsNumber(lr.MinValue), max = EvaluateAsNumber(lr.MaxValue);
                return comparer.Compare(min, number) <= 0 && comparer.Compare(number, max) <= 0;
            }
            else
            if (rightExp is IRangeExpression rr)
            {
                // Evaluate number
                IPluralNumber number = EvaluateAsNumber(leftExp);

                // As per contract in https://www.unicode.org/reports/tr35/tr35-numbers.html#Relations
                // "The positive relations are of the format x = y and x = y mod z. 
                //  The y value can be a comma-separated list, such as n = 3, 5, 7..15, and is treated as if each relation were expanded into an OR statement. 
                //  The range value a..b is equivalent to listing all the integers between a and b, inclusive. When != is used, it means the entire relation is negated."
                if (number.T_Digits > 0) return false;

                // Range comparison
                IPluralNumber min = EvaluateAsNumber(rr.MinValue), max = EvaluateAsNumber(rr.MaxValue);

                return comparer.Compare(min, number) <= 0 && comparer.Compare(number, max) <= 0;
            }
            else
            {
                IPluralNumber l = EvaluateAsNumber(leftExp), r = EvaluateAsNumber(rightExp);
                return comparer.Compare(l, r) == 0;
            }
        }

        /// <summary>
        /// Evaluate not-equals "!=" <see cref="BinaryOpExpression"/>. 
        /// 
        /// As a special rule, CLDR Equals handles <see cref="IGroupExpression"/> and <see cref="IRangeExpression"/>s.
        /// Group and range expressions returns false, if none of the group item matches. 
        /// For example "i % 100 != 0,20,40,60,80" is false if i is none of the group.
        /// </summary>
        /// <param name="leftExp"></param>
        /// <param name="rightExp"></param>
        /// <returns></returns>
        bool EvaluateNotEqual(IExpression leftExp, IExpression rightExp)
        {
            // Group, recurse into each element
            if (leftExp is IGroupExpression lGroup)
            {
                foreach (IExpression lGroupItem in lGroup.Values)
                    if (!EvaluateNotEqual(lGroupItem, rightExp)) return false;
                return true;
            }
            if (rightExp is IGroupExpression rGroup)
            {
                foreach (IExpression rGroupItem in rGroup.Values)
                    if (!EvaluateNotEqual(leftExp, rGroupItem)) return false;
                return true;
            }

            // Evaluate into number ranges
            PluralNumberComparer comparer = PluralNumberComparer.Default;
            if (leftExp is IRangeExpression lr)
            {
                // Evaluate number.
                IPluralNumber number = EvaluateAsNumber(rightExp);
                // As per contract in https://www.unicode.org/reports/tr35/tr35-numbers.html#Relations
                // "The positive relations are of the format x = y and x = y mod z. 
                //  The y value can be a comma-separated list, such as n = 3, 5, 7..15, and is treated as if each relation were expanded into an OR statement. 
                //  The range value a..b is equivalent to listing all the integers between a and b, inclusive. When != is used, it means the entire relation is negated."
                if (number.T_Digits > 0) return true;

                IPluralNumber min = EvaluateAsNumber(lr.MinValue), max = EvaluateAsNumber(lr.MaxValue);
                return comparer.Compare(min, number) > 0 || comparer.Compare(number, max) > 0;
            }
            else
            if (rightExp is IRangeExpression rr)
            {
                // Evaluate number
                IPluralNumber number = EvaluateAsNumber(leftExp);

                // As per contract in https://www.unicode.org/reports/tr35/tr35-numbers.html#Relations
                // "The positive relations are of the format x = y and x = y mod z. 
                //  The y value can be a comma-separated list, such as n = 3, 5, 7..15, and is treated as if each relation were expanded into an OR statement. 
                //  The range value a..b is equivalent to listing all the integers between a and b, inclusive. When != is used, it means the entire relation is negated."
                if (number.T_Digits > 0) return true;

                // Range comparison
                IPluralNumber min = EvaluateAsNumber(rr.MinValue), max = EvaluateAsNumber(rr.MaxValue);

                return comparer.Compare(min, number) > 0 || comparer.Compare(number, max) > 0;
            }
            else
            {
                IPluralNumber l = EvaluateAsNumber(leftExp), r = EvaluateAsNumber(rightExp);
                return comparer.Compare(l, r) != 0;
            }
        }

    }

}
