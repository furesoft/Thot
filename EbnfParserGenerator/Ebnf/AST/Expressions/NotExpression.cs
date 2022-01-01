namespace EbnfParserGenerator.Ebnf.AST.Expressions
{
    public class NotExpression : UnaryExpression
    {
        public NotExpression(Expr expr) : base("!", expr)
        {
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}