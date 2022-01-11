namespace Thot.Ebnf.AST;

public class TypeDeclaration : ASTNode
{
    public TypeDeclaration(string name, Block block, ASTNode? parent = null) : base(parent)
    {
        Name = name;
        Block = block;
    }

    public Block Block { get; }
    public string Name { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return $"{Name} -> {string.Join("\n", Block)}";
    }
}
