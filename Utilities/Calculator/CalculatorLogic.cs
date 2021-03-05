using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Calculator
{
    class CalculatorLogic
    {
        public static double Add(params double[] numbers)
        {
            double sum = 0;
            foreach (var v in numbers) sum += v;
            return sum;
        }

        public static double Subtract(params double[] numbers)
        {
            if (numbers.Length <= 1) return numbers.Length > 0 ? numbers[0] : 0d;
            double sub = numbers[0];
            numbers[0] = 0;
            foreach (var v in numbers) sub -= v;
            return sub;
        }

        public static double Multiply(params double[] numbers)
        {
            if (numbers.Length <= 1) return numbers.Length > 0 ? numbers[0] : 0d;
            var nums = new List<double>(numbers);
            double mul = nums[0];
            nums.RemoveAt(0);
            nums.ForEach((double v) => mul *= v);
            return mul;
        }

        public static double Divide(params double[] numbers)
        {
            if (numbers.Length <= 1) return numbers.Length > 0 ? numbers[0] : 0d;
            var nums = new List<double>(numbers);
            double div = nums[0];
            nums.RemoveAt(0);
            nums.ForEach((double v) => div /= v);
            return div;
        }

        public static double Power(params double[] numbers)
        {
            if (numbers.Length <= 1) return numbers.Length > 0 ? numbers[0] : 0d;
            if (numbers.Length == 2)
                return Math.Pow(numbers[0], numbers[1]);
            else
                return Math.Pow(numbers[0], Power(numbers[1..]));
        }

        public static double Root(double number, double root = 2) => Math.Pow(number, 1 / root);

        public static string SolveExpression(string expression)
        {
            Expression exp = new Expression(expression);
            exp.Parse();
            exp.Evaluate();
            return exp.Value;
        }

        public static double SolveExpressionReturnDouble(string expression)
        {
            Expression exp = new Expression(expression);
            exp.Parse();
            exp.Evaluate();
            return double.Parse(exp.Value);
        }

    }
        
}
