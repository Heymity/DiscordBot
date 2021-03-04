using Discord.Commands;
using System.Threading.Tasks;
using DiscordBot.Utilities.Calculator;

namespace DiscordBot.Modules
{
    [Group("calc")]
    public class CalculatorModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public Task ExpresionCalculator([Remainder][Summary("The Expression")] string expression)
        {
            ReplyAsync($"Calculating... {expression}");
            CalculatorLogic.SolveExpression(expression);
            return Task.CompletedTask;
        }
        
        [Command("add")]
        [Alias("+", "a", "soma", "sum")]
        public Task Add(params double[] numbers) => ReplyAsync(CalculatorLogic.Add(numbers).ToString());

        [Command("sub")]
        [Alias("-", "s", "subtract", "subtrair")]
        public Task Subtract(params double[] numbers) => ReplyAsync(CalculatorLogic.Subtract(numbers).ToString());

        [Command("mul")]
        [Alias("*", "x", "multiply", "multiplicar")]
        public Task Multiply(params double[] numbers) => ReplyAsync(CalculatorLogic.Multiply(numbers).ToString());

        [Command("div")]
        [Alias("/", "d", "division", "dividir")]
        public Task Divide(params double[] numbers) => ReplyAsync(CalculatorLogic.Divide(numbers).ToString());

        [Command("pow")]
        [Alias("^", "^^", "p", "power", "exp", "exponent", "exponencial")]
        public Task Power(params double[] numbers) => ReplyAsync(CalculatorLogic.Power(numbers).ToString());

        [Command("sqrt")]
        [Alias("sq", "root", "r")]
        public Task SquareRoot(double number, double root = 2) => ReplyAsync(CalculatorLogic.Root(number, root).ToString());
    }
}
