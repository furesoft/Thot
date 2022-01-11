namespace Thot.Ebnf.AST.Expressions;

public class ZeroOrMoreExpression : UnaryExpression
{
    public ZeroOrMoreExpression(Expr expr) : base("*", expr)
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
