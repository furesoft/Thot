namespace EbnfParserGenerator.Ebnf.AST;

public class RuleNode : ASTNode
{
    public RuleNode(string name, Block body, ASTNode parent)
    {
        Name = name;
        Body = body;
        Parent = parent;
    }

    public Block Body { get; set; } = new();
    public string Name { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return $"{Name} -> {string.Join(" ", Body)}";
    }
}