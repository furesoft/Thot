namespace EbnfParserGenerator.Ebnf.AST;

public class SubTypeDeclaration : ASTNode
{
    public SubTypeDeclaration(string name, List<(string name, string type)> properties, ASTNode? parent = null) : base(parent)
    {
        Name = name;
        Properties = properties;
    }

    public string Name { get; }
    public List<(string name, string type)> Properties { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return $"| {Name}({string.Join(",", ToString(Properties))})";
    }

    private IEnumerable<string> ToString(List<(string name, string type)> properties)
    {
        foreach (var prop in properties)
        {
            yield return $"{prop.name} : {prop.type}";
        }
    }
}