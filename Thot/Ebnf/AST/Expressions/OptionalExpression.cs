namespace Thot.Ebnf.AST.Expressions;

public class OptionalExpression : UnaryExpression
{
    public OptionalExpression(Expr expr) : base("?", expr)
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
