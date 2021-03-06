using System.Text;
using Thot.Ebnf.AST;

namespace Thot.Ebnf.AST;

public class GrammarNode : ASTNode
{
    public GrammarNode(string name, string type, Block body, ASTNode? parent = null) : base(parent)
    {
        Name = name;
        Type = type;
        Body = body;
    }

    public Block Body { get; set; }
    public string Name { get; set; }
    public string Type { get; }

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
