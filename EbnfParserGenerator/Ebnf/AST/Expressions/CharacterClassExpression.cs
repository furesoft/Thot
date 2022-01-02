namespace EbnfParserGenerator.Ebnf.AST.Expressions;

public class CharackterClassRange : Expr
{
    public CharackterClassRange(char from, char to)
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
}

public class CharacterClassExpression : Expr
{
    public List<Expr> Rules { get; set; } = new();

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}