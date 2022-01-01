namespace EbnfParserGenerator.Ebnf.AST
{
    public class TypeDeclaration : ASTNode
    {
        public TypeDeclaration(string name, Block block)
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
    }
}