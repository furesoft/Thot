namespace EbnfParserGenerator.Ebnf.AST;

public class RuleNode : ASTNode
{
    public RuleNode(string name, List<Expr> body)
    {
        Name = name;
        Body = body;
    }

    public List<Expr> Body { get; set; } = new List<Expr>();
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