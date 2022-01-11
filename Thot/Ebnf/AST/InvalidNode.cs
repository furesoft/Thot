using Thot.Ebnf.AST;
namespace Thot.Ebnf.AST;

public class InvalidNode : ASTNode
{
    public InvalidNode(ASTNode? parent = null) : base(parent)
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
