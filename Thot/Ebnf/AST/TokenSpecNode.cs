namespace Thot.Ebnf.AST;

public class TokenSpecNode : ASTNode
{
    public TokenSpecNode(RuleNode rule, ASTNode? parent = null) : base(parent)
    {
        Rule = rule;
    }

    public RuleNode Rule { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public override string ToString()
    {
        return $"token {Rule};";
    }
}
