using Discord;
using Discord.Commands;
using DiscordBot.Utilities.Calculator;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("calc")]
    public class CalculatorModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Alias("expre", "expression", "e")]
        [Summary("Given a commun maths expression, like 5 + 5(3 / 2)^4, it will solve it and output the result")]
        public Task ExpresionCalculator([Remainder][Summary("The Expression")] string expression)
        {
            try
            {
                var a = ReplyAsync("Calculating...").Result;

                a.ModifyAsync((MessageProperties prop) => prop.Content = CalculatorLogic.SolveExpression(expression));
            }
            catch (Exception e)
            {
                ReplyAsync($"Error: {e.Message}");
            }
            return Task.CompletedTask;
        }

        [Command("add")]
        [Alias("+", "a", "soma", "sum")]
        [Summary("Given two or more numbers separated by a space, it sums them and output the result")]
        public Task Add(params double[] numbers) => ReplyAsync(CalculatorLogic.Add(numbers).ToString());

        [Command("sub")]
        [Alias("-", "s", "subtract", "subtrair")]
        [Summary("Given two  or more numbers separated by a space, it subtracts them and output the result")]
        public Task Subtract(params double[] numbers) => ReplyAsync(CalculatorLogic.Subtract(numbers).ToString());

        [Command("mul")]
        [Alias("*", "x", "multiply", "multiplicar")]
        [Summary("Given two or more numbers separated by a space, it multiplies them and output the result")]
        public Task Multiply(params double[] numbers) => ReplyAsync(CalculatorLogic.Multiply(numbers).ToString());

        [Command("div")]
        [Alias("/", "d", "division", "dividir")]
        [Summary("Given two or more numbers separated by a space, it divides them and output the result")]
        public Task Divide(params double[] numbers) => ReplyAsync(CalculatorLogic.Divide(numbers).ToString());

        [Command("pow")]
        [Alias("^", "^^", "**", "p", "power", "exp", "exponent", "exponencial")]
        [Summary("Given two or more numbers separated by a space, it powers one to the next then and output the result")]
        public Task Power(params double[] numbers) => ReplyAsync(CalculatorLogic.Power(numbers).ToString());

        [Command("sqrt")]
        [Alias("sq", "root", "r")]
        [Summary("Given two numbers, the second being optional, separated by a space, it makes the root of the first number to the second, which default is 2 and then output the result")]
        public Task SquareRoot(double number, double root = 2) => ReplyAsync(CalculatorLogic.Root(number, root).ToString());
    }
}