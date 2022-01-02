namespace EbnfParserGenerator.Ebnf.AST;

public class AlternateNode : Expr
{
    public AlternateNode(Expr left, Expr right)
    {
        Left = left;
        Right = right;
    }

    public Expr Left { get; set; }
    public Expr Right { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return $"{Left} | {Right}";
    }
}