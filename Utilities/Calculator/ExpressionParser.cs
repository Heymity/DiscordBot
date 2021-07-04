using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DiscordBot.Utilities.Calculator
{
    /// TODO: Fix implicit multiplication, expressions like !calc 5 + 5(4 / 2)^4 dont work because it first multiplies y 5 and then power to 4
    internal static class ExpressionParser
    {
        public static readonly List<char> openingGroupChars = new List<char>() { '(', '[', '{' };
        public static readonly List<char> closingGroupChars = new List<char>() { ')', ']', '}' };

        public static List<Expression> Parse(Expression expression)
        {
            List<Expression> parsedExp = new List<Expression>();
            string exp = expression.Value;

            /// Remove all spaces
            exp = Regex.Replace(exp, @"\s+", "");

            string tmp = "";
            /// Adds the value to the tmp variable if its not an operator or a opening group char,
            /// When it finds a operator it adds to the list the tmp variable
            /// If it is a opening group char it will get the nested expression
            /// If it is a operator it will add the operator
            for (int i = 0; i < exp.Length; i++)
            {
                if (char.IsDigit(exp[i]) || exp[i] == ',' || exp[i] == '.')
                {
                    tmp += exp[i];
                }
                else
                {
                    Expression symbol;
                    /// In case a opening bracket is found it creates a new expression
                    if (openingGroupChars.Contains(exp[i]))
                    {
                        symbol = GetNestedExpressions(exp, ref i);
                        /// In case there is something before it that is not a operator, or its just a operator without a number, it puts a * operator
                        if (tmp != "")
                        {
                            if (tmp == "-" || tmp == "+") tmp += "1";
                            symbol = new Expression(SymbolType.Expression, new Expression(SymbolType.Number, tmp), new Expression(SymbolType.Operator, "*"), symbol);

                            tmp = "";
                            parsedExp.Add(symbol);
                            continue;
                        }
                    }
                    else
                    {
                        /// This makes it possible to use negative numbers
                        if (tmp == "" && (exp[i] == '-' || exp[i] == '+'))
                        {
                            tmp += exp[i];
                            continue;
                        }
                        symbol = new Expression(SymbolType.Operator, exp[i].ToString());
                    }

                    if (tmp != "")
                        parsedExp.Add(new Expression(SymbolType.Number, tmp));

                    tmp = "";
                    parsedExp.Add(symbol);
                    continue;
                }
            }
            /// If the expression ends with a number it need to be added after the loop,
            /// since in the for loop it only adds numbers when a operator is found after
            if (tmp != "")
                parsedExp.Add(new Expression(SymbolType.Number, tmp));

            /// If the list has only three items, it is already in its most simplified state
            if (parsedExp.Count <= 3) return parsedExp;

            /// This will interate the list and reduce all items to its simplest form, that is
            /// having only three items (value operator value)
            /// The result will be a tree of nested expressions.
            Expression s = Expression.Empty;
            while (s != null)
            {
                s = ReduceParse(parsedExp, out int i);

                if (s == null) break;

                parsedExp.RemoveRange(i - 2, 3);
                parsedExp.Insert(i - 2, s);
            }

            return parsedExp;
        }

        private static Expression GetNestedExpressions(string exp, ref int index)
        {
            index++;
            /// This is the index after the opening group char, and nestedIndex count how many
            /// nested groups there are, so it closes on the right index
            int startIndex = index;
            int nestedIndex = 0;
            for (_ = 0; index < exp.Length; index++)
            {
                if (openingGroupChars.Contains(exp[index]))
                    nestedIndex++;
                else if (closingGroupChars.Contains(exp[index]))
                {
                    if (nestedIndex <= 0)
                        break;

                    nestedIndex--;
                }
            }
            /// Create a new expression with the content between the opening and closing group chars and then parse it
            Expression expression = new Expression(exp[startIndex..index]);
            expression.Parse();

            return expression;
        }

        private static Expression ReduceParse(List<Expression> parsedExp, out int i)
        {
            i = 0;
            if (parsedExp.Count <= 3) return null;

            int? indexToSearch = FindIndexToSearch(parsedExp);
            if (indexToSearch == null) return null;

            for (i = indexToSearch.GetValueOrDefault(); i < parsedExp.Count; i++)
            {
                if (parsedExp[i].type == SymbolType.Operator)
                {
                    Expression pre;
                    if (i == 0) pre = new Expression(SymbolType.Number, "0");
                    else
                    {
                        if (parsedExp[i - 1].type != SymbolType.Operator) pre = parsedExp[i - 1];
                        else throw new Exception("Operator placed next to another one");
                    }

                    Expression pos;
                    if (i == parsedExp.Count - 1) throw new Exception("Operator placed at end of expression");
                    else
                    {
                        if (parsedExp[i + 1].type != SymbolType.Operator) pos = parsedExp[i + 1];
                        else throw new Exception("Operator placed next to another one");
                    }

                    return new Expression(SymbolType.Expression, pre, parsedExp[i++], pos);
                }
            }
            return null;
        }

        // Could be optimized by running it only one time and then storing the values in just one list, and get the index from that
        private static int? FindIndexToSearch(List<Expression> list)
        {
            List<int> multiplicationIndexes = new List<int>();
            List<int> additiveIndexes = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].type != SymbolType.Operator) continue;
                if (list[i].Value == "^" || list[i].Value == "log") return i;
                if (list[i].Value == "*" || list[i].Value == "/" || list[i].Value == "%") multiplicationIndexes.Add(i);
                if (list[i].Value == "+" || list[i].Value == "-") additiveIndexes.Add(i);
            }

            if (multiplicationIndexes.Count > 0) return multiplicationIndexes[0];
            if (additiveIndexes.Count > 0) return additiveIndexes[0];
            return null;
        }
    }
}