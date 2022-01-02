namespace EbnfParserGenerator.Ebnf.AST;

public class GrammarNode : ASTNode
{
    public GrammarNode(string name, Block body)
    {
        Name = name;
        Body = body;
    }

    public Block Body { get; set; }
    public string Name { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}