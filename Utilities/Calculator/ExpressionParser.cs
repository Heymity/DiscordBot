using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DiscordBot.Utilities.Calculator
{
    static class ExpressionParser
    {
        public static List<char> groupChars = new List<char>() { '(', ')', '[', ']', '{', '}' };
        public static List<char> openingGroupChars = new List<char>() { '(', '[', '{' };
        public static List<char> closingGroupChars = new List<char>() { ')', ']', '}' };

        public static List<Expression> Parse(Expression expression)
        {
            List<Expression> parsedExp = new List<Expression>();
            string exp = expression.Value;
            exp = Regex.Replace(exp, @"\s+", "");

            string tmp = "";
            for (int i = 0; i < exp.Length; i++)
            {
                if (!char.IsDigit(exp[i]))
                {
                    if (tmp != "") 
                        parsedExp.Add(new Expression(SymbolType.Number, tmp));
                    tmp = "";

                    Expression symbol;
                    if (openingGroupChars.Contains(exp[i]))
                        symbol = GetNestedExpressions(exp, ref i);
                    else
                        symbol = new Expression(SymbolType.Operator, exp[i].ToString());

                    parsedExp.Add(symbol);
                    continue;
                }

                tmp += exp[i];
            }
            if (tmp != "")
                parsedExp.Add(new Expression(SymbolType.Number, tmp));

            if (parsedExp.Count <= 3) return parsedExp;

            Expression s = Expression.Empty;
            while (s != null)
            {
                int i = 0;
                s = ReduceParse(parsedExp, out i);

                if (s == null) break;

                parsedExp.RemoveRange(i - 2, 3);
                parsedExp.Insert(i - 2, s);
            }
         
            return parsedExp;
        }

        static Expression GetNestedExpressions(string exp, ref int index)
        {
            index++;
            int startIndex = index;
            int nestedIndex = 0;
            for(_ = 0; index < exp.Length; index++)
            {
                if (/*index > startIndex && */openingGroupChars.Contains(exp[index]))
                    nestedIndex++;
                
                if (closingGroupChars.Contains(exp[index]))
                {
                    if (nestedIndex <= 0)
                    {
                        //index++;
                        break;
                    }
                    nestedIndex--;
                }
            }
            var substring = exp.Substring(startIndex, index - 1);
            Expression expression = new Expression(substring);
            expression.Parse();
            //Expression s = new Expression(SymbolType.Expression, substring, expression);

            return expression;
        }

        static Expression ReduceParse(List<Expression> parsedExp, out int i)
        {
            i = 0;
            if (parsedExp.Count <= 3) return null;

            int? indexToSearch = FindIndexToSearch(parsedExp);
            if (indexToSearch == null) return null;

            for (i = indexToSearch.GetValueOrDefault(); i < parsedExp.Count; i++)
            {
                if (parsedExp[i].type == SymbolType.Operator)
                {
                    Expression pre = Expression.Empty;
                    Expression pos = Expression.Empty;

                    if (i == 0) pre = new Expression(SymbolType.Number, "0");
                    else
                    {
                        if (parsedExp[i - 1].type != SymbolType.Operator) pre = parsedExp[i - 1];
                    }
                    if (i == parsedExp.Count - 1) throw new Exception("operator at end of expression");
                    else
                    {
                        if (parsedExp[i + 1].type != SymbolType.Operator) pos = parsedExp[i + 1];
                    }

                    Expression newExpresion = new Expression(SymbolType.Expression, pre, parsedExp[i], pos); // Should be merged
                    i++;
                    return newExpresion;
                }
            }
            return null;
        }

        static int? FindIndexToSearch(List<Expression> list)
        {
            List<int> multiplicationIndexes = new List<int>();
            List<int> additiveIndexes = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].type != SymbolType.Operator) continue;
                if (list[i].Value == "^") return i;
                if (list[i].Value == "*" || list[i].Value == "/") multiplicationIndexes.Add(i);
                if (list[i].Value == "+" || list[i].Value == "-") additiveIndexes.Add(i);
            }

            if (multiplicationIndexes.Count > 0) return multiplicationIndexes[0];
            if (additiveIndexes.Count > 0) return additiveIndexes[0];
            return null;
        }
    }
}
