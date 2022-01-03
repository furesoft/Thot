using System.Text;

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

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"grammar {Name} {{");

        sb.AppendLine($"\t{Body}");

        sb.AppendLine("}");
        return sb.ToString();
    }
}