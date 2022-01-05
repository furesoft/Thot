namespace EbnfParserGenerator.Ebnf.AST;

public abstract class ASTNode
{
    public ASTNode? Parent { get; set; }

    public abstract T Accept<T>(IVisitor<T> visitor);
}