using System;
using System.Collections.Generic;

namespace DiscordBot.Utilities.Calculator
{
    internal static class ExpressionEvaluator
    {
        public static string Evaluate(Expression exp)
        {
            List<Expression> symbols = exp.parsedExpression;
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i].type == SymbolType.Expression)
                    symbols[i].SetValue(Evaluate(symbols[i]), SymbolType.Number);
            }

            if (exp.parsedExpression.Count == 1) return exp.parsedExpression[0].Value;
            if (exp.parsedExpression.Count != 3) throw new Exception("There probally is something wrong with your sintaxe, maybe a wring opening or closing bracket? or a lonely operator. If it seems to be all correct, try to specify the most your expression by adding brackets or using -1 * value instead of just -value");
            var h = HandleSolve(exp.parsedExpression[0], exp.parsedExpression[1], exp.parsedExpression[2]);
            return h;
        }

        private static string HandleSolve(Expression first, Expression opt, Expression second)
        {
            double firstNum = double.Parse(first.Value.Replace('.', ','));
            double secondNum = double.Parse(second.Value.Replace('.', ','));
            return opt.Value switch
            {
                "log" => Math.Log(secondNum, newBase: firstNum).ToString(),
                "^" => Math.Pow(firstNum, secondNum).ToString(),
                "*" => (firstNum * secondNum).ToString(),
                "/" => (firstNum / secondNum).ToString(),
                "+" => (firstNum + secondNum).ToString(),
                "-" => (firstNum - secondNum).ToString(),
                "%" => (firstNum % secondNum).ToString(),
                _ => throw new Exception($"Operator \"{opt.Value}\" not reconized"),
            };
        }
    }
}