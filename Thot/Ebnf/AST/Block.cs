using Thot.Ebnf.AST;
namespace Thot.Ebnf.AST;

public class Block : ASTNode
{
    public Block(List<ASTNode> body, ASTNode? parent = null) : base(parent)
    {
        Body = body;
    }

    public Block(ASTNode? parent = null) : base(parent)
    {
        Body = new List<ASTNode>();
    }

    public List<ASTNode> Body { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return string.Join("\n", Body);
    }
}
