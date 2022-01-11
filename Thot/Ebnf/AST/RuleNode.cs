namespace Thot.Ebnf.AST;

public class RuleNode : ASTNode
{
    public RuleNode(Token nameToken, Block body, ASTNode? parent = null) : base(parent)
    {
        Name = nameToken.Text;
        Body = body;
        NameToken = nameToken;
    }

    public Block Body { get; set; } = new();
    public string Name { get; set; }
    public Token NameToken { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return $"{Name} -> {string.Join(" ", Body)}";
    }
}
