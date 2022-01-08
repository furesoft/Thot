namespace EbnfParserGenerator.Ebnf.AST.Expressions;

public class CharacterClassExpression : Expr
{
    public CharacterClassExpression(ASTNode? parent = null) : base(parent)
    {
    }

    public List<Expr> Rules { get; set; } = new();

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}