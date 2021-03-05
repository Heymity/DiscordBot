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
                    symbols[i].SetValue(Evaluate(symbols[i]), SymbolType.Number);                       
            }

            if (exp.parsedExpression.Count == 1) return exp.parsedExpression[0].Value;
            if (exp.parsedExpression.Count != 3) throw new Exception("There probally is something wrong with your sintaxe, maybe a wring opening or closing bracket? or a lonely operator");
            var h = HandleSolve(exp.parsedExpression[1], (exp.parsedExpression[0], exp.parsedExpression[2]));
            return h;
        }

        static string HandleSolve(Expression opt, (Expression first, Expression second) values)
        {
            double firstNum = double.Parse(values.first.Value.Replace('.', ','));
            double secondNum = double.Parse(values.second.Value.Replace('.', ','));
            return opt.Value switch
            {
                "^" => Math.Pow(firstNum, secondNum).ToString(),
                "*" => (firstNum * secondNum).ToString(),
                "/" => (firstNum / secondNum).ToString(),
                "+" => (firstNum + secondNum).ToString(),
                "-" => (firstNum - secondNum).ToString(),
                "%" => (firstNum % secondNum).ToString(),
                _ => throw new Exception("Operator not reconized"),
            };
        }
    }
}
