using Thot.Ebnf.AST;
namespace Thot.Ebnf.AST;

public class CompilationUnit : ASTNode
{
    public CompilationUnit() : base(null)
    {
    }

    public Block Body { get; set; } = new Block();
    public List<Message> Messages { get; set; } = new List<Message>();

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
