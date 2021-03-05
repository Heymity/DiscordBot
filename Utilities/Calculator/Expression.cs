using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Calculator
{
    public class Expression
    {
        public static readonly Expression Empty = new Expression("");

        public SymbolType type;
        public string Value { get; private set; } // Same as Expression here because I am lazy to change in the other files 

        public List<Expression> parsedExpression;

        public Expression(string expression)
        {
            this.Value = expression;
            type = SymbolType.Expression;
        }

        public Expression(SymbolType type, params Expression[] symbols)
        {
            this.type = type;
            parsedExpression = new List<Expression>(symbols);
            parsedExpression.ForEach((Expression s) => Value += s.Value);
        }

        public Expression(SymbolType type, string expression)
        {
            this.type = type;
            this.Value = expression;
        }

        public List<Expression> Parse() => parsedExpression = ExpressionParser.Parse(this);

        public void Evaluate()
        {

        }

        public void SetValue(string newValue, SymbolType newType)
        {
            type = newType;
            Value = newValue;
            parsedExpression = null;
        }
    }

    public enum SymbolType
    {
        Number,
        Operator,
        Expression,
        Variable
    }
}
