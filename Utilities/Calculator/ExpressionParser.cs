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
    }
}
