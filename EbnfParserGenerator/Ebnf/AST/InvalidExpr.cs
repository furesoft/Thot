namespace EbnfParserGenerator.Ebnf.AST;

public class InvalidExpr : Expr
{
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}