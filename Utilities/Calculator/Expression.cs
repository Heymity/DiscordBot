using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Calculator
{
    public class Expression
    {
        public string expression;
        public List<Symbol> parsedExpression;

        public Expression(string expression)
        {
            this.expression = expression;
        }

        public List<Symbol> Parse() => parsedExpression = ExpressionParser.Parse(this);

        public void Evaluate()
        {

        }
    }

    public struct Symbol
    {
        public SymbolType type;
        public string value;
        public Expression nestedExpression;

        public Symbol(SymbolType type, string value, Expression nestedExpression = null)
        {
            this.type = type;
            this.value = value;
            this.nestedExpression = nestedExpression;
        }

        public void SetValue(string value) => this.value = value;
    }

    public enum SymbolType
    {
        Number,
        Operator,
        Expression,
        Variable
    }
}
