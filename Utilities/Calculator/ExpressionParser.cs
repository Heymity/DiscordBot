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

        public static List<Symbol> Parse(Expression expression)
        {
            List<Symbol> parsedExp = new List<Symbol>();
            string exp = expression.expression;
            exp = Regex.Replace(exp, @"\s+", "");

            string tmp = "";
            for (int i = 0; i < exp.Length; i++)
            {
                if (!char.IsDigit(exp[i]))
                {
                    if (tmp != "") 
                        parsedExp.Add(new Symbol(SymbolType.Number, tmp));
                    tmp = "";

                    Symbol symbol;
                    if (openingGroupChars.Contains(exp[i]))
                        symbol = GetNestedExpressions(exp, ref i);
                    else
                        symbol = new Symbol(SymbolType.Operator, exp[i].ToString());

                    parsedExp.Add(symbol);
                    continue;
                }

                tmp += exp[i];
            }
            if (tmp != "")
                parsedExp.Add(new Symbol(SymbolType.Number, tmp));

            if (parsedExp.Count <= 3) return parsedExp;

            List<Symbol> reducedParse = new List<Symbol>();

            Symbol? s = Symbol.Empty;
            while (s != null)
            {
                int i = 0;
                s = ReduceParse(parsedExp, out i);

                if (s == null) break;

                parsedExp.RemoveRange(i - 2, 3);
                parsedExp.Insert(i - 2, s.GetValueOrDefault());
            }
         
            return parsedExp;
        }

        static Symbol GetNestedExpressions(string exp, ref int index)
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
            Symbol s = new Symbol(SymbolType.Expression, substring, expression);

            return s;
        }

        static Symbol? ReduceParse(List<Symbol> parsedExp, out int i)
        {
            i = 0;
            if (parsedExp.Count <= 3) return null;

            int? indexToSearch = FindIndexToSearch(parsedExp);
            if (indexToSearch == null) return null;

            for (i = indexToSearch.GetValueOrDefault(); i < parsedExp.Count; i++)
            {
                if (parsedExp[i].type == SymbolType.Operator)
                {
                    Symbol pre = Symbol.Empty;
                    Symbol pos = Symbol.Empty;

                    if (i == 0) pre = new Symbol(SymbolType.Number, "0");
                    else
                    {
                        if (parsedExp[i - 1].type != SymbolType.Operator) pre = parsedExp[i - 1];
                    }
                    if (i == parsedExp.Count - 1) throw new Exception("operator at end of expression");
                    else
                    {
                        if (parsedExp[i + 1].type != SymbolType.Operator) pos = parsedExp[i + 1];
                    }

                    Expression newExpresion = new Expression(pre, parsedExp[i], pos);
                    i++;
                    return new Symbol(SymbolType.Expression, newExpresion);
                }
            }
            return null;
        }

        static int? FindIndexToSearch(List<Symbol> list)
        {
            List<int> multiplicationIndexes = new List<int>();
            List<int> additiveIndexes = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].type != SymbolType.Operator) continue;
                if (list[i].value == "^") return i;
                if (list[i].value == "*" || list[i].value == "/") multiplicationIndexes.Add(i);
                if (list[i].value == "+" || list[i].value == "-") additiveIndexes.Add(i);
            }

            if (multiplicationIndexes.Count > 0) return multiplicationIndexes[0];
            if (additiveIndexes.Count > 0) return additiveIndexes[0];
            return null;
        }
    }
}
