namespace EbnfParserGenerator.Ebnf.AST.Expressions;

public class LiteralNode : Expr
{
    public LiteralNode(object value, ASTNode? parent = null) : base(parent)
    {
        Value = value;
    }

    public object Value { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return $"\"{Value}\"";
    }
}