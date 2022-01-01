namespace EbnfParserGenerator.Ebnf.AST
{
    public abstract class ASTNode
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}