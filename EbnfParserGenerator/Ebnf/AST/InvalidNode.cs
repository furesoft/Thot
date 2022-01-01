namespace EbnfParserGenerator.Ebnf.AST
{
    public class InvalidNode : ASTNode
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}