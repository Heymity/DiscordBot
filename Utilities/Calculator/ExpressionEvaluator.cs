using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Calculator
{
    static class ExpressionEvaluator
    {
        public static string Evaluate(Expression exp)
        {
            List<Expression> symbols = exp.parsedExpression;
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i].type == SymbolType.Expression) 
                    symbols[i].SetValue(Evaluate(symbols[i]));

            }

            return "";
        }
    }
}
