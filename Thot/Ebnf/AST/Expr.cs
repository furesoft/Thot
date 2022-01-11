namespace Thot.Ebnf.AST;

public abstract class Expr : ASTNode
{
    protected Expr(ASTNode? parent) : base(parent)
    {
    }
}
