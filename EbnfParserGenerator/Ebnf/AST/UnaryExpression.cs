namespace EbnfParserGenerator.Ebnf.AST
{
    public abstract class UnaryExpression : Expr
    {
        protected UnaryExpression(string symbol, Expr expression)
        {
            Symbol = symbol;
            Expression = expression;
        }

        public Expr Expression { get; set; }
        public string Symbol { get; set; }

        public override string ToString()
        {
            return $"({Expression}){Symbol}";
        }
    }
}