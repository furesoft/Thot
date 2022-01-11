namespace Thot.Ebnf.AST.Expressions;

public class RangeExpr : Expr
{
    public RangeExpr(char from, char to, ASTNode? parent = null) : base(parent)
    {
        From = from;
        To = to;
    }

    public char From { get; set; }
    public char To { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return $"{From}..{To}";
    }
}
