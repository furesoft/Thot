using Thot.Ebnf.AST;
namespace Thot.Ebnf.AST;

public abstract class ASTNode
{
    public ASTNode(ASTNode? parent)
    {
        Parent = parent;
    }

    public ASTNode? Parent { get; set; }

    public abstract T Accept<T>(IVisitor<T> visitor);
}
