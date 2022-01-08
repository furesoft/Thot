namespace EbnfParserGenerator.Ebnf.AST;

public class InvalidExpr : Expr
{
    public InvalidExpr(ASTNode? parent = null) : base(parent)
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}