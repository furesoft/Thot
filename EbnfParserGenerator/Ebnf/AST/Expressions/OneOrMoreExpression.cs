namespace EbnfParserGenerator.Ebnf.AST.Expressions;

public class OneOrMoreExpression : UnaryExpression
{
    public OneOrMoreExpression(Expr expr) : base("+", expr)
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}